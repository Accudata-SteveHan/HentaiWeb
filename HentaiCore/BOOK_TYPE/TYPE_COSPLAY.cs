using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HentaiCore {
    public class TYPE_COSPLAY : BOOK_TYPE {
        public TYPE_COSPLAY() {
            this.nameSave = "COSPLAY";
            this.nameDisplay = "COSPLAY";
            this.color = Color.Brown;
            this.Hantai_Active = true;
            this.HantaiEX_Active = true;

        }

    }

}
