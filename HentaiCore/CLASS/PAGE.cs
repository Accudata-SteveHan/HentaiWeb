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

namespace HentaiGetchu2 {
    //public class PAGE {
    //    bool firstPage = false;
    //    DataTable pageData = null;
    //    int bookCount = 0;
    //    string baseUrl = "";
    //    List<BOOK_INFO> bookList = new List<BOOK_INFO>();

    //    public int totalPageNum = -1;

    //    public PAGE() {
    //        this.InitPageData();

    //    }

    //    public PAGE(string baseUrl) {
    //        this.InitPageData();

    //        this.baseUrl = baseUrl;
    //    }

    //    public PAGE(string baseUrl, bool first) {
    //        this.InitPageData();

    //        this.baseUrl = baseUrl;
    //        this.firstPage = first;

    //    }

    //    //private void InitPageData() {
    //    //    DataTable table = new DataTable();

    //    //    table.Columns.Add("TYPE");
    //    //    table.Columns.Add("UPLOAD_TIME");
    //    //    table.Columns.Add("POSTED_TIME");
    //    //    table.Columns.Add("PIC");
    //    //    table.Columns.Add("TITLE");
    //    //    table.Columns.Add("TOTAL_PAGE");
    //    //    table.Columns.Add("P_KEY");
    //    //    table.Columns.Add("S_KEY");
    //    //    table.Columns.Add("LANGUAGE");
    //    //    table.Columns.Add("RATE_TIME");
    //    //    table.Columns.Add("RATE_AVG");

    //    //    this.pageData = table;

    //    //}

    //    //public DataTable ProcessPage(string url) {
    //    //    HTML web = new HTML();
    //    //    string html = "";

    //    //    web.webUrl = url;
    //    //    web.webCookie = FormMain.authUser.loginCookie;
    //    //    web.cookieContainer = FormMain.authUser.loginContainer;
    //    //    web.GetHtml();
    //    //    html = web.webHtml;

    //    //    if (url.Contains("exhentai")) {
    //    //        web = new HTML();
    //    //        web.webUrl = url;
    //    //        web.webCookie = FormMain.authUser.loginCookie;
    //    //        //web.cookieContainer = Form1.authUser.loginContainer;
    //    //        web.GetHtml();
    //    //        html = web.webHtml;
    //    //    }

    //    //    HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
    //    //    document.LoadHtml(html);

    //    //    HtmlNode rootNode = document.DocumentNode;
    //    //    string tableXPath = "";

    //    //    if (this.firstPage) {
    //    //        tableXPath = "/html[1]/body[1]/div[1]/div[2]/table[1]/tr[1]";
    //    //        HtmlNode pageNode = rootNode.SelectSingleNode(tableXPath);
    //    //        string pageNum = pageNode.ChildNodes[pageNode.ChildNodes.Count - 2].InnerText;
    //    //        if (!int.TryParse(pageNum, out this.totalPageNum)) {
    //    //            this.totalPageNum = -1;
    //    //        }

    //    //    }

    //    //    tableXPath = "/html[1]/body[1]/div[1]/div[2]/table[2]";
    //    //    foreach (HtmlNode node in rootNode.SelectNodes("//tr")) {

    //    //        if (!node.XPath.StartsWith(tableXPath)) {
    //    //            continue;
    //    //        }

    //    //        this.ProcessData(node);

    //    //    }

    //    //    foreach (DataRow row in this.pageData.Rows) {
    //    //        GetDetail(row, "");
    //    //        GetDetail(row, "cover");
    //    //    }

    //    //    return pageData;

    //    //}

    //    //private void ProcessData(HtmlNode node) {
    //    //    #region NEW_ROW
    //    //    DataRow newRow = this.pageData.NewRow();
    //    //    string error = "";

    //    //    error += this.GetBasic(newRow, node, "TYPE");
    //    //    error += this.GetBasic(newRow, node, "UPLOAD_TIME");
    //    //    error += this.GetBasic(newRow, node, "PIC");
    //    //    error += this.GetBasic(newRow, node, "TITLE");
    //    //    error += this.GetBasic(newRow, node, "P_KEY");
    //    //    error += this.GetBasic(newRow, node, "S_KEY");

    //    //    #endregion NEW_ROW

    //    //    if (error.IndexOf("0") < 0) {
    //    //        this.pageData.Rows.Add(newRow);
    //    //    }

    //    //}

    //    //private string GetBasic(DataRow row, HtmlNode node, string key) {
    //    //    try {
    //    //        int status = 0;

    //    //        //root
    //    //        HtmlNode tmpNode = null;

    //    //        switch (key.ToLower()) {
    //    //            //類別
    //    //            // = root + /td[1]/a[1]/img[1]
    //    //            case "type": {
    //    //                tmpNode = node.SelectSingleNode(node.XPath + "/td[1]/a[1]/img[1]");
    //    //                if (tmpNode != null) {
    //    //                    row[key] = tmpNode.Attributes["alt"].Value;
    //    //                    status = 1;
    //    //                }

    //    //            }
    //    //            break;

    //    //            //上傳時間
    //    //            // = root + td[2]
    //    //            case "upload_time": {
    //    //                tmpNode = node.SelectSingleNode(node.XPath + "/td[2]");
    //    //                if (tmpNode != null) {
    //    //                    row[key] = tmpNode.InnerHtml;
    //    //                    status = 1;
    //    //                }

    //    //            }
    //    //            break;

