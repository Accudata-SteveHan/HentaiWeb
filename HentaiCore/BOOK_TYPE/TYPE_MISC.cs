using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HentaiCore {
    public class TYPE_MISC : BOOK_TYPE {
        public TYPE_MISC() {
            this.nameSave = "MISC";
            this.nameDisplay = "MISC";
            this.color = Color.DarkGray;
            this.Hantai_Active = true;
            this.HantaiEX_Active = true;

        }

    }
}
