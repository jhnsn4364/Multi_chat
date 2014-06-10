using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SteamKit2;

namespace friends_test
{
    public partial class MessageWindow : Form
    {
        private List<SteamID> master;
        private SteamFriends steamFriends;
        private List<SteamID> indexHelper;

        public MessageWindow()
        {
            InitializeComponent();
        }

        public MessageWindow(List<SteamID> inFriends, SteamFriends inSteamFriends)
        {
            steamFriends = inSteamFriends;

            indexHelper = new List<SteamID>();
            master = new List<SteamID>();

            InitializeComponent();

        }

        public void updateName(SteamID updatedID)
        {
            if (!master.Contains(updatedID))
                master.Add(updatedID);

            if (!indexHelper.Contains(updatedID) && !updatedID.AccountType.Equals(EAccountType.Clan))
            {
                if (!((steamFriends.GetFriendPersonaState(updatedID).Equals(EPersonaState.Offline) && checkBox1.Checked) || (steamFriends.GetFriendPersonaState(updatedID).Equals(EPersonaState.Busy) && checkBox2.Checked) || (steamFriends.GetFriendPersonaState(updatedID).Equals(EPersonaState.Away) && checkBox3.Checked) || (steamFriends.GetFriendPersonaState(updatedID).Equals(EPersonaState.Snooze) && checkBox4.Checked)))
                {
                    if (steamFriends.GetFriendPersonaName(updatedID).Equals(""))
                        checkedListBox1.Items.Add(updatedID.Render());
                    else
                        checkedListBox1.Items.Add(steamFriends.GetFriendPersonaName(updatedID));
                    indexHelper.Add(updatedID);
                }
            }
            else if (indexHelper.Contains(updatedID))
            {
                if (!((steamFriends.GetFriendPersonaState(updatedID).Equals(EPersonaState.Offline) && checkBox1.Checked) || (steamFriends.GetFriendPersonaState(updatedID).Equals(EPersonaState.Busy) && checkBox2.Checked) || (steamFriends.GetFriendPersonaState(updatedID).Equals(EPersonaState.Away) && checkBox3.Checked) || (steamFriends.GetFriendPersonaState(updatedID).Equals(EPersonaState.Snooze) && checkBox4.Checked)))
                {
                    checkedListBox1.Items[indexHelper.IndexOf(updatedID)] = steamFriends.GetFriendPersonaName(updatedID);
                }
            }

            if (indexHelper.Contains(updatedID))
            {
                if (steamFriends.GetFriendPersonaState(updatedID).Equals(EPersonaState.Offline) && checkBox1.Checked)
                {
                    checkedListBox1.Items.RemoveAt(indexHelper.IndexOf(updatedID));
                    indexHelper.Remove(updatedID);
                }
                
                if (steamFriends.GetFriendPersonaState(updatedID).Equals(EPersonaState.Busy) && checkBox2.Checked)
                {
                    checkedListBox1.Items.RemoveAt(indexHelper.IndexOf(updatedID));
                    indexHelper.Remove(updatedID);
                }

                if (steamFriends.GetFriendPersonaState(updatedID).Equals(EPersonaState.Away) && checkBox3.Checked)
                {
                    checkedListBox1.Items.RemoveAt(indexHelper.IndexOf(updatedID));
                    indexHelper.Remove(updatedID);
                }

                if (steamFriends.GetFriendPersonaState(updatedID).Equals(EPersonaState.Snooze) && checkBox4.Checked)
                {
                    checkedListBox1.Items.RemoveAt(indexHelper.IndexOf(updatedID));
                    indexHelper.Remove(updatedID);
                }
            }
        }

        private void removeAllAway()
        {
            List<SteamID> temp = new List<SteamID>();
            temp.AddRange(indexHelper);

            foreach (SteamID id in temp)
            {
                if (steamFriends.GetFriendPersonaState(id).Equals(EPersonaState.Away))
                {
                    checkedListBox1.Items.RemoveAt(indexHelper.IndexOf(id));
                    indexHelper.Remove(id);
                }
            }
        }

