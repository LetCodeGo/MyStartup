using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStartup
{
    public class ChromeInfo
    {
        private static readonly string chromeHistoryFilePath =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                @"Google\Chrome\User Data\Default\History");
        private static readonly string chromeBookmarksFilePath =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                @"Google\Chrome\User Data\Default\Bookmarks");

        public class GoogleChrome_bookMark_meta_info
        {
            public string last_visited_desktop { get; set; }
        }

        public class GoogleChrome_BookMark_children
        {
            public string date_added { get; set; }
            public string id { get; set; }
            public GoogleChrome_bookMark_meta_info meta_info { get; set; }
            public string name { get; set; }
            public string type { get; set; }
            public string url { get; set; }
            public List<GoogleChrome_BookMark_children> children { get; set; }

        }

        public class GoogleChrome_BookMark_bookmark_bar_other_synced
        {
            public string date_added { get; set; }
            public string date_modified { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public string type { get; set; }
            public List<GoogleChrome_BookMark_children> children { get; set; }
        }

        public class GoogleChrome_BookMark_roots
        {
            public GoogleChrome_BookMark_bookmark_bar_other_synced bookmark_bar { get; set; }
            public GoogleChrome_BookMark_bookmark_bar_other_synced other { get; set; }
            public GoogleChrome_BookMark_bookmark_bar_other_synced synced { get; set; }
        }

        public class GoogleChrome_BookMarkAllModel
        {
            public string checksum { get; set; }
            public GoogleChrome_BookMark_roots roots { get; set; }
            public string version { get; set; }
        }

        public static DataTable GetGridViewDataFromChromeBookMark(List<string> folderNameList)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("index", typeof(int));
            dt.Columns.Add("url", typeof(string));
            dt.Columns.Add("domain", typeof(string));
            dt.Columns.Add("interval", typeof(double));
            dt.Columns.Add("last", typeof(string));
            dt.Columns.Add("need", typeof(double));

            List<string> urlNameList = new List<string>();

            using (StreamReader sr = new StreamReader(chromeBookmarksFilePath))
            {
                string str = sr.ReadToEnd();
                var obj = JsonConvert.DeserializeObject<GoogleChrome_BookMarkAllModel>(str);

                foreach (var ch in obj.roots.bookmark_bar.children)
                {
                    if (folderNameList.Contains(ch.name))
                    {
                        GetAllNameInChromeBookMarkChildren(ch, ref urlNameList);
                    }
                }
            }

            int i = 1;
            foreach (string urlName in urlNameList)
            {
                int i1 = urlName.IndexOf("://");
                int i2 = urlName.IndexOf('/', i1 + 3);
                string domainName = urlName.Substring(i1 + 3);
                if (i2 != -1)
                {
                    domainName = urlName.Substring(i1 + 3, i2 - i1 - 3);
                }
                int i3 = domainName.IndexOf('@');
                if (i3 != -1)
                {
                    domainName = domainName.Substring(i3 + 1);
                }

                DataRow dr = dt.NewRow();
                dr[0] = i++;
                dr[1] = urlName;
                dr[2] = domainName;
                dr[3] = 7;
                dr[4] = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
                dr[5] = -1;
                dt.Rows.Add(dr);
            }

            return dt;
        }

        private static void GetAllNameInChromeBookMarkChildren(
            GoogleChrome_BookMark_children children, ref List<string> nameList)
        {
            foreach (var ch in children.children)
            {
                if (ch.type == "folder") GetAllNameInChromeBookMarkChildren(ch, ref nameList);
                else if (ch.type == "url") nameList.Add(ch.url);
            }
        }

        public static Dictionary<string, DateTime> GetChromeHistory(
            List<string> domainNameList = null)
        {
            Dictionary<string, DateTime> dic = new Dictionary<string, DateTime>();

            using (SQLiteConnection conn = new SQLiteConnection(
                string.Format(@"Data Source={0};", chromeHistoryFilePath)))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(
                    "select url, last_visit_time from urls where last_visit_time > 0 order by last_visit_time desc;", conn))
                {
                    SQLiteDataReader dr = cmd.ExecuteReader();
                    if (domainNameList != null)
                    {
                        if (domainNameList.Count > 0)
                        {
                            DateTime st = new DateTime(1601, 1, 1).AddHours(8);
                            while (dr.Read())
                            {
                                // scheme://username:password@example.com:123/*
                                string url = dr[0].ToString();
                                int i1 = url.IndexOf("://");
                                int i2 = url.IndexOf('/', i1 + 3);
                                string domainName = url.Substring(i1 + 3);
                                string temp = "";
                                if (i2 != -1)
                                {
                                    domainName = url.Substring(i1 + 3, i2 - i1 - 3);
                                    temp = url.Substring(i2 + 1).ToLower();
                                }
                                int i3 = domainName.IndexOf('@');
                                if (i3 != -1)
                                {
                                    domainName = domainName.Substring(i3 + 1);
                                }

                                if ((!dic.Keys.Contains(domainName)) &&
                                    domainNameList.Contains(domainName) &&
                                    (!string.IsNullOrWhiteSpace(temp)) &&
                                    (!temp.StartsWith("login")))
                                {
                                    dic.Add(domainName, st.AddTicks(10 * dr.GetInt64(1)));
                                    if (dic.Count == domainNameList.Count) break;
                                }
                            }
                        }
                    }
                    else
                    {
                        while (dr.Read())
                        {
                            dic.Add(dr[0].ToString(), new DateTime(dr.GetInt64(1)));
                        }
                    }
                }
                conn.Close();
            }

            return dic;
        }
    }
}
