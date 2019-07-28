using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyStartup
{
    public class StartWithSystem
    {
        /// <summary>
        /// 快捷方式名称
        /// </summary>
        private static readonly string ShortCutName =
            Path.GetFileNameWithoutExtension(Application.ExecutablePath);

        /// <summary>
        /// 系统自动启动目录
        /// </summary>
        private static readonly string SystemAutoStartPath =
            Environment.GetFolderPath(Environment.SpecialFolder.Startup);

        public static void SetStartWithSystem(bool isStartWithSystem)
        {
            List<string> linkedFileList =
                Directory.GetFiles(SystemAutoStartPath, "*.lnk").ToList();
            linkedFileList.Sort();

            List<string> linkedThisApplicationList =
                linkedFileList.TakeWhile(x =>
                {
                    WshShell shell = new WshShell();
                    IWshShortcut shortCut = (IWshShortcut)shell.CreateShortcut(x);
                    return string.Equals(
                        shortCut.TargetPath,
                        Application.ExecutablePath,
                        StringComparison.OrdinalIgnoreCase);
                }).ToList();

            if (isStartWithSystem)
            {
                if (linkedThisApplicationList.Count > 1)
                {
                    for (int i = 1; i < linkedThisApplicationList.Count; i++)
                        System.IO.File.Delete(linkedThisApplicationList[i]);
                }
                else if (linkedThisApplicationList.Count == 0)
                {
                    CreateShortCut(
                        SystemAutoStartPath, ShortCutName, Application.ExecutablePath);
                }
            }
            else
            {
                linkedThisApplicationList.ForEach(x =>
                {
                    System.IO.File.Delete(x);
                });
            }
        }

        public static void CreateShortCut(
            string shortCutFolderPath, string shortCutName, string linkedTargetPath,
            string description = null, string iconLocation = null)
        {
            if (!System.IO.Directory.Exists(shortCutFolderPath))
            {
                System.IO.Directory.CreateDirectory(shortCutFolderPath);
            }

            string shortCutPath = Path.Combine(
                shortCutFolderPath, string.Format("{0}.lnk", shortCutName));
            WshShell shell = new WshShell();
            IWshShortcut shortCut =
                (IWshShortcut)shell.CreateShortcut(shortCutPath);
            shortCut.TargetPath = linkedTargetPath;
            shortCut.WorkingDirectory = Path.GetDirectoryName(linkedTargetPath);
            shortCut.WindowStyle = 1;
            shortCut.Description = description;
            shortCut.IconLocation =
                string.IsNullOrWhiteSpace(iconLocation) ?
                linkedTargetPath : iconLocation;
            shortCut.Save();
        }
    }
}
