using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HentaiCore {
    public class URL {
        public string http = "";
        public string host = "";
        public Dictionary<string, string> param = null;

        public URL() {
            this.param = new Dictionary<string, string>();

        }

        public string GetUrl() {
            string urlStr = "{0}://{1}{2}";
            string paramStr = "?{0}";

            string query = "";
            foreach (string p in param.Keys) {
                query += (query != "" ? "&" : "") + string.Format("{0}={1}", p, param[p]);

            }

            if (query != "") {
                paramStr = string.Format(paramStr, query);
            } else {
                paramStr = "";
            }

            return string.Format(urlStr, this.http, this.host, paramStr);

        }

    }

}
