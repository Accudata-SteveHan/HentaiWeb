using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using HentaiCore;

namespace HentaiGetchu2 {
    public partial class PIC_BOOK : UserControl {
        public BOOK_INFO bookInfo = null;

        public string rateNum = "";
        public string rateTime = "";
        public int totalPage = -1;
        public string language = "";
        public Image image = null;

        public PIC_BOOK() {
            InitializeComponent();
        }

        public void BookLoad() {
            this.Name = bookInfo.p_key;
            this.language = bookInfo.language;
            this.rateNum = bookInfo.rate_avg.ToString();
            this.rateTime = bookInfo.rate_time.ToString();
            this.totalPage = int.Parse(bookInfo.total_page.ToString());

            if (bookInfo.pic_Stream != null) {
                this.image = Image.FromStream(bookInfo.pic_Stream);

            }

        }

        private void pic_Click(object sender, EventArgs e) {
            PictureBox p = sender as PictureBox;

            if ((p.Parent as UserControl).BorderStyle == BorderStyle.FixedSingle) {
                (p.Parent as UserControl).BorderStyle = BorderStyle.Fixed3D;
                this.lbSel.Visible = true;

            } else if ((p.Parent as UserControl).BorderStyle == BorderStyle.Fixed3D) {
                (p.Parent as UserControl).BorderStyle = BorderStyle.FixedSingle;
                this.lbSel.Visible = false;

            }

        }

        private void PIC_BOOK_Load(object sender, EventArgs e) {
            if (image == null) {
                string notFound = string.Format("{0}/IMG/NotFound.jpg", Environment.CurrentDirectory);
                image = Image.FromFile(notFound);

            }

            this.pic.Image = image.GetThumbnailImage(this.pic.Width, this.pic.Height, null, IntPtr.Zero);
            this.BackColor = this.bookInfo.typeObj != null ? this.bookInfo.typeObj.color : SystemColors.Control;
            
            string countryFile = string.Format(
                "{0}/IMG/Country/{1}.png" , 
                Environment.CurrentDirectory , this.language.ToLower());

            if (File.Exists(countryFile)){
                this.picCountry.Visible = true;
                this.lbLanguage.Visible = false;
            } else {
                this.picCountry.Visible = false;
                this.lbLanguage.Visible = true;
            }

            if (this.lbLanguage.Visible == true) {
                this.lbLanguage.Parent = this.pic;
                this.lbLanguage.Text = this.language;
                this.lbLanguage.BackColor = Color.Transparent;
            } else {
                this.picCountry.Parent = this.pic;
                this.picCountry.Image = Image.FromFile(countryFile);
                this.picCountry.BackColor = Color.Transparent;
            }
            
            this.lbPage.Parent = this.pic;
            this.lbPage.Text = string.Format("{0} P", this.totalPage);
            this.lbPage.BackColor = Color.Transparent;
            this.lbRate.Parent = this.pic;
            this.lbRate.Text = string.Format("{0}/{1}", this.rateNum, rateTime);
            this.lbRate.BackColor = Color.Transparent;

            this.lbSel.Parent = this.pic;
            this.lbSel.Visible = false;
            this.lbSel.BackColor = Color.Transparent;

            ToolTip t = new ToolTip();
            if (this.bookInfo.uni_title.Trim() != "") {
                t.SetToolTip(this.pic, this.bookInfo.uni_title);
            } else {
                t.SetToolTip(this.pic, this.bookInfo.eng_title);
            }

        }

    }
}
