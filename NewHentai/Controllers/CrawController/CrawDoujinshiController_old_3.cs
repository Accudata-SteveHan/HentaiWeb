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
    public class CrawDoujinshiController_old_3 : Controller
    {
        private DataControlMdf control = new DataControlMdf();
        private string conStrMain = @"Data Source={0}\SQLEXPRESS;DATABASE=HENTAI;Integrated Security=false;User ID=sa;PASSWORD=c06g4cl6;Connection Timeout=180";
        private string conStrLib = @"Data Source={0}\SQLEXPRESS;DATABASE=ANIMELIBRARY;Integrated Security=false;User ID=sa;PASSWORD=c06g4cl6;Connection Timeout=180";
        private string conStrPic = @"Data Source={0}\SQLEXPRESS;DATABASE=HENTAIPIC;Integrated Security=false;User ID=sa;PASSWORD=c06g4cl6;Connection Timeout=180";
        private string conStrBook = @"Data Source={0}\SQLEXPRESS;DATABASE=MELONBOOK;Integrated Security=false;User ID=sa;PASSWORD=c06g4cl6;Connection Timeout=600";

        public CrawDoujinshiController_old_3()
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
            string type = "DOUJINSHI";
            string key = string.Format("{0}_{1}", main, type);
            string basePage = "https://exhentai.org";

            //DataTable data = this.GetWeb(key, "");
            DataTable data = this.GetStatus(key);

            string url = data.Rows[0]["PATH"].ToString();

            #region Process_Site
            Central.logger = new Logger();
            //Central.jobControl = new JOB_CONTROL();
            //Central.jobDownload = new JOB_CONTROL();

            if (Central.authUser == null)
            {
                Central.authUser = this.Login("username", "password");

            }

            SITE_INFO site = new SITE_INFO(url);
            // ProcessSite returns page list
            List<PAGE_INFO> pageList = ProcessSite(site, Central.authUser);

            #endregion Process_Site

            int pageCount = (pageList == null) ? 0 : pageList.Count;
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
            string type = "DOUJINSHI";
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

            DataRow webRow = dataWeb.Rows[0];
            string basePage = webRow["BASE"].ToString();
            String pathPattern = webRow["PATH"].ToString();
            string lastPkey = webRow["LAST_KEY"].ToString();
            string procPage = param == null || !int.TryParse(param, out tmp) ? webRow["PROC_PAGE"].ToString() : param;
            //url = string.Format(pathPattern, procPage);
            url = string.Format(pathPattern, lastPkey);

            #region Process_Web
            Central.logger = new Logger();
            //Central.jobControl = new JOB_CONTROL();
            //Central.jobDownload = new JOB_CONTROL();

            if (Central.authUser == null)
            {
                Central.authUser = this.Login("username", "password");

            }

            PAGE_INFO pageInfo = new PAGE_INFO(url);

            // GetThreadListFromPage returns the thread url list
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

                    if (!jsonObject.ContainsKey(id))
                    {
                        jsonObject[id] = obj;

                    }
                    else
                    {
                        jsonObject[id] = obj;

                    }

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