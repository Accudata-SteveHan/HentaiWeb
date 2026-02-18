using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using System.Web;

namespace HentaiCore {
    /// <summary>
    /// 網站登入處理
    /// </summary>
    public class AUTH {
        //登錄伺服器網址
        private string domain = "https://forums.e-hentai.org/";
        private string username = "", password = "";
        private Logger logger = null;

        public string loginCookie = "";
        public CookieContainer loginContainer = null;

        public AUTH(string user, string pass) {
            this.username = user;
            this.password = pass;

            this.loginContainer = new CookieContainer();

            this.logger = Central.logger;

        }

        public int Login() {
            string msg = "";

            try {
                //function log
                msg = "登入網站";
                logger.WriteInfo(msg);

                //data log
                msg = "UN = {0} , PW = {1}";
                logger.WriteTrace(string.Format(msg, this.username, this.password));

                //process log
                msg = "處理登入網站";
                logger.Write(this, msg);

                object[] webPass = this.Process();

                //process log
                msg = "完成處理登入網站";
                logger.Write(this, msg);

                string cookieStr = "ipb_member_id={0}; ipb_pass_hash={1};";
                string strCookie = String.Format(
                    cookieStr, webPass[0].ToString(), webPass[1].ToString());

                if (webPass[0].ToString() == "" || webPass[1].ToString() == "") {
                    return -1;
                }

                this.loginCookie = strCookie;
                this.loginContainer = (webPass[2] as HTML).cookieContainer;

                //data log
                msg = "COOKIE = {0}";
                logger.WriteTrace(string.Format(msg, strCookie));

                //function log
                msg = "完成登入網站";
                logger.WriteInfo(msg);

                return 0;

            } catch (Exception ex) {
                return -1;

            }

        }

        //public bool Check() {
        //    string msg = "";

        //    //function log
        //    msg = "測試登入網站";
        //    logger.WriteInfo(msg);

        //    //data log
        //    msg = "UN = {0} , PW = {1}";
        //    logger.WriteTrace(string.Format(msg, this.username, this.password));

        //    //process log
        //    msg = "處理測試登入網站";
        //    logger.Write(this, msg);

        //    object[] webPass = this.Process();

        //    //process log
        //    msg = "完成處理測試登入網站";
        //    logger.Write(this, msg);

        //    string m_ipbMemberID = webPass[0].ToString();
        //    string m_ipbPassHash = webPass[1].ToString();

        //    //data log
        //    msg = "MemberID = {0} , PassHash = {1}";
        //    logger.WriteTrace(string.Format(msg, m_ipbMemberID, m_ipbPassHash));

        //    //function log
        //    msg = "完成測試登入網站";
        //    logger.WriteInfo(msg);

        //    return m_ipbMemberID != "" && m_ipbPassHash != "";

        //}

        private object[] Process() {
            string url1 = "";
            HTML html = null;

            url1 = string.Format("{0}index.php?act=Login&CODE=00", domain);
            html = new HTML(url1);

            html.cookieContainer = this.loginContainer;
            html.GetHtml();

            url1 = string.Format("{0}index.php?act=Login&CODE=01", domain);

            //log
            Central.logger.WriteTrace(string.Format("URL = {0}", url1));

            byte[] byteArray = this.SetLoginData();

            Central.logger.Write(this, "連接登入網站");
            html.webUrl = url1;
            html.postData = byteArray;
            //html.cookieContainer = this.loginContainer;
            //html.GetHtml();
            Central.logger.Write(this, "完成連接登入網站");

            Uri uri = new Uri(domain);
            CookieCollection cookieCol = html.cookieContainer.GetCookies(uri);

            //string m_ipbMemberID = cookieCol["ipb_member_id"] != null ? cookieCol["ipb_member_id"].Value : "";
            //string m_ipbPassHash = cookieCol["ipb_pass_hash"] != null ? cookieCol["ipb_pass_hash"].Value : "";


            string m_ipbMemberID = "1404424"; //cookieCol["ipb_member_id"] != null ? cookieCol["ipb_member_id"].Value : "";
            string m_ipbPassHash = "64ba5de3cae50de0347a99e30f8daaa9";//cookieCol["ipb_pass_hash"] != null ? cookieCol["ipb_pass_hash"].Value : "";

            CookieContainer con = new CookieContainer();
            con.SetCookies(new Uri(domain), "ipb_member_id=1404424;");
            con.SetCookies(new Uri(domain), "ipb_pass_hash=64ba5de3cae50de0347a99e30f8daaa9;");
            html.cookieContainer = con;

            //log
            Central.logger.WriteTrace(string.Format("ipb_member_id = {0} , ipb_pass_hash = {1}", m_ipbMemberID, m_ipbPassHash));

            return new object[] { m_ipbMemberID, m_ipbPassHash, html };

        }

        private byte[] SetLoginData() {
            string web = domain + "index.php?act=Login&CODE=00";

            NameValueCollection outgoingQueryString = HttpUtility.ParseQueryString(String.Empty);
            outgoingQueryString.Add("referer", web);
            outgoingQueryString.Add("UserName", username);
            outgoingQueryString.Add("PassWord", password);
            outgoingQueryString.Add("CookieDate", "1");
            byte[] byteArray = Encoding.ASCII.GetBytes(outgoingQueryString.ToString());

            return byteArray;

        }

    }

}
