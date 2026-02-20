using HentaiCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;
//using System.Drawing;
using ImageMagick;
using System.Drawing;

namespace NewHentai.Controllers
{
    public partial class CrawController : Controller
    {
        private DataControlMdf control = new DataControlMdf();
        private string conStrMain = @"Data Source={0}\SQLEXPRESS;DATABASE=HENTAI;Integrated Security=false;User ID=sa;PASSWORD=c06g4cl6;Connection Timeout=180";
        private string conStrLib = @"Data Source={0}\SQLEXPRESS;DATABASE=ANIMELIBRARY;Integrated Security=false;User ID=sa;PASSWORD=c06g4cl6;Connection Timeout=180";
        private string conStrPic = @"Data Source={0}\SQLEXPRESS;DATABASE=HENTAIPIC;Integrated Security=false;User ID=sa;PASSWORD=c06g4cl6;Connection Timeout=180";
        private string conStrBook = @"Data Source={0}\SQLEXPRESS;DATABASE=MELONBOOK;Integrated Security=false;User ID=sa;PASSWORD=c06g4cl6;Connection Timeout=600";

        public CrawController()
        {

        }

        #region Core
        private DataTable QueryDBData(string sql, string conStr)
        {
            string server = System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("DB") ?
                System.Configuration.ConfigurationManager.AppSettings["DB"] :
                ".";

            conStr = string.Format(conStr, server);

            this.control.conStr = conStr;

            DataTable data = this.control.GetData(sql);
            return data;

        }

        private void UploadDBData(string[] sqls, string conStr)
        {
            try
            {
                string server = System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("DB") ?
                    System.Configuration.ConfigurationManager.AppSettings["DB"] :
                    ".";

                conStr = string.Format(conStr, server);

                this.control.conStr = conStr;
                int result = this.control.UploadData(sqls);

                if (result < 0)
                {
                    throw new Exception("存檔失敗");

                }

            }
            catch (Exception ex)
            {
                throw ex;

            }

        }

        private void UploadDBData(DataTable data, string mainSql, string conStr)
        {
            try
            {
                string server = System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("DB") ?
                    System.Configuration.ConfigurationManager.AppSettings["DB"] :
                    ".";

                conStr = string.Format(conStr, server);

                this.control.conStr = conStr;
                int result = this.control.UploadData(mainSql, data);

                if (result < 0)
                {
                    throw new Exception("存檔失敗");

                }

            }
            catch (Exception ex)
            {
                throw ex;

            }

        }

        private AUTH Login(string user, string password)
        {
            AUTH auth = new AUTH(user, password);
            if (auth.Login() >= 0)
            {
                return auth;

            }
            else
            {
                return null;

            }

        }

        #endregion Core

        #region DA
        private DataTable GetStatus(string key)
        {
            string sql = "SELECT * FROM STATUS WHERE 1=1 {0}";
            string filter = "";

            if (key != "")
            {
                filter += string.Format(" and WEB = '{0}'", key);

            }

            sql = string.Format(sql, filter);

            DataTable data = this.QueryDBData(sql, conStrMain);
            data.PrimaryKey = new DataColumn[] { data.Columns["WEB"] };

            return data;

        }

        private void UpdateStatus(DataTable data)
        {
            string sql = "SELECT * FROM STATUS";

            this.UploadDBData(data, sql, conStrMain);

        }

        private DataTable GetObject(string key)
        {
            string sql = "SELECT * FROM OBJECT WHERE 1=1 AND WEB_ID = 'EXHENTAI' {0} ORDER BY STATUS , UPDATE_TIME";
            string filter = "";
            if (key == null)
            {
                filter += string.Format(
                    " and (STATUS = '0' or STATUS = '1')",
                    key);
            }
            else if (key != "")
            {
                filter += string.Format(" and ID = '{0}'", key);

            }


            sql = string.Format(sql, filter);

            DataTable data = this.QueryDBData(sql, conStrMain);
            data.PrimaryKey = new DataColumn[] { data.Columns["ID"] };

            return data;

        }

        private void UpdateObject(DataTable data)
        {
            string sql = "SELECT * FROM OBJECT";

            this.UploadDBData(data, sql, conStrMain);

        }

        private DataTable GetPicObject(string key)
        {
            string sql = "SELECT * FROM OBJECT WHERE 1=1 AND WEB_ID = 'EXHENTAI' {0} ORDER BY STATUS , UPDATE_TIME";
            string filter = "";
            if (key == null)
            {
                filter += string.Format(
                    " and (STATUS = '1' or STATUS = '2')",
                    key);
            }
            else if (key != "")
            {
                filter += string.Format(" and ID = '{0}'", key);

            }


            sql = string.Format(sql, filter);

            DataTable data = this.QueryDBData(sql, conStrMain);
            data.PrimaryKey = new DataColumn[] { data.Columns["ID"] };

            return data;

        }

