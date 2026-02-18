using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewHentai.Controllers
{
    public partial class AdminController : Controller
    {
        public ActionResult Summary()
        {
            object obj = this.GetSummaryUpdateTime();

            DataTable data = null;

            data = this.GetObjectStatusData();

            DataTable dataCount = data.Copy();
            ViewData["ALL_CNT"] = dataCount.Select("CODE_ID = 'ALL'")[0]["CNT"];
            ViewData["OBJ_CNT"] = dataCount.Select("CODE_ID = 'OBJ'")[0]["CNT"];
            ViewData["EXHENTAI_CNT"] = dataCount.Select("CODE_ID = 'EXHENTAI'")[0]["CNT"];
            ViewData["ERROR_CNT"] = dataCount.Select("CODE_ID = 'ERROR'")[0]["CNT"];

            data = this.GetLangTypeStatusData();
            ViewData["TYPE_LANG_CNT"] = data.Copy();

            data = this.GetStatus("");
            ViewData["STATUS"] = data.Copy();

            return View();

        }
    }
}