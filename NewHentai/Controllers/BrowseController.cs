using HentaiCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewHentai.Controllers
{
    public partial class BrowseController : Controller
    {
        private DataControlMdf control = new DataControlMdf();
        private string conStrMain = @"Data Source={0}\SQLEXPRESS;DATABASE=HENTAI;Integrated Security=false;User ID=sa;PASSWORD=c06g4cl6;Connection Timeout=90";
        private string conStrLib = @"Data Source={0}\SQLEXPRESS;DATABASE=ANIMELIBRARY;Integrated Security=false;User ID=sa;PASSWORD=c06g4cl6;Connection Timeout=90";
        private string conStrPic = @"Data Source={0}\SQLEXPRESS;DATABASE=HENTAIPIC;Integrated Security=false;User ID=sa;PASSWORD=c06g4cl6;Connection Timeout=90";
        private string conStrBook = @"Data Source={0}\SQLEXPRESS;DATABASE=MELONBOOK;Integrated Security=false;User ID=sa;PASSWORD=c06g4cl6;Connection Timeout=90";

        private int maxDisplay = System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("MaxItemDisplay") ?
                                    int.Parse(System.Configuration.ConfigurationManager.AppSettings["MaxItemDisplay"]) :
                                    30;
        private int maxDisplayExhentai = System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("ExhentaiMaxItemDisplay") ?
                                    int.Parse(System.Configuration.ConfigurationManager.AppSettings["ExhentaiMaxItemDisplay"]) :
                                    30;
        private int maxDisplayMelonBook = System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("MelonBookMaxItemDisplay") ?
                                    int.Parse(System.Configuration.ConfigurationManager.AppSettings["MelonBookMaxItemDisplay"]) :
                                    30;

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

        #endregion Core

        #region DA
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

        private DataTable GetGallerySingleIndex(string type, string lang)
        {
            string sql = @"
select OBJ_ID, PKEY, SKEY, TYPE, LANG, RELEASE_TIME
  from DATA_EXHENTAI
 where RELEASE_TIME IS NOT NULL {0}
 order by RELEASE_TIME desc";

            string filter = "";

            if (type != "ALL")
            {
                filter += string.Format("   AND TYPE = '{0}'", type);

            }

            if (lang != "ALL")
            {
                filter += string.Format("   AND LANG = '{0}'", lang);

            }

            sql = string.Format(sql, filter);

            DataTable data = this.QueryDBData(sql, conStrMain);

            return data;

        }

        private DataTable GetGalleryData(string type, string lang)
        {
            string sql = @"
SELECT *
  FROM DATA_EXHENTAI_DETAIL
 WHERE 1 = 1 {0}
 ORDER BY CONVERT(INT, PKEY) DESC";

            string filter = "";

            if (type != "ALL")
            {
                filter += string.Format("   AND TYPE = '{0}'", type);

            }

            if (lang != "ALL")
            {
                filter += string.Format("   AND LANG = '{0}'", lang);

            }

            sql = string.Format(sql, filter);

            DataTable data = this.QueryDBData(sql, conStrLib);

            return data;

        }

        private DataTable GetGalleryPageData(int page)
        {
            int dataCount = (page + 1) * maxDisplayExhentai;
            string sql = @"
SELECT TOP {0} *
  FROM DATA_EXHENTAI_DETAIL
 WHERE 1 = 1
 ORDER BY CONVERT(INT, PKEY) DESC";
            sql = string.Format(sql, dataCount);

            DataTable data = this.QueryDBData(sql, conStrLib);

            return data;

        }

        private DataTable GetGalleryDetail(string type, string lang, string page, string param)
        {
            string sql = @"
SELECT {1} D.*
  FROM HENTAI..DATA_EXHENTAI M
 INNER JOIN ANIMELIBRARY..DATA_EXHENTAI_DETAIL D ON M.PKEY = D.PKEY
 WHERE {0}
 ORDER BY CONVERT(INT, M.PKEY) {2}";

            string filter = "";
            string top = "";
            string sort = "DESC";

            if (type != "ALL")
            {
                string typeStr = type;

                if (typeStr.StartsWith("S"))
                {
                    typeStr = type.Substring(1);

                    filter += (filter != "" ? " AND " : "") + string.Format("D.PAGE < 40");

                }

                if (typeStr.StartsWith("M"))
                {
                    typeStr = type.Substring(1);

                    filter += (filter != "" ? " AND " : "") + string.Format("D.PAGE >= 40");
                    filter += (filter != "" ? " AND " : "") + string.Format("D.PAGE < 100");

                }

                if (typeStr.StartsWith("L"))
                {
                    typeStr = type.Substring(1);

                    filter += (filter != "" ? " AND " : "") + string.Format("D.PAGE >= 100");
                    filter += (filter != "" ? " AND " : "") + string.Format("D.PAGE < 400");

                }

                if (typeStr.StartsWith("X"))
                {
                    typeStr = type.Substring(1);

                    filter += (filter != "" ? " AND " : "") + string.Format("D.PAGE >= 400");

                }

                filter += (filter != "" ? " AND " : "") + string.Format("M.TYPE = '{0}'", typeStr);

            }

            if (lang != "ALL")
            {
                filter += (filter != "" ? " AND " : "") + string.Format("M.LANG = '{0}'", lang);

            }

            if (param == "")
            {
                if (page != "ALL")
                {
                    top = string.Format("TOP {0}", maxDisplayExhentai * (int.Parse(page) + 1));

                }

            }
            else
            {
                if (int.Parse(param) >= 0)
                {
                    filter += (filter != "" ? " AND " : "") +
                        string.Format("CAST(M.PKEY as INT) < '{0}'", param);

                    sort = "DESC";

                }
                else
                {
                    filter += (filter != "" ? " AND " : "") +
                        string.Format("CAST(M.PKEY as INT) > '{0}'", param.ToString().Replace("-", ""));

                    sort = "ASC";

                }

                top = string.Format("TOP {0}", maxDisplayExhentai * 1);

            }

            if (filter == "")
            {
                filter = "1 = 1";

            }

            sql = string.Format(sql, filter, top, sort);

            DataTable data = this.QueryDBData(sql, conStrMain);

            return data;

        }

        private DataTable GetGalleryDisplay(string type, string lang, string page, string param)
        {
            string sql = @"
SELECT {1} 
D.*,
L.PAGE
  FROM ANIMELIBRARY..DATA_EXHENTAI_DISPLAY D
 INNER JOIN ANIMELIBRARY..DATA_EXHENTAI_DETAIL L ON L.PKEY = D.PKEY AND L.SKEY = D.SKEY
 WHERE {0}
 ORDER BY CONVERT(INT, D.PKEY) {2}";

            string filter = "";
            string top = "";
            string sort = "DESC";

            if (type != "ALL")
            {
                filter += (filter != "" ? " AND " : "") + string.Format("D.TYPE = '{0}'", type);

            }

            if (type != "ALL")
            {
                string typeStr = type;

                if (typeStr.StartsWith("S"))
                {
                    typeStr = type.Substring(1);

                    filter += (filter != "" ? " AND " : "") + string.Format("L.PAGE < 40");

                }

                if (typeStr.StartsWith("M"))
                {
                    typeStr = type.Substring(1);

                    filter += (filter != "" ? " AND " : "") + string.Format("L.PAGE >= 40");
                    filter += (filter != "" ? " AND " : "") + string.Format("L.PAGE < 100");

                }

                if (typeStr.StartsWith("L"))
                {
                    typeStr = type.Substring(1);

                    filter += (filter != "" ? " AND " : "") + string.Format("L.PAGE >= 100");
                    filter += (filter != "" ? " AND " : "") + string.Format("L.PAGE < 400");

                }

                if (typeStr.StartsWith("X"))
                {
                    typeStr = type.Substring(1);

                    filter += (filter != "" ? " AND " : "") + string.Format("L.PAGE >= 400");

                }

                filter += (filter != "" ? " AND " : "") + string.Format("D.TYPE = '{0}'", typeStr);

            }

            if (lang != "ALL")
            {
                filter += (filter != "" ? " AND " : "") + string.Format("D.LANG = '{0}'", lang);

            }

            if (param == "")
            {
                if (page != "ALL")
                {
                    top = string.Format("TOP {0}", maxDisplayExhentai * (int.Parse(page) + 1));

                }

            }
            else
            {
                if (int.Parse(param) >= 0)
                {
                    filter += (filter != "" ? " AND " : "") +
                        string.Format("CAST(D.PKEY as INT) < '{0}'", param);

                    sort = "DESC";

                }
                else
                {
                    filter += (filter != "" ? " AND " : "") +
                        string.Format("CAST(D.PKEY as INT) > '{0}'", param.ToString().Replace("-", ""));

                    sort = "ASC";

                }

                top = string.Format("TOP {0}", maxDisplayExhentai * 1);

            }
            if (filter == "")
            {
                filter = "1 = 1";

            }

            sql = string.Format(sql, filter, top, sort);

            DataTable data = this.QueryDBData(sql, conStrLib);

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

        private DataTable GetMelonData(string id)
        {
            string sql = "";

            sql = "SELECT * FROM ITEM WHERE 1=1 and ID = '{0}'";
            sql = string.Format(sql, id);
            DataTable data = this.QueryDBData(sql, conStrBook);

            return data;

        }

        private DataTable GetMelonTagData(string melonId, string type, string lang)
        {
            string sql = "";

            sql = @"
SELECT
--*
ITEM_TAG.ITEM_ID,
TAG.GUID,
TAG.TAG_NAME,
TAG.TAG_DISPLAY
FROM   MELONBOOK.DBO.ITEM_TAG ITEM_TAG
       INNER JOIN MELONBOOK.DBO.TAG TAG
               ON TAG.GUID = ITEM_TAG.TAG_GUID
WHERE  1 = 1
       AND TAG.TAG_SHOW = 'Y'
       AND TAG.TAG_CHECK = 'Y'
       AND ITEM_TAG.ITEM_ID = '{0}'";
            sql = string.Format(sql, melonId);
            DataTable data = this.QueryDBData(sql, conStrBook);

            return data;
            
        }

        private DataTable GetMelonTagBookMappingTopData(string melonId, string type, string lang)
        {
            int groupCount = 3;
            int maxDisplay = 10;

            string sql = "";

            string existFilter = "";

            if (type != "ALL")
            {
                existFilter += string.Format(" AND MAP.TYPE = '{0}'", type);

            }

            if (lang != "ALL")
            {
                existFilter += string.Format(" AND MAP.LANG = '{0}'", lang);

            }

            sql = @"
SELECT DISTINCT 
    *
  FROM MELONBOOK.DBO.ITEM
 INNER JOIN
(
    SELECT 
        ITEM_ID , COUNT(*) CNT
      FROM MELONBOOK.DBO.ITEM_TAG I
     INNER JOIN MELONBOOK.DBO.TAG T ON T.GUID = I.TAG_GUID
     WHERE 1=1
       AND I.ITEM_ID <> '{0}'  
	   AND I.ITEM_ID IN
	   (
		   SELECT MELON_ID
		     FROM MELONBOOK.DBO.MAP_HENTAI MAP 
		    WHERE 1=1
		      AND MAP.STATUS = 'Y' {1}

	   )
       AND EXISTS
        (
            SELECT
            *
              FROM MELONBOOK.DBO.ITEM_TAG ITEM_TAG
             INNER JOIN MELONBOOK.DBO.TAG TAG ON TAG.GUID = ITEM_TAG.TAG_GUID
             WHERE 1 = 1
               AND I.TAG_GUID = ITEM_TAG.TAG_GUID
               AND TAG.TAG_SHOW = 'Y'
               AND TAG.TAG_CHECK = 'Y'
               AND ITEM_TAG.ITEM_ID = '{0}' 
        )
    GROUP BY ITEM_ID
 )X ON X.ITEM_ID = ITEM.ID
 ORDER BY CNT DESC";
            
            sql = string.Format(sql, melonId, existFilter);

            DataTable dataMain = this.QueryDBData(sql, conStrBook);
            DataTable dataCnt = dataMain.DefaultView.ToTable(true, new string[] { "CNT" });

            DataTable data = new DataTable();
            int cnt = 0;

            foreach (DataRow rowCnt in dataCnt.Rows)
            {
                DataRow[] selRows = dataMain.Select(string.Format("CNT = '{0}'", rowCnt["CNT"]));

                if (data.Rows.Count == 0)
                {
                    data = dataMain.Clone();

                }

                if (selRows.Length <= groupCount)
                {
                    foreach (DataRow sub in selRows)
                    {
                        data.ImportRow(sub);

                    }

                }
                else
                {
                    int min = 0, max = selRows.Length - 1;
                    for (int i = 0; i < groupCount; i++)
                    {
                        int x = new Random(int.Parse(DateTime.Now.ToString("MMddmmss"))).Next(min, max);

                        DataRow sub = selRows[x];

                        if (data.Select(string.Format("ID = '{0}'", sub["ID"].ToString())).Length == 0)
                        {
                            data.ImportRow(sub);

                        }

                        if (Math.Abs(min - x) > Math.Abs(max - x))
                        {
                            max = x;

                        }
                        else
                        {
                            min = x;

                        }

                        if (min > max)
                        {
                            break;

                        }

                    }
                    
                }

                if (data.Rows.Count > maxDisplay)
                {
                    break;

                }

            }

            return data;

        }
        
        private DataTable GetMelonBookMapItemData(string map, string type, string lang, string page, string param)
        {
            string sql = @"
SELECT {2} *
  FROM MELONBOOK..ITEM ITEM
 WHERE 1=1
   AND EXISTS(
	SELECT * 
	  FROM MELONBOOK..MAP_HENTAI MAP
	 WHERE 1=1
	   AND MAP.MELON_ID = ITEM.ID
	   AND MAP.STATUS = 'Y' {0}
   )
   {1}
 ORDER BY ITEM.ID {3}";

            string existFilter = "";
            string whereFilter = "";
            string top = "";
            string sort = "DESC";

            if (type != "ALL")
            {
                existFilter += string.Format(" AND MAP.TYPE = '{0}'", type);

            }

            if (lang != "ALL")
            {
                existFilter += string.Format(" AND MAP.LANG = '{0}'", lang);

            }

            if (param == "")
            {
                if (page != "ALL")
                {
                    top = string.Format("TOP {0}", maxDisplayMelonBook * (int.Parse(page) + 1));

                }

            }
            else
            {
                if (int.Parse(param) >= 0)
                {
                    whereFilter += string.Format(" AND ITEM.ID < {0}", param);

                    sort = "DESC";

                }
                else
                {
                    whereFilter +=  string.Format(" AND ITEM.ID > {0}", param.ToString().Replace("-", ""));

                    sort = "ASC";

                }

                top = string.Format("TOP {0}", maxDisplayMelonBook * 1);

            }

            if (map != "")
            {
                if (map.ToUpper() == "MAP")
                {
                    whereFilter += "    AND ITEM.MAP_DONE = 'Y'";
                    
                }

            }
            
            sql = string.Format(sql, existFilter, whereFilter, top, sort);

            DataTable data = this.QueryDBData(sql, conStrBook);

            return data;

        }

        private DataTable GetMelonBookMapDetailData(string type, string lang, string page)
        {
            string sql = @"
SELECT MAP.MELON_ID , D.*
  FROM MELONBOOK..MAP_HENTAI MAP
 INNER JOIN MELONBOOK..ITEM MI ON MAP.MELON_ID = MI.ID
 INNER JOIN ANIMELIBRARY..DATA_EXHENTAI_DETAIL D ON MAP.HENTAI_ID = D.PKEY
   AND {0}
 WHERE MAP.STATUS = 'Y'
   AND exists(
	SELECT DISTINCT {1} MI.ID
	  FROM MELONBOOK..MAP_HENTAI MAP
	 INNER JOIN MELONBOOK..ITEM MI ON MAP.MELON_ID = MI.ID
	 INNER JOIN ANIMELIBRARY..DATA_EXHENTAI_DETAIL D ON MAP.HENTAI_ID = D.PKEY
	   AND {0}
	 WHERE MAP.STATUS = 'Y'
 )
 ORDER BY MI.ID DESC";

            string filter = "";
            string top = "";
            if (type != "ALL")
            {
                filter += (filter != "" ? " AND " : "") + string.Format("D.TYPE = '{0}'", type);

            }
            if (lang != "ALL")
            {
                filter += (filter != "" ? " AND " : "") + string.Format("D.LANG = '{0}'", lang);

            }
            if (page != "ALL")
            {
                top = string.Format("TOP {0}", maxDisplayMelonBook * (int.Parse(page) + 1));

            }
            if (filter == "")
            {
                filter = "1 = 1";

            }

            sql = string.Format(sql, filter, top);

            DataTable data = this.QueryDBData(sql, conStrMain);

            return data;

        }

        private DataTable GetMelonBookMapData(string type, string lang, string melonID)
        {
            string sql = @"
SELECT MAP.MELON_ID , D.*
  FROM MELONBOOK..MAP_HENTAI MAP
 INNER JOIN MELONBOOK..ITEM MI ON MAP.MELON_ID = MI.ID
 INNER JOIN ANIMELIBRARY..DATA_EXHENTAI_DETAIL D ON MAP.HENTAI_ID = D.PKEY
   AND {0}
 WHERE MAP.STATUS = 'Y'
AND MI.ID = '{1}'
   AND exists(
	SELECT DISTINCT MI.ID
	  FROM MELONBOOK..MAP_HENTAI MAP
	 INNER JOIN MELONBOOK..ITEM MI ON MAP.MELON_ID = MI.ID
	 INNER JOIN ANIMELIBRARY..DATA_EXHENTAI_DETAIL D ON MAP.HENTAI_ID = D.PKEY
	   AND {0}
	 WHERE MAP.STATUS = 'Y'
 )
 ORDER BY MI.ID DESC";

            string filter = "";
            
            if (type != "ALL")
            {
                filter += (filter != "" ? " AND " : "") + string.Format("D.TYPE = '{0}'", type);

            }
            if (lang != "ALL")
            {
                filter += (filter != "" ? " AND " : "") + string.Format("D.LANG = '{0}'", lang);

            }
            if (filter == "")
            {
                filter = "1 = 1";

            }

            sql = string.Format(sql, filter, melonID);

            DataTable data = this.QueryDBData(sql, conStrMain);

            return data;

        }

        private DataTable GetMelonBookMapDetailData(string type, string lang, List<string> melonList)
        {
            string sql = @"
SELECT MAP.MELON_ID , D.*
  FROM MELONBOOK..MAP_HENTAI MAP
 INNER JOIN ANIMELIBRARY..DATA_EXHENTAI_DETAIL D ON MAP.HENTAI_ID = D.PKEY
   AND {0}
 WHERE MAP.STATUS = 'Y'
   and MAP.MELON_ID in ({1})";

            string filter = "";
            string top = "";
            if (type != "ALL")
            {
                filter += (filter != "" ? " AND " : "") + string.Format("D.TYPE = '{0}'", type);

            }
            if (lang != "ALL")
            {
                filter += (filter != "" ? " AND " : "") + string.Format("D.LANG = '{0}'", lang);

            }
            
            if (filter == "")
            {
                filter = "1 = 1";

            }

            sql = string.Format(sql, filter, string.Join(",", melonList));

            DataTable data = this.QueryDBData(sql, conStrMain);

            return data;

        }
        
        private DataTable GetMelonBookTagMappingData(string tagId, string type, string lang, string page, string param)
        {
            string sql = @"
SELECT 
{3}
*
FROM MELONBOOK.DBO.ITEM ITEM
WHERE 1=1
AND  EXISTS
(
	SELECT *
	FROM MELONBOOK.DBO.MAP_HENTAI MAP_HENTAI
	INNER JOIN MELONBOOK.DBO.ITEM_TAG ITEM_TAG ON MAP_HENTAI.MELON_ID = ITEM_TAG.ITEM_ID
	INNER JOIN MELONBOOK.DBO.TAG TAG ON TAG.GUID = ITEM_TAG.TAG_GUID
	WHERE 1=1
	AND MAP_HENTAI.MELON_ID = ITEM.ID
	AND (TAG.GUID = '{0}' OR TAG.TAG_NAME = N'{0}') {1}
)
{2}
ORDER BY ITEM.ID {4}";

            string filter = "";
            string whereFilter = "";
            string top = "";
            string sort = "DESC";

            if (type != "ALL")
            {
                filter += string.Format(" AND MAP_HENTAI.TYPE = '{0}'", type);

            }
            if (lang != "ALL")
            {
                filter += string.Format(" AND MAP_HENTAI.LANG = '{0}'", lang);

            }

            if (param == "")
            {
                if (page != "ALL")
                {
                    top = string.Format("TOP {0}", maxDisplayMelonBook * (int.Parse(page) + 1));

                }

            }
            else
            {
                if (int.Parse(param) >= 0)
                {
                    whereFilter += string.Format(" AND ITEM.ID < {0}", param);

                    sort = "DESC";

                }
                else
                {
                    whereFilter += string.Format(" AND ITEM.ID > {0}", param.ToString().Replace("-", ""));

                    sort = "ASC";

                }

                top = string.Format("TOP {0}", maxDisplayMelonBook * 1);

            }
            sql = string.Format(sql, tagId, filter, whereFilter, top, sort);

            DataTable data = this.QueryDBData(sql, conStrBook);

            return data;

        }

        private DataTable GetMelonBookTagMapSummaryData(string type, string lang, string mapDone)
        {
            string sql = @"
select 
    TAG.GUID , 
    TAG.TAG_NAME ,
    count(*) CNT
from MELONBOOK.dbo.TAG TAG
inner join MELONBOOK.dbo.ITEM_TAG ITEM_TAG on TAG.GUID = ITEM_TAG.TAG_GUID
where 1 = 1
and TAG.TAG_CHECK = 'Y'
and exists
(
	select *
	from MELONBOOK.dbo.MAP_HENTAI MAP_HENTAI
	inner join MELONBOOK.dbo.ITEM ITEM on MAP_HENTAI.MELON_ID = ITEM.ID
	where 1=1
	and MAP_HENTAI.STATUS = 'Y' {0}
	and MAP_HENTAI.MELON_ID = ITEM_TAG.ITEM_ID
)
group by TAG.GUID , TAG.TAG_NAME 
having count(*) > 1
order by count(*) DESC , TAG.GUID , TAG.TAG_NAME";

            string filter = "";

            if (type != "ALL")
            {
                filter += string.Format(" AND MAP_HENTAI.TYPE = '{0}'", type);

            }
            if (lang != "ALL")
            {
                filter += string.Format(" AND MAP_HENTAI.LANG = '{0}'", lang);

            }
            if (mapDone != "")
            {
                filter += string.Format(" AND ITEM.MAP_DONE = '{0}'", mapDone);

            }

            sql = string.Format(sql, filter);

            DataTable data = this.QueryDBData(sql, conStrBook);

            return data;

        }

        #endregion DA

        public ActionResult Index(string type, string lang, string page, string param)
        {
            ViewData["TYPE"] = type;
            ViewData["LANG"] = lang;
            ViewData["PAGE"] = page;

            return View();

        }

    }
}