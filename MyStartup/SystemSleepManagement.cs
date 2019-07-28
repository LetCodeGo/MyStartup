using System.Runtime.InteropServices;

namespace MyStartup
{
    class SystemSleepManagement
    {
        const uint ES_SYSTEM_REQUIRED = 0x00000001;
        const uint ES_DISPLAY_REQUIRED = 0x00000002;
        const uint ES_CONTINUOUS = 0x80000000;

        //定义API函数
        [DllImport("kernel32.dll")]
        static extern uint SetThreadExecutionState(uint esFlags);

        /// <summary>
        ///阻止系统休眠，直到线程结束恢复休眠策略
        /// </summary>
        /// <param name="includeDisplay">是否阻止关闭显示器</param>
        public static void PreventSleep(bool includeDisplay = false)
        {
            if (includeDisplay)
                SetThreadExecutionState(ES_SYSTEM_REQUIRED | ES_DISPLAY_REQUIRED | ES_CONTINUOUS);
            else
                SetThreadExecutionState(ES_SYSTEM_REQUIRED | ES_CONTINUOUS);
        }

        /// <summary>
        ///恢复系统休眠策略
        /// </summary>
        public static void ResotreSleep()
        {
            SetThreadExecutionState(ES_CONTINUOUS);
        }

        /// <summary>
        ///重置系统休眠计时器
        /// </summary>
        /// <param name="includeDisplay">是否阻止关闭显示器</param>
        public static void ResetSleepTimer(bool includeDisplay = false)
        {
            if (includeDisplay)
                SetThreadExecutionState(ES_SYSTEM_REQUIRED | ES_DISPLAY_REQUIRED);
            else
                SetThreadExecutionState(ES_SYSTEM_REQUIRED);
        }
    }
}
