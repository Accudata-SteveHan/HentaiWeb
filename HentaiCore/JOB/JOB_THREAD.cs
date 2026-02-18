using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

using HtmlAgilityPack;
using HentaiCore;

namespace HentaiCore {
    //列表的工作
    public class JOB_THREAD {
        THREAD_INFO thread_info = null;
        public BOOK_INFO book_info = null;

        // --- JOB (original) fields moved here ---
        public AUTH authUser = null;
        public string domain = "";
        HTML web = null;

        protected Logger logger = null;

        int retry = 3;

        // --- Crawler-derived fields used by this JOB_THREAD ---
        private HtmlAgilityPack.HtmlDocument document = null;
        private HtmlNode node = null;
        private string msg = "";

        public string html = "";
        public string mainUrl = "";
        public string pKey = "", sKey = "";

        #region Path
        private string strGetTypePath = "/html[1]/body[1]/div[2]/div[3]/div[1]/div[1]/div[1]";
        private string strGetUnloadTimePath = "/html[1]/body[1]/div[2]/div[3]/div[1]/div[3]/table[1]/tr[1]/td[2]";
        private string strGetEngTitlePath = "/html[1]/body[1]/div[2]/div[2]/h1[1]";
        private string strGetUniTitlePath = "/html[1]/body[1]/div[2]/div[2]/h1[2]";
        private string strGetPostTimePath = "/html[1]/body[1]/div[2]/div[3]/div[1]/div[3]/table[1]/tr[1]/td[2]";
        private string strGetLanguagePath = "/html[1]/body[1]/div[2]/div[3]/div[1]/div[3]/table[1]/tr[4]/td[2]";
        private string strGetTotalPagePath = "/html[1]/body[1]/div[2]/div[3]/div[1]/div[3]/table[1]/tr[6]/td[2]";
        private string strGetRateTimePath = "/html[1]/body[1]/div[2]/div[3]/div[1]/div[4]/table[1]/tr[1]/td[3]/span[1]";
        private string strGetRateAvgPath = "/html[1]/body[1]/div[2]/div[3]/div[1]/div[4]/table[1]/tr[2]/td[1]";
        private string strGetTabCountPath1 = "/html[1]/body[1]/div[3]/table[1]/tr[1]";
        private string strGetTabCountPath2 = "/html[1]/body[1]/div[4]/table[1]/tr[1]";
        private string strGetTabCountPath3 = "/html[1]/body[1]/div[5]/table[1]/tr[1]";
        private string strGetTabCountPath4 = "/html[1]/body[1]/div[6]/table[1]/tr[1]";
        #endregion

        // --- JOB (original) methods moved here ---
        private void InitWebConfig(string url) {
            web.webUrl = url;
            var au = this.authUser ?? Central.authUser;
            if (au != null) {
                web.webCookie = au.loginCookie;
                web.cookieContainer = au.loginContainer;
            }
        }

        protected string GetWeb(string url){
            int i = 0;
            if (this.logger != null) this.logger.Write(this, string.Format("開始 JOB GetWeb = {0}" , url));
            for (i = 0 ; i < retry ; i++) {
                if (this.logger != null) this.logger.WriteTrace(string.Format("GetWeb : {0} URL = {1}", i, url));
                this.InitWebConfig(url);
                web.GetHtml();

                if (web.webHtml != null && web.webHtml.ToLower().Contains("html")) {
                    break;
                }

                if (this.logger != null) this.logger.WriteTrace(string.Format("GetWeb Fail : {0} URL = {1}", i, url));
            }

            if (this.logger != null) this.logger.Write(this, string.Format("完成 JOB GetWeb = {0}", url));
            return web.webHtml;
        }

        protected Stream GetStream(string url) {
            int i = 0;
            if (this.logger != null) this.logger.Write(this, string.Format("開始 JOB GetStream = {0}", url));

            for (i = 0 ; i < retry ; i++) {
                try {
                    this.InitWebConfig(url);
                    web.GetStream();
                    break;
                } catch (Exception ex) {
                    if (this.logger != null) this.logger.WriteTrace(string.Format("GetStream Fail : {0} URL = {1}", i, url));
                }
            }

            if (this.logger != null) this.logger.Write(this, string.Format("完成 JOB GetStream = {0}", url));
            return web.webStream;
        }

