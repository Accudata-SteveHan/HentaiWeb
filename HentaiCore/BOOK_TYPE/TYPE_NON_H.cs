using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HentaiCore {
    public class TYPE_NON_H : BOOK_TYPE {
        public TYPE_NON_H() {
            this.nameSave = "NON-H";
            this.nameDisplay = "NON_H";
            this.color = Color.Blue;
            this.Hantai_Active = true;
            this.HantaiEX_Active = true;

        }

    }
}
