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
        public ActionResult MelonBookTag(string tag, string type, string lang, string page, string param)
        {
            if (page == "ALL")
            {
                page = "0";
            }

            DataTable dataMain = this.GetMelonBookTagMappingData(tag, type, lang, page, param);

            ViewData["TAG_ID"] = tag;
            ViewData["TYPE"] = type;
            ViewData["LANG"] = lang;
            ViewData["PAGE"] = page;
            ViewData["PARAM"] = param;

            ViewData["MAX"] = dataMain.Rows.Count > 0 ?
                int.Parse(dataMain.Compute("MAX(ID)", "").ToString()) : 1;
            ViewData["MIN"] = dataMain.Rows.Count > 0 ?
                int.Parse(dataMain.Compute("MIN(ID)", "").ToString()) : 0;

            return View();

        }

    }

}
