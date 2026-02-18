using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HentaiCore {
    public class TYPE_IMAGE_SET : BOOK_TYPE {
        public TYPE_IMAGE_SET() {
            this.nameSave = "IMAGESET";
            this.nameDisplay = "IMAGE_SET";
            this.color = Color.BlueViolet;
            this.Hantai_Active = true;
            this.HantaiEX_Active = true;

        }

    }

}
