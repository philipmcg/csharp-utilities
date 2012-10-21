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
    public partial class ProgressForm : Form
    {
        public ProgressForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        public Label Label { get { return label1; } set { label1 = value; } }
        public ProgressBar ProgressBar { get { return progressBar1; } set { progressBar1 = value; } }
    }
}
