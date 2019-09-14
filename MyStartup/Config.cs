using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MyStartup
{
    public class Config
    {
        [XmlRoot("URLTimedAccess")]
        public class URLTimedAccess
        {
            [XmlAttribute("URL")]
            public string URL;
            [XmlAttribute("Domain")]
            public string Domain;
            [XmlAttribute("Interval")]
            public double Interval;
            [XmlAttribute("Last")]
            public string Last;
        }

        [XmlRoot("SettingConfig")]
        public class SettingConfig
        {
            [XmlAttribute("StartWithSystem")]
            public bool StartWithSystem;
            [XmlAttribute("BlockScreenOff")]
            public bool BlockScreenOff;
            [XmlAttribute("BlockSystemSleep")]
            public bool BlockSystemSleep;
            [XmlAttribute("NotAutoVisitTime")]
            public string NotAutoVisitTime;
            [XmlAttribute("DelayHours")]
            public double DelayHours;
            [XmlArray("URLTimedAccessList")]
            public List<URLTimedAccess> URLTimedAccessList;
        }

        public SettingConfig settingData = null;

        private static readonly object lockObject = new object();
        private static Config instance = null;
        private static readonly string ConfigFilePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.xml");

        private Config() { }

        public static Config GetInstance()
        {
            bool loadWasFailed = false;
            if (instance == null)
            {
                bool lockWasTaken = false;
                try
                {
                    System.Threading.Monitor.Enter(lockObject, ref lockWasTaken);
                    instance = new Config();
                    instance.Load();
                }
                catch (Exception ex)
                {
                    loadWasFailed = true;
                    MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK);
                }
                finally
                {
                    if (lockWasTaken) System.Threading.Monitor.Exit(lockObject);
                }
            }

            if (loadWasFailed) Application.Exit();
            return instance;
        }

        private void Load()
        {
            if (File.Exists(ConfigFilePath))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(SettingConfig));
                using (FileStream fileStream = File.OpenRead(ConfigFilePath))
                {
                    settingData =
                        xmlSerializer.Deserialize(fileStream) as SettingConfig;
                }
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigFilePath));
            }

            if (settingData == null)
            {
                settingData = new SettingConfig()
                {
                    StartWithSystem = true,
                    BlockScreenOff = true,
                    BlockSystemSleep = true,
                    NotAutoVisitTime = "",
                    DelayHours = 12,
                    URLTimedAccessList = new List<URLTimedAccess>()
                };
            }
            if (settingData.URLTimedAccessList == null)
                settingData.URLTimedAccessList = new List<URLTimedAccess>();

        }

        public void Save()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SettingConfig));
            using (FileStream fileStream = File.Create(ConfigFilePath))
            {
                xmlSerializer.Serialize(fileStream, settingData);
            }
        }
    }
}
