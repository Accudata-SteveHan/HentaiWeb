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
        public ActionResult MapMelonBook(string key, string type, string lang, string page, string param)
        {
            if (page == "ALL")
            {
                page = "0";
            }

            DataTable dataMain = this.GetMelonBookMapItemData("MAP", type, lang, page, param);

            ViewData["TYPE"] = type;
            ViewData["LANG"] = lang;
            ViewData["PAGE"] = page;
            ViewData["PARAM"] = param;

            ViewData["KEY"] = key;

            ViewData["MAX"] = dataMain.Rows.Count > 0 ?
                int.Parse(dataMain.Compute("MAX(ID)", "").ToString()) : Convert.ToInt32(Math.Pow(10, 9));
            ViewData["MIN"] = dataMain.Rows.Count > 0 ?
                int.Parse(dataMain.Compute("MIN(ID)", "").ToString()) : 0;

            return View();

        }

    }
}