        private DataTable GetDataExHentai(string pKey , string sKey)
        {
            string sql = "SELECT * FROM DATA_EXHENTAI WHERE 1=1 {0}";
            string filter = "";

            if (pKey != "")
            {
                filter += string.Format(" and PKEY = '{0}'", pKey);

            }

            if (sKey != "")
            {
                filter += string.Format(" and SKEY = '{0}'", sKey);

            }

            sql = string.Format(sql, filter);

            DataTable data = this.QueryDBData(sql, conStrMain);
            data.PrimaryKey = new DataColumn[] { data.Columns["OBJ_ID"] };

            return data;

        }

        private void UpdateDataExHentai(DataTable data)
        {
            string sql = "SELECT * FROM DATA_EXHENTAI";

            this.UploadDBData(data, sql, conStrMain);

        }

        private DataTable GetGalleryBookData(string pKey, string sKey)
        {
            string sql = "";

            sql = "SELECT * FROM DATA_EXHENTAI_DETAIL WHERE 1=1 and PKEY = '{0}' and SKEY = '{1}' ";
            sql = string.Format(sql, pKey, sKey);
            DataTable dataBook = this.QueryDBData(sql, conStrLib);

            return dataBook;

        }

        private DataTable GetDataExHentaiPic(string pKey, string sKey)
        {
            string sql = "SELECT * FROM DATA_EXHENTAI_PIC WHERE 1=1 {0}";
            string filter = "";

            if (pKey != "")
            {
                filter += string.Format(" and PKEY = '{0}'", pKey);

            }

            if (sKey != "")
            {
                filter += string.Format(" and SKEY = '{0}'", sKey);

            }

            sql = string.Format(sql, filter);

            DataTable data = this.QueryDBData(sql, conStrPic);
            data.PrimaryKey = new DataColumn[] { data.Columns["PKEY"], data.Columns["SKEY"], data.Columns["PAGE"] };

            return data;

        }

        private void UpdateDataExHentaiPic(DataTable data)
        {
            string sql = "SELECT * FROM DATA_EXHENTAI_PIC";

            this.UploadDBData(data, sql, conStrPic);

        }

        #endregion DA

        // GET: Craw
        public ActionResult Index()
        {
            return View();

        }

        // --- Helpers ported from JOB and JOB_* classes (inlined into this controller) ---
        private string FetchWeb(string url, AUTH auth = null, int retry = 3)
        {
            AUTH au = auth ?? Central.authUser;
            HTML web = new HTML();
            web.webUrl = url;
            if (au != null)
            {
                web.webCookie = au.loginCookie;
                web.cookieContainer = au.loginContainer;
            }

            string html = "";
            for (int i = 0; i < retry; i++)
            {
                try
                {
                    web.GetHtml();
                    html = web.webHtml;
                    if (!string.IsNullOrEmpty(html) && html.ToLower().Contains("html"))
                    {
                        break;
                    }
                }
                catch
                {
                    // swallow and retry
                }
            }

            return html;
        }

        private Stream FetchStream(string url, AUTH auth = null, int retry = 3)
        {
            AUTH au = auth ?? Central.authUser;
            HTML web = new HTML();
            web.webUrl = url;
            if (au != null)
            {
                web.webCookie = au.loginCookie;
                web.cookieContainer = au.loginContainer;
            }

            for (int i = 0; i < retry; i++)
            {
                try
                {
                    web.GetStream();
                    return web.webStream;
                }
                catch
                {
                    // retry
                }
            }

            return null;
        }

        private string GetCoverUrl(string mainUrl, string pKey, string sKey, AUTH auth)
        {
            string urlTemplate = "{2}/g/{0}/{1}/?nw=session";
            string url = string.Format(urlTemplate, pKey, sKey, mainUrl);

            HTML web = new HTML(url);
            web.webCookie = (auth ?? Central.authUser).loginCookie;
            web.cookieContainer = (auth ?? Central.authUser).loginContainer;
            try
            {
                web.cookieContainer.Add(new Cookie("nw", "1", "/", "e-hentai.org"));
            }
            catch { }
            web.GetHtml();
            string pageHtml = web.webHtml;

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(pageHtml);
            HtmlNode root = doc.DocumentNode;

            string xpath = "/html[1]/body[1]/div[2]/div[1]/div[1]/div[1]";
            HtmlNode found = root.SelectSingleNode(xpath);
            if (found != null && found.Attributes["style"] != null)
            {
                Match match = Regex.Match(found.Attributes["style"].Value, @"url\((.+?)\)");
                if (match.Length != 0)
                {
                    return match.Groups[1].Value;
                }
            }
            else
            {
                xpath = "/html[1]/body[1]/div[2]/div[1]/div[1]";
                found = root.SelectSingleNode(xpath);
                if (found != null && found.Attributes["style"] != null)
                {
                    Match match = Regex.Match(found.Attributes["style"].Value, @"url\((.+?)\)");
                    if (match.Length != 0)
                    {
                        return match.Groups[1].Value;
                    }
                }
            }

            return "";
        }

