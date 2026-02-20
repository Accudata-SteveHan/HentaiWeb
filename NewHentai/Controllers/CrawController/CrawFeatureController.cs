using HentaiCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace NewHentai.Controllers
{
    public class CrawFeatureController : Controller
    {
        private DataControlMdf control = new DataControlMdf();
        private string conStrMain = @"Data Source={0}\SQLEXPRESS;DATABASE=HENTAI;Integrated Security=false;User ID=sa;PASSWORD=c06g4cl6;Connection Timeout=180";
        private string conStrLib = @"Data Source={0}\SQLEXPRESS;DATABASE=ANIMELIBRARY;Integrated Security=false;User ID=sa;PASSWORD=c06g4cl6;Connection Timeout=180";
        private string conStrPic = @"Data Source={0}\SQLEXPRESS;DATABASE=HENTAIPIC;Integrated Security=false;User ID=sa;PASSWORD=c06g4cl6;Connection Timeout=180";
        private string conStrBook = @"Data Source={0}\SQLEXPRESS;DATABASE=MELONBOOK;Integrated Security=false;User ID=sa;PASSWORD=c06g4cl6;Connection Timeout=600";

        public ActionResult Index()
        {
            return View();
        }

        #region Core
        private DataTable QueryDBData(string sql, string conStr)
        {
            string server = System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("DB") ?
                System.Configuration.ConfigurationManager.AppSettings["DB"] :
                ".";

            conStr = string.Format(conStr, server);

            this.control.conStr = conStr;

            DataTable data = this.control.GetData(sql);
            return data;

        }

        private void UploadDBData(string[] sqls, string conStr)
        {
            try
            {
                string server = System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("DB") ?
                    System.Configuration.ConfigurationManager.AppSettings["DB"] :
                    ".";

                conStr = string.Format(conStr, server);

                this.control.conStr = conStr;
                int result = this.control.UploadData(sqls);

                if (result < 0)
                {
                    throw new Exception("存檔失敗");

                }

            }
            catch (Exception ex)
            {
                throw ex;

            }

        }

        private void UploadDBData(DataTable data, string mainSql, string conStr)
        {
            try
            {
                string server = System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("DB") ?
                    System.Configuration.ConfigurationManager.AppSettings["DB"] :
                    ".";

                conStr = string.Format(conStr, server);

                this.control.conStr = conStr;
                int result = this.control.UploadData(mainSql, data);

                if (result < 0)
                {
                    throw new Exception("存檔失敗");

                }

            }
            catch (Exception ex)
            {
                throw ex;

            }

        }

        #endregion Core

        #region DA
        private DataTable GetStatus(string key)
        {
            string sql = "SELECT * FROM STATUS WHERE 1=1 {0}";
            string filter = "";

            if (key != "")
            {
                filter += string.Format(" and WEB = '{0}'", key);

            }

            sql = string.Format(sql, filter);

            DataTable data = this.QueryDBData(sql, conStrMain);
            data.PrimaryKey = new DataColumn[] { data.Columns["WEB"] };

            return data;

        }

        private void UpdateStatus(DataTable data)
        {
            string sql = "SELECT * FROM STATUS";

            this.UploadDBData(data, sql, conStrMain);

        }

        private DataTable GetDataExHentaiDetail(string pKey, string sKey)
        {
            string sql = "SELECT * FROM DATA_EXHENTAI_DETAIL WHERE 1=1 {0}";
            string filter = "";

            if (pKey != "")
            {
                filter += string.Format(" and PKEY = '{0}'", pKey);

            }

            if (sKey != "")
            {
                filter += string.Format(" and SKEY = '{0}'", sKey);

            }

            sql = string.Format(sql, filter);

            DataTable data = this.QueryDBData(sql, conStrLib);
            data.PrimaryKey = new DataColumn[] { data.Columns["PKEY"], data.Columns["SKEY"] };

            return data;

        }

        private void UpdateDataExHentaiDetail(DataTable data)
        {
            string sql = "SELECT * FROM DATA_EXHENTAI_DETAIL";

            this.UploadDBData(data, sql, conStrLib);

        }

        private DataTable GetDataExHentaiDisplay(string pKey, string sKey)
        {
            string sql = "SELECT * FROM DATA_EXHENTAI_DISPLAY WHERE 1=1 {0}";
            string filter = "";

            if (pKey != "")
            {
                filter += string.Format(" and PKEY = '{0}'", pKey);

            }

            if (sKey != "")
            {
                filter += string.Format(" and SKEY = '{0}'", sKey);

            }

            sql = string.Format(sql, filter);

            DataTable data = this.QueryDBData(sql, conStrLib);
            data.PrimaryKey = new DataColumn[] {
                data.Columns["PKEY"],
                data.Columns["SKEY"],
                data.Columns["DISPLAY_TYPE"]};

            return data;

        }

        private void UpdateDataExHentaiDisplay(DataTable data)
        {
            string sql = "SELECT * FROM DATA_EXHENTAI_DISPLAY";

            this.UploadDBData(data, sql, conStrLib);

        }

        private DataTable GetDataSymbol(string id, string hash)
        {
            string sql = "SELECT * FROM DATA_SYMBOL WHERE 1=1 {0}";
            string filter = "";

            if (id != "")
            {
                filter += string.Format(" and ID = '{0}'", id);

            }

            if (hash != "")
            {
                filter += string.Format(" and HASH = '{0}'", hash);

            }

            sql = string.Format(sql, filter);

            DataTable data = this.QueryDBData(sql, conStrLib);

            return data;

        }

        private void UpdateDataSymbol(DataTable data)
        {
            string sql = "SELECT * FROM DATA_SYMBOL";

            this.UploadDBData(data, sql, conStrLib);

        }

        private DataTable GetDataExhentaiSymbol(string id, string hash)
        {
            string sql = "SELECT * FROM DATA_EXHENTAI_SYMBOL WHERE 1=1 {0}";
            string filter = "";

            if (id != "")
            {
                filter += string.Format(" and ID = '{0}'", id);

            }

            if (hash != "")
            {
                filter += string.Format(" and SYMBOL_KEY = '{0}'", hash);

            }

            sql = string.Format(sql, filter);

            DataTable data = this.QueryDBData(sql, conStrMain);

            return data;

        }

        private void UpdateDataExhentaiSymbol(DataTable data)
        {
            string sql = "SELECT * FROM DATA_EXHENTAI_SYMBOL";

            this.UploadDBData(data, sql, conStrMain);

        }

        private DataTable GetGroupSummaryData()
        {
            string sql = "SELECT DISTINCT CAST(GROUP_ID as int) GROUP_ID FROM SUMMARY ORDER BY CAST(GROUP_ID as int)";
            //string sql = "SELECT MAX( CAST(GROUP_ID as int) ) GROUP_ID FROM SUMMARY";

            DataTable data = this.QueryDBData(sql, conStrMain);

            return data;

        }

        #endregion DA

        #region FUNCTION
        private List<string> RegexFeature(string word)
        {
            List<string> list = new List<string>();

            Regex r = null;
            string regex = "";
            //regex = @"[\[\(](\w+)[\]\)]";

            regex = @"\([^\(\)]*\)";
            r = new Regex(regex);
            while (true)
            {
                MatchCollection mc = r.Matches(word);

                if (mc.Count == 0) { break; }

                foreach (Match m in mc)
                {
                    string val = m.Value;

                    list.Add(val);

                    word = word.Replace(val + " ", "");
                    word = word.Replace(" " + val, "");
                    word = word.Replace(val, "");

                }

            }

            regex = @"\[[^\[\]]*\]";
            r = new Regex(regex);
            while (true)
            {
                MatchCollection mc = r.Matches(word);

                if (mc.Count == 0) { break; }

                foreach (Match m in mc)
                {
                    string val = m.Value;

                    list.Add(val);

                    word = word.Replace(val + " ", "");
                    word = word.Replace(" " + val, "");
                    word = word.Replace(val, "");

                }

            }

            list.Insert(0, word);
            
            return list;

        }

        private DataTable ProcessSymbolData(List<string> symbolList)
        {
            DataTable symData = this.GetDataSymbol("-1", "-1");

            MD5 md5 = new MD5CryptoServiceProvider();

            foreach (string symbol in symbolList)
            {
                string hash = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(symbol))).Replace("-", "");

                DataTable dbData = this.GetDataSymbol("", hash);
                DataRow[] drsSymData = symData.Select(string.Format("HASH = '{0}'", hash));

                if (dbData.Rows.Count == 0 && drsSymData.Length == 0)
                {
                    DataRow newRow = symData.NewRow();

                    newRow["NAME"] = symbol;
                    newRow["HASH"] = hash;

                    symData.Rows.Add(newRow);

                }
                else if (drsSymData.Length == 0)
                {
                    symData.ImportRow(dbData.Rows[0]);

                }

            }

            this.UpdateDataSymbol(symData);
            symData.AcceptChanges();

            return symData;

        }


        #endregion FUNCTION

        //取得 ExHentai 書籍詳細資訊
        public ActionResult GET_FEATURE(string param)
        {
            string main = "EXHENTAI";
            string type = "FEATURE";
            string key = string.Format("{0}_{1}", main, type);
            //string basePage = "https://exhentai.org";
            int tmp = -1;

            string url = "";
            DataTable dataWeb = this.GetStatus(key);

            if (dataWeb.Rows.Count == 0)
            {
                ViewData["DATA"] = "資料錯誤";

                return View();

            }

            //Json json = new Json();

            DataRow webRow = dataWeb.Rows[0];

            string procPage =
                    param == null || !int.TryParse(param, out tmp) ?
                    webRow["PROC_PAGE"].ToString() :
                    param;

            //MD5 md5 = new MD5CryptoServiceProvider();

            double period = double.Parse(ConfigurationManager.AppSettings["WatermarkPeriod"].ToString());
            int max = Convert.ToInt32(Math.Pow((double)10, period));

            //for (int x = 0; x < max; x++)
            //{
            //    Json jsonObject = new Json();
            //    int doPage = int.Parse(procPage.ToString()) * (int)period + x;

            //}
            //
            foreach (DataRow groupRow in this.GetGroupSummaryData().Rows)
            {
                //Json jsonObject = new Json();
                int doPage = int.Parse(groupRow["GROUP_ID"].ToString()) * 10000 + int.Parse(procPage);
                //url = string.Format(pathPattern, procPage);
                //
                //    //string reex = @"[\[\(](\w+)[\]\)]";

                jsonObject.Add("PKEY", doPage);

                DataTable data = this.GetDataExHentaiDetail(doPage.ToString(), "");
                DataTable dataDisplay = this.GetDataExHentaiDisplay(doPage.ToString(), "");

                if (data.Rows.Count == 0)
                {
                    jsonObject.Add("RESULT", "無條件資料");

                }
                else
                {
                    DataRow row = data.Rows[0];

                    string pKey = row["PKEY"].ToString();

                    DataTable dtSymData = this.GetDataExhentaiSymbol("-1", "-1");

                    List<string> fieldList = new List<string>();
                    fieldList.Add("NAME_UNI");
                    fieldList.Add("NAME_ENG");

                    List<string> titleList = new List<string>();
                    foreach (string field in fieldList)
                    {
                        titleList.Add(row[field].ToString());

                    }
                    for (int i = 0; i < fieldList.Count; i++)
                    //foreach (string title in titleList)
                    {
                        string title = titleList[i];

                        Console.WriteLine(title);

                        List<string> featureList = this.RegexFeature(title);

                        string word = featureList[0];
                        featureList.Remove(word);

                        DataTable symData = this.ProcessSymbolData(featureList);

                        foreach (DataRow symRow in symData.Rows)
                        {
                            string hash = symRow["HASH"].ToString();

                            DataTable dataExhentaiSymbol = this.GetDataExhentaiSymbol(pKey, hash);
                            DataRow[] drsSymData =
                                dtSymData.Select(
                                    string.Format(
                                        "ID = '{0}' and SYMBOL_KEY = '{1}'", pKey, hash));

                            if (dataExhentaiSymbol.Rows.Count == 0 && drsSymData.Length == 0)
                            {
                                DataRow newRow = dtSymData.NewRow();

                                newRow["ID"] = pKey;
                                newRow["SYMBOL_KEY"] = hash;
                                newRow["SYMBOL_STATUS"] = "Y";

                                dtSymData.Rows.Add(newRow);

                            }
                            else
                            {
                                dtSymData.ImportRow(dataExhentaiSymbol.Rows[0]);

                            }

                        }

                        string display = word.Trim();

                        DataRow[] displayRows = dataDisplay.Select(string.Format("DISPLAY_TYPE = '{0}'", fieldList[i]));

                        if (displayRows.Length == 0)
                        {
                            DataRow displayRow = dataDisplay.NewRow();

                            displayRow["PKEY"] = row["PKEY"].ToString();
                            displayRow["SKEY"] = row["SKEY"].ToString();
                            displayRow["DISPLAY_TYPE"] = fieldList[i].ToString();
                            displayRow["TYPE"] = row["TYPE"].ToString();
                            displayRow["LANG"] = row["LANG"].ToString();
                            displayRow["NAME_DISPLAY"] = display;
                            displayRow["PIC_DISPLAY"] = row["PIC_THUMB"].ToString();
                            displayRow["USER_ID"] = "SYSTEM";
                            displayRow["UPDATE_TIME"] = DateTime.Now;

                            dataDisplay.Rows.Add(displayRow);

                        }
                        else
                        {
                            DataRow displayRow = displayRows[0];

                            displayRow["NAME_DISPLAY"] = display;
                            displayRow["USER_ID"] = "SYSTEM";
                            displayRow["UPDATE_TIME"] = DateTime.Now;

                        }

                        this.UpdateDataExHentaiDetail(data);
                        this.UpdateDataExHentaiDisplay(dataDisplay);
                        this.UpdateDataExhentaiSymbol(dtSymData);

                        jsonObject.Add($"{fieldList[i]}COUNT", symData.Rows.Count);

                        int symCount = 0;
                        foreach (DataRow symRow in symData.Rows)
                        {
                            symCount++;
                            jsonObject.Add(string.Format("{0}_SYMBOL_{1}", fieldList[i], symCount), symRow["NAME"].ToString());

                        }

                    }

                }

                json.Add(groupRow["GROUP_ID"].ToString(), jsonObject.jsonObject);
                //json.Add(x.ToString(), jsonObject.jsonObject);

            }

            if (param == null)
            {
                tmp = int.Parse(procPage);
                if (tmp < int.Parse(webRow["TOTAL_PAGE"].ToString()))
                {
                    tmp++;

                }

                DataTable summaryTable = this.GetGroupSummaryData();

                //webRow["TOTAL_PAGE"] = summaryTable.Rows[0]["GROUP_ID"].ToString();
                webRow["TOTAL_PAGE"] = "9999";
                webRow["PROC_PAGE"] = tmp;
                webRow["UPDATE_TIME"] = DateTime.Now;

            }

            this.UpdateStatus(dataWeb);

            ViewData["DATA"] = json.ToString();

            return View();

        }

    }

}
