using HentaiCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;
using System.Net;

namespace NewHentai.Controllers
{
    public class CrawNonHController : Controller
    {
        private DataControlMdf control = new DataControlMdf();
        private string conStrMain = @"Data Source={0}\SQLEXPRESS;DATABASE=HENTAI;Integrated Security=false;User ID=sa;PASSWORD=c06g4cl6;Connection Timeout=180";
        private string conStrLib = @"Data Source={0}\SQLEXPRESS;DATABASE=ANIMELIBRARY;Integrated Security=false;User ID=sa;PASSWORD=c06g4cl6;Connection Timeout=180";
        private string conStrPic = @"Data Source={0}\SQLEXPRESS;DATABASE=HENTAIPIC;Integrated Security=false;User ID=sa;PASSWORD=c06g4cl6;Connection Timeout=180";
        private string conStrBook = @"Data Source={0}\SQLEXPRESS;DATABASE=MELONBOOK;Integrated Security=false;User ID=sa;PASSWORD=c06g4cl6;Connection Timeout=600";

        public CrawNonHController()
        {

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

        private AUTH Login(string user, string password)
        {
            AUTH auth = new AUTH(user, password);
            if (auth.Login() >= 0)
            {
                return auth;

            }
            else
            {
                return null;

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

        private DataTable GetWeb(string key, string page)
        {
            string sql = "SELECT * FROM WEB WHERE 1=1 {0}";
            string filter = "";

            if (key != "")
            {
                filter += string.Format(" and WEB = '{0}'", key);

            }

            if (page == null)
            {
                filter += string.Format(
                    " and LOAD_TIME in (select MIN(LOAD_TIME) FROM WEB WHERE WEB = '{0}')",
                    key);
            }
            else if (page != "")
            {
                filter += string.Format(" and PAGE = '{0}'", page);

            }

            sql = string.Format(sql, filter);

            DataTable data = this.QueryDBData(sql, conStrMain);
            data.PrimaryKey = new DataColumn[] { data.Columns["WEB"], data.Columns["PAGE"] };

            return data;

        }

        private void UpdateWeb(DataTable data)
        {
            string sql = "SELECT * FROM WEB";

            this.UploadDBData(data, sql, conStrMain);

        }

        private DataTable GetObject(string key)
        {
            string sql = @"
SELECT O.* ,
    SUBSTRING(
		id , 
		CHARINDEX('_' , ID) + 1 , 
		LEN(ID) - (LEN(ID) - CHARINDEX('_' , ID , CHARINDEX('_' , ID) + 1) + 1) - (CHARINDEX('_' , ID))
	  ) PKEY
  FROM OBJECT O
 WHERE 1=1 AND WEB_ID = 'EXHENTAI' {0} 
 ORDER BY STATUS , CONVERT(DATE,UPDATE_TIME) , PKEY DESC";
            string filter = "";
            if (key == null)
            {
                filter += string.Format(
                    " and (STATUS = '0')",
                    key);
            }
            else if (key != "")
            {
                filter += string.Format(" and ID = '{0}'", key);

            }

            sql = string.Format(sql, filter);

            DataTable data = this.QueryDBData(sql, conStrMain);
            data.PrimaryKey = new DataColumn[] { data.Columns["ID"] };

            return data;

        }

        private DataTable GetPicObject(string key)
        {
            string sql = "SELECT * FROM OBJECT WHERE 1=1 AND WEB_ID = 'EXHENTAI' {0} ORDER BY STATUS , UPDATE_TIME";
            string filter = "";
            if (key == null)
            {
                filter += string.Format(
                    " and (STATUS = '1' or STATUS = '2')",
                    key);
            }
            else if (key != "")
            {
                filter += string.Format(" and ID = '{0}'", key);

            }


            sql = string.Format(sql, filter);

            DataTable data = this.QueryDBData(sql, conStrMain);
            data.PrimaryKey = new DataColumn[] { data.Columns["ID"] };

            return data;

        }

        private void UpdateObject(DataTable data)
        {
            string sql = "SELECT * FROM OBJECT";

            this.UploadDBData(data, sql, conStrMain);

        }

        private DataTable GetDataExHentai(string pKey, string sKey)
        {
            string sql = "SELECT * FROM DATA_EXHENTAI WHERE 1=1 {0}";
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

            DataTable data = this.QueryDBData(sql, conStrMain);
            data.PrimaryKey = new DataColumn[] { data.Columns["OBJ_ID"] };

            return data;

        }

        private void UpdateDataExHentai(DataTable data)
        {
            string sql = "SELECT * FROM DATA_EXHENTAI";

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

        private DataTable GetDataExHentaiPic(string pKey, string sKey)
        {
            string sql = "SELECT * FROM DATA_EXHENTAI_PIC WHERE 1=1 {0}";
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
            data.PrimaryKey = new DataColumn[] { data.Columns["PKEY"], data.Columns["SKEY"], data.Columns["PAGE"] };

            return data;

        }

        private void UpdateDataExHentaiPic(DataTable data)
        {
            string sql = "SELECT * FROM DATA_EXHENTAI_PIC";

            this.UploadDBData(data, sql, conStrLib);

        }

        #endregion DA

        // GET: Craw
        public ActionResult Index()
        {
            return View();

        }

        //取得 ExHentai 主頁資訊
        public ActionResult GET_EXHENTAI_GALLERY()
        {
            string main = "EXHENTAI";
            string type = "NON-H";
            string key = string.Format("{0}_{1}", main, type);
            string basePage = "https://exhentai.org";

            DataTable data = this.GetStatus(key);

            string url = data.Rows[0]["PATH"].ToString();

            #region Process_Site
            Central.logger = new Logger();

            if (Central.authUser == null)
            {
                Central.authUser = this.Login("username", "password");

            }

            SITE_INFO site = new SITE_INFO(url);
            List<PAGE_INFO> pages = ProcessSite(site, Central.authUser);

            #endregion Process_Site

            int pageCount = (pages == null) ? 0 : pages.Count;
            JObject jsonObject = new JObject();

            DataRow[] rows = data.Select(string.Format("WEB = '{0}'", key));

            if (rows.Length > 0)
            {
                DataRow row = rows[0];
                row["TOTAL_PAGE"] = pageCount;
                row["UPDATE_TIME"] = DateTime.Now;
                row["REFRESH_TIME"] = DateTime.Now;

                JObject newObject = new JObject();
                newObject["TYPE"] = "EDIT";
                newObject["PAGE"] = pageCount;
                jsonObject["DATA"] = newObject;

            }
            else
            {
                DataRow newRow = data.NewRow();

                string regexStr = string.Format(@"{0}/{1}/(\w+)", basePage, type.ToLower());

                newRow["WEB"] = key;
                newRow["BASE"] = basePage;
                newRow["PROC_PAGE"] = 0;
                newRow["TOTAL_PAGE"] = pageCount;
                newRow["PATH"] = regexStr.Replace(@"(\w+)", "{0}");
                newRow["LOAD_TIME"] = DateTime.Now;
                newRow["REFRESH_TIME"] = DateTime.Now;
                newRow["CREATE_TIME"] = DateTime.Now;
                newRow["UPDATE_TIME"] = DateTime.Now;

                data.Rows.Add(newRow);

                JObject newObject = new JObject();
                newObject["TYPE"] = "ADD";
                newObject["PAGE"] = pageCount;
                jsonObject["DATA"] = newObject;

            }

            this.UpdateStatus(data);

            ViewData["DATA"] = jsonObject.ToString();

            return View();

        }

        //取得 ExHentai 書籍詳細資訊
        public ActionResult GET_EXHENTAI_GALLERY_DETAIL(string param)
        {
            string main = "EXHENTAI";
            string type = "NON-H";
            string key = string.Format("{0}_{1}", main, type);
            int tmp = -1;

            string url = "";
            DataTable dataWeb = this.GetStatus(key);

            if (dataWeb.Rows.Count == 0)
            {
                ViewData["DATA"] = "資料錯誤";

                return View();

            }

            DataRow webRow = dataWeb.Rows[0];
            string basePage = webRow["BASE"].ToString();
            String pathPattern = webRow["PATH"].ToString();
            string lastPkey = webRow["LAST_KEY"].ToString();
            string procPage = param == null || !int.TryParse(param, out tmp) ? webRow["PROC_PAGE"].ToString() : param;
            url = string.Format(pathPattern, lastPkey);

            #region Process_Web
            Central.logger = new Logger();

            if (Central.authUser == null)
            {
                Central.authUser = this.Login("username", "password");

            }

            PAGE_INFO pageInfo = new PAGE_INFO(url);

            List<THREAD_INFO> threads = GetThreadListFromPage(pageInfo.page_url, Central.authUser);

            #endregion Process_Web

            DataTable dataExHentai = this.GetDataExHentai("-1", "-1");
            DataTable dataObject = this.GetObject("-1");

            int count = 0;
            JObject jsonObject = new JObject();
            jsonObject["URL"] = url;
            jsonObject["COUNT"] = 0;

            foreach (THREAD_INFO thread in threads)
            {
                string regex = basePage + @"/g/(\w+)/(\w+)/";
                string threadUrl = thread.thread_url;

                Match m = new Regex(regex).Match(threadUrl);

                string pKey = m.Groups[1].Value;
                string sKey = m.Groups[2].Value;
                string id = string.Format("EX_{0}_{1}", pKey, sKey);

                if (dataExHentai.Select(string.Format("OBJ_ID = '{0}'", id)).Length > 0)
                {
                    continue;

                }

                DataTable mainTable = this.GetDataExHentai(pKey, sKey);
                DataRow drMain = null;
                DataRow drKey = null;
                if (mainTable.Rows.Count == 0)
                {
                    #region ExHentai
                    drMain = dataExHentai.NewRow();

                    drMain["OBJ_ID"] = id;
                    drMain["PKEY"] = pKey;
                    drMain["SKEY"] = sKey;
                    drMain["CREATE_TIME"] = DateTime.Now;
                    drMain["UPDATE_TIME"] = DateTime.Now;

                    dataExHentai.Rows.Add(drMain);

                    #endregion ExHentai

                    #region Object
                    drKey = dataObject.NewRow();

                    drKey["WEB_ID"] = "EXHENTAI";
                    drKey["ID"] = id;
                    drKey["STATUS"] = "0";
                    drKey["CREATE_USER"] = "ADMIN";
                    drKey["CREATE_TIME"] = DateTime.Now;
                    drKey["UPDATE_USER"] = "ADMIN";
                    drKey["UPDATE_TIME"] = DateTime.Now;

                    dataObject.Rows.Add(drKey);

                    #endregion Object

                    JObject obj = new JObject();
                    obj["TYPE"] = "ADD";
                    obj["PKEY"] = pKey;
                    obj["SKEY"] = sKey;
                    jsonObject[id] = obj;

                    count++;

                }
                else
                {
                    dataExHentai.ImportRow(mainTable.Rows[0]);

                    JObject obj = new JObject();
                    obj["TYPE"] = "EXIST";
                    obj["PKEY"] = pKey;
                    obj["SKEY"] = sKey;

                    jsonObject[id] = obj;

                }

            }

            jsonObject["COUNT"] = count;

            int minKey = int.Parse(dataExHentai.Compute("MIN(PKEY)", "").ToString());
            int maxKey = int.Parse(dataExHentai.Compute("MAX(PKEY)", "").ToString());
            if (dataExHentai.Rows.Count > 0 &&
             (
                int.Parse(lastPkey) >= maxKey || int.Parse(lastPkey) >= minKey
              )
            )
            {
                if (param == null)
                {
                    tmp = int.Parse(procPage);
                    if (tmp < int.Parse(webRow["TOTAL_PAGE"].ToString()))
                    {
                        tmp++;

                    }

                    string p =
                        System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("WatermarkPeriod") ?
                        System.Configuration.ConfigurationManager.AppSettings["WatermarkPeriod"] :
                        "0";

                    int period = Convert.ToInt32(Math.Pow(10, int.Parse(p)));
                    int maxPoint = minKey + period;
                    int lastKey = maxPoint;

                    Central.logger.WriteInfo(string.Format("maxKey = {0}", maxKey));
                    Central.logger.WriteInfo(string.Format("Period = {0}", period));
                    Central.logger.WriteInfo(string.Format("maxPoint = {0}", maxPoint));
                    Central.logger.WriteInfo(string.Format("lastKey = {0}", lastKey));

                    webRow["PROC_PAGE"] = tmp;
                    webRow["LAST_KEY"] = lastKey.ToString();
                    webRow["UPDATE_TIME"] = DateTime.Now;

                }

            }

            if (DateTime.Parse(webRow["UPDATE_TIME"].ToString()) < DateTime.Now.AddHours(-1))
            {
                webRow["LAST_KEY"] = "9999999";
                webRow["UPDATE_TIME"] = DateTime.Now;

            }

            this.UpdateDataExHentai(dataExHentai);
            this.UpdateObject(dataObject);
            this.UpdateStatus(dataWeb);

            ViewData["DATA"] = jsonObject.ToString();

            return View();

        }

        //取得 ExHentai Hidden 書籍詳細資訊
        public ActionResult GET_EXHENTAI_HIDDEN_GALLERY_DETAIL(string param)
        {
            string main = "EXHENTAI";
            string type = "NON-H_HIDDEN";
            string key = string.Format("{0}_{1}", main, type);
            int tmp = -1;

            string url = "";
            DataTable dataWeb = this.GetStatus(key);

            if (dataWeb.Rows.Count == 0)
            {
                ViewData["DATA"] = "資料錯誤";

                return View();

            }

            DataRow webRow = dataWeb.Rows[0];
            string basePage = webRow["BASE"].ToString();
            String pathPattern = webRow["PATH"].ToString();
            string lastPkey = webRow["LAST_KEY"].ToString();
            string procPage = param == null || !int.TryParse(param, out tmp) ? webRow["PROC_PAGE"].ToString() : param;
            url = string.Format(pathPattern, lastPkey);

            #region Process_Web
            Central.logger = new Logger();

            if (Central.authUser == null)
            {
                Central.authUser = this.Login("username", "password");

            }

            PAGE_INFO pageInfo = new PAGE_INFO(url);

            List<THREAD_INFO> threads = GetThreadListFromPage(pageInfo.page_url, Central.authUser);

            #endregion Process_Web

            DataTable dataExHentai = this.GetDataExHentai("-1", "-1");
            DataTable dataObject = this.GetObject("-1");

            int count = 0;
            JObject jsonObject = new JObject();
            jsonObject["URL"] = url;
            jsonObject["COUNT"] = 0;

            foreach (THREAD_INFO thread in threads)
            {
                string regex = basePage + @"/g/(\w+)/(\w+)/";
                string threadUrl = thread.thread_url;

                Match m = new Regex(regex).Match(threadUrl);

                string pKey = m.Groups[1].Value;
                string sKey = m.Groups[2].Value;
                string id = string.Format("EX_{0}_{1}", pKey, sKey);

                if (dataExHentai.Select(string.Format("OBJ_ID = '{0}'", id)).Length > 0)
                {
                    continue;

                }

                DataTable mainTable = this.GetDataExHentai(pKey, sKey);
                DataRow drMain = null;
                DataRow drKey = null;
                if (mainTable.Rows.Count == 0)
                {
                    #region ExHentai
                    drMain = dataExHentai.NewRow();

                    drMain["OBJ_ID"] = id;
                    drMain["PKEY"] = pKey;
                    drMain["SKEY"] = sKey;
                    drMain["CREATE_TIME"] = DateTime.Now;
                    drMain["UPDATE_TIME"] = DateTime.Now;

                    dataExHentai.Rows.Add(drMain);

                    #endregion ExHentai

                    #region Object
                    drKey = dataObject.NewRow();

                    drKey["WEB_ID"] = "EXHENTAI";
                    drKey["ID"] = id;
                    drKey["STATUS"] = "0";
                    drKey["CREATE_USER"] = "ADMIN";
                    drKey["CREATE_TIME"] = DateTime.Now;
                    drKey["UPDATE_USER"] = "ADMIN";
                    drKey["UPDATE_TIME"] = DateTime.Now;

                    dataObject.Rows.Add(drKey);

                    #endregion Object

                    JObject obj = new JObject();
                    obj["TYPE"] = "ADD";
                    obj["PKEY"] = pKey;
                    obj["SKEY"] = sKey;
                    jsonObject[id] = obj;

                    count++;

                }
                else
                {
                    dataExHentai.ImportRow(mainTable.Rows[0]);

                    JObject obj = new JObject();
                    obj["TYPE"] = "EXIST";
                    obj["PKEY"] = pKey;
                    obj["SKEY"] = sKey;

                    jsonObject[id] = obj;

                }

            }

            jsonObject["COUNT"] = count;

            int minKey = int.Parse(dataExHentai.Compute("MIN(PKEY)", "").ToString());
            int maxKey = int.Parse(dataExHentai.Compute("MAX(PKEY)", "").ToString());
            if (dataExHentai.Rows.Count > 0 &&
             (
                int.Parse(lastPkey) >= maxKey || int.Parse(lastPkey) >= minKey
              )
            )
            {
                if (param == null)
                {
                    tmp = int.Parse(procPage);
                    if (tmp < int.Parse(webRow["TOTAL_PAGE"].ToString()))
                    {
                        tmp++;

                    }

                    string p =
                        System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("WatermarkPeriod") ?
                        System.Configuration.ConfigurationManager.AppSettings["WatermarkPeriod"] :
                        "0";

                    int period = Convert.ToInt32(Math.Pow(10, int.Parse(p)));
                    int maxPoint = minKey + period;
                    int lastKey = maxPoint;

                    Central.logger.WriteInfo(string.Format("maxKey = {0}", maxKey));
                    Central.logger.WriteInfo(string.Format("Period = {0}", period));
                    Central.logger.WriteInfo(string.Format("maxPoint = {0}", maxPoint));
                    Central.logger.WriteInfo(string.Format("lastKey = {0}", lastKey));

                    webRow["PROC_PAGE"] = tmp;
                    webRow["LAST_KEY"] = lastKey.ToString();
                    webRow["UPDATE_TIME"] = DateTime.Now;

                }

            }

            if (DateTime.Parse(webRow["UPDATE_TIME"].ToString()) < DateTime.Now.AddHours(-1))
            {
                webRow["LAST_KEY"] = "9999999";
                webRow["UPDATE_TIME"] = DateTime.Now;

            }

            this.UpdateDataExHentai(dataExHentai);
            this.UpdateObject(dataObject);
            this.UpdateStatus(dataWeb);

            ViewData["DATA"] = jsonObject.ToString();

            return View();

        }

        //取得 ExHentai API 書籍資訊
        public ActionResult GET_EXHENTAI_API_DETAIL(string param)
        {
            int maxRow = 20;

            JArray bookArr = new JArray();
            Regex regex = new Regex(@"EX_(\w+)_(\w+)");

            DataTable dataObject = null;
            if (param == null)
            {
                dataObject = this.GetObject(null);
                int i = 0;

                foreach (DataRow rowObject in dataObject.Rows)
                {
                    string pKey = regex.Match(rowObject["ID"].ToString()).Groups[1].Value;
                    string sKey = regex.Match(rowObject["ID"].ToString()).Groups[2].Value;

                    JArray sub = new JArray();
                    sub.Add(int.Parse(pKey));
                    sub.Add(sKey);
                    bookArr.Add(sub);
                    if (rowObject["STATUS"].ToString() != "2")
                    {
                        rowObject["STATUS"] = "1";

                    }

                    rowObject["UPDATE_USER"] = "API";
                    rowObject["UPDATE_TIME"] = DateTime.Now;

                    i++;
                    if (i == maxRow)
                    {
                        break;

                    }

                }

            }
            else
            {
                string id = string.Format("EX_{0}", param);
                dataObject = this.GetObject(id);

                DataRow rowObject = dataObject.Rows[0];
                string pKey = regex.Match(rowObject["ID"].ToString()).Groups[1].Value;
                string sKey = regex.Match(rowObject["ID"].ToString()).Groups[2].Value;

                JArray sub = new JArray();
                sub.Add(int.Parse(pKey));
                sub.Add(sKey);
                bookArr.Add(sub);

                if (rowObject["STATUS"].ToString() != "2")
                {
                    rowObject["STATUS"] = "1";

                }

                rowObject["UPDATE_USER"] = "API";
                rowObject["UPDATE_TIME"] = DateTime.Now;

            }

            this.UpdateObject(dataObject);

            #region QUERY_API
            Central.logger = new Logger();

            JObject queryObj = new JObject();
            queryObj.Add("gidlist", bookArr);
            queryObj.Add("method", "gdata");
            queryObj.Add("namespace", 1);

            string apiUrl = "https://api.e-hentai.org/api.php";
            HTML html = new HTML(apiUrl);
            html.postData = Encoding.UTF8.GetBytes(queryObj.ToString());
            html.GetHtml();
            string jsonStr = html.webHtml;

            JObject res = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonStr) as JObject;

            #endregion QUERY_API

            DataTable dataExHentai = this.GetDataExHentai("-1", "-1");
            DataTable dataExHentaiDetail = this.GetDataExHentaiDetail("-1", "-1");

            JObject jsonObject = new JObject();

            JArray resArr = res["gmetadata"] as JArray;
            if (resArr == null)
            {
                resArr = new JArray();

            }
            foreach (JObject resObj in resArr)
            {
                if (resObj.ContainsKey("error"))
                {
                    continue;
                }

                string pkey = resObj["gid"].ToString();
                string sKey = resObj["token"].ToString();
                string title = resObj["title"].ToString();
                string title_jpn = resObj["title_jpn"].ToString();
                string thumb = resObj["thumb"].ToString();
                string uploader = resObj["uploader"].ToString();
                DateTime posted = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)
                    .AddSeconds(long.Parse(resObj["posted"].ToString())).ToLocalTime();
                int filecount = int.Parse(resObj["filecount"].ToString());

                DataTable subExHentai = this.GetDataExHentai(pkey, sKey);
                DataRow drExHentai = null;
                DataTable subExHentaiDetail = this.GetDataExHentaiDetail(pkey, sKey);
                DataRow drExHentaiDetail = null;

                if (subExHentai.Rows.Count == 0)
                {
                    drExHentai = dataExHentai.NewRow();

                    drExHentai["OBJ_ID"] = string.Format("EX_{0}_{1}", pkey, sKey);
                    drExHentai["PKEY"] = pkey;
                    drExHentai["SKEY"] = sKey;
                    drExHentai["RELEASE_TIME"] = posted.ToString("yyyy-MM-dd HH:mm:ss");
                    drExHentai["CREATE_TIME"] = DateTime.Now;
                    drExHentai["UPDATE_TIME"] = DateTime.Now;

                    dataExHentai.Rows.Add(drExHentai);

                }
                else
                {
                    drExHentai = subExHentai.Rows[0];

                    drExHentai["RELEASE_TIME"] = posted.ToString("yyyy-MM-dd HH:mm:ss");
                    drExHentai["UPDATE_TIME"] = DateTime.Now;

                }

                if (subExHentaiDetail.Rows.Count == 0)
                {
                    drExHentaiDetail = dataExHentaiDetail.NewRow();
                    drExHentaiDetail["PKEY"] = pkey;
                    drExHentaiDetail["SKEY"] = sKey;

                    dataExHentaiDetail.Rows.Add(drExHentaiDetail);

                    JObject obj = new JObject();
                    obj.Add("TYPE", "ADD");
                    obj.Add("PKEY", pkey);
                    obj.Add("SKEY", sKey);

                    jsonObject[pkey] = obj;

                }
                else
                {
                    drExHentaiDetail = subExHentaiDetail.Rows[0];

                    JObject obj = new JObject();
                    obj.Add("TYPE", "EDIT");
                    obj.Add("PKEY", pkey);
                    obj.Add("SKEY", sKey);

                    jsonObject[pkey] = obj;

                }

                drExHentaiDetail["NAME_ENG"] = title;
                drExHentaiDetail["NAME_UNI"] = title_jpn;
                drExHentaiDetail["PAGE"] = filecount;
                drExHentaiDetail["PIC_THUMB"] = thumb;
                drExHentaiDetail["USER_ID"] = "API";
                drExHentaiDetail["UPDATE_TIME"] = DateTime.Now;

                if (subExHentaiDetail.Rows.Count != 0)
                {
                    dataExHentaiDetail.ImportRow(drExHentaiDetail);

                }

            }

            this.UpdateDataExHentaiDetail(dataExHentaiDetail);
            this.UpdateDataExHentai(dataExHentai);
            this.UpdateObject(dataObject);

            ViewData["DATA"] = jsonObject.ToString();

            return View();

        }

        public ActionResult GET_EXHENTAI_DATA_DETAIL(string param)
        {
            DataTable dataObject = null;
            string id = "";

            if (param == null)
            {
                dataObject = this.GetObject(param);

                id = dataObject.Rows[0]["ID"].ToString();

            }
            else if (param != "")
            {
                id = string.Format("EX_{0}", param);

                dataObject = this.GetObject(id);

            }

            if (dataObject.Rows[0]["STATUS"].ToString() != "2")
            {
                dataObject.Rows[0]["STATUS"] = "1";

            }
            dataObject.Rows[0]["UPDATE_USER"] = "WEB";
            dataObject.Rows[0]["UPDATE_TIME"] = DateTime.Now;

            this.UpdateObject(dataObject);

            Regex regex = new Regex(@"EX_(\w+)_(\w+)");

            string pKey = regex.Match(id).Groups[1].Value;
            string sKey = regex.Match(id).Groups[2].Value;

            string url = "https://exhentai.org/g/{0}/{1}/";
            url = string.Format(url, pKey, sKey);

            #region JOB_THREAD
            Central.logger = new Logger();
            Central.PathDirector = new FileDirector();

            if (Central.authUser == null)
            {
                Central.authUser = this.Login("username", "password");

            }

            THREAD_INFO threadInfo = new THREAD_INFO(url);

            string baseUrl = "https://exhentai.org";

            #endregion JOB_THREAD

            DataTable dataExHentai = this.GetDataExHentai(pKey, sKey);
            DataRow drExHentai = null;
            DataTable dataExHentaiDetail = this.GetDataExHentaiDetail(pKey, sKey);
            DataRow drExHentaiDetail = null;

            if (dataExHentai.Rows.Count == 0)
            {
                drExHentai = dataExHentai.NewRow();

                drExHentai["OBJ_ID"] = id;
                drExHentai["PKEY"] = pKey;
                drExHentai["SKEY"] = sKey;
                drExHentai["CREATE_TIME"] = DateTime.Now;
                drExHentai["UPDATE_TIME"] = DateTime.Now;

                dataExHentai.Rows.Add(drExHentai);

            }
            else
            {
                drExHentai = dataExHentai.Rows[0];

                drExHentai["UPDATE_TIME"] = DateTime.Now;

            }

            JObject jsonObject = new JObject();

            if (dataExHentaiDetail.Rows.Count == 0)
            {
                drExHentaiDetail = dataExHentaiDetail.NewRow();
                drExHentaiDetail["PKEY"] = pKey;
                drExHentaiDetail["SKEY"] = sKey;

                jsonObject["TYPE"] = "ADD";
                jsonObject["PKEY"] = pKey;
                jsonObject["SKEY"] = sKey;

            }
            else
            {
                drExHentaiDetail = dataExHentaiDetail.Rows[0];

                jsonObject["TYPE"] = "EDIT";
                jsonObject["PKEY"] = pKey;
                jsonObject["SKEY"] = sKey;

            }

            drExHentaiDetail["USER_ID"] = "WEB";
            drExHentaiDetail["UPDATE_TIME"] = DateTime.Now;

            if (dataExHentaiDetail.Rows.Count == 0)
            {
                dataExHentaiDetail.Rows.Add(drExHentaiDetail);

            }

            dataObject.Rows[0]["STATUS"] = "2";
            dataObject.Rows[0]["UPDATE_USER"] = "WEB";
            dataObject.Rows[0]["UPDATE_TIME"] = DateTime.Now;

            this.UpdateDataExHentaiDetail(dataExHentaiDetail);
            this.UpdateDataExHentai(dataExHentai);
            this.UpdateObject(dataObject);

            ViewData["DATA"] = jsonObject.ToString();

            return View();

        }

        public ActionResult GET_EXHENTAI_DATAALL_DETAIL(string param)
        {
            DataTable dataObject = null;
            JObject jsonObject = new JObject();

            string id = "";
            int selRow = -1;

            if (param == null)
            {
                int maxRandRow = 20;

                Random rand = new Random(int.Parse((DateTime.Now.Ticks % Math.Pow(10, 8)).ToString()));
                selRow = rand.Next(0, maxRandRow);
                jsonObject["SEL"] = selRow;

                dataObject = this.GetObject(param);

                if (dataObject.Rows.Count == 0)
                {
                    jsonObject["ERROR"] = "DATA EMPTY";
                    ViewData["DATA"] = jsonObject.ToString();
                    return View();

                }

                if (dataObject.Rows.Count < selRow)
                {
                    selRow = 0;

                }

                id = dataObject.Rows[selRow]["ID"].ToString();

            }
            else if (param != "")
            {
                selRow = 0;

                id = string.Format("EX_{0}", param);

                dataObject = this.GetObject(id);

            }
            try
            {
                if (dataObject.Rows[selRow]["STATUS"].ToString() != "2")
                {
                    dataObject.Rows[selRow]["STATUS"] = "1";

                }
                dataObject.Rows[selRow]["UPDATE_USER"] = "DATAALL";
                dataObject.Rows[selRow]["UPDATE_TIME"] = DateTime.Now;

                this.UpdateObject(dataObject);

                Regex regex = new Regex(@"EX_(\w+)_(\w+)");

                string pKey = regex.Match(id).Groups[1].Value;
                string sKey = regex.Match(id).Groups[2].Value;

                string url = "https://exhentai.org/g/{0}/{1}/";
                url = string.Format(url, pKey, sKey);

                #region QUERY_API
                Central.logger = new Logger();

                JObject queryObj = new JObject();

                JArray bookArr = new JArray();
                JArray sub = new JArray();
                sub.Add(int.Parse(pKey));
                sub.Add(sKey);
                bookArr.Add(sub);

                queryObj.Add("gidlist", bookArr);
                queryObj.Add("method", "gdata");
                queryObj.Add("namespace", 1);

                string apiUrl = "https://api.e-hentai.org/api.php";
                HTML html = new HTML(apiUrl);
                html.postData = Encoding.UTF8.GetBytes(queryObj.ToString());
                html.GetHtml();
                string jsonStr = html.webHtml;

                JObject res = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonStr) as JObject;

                #endregion QUERY_API

                DataTable dataExHentai = this.GetDataExHentai(pKey, sKey);
                DataRow drExHentai = null;
                DataTable dataExHentaiDetail = this.GetDataExHentaiDetail(pKey, sKey);
                DataRow drExHentaiDetail = null;

                if (dataExHentai.Rows.Count == 0)
                {
                    drExHentai = dataExHentai.NewRow();

                    drExHentai["OBJ_ID"] = id;
                    drExHentai["PKEY"] = pKey;
                    drExHentai["SKEY"] = sKey;
                    drExHentai["CREATE_TIME"] = DateTime.Now;
                    drExHentai["UPDATE_TIME"] = DateTime.Now;

                    dataExHentai.Rows.Add(drExHentai);
                }
                else
                {
                    drExHentai = dataExHentai.Rows[0];

                    drExHentai["UPDATE_TIME"] = DateTime.Now;

                }

                if (dataExHentaiDetail.Rows.Count == 0)
                {
                    drExHentaiDetail = dataExHentaiDetail.NewRow();
                    drExHentaiDetail["PKEY"] = pKey;
                    drExHentaiDetail["SKEY"] = sKey;

                    jsonObject["TYPE"] = "ADD";
                    jsonObject["PKEY"] = pKey;
                    jsonObject["SKEY"] = sKey;

                }
                else
                {
                    drExHentaiDetail = dataExHentaiDetail.Rows[0];

                    jsonObject["TYPE"] = "EDIT";
                    jsonObject["PKEY"] = pKey;
                    jsonObject["SKEY"] = sKey;

                }

                drExHentaiDetail["USER_ID"] = "DATAALL";
                drExHentaiDetail["UPDATE_TIME"] = DateTime.Now;

                jsonObject["NAME"] = drExHentaiDetail["NAME_UNI"] != null ? drExHentaiDetail["NAME_UNI"].ToString() : "";
                if (dataExHentaiDetail.Rows.Count == 0)
                {
                    dataExHentaiDetail.Rows.Add(drExHentaiDetail);

                }

                dataObject.Rows[selRow]["STATUS"] = "2";
                dataObject.Rows[selRow]["UPDATE_USER"] = "DATAALL";
                dataObject.Rows[selRow]["UPDATE_TIME"] = DateTime.Now;

                this.UpdateDataExHentaiDetail(dataExHentaiDetail);
                this.UpdateDataExHentai(dataExHentai);
                this.UpdateObject(dataObject);

                ViewData["DATA"] = jsonObject.ToString();

            }
            catch (Exception ex)
            {
                dataObject.Rows[selRow]["STATUS"] = "9";

                dataObject.Rows[selRow]["UPDATE_USER"] = "CRAW";
                dataObject.Rows[selRow]["UPDATE_TIME"] = DateTime.Now;

                this.UpdateObject(dataObject);

                jsonObject["ERROR"] = string.Format("ID = {0} ERROR , Message = {1}", dataObject.Rows[selRow]["ID"], ex.Message);
                ViewData["DATA"] = jsonObject.ToString();

            }

            return View();

        }

        // Helper methods ported from JOB_* classes (inlined into controller)
        private string FetchWeb(string url, AUTH auth = null, int retry = 3)
        {
            AUTH au = auth ?? Central.authUser;
            HTML web = new HTML();
            web.webUrl = url;
            if (au != null)
            {
                web.webCookie = au.loginCookie;
                web.cookieContainer = au.loginContainer;
            }

            string html = "";
            for (int i = 0; i < retry; i++)
            {
                web.GetHtml();
                html = web.webHtml;
                if (!string.IsNullOrEmpty(html) && html.ToLower().Contains("html"))
                {
                    break;
                }
            }

            return html;
        }

        private List<PAGE_INFO> ProcessSite(SITE_INFO siteInfo, AUTH auth)
        {
            try
            {
                string html = FetchWeb(siteInfo.site_url, auth);
                if (siteInfo.site_url.Contains("exhentai"))
                {
                    html = FetchWeb(siteInfo.site_url, auth);
                }

                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(html);
                HtmlNode node = document.DocumentNode;

                string xpath = "/html[1]/body[1]/div[2]/div[2]/table[1]/tr[1]";
                HtmlNode tmpNode = node.SelectSingleNode(xpath);
                List<PAGE_INFO> pages = new List<PAGE_INFO>();
                if (tmpNode != null)
                {
                    siteInfo.page_info_count = int.Parse(tmpNode.ChildNodes[tmpNode.ChildNodes.Count - 2].InnerText);

                    string page = "/{0}";
                    for (int i = 0; i < siteInfo.page_info_count; i++)
                    {
                        PAGE_INFO pageInfo = new PAGE_INFO(siteInfo.site_url + string.Format(page, i));
                        pageInfo.site_url = siteInfo.site_url;
                        pages.Add(pageInfo);
                    }
                }

                return pages;
            }
            catch (Exception ex)
            {
                Central.logger.WriteError($"ProcessSite error: {ex}");
                return new List<PAGE_INFO>();
            }
        }

        private List<THREAD_INFO> GetThreadListFromPage(string pageUrl, AUTH auth)
        {
            List<THREAD_INFO> threadList = new List<THREAD_INFO>();
            try
            {
                string html = FetchWeb(pageUrl, auth);
                if (pageUrl.Contains("exhentai"))
                {
                    html = FetchWeb(pageUrl, auth);
                }

                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(html);
                HtmlNode rootNode = document.DocumentNode;

                string tableXPath = "/html[1]/body[1]/div[2]/div[2]/table[1]";
                var rows = rootNode.SelectNodes("//tr");
                if (rows != null)
                {
                    foreach (HtmlNode row in rows)
                    {
                        if (!row.XPath.StartsWith(tableXPath) || row.XPath == tableXPath + "/tr[1]")
                        {
                            continue;
                        }

                        string appendPath = "/td[3]/a[1]";
                        HtmlNode tmpNode = row.SelectSingleNode(row.XPath + appendPath);
                        if (tmpNode != null)
                        {
                            string keyUrl = tmpNode.Attributes["href"].Value;
                            THREAD_INFO ti = new THREAD_INFO(keyUrl);
                            ti.page_url = pageUrl;
                            threadList.Add(ti);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Central.logger.WriteError($"GetThreadListFromPage error: {ex}");
            }

            return threadList;
        }

    }

}