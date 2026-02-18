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
        public ActionResult Table(string param)
        {
            int maxRow = -1;

            ViewData["TYPE"] = param;

            DataTable dataMain = null;
            DataTable dataBook = null;
            DataTable dataWeb = null;
            DataTable dataObject = null;

            if (param == "" || param == "INDEX")
            {
                dataMain = this.GetGalleryIndex();

            }

            if (param == "" || param == "DATA")
            {
                dataBook = this.GetGalleryData();

            }

            if (param == "" || param == "WEB")
            {
                dataWeb = this.GetGalleryWeb();

            }

            if (param == "" || param == "OBJECT")
            {
                dataObject = this.GetGalleryObject();

            }
            
            ViewData["INDEX"] = dataMain;
            ViewData["DATA"] = dataBook;
            ViewData["WEB"] = dataWeb;
            ViewData["OBJECT"] = dataObject;

            return View();

        }

    }

}