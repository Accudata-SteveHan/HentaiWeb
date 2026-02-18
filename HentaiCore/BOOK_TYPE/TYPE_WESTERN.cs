using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HentaiCore {
    public class TYPE_WESTERN : BOOK_TYPE {
        public TYPE_WESTERN() {
            this.nameSave = "WESTERN";
            this.nameDisplay = "WESTERN";
            this.color = Color.Green;
            this.Hantai_Active = true;
            this.HantaiEX_Active = true;

        }

    }

}
