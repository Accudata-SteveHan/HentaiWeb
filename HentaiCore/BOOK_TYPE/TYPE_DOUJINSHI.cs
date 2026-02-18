using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HentaiCore {
    public class TYPE_DOUJINSHI : BOOK_TYPE {
        public TYPE_DOUJINSHI() {
            this.nameSave = "DOUJINSHI";
            this.nameDisplay = "DOUJINSHI";
            this.color = Color.Red;
            this.Hantai_Active = true;
            this.HantaiEX_Active = true;

        }

    }

}
