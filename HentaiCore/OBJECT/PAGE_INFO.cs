using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HentaiCore {
    public class PAGE_INFO {
        public string page_url;
        public string site_url;
        public int page_threadCount = 0;

        public PAGE_INFO(string pageUrl) {
            this.page_url = pageUrl;

        }

    }
}
