﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace MyStartup
{
    public partial class MainForm : Form
    {
        private bool exitFlag = false;
        private static readonly object lockThreadObject = new object();
        private DataTable gridViewData = null;
        private DateTime? notAutoVisitTime = null;

        bool RunningFullScreenApp = false;
        private IntPtr desktopHandle;
        private IntPtr shellHandle;
        int uCallBackMsg;

        private static int WM_QUERYENDSESSION = 0x11;
        private bool systemShutdown = false;
        private int startStopTimeId = -1;
        private DateTime stopDateTime = DateTime.MinValue;
        private Sqlite sqlite = null;

        public static String MyStartupApplicationDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyStartup");

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

            if (!Debugger.IsAttached)
            {
                StartWithSystem.SetStartWithSystem(this.cbStartWithSystem.Checked);
            }

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
                dr[5] = 0;
                gridViewData.Rows.Add(dr);
            }

            lock (lockThreadObject)
            {
                UpdateByChromeHistory(false);
                UpdateNeededTime();
            }
            this.dataGridView.DataSource = gridViewData;

            Thread thread = new Thread(new ThreadStart(ThreadAction));
            thread.Start();

            RegisterAppBar(false);

            this.sqlite = new Sqlite(Sqlite.SqliteDefaultDateBasePath);
            this.sqlite.CreateStartStopTimeTable();

            //DateTime lastShutdownTime = Helper.GetSystemLastShutdownTime();
            //if (lastShutdownTime != DateTime.MinValue)
            //{
            //    DataTable dt = this.sqlite.GetLastRowData();
            //    if (dt != null && dt.Rows.Count == 1 &&
            //        Convert.ToBoolean(dt.Rows[0]["complete"]) &&
            //        (!Convert.ToBoolean(dt.Rows[0]["manual_exit"])))
            //    {
            //        this.sqlite.UpdateStopTime(Convert.ToInt32(dt.Rows[0]["id"]),
            //            lastShutdownTime, false);
            //    }
            //}

            this.startStopTimeId = sqlite.InsertStartTime(DateTime.Now);

            if (!Directory.Exists(MyStartupApplicationDataFolder))
                Directory.CreateDirectory(MyStartupApplicationDataFolder);

            this.WindowState = FormWindowState.Minimized;
        }

        private void RegisterAppBar(bool registered)
        {
            APPBARDATA abd = new APPBARDATA();
            abd.cbSize = Marshal.SizeOf(abd);
            abd.hWnd = this.Handle;

            desktopHandle = APIWrapper.GetDesktopWindow();
            shellHandle = APIWrapper.GetShellWindow();
            if (!registered)
            {
                //register
                uCallBackMsg = APIWrapper.RegisterWindowMessage("APPBARMSG_MYSTARTUP");
                abd.uCallbackMessage = uCallBackMsg;
                uint ret = APIWrapper.SHAppBarMessage((int)ABMsg.ABM_NEW, ref abd);
            }
            else
            {
                APIWrapper.SHAppBarMessage((int)ABMsg.ABM_REMOVE, ref abd);
            }
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == uCallBackMsg)
            {
                switch (m.WParam.ToInt32())
                {
                    case (int)ABNotify.ABN_FULLSCREENAPP:
                        {
                            IntPtr hWnd = APIWrapper.GetForegroundWindow();
                            //判断当前全屏的应用是否是桌面
                            if (hWnd.Equals(desktopHandle) || hWnd.Equals(shellHandle))
                            {
                                RunningFullScreenApp = false;
                                break;
                            }
                            //判断是否全屏
                            if ((int)m.LParam == 1)
                                this.RunningFullScreenApp = true;
                            else
                                this.RunningFullScreenApp = false;
                            break;
                        }
                    default:
                        break;
                }
            }
            else if (m.Msg == WM_QUERYENDSESSION)
            {
                stopDateTime = DateTime.Now;
                exitFlag = true;
                systemShutdown = true;
            }

            base.WndProc(ref m);
        }

        private void UpdateByChromeHistory(bool isPopUp)
        {
            if (System.Diagnostics.Process.GetProcessesByName("Chrome").Length > 0)
            {
                if (isPopUp)
                {
                    MessageBox.Show("Chrome 已在运行，请关闭后再执行此操作！",
                        "消息", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                return;
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

                    double setDays = Convert.ToDouble(gridViewData.Rows[i][3]);
                    long needTime = Convert.ToInt64((dic[strDomain] - DateTime.Now).TotalSeconds + setDays * 86400);
                    gridViewData.Rows[i][5] = string.Format("{0:F}", (double)needTime / 3600);
                }
            }
        }

        private void ThreadAction()
        {
            while (!exitFlag)
            {
                for (int i = 0; i < 300; i++)
                {
                    Thread.Sleep(200);
                    if (exitFlag) break;
                }
                if (exitFlag) break;

                if (this.RunningFullScreenApp || IsInNotAutoVisit()) continue;

                lock (lockThreadObject)
                {
                    List<int> indexList = null;
                    if ((indexList = UpdateNeededTime()).Count > 0)
                    { VisitURLs(indexList, false); }

                    UpdateDataGridView();
                }
            }
        }

        private bool IsInNotAutoVisit()
        {
            if (this.InvokeRequired)
            {
                Func<bool> IsInNotAutoVisitFunc = IsInNotAutoVisit;
                return Convert.ToBoolean(this.Invoke(IsInNotAutoVisitFunc));
            }
            else
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
        }

        private void UpdateDataGridView()
        {
            if (this.dataGridView.InvokeRequired)
            {
                this.dataGridView.Invoke(new Action(() => { UpdateDataGridView(); }));
            }
            else
            {
                this.dataGridView.DataSource = gridViewData;
                this.dataGridView.Invalidate();
            }
        }

        private List<int> UpdateNeededTime()
        {
            List<int> indexList = new List<int>();
            for (int i = 0; i < gridViewData.Rows.Count; i++)
            {
                double setDays = Convert.ToDouble(gridViewData.Rows[i][3]);

                long needTime = Convert.ToInt64((DateTime.Parse(
                    gridViewData.Rows[i][4].ToString()) - DateTime.Now).TotalSeconds + setDays * 86400);
                gridViewData.Rows[i][5] = string.Format("{0:F}", (double)needTime / 3600);
                if (setDays > 0 && needTime <= 0) indexList.Add(i);
            }
            return indexList;
        }

        private void VisitURLs(List<int> indexList, bool isIngoreChromeRunning)
        {
            bool isChromeRunning =
                (System.Diagnostics.Process.GetProcessesByName("Chrome").Length > 0);

            foreach (int index in indexList)
            {
                bool isTooLong =
                    ((DateTime.Now - DateTime.Parse(gridViewData.Rows[index][4].ToString())).TotalSeconds >=
                    Convert.ToDouble(gridViewData.Rows[index][3]) * 86400 * 2);

                if (isTooLong || isIngoreChromeRunning || isChromeRunning)
                {
                    System.Diagnostics.Process.Start(gridViewData.Rows[index][1].ToString());
                    gridViewData.Rows[index][4] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    double setDays = Convert.ToDouble(gridViewData.Rows[index][3]);
                    long needTime = Convert.ToInt64((DateTime.Parse(
                        gridViewData.Rows[index][4].ToString()) - DateTime.Now).TotalSeconds + setDays * 86400);
                    gridViewData.Rows[index][5] = string.Format("{0:F}", (double)needTime / 3600);
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 通知栏右键退出
            if (exitFlag)
            {
                SaveConfig();
                if (stopDateTime == DateTime.MinValue) stopDateTime = DateTime.Now;
                this.sqlite.UpdateStopTime(this.startStopTimeId, stopDateTime, !systemShutdown);
                SystemSleepManagement.ResotreSleep();
                RegisterAppBar(true);
            }
            else
            {
                switch (e.CloseReason)
                {
                    case CloseReason.UserClosing:
                        e.Cancel = true;
                        this.WindowState = FormWindowState.Minimized;
                        this.ShowInTaskbar = false;
                        this.notifyIcon.Visible = true;
                        break;
                    default:
                        exitFlag = true;
                        SaveConfig();
                        if (stopDateTime == DateTime.MinValue) stopDateTime = DateTime.Now;
                        this.sqlite.UpdateStopTime(this.startStopTimeId, stopDateTime, !systemShutdown);
                        SystemSleepManagement.ResotreSleep();
                        RegisterAppBar(true);
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
            string url = this.textBoxURL.Text.Trim();
            double interval = -1;
            if (string.IsNullOrWhiteSpace(url) ||
                string.IsNullOrWhiteSpace(this.textBoxInterval.Text) ||
                (!double.TryParse(this.textBoxInterval.Text, out interval)))
            {
                MessageBox.Show("输入字符不合法");
                return;
            }

            if (interval <= 0)
            {
                MessageBox.Show("时间间隔数应为一个大于0的数");
                return;
            }

            string domian = GetURLDomain(url);

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
                dr[1] = url;
                dr[2] = domian;
                dr[3] = interval;
                dr[4] = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
                dr[5] = 0;
                gridViewData.Rows.Add(dr);

                List<int> indexList = null;
                if ((indexList = UpdateNeededTime()).Count > 0)
                { VisitURLs(indexList, true); }

                UpdateDataGridView();
            }
        }

        private string GetURLDomain(string URL)
        {
            int i1 = URL.IndexOf("://");
            int i2 = URL.IndexOf('/', i1 + 3);
            string domainName = URL.Substring(i1 + 3);
            if (i2 != -1)
            {
                domainName = URL.Substring(i1 + 3, i2 - i1 - 3);
            }
            int i3 = domainName.IndexOf('@');
            if (i3 != -1)
            {
                domainName = domainName.Substring(i3 + 1);
            }

            return domainName;
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
                VisitURLs(visitList, true);

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

                if ((updateList = UpdateNeededTime()).Count > 0)
                { VisitURLs(updateList, true); }

                UpdateDataGridView();
            }
        }

        private void ButtonUpdateLastVisitByChromeHistory_Click(object sender, EventArgs e)
        {
            lock (lockThreadObject)
            {
                UpdateByChromeHistory(true);
                UpdateNeededTime();
            }
            UpdateDataGridView();
        }

        private void btnOutputAllStartAndStopTime_Click(object sender, EventArgs e)
        {
            DataTable dt1 = this.sqlite.GetStartStopTimeTableAllData();
            DataTable dt2 = new DataTable();
            List<int> dateTimeColumnIndexList = new List<int>();
            foreach (DataColumn dc1 in dt1.Columns)
            {
                if (dc1.DataType == typeof(DateTime))
                {
                    dt2.Columns.Add(dc1.ColumnName, typeof(string));
                    dateTimeColumnIndexList.Add(dc1.Ordinal);
                }
                else dt2.Columns.Add(dc1.ColumnName, dc1.DataType);
            }
            foreach (DataRow dr1 in dt1.Rows)
            {
                DataRow dr2 = dt2.NewRow();
                for (int i = 0; i < dt1.Columns.Count; i++)
                {
                    if (dateTimeColumnIndexList.Contains(i))
                        dr2[i] = ((DateTime)dr1[i]).ToString("yyyy-MM-dd hh:mm:ss");
                    else dr2[i] = dr1[i];
                }
                dt2.Rows.Add(dr2);
            }
            string dtString = Helper.DataTableFormatToString(dt2, null);

            String filePath = Path.Combine(MyStartupApplicationDataFolder, "MyStartupTemp.txt");
            File.WriteAllText(filePath, dtString, System.Text.Encoding.UTF8);

            Helper.OpenEdit(filePath, dtString);
        }
    }
}
