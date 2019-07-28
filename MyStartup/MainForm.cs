using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyStartup
{
    public partial class MainForm : Form
    {
        private bool exitFlag = false;
        private readonly string chromeHistoryFilePath =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                @"Google\Chrome\User Data\Default\History");

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Icon = Properties.Resources.Startup;

            this.dataGridView.AutoGenerateColumns = false;
            this.dataGridView.AllowUserToResizeRows = false;

            this.notifyIcon.Visible = false;
            this.notifyIcon.Icon = Properties.Resources.Startup;
            this.notifyIcon.Text = "MyStartup";
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;

            //StartWithSystem.SetStartWithSystem(this.cbStartWithSystem.Checked);

            if (this.cbBlockSystemSleep.Checked)
            {
                SystemSleepManagement.PreventSleep(this.cbBlockScreenOff.Checked);
            }

            this.WindowState = FormWindowState.Minimized;

            GetChromeHistory();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 通知栏右键退出
            if (exitFlag)
            {
                SystemSleepManagement.ResotreSleep();
            }
            else
            {
                switch (e.CloseReason)
                {
                    case CloseReason.None:
                    case CloseReason.MdiFormClosing:
                    case CloseReason.ApplicationExitCall:
                    case CloseReason.TaskManagerClosing:
                    case CloseReason.WindowsShutDown:
                    case CloseReason.FormOwnerClosing:
                        SystemSleepManagement.ResotreSleep();
                        break;
                    case CloseReason.UserClosing:
                        e.Cancel = true;
                        this.WindowState = FormWindowState.Minimized;
                        this.ShowInTaskbar = false;
                        this.notifyIcon.Visible = true;
                        break;
                    default:
                        SystemSleepManagement.ResotreSleep();
                        break;
                }
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                this.notifyIcon.Visible = true;
            }
        }

        private void CbStartWithSystem_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void CbBlockScreenOff_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void CbBlockSystemSleep_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void ButtonAdd_Click(object sender, EventArgs e)
        {

        }

        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
                this.StartPosition = FormStartPosition.CenterScreen;

                this.ShowInTaskbar = true;
                this.notifyIcon.Visible = false;
            }
        }

        private void ShowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
                this.StartPosition = FormStartPosition.CenterScreen;

                this.ShowInTaskbar = true;
                this.notifyIcon.Visible = false;
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exitFlag = true;
            this.Close();
        }

        private Dictionary<string, long> GetChromeHistory()
        {
            Dictionary<string, long> dic = new Dictionary<string, long>();

            using (SQLiteConnection conn = new SQLiteConnection(
                string.Format(@"Data Source={0};", chromeHistoryFilePath)))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(
                    "select url, last_visit_time from urls order by last_visit_time desc;", conn))
                {
                    SQLiteDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        string url = dr[0].ToString();
                        if (!dic.Keys.Contains(url))
                        {
                            dic.Add(url, dr.GetInt64(1));
                        }
                    }
                }
                conn.Close();
            }

            return dic;
        }
    }
}