        private HtmlNode GetNode(string nodePath) {
            this.msg = "XPATH = {0}";
            if (this.logger != null) this.logger.WriteLog(string.Format(msg, nodePath));

            if (this.node == null) return null;
            HtmlNode tmpNode = node.SelectSingleNode(nodePath);
            return tmpNode;
        }

        private void ErrorLog(Exception ex) {
            if (this.logger != null) {
                this.logger.WriteError(ex.ToString());
                this.logger.WriteError(ex.StackTrace);
            }
        }

        private void InitializeCrawler(string htmlStr) {
            this.html = htmlStr ?? "";
            this.document = new HtmlAgilityPack.HtmlDocument();
            this.document.LoadHtml(this.html);
            this.node = document.DocumentNode;

            if (this.logger == null) {
                this.logger = Central.logger;
            }
        }

        public JOB_THREAD(THREAD_INFO threadInfo) {
            this.thread_info = threadInfo;
            this.book_info = new BOOK_INFO();

            // initialize web & logger (original JOB ctor)
            this.web = new HTML();
            this.logger = Central.logger;
        }

        public void GetBookInfo() {
            try {
                if (this.logger != null) this.logger.WriteInfo(string.Format("開始處理文件內容 : {0}", this.thread_info.thread_url));

                //取得 Web
                string webStr = this.GetWeb(this.thread_info.thread_url);

                string pKey = this.thread_info.thread_url.Replace(this.domain + "/g/", "").Split('/')[0];
                string sKey = this.thread_info.thread_url.Replace(this.domain + "/g/", "").Split('/')[1];

                // Initialize crawler-like state based on the loaded HTML
                this.InitializeCrawler(webStr);
                this.mainUrl = this.domain;
                this.pKey = pKey;
                this.sKey = sKey;

                //產生 Book Info
                this.book_info = new BOOK_INFO(
                    this.thread_info.thread_url,
                    pKey, sKey
                    );

                string par = "";
                decimal tmpDec;

                if (this.logger != null) this.logger.Write(this, string.Format("處理 Page Url : {0}", this.thread_info.thread_url));
                //Page Url
                par = this.thread_info.page_url;
                this.book_info.pageUrl = par != "" ? par : "";

                if (this.logger != null) this.logger.Write(this, string.Format("處理 P_Key : {0}", this.thread_info.thread_url));
                //P_Key
                par = pKey;
                this.book_info.p_key = par != "" ? par : "";

                if (this.logger != null) this.logger.Write(this, string.Format("處理 S_Key : {0}", this.thread_info.thread_url));
                //S_Key
                par = sKey;
                this.book_info.s_key = par != "" ? par : "";

                if (this.logger != null) this.logger.Write(this, string.Format("處理 EngTitle : {0}", this.thread_info.thread_url));
                //EngTitle
                par = this.Get_Eng_Title();
                this.book_info.eng_title = par != "" ? par : "";

                if (this.logger != null) this.logger.Write(this, string.Format("處理 UniTitle : {0}", this.thread_info.thread_url));
                //UniTitle
                par = this.Get_Uni_Title();
                this.book_info.uni_title = par != "" ? par : "";

                if (this.logger != null) this.logger.Write(this, string.Format("處理 Language : {0}", this.thread_info.thread_url));
                //Language
                par = this.Get_Language();
                this.book_info.language = par != "" ? par : "";

                if (this.logger != null) this.logger.Write(this, string.Format("處理 Type : {0}", this.thread_info.thread_url));
                //Type
                par = this.Get_Type();
                this.book_info.type = par != "" ? par : "";

                if (this.logger != null) this.logger.Write(this, string.Format("處理 TotalPage : {0}", this.thread_info.thread_url));
                //TotalPage
                par = this.Get_Total_Page();
                this.book_info.total_page = par != "" ? int.Parse(par) : 0;

                if (this.logger != null) this.logger.Write(this, string.Format("處理 TabCount : {0}", this.thread_info.thread_url));
                //TabCount
                par = this.Get_Tab_Count();
                this.book_info.tab_count = par != "" ? int.Parse(par) : 0;

                if (this.logger != null) this.logger.Write(this, string.Format("處理 UploadTime : {0}", this.thread_info.thread_url));
                //UploadTime
                par = this.Get_Upload_Time();
                book_info.upload_time = par != "" ? par : "";

                if (this.logger != null) this.logger.Write(this, string.Format("處理 PostedTime : {0}", this.thread_info.thread_url));
                //PostedTime
                par = this.Get_Posted_Time();
                this.book_info.posted_time = par != "" ? par : "";

                if (this.logger != null) this.logger.Write(this, string.Format("處理 RateTime : {0}", this.thread_info.thread_url));
                //RateTime
                par = this.Get_Rate_Time();
                this.book_info.rate_time = par != "" && decimal.TryParse(par, out tmpDec) ? tmpDec : 0;

                if (this.logger != null) this.logger.Write(this, string.Format("處理 RateAvg : {0}", this.thread_info.thread_url));
                //RateAvg
                par = this.Get_Rate_Avg();
                this.book_info.rate_avg = par != "" && decimal.TryParse(par, out tmpDec) ? tmpDec : 0;

                if (this.logger != null) this.logger.Write(this, string.Format("處理 Cover : {0}", this.thread_info.thread_url));
                //Cover
                par = this.Get_Cover();
                this.book_info.pic = par != "" ? par : "";

                if (this.logger != null) this.logger.Write(this, string.Format("處理 Cover Stream : {0}", this.thread_info.thread_url));
                //Cover Stream
                if (this.book_info.pic != "") {
                    HTML picHtml = new HTML(this.book_info.pic);
                    picHtml.webCookie = (this.authUser ?? Central.authUser).loginCookie;
                    picHtml.cookieContainer = (this.authUser ?? Central.authUser).loginContainer;
                    picHtml.GetStream();

                    this.book_info.pic_Stream = picHtml.webStream;
                }

            } catch (Exception ex) {
                if (this.logger != null) this.logger.WriteError(string.Format("JOB_THREAD.GetBookInfo 錯誤，{0}", ex.ToString()));
            }
        }

