using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

using SteamKit2;


namespace friends_test
{
    class Program
    {
        static SteamClient steamClient;
        static CallbackManager manager;
        static Login loginWindow;
        static MessageWindow messageManager;
        static List<SteamID> friendList = new List<SteamID>();
        static string authCode;

        static SteamUser steamUser;
        static SteamFriends steamFriends;

        static bool isRunning;

        static string user, pass;


        static void Main(string[] args)
        {
            isRunning = true;

            loginWindow = new Login();
            loginWindow.ShowDialog();

            if (loginWindow.DialogResult== DialogResult.OK)
            {
                if (!loginWindow.user.Equals(String.Empty) && !loginWindow.pass.Equals(String.Empty))
                {
                    user = loginWindow.user;
                    pass = loginWindow.pass;
                }
                else
                {
                    isRunning = false;
                    Console.Out.WriteLine("Please use a username and password next time.");
                    alert("Exiting, please fill in both fields");
                }
            }
            else
            {
                isRunning = false;
            }



            // create our steamclient instance
            steamClient = new SteamClient(System.Net.Sockets.ProtocolType.Tcp);
            // create the callback manager which will route callbacks to function calls
            manager = new CallbackManager(steamClient);

            // get the steamuser handler, which is used for logging on after successfully connecting
            steamUser = steamClient.GetHandler<SteamUser>();
            // get the steam friends handler, which is used for interacting with friends on the network after logging on
            steamFriends = steamClient.GetHandler<SteamFriends>();


            //Register connection callbacks
            new Callback<SteamClient.ConnectedCallback>(OnConnected, manager);
            new Callback<SteamClient.DisconnectedCallback>(OnDisconnected, manager);

            new Callback<SteamUser.LoggedOnCallback>(OnLoggedOn, manager);
            new Callback<SteamUser.LoggedOffCallback>(OnLoggedOff, manager);

            // Register Friends callbacks
            new Callback<SteamUser.AccountInfoCallback>(OnAccountInfo, manager);
            new Callback<SteamFriends.FriendsListCallback>(OnFriendsList, manager);
            new Callback<SteamFriends.PersonaStateCallback>(OnPersonaState, manager);
            new Callback<SteamFriends.FriendAddedCallback>(OnFriendAdded, manager);

            new JobCallback<SteamUser.UpdateMachineAuthCallback>(OnMachineAuth, manager);

            Console.WriteLine("Connecting to Steam...");

            // initiate the connection
            steamClient.Connect();

            ShowForm();
            //messageManager = new MessageWindow(friendList, steamFriends);//, steamClient);

            //messageManager.Show();
            //messageManager.Refresh();
            // create our callback handling loop
            while (isRunning)
            {
                // in order for the callbacks to get routed, they need to be handled by the manager
                manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
            }
            return;
        }

        static void ShowForm()
        {
            if (messageManager != null)
                messageManager.BeginInvoke(new MethodInvoker(delegate { messageManager.Dispose(); }));

            ThreadPool.QueueUserWorkItem(delegate(object state)
            {
                Application.Run(messageManager = new MessageWindow(friendList, steamFriends));
            });
        }

        static void update(SteamID idToUpdate)
        {
            messageManager.Invoke(new MethodInvoker(delegate { messageManager.updateName(idToUpdate); }));
        }

        static void OnConnected(SteamClient.ConnectedCallback callback)
        {
            if (callback.Result != EResult.OK)
            {
                Console.WriteLine("Unable to connect to Steam: {0}", callback.Result);
                alert("Unable to connect to Steam: "+callback.Result);
                isRunning = false;
                return;
            }
            Console.WriteLine("Connected to Steam! Logging in '{0}'...", user);

            byte[] sentryHash = null;
            if (File.Exists("sentry.bin"))
            {
                byte[] sentryFile = File.ReadAllBytes("sentry.bin");
                sentryHash = CryptoHelper.SHAHash(sentryFile);
            }

            steamUser.LogOn(new SteamUser.LogOnDetails
            {
                Username = user,
                Password = pass,

                AuthCode = authCode,

                SentryFileHash = sentryHash,
            });
        }

        static void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            Console.WriteLine("Disconnected from Steam");

            Thread.Sleep(TimeSpan.FromSeconds(5));

            steamClient.Connect();
        }

        static void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            if (callback.Result != EResult.OK)
            {
                if (callback.Result == EResult.AccountLogonDenied)
                {

                    Console.WriteLine("Unable to logon to Steam: This account is SteamGuard protected.");

                    AuthForm authDialog = new AuthForm();
                    authDialog.ShowDialog();

                    if (authDialog.DialogResult == DialogResult.OK)
                    {
                        authCode = authDialog.AuthValue;
                    }
                    else
                    {
                        isRunning = false;
                    }
                    return;
                }

                Console.WriteLine("Unable to logon to Steam: {0} / {1}", callback.Result, callback.ExtendedResult);
                alert("Unable to logon to Steam: "+ callback.Result+" / "+callback.ExtendedResult);
                isRunning = false;
                return;
            }

            Console.WriteLine("Successfully logged on!");

        }

        static void OnAccountInfo(SteamUser.AccountInfoCallback callback)
        {
            steamFriends.SetPersonaState(EPersonaState.Online);
        }

        static void OnFriendsList(SteamFriends.FriendsListCallback callback)
        {

            int friendCount = steamFriends.GetFriendCount();

            Console.WriteLine("We have {0} friends", friendCount);

            foreach (var friend in callback.FriendList)
            {
                friendList.Add(friend.SteamID);
            }
        }

        static void OnFriendAdded(SteamFriends.FriendAddedCallback callback)
        {
            friendList.Add(callback.SteamID);
            Console.WriteLine("{0} is now a friend", callback.PersonaName);
        }

        static void OnPersonaState(SteamFriends.PersonaStateCallback callback)
        {
            update(callback.FriendID);
        }

        static void OnLoggedOff(SteamUser.LoggedOffCallback callback)
        {
            Console.WriteLine("Logged off of Steam: {0}", callback.Result);
        }

        static void alert(String message)
        {
            alertWindow announce = new alertWindow(message);
            announce.ShowDialog();
        }

        static void OnMachineAuth(SteamUser.UpdateMachineAuthCallback callback, JobID jobId)
        {
            Console.WriteLine("Updating sentryfile...");

            byte[] sentryHash = CryptoHelper.SHAHash(callback.Data);

            File.WriteAllBytes("sentry.bin", callback.Data);

            steamUser.SendMachineAuthResponse(new SteamUser.MachineAuthDetails
            {
                JobID = jobId,

                FileName = callback.FileName,

                BytesWritten = callback.BytesToWrite,
                FileSize = callback.Data.Length,
                Offset = callback.Offset,

                Result = EResult.OK,
                LastError = 0,

                OneTimePassword = callback.OneTimePassword,

                SentryFileHash = sentryHash,
            });

            Console.WriteLine("Done!");
        }
    }
}

