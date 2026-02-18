using HentaiCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace NewHentai.Controllers
{
    public class TelegramController : Controller
    {
        TelegramBotClient botClient = null;

        string token = "1370863941:AAHDfz-yI-YmXz8S_lcz7OOYDHi4Idk4atY";
        string chat = "-1001456278460";

        public TelegramController()
        {
            if (botClient == null)
            {
                botClient = new TelegramBotClient(token);

            }
            
        }
        
        private string SendMessage(string text)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            
            Message x = botClient.SendTextMessageAsync(
                    chatId: chat,
                    text: text,
                    replyToMessageId: 0
                ).Result;

            return x.Text;

        }

        private string GetWebData(string data)
        {
            string baseUrl = ConfigurationManager.AppSettings["BasePage"];
            string navUrl = baseUrl + "/" + data;

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            HttpWebRequest req = HttpWebRequest.Create(navUrl) as HttpWebRequest;
            using (HttpWebResponse res = req.GetResponse() as HttpWebResponse)
            {
                using (StreamReader reader = new StreamReader(res.GetResponseStream()))
                {
                    DateTime timeEnd = DateTime.Now;
                    string result = reader.ReadToEnd().Replace("\r\n", "");
                    
                    doc.LoadHtml(result);

                }

            }

            return doc.DocumentNode.InnerText;

        }

        // GET: Telegram
        public ActionResult Index()
        {
            try
            {
                string text = "Hello Bot";

                string result = this.SendMessage(text);

                ViewData["DATA"] = result;

            }
            catch (Exception ex)
            {
                ViewData["DATA"] = ex.ToString();

            }

            return View();

        }

        public ActionResult Talk(string param)
        {
            try
            {
                string reponse = "";

                if (param == null)
                {
                    param = "";

                }

                if (param == "")
                {
                    this.SendMessage("我沒看到你打東西");

                }

                string result = "";

                switch (param.ToUpper())
                {
                    case "SUMMARY":
                        result = this.GetWebData("HOME/Summary");
                        break;
                    default:
                        result = this.GetWebData(param);

                        break;

                }

                this.SendMessage(result);

            }
            catch (Exception ex)
            {
                ViewData["DATA"] = ex.ToString();

            }

            return View();

        }
        
    }

}