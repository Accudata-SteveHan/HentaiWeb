using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HentaiCore {
    public class TYPE_ARTIST_CG : BOOK_TYPE {
        public TYPE_ARTIST_CG() {
            this.nameSave = "ARTISTCG";
            this.nameDisplay = "ARTIST_CG";
            this.color = Color.Orange;
            this.Hantai_Active = true;
            this.HantaiEX_Active = true;

        }

    }

}