        // --- Methods ported from Crawler.cs ---

        public List<string> RollList() {
            if (this.logger != null) this.logger.WriteInfo("處理文章列表");
            string tableXPath = "/html[1]/body[1]/div[2]/div[2]/table[1]";

            HtmlNode rootNode = document.DocumentNode;
            List<string> threadList = new List<string>();

            if (this.logger != null) this.logger.Write(this, "處理文章列表的標籤");
            var rows = rootNode.SelectNodes("//tr");
            if (rows == null) rows = new HtmlNodeCollection(rootNode);
            foreach (HtmlNode row in rows) {
                if (!row.XPath.StartsWith(tableXPath) || row.XPath == tableXPath + "/tr[1]") {
                    continue;
                }

                string appendPath = "/td[3]/a[1]";
                HtmlNode tmpNode = row.SelectSingleNode(row.XPath + appendPath);
                if (tmpNode != null) {
                    string keyUrl = tmpNode.Attributes["href"].Value;

                    //log
                    if (this.logger != null) this.logger.WriteTrace(
                        string.Format(
                        "XPATH = {0} , NODE_HTML = {1} , THREAD = {2}",
                        tmpNode.XPath, tmpNode.InnerHtml, keyUrl));

                    threadList.Add(keyUrl);
                }
            }

            //log
            if (this.logger != null) this.logger.WriteInfo("完成處理文章列表");
            this.msg = "THREAD COUNT = {0}";
            if (this.logger != null) this.logger.WriteInfo(string.Format(msg, threadList.Count));
            return threadList;
        }

