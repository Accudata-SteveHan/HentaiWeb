using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HentaiCore {
    public class THREAD_INFO {
        public string page_url = "";
        public string thread_url = "";
        public bool exhentai = false;

        public THREAD_INFO(string threadUrl) {
            this.thread_url = threadUrl;

        }

    }
}
