using HentaiCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewHentai.Controllers
{
    public partial class AdminController : Controller
    {

        private DataControlMdf control = new DataControlMdf();
        private string conStrMain = @"Data Source={0}\SQLEXPRESS;DATABASE=HENTAI;Integrated Security=false;User ID=sa;PASSWORD=c06g4cl6;Connection Timeout=180";
        private string conStrLib = @"Data Source={0}\SQLEXPRESS;DATABASE=ANIMELIBRARY;Integrated Security=false;User ID=sa;PASSWORD=c06g4cl6;Connection Timeout=180";
        private string conStrPic = @"Data Source={0}\SQLEXPRESS;DATABASE=HENTAIPIC;Integrated Security=false;User ID=sa;PASSWORD=c06g4cl6;Connection Timeout=180";
        private string conStrBook = @"Data Source={0}\SQLEXPRESS;DATABASE=MELONBOOK;Integrated Security=false;User ID=sa;PASSWORD=c06g4cl6;Connection Timeout=600";

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

        #endregion Core

        #region DA
        private DateTime? GetSummaryUpdateTime()
        {
            string sql = "SELECT MAX(M.GROUP_UPDATE_TIME) TIME FROM SUMMARY M";

            DataTable data = this.QueryDBData(sql, conStrMain);

            if (data.Rows.Count == 0 || data.Rows[0]["TIME"] == DBNull.Value)
            {
                return null;

            }

            DateTime time = Convert.ToDateTime(data.Rows[0]["TIME"].ToString());

            return time;

        }

        private DataTable GetEmptySummaryData()
        {
            string sql = "SELECT * FROM SUMMARY WHERE 1 = 2";

            DataTable data = this.QueryDBData(sql, conStrMain);

            return data;

        }

        private DataTable GetSummaryCalcData()
        {
            string sql = @"
SELECT *
FROM(
	SELECT (PKEY / 10000) GROUP_ID,
	'ERROR' GROUP_TYPE,
	'' GROUP_LANGUAGE,
	COUNT(*) GROUP_COUNT
	FROM(
	SELECT 
	CONVERT(INT, SUBSTRING(ID,4 , LEN(ID)-14)) PKEY
	FROM [OBJECT]
	WHERE STATUS = '9') Y
	GROUP BY (PKEY / 10000)

	UNION ALL

	SELECT
	  T.GROUP_ID GROUP_ID,
      X.TYPE GROUP_TYPE,	  
      X.LANG GROUP_LANGUAGE,
	  X.CNT GROUP_COUNT
	FROM (
	  SELECT DISTINCT
		(PKEY / 10000) GROUP_ID
		--ISNULL(TYPE, ' ') GROUP_TYPE,
		--ISNULL(LANG, ' ')  GROUP_LANGUAGE
	  FROM DATA_EXHENTAI) T
	  LEFT OUTER JOIN (
		  SELECT
			PKEY / 10000 P,
			ISNULL(M.TYPE, ' ') TYPE,
			ISNULL(M.LANG, ' ') LANG,
			COUNT(*) CNT
		  FROM DATA_EXHENTAI M
		  GROUP BY (PKEY / 10000), M.TYPE, M.LANG
	  ) X
	  ON X.P = T.GROUP_ID --AND T.GROUP_LANGUAGE = X.LANG AND T.GROUP_TYPE = X.TYPE
) Z
--ORDER BY 
--Z.GROUP_ID DESC,
--CASE Z.GROUP_TYPE
--  WHEN ' ' THEN '1'
--  WHEN 'ERROR' THEN '2'
--  ELSE Z.GROUP_TYPE END";

            DataTable data = this.QueryDBData(sql, conStrMain);

            return data;

        }

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

        private DataTable GetObjectStatusData()
        {
            string sql = @"
select 'ALL' CODE_ID , (select SUM(GROUP_COUNT) from SUMMARY ) CNT
union all
select 'OBJ' CODE_ID , (select SUM(GROUP_COUNT) from SUMMARY where GROUP_TYPE = '' and GROUP_LANGUAGE = '') CNT
union all
select 'EXHENTAI' CODE_ID , (select SUM(GROUP_COUNT) from SUMMARY where GROUP_TYPE <> 'ERROR' and GROUP_LANGUAGE <> '' ) CNT
union all
select 'ERROR' CODE_ID , (select SUM(GROUP_COUNT) from SUMMARY where GROUP_TYPE = 'ERROR') CNT";

            DataTable data = this.QueryDBData(sql, conStrMain);

            return data;

        }

        private DataTable GetLangTypeStatusData()
        {
            string sql = @"
SELECT 'Japanese' LANG,
(SELECT SUM(GROUP_COUNT) FROM SUMMARY M 
  WHERE 1=1 and GROUP_LANGUAGE = 'Japanese' and GROUP_TYPE = 'doujinshi') AS CNT_DOUJINSHI,
(SELECT SUM(GROUP_COUNT) FROM SUMMARY M 
  WHERE 1=1 and GROUP_LANGUAGE = 'Japanese' and GROUP_TYPE = 'manga') AS CNT_MANGA,
(SELECT SUM(GROUP_COUNT) FROM SUMMARY M 
  WHERE 1=1 and GROUP_LANGUAGE = 'Japanese' and GROUP_TYPE = 'non-h') AS CNT_NON_H
UNION ALL
SELECT 'Chinese' LANG,
(SELECT SUM(GROUP_COUNT) FROM SUMMARY M 
  WHERE 1=1 and GROUP_LANGUAGE = 'Chinese' and GROUP_TYPE = 'doujinshi') AS CNT_DOUJINSHI,
(SELECT SUM(GROUP_COUNT) FROM SUMMARY M 
  WHERE 1=1 and GROUP_LANGUAGE = 'Chinese' and GROUP_TYPE = 'manga') AS CNT_MANGA,
(SELECT SUM(GROUP_COUNT) FROM SUMMARY M 
  WHERE 1=1 and GROUP_LANGUAGE = 'Chinese' and GROUP_TYPE = 'non-h') AS CNT_NON_H
UNION ALL
SELECT 'Other' LANG,
(SELECT SUM(GROUP_COUNT) FROM SUMMARY M 
  WHERE 1=1 and GROUP_LANGUAGE NOT IN ('Japanese','Chinese') and GROUP_TYPE = 'doujinshi') AS CNT_DOUJINSHI,
(SELECT SUM(GROUP_COUNT) FROM SUMMARY M 
  WHERE 1=1 and GROUP_LANGUAGE NOT IN ('Japanese','Chinese') and GROUP_TYPE = 'manga') AS CNT_MANGA,
(SELECT SUM(GROUP_COUNT) FROM SUMMARY M 
  WHERE 1=1 and GROUP_LANGUAGE NOT IN ('Japanese','Chinese') and GROUP_TYPE = 'non-h') AS CNT_NON_H
";

            DataTable data = this.QueryDBData(sql, conStrMain);

            return data;

        }

        private DataTable GetGroupTypeData()
        {
            string sql = @"
SELECT 
    M.GROUP_ID P, 
    M.GROUP_TYPE TYPE,
    SUM(GROUP_COUNT) CNT
  FROM SUMMARY M
 GROUP BY M.GROUP_ID , M.GROUP_TYPE
 ORDER BY 
CONVERT(INT, M.GROUP_ID) DESC,
CASE M.GROUP_TYPE
  WHEN ' ' THEN '1'
  WHEN 'ERROR' THEN '2'
  ELSE M.GROUP_TYPE END";

            DataTable data = this.QueryDBData(sql, conStrMain);

            return data;

        }

        private DataTable GetMelonGroupTypeData()
        {
            string sql = @"
SELECT 
       CONVERT(INT, ( ID / 20000 )) GR,
       '0' SORT,
       'ALL' CAT,
       COUNT(*)CNT
  FROM [MELONBOOK].[DBO].[ITEM]
 WHERE STATUS = '1'
 GROUP BY CONVERT(INT, ( ID / 20000 ))
 UNION ALL
SELECT 
       '-1' GR,
       '0' SORT,
       CAT_2 CAT,
       COUNT(*)CNT
  FROM [MELONBOOK].[DBO].[ITEM]
 WHERE STATUS = '1'
 GROUP BY CAT_2
 UNION ALL
SELECT 
       CONVERT(INT, ( ID / 20000 )) GR,
       '2' SORT,
       CAT_2 CAT,
       COUNT(*) CNT
  FROM [MELONBOOK].[DBO].[ITEM]
 WHERE STATUS = '1'
 GROUP BY CONVERT(INT, ( ID / 20000 )), CAT_2
 ORDER BY SORT, GR, CAT ";

            DataTable data = this.QueryDBData(sql, conStrBook);

            return data;

        }

        private void ProcessSummary()
        {
            DataTable dataSummary = this.GetEmptySummaryData();
            DataTable dataCalc = this.GetSummaryCalcData();

            foreach (DataRow row in dataCalc.Rows)
            {
                DataRow[] rows = dataSummary.Select(
                    string.Format(
                        "GROUP_ID = '{0}' and GROUP_TYPE = '{1}' and GROUP_LANGUAGE = '{2}'",
                        row["GROUP_ID"], row["GROUP_TYPE"], row["GROUP_LANGUAGE"]));

                if (rows.Length == 0)
                {
                    DataRow newRow = dataSummary.NewRow();

                    foreach (DataColumn col in dataCalc.Columns)
                    {
                        newRow[col.ColumnName] = row[col.ColumnName];

                    }

                    newRow["GROUP_USER"] = "SYSTEM";
                    newRow["GROUP_UPDATE_TIME"] = DateTime.Now;

                    dataSummary.Rows.Add(newRow);

                }
                else
                {
                    DataRow selRow = rows[0];

                    selRow["GROUP_COUNT"] = row["GROUP_COUNT"].ToString();
                    selRow["GROUP_USER"] = "SYSTEM";
                    selRow["GROUP_UPDATE_TIME"] = DateTime.Now;

                }

            }
            this.UploadDBData(new string[] { "TRUNCATE TABLE SUMMARY" }, conStrMain);
            this.UploadDBData(dataSummary, "SELECT * FROM SUMMARY", conStrMain);

        }

        public void ExexuteSummaryData()
        {
            string server = System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("DB") ?
                System.Configuration.ConfigurationManager.AppSettings["DB"] :
                ".";

            string conStr = string.Format(conStrMain, server);

            using (SqlConnection conn = new SqlConnection(conStr))
            {
                conn.Open();

                SqlCommand com = conn.CreateCommand();
                com.CommandTimeout = 0;
                com.CommandType = CommandType.StoredProcedure;
                com.CommandText = "HSP_HENTAI_SUMMARY";

                com.ExecuteNonQuery();

            }

        }

        #endregion DA

    }
}