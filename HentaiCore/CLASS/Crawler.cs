using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using HtmlAgilityPack;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Web;
using HentaiCore;

namespace HentaiCore
{
    public class PIC
    {
        public string picUrl = "";

        string pKey = "", sKey = "";
        string url = "{2}/g/{0}/{1}/?nw=session";
        string domain = "";
        string cookie = "nw=1";

        public PIC(string baseUrl, string P_KEY, string S_KEY)
        {
            this.pKey = P_KEY;
            this.sKey = S_KEY;

            this.domain = new Uri(baseUrl).Host;
            this.url = string.Format(url, pKey, sKey, baseUrl);

        }

        public void GetPic()
        {
            string html = "";
            HTML web = null;
            Cookie alwayCookie = new Cookie("nw", "1", "/", "e-hentai.org");

            web = new HTML(this.url);
            web.webCookie = Central.authUser.loginCookie;
            web.cookieContainer = Central.authUser.loginContainer;
            web.cookieContainer.Add(alwayCookie);
            web.GetHtml();
            html = web.webHtml;

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            HtmlNode node = doc.DocumentNode;
            string xpath = "";

            xpath = "/html[1]/body[1]/div[2]/div[1]/div[1]/div[1]";
            node = node.SelectSingleNode(xpath);
            if (node != null)
            {
                Match match = Regex.Match(
                    node.Attributes["style"].Value,
                    @"url\((.+?)\)");
                if (match.Length != 0)
                {
                    this.picUrl = match.Groups[1].Value;

                }
            }
            else
            {
                xpath += "/html[1]/body[1]/div[2]/div[1]/div[1]";
                node = node.SelectSingleNode(xpath);
                if (node != null)
                {
                    Match match = Regex.Match(
                        node.Attributes["style"].Value,
                        @"url\((.+?)\)");
                    if (match.Length != 0)
                    {
                        this.picUrl = match.Groups[1].Value;

                    }
                }

            }

        }

    }

    public class Crawler
    {
        private HtmlAgilityPack.HtmlDocument document = null;
        private HtmlNode node = null;
        private Logger logger = null;
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

        #endregion Path

        private HtmlNode GetNode(string nodePath)
        {
            this.msg = "XPATH = {0}";
            this.logger.WriteLog(string.Format(msg, nodePath));

            HtmlNode tmpNode = node.SelectSingleNode(nodePath);
            return tmpNode;

        }

        private void ErrorLog(Exception ex)
        {
            this.logger.WriteError(ex.ToString());
            this.logger.WriteError(ex.StackTrace);

        }

        public Crawler(string htmlStr)
        {
            this.html = htmlStr;

            this.document = new HtmlAgilityPack.HtmlDocument();
            this.document.LoadHtml(html);

            this.node = document.DocumentNode;

            this.logger = Central.logger;

        }

        public List<string> RollList()
        {
            this.logger.WriteInfo("處理文章列表");
            string tableXPath = "/html[1]/body[1]/div[2]/div[2]/table[1]";

            HtmlNode rootNode = document.DocumentNode;
            List<string> threadList = new List<string>();

            this.logger.Write(this, "處理文章列表的標籤");
            foreach (HtmlNode node in rootNode.SelectNodes("//tr"))
            {
                if (!node.XPath.StartsWith(tableXPath) || node.XPath == tableXPath + "/tr[1]")
                {
                    continue;
                }

                string appendPath = "/td[3]/a[1]";
                HtmlNode tmpNode = node.SelectSingleNode(node.XPath + appendPath);
                if (tmpNode != null)
                {
                    string keyUrl = tmpNode.Attributes["href"].Value;

                    //log
                    this.logger.WriteTrace(
                        string.Format(
                        "XPATH = {0} , NODE_HTML = {1} , THREAD = {2}",
                        tmpNode.XPath, tmpNode.InnerHtml, keyUrl));

                    threadList.Add(keyUrl);

                }

            }

            //log
            this.logger.WriteInfo("完成處理文章列表");
            this.msg = "THREAD COUNT = {0}";
            this.logger.WriteInfo(string.Format(msg, threadList.Count));
            return threadList;

        }

        public string Get_Type()
        {
            this.logger.Write(this, "處理 Get_Type");

            string result = "";
            try
            {
                HtmlNode tmpNode = this.GetNode(strGetTypePath);
                this.logger.WriteLog(string.Format("取得 HtmlNode = {0}", tmpNode != null));
                if (tmpNode != null)
                {
                    this.logger.WriteHtml(string.Format("{0}_{1}_Get_Type", pKey, sKey), tmpNode.InnerHtml);
                    string text = tmpNode.InnerText;
                    result = text.ToLower();

                }

            }
            catch (Exception ex)
            {
                this.ErrorLog(ex);

            }
            this.logger.WriteTrace(string.Format("Crawler_Type = {0}", result));
            this.logger.Write(this, "完成處理 Get_Type");

            return result;

        }

