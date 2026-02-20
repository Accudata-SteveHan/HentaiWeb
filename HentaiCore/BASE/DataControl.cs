using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
//using System.Data.SqlServerCe;
using System.Data.SqlClient;

namespace HentaiCore
{
    public class DataControlMdf
    {
        public string conStr = @"";

        public DataTable GetData(string sql)
        {
            int nowCount = 0;
            int maxCount = 3;

            while (nowCount < maxCount)
            {
                try
                {
                    DataTable result = new DataTable();

                    using (SqlDataAdapter ap = new SqlDataAdapter(sql, conStr))
                    {
                        ap.SelectCommand.CommandTimeout = 300;
                        ap.Fill(result);


                    }
                    return result;

                }
                catch (Exception ex)
                {
                    nowCount++;

                    if (nowCount >= maxCount)
                    {
                        throw ex;

                    }

                }

            }

            return null;

        }

        //public DataTable GetData(string webApiPath, string sql)
        //{
        //    Json paramJson = new Json();
        //    paramJson.Add("COMMAND", sql);

        //    Uri url = new Uri(webApiPath);

        //    if (HentaiCore.Central.logger == null)
        //    {
        //        HentaiCore.Logger logger = new HentaiCore.Logger(HentaiCore.Logger.Level.Critical);
        //        HentaiCore.Central.logger = logger;

        //    }

        //    HentaiCore.HTML html = new HentaiCore.HTML(url.ToString());
        //    html.postData = Encoding.UTF8.GetBytes(paramJson.ToString());

        //    html.GetJson();
        //    string result = html.webHtml;

        //    DataTable data = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(result);
        //    data.AcceptChanges();

        //    return data;

        //}

        public void UploadData(DataTable data, string destTable, string[] keys)
        {
            string select = "SELECT * FROM {0} WHERE 1 = 2";
            string insert = "INSERT INTO {0} ({1}) VALUES ({2});";
            string update = "UPDATE {0} SET {1} WHERE {2}";
            string delete = "DELETE FROM {0} WHERE {1};";

            try
            {
                List<string> sqlList = new List<string>();

                DataTable emptyTable = this.GetData(string.Format(select, destTable));

                foreach (DataRow row in data.Rows)
                {
                    string sqlStr = "";

                    #region Added
                    if (row.RowState == DataRowState.Added)
                    {
                        sqlStr = insert;

                        string sqlColStr = "", sqlValStr = "";
                        foreach (DataColumn col in data.Columns)
                        {
                            if (emptyTable.Columns.IndexOf(col.ColumnName) < 0)
                            {
                                continue;

                            }

                            sqlColStr += (sqlColStr != "" ? "," : "") + string.Format("[{0}]", col.ColumnName);

                            if (col.DataType == typeof(string) || col.DataType == typeof(String))
                            {
                                sqlValStr += (sqlValStr != "" ? "," : "") + string.Format("N'{0}'", row[col.ColumnName]);

                            }
                            else if (col.DataType != typeof(DateTime))
                            {
                                sqlValStr += (sqlValStr != "" ? "," : "") + string.Format("'{0}'", row[col.ColumnName]);

                            }
                            else
                            {
                                sqlValStr += (sqlValStr != "" ? "," : "") + string.Format("'{0}'", DateTime.Parse(row[col.ColumnName].ToString()).ToString("yyyy-MM-dd HH:mm:ss"));

                            }

                        }

                        sqlStr = string.Format(sqlStr, destTable, sqlColStr, sqlValStr);
                        sqlList.Add(sqlStr);

                    }

                    #endregion Added

                    #region Modified
                    if (row.RowState == DataRowState.Modified)
                    {
                        sqlStr = update;

                        string updateColStr = "";
                        foreach (DataColumn col in data.Columns)
                        {
                            if (emptyTable.Columns.IndexOf(col.ColumnName) < 0)
                            {
                                continue;

                            }

                            if (col.DataType == typeof(string) || col.DataType == typeof(String))
                            {
                                updateColStr +=
                                (updateColStr != "" ? "," : "") +
                                string.Format("[{0}] = N'{1}'", col.ColumnName, row[col.ColumnName]);

                            }
                            else if (col.DataType != typeof(DateTime))
                            {
                                updateColStr +=
                                (updateColStr != "" ? "," : "") +
                                string.Format("[{0}] = '{1}'", col.ColumnName, row[col.ColumnName]);

                            }
                            else
                            {
                                updateColStr +=
                                (updateColStr != "" ? "," : "") +
                                string.Format("[{0}] = '{1}'", col.ColumnName, DateTime.Parse(row[col.ColumnName].ToString()).ToString("yyyy-MM-dd HH:mm:ss"));

                            }

                        }

                        string updateWhereStr = "";
                        foreach (string key in keys)
                        {
                            updateWhereStr +=
                                (updateWhereStr != "" ? " AND " : "") +
                                string.Format("{0} = '{1}'", key, row[key]);

                        }

                        sqlStr = string.Format(sqlStr, destTable, updateColStr, updateWhereStr);
                        sqlList.Add(sqlStr);

                    }

                    #endregion Modified

                    #region Deleted
                    if (row.RowState == DataRowState.Deleted)
                    {
                        sqlStr = delete;
                        row.RejectChanges();
                        string updateWhereStr = "";
                        foreach (string key in keys)
                        {
                            updateWhereStr +=
                                (updateWhereStr != "" ? " AND " : "") +
                                string.Format("[{0}] = '{1}'", key, row[key]);

                        }

                        sqlStr = string.Format(sqlStr, destTable, updateWhereStr);
                        sqlList.Add(sqlStr);
                        row.Delete();

                    }

                    #endregion Deleted

                }

                this.UploadData(sqlList.ToArray());

            }
            catch (Exception ex)
            {
                throw ex;

            }

        }

