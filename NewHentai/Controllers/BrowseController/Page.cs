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
        public ActionResult Page(string type, string lang, string page, string param)
        {
            DataTable dataMain = this.GetGalleryDetail(type, lang, page, param);
            DataTable dataResult = dataMain.Clone();

            int totalSkip;
            int skip = 0, count = 0;

            if (!int.TryParse(page, out totalSkip) && totalSkip < 0)
            {
                totalSkip = 0;

            }
            else
            {
                totalSkip *= maxDisplayExhentai;

            }

            foreach (DataRow row in dataMain.Rows)
            {
                if (skip != totalSkip)
                {
                    skip++;
                    continue;

                }


                dataResult.ImportRow(row);
                count++;

                if (count == maxDisplayExhentai)
                {
                    break;

                }

            }

            //DataTable dataBook = this.GetGalleryPageData(tmp);

            ViewData["DATA"] = dataResult;
            //ViewData["DATA"] = dataBook;

            return View();

        }

    }
}