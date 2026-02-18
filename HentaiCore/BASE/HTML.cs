using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace HentaiCore
{
    public class HTML
    {
        private Logger logger = null;
        private string msg = "";

        public string webUrl = "";
        public CookieContainer cookieContainer = null;
        public string webCookie = "";
        public byte[] postData = null;

        public string webHtml = "";
        public Stream webStream = null;

        public HTML()
        {
            this.logger = Central.logger;

        }

        public HTML(string url)
        {
            this.webUrl = url;
            this.logger = Central.logger;

        }

        public void GetHtml()
        {
            this.logger.WriteInfo("讀取指定網頁並取得 HTML");
            this.logger.Write(this, "開始讀取網站");

            //HttpWebRequest req = null;
            //HttpWebResponse res = null;

            try
            {
                this.msg = "URL = {0}";
                this.logger.Write(this, string.Format(msg, this.webUrl));

                HttpWebRequest req = null;
                req = HttpWebRequest.Create(this.webUrl) as HttpWebRequest;
                req.Proxy = WebRequest.DefaultWebProxy;
                req.Method = "GET";

                if (this.webCookie != "")
                {
                    req.Headers.Set("cookie", this.webCookie);

                    this.msg = "讀取 Cookie = {0}";
                    this.logger.WriteTrace(string.Format(msg, this.webCookie));

                }

                req.CookieContainer = this.cookieContainer;

                if (this.postData != null)
                {
                    int dataLength = this.postData.Length;
                    req.Method = "POST";
                    req.ContentType = "application/x-www-form-urlencoded";
                    req.ContentLength = dataLength;

                    using (Stream dataStream = req.GetRequestStream())
                    {
                        dataStream.Write(this.postData, 0, dataLength);

                    }

                    this.msg = "讀取 PostData , Length = {0}";
                    this.logger.WriteTrace(string.Format(msg, this.postData.Length));

                }

                this.logger.WriteTrace("取得網頁資料流");
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                string htmlStr = "";
                using (HttpWebResponse res = req.GetResponse() as HttpWebResponse)
                {
                    this.logger.WriteTrace("處理網頁 Cookie");

                    if (this.cookieContainer == null)
                    {
                        this.cookieContainer = new CookieContainer();
                    }
                    this.fixCookies(req, res, this.cookieContainer);

                    this.logger.WriteTrace("擷取網頁資料流");

                    using (StreamReader reader = new StreamReader(res.GetResponseStream()))
                    {
                        htmlStr = reader.ReadToEnd();

                    }

                }

                msg = "網站HTML_Length = {0}";
                this.logger.WriteTrace(htmlStr.Length.ToString());
                this.logger.WriteHtml(this.webUrl, htmlStr);
                this.logger.Write(this, "完成讀取網站");
                this.webHtml = htmlStr;

            }
            catch (Exception ex)
            {
                string text = "{0}";
                this.webHtml = string.Format(text, ex.ToString());

                //log
                this.logger.WriteError(string.Format("URL = {0}", this.webUrl));
                this.logger.WriteError(string.Format("RESULT = {0}", this.webHtml));

            }

        }

        public void GetJson()
        {
            this.logger.WriteInfo("讀取指定網頁並取得 JSON");
            this.logger.Write(this, "開始讀取網站");

            HttpWebRequest req = null;
            HttpWebResponse res = null;

            try
            {
                this.msg = "URL = {0}";
                this.logger.Write(this, string.Format(msg, this.webUrl));
                req = HttpWebRequest.Create(this.webUrl) as HttpWebRequest;
                req.Proxy = WebRequest.DefaultWebProxy;
                req.Method = "GET";

                if (this.webCookie != "")
                {
                    req.Headers.Set("cookie", this.webCookie);

                    this.msg = "讀取 Cookie = {0}";
                    this.logger.WriteTrace(string.Format(msg, this.webCookie));

                }

                req.CookieContainer = this.cookieContainer;

                if (this.postData != null)
                {
                    int dataLength = this.postData.Length;
                    req.Method = "POST";
                    req.ContentType = "application/json";
                    req.ContentLength = dataLength;

                    Stream dataStream = req.GetRequestStream();
                    dataStream.Write(this.postData, 0, dataLength);
                    dataStream.Close();

                    this.msg = "讀取 PostData , Length = {0}";
                    this.logger.WriteTrace(string.Format(msg, this.postData.Length));

                }

                this.logger.WriteTrace("取得網頁資料流");
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                res = req.GetResponse() as HttpWebResponse;

                this.logger.WriteTrace("處理網頁 Cookie");
                if (this.cookieContainer == null)
                {
                    this.cookieContainer = new CookieContainer();
                }
                this.fixCookies(req, res, this.cookieContainer);

                this.logger.WriteTrace("擷取網頁資料流");
                StreamReader reader = new StreamReader(res.GetResponseStream());
                string htmlStr = reader.ReadToEnd();
                res.Close();
                reader.Close();

                msg = "網站HTML_Length = {0}";
                this.logger.WriteTrace(htmlStr.Length.ToString());
                this.logger.WriteHtml(this.webUrl, htmlStr);
                this.logger.Write(this, "完成讀取網站");
                this.webHtml = htmlStr;

            }
            catch (Exception ex)
            {
                string text = "{0}";
                this.webHtml = string.Format(text, ex.ToString());

                //log
                this.logger.WriteError(string.Format("URL = {0}", this.webUrl));
                this.logger.WriteError(string.Format("RESULT = {0}", this.webHtml));

            }

        }

        public void GetStream()
        {
            this.logger.WriteInfo("讀取指定網頁並取得 FileStream");
            this.logger.Write(this, "開始讀取網站");

            HttpWebRequest req = null;
            HttpWebResponse res = null;

            try
            {
                this.msg = "URL = {0}";
                this.logger.Write(this, string.Format(msg, this.webUrl));
                req = HttpWebRequest.Create(this.webUrl) as HttpWebRequest;
                req.Proxy = WebRequest.DefaultWebProxy;
                req.Method = "GET";

                if (this.webCookie != "")
                {
                    req.Headers.Set("cookie", this.webCookie);

                    this.msg = "讀取 Cookie = {0}";
                    this.logger.WriteTrace(string.Format(msg, this.webCookie));

                }

                if (this.cookieContainer != null)
                {
                    req.CookieContainer = this.cookieContainer;
                }

                if (this.postData != null)
                {
                    int dataLength = this.postData.Length;
                    req.Method = "POST";
                    req.ContentType = "application/x-www-form-urlencoded";
                    req.ContentLength = dataLength;

                    Stream dataStream = req.GetRequestStream();
                    dataStream.Write(this.postData, 0, dataLength);
                    dataStream.Close();

                    this.msg = "讀取 PostData , Length = {0}";
                    this.logger.WriteTrace(string.Format(msg, this.postData.Length));


                }

                this.logger.WriteTrace("取得網頁資料流");
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                res = req.GetResponse() as HttpWebResponse;

                this.logger.WriteTrace("處理網頁 Cookie");
                if (this.cookieContainer == null)
                {
                    this.cookieContainer = new CookieContainer();
                }
                this.fixCookies(req, res, this.cookieContainer);

                this.logger.WriteTrace("擷取網頁資料流");
                this.webStream = new MemoryStream();
                res.GetResponseStream().CopyTo(this.webStream);
                res.Close();

                msg = "網站串流長度 = {0}";
                this.logger.WriteTrace(string.Format(msg, webStream.Length));
                this.logger.Write(this, "完成讀取網站");

            }
            catch (Exception ex)
            {
                string text = "{0}";
                this.webHtml = string.Format(text, ex.ToString());

                //log
                this.logger.WriteError(string.Format("URL = {0}", this.webUrl));
                this.logger.WriteError(string.Format("RESULT = {0}", this.webHtml));

            }

        }

        private void fixCookies(HttpWebRequest request, HttpWebResponse response, CookieContainer container)
        {
            CookieCollection cookies = new CookieCollection();

            for (int i = 0; i < response.Headers.Count; i++)
            {
                string name = response.Headers.GetKey(i);
                string value = response.Headers.Get(i);
                string subValue = "";
                Match match = null;

                if (name != "Set-Cookie")
                {
                    continue;
                }

                foreach (var singleCookie in value.Split(','))
                {
                    subValue += singleCookie;

                    match = Regex.Match(subValue, "(.+?)=(.+?); path=(.+?); domain=(.+?)");
                    if (match.Captures.Count == 0)
                    {
                        continue;
                    }

                    match = Regex.Match(subValue, "(.+?)=(.+?);");
                    Cookie newCookie = new Cookie(
                            match.Groups[1].ToString(),
                            match.Groups[2].ToString(),
                            "/",
                            request.Host.Split(':')[0]);
                    cookies.Add(newCookie);
                    subValue = "";

                }
            }

            container.Add(cookies);

        }

    }

}