        private BOOK_INFO GetBookInfoFromThread(string threadUrl, string domain, AUTH auth)
        {
            BOOK_INFO book_info = new BOOK_INFO();

            try
            {
                string html = FetchWeb(threadUrl, auth);

                string pKey = threadUrl.Replace(domain + "/g/", "").Split('/')[0];
                string sKey = threadUrl.Replace(domain + "/g/", "").Split('/')[1];

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                HtmlNode node = doc.DocumentNode;

                Func<string, HtmlNode> GetNode = (path) =>
                {
                    try { return node.SelectSingleNode(path); } catch { return null; }
                };

                string strGetTypePath = "/html[1]/body[1]/div[2]/div[3]/div[1]/div[1]/div[1]";
                string strGetUnloadTimePath = "/html[1]/body[1]/div[2]/div[3]/div[1]/div[3]/table[1]/tr[1]/td[2]";
                string strGetEngTitlePath = "/html[1]/body[1]/div[2]/div[2]/h1[1]";
                string strGetUniTitlePath = "/html[1]/body[1]/div[2]/div[2]/h1[2]";
                string strGetPostTimePath = "/html[1]/body[1]/div[2]/div[3]/div[1]/div[3]/table[1]/tr[1]/td[2]";
                string strGetLanguagePath = "/html[1]/body[1]/div[2]/div[3]/div[1]/div[3]/table[1]/tr[4]/td[2]";
                string strGetTotalPagePath = "/html[1]/body[1]/div[2]/div[3]/div[1]/div[3]/table[1]/tr[6]/td[2]";
                string strGetRateTimePath = "/html[1]/body[1]/div[2]/div[3]/div[1]/div[4]/table[1]/tr[1]/td[3]/span[1]";
                string strGetRateAvgPath = "/html[1]/body[1]/div[2]/div[3]/div[1]/div[4]/table[1]/tr[2]/td[1]";
                string strGetTabCountPath1 = "/html[1]/body[1]/div[3]/table[1]/tr[1]";
                string strGetTabCountPath2 = "/html[1]/body[1]/div[4]/table[1]/tr[1]";
                string strGetTabCountPath3 = "/html[1]/body[1]/div[5]/table[1]/tr[1]";
                string strGetTabCountPath4 = "/html[1]/body[1]/div[6]/table[1]/tr[1]";

                book_info = new BOOK_INFO(threadUrl, pKey, sKey);

                var n = GetNode(strGetEngTitlePath);
                if (n != null) book_info.eng_title = n.InnerHtml;

                n = GetNode(strGetUniTitlePath);
                if (n != null) book_info.uni_title = n.InnerHtml;

                n = GetNode(strGetLanguagePath);
                if (n != null)
                {
                    try
                    {
                        string text = n.FirstChild.OuterHtml
                            .Replace(" &nbsp;", "")
                            .Replace("<span class=\"halp\" title=\"This gallery has been translated from the original language text.\">TR</span>", "");
                        book_info.language = text;
                    }
                    catch { book_info.language = n.InnerText; }
                }

                n = GetNode(strGetTypePath);
                if (n != null) book_info.type = n.InnerText.ToLower();

                n = GetNode(strGetTotalPagePath);
                if (n != null && !string.IsNullOrEmpty(n.InnerHtml))
                {
                    if (int.TryParse(n.InnerHtml.Replace(" pages", ""), out int tp)) book_info.total_page = tp;
                }

                // Tab count
                int tabCount = 0;
                HtmlNode tmpNode = null;
                string[] tabPaths = new string[] { strGetTabCountPath1, strGetTabCountPath2, strGetTabCountPath3, strGetTabCountPath4 };
                foreach (var path in tabPaths)
                {
                    tmpNode = GetNode(path);
                    if (tmpNode != null)
                    {
                        try
                        {
                            var child = tmpNode.ChildNodes[tmpNode.ChildNodes.Count - 2];
                            if (child != null && int.TryParse(child.InnerText, out tabCount))
                            {
                                break;
                            }
                        }
                        catch { }
                    }
                }
                book_info.tab_count = tabCount;

                n = GetNode(strGetUnloadTimePath);
                if (n != null) book_info.upload_time = n.InnerHtml;

                n = GetNode(strGetPostTimePath);
                if (n != null) book_info.posted_time = n.InnerHtml;

                n = GetNode(strGetRateTimePath);
                if (n != null) book_info.rate_time = decimal.TryParse(n.InnerHtml, out decimal rt) ? rt : 0;

                n = GetNode(strGetRateAvgPath);
                if (n != null) book_info.rate_avg = decimal.TryParse(n.InnerHtml.Replace("Average: ", ""), out decimal ra) ? ra : 0;

                // Cover
                try
                {
                    string coverUrl = GetCoverUrl(domain, pKey, sKey, auth);
                    if (!string.IsNullOrEmpty(coverUrl))
                    {
                        book_info.pic = coverUrl;
                        HTML picHtml = new HTML(book_info.pic);
                        picHtml.webCookie = (auth ?? Central.authUser).loginCookie;
                        picHtml.cookieContainer = (auth ?? Central.authUser).loginContainer;
                        picHtml.GetStream();
                        book_info.pic_Stream = picHtml.webStream;
                    }
                }
                catch (Exception ex)
                {
                    Central.logger.WriteError($"GetBookInfoFromThread cover fetch error: {ex}");
                }
            }
            catch (Exception ex)
            {
                Central.logger.WriteError($"GetBookInfoFromThread error: {ex}");
            }

            return book_info;
        }

