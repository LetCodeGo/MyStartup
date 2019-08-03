using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyStartup
{
    public partial class MainForm : Form
    {
        private bool exitFlag = false;
        private static readonly object lockThreadObject = new object();
        private DataTable gridViewData = null;
        private DateTime? notAutoVisitTime = null;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Icon = Properties.Resources.Startup;

            this.dataGridView.AutoGenerateColumns = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.ContextMenuStrip = this.contextMenuStripForDataGridView;

            this.ColumnIndex.DataPropertyName = "index";
            this.ColumnURL.DataPropertyName = "url";
            this.ColumnDomain.DataPropertyName = "domain";
            this.ColumnInterval.DataPropertyName = "interval";
            this.ColumnLast.DataPropertyName = "last";
            this.ColumnNeed.DataPropertyName = "need";

            this.notifyIcon.Visible = false;
            this.notifyIcon.Icon = Properties.Resources.Startup;
            this.notifyIcon.Text = "MyStartup";
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;

            this.cbStartWithSystem.Checked = Config.GetInstance().settingData.StartWithSystem;
            this.cbBlockScreenOff.Checked = Config.GetInstance().settingData.BlockScreenOff;
            this.cbBlockSystemSleep.Checked = Config.GetInstance().settingData.BlockSystemSleep;
            this.textBoxDelayHour.Text = Config.GetInstance().settingData.DelayHours.ToString();

            string strTemp = Config.GetInstance().settingData.NotAutoVisitTime;
            if (!string.IsNullOrWhiteSpace(strTemp))
            {
                DateTime dateTime = DateTime.Now;
                if (DateTime.TryParse(strTemp, out dateTime) && dateTime > DateTime.Now)
                {
                    this.cbNotAutoVisit.Checked = true;
                    notAutoVisitTime = dateTime;
                    this.labelInfo.Text = string.Format(
                        "到 {0} 结束", dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                }
            }

            // 打开程序30分钟内不自动访问网址
            if (notAutoVisitTime == null || (notAutoVisitTime < DateTime.Now.AddHours(0.5)))
            {
                notAutoVisitTime = DateTime.Now.AddHours(0.5);
                this.cbNotAutoVisit.Checked = true;
                this.textBoxDelayHour.Text = "0.5";
                this.labelInfo.Text = string.Format(
                    "到 {0} 结束", notAutoVisitTime?.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            this.textBoxDelayHour.Enabled = !this.cbNotAutoVisit.Checked;
            this.cbNotAutoVisit.CheckedChanged +=
                new System.EventHandler(this.CbNotAutoVisit_CheckedChanged);

            StartWithSystem.SetStartWithSystem(this.cbStartWithSystem.Checked);

            if (this.cbBlockSystemSleep.Checked)
            {
                SystemSleepManagement.PreventSleep(this.cbBlockScreenOff.Checked);
            }

            this.cbBlockSystemSleep.CheckedChanged +=
                new System.EventHandler(this.CbBlockSystemSleep_CheckedChanged);
            this.cbBlockScreenOff.CheckedChanged +=
                new System.EventHandler(this.CbBlockScreenOff_CheckedChanged);
            this.cbStartWithSystem.CheckedChanged +=
                new System.EventHandler(this.CbStartWithSystem_CheckedChanged);

            //gridViewData = ChromeInfo.GetGridViewDataFromChromeBookMark(
            //    new List<string>() { "P1", "P2" });

            gridViewData = new DataTable();
            gridViewData.Columns.Add("index", typeof(int));
            gridViewData.Columns.Add("url", typeof(string));
            gridViewData.Columns.Add("domain", typeof(string));
            gridViewData.Columns.Add("interval", typeof(double));
            gridViewData.Columns.Add("last", typeof(string));
            gridViewData.Columns.Add("need", typeof(double));

            List<Config.URLTimedAccess> URLTimedAccessList =
                Config.GetInstance().settingData.URLTimedAccessList;
            int index = 1;
            foreach (Config.URLTimedAccess urlTimedAccess in URLTimedAccessList)
            {
                DataRow dr = gridViewData.NewRow();
                dr[0] = index++;
                dr[1] = urlTimedAccess.URL;
                dr[2] = urlTimedAccess.Domain;
                dr[3] = urlTimedAccess.Interval;
                dr[4] = urlTimedAccess.Last;
                dr[5] = "-1";
                gridViewData.Rows.Add(dr);
            }

            List<string> domainList = new List<string>();
            for (int i = 0; i < gridViewData.Rows.Count; i++)
            {
                domainList.Add(gridViewData.Rows[i][2].ToString());
            }
            Dictionary<string, DateTime> dic = ChromeInfo.GetChromeHistory(domainList);
            for (int i = 0; i < gridViewData.Rows.Count; i++)
            {
                string strDomain = gridViewData.Rows[i][2].ToString();
                DateTime lastVisit = DateTime.MinValue;
                if (dic.ContainsKey(strDomain) && 
                    DateTime.TryParse(gridViewData.Rows[i][4].ToString(), out lastVisit) &&
                    dic[strDomain] > lastVisit)
                {
                    gridViewData.Rows[i][4] = dic[strDomain].ToString("yyyy-MM-dd HH:mm:ss");
                }
            }

            UpdateNeededTime();
            this.dataGridView.DataSource = gridViewData;

            Thread thread = new Thread(new ThreadStart(ThreadAction));
            thread.Start();

            this.WindowState = FormWindowState.Minimized;
        }

        private void ThreadAction()
        {
            Func<bool> IsInNotAutoVisitFunc = IsInNotAutoVisit;
            Action UpdateDataGridViewAction = UpdateDataGridView;

            while (!exitFlag)
            {
                for (int i = 0; i < 300; i++)
                {
                    Thread.Sleep(100);
                    if (exitFlag) break;
                }
                if (exitFlag) break;

                if (Convert.ToBoolean(this.Invoke(IsInNotAutoVisitFunc))) continue;

                lock (lockThreadObject)
                {
                    List<int> indexList = null;
                    while ((indexList = UpdateNeededTime()).Count > 0)
                    { VisitURLs(indexList); }

                    this.Invoke(UpdateDataGridViewAction);
                }
            }
        }

        private bool IsInNotAutoVisit()
        {
            bool rst = false;

            this.cbNotAutoVisit.Enabled = false;
            if (notAutoVisitTime != null)
            {
                if (DateTime.Now < notAutoVisitTime) rst = true;
                else
                {
                    notAutoVisitTime = null;
                    this.labelInfo.Text = "到 0000-00-00 00:00:00 结束";
                    this.cbNotAutoVisit.CheckedChanged -=
                        new System.EventHandler(this.CbNotAutoVisit_CheckedChanged);
                    this.cbNotAutoVisit.Checked = false;
                    this.textBoxDelayHour.Enabled = true;
                    this.cbNotAutoVisit.CheckedChanged +=
                        new System.EventHandler(this.CbNotAutoVisit_CheckedChanged);
                }
            }
            this.cbNotAutoVisit.Enabled = true;

            return rst;
        }

        private void UpdateDataGridView()
        {
            this.dataGridView.DataSource = gridViewData;
            this.dataGridView.Invalidate();
        }

        private List<int> UpdateNeededTime()
        {
            List<int> indexList = new List<int>();
            for (int i = 0; i < gridViewData.Rows.Count; i++)
            {
                double setDays = Convert.ToDouble(gridViewData.Rows[i][3]);

                if (setDays <= 0)
                {
                    gridViewData.Rows[i][5] = "-1";
                }
                else
                {
                    long needTime = Convert.ToInt64(
                        (DateTime.Parse(gridViewData.Rows[i][4].ToString()) - DateTime.Now).TotalSeconds + setDays * 86400);
                    if (needTime <= 0)
                    {
                        gridViewData.Rows[i][5] = "0";
                        indexList.Add(i);
                    }
                    else
                    {
                        gridViewData.Rows[i][5] =
                            string.Format("{0:F}", (double)needTime / 3600);
                    }
                }
            }
            return indexList;
        }

        private void VisitURLs(List<int> indexList)
        {
            foreach (int index in indexList)
            {
                System.Diagnostics.Process.Start(gridViewData.Rows[index][1].ToString());
                gridViewData.Rows[index][4] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 通知栏右键退出
            if (exitFlag)
            {
                SaveConfig();
                SystemSleepManagement.ResotreSleep();
            }
            else
            {
                switch (e.CloseReason)
                {
                    case CloseReason.WindowsShutDown:
                        exitFlag = true;
                        SaveConfig();
                        SystemSleepManagement.ResotreSleep();
                        break;
                    case CloseReason.None:
                    case CloseReason.MdiFormClosing:
                    case CloseReason.ApplicationExitCall:
                    case CloseReason.TaskManagerClosing:
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

        private void SaveConfig()
        {
            Config.SettingConfig config = Config.GetInstance().settingData;
            config.StartWithSystem = this.cbStartWithSystem.Checked;
            config.BlockScreenOff = this.cbBlockScreenOff.Checked;
            config.BlockSystemSleep = this.cbBlockSystemSleep.Checked;
            if (notAutoVisitTime == null) config.NotAutoVisitTime = "";
            else config.NotAutoVisitTime = notAutoVisitTime?.ToString("yyyy-MM-dd HH:mm:ss");
            config.DelayHours = Convert.ToDouble(this.textBoxDelayHour.Text);

            config.URLTimedAccessList.Clear();
            for (int i = 0; i < gridViewData.Rows.Count; i++)
            {
                config.URLTimedAccessList.Add(new Config.URLTimedAccess()
                {
                    URL = gridViewData.Rows[i][1].ToString(),
                    Domain = gridViewData.Rows[i][2].ToString(),
                    Interval = Convert.ToDouble(gridViewData.Rows[i][3]),
                    Last = gridViewData.Rows[i][4].ToString()
                });
            }

            Config.GetInstance().Save();
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
            StartWithSystem.SetStartWithSystem(this.cbStartWithSystem.Checked);
        }

        private void CbBlockScreenOff_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbBlockScreenOff.Checked)
            {
                this.cbBlockSystemSleep.CheckedChanged -=
                    new System.EventHandler(this.CbBlockSystemSleep_CheckedChanged);
                this.cbBlockSystemSleep.Checked = true;
                this.cbBlockSystemSleep.CheckedChanged +=
                    new System.EventHandler(this.CbBlockSystemSleep_CheckedChanged);

                SystemSleepManagement.PreventSleep(true);
            }
            else
            {
                if (this.cbBlockSystemSleep.Checked)
                {
                    SystemSleepManagement.PreventSleep(false);
                }
            }
        }

        private void CbBlockSystemSleep_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbBlockSystemSleep.Checked)
            {
                SystemSleepManagement.PreventSleep(this.cbBlockScreenOff.Checked);
            }
            else
            {
                this.cbBlockScreenOff.CheckedChanged -=
                    new System.EventHandler(this.CbBlockScreenOff_CheckedChanged);
                this.cbBlockScreenOff.Checked = false;
                this.cbBlockScreenOff.CheckedChanged +=
                    new System.EventHandler(this.CbBlockScreenOff_CheckedChanged);

                SystemSleepManagement.ResotreSleep();
            }
        }

        private void CbNotAutoVisit_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbNotAutoVisit.Checked)
            {
                this.textBoxDelayHour.Enabled = false;
                notAutoVisitTime =
                    DateTime.Now.AddHours(Convert.ToDouble(this.textBoxDelayHour.Text));
                this.labelInfo.Text = string.Format(
                    "到 {0} 结束", notAutoVisitTime?.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            else
            {
                this.textBoxDelayHour.Enabled = true;
                notAutoVisitTime = null;
                this.labelInfo.Text = "到 0000-00-00 00:00:00 结束";
            }
        }

        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            string url = this.textBoxURL.Text;
            string domian = this.textBoxDomain.Text;
            double interval = -1;
            if (string.IsNullOrWhiteSpace(url) ||
                string.IsNullOrWhiteSpace(domian) ||
                string.IsNullOrWhiteSpace(this.textBoxInterval.Text) ||
                (!double.TryParse(this.textBoxInterval.Text, out interval)))
            {
                MessageBox.Show("输入字符不合法");
                return;
            }

            for (int i = 0; i < gridViewData.Rows.Count; i++)
            {
                if (url == gridViewData.Rows[i][1].ToString() ||
                    domian == gridViewData.Rows[i][2].ToString())
                {
                    MessageBox.Show("输入网址或域名重复");
                    return;
                }
            }

            lock (lockThreadObject)
            {
                int maxIndex = 0;
                for (int i = 0; i < gridViewData.Rows.Count; i++)
                {
                    int index = Convert.ToInt32(gridViewData.Rows[i][0]);
                    if (maxIndex < index) maxIndex = index;
                }
                maxIndex++;

                DataRow dr = gridViewData.NewRow();
                dr[0] = maxIndex;
                dr[1] = this.textBoxURL.Text;
                dr[2] = this.textBoxDomain.Text;
                dr[3] = interval;
                dr[4] = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
                dr[5] = -1;
                gridViewData.Rows.Add(dr);

                List<int> indexList = null;
                while ((indexList = UpdateNeededTime()).Count > 0)
                { VisitURLs(indexList); }

                UpdateDataGridView();
            }
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

        private void ContextMenuStripForDataGridView_Opening(object sender, CancelEventArgs e)
        {
            bool show = false;
            if (this.dataGridView.SelectedRows.Count > 0)
            {
                show = true;
            }

            this.toolStripMenuItemVisit.Enabled = show;
            this.toolStripMenuItemDelete.Enabled = show;
            this.toolStripMenuItemModifyInterval.Enabled = show;
        }

        private void ToolStripMenuItemVisit_Click(object sender, EventArgs e)
        {
            List<int> indexList = new List<int>();
            for (int i = 0; i < this.dataGridView.SelectedRows.Count; i++)
            {
                int index = this.dataGridView.SelectedRows[i].Index;
                indexList.Add(Convert.ToInt32(this.dataGridView.Rows[index].Cells[0].Value));
            }

            List<int> visitList = new List<int>();
            for (int i = 0; i < gridViewData.Rows.Count; i++)
            {
                if (indexList.Contains(Convert.ToInt32(gridViewData.Rows[i][0])))
                {
                    visitList.Add(i);
                }
            }

            lock (lockThreadObject)
            {
                do { VisitURLs(visitList); }
                while ((visitList = UpdateNeededTime()).Count > 0);

                UpdateDataGridView();
            }
        }

        private void ToolStripMenuItemDelete_Click(object sender, EventArgs e)
        {
            List<int> indexList = new List<int>();
            for (int i = 0; i < this.dataGridView.SelectedRows.Count; i++)
            {
                int index = this.dataGridView.SelectedRows[i].Index;
                indexList.Add(Convert.ToInt32(this.dataGridView.Rows[index].Cells[0].Value));
            }

            List<int> deleteList = new List<int>();
            for (int i = gridViewData.Rows.Count - 1; i >= 0; i--)
            {
                if (indexList.Contains(Convert.ToInt32(gridViewData.Rows[i][0])))
                {
                    deleteList.Add(i);
                }
            }

            lock (lockThreadObject)
            {
                foreach (int index in deleteList)
                {
                    gridViewData.Rows.RemoveAt(index);
                }

                UpdateDataGridView();
            }
        }

        private void ToolStripMenuItemModifyInterval_Click(object sender, EventArgs e)
        {
            ModifyIntervalForm form = new ModifyIntervalForm();
            form.ModifyIntervalAction += this.ModifyInterval;
            form.ShowDialog();
        }

        private void ModifyInterval(double interval)
        {
            List<int> indexList = new List<int>();
            for (int i = 0; i < this.dataGridView.SelectedRows.Count; i++)
            {
                int index = this.dataGridView.SelectedRows[i].Index;
                indexList.Add(Convert.ToInt32(this.dataGridView.Rows[index].Cells[0].Value));
            }

            List<int> updateList = new List<int>();
            for (int i = 0; i < gridViewData.Rows.Count; i++)
            {
                if (indexList.Contains(Convert.ToInt32(gridViewData.Rows[i][0])))
                {
                    updateList.Add(i);
                }
            }

            lock (lockThreadObject)
            {
                foreach (int index in updateList)
                {
                    gridViewData.Rows[index][3] = interval;
                }

                while ((updateList = UpdateNeededTime()).Count > 0)
                { VisitURLs(updateList); }

                UpdateDataGridView();
            }
        }
    }
}
