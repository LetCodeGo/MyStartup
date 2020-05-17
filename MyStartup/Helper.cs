using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MyStartup
{
    public class Helper
    {
        /// <summary>
        /// 用记事本打开文件路径或内容
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="strContext"></param>
        public static void OpenEdit(String filePath, String strContext)
        {
            #region 启动 notepad++

            System.Diagnostics.Process ProcNotePad = null;

            List<String> programFolderList = new List<String>();
            //programFolderList.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
            //if (Environment.Is64BitOperatingSystem)
            //    programFolderList.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));
            programFolderList.Add("C:\\Program Files");
            programFolderList.Add("C:\\Program Files (x86)");

            foreach (String programFolder in programFolderList)
            {
                if (String.IsNullOrWhiteSpace(programFolder)) continue;

                String notePadPath = Path.Combine(programFolder, "Notepad++", "notepad++.exe");

                if (File.Exists(notePadPath))
                {
                    try
                    {
                        ProcNotePad = new System.Diagnostics.Process();
                        ProcNotePad.StartInfo.FileName = notePadPath;
                        ProcNotePad.StartInfo.Arguments = filePath;
                        ProcNotePad.StartInfo.UseShellExecute = true;
                        ProcNotePad.StartInfo.RedirectStandardInput = false;
                        ProcNotePad.StartInfo.RedirectStandardOutput = false;

                        ProcNotePad.Start();
                        return;
                    }
                    catch
                    {
                        ProcNotePad = null;
                    }
                }
            }
            #endregion

            if (ProcNotePad == null)
            {
                #region [ 启动记事本 ] 

                System.Diagnostics.Process Proc;

                try
                {
                    // 启动记事本 
                    Proc = new System.Diagnostics.Process();
                    Proc.StartInfo.FileName = "notepad.exe";
                    Proc.StartInfo.UseShellExecute = false;
                    Proc.StartInfo.RedirectStandardInput = true;
                    Proc.StartInfo.RedirectStandardOutput = true;

                    Proc.Start();
                }
                catch
                {
                    Proc = null;
                }

                #endregion

                #region [ 传递数据给记事本 ] 

                if (Proc != null)
                {
                    // 调用 API, 传递数据 
                    while (Proc.MainWindowHandle == IntPtr.Zero)
                    {
                        Proc.Refresh();
                    }

                    IntPtr vHandle = Win32API.FindWindowEx(Proc.MainWindowHandle, IntPtr.Zero, "Edit", null);

                    // 传递数据给记事本 
                    Win32API.SendMessage(vHandle, Win32API.WM_SETTEXT, 0, strContext);
                }
                else
                {
                    LogForm form = new LogForm(strContext);
                    form.ShowDialog();
                }

                #endregion
            }
        }

        /// <summary>
        /// 最多输出的列（最小设为 5）
        /// </summary>
        private static int MaxOutputRow = Int32.MaxValue;

        /// <summary>
        /// DataTable以文本输出
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="outputSet">要输出的字段，为 null 时输出全部</param>
        /// <returns></returns>
        public static String DataTableFormatToString(DataTable dt, HashSet<String> outputSet)
        {
            char angle = '+';
            char horizontal = '-';
            char vertical = '|';
            char star = '*';

            // 每列的最大长度
            List<int> lenList = new List<int>();
            // 每列的字符串
            List<string> strList = new List<string>();
            // 每列是否右对齐
            List<bool> padList = new List<bool>();
            // dt 实际会输出的列索引
            List<int> colIndex = new List<int>();
            // 是否会折叠输出
            bool omitOutput = dt.Rows.Count > MaxOutputRow;
            // 0-(tempRowCount-1) 正常输出，tempRowCount-(MaxOutputRow-2)输出*
            // (MaxOutputRow-2) 输出 dt 最后一行
            int tempRowCount = omitOutput ? MaxOutputRow - 4 : dt.Rows.Count;

            // 记录字符串转换为单字节与本身长度差
            List<List<int>> lll = new List<List<int>>();

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (outputSet != null && !outputSet.Contains(dt.Columns[i].ColumnName))
                    continue;

                List<int> ll = new List<int>();

                String strTemp = dt.Columns[i].ColumnName;
                int maxLen = System.Text.UTF8Encoding.Default.GetBytes(strTemp).Length;
                ll.Add(maxLen - strTemp.Length);

                for (int j = 0; j < tempRowCount; j++)
                {
                    strTemp = dt.Rows[j][i].ToString();
                    int len = System.Text.UTF8Encoding.Default.GetBytes(strTemp).Length;
                    ll.Add(len - strTemp.Length);
                    if (maxLen < len) maxLen = len;
                }
                if (omitOutput)
                {
                    // 最后一行
                    strTemp = dt.Rows[dt.Rows.Count - 1][i].ToString();
                    int len = System.Text.UTF8Encoding.Default.GetBytes(strTemp).Length;
                    ll.Add(len - strTemp.Length);
                    if (maxLen < len) maxLen = len;
                }

                lll.Add(ll);
                lenList.Add(maxLen + 2);
                strList.Add(new string(horizontal, maxLen + 2));
                padList.Add(dt.Columns[i].DataType != typeof(int));
                colIndex.Add(i);
            }

            string rstStr = string.Empty;
            string tmpStr = string.Empty;
            string spcStr = angle + string.Join(angle.ToString(), strList) + angle;

            // 列标题
            for (int i = 0; i < colIndex.Count; i++)
            {
                if (padList[i])
                    strList[i] = " " + dt.Columns[colIndex[i]].ColumnName.PadRight(
                        lenList[i] - 1 - lll[i][0]);
                else
                    strList[i] = dt.Columns[colIndex[i]].ColumnName.PadLeft(
                        lenList[i] - 1 - lll[i][0]) + " ";
                tmpStr = vertical + string.Join(vertical.ToString(), strList) + vertical;
            }
            rstStr += (spcStr + Environment.NewLine);
            rstStr += (tmpStr + Environment.NewLine);
            rstStr += (spcStr + Environment.NewLine);

            // 列数据
            for (int i = 0, nl = 1; i < tempRowCount || (omitOutput && i < MaxOutputRow);
                i++, nl++)
            {
                // 星号输出
                bool starOutput = (omitOutput && i >= tempRowCount && i < (MaxOutputRow - 1));
                // 折叠输出最后一行为 dt 最后一行
                if (omitOutput && (i == MaxOutputRow - 1))
                {
                    i = dt.Rows.Count - 1;
                    nl = lll[0].Count - 1;
                }

                for (int j = 0; j < colIndex.Count; j++)
                {
                    if (starOutput)
                    {
                        if (j == 0 && dt.Columns[colIndex[j]].ColumnName == "index")
                        {
                            strList[j] = new string(star, i == MaxOutputRow - 2 ?
                                dt.Rows[dt.Rows.Count - 1]["index"].ToString().Length :
                                dt.Rows[i]["index"].ToString().Length).PadLeft(
                                lenList[j] - 1) + " ";
                        }
                        else strList[j] = " " + new string(star, lenList[j] - 2) + " ";
                    }
                    else
                    {
                        if (padList[j])
                            strList[j] = " " + dt.Rows[i][colIndex[j]].ToString().PadRight(
                                lenList[j] - 1 - lll[j][nl]);
                        else
                            strList[j] = dt.Rows[i][colIndex[j]].ToString().PadLeft(
                                lenList[j] - 1 - lll[j][nl]) + " ";
                    }
                }
                tmpStr = vertical + string.Join(vertical.ToString(), strList) + vertical;

                rstStr += (tmpStr + Environment.NewLine);
                if (i == dt.Rows.Count - 1)
                    rstStr += (spcStr + Environment.NewLine);
            }

            return rstStr;
        }

        public static DateTime GetSystemLastShutdownTime()
        {
            //string sKey = @"System\CurrentControlSet\Control\Windows";
            //Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(sKey);

            //string sValueName = "ShutdownTime";
            //byte[] val = (byte[])key.GetValue(sValueName);
            //long valueAsLong = BitConverter.ToInt64(val, 0);
            //return DateTime.FromFileTime(valueAsLong);

            if (EventLog.Exists("System"))
            {
                var log = new EventLog("System", Environment.MachineName, "EventLog");

                var entries = new EventLogEntry[log.Entries.Count];
                log.Entries.CopyTo(entries, 0);

                var startupTimes = entries.Where(x => x.InstanceId == 2147489653).Select(x => x.TimeWritten);
                var shutdownTimes = entries.Where(x => x.InstanceId == 2147489654).Select(x => x.TimeWritten);
                if (shutdownTimes != null && shutdownTimes.Count() != 0)
                    return shutdownTimes.Max();
                else return DateTime.MinValue;
            }
            else return DateTime.MinValue;
        }
    }
}