        private List<PIC_INFO> GetDLList(BOOK_INFO bookInfo, string domain, AUTH auth, string divCount = "6")
        {
            List<PIC_INFO> pic_info = new List<PIC_INFO>();
            try
            {
                int totalCount = 0;
                decimal tabCount = bookInfo.tab_count;
                string webTemplate = bookInfo.threadUrl + "?p={0}";

                for (int i = 0; i < tabCount; i++)
                {
                    string url = string.Format(webTemplate, i);
                    string html = FetchWeb(url, auth);
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(html);
                    HtmlNode rootNode = doc.DocumentNode;

                    string tableXPath = "/html[1]/body[1]/div[{0}]";
                    tableXPath = string.Format(tableXPath, divCount);
                    HtmlNode tableNode = rootNode.SelectSingleNode(tableXPath);
                    if (tableNode == null) continue;

                    foreach (HtmlNode node in tableNode.ChildNodes)
                    {
                        if (!node.XPath.StartsWith(tableXPath))
                        {
                            continue;
                        }

                        totalCount++;

                        // original code tried up to 3 times with different xpaths; we keep a simple approach:
                        HtmlNode imgNode = rootNode.SelectSingleNode(node.XPath);
                        if (imgNode != null)
                        {
                            // create PIC_INFO
                            PIC_INFO picInfo = new PIC_INFO();
                            picInfo.threadUrl = bookInfo.pageUrl;
                            picInfo.picNo = totalCount.ToString().PadLeft(bookInfo.total_page.ToString().Length, '0');

                            // Try to get href attribute if exists on the node or its descendants
                            string href = null;
                            if (imgNode.Attributes["href"] != null)
                            {
                                href = imgNode.Attributes["href"].Value;
                            }
                            else
                            {
                                var aNode = imgNode.SelectSingleNode(".//a[@href]");
                                if (aNode != null && aNode.Attributes["href"] != null)
                                {
                                    href = aNode.Attributes["href"].Value;
                                }
                            }

                            if (!string.IsNullOrEmpty(href))
                            {
                                picInfo.picUrl = href;
                                picInfo.picFileName = Central.PathDirector.GetSaveFileName(bookInfo.p_key, bookInfo.s_key, picInfo.picNo);
                                pic_info.Add(picInfo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Central.logger.WriteError($"GetDLList error: {ex}");
            }

            return pic_info;
        }

        private void ProcessDeepDLList_Local(List<PIC_INFO> pic_info, AUTH auth)
        {
            try
            {
                for (int i = 0; i < pic_info.Count; i++)
                {
                    string picUrl = "";
                    try
                    {
                        PIC_INFO picInfo = pic_info[i];
                        picUrl = picInfo.picUrl;
                        string html = FetchWeb(picInfo.picUrl, auth);

                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(html);

                        HtmlNode rootNode = doc.DocumentNode;
                        string xpath = "/html[1]/body[1]/div[1]/div[5]/a[2]";
                        var node = rootNode.SelectSingleNode(xpath);
                        if (node != null && node.Attributes["onclick"] != null)
                        {
                            string imgUrl = node.Attributes["onclick"].Value;
                            Regex imgAdd = new Regex("return nl\\('(.+?)-(.+?)'\\)");
                            var m = imgAdd.Match(imgUrl);
                            if (m.Success)
                            {
                                picInfo.picUrl = string.Format("{0}?nl={1}-{2}", picInfo.picUrl, m.Groups[1], m.Groups[2]);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Central.logger.WriteInfo(string.Format(
                            "異常處理深度頁面圖片 : {1}_{0}_{2}",
                            picUrl,
                            i.ToString().PadLeft(pic_info.Count.ToString().Length, '0'),
                            ex.ToString()));
                    }

                    System.Threading.Thread.Sleep(new Random(int.Parse(DateTime.Now.ToString("MMddmmss"))).Next(1000, 2500));
                }
            }
            catch (Exception ex)
            {
                Central.logger.WriteError($"ProcessDeepDLList_Local error: {ex}");
            }
        }

        private void DownloadPic_Local(PIC_INFO pic_info, string save_path, AUTH auth)
        {
            try
            {
                Central.logger.WriteInfo(string.Format("開始下載圖片 : {0}", pic_info.picUrl));
                string html = FetchWeb(pic_info.picUrl, auth);

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                HtmlNode rootNode = doc.DocumentNode;

                string xpath = "/html[1]/body[1]/div[1]/div[2]/a[1]/img[1]";
                HtmlNode imgNode = rootNode.SelectSingleNode(xpath);
                if (imgNode == null || imgNode.Attributes["src"] == null)
                {
                    Central.logger.WriteError($"Image node not found for {pic_info.picUrl}");
                    return;
                }

                string img = imgNode.Attributes["src"].Value;

                Central.logger.Write(this, string.Format("下載圖片頁面 : {0} 下載圖片:{1}", pic_info.picUrl, img));
                using (Stream imgStream = FetchStream(img, auth))
                {
                    if (imgStream == null)
                    {
                        Central.logger.WriteError($"Failed to get stream for image: {img}");
                        return;
                    }

                    try
                    {
                        if (!Directory.Exists(save_path))
                        {
                            Directory.CreateDirectory(save_path);
                        }

                        imgStream.Seek(0, SeekOrigin.Begin);

                        using (var magickImage = new MagickImage(imgStream))
                        {
                            using (var jpgStream = new MemoryStream())
                            {
                                magickImage.Format = MagickFormat.Jpg;
                                magickImage.Write(jpgStream);
                                jpgStream.Position = 0;

                                using (Image i = Image.FromStream(jpgStream))
                                {
                                    string dest = Path.Combine(save_path, string.Format("{0}.jpg", pic_info.picFileName));
                                    i.Save(dest);
                                }
                            }
                        }

                        pic_info.success = true;
                        Central.logger.WriteInfo(string.Format("完成下載圖片 : {0}", pic_info.picUrl));
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                }
            }
            catch (Exception ex)
            {
                Central.logger.WriteError(string.Format("DownloadPic_Local 錯誤，{0}", ex.ToString()));
            }
        }

        // --- End helpers ---

        public ActionResult GET_EXHENTAI_PIC(string param)
        {
            string id = string.Format("EX_{0}", param);

            try
            {
                Regex regex = new Regex(@"EX_(\w+)_(\w+)");

                string pKey = regex.Match(id).Groups[1].Value;
                string sKey = regex.Match(id).Groups[2].Value;

                DataTable dataExHentai = this.GetDataExHentai(pKey, sKey);
                DataTable dataExHentaiDetail = this.GetGalleryBookData(pKey, sKey);
                DataTable dataExHentaiPic = this.GetDataExHentaiPic(pKey, sKey);

                string localPath = System.Configuration.ConfigurationManager.AppSettings["LocalSavePath"].ToString();
                int totalPage = int.Parse(dataExHentaiDetail.Rows[0]["PAGE"].ToString());
                DirectoryInfo dirInfo = new DirectoryInfo(string.Format(Server.MapPath("/{0}/{1}"), localPath, pKey.PadLeft(7, '0')));

                if (dataExHentaiPic.Rows.Count == 0 ||
                    dataExHentaiPic.Rows.Count != totalPage ||
                    !dirInfo.Exists ||
                    dirInfo.GetFiles().Length == 0 ||
                    dirInfo.GetFiles().Length != totalPage)
                {
                    string url = "{0}/CRAW/CRAW_EXHENTAI_PIC/{1}_{2}";
                    string basePage = System.Configuration.ConfigurationManager.AppSettings["BasePage"].ToString();
                    url = string.Format(url, basePage, pKey, sKey);

                    HttpWebRequest req = HttpWebRequest.Create(url) as HttpWebRequest;
                    req.Timeout = 1000 * 15 * totalPage;

                    using (HttpWebResponse res = req.GetResponse() as HttpWebResponse)
                    {
                        if (res.StatusCode != HttpStatusCode.OK)
                        {
                            throw new Exception("圖片擷取失敗");

                        }

                    }
                }

                ViewData["DATA"] = "";

            }
            catch (Exception ex)
            {
                ViewData["MSG"] = ex.ToString();

            }

            return View();

        }

        public ActionResult CRAW_EXHENTAI_PIC(string param)
        {
            DataTable dataObject = null;
            string id = "";

            if (param == null)
            {
                dataObject = this.GetPicObject(param);

                id = dataObject.Rows[0]["ID"].ToString();

            }
            else if (param != "")
            {
                id = string.Format("EX_{0}", param);

                dataObject = this.GetPicObject(id);

            }

            dataObject.Rows[0]["STATUS"] = "5";
            dataObject.Rows[0]["UPDATE_TIME"] = DateTime.Now;

            try
            {
                Regex regex = new Regex(@"EX_(\w+)_(\w+)");

                string pKey = regex.Match(id).Groups[1].Value;
                string sKey = regex.Match(id).Groups[2].Value;
                
                DataTable dataExHentaiDetail = this.GetGalleryBookData(pKey, sKey);
                DataTable dataExHentaiPic = this.GetDataExHentaiPic(pKey, sKey);

                //Json jsonObject = new Json();

                string baseUrl = "https://exhentai.org";
                string url = "https://exhentai.org/g/{0}/{1}/";
                url = string.Format(url, pKey, sKey);

                Central.logger = new Logger();
                Central.PathDirector = new FileDirector();

                if (Central.authUser == null)
                {
                    Central.authUser = this.Login("username", "password");

                }

                // Replaced JOB_THREAD: fetch book info directly
                BOOK_INFO bookInfo = GetBookInfoFromThread(url, baseUrl, Central.authUser);

                // Replaced JOB_DL_LIST: get pic list
                List<PIC_INFO> picList = new List<PIC_INFO>();
                for (int div = 5; div <= 7; div++)
                {
                    picList = GetDLList(bookInfo, baseUrl, Central.authUser, div.ToString());
                    if (picList.Count > 0) break;
                }

                foreach (PIC_INFO picInfo in picList)
                {
                    DataRow rowPic = null;

                    string picUrl = picInfo.picUrl;
                    string imgUrl = picInfo.picImgUrl;
                    string page = picInfo.picNo;

                    DataRow[] rows = dataExHentaiPic.Select(
                        string.Format("PKEY = '{0}' and PAGE = '{1}'", pKey, page));
                    if (rows.Length == 0)
                    {
                        rowPic = dataExHentaiPic.NewRow();
                        rowPic["PKEY"] = pKey;
                        rowPic["SKEY"] = sKey;
                        rowPic["PAGE"] = page;
                        rowPic["SAVE_PATH"] = Path.Combine(
                            @"Gallery\Image",
                            string.Format("{0}_{1}_{2}", pKey, sKey, page));
                        rowPic["LOAD_TIME"] = DateTime.Now;

                        JObject obj = new JObject();
                        obj.Add("TYPE", "ADD");
                        obj.Add("PKEY", pKey);
                        obj.Add("SKEY", sKey);
                        obj.Add("PAGE", page);

                        //jsonObject.Add(string.Format("{0}_{1}_{2}", pKey, sKey, page), obj);

                    }
                    else
                    {
                        rowPic = rows[0];

                        JObject obj = new JObject();
                        obj.Add("TYPE", "EDIT");
                        obj.Add("PKEY", pKey);
                        obj.Add("SKEY", sKey);
                        obj.Add("PAGE", page);

                        //jsonObject.Add(string.Format("{0}_{1}_{2}", pKey, sKey, page), obj);

                    }

                    rowPic["URL"] = picUrl;
                    rowPic["PIC_URL"] = imgUrl;
                    rowPic["USER_ID"] = "PIC";
                    rowPic["UPDATE_TIME"] = DateTime.Now;

                    if (rows.Length == 0)
                    {
                        dataExHentaiPic.Rows.Add(rowPic);

                    }

                }

                this.UpdateDataExHentaiPic(dataExHentaiPic);
                this.UpdateObject(dataObject);

                //ViewData["DATA"] = jsonObject.ToString();

            }
            catch (Exception ex)
            {
                ViewData["MSG"] = ex.ToString();

            }

            return View();

        }

        public ActionResult CRAW_EXHENTAIDEEP_PIC(string param)
        {
            DataTable dataObject = null;
            string id = "";

            if (param == null)
            {
                dataObject = this.GetPicObject(param);

                id = dataObject.Rows[0]["ID"].ToString();

            }
            else if (param != "")
            {
                id = string.Format("EX_{0}", param);

                dataObject = this.GetPicObject(id);

            }

            dataObject.Rows[0]["STATUS"] = "5";
            dataObject.Rows[0]["UPDATE_TIME"] = DateTime.Now;

            try
            {
                Regex regex = new Regex(@"EX_(\w+)_(\w+)");

                string pKey = regex.Match(id).Groups[1].Value;
                string sKey = regex.Match(id).Groups[2].Value;

                DataTable dataExHentaiDetail = this.GetGalleryBookData(pKey, sKey);
                DataTable dataExHentaiPic = this.GetDataExHentaiPic(pKey, sKey);

                //Json jsonObject = new Json();

                string baseUrl = "https://exhentai.org";
                string url = "https://exhentai.org/g/{0}/{1}/";
                url = string.Format(url, pKey, sKey);

                Central.logger = new Logger();
                Central.PathDirector = new FileDirector();

                if (Central.authUser == null)
                {
                    Central.authUser = this.Login("username", "password");

                }

                // Replaced JOB_THREAD
                BOOK_INFO bookInfo = GetBookInfoFromThread(url, baseUrl, Central.authUser);

                // Replaced JOB_DL_LIST deep logic
                List<PIC_INFO> picList = GetDLList(bookInfo, baseUrl, Central.authUser, "6");
                ProcessDeepDLList_Local(picList, Central.authUser);

                if (picList.Count == 0)
                {
                    picList = GetDLList(bookInfo, baseUrl, Central.authUser, "7");
                    ProcessDeepDLList_Local(picList, Central.authUser);
                }

                foreach (PIC_INFO picInfo in picList)
                {
                    DataRow rowPic = null;

                    string picUrl = picInfo.picUrl;
                    string imgUrl = picInfo.picImgUrl;
                    string page = picInfo.picNo;

                    DataRow[] rows = dataExHentaiPic.Select(
                        string.Format("PKEY = '{0}' and PAGE = '{1}'", pKey, page));
                    if (rows.Length == 0)
                    {
                        rowPic = dataExHentaiPic.NewRow();
                        rowPic["PKEY"] = pKey;
                        rowPic["SKEY"] = sKey;
                        rowPic["PAGE"] = page;
                        rowPic["SAVE_PATH"] = Path.Combine(
                            @"Gallery\Image",
                            string.Format("{0}_{1}_{2}", pKey, sKey, page));
                        rowPic["LOAD_TIME"] = DateTime.Now;

                        JObject obj = new JObject();
                        obj.Add("TYPE", "ADD");
                        obj.Add("PKEY", pKey);
                        obj.Add("SKEY", sKey);
                        obj.Add("PAGE", page);

                        //jsonObject.Add(string.Format("{0}_{1}_{2}", pKey, sKey, page), obj);

                    }
                    else
                    {
                        rowPic = rows[0];

                        JObject obj = new JObject();
                        obj.Add("TYPE", "EDIT");
                        obj.Add("PKEY", pKey);
                        obj.Add("SKEY", sKey);
                        obj.Add("PAGE", page);

                        //jsonObject.Add(string.Format("{0}_{1}_{2}", pKey, sKey, page), obj);

                    }

                    rowPic["URL"] = picUrl;
                    rowPic["PIC_URL"] = imgUrl;
                    rowPic["USER_ID"] = "PIC";
                    rowPic["UPDATE_TIME"] = DateTime.Now;

                    if (rows.Length == 0)
                    {
                        dataExHentaiPic.Rows.Add(rowPic);

                    }

                }

                this.UpdateDataExHentaiPic(dataExHentaiPic);
                this.UpdateObject(dataObject);

                //ViewData["DATA"] = jsonObject.ToString();

            }
            catch (Exception ex)
            {
                ViewData["MSG"] = ex.ToString();

            }

            return View();

        }

        public ActionResult RESET_EXHENTAI_DATA_ERROR_CODE(string param)
        {
            string id = "";
            string key = "EX_{0}_{1}";
            DataTable data = null;

            JObject result = new JObject();

            if (param != null && param.Split('_').Length == 2)
            {
                string pKey = param.Split('_')[0];
                string sKey = param.Split('_')[1];
                id = string.Format(key, pKey, sKey);

                data = this.GetObject(id);

                if (data.Rows.Count == 0)
                {
                    result.Add("RESULT", "找不到資料");

                }
                else if (data.Rows.Count > 1)
                {
                    result.Add("RESULT", "資料筆數非唯一");

                }

            }
            else
            {
                string sql = "SELECT * FROM OBJECT WHERE STATUS = '9'";
                data = this.QueryDBData(sql, conStrMain);
                data.PrimaryKey = new DataColumn[] { data.Columns["ID"] };

            }

            foreach (DataRow row in data.Rows)
            {
                row["STATUS"] = "0";
                row["UPDATE_USER"] = "CRAW";
                row["UPDATE_TIME"] = DateTime.Now;

            }

            this.UpdateObject(data);

            result.Add("RESULT", "OK");

            ViewData["RESULT"] = result.ToString();

            return View();

        }

        public ActionResult RESET_EXHENTAI_DATA_PAGE(string param)
        {
            string id = "";
            string key = "EXHENTAI_{0}";
            DataTable data = null;

            JObject result = new JObject();

            if (param != null)
            {
                id = string.Format(key, param);

                data = this.GetStatus(id);

                if (data.Rows.Count == 0)
                {
                    result.Add("RESULT", "找不到資料");

                }
                else if (data.Rows.Count > 1)
                {
                    result.Add("RESULT", "資料筆數非唯一");

                }

            }
            else
            {
                string sql = "SELECT * FROM STATUS WHERE WEB LIKE 'EXHENTAI_%'";
                data = this.QueryDBData(sql, conStrMain);
                data.PrimaryKey = new DataColumn[] { data.Columns["WEB"] };

            }

            foreach (DataRow row in data.Rows)
            {
                row["PROC_PAGE"] = "0";
                row["UPDATE_TIME"] = DateTime.Now;

            }

            this.UpdateStatus(data);
            result.Add("RESULT", "OK");

            ViewData["RESULT"] = result.ToString();

            return View();

        }

        public ActionResult RESET_EXHENTAI_DATA_MAXKEY(string param)
        {
            string id = "";
            string key = "EXHENTAI_{0}";
            DataTable data = null;

            JObject result = new JObject();

            if (param != null)
            {
                id = string.Format(key, param);

                data = this.GetStatus(id);

                if (data.Rows.Count == 0)
                {
                    result.Add("RESULT", "找不到資料");

                }
                else if (data.Rows.Count > 1)
                {
                    result.Add("RESULT", "資料筆數非唯一");

                }

            }
            else
            {
                string sql = "SELECT * FROM STATUS WHERE WEB LIKE 'EXHENTAI_%'";
                data = this.QueryDBData(sql, conStrMain);
                data.PrimaryKey = new DataColumn[] { data.Columns["WEB"] };

            }

            foreach (DataRow row in data.Rows)
            {
                row["LAST_KEY"] = "9999999";
                row["UPDATE_TIME"] = DateTime.Now;

            }

            this.UpdateStatus(data);
            result.Add("RESULT", "OK");

            ViewData["RESULT"] = result.ToString();

            return View();

        }

        public ActionResult DOWNLOAD_EXHENTAI_PIC(string param)
        {
            if (param == null)
            {
                return View();

            }

            string localPath = System.Configuration.ConfigurationManager.AppSettings["LocalSavePath"].ToString();
            string webPath = System.Configuration.ConfigurationManager.AppSettings["WebSavePath"].ToString();

            if (Central.authUser == null)
            {
                Central.authUser = this.Login("username", "password");

            }

            string[] p = param.ToString().Split('_');
            string pKey = p[0];
            string sKey = p[1];
            string imagePage = p[2];

            string path = string.Format(Server.MapPath("/{0}/{1}"), localPath, pKey.PadLeft(7, '0'));
            

            DataRow[] picRows = this.GetDataExHentaiPic(pKey, sKey).Select(string.Format("PAGE = '{0}'", imagePage));
            if (picRows.Length == 0)
            {
                return View();

            }
            DataRow picRow = picRows[0];

            Random sleepTime = new Random(int.Parse(DateTime.Now.ToString("HHmmss")));
            Thread.Sleep(sleepTime.Next(100, 250) *
                sleepTime.Next(
                    (int.Parse(imagePage) / 10) <= 10 ? 1 : (int.Parse(imagePage) / 10),
                    (int.Parse(imagePage) / 05) <= 10 ? 10 : (int.Parse(imagePage) / 05)
                    )
                );

            Central.logger = new Logger();

            PIC_INFO picInfo = new PIC_INFO();
            picInfo.picUrl = picRow["URL"].ToString();
            picInfo.picImgUrl = picRow["PIC_URL"].ToString();
            picInfo.picNo = picRow["PAGE"].ToString();
            picInfo.picFileName = param;

            // Replaced JOB_PIC: download directly
            DownloadPic_Local(picInfo, path, Central.authUser);

            FileInfo fileInfoSrc = new FileInfo(Path.Combine(string.Format(Server.MapPath("/{0}/{1}"), localPath, pKey.PadLeft(7, '0')), param + ".jpg"));
            FileInfo fileInfoDst = new FileInfo(Path.Combine(string.Format(Server.MapPath("/{0}/{1}"), webPath, pKey.PadLeft(7, '0')), param + ".jpg"));
            if (fileInfoSrc.Exists)
            {
                fileInfoSrc.CopyTo(fileInfoDst.FullName, false);

            }
            
            return View();

        }

    }

    public class PIC_INFO
    {
        public bool success = false;
        public string threadUrl = "";
        public string picNo = "";
        public string picFileName = "";
        public string picUrl = "";
        public Stream picStream = null;
        public Image picImage = null;
        public string picImgUrl = "";

    }

    public class THREAD_INFO
    {
        public string page_url = "";
        public string thread_url = "";
        public bool exhentai = false;

        public THREAD_INFO(string threadUrl)
        {
            this.thread_url = threadUrl;

        }

    }

    public class SITE_INFO
    {
        public string host = "";
        public string param = "";

        public string site_url = "";
        public int page_info_count = 0;

        public SITE_INFO(string site)
        {
            this.site_url = site;

            Uri u = new Uri(site);
            this.host = u.Host;
            //this.param = u.Query.Substring(1);


        }

    }

    public class PAGE_INFO
    {
        public string page_url;
        public string site_url;
        public int page_threadCount = 0;

        public PAGE_INFO(string pageUrl)
        {
            this.page_url = pageUrl;

        }

    }

    public class BOOK_INFO
    {
        public string pageUrl = "";
        public string threadUrl = "";
        public string type;
        public string upload_time;
        public string posted_time;
        public string pic;
        public string eng_title;
        public string uni_title;
        public decimal total_page;
        public string p_key;
        public string s_key;
        public string language;
        public decimal rate_time;
        public decimal rate_avg;
        public decimal tab_count;
        public Stream pic_Stream;

        //public BOOK_TYPE typeObj = null;

        public BOOK_INFO()
        {

        }

        public BOOK_INFO(string thread)
        {
            this.threadUrl = thread;
        }

        public BOOK_INFO(string thread, string pKey, string sKey)
        {
            this.threadUrl = thread;
            this.p_key = pKey;
            this.s_key = sKey;

        }

    }

}