        public string Get_Type() {
            if (this.logger != null) this.logger.Write(this, "處理 Get_Type");

            string result = "";
            try {
                HtmlNode tmpNode = this.GetNode(strGetTypePath);
                if (this.logger != null) this.logger.WriteLog(string.Format("取得 HtmlNode = {0}", tmpNode != null));
                if (tmpNode != null) {
                    if (this.logger != null) this.logger.WriteHtml(string.Format("{0}_{1}_Get_Type", pKey, sKey), tmpNode.InnerHtml);
                    string text = tmpNode.InnerText;
                    result = text.ToLower();
                }
            } catch (Exception ex) {
                this.ErrorLog(ex);
            }
            if (this.logger != null) this.logger.WriteTrace(string.Format("Crawler_Type = {0}", result));
            if (this.logger != null) this.logger.Write(this, "完成處理 Get_Type");

            return result;
        }

        public string Get_Upload_Time() {
            if (this.logger != null) this.logger.Write(this, "處理 Get_Upload_Time");

            string result = "";
            try {
                HtmlNode tmpNode = this.GetNode(strGetUnloadTimePath);
                if (this.logger != null) this.logger.WriteLog(string.Format("取得 HtmlNode = {0}", tmpNode != null));
                if (tmpNode != null) {
                    if (this.logger != null) this.logger.WriteHtml(string.Format("{0}_{1}_Get_Upload_Time", pKey, sKey), tmpNode.InnerHtml);
                    result = tmpNode.InnerHtml;
                }
            } catch (Exception ex) {
                this.ErrorLog(ex);
            }

            if (this.logger != null) this.logger.WriteTrace(string.Format("Crawler_Upload_Time = {0}", result));
            if (this.logger != null) this.logger.Write(this, "完成處理 Get_Upload_Time");

            return result;
        }

        public string Get_Eng_Title() {
            if (this.logger != null) this.logger.Write(this, "處理 Get_Eng_Title");

            string result = "";
            try {
                HtmlNode tmpNode = this.GetNode(strGetEngTitlePath);
                if (this.logger != null) this.logger.WriteLog(string.Format("取得 HtmlNode = {0}", tmpNode != null));
                if (tmpNode != null) {
                    if (this.logger != null) this.logger.WriteHtml(string.Format("{0}_{1}_Get_Eng_Title", pKey, sKey), tmpNode.InnerHtml);
                    result = tmpNode.InnerHtml;
                }
            } catch (Exception ex) {
                this.ErrorLog(ex);
            }

            if (this.logger != null) this.logger.WriteTrace(string.Format("Crawler_Eng_Title = {0}", result));
            if (this.logger != null) this.logger.Write(this, "完成處理 Get_Eng_Title");
            return result;
        }

        public string Get_Uni_Title() {
            if (this.logger != null) this.logger.Write(this, "處理 Get_Uni_Title");

            string result = "";
            try {
                HtmlNode tmpNode = this.GetNode(strGetUniTitlePath);
                if (this.logger != null) this.logger.WriteLog(string.Format("取得 HtmlNode = {0}", tmpNode != null));
                if (tmpNode != null) {
                    if (this.logger != null) this.logger.WriteHtml(string.Format("{0}_{1}_Get_Uni_Title", pKey, sKey), tmpNode.InnerHtml);
                    result = tmpNode.InnerHtml;
                }
            } catch (Exception ex) {
                this.ErrorLog(ex);
            }

            if (this.logger != null) this.logger.WriteTrace(string.Format("Crawler_Uni_Title = {0}", result));
            if (this.logger != null) this.logger.Write(this, "完成處理 Get_Uni_Title");
            return result;
        }

        public string Get_Posted_Time() {
            if (this.logger != null) this.logger.Write(this, "處理 Get_Posted_Time");

            string result = "";
            try {
                HtmlNode tmpNode = this.GetNode(strGetPostTimePath);
                if (this.logger != null) this.logger.WriteLog(string.Format("取得 HtmlNode = {0}", tmpNode != null));
                if (tmpNode != null) {
                    if (this.logger != null) this.logger.WriteHtml(string.Format("{0}_{1}_Get_Posted_Time", pKey, sKey), tmpNode.InnerHtml);
                    result = tmpNode.InnerHtml;
                }
            } catch (Exception ex) {
                this.ErrorLog(ex);
            }

            if (this.logger != null) this.logger.WriteTrace(string.Format("Crawler_Posted_Time = {0}", result));
            if (this.logger != null) this.logger.Write(this, "完成處理 Get_Posted_Time");
            return result;
        }

