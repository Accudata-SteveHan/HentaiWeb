using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using HtmlAgilityPack;
using HentaiCore;
using System.IO;

namespace HentaiCore {
    //頁面的工作
    public class JOB_PAGE {
        PAGE_INFO page_info = null;
        public List<THREAD_INFO> thread_info = null;

        // --- JOB (original) fields moved here ---
        public AUTH authUser = null;
        public string domain = "";
        HTML web = null;

        protected Logger logger = null;

        int retry = 3;

        // --- Crawler-derived fields used by this JOB_PAGE ---
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

        public JOB_PAGE(PAGE_INFO pageInfo) {
            this.page_info = pageInfo;
            this.thread_info = new List<THREAD_INFO>();

            // initialize web & logger (original JOB ctor)
            this.web = new HTML();
            this.logger = Central.logger;
        }

        public void GetThreadList() {
            try {
                //檢查輸入 PAGE INFO是否輸入
                if (this.page_info == null) {
                    throw new Exception("PAGE_INFO = null");
                }

                if (this.logger != null) this.logger.WriteInfo(string.Format("開始處理列表頁面 : {0}", this.page_info.page_url));


                //取得網頁資料
                string html = this.GetWeb(this.page_info.page_url);
                //如果是 exHentai 的話要在讀取一次頁面
                if (this.page_info.page_url.Contains("exhentai")) {
                    html = this.GetWeb(this.page_info.page_url);

                }

                if (this.logger != null) this.logger.Write(this, string.Format("開始分析列表 : {0}", this.page_info.page_url));
                //處理 Crawler RollList
                this.InitializeCrawler(html);
                List<string> threadStrList = this.RollList();

                if (this.logger != null) this.logger.Write(this, string.Format("完成分析列表 : {0}", this.page_info.page_url));

                //組成 Thread List
                foreach (string thread in threadStrList) {
                    THREAD_INFO threadInfo = new THREAD_INFO(thread);
                    threadInfo.page_url = this.page_info.page_url;

                    this.thread_info.Add(threadInfo);

                }

                //組成 Thread List Count
                this.page_info.page_threadCount = this.thread_info.Count;

                if (this.logger != null) this.logger.WriteInfo(string.Format("完成處理列表頁面 : {0} 總數：{1}", this.page_info.page_url, this.thread_info.Count));

            } catch (Exception ex) {
                if (this.logger != null) this.logger.WriteError(string.Format("JOB_PAGE.GetThreadList 錯誤，{0}", ex.ToString()));

            }

        }

        // --- Methods ported from Crawler.cs (only those needed by JOB_PAGE) ---

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

    }

}