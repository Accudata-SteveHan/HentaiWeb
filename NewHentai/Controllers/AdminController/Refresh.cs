using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewHentai.Controllers
{
    public partial class AdminController : Controller
    {
        public ActionResult Refresh()
        {
            JObject result = new JObject();

            object obj = this.GetSummaryUpdateTime();
            
            if (obj == null || (DateTime)obj < DateTime.Now.AddHours(-1))
            {
                this.ExexuteSummaryData();

                result.Add("RESULT", "OK");
            }
            else
            {
                result.Add("RESULT","時間未達解凍時間");

            }

            ViewData["RESULT"] = result.ToString();

            return View();

        }
    }
}