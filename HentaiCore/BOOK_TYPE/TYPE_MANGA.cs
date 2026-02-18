using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HentaiCore {
    public class TYPE_MANGA : BOOK_TYPE {
        public TYPE_MANGA() {
            this.nameSave = "MANGA";
            this.nameDisplay = "MANGA";
            this.color = Color.OrangeRed;
            this.Hantai_Active = true;
            this.HantaiEX_Active = true;

        }

    }
}
