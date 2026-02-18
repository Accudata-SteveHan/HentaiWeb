using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;

namespace HentaiCore
{
    public class Logger
    {
        public enum Level
        {
            Trace = 0,     //偵錯模式
            Log = 1,        //全記錄
            Info = 2,       //資料訊息
            Warning = 3,    //警告
            Error = 4,      //錯誤
            Critical = 5    //系統崩毀
        }

        public Level LogType;
        public string LogPath = "";

        public Logger()
        {
            string level = Central.GetPara("LOG");
            this.LogType = level != "" ? Central.GetLevel(level) : Level.Info;
            this.LogPath = Central.GetPara("LOG_PATH");
        }

        public Logger(Level type)
        {
            this.LogType = type;
            this.LogPath = Central.GetPara("LOG_PATH");

        }

        public void Write(object obj, string message)
        {
            string name = obj.GetType().Name.ToString().ToUpper();
            string para = Central.GetPara("LOG_" + name);
            Level level = Central.GetLevel(para);

            switch (level)
            {
                case Level.Critical:
                    this.WriteCritical(message);
                    break;

                case Level.Error:
                    this.WriteError(message);
                    break;

                case Level.Warning:
                    this.WriteWarning(message);
                    break;

                case Level.Info:
                    this.WriteInfo(message);
                    break;

                case Level.Log:
                    this.WriteLog(message);
                    break;

                case Level.Trace:
                    this.WriteTrace(message);
                    break;

                default:
                    this.WriteTrace(string.Format("{0}:{1}", name, message));
                    break;

            }

        }

        public void WriteTrace(string message)
        {
            this.log(Level.Trace, message);

        }

        public void WriteCritical(string message)
        {
            this.log(Level.Critical, message);

        }

        public void WriteError(string message)
        {
            this.log(Level.Error, message);

        }

        public void WriteWarning(string message)
        {
            this.log(Level.Warning, message);

        }

        public void WriteInfo(string message)
        {
            this.log(Level.Info, message);

        }

        public void WriteLog(string message)
        {
            this.log(Level.Log, message);

        }

        public void WriteHtml(string url, string message)
        {
            if (Level.Trace < this.LogType)
            {
                return;
            }

            StreamWriter writer = null;

            try
            {
                string fileName = "";
                if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    Uri uri = new Uri(url);
                    string host = uri.Host;
                    string path = uri.PathAndQuery.Split('?')[0];

                    string urlPath = System.Web.HttpUtility.UrlEncode(
                        path.StartsWith("/") ? path.Substring(1, path.Length - 1) : path);
                    fileName = string.Format("{0}_log.txt", urlPath.Trim() != "" ? urlPath : DateTime.Now.ToString("yyyyMMddHHmmss"));

                    fileName = Path.Combine(this.DirCreate("HTML"), fileName);

                    using (writer = new StreamWriter(fileName, true, Encoding.UTF8))
                    {
                        writer.WriteLine(url);
                        writer.WriteLine(message);

                    }

                }
                else
                {
                    fileName = string.Format("{0}_{1}_log.txt", url.Split('_')[0], url.Split('_')[1]);

                    fileName = Path.Combine(this.DirCreate("HTML"), fileName);

                    using (writer = new StreamWriter(fileName, true, Encoding.UTF8))
                    {
                        writer.WriteLine(url);
                        writer.WriteLine(string.Format("{0} : {1}", url, message));

                    }

                }

            }
            catch (Exception ex)
            {
                this.WriteLog(string.Format("HTML = {0} , LOG失敗", url));

            }

        }

        private void log(Level type, string message)
        {
            if (type < this.LogType)
            {
                return;
            }

            this.LogPath = this.DirCreate("");

            lock (typeof(int))
            {
                string filePath = Path.Combine(
                    this.LogPath,
                    string.Format(
                        "{0}_{1}.log",
                        new FileInfo(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName).Name,
                        DateTime.Now.Date.ToString("yyyyMMdd")));

                using (StreamWriter writer = new StreamWriter(filePath, true, Encoding.UTF8))
                {
                    string msgStr = "{0}:{1}_{2}  {3}";
                    string logStr = string.Format(msgStr,
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        System.Threading.Thread.CurrentThread.ManagedThreadId.ToString("000"),
                        type.ToString(),
                        message);

                    writer.WriteLine(logStr);

                }

            }

        }

        private string DirCreate(string dir)
        {
            try
            {
                if (this.LogPath == "")
                {
                    this.LogPath = Environment.CurrentDirectory;

                }

                dir = Path.Combine(this.LogPath, dir);

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);

                }

                return dir;

            }
            catch (Exception ex)
            {
                throw new Exception("無法建立Log資料夾\n" + ex.ToString());

            }

        }

    }

}
