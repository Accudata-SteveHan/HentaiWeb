using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HentaiCore {
    public class TYPE_GAME_CG : BOOK_TYPE {
        public TYPE_GAME_CG() {
            this.nameSave = "GAMECG";
            this.nameDisplay = "GAME_CG";
            this.color = Color.GreenYellow;
            this.Hantai_Active = true;
            this.HantaiEX_Active = true;

        }

    }

}
