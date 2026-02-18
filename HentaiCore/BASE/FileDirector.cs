using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HentaiCore {
    public class FileDirector {
        private string keyDirStr = "{0}_{1}";
        private string keyFileStr = "{0}_{1}_{2}";
        private string skipChar = "\\/:*?\"><|";

        //儲存路徑
        public string SaveDirPath = "";
        //儲存資料夾名稱格式
        //"CODE" = "PKEY_SKEY"
        //"NAME" = "FILE_NAME"
        public string SaveDirNameType = "";
        //儲存檔案名稱格式
        //"CODE" = "PKEY_SKEY_流水號"
        //"NUM" = "流水號"
        public string SaveFileNameType = "";

        public FileDirector(string dirType = "CODE", string fileType = "CODE") {
            string errMsg = "";

            string dir = dirType.ToUpper();
            string file = fileType.ToUpper();

            if (dir != "CODE" && dir != "NAME") {
                errMsg = "設定資料夾格式參數錯誤";

            }
            if (file != "CODE" && file != "NUM") {
                errMsg = "設定檔案格式參數錯誤";

            }
            if (errMsg != "") {
                throw new Exception(errMsg);

            }

            this.SaveFileNameType = fileType;
            this.SaveDirNameType = dirType;

        }

        public string GetSaveDirName(string P_KEY, string S_KEY, string name) {
            string dirName = "";

            if (this.SaveDirNameType == "CODE") {
                dirName = string.Format(keyDirStr, P_KEY, S_KEY);

            } else if (this.SaveDirNameType == "NAME") {
                dirName = name;
                foreach (char c in skipChar) {
                    dirName.Replace(c, '_');

                }

            } else {
                throw new Exception("設定資料夾名稱格式錯誤");

            }

            return dirName;

        }

        public string GetSaveFileName(string P_KEY, string S_KEY, string name) {
            string fileName = "";

            if (this.SaveDirNameType == "CODE") {
                fileName = string.Format(keyFileStr, P_KEY, S_KEY, name);

            } else if (this.SaveDirNameType == "NUM") {
                fileName = name;

            } else {
                throw new Exception("設定資料夾名稱格式錯誤");

            }

            return fileName;

        }

        public string GetDirName(string dirName) {
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (dirName != "") {
                try {
                    DirectoryInfo dirInfo = new DirectoryInfo(dirName);
                    if (!dirInfo.Exists) {
                        dirInfo.Create();

                        dir = dirName;
                    }

                } catch (Exception ex) {
                    string errMsg = "無法處理儲存資料夾路徑 : {0}";
                    Central.logger.WriteWarning(string.Format(errMsg, dirName));
                    Central.logger.WriteWarning(ex.ToString());

                    dir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                }

            }

            return dir;

        }

    }
}
