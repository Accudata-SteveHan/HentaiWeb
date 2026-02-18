using HentaiCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;

namespace NewHentai.Controllers
{
    public partial class GalleryController : Controller
    {
        public ActionResult Preview(string param)
        {
            if (param != null && param.Split('_').Length == 2)
            {
                string pKey = param.Split('_')[0];
                string sKey = param.Split('_')[1];

                DataTable dataBook = this.GetGalleryBookData(pKey, sKey);
                DataTable dataPic = this.GetGalleryBookPic(pKey, sKey);
                DataTable dataPreview = dataPic.Clone();

                if (dataBook.Rows.Count == 0)
                {
                    ViewData["MSG"] = "找不到資料";

                }
                else
                {
                    int picPage = int.Parse(dataBook.Rows[0]["PAGE"].ToString());

                    if (dataPic.Rows.Count > 0)
                    {
                        for (int i = 0; i < dataPic.Rows.Count; i += Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(dataPic.Rows.Count / 10.0))))
                        {
                            DataRow row = dataPic.Rows[i];

                            dataPreview.ImportRow(row);

                        }

                    }
                    
                    ViewData["BOOK"] = dataBook;
                    ViewData["PIC"] = dataPreview;

                }

            }
            else
            {
                ViewData["MSG"] = "找不到資料";

            }

            return View();

        }

    }
}