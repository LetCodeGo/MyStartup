using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace MyStartup
{
    public class Sqlite
    {
        public static readonly String SqliteDefaultDateBasePath = Path.Combine(
            System.Windows.Forms.Application.StartupPath, "MyStartup.db");

        private readonly String SqliteDataBasePath = String.Empty;

        private readonly String SQLOpenCmdText = String.Empty;

        public Sqlite(String sqliteDataBasePath)
        {
            this.SqliteDataBasePath = sqliteDataBasePath;
            this.SQLOpenCmdText = String.Format("data source = {0};", sqliteDataBasePath);
        }

        public void CreateStartStopTimeTable()
        {
            String cmdText = String.Format(@"create table if not exists {0} ( ", "start_stop_time");
            cmdText += String.Format(@"{0} integer primary key AUTOINCREMENT, ", "id");
            cmdText += String.Format(@"{0} datetime not null, ", "start_time");
            cmdText += String.Format(@"{0} datetime not null, ", "stop_time");
            cmdText += String.Format(@"{0} bool not null, ", "manual_exit");
            cmdText += String.Format(@"{0} bool not null );", "complete");

            ExecuteNonQueryGetAffected(cmdText, null);
        }

        public DataTable GetStartStopTimeTableAllData()
        {
            String cmdText = String.Format("select * from {0};", "start_stop_time");
            return ExecuteReaderGetAll(cmdText, null);
        }

        public int InsertStartTime(DateTime startTime)
        {
            String cmdText = String.Format(
                @"insert into {0} (start_time, stop_time, manual_exit, complete) values(
                @start_time, @stop_time, @manual_exit, @complete);select last_insert_rowid() from {0};",
                "start_stop_time");

            Dictionary<String, Object> sqlParamDic = new Dictionary<string, object>();
            sqlParamDic.Add("@start_time", startTime);
            sqlParamDic.Add("@stop_time", startTime);
            sqlParamDic.Add("@manual_exit", false);
            sqlParamDic.Add("@complete", false);

            return ExecuteScalarGetNum(cmdText, sqlParamDic);
        }

        public void UpdateStopTime(int id, DateTime stopTime, bool manualExit)
        {
            String cmdText = String.Format(
                "update {0} set stop_time=@stop_time,manual_exit=@manual_exit,complete=@complete where id=@id;",
                "start_stop_time");

            Dictionary<String, Object> sqlParamDic = new Dictionary<string, object>();
            sqlParamDic.Add("@id", id);
            sqlParamDic.Add("@stop_time", stopTime);
            sqlParamDic.Add("@manual_exit", manualExit);
            sqlParamDic.Add("@complete", true);

            ExecuteNonQueryGetAffected(cmdText, sqlParamDic);
        }

        public int GetMaxId()
        {
            String cmdText = String.Format("select max(id) from {0};", "start_stop_time");
            return ExecuteScalarGetNum(cmdText, null);
        }

        public DataTable GetLastRowData()
        {
            String cmdText = String.Format("select * from {0} order by id desc limit 1;",
                "start_stop_time");
            return ExecuteReaderGetAll(cmdText, null);
        }

        private int ExecuteNonQueryGetAffected(
            String cmdText, Dictionary<String, Object> sqlParamDic = null)
        {
            int affected = 0;
            using (SQLiteConnection sqlCon = new SQLiteConnection(SQLOpenCmdText))
            {
                sqlCon.Open();
                using (SQLiteCommand sqlCmd = new SQLiteCommand(cmdText, sqlCon))
                {
                    if (sqlParamDic != null)
                    {
                        foreach (KeyValuePair<String, Object> kv in sqlParamDic)
                        {
                            sqlCmd.Parameters.AddWithValue(kv.Key, kv.Value);
                        }
                    }
                    affected = sqlCmd.ExecuteNonQuery();
                }
                sqlCon.Close();
            }

            return affected;
        }

        private int ExecuteScalarGetNum(
            String cmdText, Dictionary<String, Object> sqlParamDic = null)
        {
            int num = 0;
            using (SQLiteConnection sqlCon = new SQLiteConnection(SQLOpenCmdText))
            {
                sqlCon.Open();
                using (SQLiteCommand sqlCmd = new SQLiteCommand(cmdText, sqlCon))
                {
                    if (sqlParamDic != null)
                    {
                        foreach (KeyValuePair<String, Object> kv in sqlParamDic)
                        {
                            sqlCmd.Parameters.AddWithValue(kv.Key, kv.Value);
                        }
                    }
                    object obj = sqlCmd.ExecuteScalar();
                    if (obj != System.DBNull.Value) num = Convert.ToInt32(obj);
                }
                sqlCon.Close();
            }

            return num;
        }

        private DataTable ExecuteReaderGetAll(
            String cmdText, Dictionary<String, Object> sqlParamDic = null)
        {
            DataTable dt = null;
            using (SQLiteConnection sqlCon = new SQLiteConnection(SQLOpenCmdText))
            {
                sqlCon.Open();
                using (SQLiteCommand sqlCmd = new SQLiteCommand(cmdText, sqlCon))
                {
                    if (sqlParamDic != null)
                    {
                        foreach (KeyValuePair<String, Object> kv in sqlParamDic)
                        {
                            sqlCmd.Parameters.AddWithValue(kv.Key, kv.Value);
                        }
                    }

                    using (SQLiteDataReader sqlDataReader = sqlCmd.ExecuteReader())
                    {
                        dt = new DataTable();
                        for (int i = 0; i < sqlDataReader.FieldCount; i++)
                        {
                            dt.Columns.Add(sqlDataReader.GetName(i), sqlDataReader.GetFieldType(i));
                        }

                        while (sqlDataReader.Read())
                        {
                            DataRow dr = dt.NewRow();
                            for (int i = 0; i < dt.Columns.Count; i++)
                            {
                                dr[i] = sqlDataReader[i];
                            }
                            dt.Rows.Add(dr);
                        }
                    }
                }
                sqlCon.Close();
            }

            return dt;
        }
    }
}
