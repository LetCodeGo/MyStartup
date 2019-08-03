using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyStartup
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool flag = false;
            using (System.Threading.Mutex hMutex = new System.Threading.Mutex(
                true, "MyStartup_{F7B89831-2D54-4B0E-B2E0-87AE838D1D3B}", out flag))
            {
                if (flag)
                {
                    if (System.Diagnostics.Process.GetProcessesByName("Chrome").Length > 0)
                    {
                        MessageBox.Show("Chrome 已在运行，请关闭后再运行此程序！",
                            "消息", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new MainForm());
                }
                else
                {
                    MessageBox.Show("当前程序已在运行！",
                        "消息", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }
        }
    }
}
