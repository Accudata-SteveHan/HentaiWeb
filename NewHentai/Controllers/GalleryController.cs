using HentaiCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;

namespace NewHentai.Controllers
{
    public partial class GalleryController : Controller
    {
        private DataControlMdf control = new DataControlMdf();
        private string conStrMain = @"Data Source={0}\SQLEXPRESS;DATABASE=HENTAI;Integrated Security=false;User ID=sa;PASSWORD=c06g4cl6;Connection Timeout=180";
        private string conStrLib = @"Data Source={0}\SQLEXPRESS;DATABASE=ANIMELIBRARY;Integrated Security=false;User ID=sa;PASSWORD=c06g4cl6;Connection Timeout=180";
        private string conStrPic = @"Data Source={0}\SQLEXPRESS;DATABASE=HENTAIPIC;Integrated Security=false;User ID=sa;PASSWORD=c06g4cl6;Connection Timeout=180";
        private string conStrBook = @"Data Source={0}\SQLEXPRESS;DATABASE=MELONBOOK;Integrated Security=false;User ID=sa;PASSWORD=c06g4cl6;Connection Timeout=600";

        private int maxDisplay = 20;

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

        #endregion Core

        #region DA
        private DataTable GetGalleryWeb()
        {
            string sql = @"
select *
  from WEB
 order by WEB, PAGE";

            DataTable data = this.QueryDBData(sql, conStrMain);

            return data;

        }

        private DataTable GetGalleryObject()
        {
            string sql = @"
select O.*,
    Convert(int, SUBSTRING(
		id , 
		CHARINDEX('_' , ID) + 1 , 
		LEN(ID) - (LEN(ID) - CHARINDEX('_' , ID , CHARINDEX('_' , ID) + 1) + 1) - (CHARINDEX('_' , ID))
	  )) PKEY
  from OBJECT O
 order by O.WEB_ID, PKEY";

            DataTable data = this.QueryDBData(sql, conStrMain);

            return data;

        }

        private DataTable GetGalleryIndex()
        {
            string sql = @"
select *
  from DATA_EXHENTAI
 where RELEASE_TIME IS NOT NULL
 order by RELEASE_TIME desc";

            DataTable data = this.QueryDBData(sql, conStrMain);

            return data;

        }

        private DataTable GetGallerySingleIndex()
        {
            string sql = @"
select OBJ_ID, PKEY, SKEY, TYPE, RELEASE_TIME
  from DATA_EXHENTAI
 where RELEASE_TIME IS NOT NULL
 order by RELEASE_TIME desc";

            DataTable data = this.QueryDBData(sql, conStrMain);

            return data;

        }

        private DataTable GetGalleryData()
        {
            string sql = @"
SELECT *
  FROM DATA_EXHENTAI_DETAIL
 WHERE 1 = 1
 ORDER BY CONVERT(INT, PKEY) DESC";

            DataTable data = this.QueryDBData(sql, conStrLib);

            return data;

        }

        private DataTable GetGalleryPageData(int page)
        {
            int dataCount = (page + 1) * maxDisplay;
            string sql = @"
SELECT TOP {0} *
  FROM DATA_EXHENTAI_DETAIL
 WHERE 1 = 1
 ORDER BY CONVERT(INT, PKEY) DESC";
            sql = string.Format(sql, dataCount);

            DataTable data = this.QueryDBData(sql, conStrLib);

            return data;

        }

        private DataTable GetGalleryDetail(string type, string lang, string page)
        {
            string sql = @"
SELECT {1} D.*
  FROM HENTAI..DATA_EXHENTAI M
 INNER JOIN ANIMELIBRARY..DATA_EXHENTAI_DETAIL D ON M.PKEY = D.PKEY
 WHERE {0}
 ORDER BY CONVERT(INT, M.PKEY) DESC";

            string filter = "";
            string top = "";
            if (type != "ALL")
            {
                filter += (filter != "" ? " AND " : "") + string.Format("M.TYPE = '{0}'", type);

            }
            if (lang != "ALL")
            {
                filter += (filter != "" ? " AND " : "") + string.Format("M.LANG = '{0}'", lang);

            }
            if (page != "ALL")
            {
                top = string.Format("TOP {0}", maxDisplay * (int.Parse(page) + 1));

            }
            if (filter == "")
            {
                filter = "1 = 1";

            }

            sql = string.Format(sql, filter, top);
            
            DataTable data = this.QueryDBData(sql, conStrMain);

            return data;

        }

        private DataTable GetGalleryBookData(string pKey, string sKey)
        {
            string sql = "";

            sql = "SELECT * FROM DATA_EXHENTAI_DETAIL WHERE 1=1 and PKEY = '{0}' and SKEY = '{1}' ";
            sql = string.Format(sql, pKey, sKey);
            DataTable dataBook = this.QueryDBData(sql, conStrLib);

            return dataBook;

        }

        private DataTable GetGalleryBookPic(string pKey, string sKey)
        {
            string sql = "";

            sql = "SELECT * FROM DATA_EXHENTAI_PIC WHERE 1=1 and PKEY = '{0}' and SKEY = '{1}' ";
            sql = string.Format(sql, pKey, sKey);
            DataTable dataPic = this.QueryDBData(sql, conStrPic);

            return dataPic;

        }

        #endregion DA

        // GET: Gallery
        public ActionResult Index(string param)
        {
            return View();

        }

    }

}