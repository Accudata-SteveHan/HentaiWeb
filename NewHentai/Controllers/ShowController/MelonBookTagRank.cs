using HentaiCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewHentai.Controllers
{
    public partial class ShowController : Controller
    {
        public ActionResult MelonBookTagRank(string key, string type, string lang, string page, string param)
        {
            if (page == "ALL")
            {
                page = "0";
            }

            DataTable dataMain = this.GetMelonBookTagMapSummaryData(type, lang, "");

            ViewData["DATA"] = dataMain;
            ViewData["TYPE"] = type;
            ViewData["LANG"] = lang;
            ViewData["PAGE"] = page;
            ViewData["PARAM"] = param;
            ViewData["KEY"] = key;

            return View();

        }

        public ActionResult MelonBookTagRankReady(string key, string type, string lang, string page, string param)
        {
            if (page == "ALL")
            {
                page = "0";
            }

            DataTable dataMain = this.GetMelonBookTagMapSummaryData(type, lang, "Y");

            ViewData["DATA"] = dataMain;
            ViewData["TYPE"] = type;
            ViewData["LANG"] = lang;
            ViewData["PAGE"] = page;
            ViewData["PARAM"] = param;
            ViewData["KEY"] = key;

            return View();

        }

    }

}
