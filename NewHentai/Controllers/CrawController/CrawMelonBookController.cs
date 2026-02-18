using HentaiCore;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace NewHentai.Controllers
{
    public class CrawMelonBookController : Controller
    {
        private DataControlMdf control = new DataControlMdf();
        private string conStrMain = @"Data Source={0}\SQLEXPRESS;DATABASE=HENTAI;Integrated Security=false;User ID=sa;PASSWORD=c06g4cl6;Connection Timeout=180";
        private string conStrLib = @"Data Source={0}\SQLEXPRESS;DATABASE=ANIMELIBRARY;Integrated Security=false;User ID=sa;PASSWORD=c06g4cl6;Connection Timeout=180";
        private string conStrPic = @"Data Source={0}\SQLEXPRESS;DATABASE=HENTAIPIC;Integrated Security=false;User ID=sa;PASSWORD=c06g4cl6;Connection Timeout=180";
        private string conStrBook = @"Data Source={0}\SQLEXPRESS;DATABASE=MELONBOOK;Integrated Security=false;User ID=sa;PASSWORD=c06g4cl6;Connection Timeout=600";

        public CrawMelonBookController()
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

        private string MakeHASH(string val)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            string hash = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(val))).Replace("-", "");

            return hash;

        }

        #endregion Core

        #region DA
        public DataTable GetStatusData(string id)
        {
            string sql = @"
SELECT * FROM STATUS WHERE WEB = '{0}'";
            sql = string.Format(sql, id);

            DataTable data = this.QueryDBData(sql, conStrMain);

            return data;

        }

        public void UpdateStatusData(DataTable data)
        {
            string sql = "SELECT * FROM STATUS";

            this.UploadDBData(data, sql, conStrMain);

        }

        public DataTable GetItemData(string id)
        {
            string sql = "select * from DATA_MELONBOOK where MELON_ID = '{0}'";
            sql = string.Format(sql, id);

            DataTable data = this.QueryDBData(sql, conStrMain);

            return data;

        }

        public void UpdateItemData(DataTable data)
        {
            string sql = "SELECT * FROM DATA_MELONBOOK";

            this.UploadDBData(data, sql, conStrMain);

        }

        public DataTable GetItemDetailData(string id)
        {
            string sql = "select * from item where id = '{0}'";
            sql = string.Format(sql, id);

            DataTable data = this.QueryDBData(sql, conStrBook);

            return data;

        }

        public void UpdateItemDetailData(DataTable data)
        {
            string sql = "SELECT * FROM ITEM";

            this.UploadDBData(data, sql, conStrBook);

        }

        public DataTable GetHentaiData(string filter)
        {
            string sql = @"
SELECT * FROM DATA_EXHENTAI_DETAIL
 WHERE (NAME_UNI LIKE (N'%{0}%') OR NAME_ENG LIKE (N'%{0}%'))";

            sql = string.Format(sql, filter);

            DataTable data = this.QueryDBData(sql, conStrLib);

            return data;

        }

        public DataTable GetMelonData(string melonId)
        {
            string sql = @"
SELECT * FROM MAP_HENTAI
 WHERE MELON_ID = '{0}'";

            sql = string.Format(sql, melonId);

            DataTable data = this.QueryDBData(sql, conStrBook);

            return data;

        }

        public void UpdateMelonData(DataTable data)
        {
            string sql = "SELECT * FROM MAP_HENTAI";

            this.UploadDBData(data, sql, conStrBook);

        }

        public DataTable GetTagData(string tagGUID)
        {
            string sql = @"
SELECT * FROM TAG
 WHERE GUID = '{0}'";

            sql = string.Format(sql, tagGUID);

            DataTable data = this.QueryDBData(sql, conStrBook);

            return data;

        }

        public void UpdateTagData(DataTable data)
        {
            string sql = "SELECT * FROM TAG";

            this.UploadDBData(data, sql, conStrBook);

        }

        public DataTable GetItemTagData(string melonID)
        {
            string sql = @"
SELECT * FROM ITEM_TAG
 WHERE ITEM_ID = '{0}'";

            sql = string.Format(sql, melonID);

            DataTable data = this.QueryDBData(sql, conStrBook);

            return data;

        }

        public void UpdateItemTagData(DataTable data)
        {
            string sql = "SELECT * FROM ITEM_TAG";

            this.UploadDBData(data, sql, conStrBook);

        }

        public int ExexuteMelonData(string melonId, string part)
        {
            int result = 0;

            using (SqlConnection conn = new SqlConnection(conStrBook))
            {
                conn.Open();

                List<SqlParameter> paramList = new List<SqlParameter>();

                SqlParameter param = null;

                param = new SqlParameter();
                param.ParameterName = "@MELON_ID";
                param.DbType = DbType.String;
                param.Direction = ParameterDirection.Input;
                param.Value = melonId;
                paramList.Add(param);

                param = new SqlParameter();
                param.ParameterName = "@EX_PART";
                param.DbType = DbType.Int32;
                param.Direction = ParameterDirection.Input;
                param.Value = part;
                paramList.Add(param);

                param = new SqlParameter();
                param.ParameterName = "@OUTPUT";
                param.DbType = DbType.Int32;
                param.Direction = ParameterDirection.Output;
                paramList.Add(param);

                SqlCommand com = conn.CreateCommand();
                com.CommandTimeout = 0;
                com.Parameters.AddRange(paramList.ToArray());
                com.CommandType = CommandType.StoredProcedure;
                com.CommandText = "HSP_MAP_ITEM_DATA";

                com.ExecuteNonQuery();

                result = Convert.ToInt32(com.Parameters["@OUTPUT"].Value);

            }

            return result;

        }

        #endregion DA

        // GET: Craw
        public ActionResult Index()
        {
            return View();

        }

        public ActionResult GET_ITEM_DATA(string param)
        {
            string id = "";
            string key = "";
            string subId = "";
            DataTable dataStatus = null;
            Json result = new Json();

            try
            {
                if (param == null)
                {
                    dataStatus = this.GetStatusData("MELONBOOK");
                    id = dataStatus.Rows[0]["PROC_PAGE"].ToString();

                }
                else
                {
                    String[] paramList = param.Split('@');
                    if (paramList.Length == 1)
                    {
                        id = paramList[0];

                    }
                    else if (paramList.Length == 2)
                    {
                        string param1 = paramList[0];
                        string param2 = paramList[1];

                        if (param2 == "-1")
                        {
                            id = param1;

                        }
                        else
                        {
                            key = param1;

                            dataStatus = this.GetStatusData(string.Format("MELONBOOK_{0}", key));
                            subId = dataStatus.Rows[0]["PROC_PAGE"].ToString();

                            id = (int.Parse(key) * 10000 + int.Parse(subId)).ToString();

                            //id = paramList[1];

                        }

                    }

                }

                string url = "https://www.melonbooks.co.jp/detail/detail.php?product_id={0}&adult_view=1";
                url = string.Format(url, id);

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

                #region 取得 WEB 資料
                try
                {
                    HttpWebRequest req = HttpWebRequest.Create(url) as HttpWebRequest;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    using (HttpWebResponse res = req.GetResponse() as HttpWebResponse)
                    {
                        using (StreamReader reader = new StreamReader(res.GetResponseStream()))
                        {
                            string html = reader.ReadToEnd();

                            doc.LoadHtml(html);

                        }

                    }

                    if (doc.DocumentNode.ChildNodes.Count == 0)
                    {
                        throw new Exception("HTML載入錯誤");

                    }

                }
                catch (Exception ex)
                {
                    throw new Exception("HTML載入錯誤" + Environment.NewLine + ex.Message);

                }

                #endregion 取得 WEB 資料

                HtmlNode table = doc.GetElementbyId("description");
                HtmlNode tag = doc.GetElementbyId("related_tags");
                HtmlNode n = null;

                string
                        Cat1 = "",
                        Cat2 = "",
                        title = "",
                        circle = "",
                        author = "",
                        releaseDate = "",
                        page = "",
                        type = "",
                        eventName = "",
                        vendor = "",
                        image = "";

                if (table != null)
                {
                    string path = table.XPath;

                    n = doc.DocumentNode.SelectSingleNode(
                        "/html/body/div[2]/div/div/div[1]/div[1]/div/div[1]/div/ol"
                        //"/html/body/div[3]/div/div/div[1]/div[1]/div/div[1]/div/ol"
                        );
                    Cat1 = n.ChildNodes[3].ChildNodes[1].ChildNodes[1].ChildNodes[0].InnerText;
                    Cat2 = n.ChildNodes[5].ChildNodes[1].ChildNodes[1].ChildNodes[0].InnerText;

                    n = doc.DocumentNode.SelectSingleNode(path + "/table[1]");
                    List<HtmlNode> m = n.ChildNodes.ToList().FindAll(x => x.Name == "tr");
                    Console.WriteLine(m.Count);

                    foreach (HtmlNode x in m)
                    {
                        title =
                            x.ChildNodes["th"].InnerHtml == "タイトル" ? x.ChildNodes["td"].InnerHtml : title;

                        circle =
                            x.ChildNodes["th"].InnerHtml == "サークル名" ? x.ChildNodes["td"].ChildNodes["a"].InnerHtml : circle;

                        author =
                            x.ChildNodes["th"].InnerHtml == "作家名" ? x.ChildNodes["td"].ChildNodes["a"].InnerHtml : author;

                        vendor =
                            x.ChildNodes["th"].InnerHtml == "出版社" ? x.ChildNodes["td"].ChildNodes["a"].InnerHtml : vendor;

                        releaseDate =
                            x.ChildNodes["th"].InnerHtml == "発行日" ? x.ChildNodes["td"].InnerHtml : releaseDate;

                        page =
                            x.ChildNodes["th"].InnerHtml == "総ページ数・CG数・曲数" ? x.ChildNodes["td"].InnerHtml : page;

                        eventName =
                            x.ChildNodes["th"].InnerHtml == "イベント" ? x.ChildNodes["td"].ChildNodes["a"].InnerHtml : eventName;

                        type =
                            x.ChildNodes["th"].InnerHtml == "作品種別" ? x.ChildNodes["td"].InnerHtml : type;

                    }

                    string[] pathList = new string[] {
                        "/html/body/div[2]/div/div/div[1]/div[1]/div/div[4]/div[1]/a/img",
                        "/html/body/div[2]/div/div/div[1]/div[1]/div/div[5]/div[1]/a/img",
                        "/html/body/div[2]/div/div/div[1]/div[1]/div/div[3]/div[2]/div[1]/div[1]/div[1]/a/img",
                        "/html/body/div[2]/div/div/div[1]/div[1]/div/div[6]/div[1]/a/img"
                    };

                    foreach (string xpath in pathList)
                    {
                        n = doc.DocumentNode.SelectSingleNode(xpath);
                        if (n != null)
                        {
                            image = n.GetAttributeValue("src", "");

                            break;

                        }

                    }

                }
                else
                {
                    ViewData["DATA"] = "找不到資料";

                }

                List<string> tagList = new List<string>();
                if (tag != null)
                {
                    List<HtmlNode> x = tag.ChildNodes["ul"].ChildNodes.ToList().FindAll(c => c.Name == "li");

                    foreach (HtmlNode node in x)
                    {
                        tagList.Add(node.InnerText);

                    }
                    
                }

                DataTable dataMain = this.GetItemData(id);
                DataTable dataDetail = this.GetItemDetailData(id);
                DataTable dataTag = this.GetTagData("");
                DataTable dataItemTag = this.GetItemTagData(id);
                DataRow rowMain = null;
                DataRow rowDetail = null;

                if (dataMain.Rows.Count > 0)
                {
                    rowMain = dataMain.Rows[0];
                    rowMain["STATUS"] =
                        table != null && rowMain["STATUS"].ToString() != "0" ?
                        "1" :
                        "0";
                    result.Add("TYPE", "Edit");

                }
                else
                {
                    rowMain = dataMain.NewRow();
                    rowMain["MELON_ID"] = id;
                    rowMain["STATUS"] = table != null ? "1" : "0";

                    rowMain["CREATE_TIME"] = DateTime.Now;

                    result.Add("TYPE", "Add");

                }

                if (dataDetail.Rows.Count > 0)
                {
                    rowDetail = dataDetail.Rows[0];
                    rowDetail["STATUS"] =
                        table != null && rowDetail["STATUS"].ToString() != "0" ?
                        "1" :
                        "0";

                }
                else
                {
                    rowDetail = dataDetail.NewRow();
                    rowDetail["ID"] = id;
                    rowDetail["STATUS"] = table != null ? "1" : "0";

                }

                rowDetail["TITLE"] = title != "" ? HttpUtility.HtmlDecode(title) : rowDetail["TITLE"];
                result.Add("ID", id);
                result.Add("TITLE", title);
                rowDetail["CIRCLE"] = circle != "" ? HttpUtility.HtmlDecode(circle).Split(' ')[0] : rowDetail["CIRCLE"];
                rowDetail["AUTHOR"] = author != "" ? HttpUtility.HtmlDecode(author) : rowDetail["AUTHOR"];
                rowDetail["VENDOR"] = vendor != "" ? HttpUtility.HtmlDecode(vendor) : rowDetail["VENDOR"];

                if (releaseDate != "")
                {
                    string[] dateStr = releaseDate.Split(' ')[0].Split('/');
                    rowDetail["DATE"] = new DateTime(
                        int.Parse(dateStr[0]), int.Parse(dateStr[1]), int.Parse(dateStr[2]));

                }

                rowDetail["CAT_1"] = Cat1 != "" ? HttpUtility.HtmlDecode(Cat1) : rowDetail["CAT_1"];
                rowDetail["CAT_2"] = Cat1 != "" ? HttpUtility.HtmlDecode(Cat2) : rowDetail["CAT_2"];
                rowDetail["PAGE"] = page != "" ? HttpUtility.HtmlDecode(page) : rowDetail["PAGE"];
                rowDetail["TYPE"] = type != "" ? HttpUtility.HtmlDecode(type) : rowDetail["TYPE"];
                rowDetail["IMAGE"] = image != "" ? string.Format("https:{0}", image) : rowDetail["IMAGE"];

                rowMain["UPDATE_TIME"] = DateTime.Now;

                if (dataMain.Rows.Count == 0)
                {
                    dataMain.Rows.Add(rowMain);

                }

                if (dataDetail.Rows.Count == 0)
                {
                    dataDetail.Rows.Add(rowDetail);

                }

                foreach (string tagWord in tagList)
                {
                    string guid = this.MakeHASH(tagWord);

                    DataTable sub = this.GetTagData(guid);

                    if (sub.Rows.Count == 0)
                    {
                        DataRow tagRow = dataTag.NewRow();
                        tagRow["GUID"] = guid;
                        tagRow["TAG_NAME"] = tagWord;
                        tagRow["TAG_DISPLAY"] = tagWord;
                        tagRow["TAG_STATUS"] = "Y";
                        tagRow["UPDATE_USER"] = "CRAW";
                        tagRow["UPDATE_TIME"] = DateTime.Now;

                        dataTag.Rows.Add(tagRow);

                    }

                    DataRow[] tagRows = dataItemTag.Select(string.Format("ITEM_ID = '{0}' and TAG_GUID = '{1}'", id , guid));

                    if (tagRows.Length == 0)
                    {
                        DataRow tagItemRow = dataItemTag.NewRow();

                        tagItemRow["ITEM_ID"] = id;
                        tagItemRow["TAG_GUID"] = guid;
                        tagItemRow["TAG_STATUS"] = "Y";
                        tagItemRow["UPDATE_USER"] = "CRAW";
                        tagItemRow["UPDATE_TIME"] = DateTime.Now;

                        dataItemTag.Rows.Add(tagItemRow);

                    }

                }

                this.UpdateItemTagData(dataItemTag);
                this.UpdateTagData(dataTag);
                this.UpdateItemDetailData(dataDetail);
                this.UpdateItemData(dataMain);

            }
            catch (Exception ex)
            {
                DataTable data = this.GetItemData(id);
                DataRow row = null;
                if (data.Rows.Count > 0)
                {
                    row = data.Rows[0];
                    row["STATUS"] = "9";
                    row["UPDATE_TIME"] = DateTime.Now;

                }
                else
                {
                    row = data.NewRow();
                    row["MELON_ID"] = id;
                    row["STATUS"] = "9";
                    row["CREATE_TIME"] = DateTime.Now;
                    row["UPDATE_TIME"] = DateTime.Now;

                    data.Rows.Add(row);

                }

                this.UpdateItemData(data);

                //result.Add("ID", id);
                result.Add("ERROR", ex.Message);

            }

            if (dataStatus != null)
            {
                dataStatus.Rows[0]["PROC_PAGE"] = int.Parse(id) + 1;
                dataStatus.Rows[0]["UPDATE_TIME"] = DateTime.Now;

                this.UpdateStatusData(dataStatus);

            }

            ViewData["DATA"] = result.ToString();

            return View();

        }

        public ActionResult REFRESH_ITEM_DATA(string param)
        {
            return View();

        }

        public ActionResult MAP_ITEM_DATA(string param)
        {
            string id = "";
            string part = "";
            Json jsonObject = new Json();
            DataTable dataStatus = null;

            try
            {
                if (param == null)
                {
                    dataStatus = this.GetStatusData("MAP_MELONBOOK");
                    id = dataStatus.Rows[0]["PROC_PAGE"].ToString();
                    part = "-1";

                }
                else
                {
                    id = param.Split('@')[0];
                    part = param.Split('@')[1];

                }

                jsonObject.Add("ID", id);

                DataTable data = this.GetItemDetailData(id);

                if (data.Rows.Count == 0)
                {
                    throw new Exception("找不到ID資料");

                }

                string title = data.Rows[0]["TITLE"].ToString();
                if (title == "")
                {
                    throw new Exception(string.Format("{0} 沒有資料", id));

                }


                jsonObject.Add("TITLE", title);
                jsonObject.Add("PART", part);
                jsonObject.Add("MAP", 0);

                jsonObject.Edit("MAP", this.ExexuteMelonData(id, part));

                if (dataStatus != null)
                {
                    dataStatus.Rows[0]["PROC_PAGE"] = int.Parse(id) + 1;
                    dataStatus.Rows[0]["UPDATE_TIME"] = DateTime.Now;

                    this.UpdateStatusData(dataStatus);

                }

            }
            catch (Exception ex)
            {
                jsonObject.Add("ERROR", ex.Message);

            }

            ViewData["DATA"] = jsonObject.ToString();

            return View();

        }

    }

}