        //public void UploadData(string webApiPathQuery, string webApiPathUpdate, DataTable data, string destTable, string[] keys)
        //{
        //    string select = "SELECT * FROM {0} WHERE 1 = 2";
        //    string insert = "INSERT INTO {0} ({1}) VALUES ({2});";
        //    string update = "UPDATE {0} SET {1} WHERE {2}";
        //    string delete = "DELETE FROM {0} WHERE {1};";

        //    try
        //    {
        //        List<string> sqlList = new List<string>();

        //        DataTable emptyTable = this.GetData(webApiPathQuery, string.Format(select, destTable));

        //        foreach (DataRow row in data.Rows)
        //        {
        //            string sqlStr = "";

        //            #region Added
        //            if (row.RowState == DataRowState.Added)
        //            {
        //                sqlStr = insert;

        //                string sqlColStr = "", sqlValStr = "";
        //                foreach (DataColumn col in data.Columns)
        //                {
        //                    if (emptyTable.Columns.IndexOf(col.ColumnName) < 0)
        //                    {
        //                        continue;

        //                    }

        //                    sqlColStr += (sqlColStr != "" ? "," : "") + string.Format("[{0}]", col.ColumnName);

        //                    if (col.DataType == typeof(string) || col.DataType == typeof(String))
        //                    {
        //                        sqlValStr += (sqlValStr != "" ? "," : "") + string.Format("N'{0}'", row[col.ColumnName]);

        //                    }
        //                    else if (col.DataType != typeof(DateTime))
        //                    {
        //                        sqlValStr += (sqlValStr != "" ? "," : "") + string.Format("'{0}'", row[col.ColumnName]);

        //                    }
        //                    else
        //                    {
        //                        sqlValStr += (sqlValStr != "" ? "," : "") + string.Format("'{0}'", DateTime.Parse(row[col.ColumnName].ToString()).ToString("yyyy-MM-dd HH:mm:ss"));

        //                    }

        //                }

        //                sqlStr = string.Format(sqlStr, destTable, sqlColStr, sqlValStr);
        //                sqlList.Add(sqlStr);

        //            }

        //            #endregion Added

        //            #region Modified
        //            if (row.RowState == DataRowState.Modified)
        //            {
        //                sqlStr = update;