        public string Get_Upload_Time()
        {
            this.logger.Write(this, "處理 Get_Upload_Time");

            string result = "";
            try
            {
                HtmlNode tmpNode = this.GetNode(strGetUnloadTimePath);
                this.logger.WriteLog(string.Format("取得 HtmlNode = {0}", tmpNode != null));
                if (tmpNode != null)
                {
                    this.logger.WriteHtml(string.Format("{0}_{1}_Get_Upload_Time", pKey, sKey), tmpNode.InnerHtml);
                    result = tmpNode.InnerHtml;

                }

            }
            catch (Exception ex)
            {
                this.ErrorLog(ex);

            }

            //log
            this.logger.WriteTrace(string.Format("Crawler_Upload_Time = {0}", result));
            this.logger.Write(this, "完成處理 Get_Upload_Time");

            return result;

        }

        public string Get_Eng_Title()
        {
            this.logger.Write(this, "處理 Get_Eng_Title");

            string result = "";
            try
            {
                HtmlNode tmpNode = this.GetNode(strGetEngTitlePath);
                this.logger.WriteLog(string.Format("取得 HtmlNode = {0}", tmpNode != null));
                if (tmpNode != null)
                {
                    this.logger.WriteHtml(string.Format("{0}_{1}_Get_Eng_Title", pKey, sKey), tmpNode.InnerHtml);
                    result = tmpNode.InnerHtml;

                }

            }
            catch (Exception ex)
            {
                this.ErrorLog(ex);

            }

            //log
            this.logger.WriteTrace(string.Format("Crawler_Eng_Title = {0}", result));
            this.logger.Write(this, "完成處理 Get_Eng_Title");
            return result;

        }

        public string Get_Uni_Title()
        {
            this.logger.Write(this, "處理 Get_Uni_Title");

            string result = "";
            try
            {
                HtmlNode tmpNode = this.GetNode(strGetUniTitlePath);
                this.logger.WriteLog(string.Format("取得 HtmlNode = {0}", tmpNode != null));
                if (tmpNode != null)
                {
                    this.logger.WriteHtml(string.Format("{0}_{1}_Get_Uni_Title", pKey, sKey), tmpNode.InnerHtml);
                    result = tmpNode.InnerHtml;

                }

            }
            catch (Exception ex)
            {
                this.ErrorLog(ex);

            }

            //log
            this.logger.WriteTrace(string.Format("Crawler_Uni_Title = {0}", result));
            this.logger.Write(this, "完成處理 Get_Uni_Title");
            return result;

        }

        public string Get_Posted_Time()
        {
            this.logger.Write(this, "處理 Get_Posted_Time");

            string result = "";
            try
            {
                HtmlNode tmpNode = this.GetNode(strGetPostTimePath);
                this.logger.WriteLog(string.Format("取得 HtmlNode = {0}", tmpNode != null));
                if (tmpNode != null)
                {
                    this.logger.WriteHtml(string.Format("{0}_{1}_Get_Posted_Time", pKey, sKey), tmpNode.InnerHtml);
                    result = tmpNode.InnerHtml;

                }

            }
            catch (Exception ex)
            {
                this.ErrorLog(ex);

            }

            //log
            this.logger.WriteTrace(string.Format("Crawler_Posted_Time = {0}", result));
            this.logger.Write(this, "完成處理 Get_Posted_Time");
            return result;

        }

        public string Get_Language()
        {
            this.logger.Write(this, "處理 Get_Language");

            string result = "";
            try
            {
                HtmlNode tmpNode = this.GetNode(strGetLanguagePath);
                this.logger.WriteLog(string.Format("取得 HtmlNode = {0}", tmpNode != null));
                if (tmpNode != null)
                {
                    this.logger.WriteHtml(string.Format("{0}_{1}_Get_Language", pKey, sKey), tmpNode.InnerHtml);

                    string text = tmpNode.FirstChild.OuterHtml
                        .Replace(" &nbsp;", "")
                        .Replace("<span class=\"halp\" title=\"This gallery has been translated from the original language text.\">TR</span>", "");
                    result = text;

                }

            }
            catch (Exception ex)
            {
                this.ErrorLog(ex);

            }

            //log
            this.logger.WriteTrace(string.Format("Crawler_Language = {0}", result));
            this.logger.Write(this, "完成處理 Get_Language");
            return result;

        }

