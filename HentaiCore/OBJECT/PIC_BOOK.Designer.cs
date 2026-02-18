namespace HentaiGetchu2 {
    partial class PIC_BOOK {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.pic = new System.Windows.Forms.PictureBox();
            this.lbPage = new System.Windows.Forms.Label();
            this.lbLanguage = new System.Windows.Forms.Label();
            this.lbRate = new System.Windows.Forms.Label();
            this.picCountry = new System.Windows.Forms.PictureBox();
            this.lbSel = new System.Windows.Forms.Label();
            this.picToolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCountry)).BeginInit();
            this.SuspendLayout();
            // 
            // pic
            // 
            this.pic.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pic.Location = new System.Drawing.Point(3, 3);
            this.pic.Margin = new System.Windows.Forms.Padding(0);
            this.pic.Name = "pic";
            this.pic.Size = new System.Drawing.Size(174, 214);
            this.pic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic.TabIndex = 0;
            this.pic.TabStop = false;
            this.pic.Click += new System.EventHandler(this.pic_Click);
            // 
            // lbPage
            // 
            this.lbPage.Font = new System.Drawing.Font("微軟正黑體", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbPage.ForeColor = System.Drawing.Color.Red;
            this.lbPage.Location = new System.Drawing.Point(102, 189);
            this.lbPage.Margin = new System.Windows.Forms.Padding(5);
            this.lbPage.Name = "lbPage";
            this.lbPage.Size = new System.Drawing.Size(70, 23);
            this.lbPage.TabIndex = 1;
            this.lbPage.Text = "999 P";
            this.lbPage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbLanguage
            // 
            this.lbLanguage.Font = new System.Drawing.Font("細明體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbLanguage.ForeColor = System.Drawing.Color.Blue;
            this.lbLanguage.Location = new System.Drawing.Point(72, 8);
            this.lbLanguage.Margin = new System.Windows.Forms.Padding(5);
            this.lbLanguage.Name = "lbLanguage";
            this.lbLanguage.Size = new System.Drawing.Size(100, 23);
            this.lbLanguage.TabIndex = 2;
            this.lbLanguage.Text = "label1";
            this.lbLanguage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbRate
            // 
            this.lbRate.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbRate.Location = new System.Drawing.Point(8, 192);
            this.lbRate.Margin = new System.Windows.Forms.Padding(5);
            this.lbRate.Name = "lbRate";
            this.lbRate.Size = new System.Drawing.Size(73, 20);
            this.lbRate.TabIndex = 3;
            this.lbRate.Text = "5.00/1000";
            this.lbRate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // picCountry
            // 
            this.picCountry.Location = new System.Drawing.Point(122, 8);
            this.picCountry.Margin = new System.Windows.Forms.Padding(5);
            this.picCountry.Name = "picCountry";
            this.picCountry.Size = new System.Drawing.Size(50, 30);
            this.picCountry.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picCountry.TabIndex = 4;
            this.picCountry.TabStop = false;
            // 
            // lbSel
            // 
            this.lbSel.Font = new System.Drawing.Font("新細明體", 36F);
            this.lbSel.Location = new System.Drawing.Point(5, 82);
            this.lbSel.Name = "lbSel";
            this.lbSel.Size = new System.Drawing.Size(172, 49);
            this.lbSel.TabIndex = 5;
            this.lbSel.Text = "已選擇";
            this.lbSel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PIC_BOOK
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.picCountry);
            this.Controls.Add(this.lbLanguage);
            this.Controls.Add(this.lbPage);
            this.Controls.Add(this.lbRate);
            this.Controls.Add(this.lbSel);
            this.Controls.Add(this.pic);
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "PIC_BOOK";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(180, 220);
            this.Load += new System.EventHandler(this.PIC_BOOK_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCountry)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pic;
        private System.Windows.Forms.Label lbPage;
        private System.Windows.Forms.Label lbLanguage;
        private System.Windows.Forms.Label lbRate;
        private System.Windows.Forms.PictureBox picCountry;
        private System.Windows.Forms.Label lbSel;
        private System.Windows.Forms.ToolTip picToolTip;
    }
}
