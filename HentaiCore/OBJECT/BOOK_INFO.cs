using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Data;
using System.Net;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.IO;

namespace HentaiCore {
    public class BOOK_INFO {
        public string pageUrl = "";
        public string threadUrl = "";
        public string type;
        public string upload_time;
        public string posted_time;
        public string pic;
        public string eng_title;
        public string uni_title;
        public decimal total_page;
        public string p_key;
        public string s_key;
        public string language;
        public decimal rate_time;
        public decimal rate_avg;
        public decimal tab_count;
        public Stream pic_Stream;

        //public BOOK_TYPE typeObj = null;

        public BOOK_INFO() {

        }

        public BOOK_INFO(string thread) {
            this.threadUrl = thread;
        }

        public BOOK_INFO(string thread , string pKey , string sKey) {
            this.threadUrl = thread;
            this.p_key = pKey;
            this.s_key = sKey;

        }

    }

}
