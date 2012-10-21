using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Utilities
{
    public partial class LoadingScreen : Form, ILoadingScreen
    {
        public int Progress
        {
            get { return progressBar1.Value; }
            set { progressBar1.Value = value; }
        }
        public int Value
        {
            get { return progressBar1.Value; }
            set { progressBar1.Value = value; }
        }
        public LoadingScreen()
        {
            InitializeComponent();
        }
        public LoadingScreen(string gameName)
        {
            InitializeComponent();
            labelGameName.Text = gameName;

           // labelGameName.Visible = false;
            //labelLoadMessage.Visible = false;
        }
        public void Reset(int max)
        {
            progressBar1.Value = 0;
            progressBar1.Maximum = max;
        }
        public string Message
        {
            set
            {
                labelLoadMessage.Text = value;
                labelGameName.Refresh();
                labelLoadMessage.Refresh();
            }
        }
    }
}
