using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Timers;

namespace DataBaseBackup
{
    class Program
    {
        static string userName = GetSystemInfo.GetUserName();
        static string ip = GetSystemInfo.GetClientLocalIPv4Address();
        static string mac = GetSystemInfo.GetMacAddress();
        static string directory_tmp = string.Empty;
        static bool isCompleted = false;
        static void Main(string[] args)
        {
            try
            {
                if (System.Configuration.ConfigurationManager.
           AppSettings["isvisiable"].Trim() != "true")
                {
                    ConsoleHelper.hideConsole();
                }
                else
                {
                    
                }
                Console.WriteLine("\n\r" + "Do not shut down at runtime ,please!" + "\n\r");
                DateTime dt_start = DateTime.Now;
                Console.Write(dt_start.ToString("yyyy-MM-dd HH:mm:ss") + " DataBase BackUp Starting.");
                System.Timers.Timer timer = new System.Timers.Timer();
                timer.Elapsed += new ElapsedEventHandler(TimeEvent);
                timer.Interval = 2000;
                timer.Enabled = true;
                if (BackUpDataBase())
                {
                    if (DelBackFile())
                    {
                        isCompleted = true;
                    }
                }
                DateTime dt_end = DateTime.Now;
                TimeSpan ts = dt_end - dt_start;
                LogHelper.WriteLogInfo(typeof(Program), "UseTime: "+ts.Seconds+" seconds");
                Console.WriteLine("\n\r\n\r" + dt_end.ToString("yyyy-MM-dd HH:mm:ss") + " DataBase BackUp End. Use Time: " + ts.Seconds + " seconds");
            }
            catch { }
            finally {
                System.Threading.Thread.Sleep(1500);
            }
        }
        private static void TimeEvent(object source, ElapsedEventArgs e)
        {
            if (isCompleted)
            {
               
            }
            else
            {
                Console.Write(".");
            }
        }
        private static SqlConnection GetSqlConnection()
        {
            SqlConnection sqlConn = null;
            string server = System.Configuration.ConfigurationManager.
              AppSettings["server"].Trim();
            string port = System.Configuration.ConfigurationManager.
             AppSettings["port"].Trim();
            string username= System.Configuration.ConfigurationManager.
             AppSettings["username"].Trim();
            string password = System.Configuration.ConfigurationManager.
             AppSettings["password"].Trim();
            string database = System.Configuration.ConfigurationManager.
             AppSettings["database"].Trim();
                  string typeofdb = System.Configuration.ConfigurationManager.
             AppSettings["typeofdb"].Trim();

            switch(typeofdb){
                case "MSSQL":
                      string string_sqlConn = string.Format("server={0},{1};user={2};password={3}; database={4};Integrated Security=false; Connection Timeout=3", server, port, username, password, database);
                     sqlConn= new SqlConnection(string_sqlConn);
                      break;
                default: break;
            }

            return sqlConn;

        }
        private static bool IsConnection(SqlConnection sqlConn)
        {
            bool IsCanConnectioned = false;

            try
            {
                sqlConn.Open();
                IsCanConnectioned = true;
                //string msg = "Database successfully connected. UserName: " + userName + " IP: " + ip + " MAC: " + mac;
                //LogHelper.WriteLogInfo(typeof(Program), msg);
            }
            catch(Exception e)
            {
                string msg = "Failed to connect to database. Exception Message: "+e.Message +"UserName: " + userName + " IP: " + ip + " MAC: " + mac;
                LogHelper.WriteLogError(typeof(Program),msg);
                IsCanConnectioned = false;
            }
            finally
            {
                sqlConn.Close();
            }
            if (sqlConn.State == ConnectionState.Closed || sqlConn.State == ConnectionState.Broken)
            {
               
                return IsCanConnectioned;
            }
            else
            {
                return IsCanConnectioned;
            }
        }
        private static void CreateDirectory()
        {
            string directory = System.Configuration.ConfigurationManager.
          AppSettings["directory"].Trim();
            directory_tmp = @"c:\tmp_bak\";
            if (!System.IO.Directory.Exists(directory_tmp))
            {
                System.IO.Directory.CreateDirectory(directory_tmp);
                DirectoryInfo di = new DirectoryInfo(directory_tmp);
                if (di.Attributes != FileAttributes.Hidden)
                {
                    di.Attributes = FileAttributes.Hidden;
                }
            }
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
                string msg = "Create Directory: " + directory + ". UserName: " + userName + " IP: " + ip + " MAC: " + mac;
                LogHelper.WriteLogInfo(typeof(Program), msg);
            }
        }
        private static bool BackUpDataBase()
        {
            Boolean isSuccess = false;
            try
            {   
                CreateDirectory();
                if (IsConnection(GetSqlConnection()))
                {
                    using (SqlConnection sqlConn = GetSqlConnection())
                    {
                        if (sqlConn.State == ConnectionState.Closed)
                        {
                            sqlConn.Open();
                        }
                        string database = System.Configuration.ConfigurationManager.
           AppSettings["database"].Trim();
                        string directory = System.Configuration.ConfigurationManager.
          AppSettings["directory"].Trim();
                        string key=NPIO.EncryptHelper.Encrypt( System.Configuration.ConfigurationManager.
        AppSettings["key"].Trim());
                        string createTime = DateTime.Now.ToString("yyyyMMddHHmmssfs");
                        string fileFullName = directory_tmp + database +"_"+ createTime + ".bak";
                        string sql_bak = string.Format("backup database  {0} to disk='{1}'", database, fileFullName);
                        SqlCommand cmd = new SqlCommand(sql_bak, sqlConn);
                        cmd.ExecuteNonQuery();
                        if (System.IO.File.Exists(fileFullName))
                        { new FileInfo(fileFullName).Attributes = FileAttributes.Hidden; }
                        string fileFullName_Zip=directory+"bak_"+createTime+".zip";

                        ZipClass.ZipFileMain(fileFullName, fileFullName_Zip, key);

                        if ((System.IO.File.Exists(fileFullName_Zip))&&(System.IO.File.Exists(fileFullName)))
                        {
                            System.IO.File.Delete(fileFullName);
                            isSuccess = true;
                            string msg2 = "Backup Database successful. FileFullName: " + fileFullName_Zip +" UserName: " + userName + " IP: " + ip + " MAC: " + mac;
                            LogHelper.WriteLogInfo(typeof(Program), msg2);
                        }
                        else {
                            if (System.IO.File.Exists(fileFullName)) { System.IO.File.Delete(fileFullName); }
                            isSuccess = false;
                            string msg2 = "Failed to Backup Database. FileFullName: " + fileFullName_Zip + " UserName: " + userName + " IP: " + ip + " MAC: " + mac;
                            LogHelper.WriteLogError(typeof(Program), msg2);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                isSuccess = false;
                string msg = "Exception happened. Exception Message: "+e.Message + "UserName: " + userName + " IP: " + ip + " MAC: " + mac;
                LogHelper.WriteLogError(typeof(Program),msg);
            }
             return isSuccess;
        }
        private static bool DelBackFile(){
            try
            {
                int days = int.Parse(System.Configuration.ConfigurationManager.
                  AppSettings["days"].Trim());
                string directory = System.Configuration.ConfigurationManager.
                 AppSettings["directory"].Trim();
                DateTime DT_NOW = DateTime.Now;
                DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                FileInfo[] fileInfo = directoryInfo.GetFiles();
                foreach (FileInfo f in fileInfo)
                {
                    DateTime DT_FILEINFO = f.CreationTime;
                    TimeSpan ts = DT_NOW - DT_FILEINFO;
                    if (ts.Days > days)
                    {
                        try {
                            System.IO.File.Delete(f.FullName);
                            string msg = "Delete File successful. FileFullName: " + f.FullName + "UserName: " + userName + " IP: " + ip + " MAC: " + mac;
                            LogHelper.WriteLogError(typeof(Program), msg);
                        }
                        catch {
                            string msg = "Failed to Delete File. FileFullName: " + f.FullName + "UserName: " + userName + " IP: " + ip + " MAC: " + mac;
                            LogHelper.WriteLogError(typeof(Program), msg);
                            continue;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                string msg = "Exception happened. Exception Message: "+e.Message+ "UserName: " + userName + " IP: " + ip + " MAC: " + mac;
                LogHelper.WriteLogError(typeof(Program), msg);
            }
            return true;
        }
    }
}
