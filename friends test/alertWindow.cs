using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace friends_test
{
    public partial class alertWindow : Form
    {
        private String message;

        public alertWindow()
        {
            InitializeComponent();
        }

        public alertWindow(String inMessage)
        {
            message = inMessage;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void alertWindow_Load(object sender, EventArgs e)
        {
            label1.Text = message;
        }
    }
}
