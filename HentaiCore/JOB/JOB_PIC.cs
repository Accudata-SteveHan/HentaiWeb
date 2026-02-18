using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HtmlAgilityPack;
using System.Drawing;
using ImageMagick;

namespace HentaiCore {
    //圖片的工作
    public class JOB_PIC {
        PIC_INFO pic_info = null;
        public string save_path = "";

        // --- JOB (original) fields moved here ---
        public AUTH authUser = null;
        public string domain = "";
        HTML web = null;

        protected Logger logger = null;

        int retry = 3;

        public JOB_PIC(PIC_INFO picInfo) {
            this.pic_info = picInfo;

            // initialize web & logger (original JOB ctor)
            this.web = new HTML();
            this.logger = Central.logger;
        }

        private void InitWebConfig(string url) {
            web.webUrl = url;
            var au = this.authUser ?? Central.authUser;
            if (au != null) {
                web.webCookie = au.loginCookie;
                web.cookieContainer = au.loginContainer;
            }
        }

        protected string GetWeb(string url){
            int i = 0;
            if (this.logger != null) this.logger.Write(this, string.Format("開始 JOB GetWeb = {0}" , url));
            for (i = 0 ; i < retry ; i++) {
                if (this.logger != null) this.logger.WriteTrace(string.Format("GetWeb : {0} URL = {1}", i, url));
                this.InitWebConfig(url);
                web.GetHtml();

                if (web.webHtml != null && web.webHtml.ToLower().Contains("html")) {
                    break;
                }

                if (this.logger != null) this.logger.WriteTrace(string.Format("GetWeb Fail : {0} URL = {1}", i, url));
            }

            if (this.logger != null) this.logger.Write(this, string.Format("完成 JOB GetWeb = {0}", url));
            return web.webHtml;
        }

        protected Stream GetStream(string url) {
            int i = 0;
            if (this.logger != null) this.logger.Write(this, string.Format("開始 JOB GetStream = {0}", url));

            for (i = 0 ; i < retry ; i++) {
                try {
                    this.InitWebConfig(url);
                    web.GetStream();
                    break;
                } catch (Exception ex) {
                    if (this.logger != null) this.logger.WriteTrace(string.Format("GetStream Fail : {0} URL = {1}", i, url));
                }
            }

            if (this.logger != null) this.logger.Write(this, string.Format("完成 JOB GetStream = {0}", url));
            return web.webStream;
        }

        public void ProcessDownloadPic() {
            try {
                if (this.logger != null) this.logger.WriteInfo(string.Format("開始下載圖片 : {0}", this.pic_info.picUrl));
                HtmlAgilityPack.HtmlDocument doc = null;
                HtmlNode rootNode = null;
                string html = "";
                string xpath = "";

                //分析圖片頁面
                if (this.logger != null) this.logger.Write(this, string.Format("分析下載圖片頁面 : {0}", this.pic_info.picUrl));
                html = this.GetWeb(this.pic_info.picUrl);
                doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);
                rootNode = doc.DocumentNode;

                xpath = "/html[1]/body[1]/div[1]/div[2]/a[1]/img[1]";

                HtmlNode imgNode = rootNode.SelectSingleNode(xpath);
                string img = imgNode.Attributes["src"].Value;

                //取得 img 資料
                if (this.logger != null) this.logger.Write(this, string.Format("下載圖片頁面 : {0} 下載圖片:{1}", this.pic_info.picUrl, img));
                using (Stream imgStream = this.GetStream(img)) {
                    if (imgStream == null) {
                        return;
                    }

                    try {
                        //儲存檔案
                        using (Stream stream = imgStream) {
                            if (!Directory.Exists(save_path)) {
                                Directory.CreateDirectory(save_path);
                            }

                            stream.Seek(0,SeekOrigin.Begin);

                            using (var magickImage = new MagickImage(stream))
                            {
                                // 將 MagickImage 寫入記憶體中的 JPG Stream
                                using (var jpgStream = new MemoryStream())
                                {
                                    magickImage.Format = MagickFormat.Jpg; // 轉換格式
                                    magickImage.Write(jpgStream);          // 寫入 JPG

                                    jpgStream.Position = 0;                // 重設指標

                                    Image i = Image.FromStream(jpgStream);    // 建立 System.Drawing.Image
                                    i.Save(Path.Combine(save_path, string.Format("{0}.jpg", pic_info.picFileName)));
                                    i.Dispose();
                                }
                            }

                            this.pic_info.success = true;
                        }

                        if (this.logger != null) this.logger.WriteInfo(string.Format("完成下載圖片 : {0}", this.pic_info.picUrl));

                    } catch (Exception ex) {
                        throw ex;
                    }

                }
            } catch (Exception ex) {
                if (this.logger != null) this.logger.WriteError(string.Format("JOB_PIC.ProcessDownloadPic 錯誤，{0}", ex.ToString()));
            }

        }

        public void ProcessGetPic() {
            try {
                if (this.logger != null) this.logger.WriteInfo(string.Format("開始下載圖片 : {0}", this.pic_info.picUrl));
                HtmlAgilityPack.HtmlDocument doc = null;
                HtmlNode rootNode = null;
                string html = "";
                string xpath = "";

                //分析圖片頁面
                if (this.logger != null) this.logger.Write(this, string.Format("分析擷取圖片頁面 : {0}", this.pic_info.picUrl));
                html = this.GetWeb(this.pic_info.picUrl);
                doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);
                rootNode = doc.DocumentNode;

                xpath = "/html[1]/body[1]/div[1]/div[2]/a[1]/img[1]";

                HtmlNode imgNode = rootNode.SelectSingleNode(xpath);
                string img = imgNode.Attributes["src"].Value;

                //取得 img 資料
                if (this.logger != null) this.logger.Write(this, string.Format("擷取圖片頁面 : {0} 擷取圖片:{1}", this.pic_info.picUrl, img));

                pic_info.picImgUrl = img;

            } catch (Exception ex) {
                if (this.logger != null) this.logger.WriteError(string.Format("JOB_PIC.ProcessDownloadPic 錯誤，{0}", ex.ToString()));
            }

        }

    }

}