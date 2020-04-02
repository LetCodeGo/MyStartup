using System;
using System.Windows.Forms;

namespace MyStartup
{
    public partial class LogForm : Form
    {
        public LogForm(String log)
        {
            InitializeComponent();
            this.richTextBox.Text = log;
            this.Icon = Properties.Resources.Startup;
        }
    }
}
