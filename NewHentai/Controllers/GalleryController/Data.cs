using HentaiCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;

namespace NewHentai.Controllers
{
    public partial class GalleryController : Controller
    {
        public ActionResult Data(string param)
        {
            if (param != null && param.Split('_').Length != 2)
            {
                ViewData["MSG"] = "找不到資料";

            }
            else
            {
                string pKey = param.Split('_')[0];
                string sKey = param.Split('_')[1];

                DataTable dataBook = this.GetGalleryBookData(pKey, sKey);
                DataTable dataPic = this.GetGalleryBookPic(pKey, sKey);

                if (dataBook.Rows.Count == 0)
                {
                    ViewData["MSG"] = "找不到資料";

                }
                else
                {
                    try
                    {
                        int maxFetchTry = 3;
                        int nowFetchTry = 0;

                        int picPage = int.Parse(dataBook.Rows[0]["PAGE"].ToString());

                        string basePage = System.Configuration.ConfigurationManager.AppSettings["BasePage"].ToString();
                        string localPath = System.Configuration.ConfigurationManager.AppSettings["LocalSavePath"].ToString();
                        string webPath = System.Configuration.ConfigurationManager.AppSettings["WebSavePath"].ToString();

                        string url = basePage + "/CRAW/GET_EXHENTAI_PIC/{0}_{1}";

                        url = string.Format(url, pKey, sKey);

                        string errMsg = "";
                        while (nowFetchTry < maxFetchTry)
                        {
                            try
                            {
                                HttpWebRequest req = HttpWebRequest.Create(url) as HttpWebRequest;
                                req.Timeout = 1000 * 15 * picPage;

                                using (HttpWebResponse res = req.GetResponse() as HttpWebResponse)
                                {
                                    if (res.StatusCode != HttpStatusCode.OK)
                                    {
                                        throw new Exception("找不到圖片列表資料");

                                    }

                                }

                                dataPic = this.GetGalleryBookPic(pKey, sKey);

                                if (dataPic.Rows.Count == 0)
                                {
                                    throw new Exception("找不到圖片資料");

                                }

                                errMsg = "";

                                break;

                            }
                            catch (Exception ex)
                            {
                                nowFetchTry++;

                                errMsg += Environment.NewLine + string.Format("FATCH {0} = {1}", nowFetchTry, ex.ToString());

                                WebException webEx = ex as WebException;
                                if (webEx != null)
                                {
                                    using (StreamReader reader = new StreamReader((webEx.Response as HttpWebResponse).GetResponseStream()))
                                    {
                                        errMsg += Environment.NewLine + reader.ReadToEnd();

                                    }

                                }

                            }

                        }

                        if (errMsg != "")
                        {
                            throw new Exception(errMsg);

                        }

                        nowFetchTry = 0;

                        while (nowFetchTry < maxFetchTry)
                        {
                            try
                            {
                                DirectoryInfo dirInfoSrc = new DirectoryInfo(string.Format(Server.MapPath("/{0}/{1}"), localPath, pKey.PadLeft(7, '0')));

                                if (!dirInfoSrc.Exists)
                                {
                                    dirInfoSrc.Create();

                                }

                                DirectoryInfo dirInfoDst = new DirectoryInfo(string.Format(Server.MapPath("/{0}/{1}"), webPath, pKey.PadLeft(7, '0')));

                                if (dirInfoDst.Exists)
                                {
                                    try
                                    {
                                        dirInfoDst.Delete(true);
                                        dirInfoDst.Create();

                                    }
                                    catch (Exception ex)
                                    {
                                        Central.logger.WriteWarning(ex.ToString());

                                    }

                                }

                                break;

                            }
                            catch (Exception ex)
                            {
                                nowFetchTry++;

                                Thread.Sleep(1000);

                                errMsg += Environment.NewLine + string.Format("DIR {0} = {1}", nowFetchTry, ex.ToString());

                                WebException webEx = ex as WebException;
                                if (webEx != null)
                                {
                                    using (StreamReader reader = new StreamReader((webEx.Response as HttpWebResponse).GetResponseStream()))
                                    {
                                        errMsg += Environment.NewLine + reader.ReadToEnd();

                                    }

                                }

                            }

                        }

                        if (errMsg != "")
                        {
                            Central.logger.WriteWarning(errMsg);

                        }

                        ViewData["BOOK"] = dataBook;
                        ViewData["PIC"] = dataPic;

                    }
                    catch (Exception ex)
                    {
                        ViewData["MSG"] = ex.ToString();

                    }

                }

                List<List<string>> picThread = new List<List<string>>();
                List<Thread> threadList = new List<Thread>();
                int maxPicThread = 5, nowPic = dataPic.Rows.Count;

                for (int i = 0; i < maxPicThread; i++)
                {
                    picThread.Add(new List<string>());

                }

                for(int j = 0; j<dataPic.Rows.Count; j++)
                {
                    picThread[j % maxPicThread].Add(
                        System.Configuration.ConfigurationManager.AppSettings["BasePage"].ToString() + "/" + dataPic.Rows[j]["SAVE_PATH"].ToString());

                }

                for (int i = 0; i < maxPicThread; i++)
                {
                    Thread t = new Thread(new ParameterizedThreadStart(RunPicGenerate));
                    t.Start(picThread[i]);

                    Thread.Sleep(2000);

                    threadList.Add(t);

                }

                while (true)
                {
                    if (
                    threadList.AsEnumerable().Where(x => x.ThreadState == ThreadState.Running)
                        .Count() == 0)
                    {
                        break;
                    }

                    Thread.Sleep(1000);

                }

            }

            return View();

        }

        private void RunPicGenerate(object list)
        {
            List<string> urlList = list as List<string>;

            foreach (string url in urlList)
            {
                try
                {
                    HttpWebRequest req = HttpWebRequest.Create(url.ToString()) as HttpWebRequest;
                    req.Timeout = 1000 * 15;

                    using (HttpWebResponse res = req.GetResponse() as HttpWebResponse)
                    {
                        if (res.StatusCode != HttpStatusCode.OK)
                        {
                            throw new Exception("找不到圖片列表資料");

                        }

                    }

                }
                catch (Exception ex)
                {

                }

            }

        }

    }

}
