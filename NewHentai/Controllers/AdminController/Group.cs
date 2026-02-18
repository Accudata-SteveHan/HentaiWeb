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
        public ActionResult Group()
        {
            object obj = this.GetSummaryUpdateTime();

            DataTable data = this.GetGroupTypeData();
            ViewData["TYPE_CNT"] = data.Copy();

            return View();

        }
    }
}