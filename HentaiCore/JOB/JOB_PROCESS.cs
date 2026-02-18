using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HentaiCore {
    public class JOB_PROCESS {
        System.Threading.Thread t = null;
        public event EventHandler finish = null;

        string url = "";
        string MainUrl = "";

        string userName = "";
        string passWord = "";
        int pageNum = 0;

        public int PageCount = -1;
        public List<BOOK_INFO> BookInfoList = null;

        // --- JOB (original) fields moved here ---
        public AUTH authUser = null;
        public string domain = "";
        HTML web = null;

        protected Logger logger = null;

        int retry = 3;

        public JOB_PROCESS(string url, string userName, string passWord) {
            this.url = url;
            this.userName = userName;
            this.passWord = passWord;

            // initialize web & logger (original JOB ctor)
            this.web = new HTML();
            this.logger = Central.logger;
        }

        public JOB_PROCESS(string url, int pageNum, string userName, string passWord) {
            this.url = url;
            this.pageNum = pageNum;
            this.userName = userName;
            this.passWord = passWord;

            // initialize web & logger (original JOB ctor)
            this.web = new HTML();
            this.logger = Central.logger;
        }

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

        protected System.IO.Stream GetStream(string url) {
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

        public void BeginProcess() {
            if (this.logger != null) this.logger.WriteInfo(string.Format("行動開始 URL：{0}", url));

            Uri uri = new Uri(url);
            this.MainUrl = string.Format("https://{0}", uri.Host);

            SITE_INFO webSite = new SITE_INFO(url);

            JOB_CONTROL jobControl = new JOB_CONTROL();
            #region 處理 AUTH
            if (this.logger != null) this.logger.Write(this, string.Format("處理 AUTH URL：{0}", url));
            if (Central.authUser == null) {
                AUTH auth = new AUTH(userName, passWord);
                auth.Login();

                Central.authUser = auth;

            }

            #endregion 處理 AUTH

            #region GET_SITE
            if (this.logger != null) this.logger.Write(this, string.Format("處理 GET_SITE URL：{0}", url));
            JOB_WEB jobWeb = new JOB_WEB(webSite);
            jobWeb.authUser = Central.authUser;
            jobWeb.domain = MainUrl;

            jobControl.AddJob(jobWeb);
            t = new Thread(new System.Threading.ThreadStart(jobControl.DoJob));
            t.Start();
            while (t.ThreadState != System.Threading.ThreadState.Stopped) {
                Thread.Sleep(100);
            }

            #endregion GET_SITE

            #region GET_PAGE
            if (this.logger != null) this.logger.Write(this, string.Format("處理 GET_PAGE URL：{0}", url));

            List<PAGE_INFO> page = jobWeb.page_info;
            JOB_PAGE jobPage = new JOB_PAGE(page[pageNum]);
            jobPage.authUser = Central.authUser;
            jobPage.domain = MainUrl;

            jobControl.AddJob(jobPage);
            t = new Thread(new System.Threading.ThreadStart(jobControl.DoJob));
            t.Start();
            while (t.ThreadState != System.Threading.ThreadState.Stopped) {
                Thread.Sleep(100);
            }

            #endregion GET_PAGE

            #region GET_THREAD
            if (this.logger != null) this.logger.Write(this, string.Format("處理 GET_THREAD URL：{0}", url));

            List<JOB_THREAD> threadList = new List<JOB_THREAD>();
            foreach (THREAD_INFO thread in jobPage.thread_info) {
                JOB_THREAD jobThread = new JOB_THREAD(thread);
                jobThread.book_info.pageUrl = thread.page_url;
                jobThread.authUser = Central.authUser;
                jobThread.domain = MainUrl;

                threadList.Add(jobThread);

            }

            List<BOOK_INFO> bookInfoList = new List<BOOK_INFO>();

            foreach (JOB_THREAD thread in threadList) {
                jobControl.AddJob(thread);
            }

            t = new Thread(new System.Threading.ThreadStart(jobControl.DoJob));
            t.Start();

            while (t.ThreadState != System.Threading.ThreadState.Stopped) {
                Thread.Sleep(100);

            }

            foreach (JOB_THREAD thread in threadList) {
                bookInfoList.Add(thread.book_info);

            }

            #endregion GET_THREAD

            this.BookInfoList = bookInfoList;
            this.PageCount = jobWeb.page_info.Count;

            if (this.logger != null) this.logger.WriteInfo(string.Format("行動結束進入顯示 URL：{0}", url));
            this.finish(this, null);
        }

    }
}