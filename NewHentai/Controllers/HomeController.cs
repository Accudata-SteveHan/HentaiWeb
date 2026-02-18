using HentaiCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewHentai.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string key)
        {
            if (key == null || key == "")
            {
                key = "-";

            }

            ViewData["KEY"] = key;

            return View();

        }

    }

}
