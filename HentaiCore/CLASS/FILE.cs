using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HtmlAgilityPack;

namespace HentaiGetchu2 {
    public class FILE {
        string template = "{2}/g/{0}/{1}/?nw=session";
        string domain = "";
        string cookie = "nw=1";
        string mainUrl = "";

        string baseSavePath = "";
        string saveTitle = "";

        List<string> mapUrlList = new List<string>();
        Dictionary<int, string> urlList = new Dictionary<int, string>();

        public FILE(string baseUrl, string p, string s) {
            this.domain = baseUrl.Replace("https://", "");
            this.mainUrl = string.Format(template, p, s, baseUrl);
            this.baseSavePath = System.Environment.CurrentDirectory;
            this.saveTitle = string.Format("{0}_{1}", p, s);

        }

        public FILE(string baseUrl, string p, string s, string title) {
            this.domain = baseUrl.Replace("https://", "");
            this.mainUrl = string.Format(template, p, s, baseUrl);
            this.saveTitle = title;

        }

        public FILE(string baseUrl, string p, string s, string title, string savePath) {
            this.domain = baseUrl.Replace("https://", "");
            this.baseSavePath = savePath;
            this.saveTitle = title;
            this.mainUrl = string.Format(template, p, s, baseUrl);

        }

        public void ProcessUrl() {
            this.GetPicMapList();
            this.GetPicUrlList();
            this.DownPicList();

        }

        private void GetPicMapList() {
            HTML web = new HTML(this.mainUrl);
            web.webCookie = FormMain.authUser.loginCookie;
            web.cookieContainer = FormMain.authUser.loginContainer;
            web.GetHtml();
            string html = web.webHtml;

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            HtmlNode rootNode = doc.DocumentNode;
            string tableXPath = "/html[1]/body[1]/div[3]/table[1]/tr[1]";
            HtmlNode rowNode = rootNode.SelectSingleNode(tableXPath);
            foreach (HtmlNode node in rowNode.ChildNodes) {
                if (node.InnerHtml == "&lt;" || node.InnerHtml == "&gt;") {
                    continue;
                }

                this.mapUrlList.Add(node.ChildNodes[0].Attributes["href"].Value);

            }
        }

        private void GetPicUrlList() {
            int count = 0;

            foreach (string mapUrl in this.mapUrlList) {
                HTML web = new HTML(mapUrl);
                web.webCookie = FormMain.authUser.loginCookie;
                web.cookieContainer = FormMain.authUser.loginContainer;
                web.GetHtml();
                string html = web.webHtml;

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                HtmlNode rootNode = doc.DocumentNode;
                string tableXPath = "/html[1]/body[1]/div[5]";

                HtmlNode tableNode = rootNode.SelectSingleNode(tableXPath);

                foreach (HtmlNode node in tableNode.ChildNodes) {
                    if (node.XPath.StartsWith(tableXPath)) {
                        string xpath = node.XPath + "/div[1]/a[1]";

                        count++;
                        HtmlNode imgNode = rootNode.SelectSingleNode(xpath);
                        if (imgNode != null) {
                            this.urlList.Add(count, imgNode.Attributes["href"].Value);

                        }

                    }

                }

            }

        }

        private void DownPicList() {
            string savePath = Path.Combine(baseSavePath, saveTitle);

            if (!Directory.Exists(savePath)) {
                Directory.CreateDirectory(savePath);
            }

            foreach (int key in this.urlList.Keys) {
                string img = this.urlList[key];

                PIC pic = new PIC(img);
                pic.DownloadPic(string.Format(@"{0}\{1}", savePath, key.ToString().PadLeft(3, '0') + ".jpg"));

            }

        }

    }

}
