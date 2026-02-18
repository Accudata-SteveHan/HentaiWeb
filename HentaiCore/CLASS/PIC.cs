using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Data;
using System.Net;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

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

}
