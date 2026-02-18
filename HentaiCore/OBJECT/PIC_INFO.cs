using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace HentaiCore {
    public class PIC_INFO {
        public bool success = false;
        public string threadUrl = "";
        public string picNo = "";
        public string picFileName = "";
        public string picUrl = "";
        public Stream picStream = null;
        public Image picImage = null;
        public string picImgUrl = "";

    }
}