        public string Get_Total_Page()
        {
            this.logger.Write(this, "處理 Get_Language");

            string result = "";
            try
            {
                HtmlNode tmpNode = this.GetNode(strGetTotalPagePath);
                this.logger.WriteLog(string.Format("取得 HtmlNode = {0}", tmpNode != null));
                if (tmpNode != null)
                {
                    this.logger.WriteHtml(string.Format("{0}_{1}_Get_Total_Page", pKey, sKey), tmpNode.InnerHtml);
                    result = tmpNode.InnerHtml.Replace(" pages", "");

                }

            }
            catch (Exception ex)
            {
                this.ErrorLog(ex);

            }

            //log
            this.logger.WriteTrace(string.Format("Crawler_Total_Page = {0}", result));
            this.logger.Write(this, "完成處理 Get_Total_Page");
            return result;

        }

        public string Get_Rate_Time()
        {
            this.logger.Write(this, "處理 Get_Rate_Time");

            string result = "";
            try
            {
                HtmlNode tmpNode = this.GetNode(strGetRateTimePath);
                this.logger.WriteLog(string.Format("取得 HtmlNode = {0}", tmpNode != null));
                if (tmpNode != null)
                {
                    this.logger.WriteHtml(string.Format("{0}_{1}_Get_Rate_Time", pKey, sKey), tmpNode.InnerHtml);
                    result = tmpNode.InnerHtml;

                }

            }
            catch (Exception ex)
            {
                this.ErrorLog(ex);

            }

            //log
            this.logger.WriteTrace(string.Format("Crawler_Rate_Time = {0}", result));
            this.logger.Write(this, "完成處理 Get_Rate_Time");
            return result;

        }

        public string Get_Rate_Avg()
        {
            this.logger.Write(this, "處理 Get_Rate_Avg");
            string result = "";
            try
            {
                HtmlNode tmpNode = this.GetNode(strGetRateAvgPath);
                this.logger.WriteLog(string.Format("取得 HtmlNode = {0}", tmpNode != null));
                if (tmpNode != null)
                {
                    this.logger.WriteHtml(string.Format("{0}_{1}_Get_Rate_Avg", pKey, sKey), tmpNode.InnerHtml);
                    result = tmpNode.InnerHtml.Replace("Average: ", "");

                }

            }
            catch (Exception ex)
            {
                this.ErrorLog(ex);

            }

            //log
            this.logger.WriteTrace(string.Format("Crawler_Rate_Avg = {0}", result));
            this.logger.Write(this, "完成處理 Get_Rate_Avg");
            return result;

        }

        public string Get_Cover()
        {
            this.logger.Write(this, "處理 Get_Cover");

            string result = "";
            try
            {
                //log
                this.logger.WriteTrace(string.Format("Get_Cover_THREAD_PKEY = {0}", pKey));
                this.logger.WriteTrace(string.Format("Get_Cover_THREAD_SKEY = {0}", sKey));

                this.logger.Write(this, "開始處理封面下載");
                PIC pic = new PIC(this.mainUrl, pKey, sKey);
                pic.GetPic();

                this.logger.Write(this, "完成處理封面下載");

                result = pic.picUrl;

            }
            catch (Exception ex)
            {
                this.ErrorLog(ex);

            }

            //log
            this.logger.WriteTrace(string.Format("Crawler_Cover = {0}", result));
            this.logger.Write(this, "完成處理 Get_Cover");
            return result;

        }

        public string Get_Tab_Count()
        {
            this.logger.Write(this, "處理 Get_Tab_Count");
            string result = "";
            HtmlNode tmpNode = null;
            try
            {

                for (int i = 0; i < 4; i++)
                {
                    string path = "";
                    switch (i)
                    {
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
                    this.logger.WriteLog(string.Format("取得 HtmlNode = {0}", tmpNode != null));

                    if (tmpNode != null)
                    {
                        this.logger.WriteHtml(string.Format("{0}_{1}_Get_Tab_Count", pKey, sKey), tmpNode.InnerHtml);
                        result = tmpNode.ChildNodes[tmpNode.ChildNodes.Count - 2].InnerText;

                    }

                }


            }
            catch (Exception ex)
            {
                this.ErrorLog(ex);

            }

            //log
            this.logger.WriteTrace(string.Format("Crawler_Tab_Count = {0}", result));
            this.logger.Write(this, "完成處理 Get_Tab_Count");
            return result;

        }

    }

}