        public string Get_Language() {
            if (this.logger != null) this.logger.Write(this, "處理 Get_Language");

            string result = "";
            try {
                HtmlNode tmpNode = this.GetNode(strGetLanguagePath);
                if (this.logger != null) this.logger.WriteLog(string.Format("取得 HtmlNode = {0}", tmpNode != null));
                if (tmpNode != null) {
                    if (this.logger != null) this.logger.WriteHtml(string.Format("{0}_{1}_Get_Language", pKey, sKey), tmpNode.InnerHtml);

                    string text = tmpNode.FirstChild.OuterHtml
                        .Replace(" &nbsp;", "")
                        .Replace("<span class=\"halp\" title=\"This gallery has been translated from the original language text.\">TR</span>", "");
                    result = text;
                }
            } catch (Exception ex) {
                this.ErrorLog(ex);
            }

            if (this.logger != null) this.logger.WriteTrace(string.Format("Crawler_Language = {0}", result));
            if (this.logger != null) this.logger.Write(this, "完成處理 Get_Language");
            return result;
        }

        public string Get_Total_Page() {
            if (this.logger != null) this.logger.Write(this, "處理 Get_Language");

            string result = "";
            try {
                HtmlNode tmpNode = this.GetNode(strGetTotalPagePath);
                if (this.logger != null) this.logger.WriteLog(string.Format("取得 HtmlNode = {0}", tmpNode != null));
                if (tmpNode != null) {
                    if (this.logger != null) this.logger.WriteHtml(string.Format("{0}_{1}_Get_Total_Page", pKey, sKey), tmpNode.InnerHtml);
                    result = tmpNode.InnerHtml.Replace(" pages", "");
                }
            } catch (Exception ex) {
                this.ErrorLog(ex);
            }

            if (this.logger != null) this.logger.WriteTrace(string.Format("Crawler_Total_Page = {0}", result));
            if (this.logger != null) this.logger.Write(this, "完成處理 Get_Total_Page");
            return result;
        }

        public string Get_Rate_Time() {
            if (this.logger != null) this.logger.Write(this, "處理 Get_Rate_Time");

            string result = "";
            try {
                HtmlNode tmpNode = this.GetNode(strGetRateTimePath);
                if (this.logger != null) this.logger.WriteLog(string.Format("取得 HtmlNode = {0}", tmpNode != null));
                if (tmpNode != null) {
                    if (this.logger != null) this.logger.WriteHtml(string.Format("{0}_{1}_Get_Rate_Time", pKey, sKey), tmpNode.InnerHtml);
                    result = tmpNode.InnerHtml;
                }
            } catch (Exception ex) {
                this.ErrorLog(ex);
            }

            if (this.logger != null) this.logger.WriteTrace(string.Format("Crawler_Rate_Time = {0}", result));
            if (this.logger != null) this.logger.Write(this, "完成處理 Get_Rate_Time");
            return result;
        }

        public string Get_Rate_Avg() {
            if (this.logger != null) this.logger.Write(this, "處理 Get_Rate_Avg");
            string result = "";
            try {
                HtmlNode tmpNode = this.GetNode(strGetRateAvgPath);
                if (this.logger != null) this.logger.WriteLog(string.Format("取得 HtmlNode = {0}", tmpNode != null));
                if (tmpNode != null) {
                    if (this.logger != null) this.logger.WriteHtml(string.Format("{0}_{1}_Get_Rate_Avg", pKey, sKey), tmpNode.InnerHtml);
                    result = tmpNode.InnerHtml.Replace("Average: ", "");
                }
            } catch (Exception ex) {
                this.ErrorLog(ex);
            }

            if (this.logger != null) this.logger.WriteTrace(string.Format("Crawler_Rate_Avg = {0}", result));
            if (this.logger != null) this.logger.Write(this, "完成處理 Get_Rate_Avg");
            return result;
        }

