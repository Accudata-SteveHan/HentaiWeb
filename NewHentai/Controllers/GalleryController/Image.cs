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
        public ActionResult Image(string param)
        {
            if (param == null)
            {
                return View();

            }

            bool success = false;
            int time = 0;
            string path = "";

            while (!success && time < 3)
            {
                time++;

                try
                {
                    string localPath = System.Configuration.ConfigurationManager.AppSettings["LocalSavePath"].ToString();
                    string webPath = System.Configuration.ConfigurationManager.AppSettings["WebSavePath"].ToString();

                    string[] p = param.ToString().Split('_');
                    string pKey = p[0];
                    string sKey = p[1];
                    string imagePage = p[2];

                    //System.Threading.Thread.Sleep(1000 * int.Parse(imagePage) / 4 + 200 * (int.Parse(imagePage) % 4));

                    DirectoryInfo dirInfoDst = new DirectoryInfo(string.Format(Server.MapPath("/{0}/{1}"), webPath, pKey.PadLeft(7, '0')));

                    if (!dirInfoDst.Exists)
                    {
                        dirInfoDst.Create();

                    }

                    path = Path.Combine(string.Format(Server.MapPath("/{0}/{1}"), webPath, pKey.PadLeft(7, '0')), param + ".jpg");

                    FileInfo file = new FileInfo(path);

                    if (!file.Exists)
                    {
                        //FileInfo localFileInfo = new FileInfo(Path.Combine(Path.Combine(localPath, pKey.PadLeft(7, '0')), param + ".jpg"));
                        FileInfo localFileInfo = new FileInfo(Path.Combine(string.Format(Server.MapPath("/{0}/{1}"), localPath, pKey.PadLeft(7, '0')), param + ".jpg"));

                        if (localFileInfo.Exists)
                        {
                            //localFileInfo.CopyTo(path, true);
                            localFileInfo.CopyTo(path, false);

                        }
                        else
                        {
                            string basePage = System.Configuration.ConfigurationManager.AppSettings["BasePage"].ToString();
                            string url = basePage + "/CRAW/DOWNLOAD_EXHENTAI_PIC/{0}";

                            url = string.Format(url, param);

                            HttpWebRequest req = HttpWebRequest.Create(url) as HttpWebRequest;
                            req.Timeout = 1000 * 60 * 10;

                            using (HttpWebResponse res = req.GetResponse() as HttpWebResponse)
                            {
                                if (res.StatusCode != HttpStatusCode.OK)
                                {
                                    throw new Exception("");

                                }

                            }

                        }

                    }

                    path = Path.Combine(string.Format(Server.MapPath("/{0}/{1}"), webPath, pKey.PadLeft(7, '0')), param + ".jpg");

                }
                catch (Exception ex)
                {


                }

                FileInfo fileInfo = new FileInfo(path);

                if (fileInfo.Exists)
                {
                    success = true;

                }

            }

            return base.File(path, "image/jpg");

        }

        public ActionResult ReImage(string param)
        {
            if (param == null)
            {
                return View();

            }

            bool success = false;
            int time = 0;
            string path = "";

            while (!success && time < 3)
            {
                time++;

                try
                {
                    string localPath = System.Configuration.ConfigurationManager.AppSettings["LocalSavePath"].ToString();
                    string webPath = System.Configuration.ConfigurationManager.AppSettings["WebSavePath"].ToString();

                    string[] p = param.ToString().Split('_');
                    string pKey = p[0];
                    string sKey = p[1];
                    string imagePage = p[2];

                    //System.Threading.Thread.Sleep(1000 * int.Parse(imagePage) / 4 + 200 * (int.Parse(imagePage) % 4));

                    DirectoryInfo dirInfoDst = new DirectoryInfo(string.Format(Server.MapPath("/{0}/{1}"), webPath, pKey.PadLeft(7, '0')));

                    if (!dirInfoDst.Exists)
                    {
                        dirInfoDst.Create();

                    }

                    path = Path.Combine(string.Format(Server.MapPath("/{0}/{1}"), webPath, pKey.PadLeft(7, '0')), param + ".jpg");

                    FileInfo file = new FileInfo(path);

                    //if (!file.Exists)
                    {
                        //FileInfo localFileInfo = new FileInfo(Path.Combine(Path.Combine(localPath, pKey.PadLeft(7, '0')), param + ".jpg"));
                        FileInfo localFileInfo = new FileInfo(Path.Combine(string.Format(Server.MapPath("/{0}/{1}"), localPath, pKey.PadLeft(7, '0')), param + ".jpg"));

                        //if (localFileInfo.Exists)
                        {
                            //localFileInfo.CopyTo(path, true);

                        }
                        //else
                        {
                            string basePage = System.Configuration.ConfigurationManager.AppSettings["BasePage"].ToString();
                            string url = basePage + "/CRAW/DOWNLOAD_EXHENTAI_PIC/{0}";

                            url = string.Format(url, param);

                            HttpWebRequest req = HttpWebRequest.Create(url) as HttpWebRequest;
                            req.Timeout = 1000 * 60 * 10;

                            using (HttpWebResponse res = req.GetResponse() as HttpWebResponse)
                            {
                                if (res.StatusCode != HttpStatusCode.OK)
                                {
                                    throw new Exception("");

                                }

                            }

                        }

                    }

                    path = Path.Combine(string.Format(Server.MapPath("/{0}/{1}"), webPath, pKey.PadLeft(7, '0')), param + ".jpg");

                }
                catch (Exception ex)
                {


                }

                FileInfo fileInfo = new FileInfo(path);

                if (fileInfo.Exists)
                {
                    success = true;

                }

            }

            return base.File(path, "image/jpg");

        }
        
    }

}