    //    //            case "pic": {
    //    //                status = 1;
    //    //            }
    //    //            break;
    //    //            //書名
    //    //            // = root + /td[3]/div[1]/div[3]/a[1]
    //    //            case "title": {
    //    //                tmpNode = node.SelectSingleNode(node.XPath + "/td[3]/div[1]/div[3]/a[1]");
    //    //                if (tmpNode != null) {
    //    //                    row[key] = tmpNode.InnerHtml;
    //    //                    status = 1;
    //    //                }

    //    //            }
    //    //            break;
    //    //            //主KEY
    //    //            // = root + /td[3]/div[1]/div[3]/a[1]
    //    //            case "p_key": {
    //    //                tmpNode = node.SelectSingleNode(node.XPath + "/td[3]/div[1]/div[3]/a[1]");
    //    //                if (tmpNode != null) {
    //    //                    string keyUrl = tmpNode.Attributes["href"].Value.Replace(this.baseUrl + "/g/", "");
    //    //                    row[key] = keyUrl.Split('/')[0];
    //    //                    status = 1;
    //    //                }

    //    //            }
    //    //            break;
    //    //            //副KEY
    //    //            // = root + /td[3]/div[1]/div[3]/a[1]
    //    //            case "s_key": {
    //    //                tmpNode = node.SelectSingleNode(node.XPath + "/td[3]/div[1]/div[3]/a[1]");
    //    //                if (tmpNode != null) {
    //    //                    string keyUrl = tmpNode.Attributes["href"].Value.Replace(this.baseUrl + "/g/", "");
    //    //                    row[key] = keyUrl.Split('/')[1];
    //    //                    status = 1;
    //    //                }

    //    //            }
    //    //            break;

    //    //            default:
    //    //            status = 9;
    //    //            break;
    //    //        }

    //    //        return status.ToString();

    //    //    } catch (Exception ex) {
    //    //        throw ex;
    //    //    }

    //    //}

    //    //private void GetDetail(DataRow row, string key) {
    //    //    if (key.ToLower() == "cover") {
    //    //        if (row["P_KEY"] != DBNull.Value && row["S_KEY"] != DBNull.Value) {
    //    //            PIC pic = new PIC(baseUrl, row, "cover");
    //    //            pic.GetPic();

    //    //        }

    //    //    } else {
    //    //        string html = "";
    //    //        HTML web = null;
    //    //        Cookie alwayCookie = new Cookie("nw", "1", "/", "e-hentai.org");

    //    //        web = new HTML(string.Format("{0}/g/{1}/{2}", this.baseUrl, row["P_KEY"], row["S_KEY"]));
    //    //        web.webCookie = FormMain.authUser.loginCookie;
    //    //        web.cookieContainer = FormMain.authUser.loginContainer;
    //    //        web.cookieContainer.Add(alwayCookie);
    //    //        web.GetHtml();
    //    //        html = web.webHtml;

    //    //        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
    //    //        doc.LoadHtml(html);

    //    //        HtmlNode rootNode = doc.DocumentNode;
    //    //        HtmlNode node = null;
    //    //        string xpath = "";

    //    //        xpath = "/html[1]/body[1]/div[1]/div[4]/div[1]/div[3]/table[1]/tr[1]/td[2]";
    //    //        node = rootNode.SelectSingleNode(xpath);
    //    //        if (node != null) {
    //    //            row["POSTED_TIME"] = node.InnerHtml;
    //    //        }

    //    //        xpath = "/html[1]/body[1]/div[1]/div[4]/div[1]/div[3]/table[1]/tr[4]/td[2]";
    //    //        node = rootNode.SelectSingleNode(xpath);
    //    //        if (node != null) {
    //    //            row["LANGUAGE"] = node.InnerHtml
    //    //                .Replace(" &nbsp;", "")
    //    //                .Replace("<span class=\"halp\" title=\"This gallery has been translated from the original language text.\">TR</span>", "");
    //    //        }

    //    //        xpath = "/html[1]/body[1]/div[1]/div[4]/div[1]/div[3]/table[1]/tr[6]/td[2]";
    //    //        node = rootNode.SelectSingleNode(xpath);
    //    //        if (node != null) {
    //    //            row["TOTAL_PAGE"] = node.InnerHtml.Replace(" pages", "");
    //    //        }

    //    //        xpath = "/html[1]/body[1]/div[1]/div[4]/div[1]/div[4]/table[1]/tr[1]/td[3]/span[1]";
    //    //        node = rootNode.SelectSingleNode(xpath);
    //    //        if (node != null) {
    //    //            row["RATE_TIME"] = node.InnerHtml;
    //    //        }

    //    //        xpath = "/html[1]/body[1]/div[1]/div[4]/div[1]/div[4]/table[1]/tr[2]/td[1]";
    //    //        node = rootNode.SelectSingleNode(xpath);
    //    //        if (node != null) {
    //    //            row["RATE_AVG"] = node.InnerHtml.Replace("Average: ", "");
    //    //        }

    //    //    }


    //    //}

    //    //private void GetDetail(BOOK_INFO book, string key) {
    //    //    if (key.ToLower() == "cover") {
    //    //        if (book.p_key != "" && book.s_key != "") {
    //    //            PIC pic = new PIC(baseUrl, book, "cover");

    //    //            System.Threading.Thread t = new System.Threading.Thread(
    //    //            pic.GetPic);
    //    //            t.Start();

    //    //        }

    //    //    } else {
                

    //    //    }


    //    //}

    //}

}
