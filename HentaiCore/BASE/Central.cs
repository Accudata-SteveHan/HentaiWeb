using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Windows.Forms;

namespace HentaiCore {
    public class Central {

        //String
        public static string UserName = "";
        public static string Password = "";
        public static string AppCode = "";
        public static string MainUrl = "";

        //Object
        public static Logger logger = null;
        public static AUTH authUser = null;
        public static FileDirector PathDirector = null;
        //public static JOB_CONTROL jobControl = null;
        //public static JOB_CONTROL jobDownload = null;

        //Form
        public static Form main = null;
        public static Form download = null;

        public static string GetPara(string Key) {
            var x = ConfigurationManager.AppSettings.AllKeys.Contains(Key) ?
                ConfigurationManager.AppSettings[Key] : null;

            return x != null ? x.ToString() : "";

        }

        public static string GetUserPara(string Key) {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var appSettings = configFile.AppSettings.Settings;

            var x = appSettings.AllKeys.Contains(Key) ?
                appSettings[Key].Value : null;

            return x != null ? x.ToString() : "";

        }

        public static void SetUserPara(string Key, string value) {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var appSettings = configFile.AppSettings.Settings;
            if (appSettings.AllKeys.Contains(Key)) {
                appSettings[Key].Value = value;

            } else {
                appSettings.Add(Key, value);

            }

            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);

        }


        public static Logger.Level GetLevel(string key) {
            return
                key.ToUpper() == "CRITICAL" ? Logger.Level.Critical :
                key.ToUpper() == "ERROR" ? Logger.Level.Error :
                key.ToUpper() == "WARNING" ? Logger.Level.Warning :
                key.ToUpper() == "INFO" ? Logger.Level.Info :
                key.ToUpper() == "LOG" ? Logger.Level.Log :
                key.ToUpper() == "TRACE" ? Logger.Level.Trace :
                Logger.Level.Trace;

        }

    }

}