        private void removeAllBusy()
        {
            List<SteamID> temp = new List<SteamID>();
            temp.AddRange(indexHelper);

            foreach (SteamID id in temp)
            {
                if (steamFriends.GetFriendPersonaState(id).Equals(EPersonaState.Busy))
                {
                    checkedListBox1.Items.RemoveAt(indexHelper.IndexOf(id));
                    indexHelper.Remove(id);
                }
            }
        }

        private void removeAllOffline()
        {
            List<SteamID> temp = new List<SteamID>();
            temp.AddRange(indexHelper);

            foreach (SteamID id in temp)
            {
                if (steamFriends.GetFriendPersonaState(id).Equals(EPersonaState.Offline))
                {
                    checkedListBox1.Items.RemoveAt(indexHelper.IndexOf(id));
                    indexHelper.Remove(id);
                }
            }
        }

        private void removeAllSnooze()
        {
            List<SteamID> temp = new List<SteamID>();
            temp.AddRange(indexHelper);

            foreach (SteamID id in temp)
            {
                if (steamFriends.GetFriendPersonaState(id).Equals(EPersonaState.Snooze))
                {
                    checkedListBox1.Items.RemoveAt(indexHelper.IndexOf(id));
                    indexHelper.Remove(id);
                }
            }
        }

        private void addAllAway()
        {
            foreach (SteamID id in master)
            {
                if (steamFriends.GetFriendPersonaState(id).Equals(EPersonaState.Away))
                {
                    if (steamFriends.GetFriendPersonaName(id).Equals(""))
                        checkedListBox1.Items.Add(id.Render());
                    else
                        checkedListBox1.Items.Add(steamFriends.GetFriendPersonaName(id));
                    indexHelper.Add(id);
                }
            }
        }
        private void addAllBusy()
        {
            foreach (SteamID id in master)
            {
                if (steamFriends.GetFriendPersonaState(id).Equals(EPersonaState.Busy))
                {
                    if (steamFriends.GetFriendPersonaName(id).Equals(""))
                        checkedListBox1.Items.Add(id.Render());
                    else
                        checkedListBox1.Items.Add(steamFriends.GetFriendPersonaName(id));
                    indexHelper.Add(id);
                }
            }
        }
        private void addAllSnooze()
        {
            foreach (SteamID id in master)
            {
                if (steamFriends.GetFriendPersonaState(id).Equals(EPersonaState.Snooze))
                {
                    if (steamFriends.GetFriendPersonaName(id).Equals(""))
                        checkedListBox1.Items.Add(id.Render());
                    else
                        checkedListBox1.Items.Add(steamFriends.GetFriendPersonaName(id));
                    indexHelper.Add(id);
                }
            }
        }
        private void addAllOffline()
        {
            foreach (SteamID id in master)
            {
                if (steamFriends.GetFriendPersonaState(id).Equals(EPersonaState.Offline))
                {
                    if (steamFriends.GetFriendPersonaName(id).Equals(""))
                        checkedListBox1.Items.Add(id.Render());
                    else
                        checkedListBox1.Items.Add(steamFriends.GetFriendPersonaName(id));
                    indexHelper.Add(id);
                }
            }
        }

        private List<SteamID> listOfCheckedSteamIDs()
        {
            List<SteamID> temp = new List<SteamID>();

            foreach (SteamID id in indexHelper)
            {
                if (checkedListBox1.CheckedItems.Contains(steamFriends.GetFriendPersonaName(id)))
                    temp.Add(id);
            }

            return temp;
        }

        private void MessageWindow_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            sendMessage();
        }

        public void sendMessage()
        {
            String message = String.Empty;
            List<SteamID> temp = new List<SteamID>();

            temp = listOfCheckedSteamIDs();
            message += textBox1.Text;

            foreach (var friend in temp)
            {
                if (!message.Equals(String.Empty))
                {
                    steamFriends.SendChatMessage(friend, EChatEntryType.ChatMsg, message);
                    if (checkBox5.Checked)
                        steamFriends.SendChatMessage(friend, EChatEntryType.InviteGame, message);
                }
            }

            textBox1.Text = String.Empty;
        }

        private void MessageWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(1);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                removeAllOffline();
            else
                addAllOffline();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
                removeAllBusy();
            else
                addAllBusy();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
                removeAllAway();
            else
                addAllAway();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
                removeAllSnooze();
            else
                addAllSnooze();
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                sendMessage();
            }
        }



    }
}

