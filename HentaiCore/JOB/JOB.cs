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

namespace HentaiCore {
    //工作基底
    public class JOB {
        public AUTH authUser = null;
        public string domain = "";
        HTML web = null;

        protected Logger logger = null;

        int retry = 3;

        public JOB(){
            this.web = new HTML();
            this.logger = Central.logger;

        }

        private void InitWebConfig(string url) {
            web.webUrl = url;
            web.webCookie = Central.authUser.loginCookie;
            web.cookieContainer = Central.authUser.loginContainer;

        }

        protected string GetWeb(string url){
            int i = 0;
            this.logger.Write(this, string.Format("開始 JOB GetWeb = {0}" , url));
            for (i = 0 ; i < retry ; i++) {
                this.logger.WriteTrace(string.Format("GetWeb : {0} URL = {1}", i, url));
                this.InitWebConfig(url);
                web.GetHtml();

                if (web.webHtml.ToLower().Contains("html")) {
                    break;
                    
                }

                this.logger.WriteTrace(string.Format("GetWeb Fail : {0} URL = {1}", i, url));

            }

            this.logger.Write(this, string.Format("完成 JOB GetWeb = {0}", url));
            return web.webHtml;

        }

        protected Stream GetStream(string url) {
            int i = 0;
            this.logger.Write(this, string.Format("開始 JOB GetStream = {0}", url));

            for (i = 0 ; i < retry ; i++) {
                try {
                    this.InitWebConfig(url);
                    web.GetStream();

                    break;

                } catch (Exception ex) {
                    this.logger.WriteTrace(string.Format("GetStream Fail : {0} URL = {1}", i, url));

                }
                

            }

            this.logger.Write(this, string.Format("完成 JOB GetStream = {0}", url));
            return web.webStream;

        }

    }

}