        public string Get_Cover() {
            if (this.logger != null) this.logger.Write(this, "處理 Get_Cover");

            string result = "";
            try {
                if (this.logger != null) this.logger.WriteTrace(string.Format("Get_Cover_THREAD_PKEY = {0}", pKey));
                if (this.logger != null) this.logger.WriteTrace(string.Format("Get_Cover_THREAD_SKEY = {0}", sKey));

                if (this.logger != null) this.logger.Write(this, "開始處理封面下載");

                // Build URL same as original PIC did
                string urlTemplate = "{2}/g/{0}/{1}/?nw=session";
                string url = string.Format(urlTemplate, pKey, sKey, mainUrl);

                HTML web2 = null;
                Cookie alwayCookie = new Cookie("nw", "1", "/", "e-hentai.org");

                web2 = new HTML(url);
                web2.webCookie = (this.authUser ?? Central.authUser).loginCookie;
                web2.cookieContainer = (this.authUser ?? Central.authUser).loginContainer;
                try {
                    web2.cookieContainer.Add(alwayCookie);
                } catch { /* ignore if already present or domain mismatch */ }
                web2.GetHtml();
                string pageHtml = web2.webHtml;

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(pageHtml);

                HtmlNode root = doc.DocumentNode;
                string xpath = "";

                xpath = "/html[1]/body[1]/div[2]/div[1]/div[1]/div[1]";
                HtmlNode found = root.SelectSingleNode(xpath);
                if (found != null) {
                    Match match = Regex.Match(
                        found.Attributes["style"].Value,
                        @"url\((.+?)\)");
                    if (match.Length != 0) {
                        result = match.Groups[1].Value;
                    }
                } else {
                    xpath += "/html[1]/body[1]/div[2]/div[1]/div[1]";
                    found = root.SelectSingleNode(xpath);
                    if (found != null) {
                        Match match = Regex.Match(
                            found.Attributes["style"].Value,
                            @"url\((.+?)\)");
                        if (match.Length != 0) {
                            result = match.Groups[1].Value;
                        }
                    }
                }

                if (this.logger != null) this.logger.Write(this, "完成處理封面下載");

            } catch (Exception ex) {
                this.ErrorLog(ex);
            }

            if (this.logger != null) this.logger.WriteTrace(string.Format("Crawler_Cover = {0}", result));
            if (this.logger != null) this.logger.Write(this, "完成處理 Get_Cover");
            return result;
        }

        public string Get_Tab_Count() {
            if (this.logger != null) this.logger.Write(this, "處理 Get_Tab_Count");
            string result = "";
            HtmlNode tmpNode = null;
            try {
                for (int i = 0; i < 4; i++) {
                    string path = "";
                    switch (i) {
                        case 0:
                            path = strGetTabCountPath1;
                            break;
                        case 1:
                            path = strGetTabCountPath2;
                            break;
                        case 2:
                            path = strGetTabCountPath3;
                            break;
                        case 3:
                            path = strGetTabCountPath4;
                            break;
                    }

                    tmpNode = this.GetNode(path);
                    if (this.logger != null) this.logger.WriteLog(string.Format("取得 HtmlNode = {0}", tmpNode != null));

                    if (tmpNode != null) {
                        if (this.logger != null) this.logger.WriteHtml(string.Format("{0}_{1}_Get_Tab_Count", pKey, sKey), tmpNode.InnerHtml);
                        result = tmpNode.ChildNodes[tmpNode.ChildNodes.Count - 2].InnerText;
                    }
                }
            } catch (Exception ex) {
                this.ErrorLog(ex);
            }

            if (this.logger != null) this.logger.WriteTrace(string.Format("Crawler_Tab_Count = {0}", result));
            if (this.logger != null) this.logger.Write(this, "完成處理 Get_Tab_Count");
            return result;
        }

        // (TypeList helper omitted - unchanged)

    }

}