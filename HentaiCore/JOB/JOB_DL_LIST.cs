using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.IO;

namespace HentaiCore {
    public class JOB_DL_LIST {
        BOOK_INFO bookInfo = null;
        public List<PIC_INFO> pic_info = null;

        // --- JOB (original) fields moved here ---
        public AUTH authUser = null;
        public string domain = "";
        HTML web = null;

        protected Logger logger = null;

        int retry = 3;

        public JOB_DL_LIST(BOOK_INFO bookInfo) {
            this.bookInfo = bookInfo;
            this.pic_info = new List<PIC_INFO>();

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

        public void ProcessDLList(string divCount = "6") {
            if (this.logger != null) this.logger.WriteInfo(string.Format("開始處理下載清單 : {0}" , this.bookInfo.threadUrl));

            //總頁數
            int totalCount = 0;
            //總 Tab
            int totalTab = int.Parse(this.bookInfo.tab_count.ToString());

            this.GetWeb(this.bookInfo.threadUrl);

            //處理各頁
            string web = this.bookInfo.threadUrl + "?p={0}";
            for (int i = 0 ; i < this.bookInfo.tab_count ; i++) {
                //取得頁面
                string url = string.Format(web, i);
                if (this.logger != null) this.logger.Write(this, string.Format("開始處理下載清單頁面 : {0}", url));

                string html = this.GetWeb(url);
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                //分析
                HtmlNode rootNode = doc.DocumentNode;
                string tableXPath = "/html[1]/body[1]/div[{0}]";
                tableXPath = string.Format(tableXPath, divCount);
                HtmlNode tableNode = rootNode.SelectSingleNode(tableXPath);
                foreach (HtmlNode node in tableNode.ChildNodes) {
                    if (!node.XPath.StartsWith(tableXPath)) {
                        continue;
                    }

                    //頁數 + 1
                    totalCount++;

                    for (int p = 0; p < 3; p++)
                    {
                        string xpath = node.XPath;
                        this.logger.Write(this, string.Format(
                            "開始處理下載清單頁面圖片 : {0} PIC_COUNT = {1}", url, totalCount));
                        //處理每個圖片頁面的URL
                        HtmlNode imgNode = rootNode.SelectSingleNode(xpath);
                        if (imgNode != null)
                        {
                            PIC_INFO picInfo = new PIC_INFO();
                            picInfo.threadUrl = bookInfo.pageUrl;
                            picInfo.picNo = totalCount.ToString().PadLeft(this.bookInfo.total_page.ToString().Length, '0');
                            picInfo.picUrl = imgNode.Attributes["href"].Value;
                            picInfo.picFileName = Central.PathDirector.GetSaveFileName(bookInfo.p_key, bookInfo.s_key, picInfo.picNo);

                            pic_info.Add(picInfo);

                            break;

                        }
                    }

                    this.logger.Write(this, string.Format(
                        "完成處理下載清單頁面圖片 : {0} PIC_COUNT = {1}", url, totalCount));

                }

                this.logger.Write(this, string.Format("完成處理下載清單頁面 : {0}", url));

            }

            if (this.logger != null) this.logger.WriteInfo(string.Format("完成處理下載清單 : {0}", this.bookInfo.threadUrl));

        }

        public void ProcessDeepDLList() {
            try {
                if (this.logger != null) this.logger.WriteInfo(string.Format("開始深度處理下載清單 : {0}", this.bookInfo.threadUrl));
                //處理每個圖片頁面
                for (int i = 0 ; i < pic_info.Count ; i++) {
                    string picUrl = "";

                    try
                    {
                        PIC_INFO picInfo = pic_info[i];
                        if (this.logger != null) this.logger.WriteInfo(string.Format(
                            "開始處理深度頁面圖片 : {1}_{0}",
                            picInfo.picUrl,
                            i.ToString().PadLeft(pic_info.Count.ToString().Length, '0')));
                        picUrl = picInfo.picUrl;
                        string html = this.GetWeb(picInfo.picUrl);

                        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                        doc.LoadHtml(html);

                        HtmlNode rootNode = doc.DocumentNode;
                        string xpath = "/html[1]/body[1]/div[1]/div[5]/a[2]";
                        string imgUrl = rootNode.SelectSingleNode(xpath).Attributes["onclick"].Value;

                        //取得無法顯示狀況下的圖片頁面
                        Regex imgAdd = new Regex("return nl\\('(.+?)-(.+?)'\\)");
                        picInfo.picUrl = string.Format(
                            "{0}?nl={1}-{2}",
                            picInfo.picUrl,
                            imgAdd.Match(imgUrl).Groups[1],
                            imgAdd.Match(imgUrl).Groups[2]);

                        if (this.logger != null) this.logger.WriteInfo(string.Format(
                            "完成處理深度頁面圖片 : {1}_{0}",
                            picInfo.picUrl,
                            i.ToString().PadLeft(pic_info.Count.ToString().Length, '0')));

                    }
                    catch (Exception ex)
                    {
                        if (this.logger != null) this.logger.WriteInfo(string.Format(
                            "異常處理深度頁面圖片 : {1}_{0}_{2}",
                            picUrl,
                            i.ToString().PadLeft(pic_info.Count.ToString().Length, '0'),
                            ex.ToString()));

                    }

                    System.Threading.Thread.Sleep(new Random(int.Parse(DateTime.Now.ToString("MMddmmss"))).Next(1000, 2500));

                }

                if (this.logger != null) this.logger.WriteInfo(string.Format("完成深度處理下載清單 : {0}", this.bookInfo.threadUrl));

            } catch (Exception ex) {
                if (this.logger != null) this.logger.WriteError(string.Format("JOB_DL_LIST.ProcessDeepDLList 錯誤，{0}", ex.ToString()));

            }

        }

    }

}