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
        public ActionResult Display(string type, string lang, string page, string param)
        {
            if (page == "ALL")
            {
                page = "0";
            }

            DataTable dataMain = this.GetGalleryDisplay(type, lang, page, param);

            ViewData["TYPE"] = type;
            ViewData["LANG"] = lang;
            ViewData["PAGE"] = page;
            ViewData["PARAM"] = param;

            ViewData["MAX"] = dataMain.Rows.Count > 0 ?
                int.Parse(dataMain.Compute("MAX(PKEY)", "").ToString()) : 1;
            ViewData["MIN"] = dataMain.Rows.Count > 0 ?
                int.Parse(dataMain.Compute("MIN(PKEY)", "").ToString()) : 0;

            return View();

        }

    }
}