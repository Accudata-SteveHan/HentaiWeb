using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HentaiCore {
    public class SITE_INFO {
        public string host = "";
        public string param = "";

        public string site_url = "";
        public int page_info_count = 0;

        public SITE_INFO(string site) {
            this.site_url = site;

            Uri u = new Uri(site);
            this.host = u.Host;
            //this.param = u.Query.Substring(1);


        }

    }
}
