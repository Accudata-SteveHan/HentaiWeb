using HentaiCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;

namespace NewHentai.Controllers
{
    public partial class GalleryController : Controller
    {
        public ActionResult Query()
        {
            DataTable data = this.GetGalleryDetail("ALL", "ALL", "ALL");

            ViewData["DATA"] = data;

            return View();

        }

    }
}