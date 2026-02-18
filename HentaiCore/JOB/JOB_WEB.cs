using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace HentaiCore {
    //網站的工作
    public class JOB_WEB {
        SITE_INFO site_info = null;
        public List<PAGE_INFO> page_info = null;
        Logger logger = Central.logger;

        // --- JOB (original) fields moved here ---
        public AUTH authUser = null;
        public string domain = "";
        HTML web = null;

        protected Logger protectedLogger = null;

        int retry = 3;

        public JOB_WEB(SITE_INFO siteInfo) {
            this.site_info = siteInfo;
            this.page_info = new List<PAGE_INFO>();

            // initialize web & logger (original JOB ctor)
            this.web = new HTML();
            this.protectedLogger = Central.logger;
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

        public void ProcessSite() {
            try {
                if (this.logger != null) this.logger.WriteInfo(string.Format("開始處理列表網站 : {0}", this.site_info.site_url));

                //取得網頁資料
                string html = this.GetWeb(this.site_info.site_url);
                //如果是 exHentai 的話要在讀取一次頁面
                if (this.site_info.site_url.Contains("exhentai")) {
                    html = this.GetWeb(this.site_info.site_url);
                }

                //產生HTML分析
                HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
                document.LoadHtml(html);
                HtmlNode node = document.DocumentNode;

                //開始處理查詢結果
                string xpath = "/html[1]/body[1]/div[2]/div[2]/table[1]/tr[1]";
                HtmlNode tmpNode = node.SelectSingleNode(xpath);
                if (tmpNode != null) {
                    if (this.logger != null) this.logger.Write(this, string.Format("處理列表網站資訊 : {0}", this.site_info.site_url));

                    //取得總頁數
                    this.site_info.page_info_count = int.Parse(tmpNode.ChildNodes[tmpNode.ChildNodes.Count - 2].InnerText);

                    //產生頁面列表
                    string page = "/{0}";
                    for (int i = 0 ; i < this.site_info.page_info_count ; i++) {
                        PAGE_INFO pageInfo = new PAGE_INFO(this.site_info.site_url + string.Format(page, i));
                        pageInfo.site_url = this.site_info.site_url;

                        this.page_info.Add(pageInfo);
                    }

                    if (this.logger != null) this.logger.WriteInfo(string.Format("開始處理列表網站 : {0} PAGE_COUNT = {1}", this.site_info.site_url, this.site_info.page_info_count));
                }

            } catch (Exception ex) {
                if (this.logger != null) this.logger.WriteError(string.Format("JOB_WEB.ProcessSite 錯誤，{0}", ex.ToString()));
            }

        }

    }

}