        //                string updateColStr = "";
        //                foreach (DataColumn col in data.Columns)
        //                {
        //                    if (emptyTable.Columns.IndexOf(col.ColumnName) < 0)
        //                    {
        //                        continue;

        //                    }

        //                    if (col.DataType == typeof(string) || col.DataType == typeof(String))
        //                    {
        //                        updateColStr +=
        //                        (updateColStr != "" ? "," : "") +
        //                        string.Format("[{0}] = N'{1}'", col.ColumnName, row[col.ColumnName]);

        //                    }
        //                    else if (col.DataType != typeof(DateTime))
        //                    {
        //                        updateColStr +=
        //                        (updateColStr != "" ? "," : "") +
        //                        string.Format("[{0}] = '{1}'", col.ColumnName, row[col.ColumnName]);

        //                    }
        //                    else
        //                    {
        //                        updateColStr +=
        //                        (updateColStr != "" ? "," : "") +
        //                        string.Format("[{0}] = '{1}'", col.ColumnName, DateTime.Parse(row[col.ColumnName].ToString()).ToString("yyyy-MM-dd HH:mm:ss"));

        //                    }

        //                }

        //                string updateWhereStr = "";
        //                foreach (string key in keys)
        //                {
        //                    updateWhereStr +=
        //                        (updateWhereStr != "" ? " AND " : "") +
        //                        string.Format("{0} = '{1}'", key, row[key]);

        //                }

        //                sqlStr = string.Format(sqlStr, destTable, updateColStr, updateWhereStr);
        //                sqlList.Add(sqlStr);

        //            }

        //            #endregion Modified

        //            #region Deleted
        //            if (row.RowState == DataRowState.Deleted)
        //            {
        //                sqlStr = delete;
        //                row.RejectChanges();
        //                string updateWhereStr = "";
        //                foreach (string key in keys)
        //                {
        //                    updateWhereStr +=
        //                        (updateWhereStr != "" ? " AND " : "") +
        //                        string.Format("[{0}] = '{1}'", key, row[key]);

        //                }

        //                sqlStr = string.Format(sqlStr, destTable, updateWhereStr);
        //                sqlList.Add(sqlStr);
        //                row.Delete();

        //            }

        //            #endregion Deleted

        //        }

        //        this.UploadData(webApiPathUpdate, sqlList.ToArray());

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;

        //    }

        //}

        public int UploadData(string[] sqls)
        {
            using (SqlConnection c = new SqlConnection(conStr))
            {
                c.Open();
                SqlTransaction t = c.BeginTransaction();

                try
                {
                    foreach (string sql in sqls)
                    {
                        SqlCommand a = new SqlCommand(sql, c, t);
                        a.ExecuteNonQuery();

                    }

                    t.Commit();

                    return 0;

                }
                catch (Exception ex)
                {
                    t.Rollback();

                    return -1;
                }

            }

        }

        public int UploadData(string mainSql, DataTable data)
        {
            int result = 0;
            try
            {
                using (SqlDataAdapter ap = new SqlDataAdapter(mainSql, conStr))
                {
                    SqlCommandBuilder builder = new SqlCommandBuilder(ap);

                    result = ap.Update(data);

                }

                return result;

            }
            catch (Exception ex)
            {
                throw ex;

            }

        }

        //public void UploadData(string webApiPath, string[] sqls)
        //{
        //    Json paramJson = new Json();
        //    paramJson.Add("COMMAND", Newtonsoft.Json.JsonConvert.SerializeObject(sqls));

        //    Uri url = new Uri(webApiPath);

        //    if (HentaiCore.Central.logger == null)
        //    {
        //        HentaiCore.Logger logger = new HentaiCore.Logger(HentaiCore.Logger.Level.Critical);
        //        HentaiCore.Central.logger = logger;

        //    }

        //    HentaiCore.HTML html = new HentaiCore.HTML(url.ToString());
        //    html.postData = Encoding.UTF8.GetBytes(paramJson.ToString());

        //    html.GetJson();
        //    string result = html.webHtml;


        //}

    }

}
