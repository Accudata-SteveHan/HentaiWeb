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
        public ActionResult Page(string param)
        {
            int tmp = -1;
            if (param == null ||
                !int.TryParse(param.ToString(), out tmp))
            {
                tmp = 0;

            }

            if (tmp < 0)
            {
                tmp = 0;

            }

            DataTable dataMain = this.GetGalleryIndex();

            DataTable dataBook = this.GetGalleryPageData(tmp);

            ViewData["PAGE"] = param;
            ViewData["INDEX"] = dataMain;
            ViewData["DATA"] = dataBook;

            return View();

        }

    }
}