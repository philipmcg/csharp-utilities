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

    public partial class ConfirmBox : Form
    {
        static ConfirmBox m_form;

        public string OKText { get { return m_acceptButton.Text; } set { m_acceptButton.Text = value; } }
        public string CancelText { get { return m_cancelButton.Text; } set { m_cancelButton.Text = value; } }

        public void Reset(string title, string message, string cancelText,string okText)
        {
            this.Text = title;
            label1.Text = message;
            CancelText = cancelText;
            OKText = okText; ;
            this.Height = 150;
        }

        public ConfirmBox()
        {
            InitializeComponent();
        }

        public static bool ShowDialog(string title, string message, string cancelText, string okText, Action<ConfirmBox> act)
        {
            if (m_form == null)
                m_form = new ConfirmBox();


            m_form.Reset(title, message,cancelText,okText);

            if (act != null)
                act(m_form);

            var result = m_form.ShowDialog();

            if (result == DialogResult.OK)
                return true;
            else
                return false;
        }

        public static bool ShowDialog(string title, string message)
        {
            return ShowDialog(title, message, "Cancel", "OK",null);
        }
    }
}
