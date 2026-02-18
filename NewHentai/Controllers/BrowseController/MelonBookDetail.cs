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
        public ActionResult MelonBookDetail(string type, string lang, string page)
        {
            DataSet dsSet = new DataSet();
            DataTable dataBook = this.GetMelonData(page);
            DataTable dataDetail = this.GetMelonBookMapData(type, lang, page);

            DataTable dataTag = this.GetMelonTagData(page, type, lang);
            DataTable dataTopRef = this.GetMelonTagBookMappingTopData(page, type, lang);

            foreach (DataRow row in dataDetail.Rows)
            {
                DataTable data = GetGalleryBookPic(row["PKEY"].ToString(), row["SKEY"].ToString());
                data.TableName = "DATA";

                dsSet.Merge(data);

            }

            ViewData["BOOK"] = dataBook;
            ViewData["DETAIL"] = dataDetail;
            ViewData["PIC"] = dsSet.Tables["DATA"];

            ViewData["TAG"] = dataTag;
            ViewData["TOP"] = dataTopRef;

            ViewData["TYPE"] = type;
            ViewData["LANG"] = lang;

            return View();

        }

    }

}