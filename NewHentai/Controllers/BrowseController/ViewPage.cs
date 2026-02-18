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
        public ActionResult ViewPage(string type, string lang, string page, string param)
        {
            DataTable dataMain = this.GetGalleryDetail(type, lang, page, param);
            DataTable dataResult = dataMain.Clone();

            int totalSkip = 0;
            int skip = 0, count = 0;


            if (param == "")
            {
                if (!int.TryParse(page, out totalSkip) && totalSkip < 0)
                {
                    totalSkip = 0;

                }
                else
                {
                    totalSkip *= maxDisplayExhentai;

                }

            }

            int tmp;
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

            if (int.TryParse(param, out tmp) && tmp < 0)
            {
                DataTable dataReverse = dataResult.Copy();
                dataResult.Rows.Clear();

                for (int i = dataReverse.Rows.Count - 1; i >= 0; i--)
                {
                    dataResult.ImportRow(dataReverse.Rows[i]);

                }

            }

            //DataTable dataBook = this.GetGalleryPageData(tmp);

            ViewData["DATA"] = dataResult;
            //ViewData["DATA"] = dataBook;

            return View();

        }

    }
}