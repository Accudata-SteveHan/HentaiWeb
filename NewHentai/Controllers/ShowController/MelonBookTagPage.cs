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
        public ActionResult MelonBookTagPage(string key, string tag, string type, string lang, string page, string param)
        {
            try
            {
                if (key == "-")
                {
                    ViewData["DATA"] = new DataTable();
                    ViewData["TYPE"] = type;
                    ViewData["LANG"] = lang;
                    ViewData["KEY"] = key;

                    return View();

                }

                DataTable dataMain = this.GetMelonBookTagMappingData(tag, type, lang, page, param);
                DataTable dataResult = dataMain.Clone();

                int totalSkip = 0;
                int skip = 0, count = 0;

                if (param == "")
                {
                    if (!int.TryParse(page, out totalSkip) && totalSkip < 0)
                    {
                        totalSkip = 0;

                    }
                    else
                    {
                        totalSkip *= maxDisplayMelonBook;

                    }

                }

                List<string> listMelonID = new List<string>();

                int tmp;
                foreach (DataRow row in dataMain.Rows)
                {
                    if (skip != totalSkip)
                    {
                        skip++;
                        continue;

                    }

                    listMelonID.Add(string.Format("'{0}'", row["ID"]));
                    dataResult.ImportRow(row);
                    count++;

                    if (count == maxDisplayMelonBook)
                    {
                        break;

                    }

                }

                if (int.TryParse(param, out tmp) && tmp < 0)
                {
                    DataTable dataReverse = dataResult.Copy();
                    dataResult.Rows.Clear();

                    for (int i = dataReverse.Rows.Count - 1; i >= 0; i--)
                    {
                        dataResult.ImportRow(dataReverse.Rows[i]);

                    }

                }

                //DataTable dataDetail = this.GetMelonBookMapDetailData(type, lang, listMelonID);

                ViewData["DATA"] = dataResult;
                //ViewData["DETAIL"] = dataDetail;

                ViewData["KEY"] = key;
                ViewData["TYPE"] = type;
                ViewData["LANG"] = lang;

            }
            catch (Exception ex)
            {
                ViewData["MSG"] = ex.ToString();

            }

            return View();

        }

    }

}
