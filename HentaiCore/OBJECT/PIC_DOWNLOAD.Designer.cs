namespace HentaiGetchu2 {
    partial class PIC_DOWNLOAD {
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
            this.picCover = new System.Windows.Forms.PictureBox();
            this.labTitle = new System.Windows.Forms.Label();
            this.labStatus = new System.Windows.Forms.Label();
            this.labNumPer = new System.Windows.Forms.Label();
            this.pro = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.picCover)).BeginInit();
            this.SuspendLayout();
            // 
            // picCover
            // 
            this.picCover.Dock = System.Windows.Forms.DockStyle.Left;
            this.picCover.Location = new System.Drawing.Point(0, 0);
            this.picCover.Name = "picCover";
            this.picCover.Size = new System.Drawing.Size(100, 100);
            this.picCover.TabIndex = 0;
            this.picCover.TabStop = false;
            // 
            // labTitle
            // 
            this.labTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labTitle.Font = new System.Drawing.Font("新細明體", 11F);
            this.labTitle.Location = new System.Drawing.Point(100, 0);
            this.labTitle.Name = "labTitle";
            this.labTitle.Size = new System.Drawing.Size(500, 40);
            this.labTitle.TabIndex = 1;
            this.labTitle.Text = "TITLE";
            this.labTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labStatus
            // 
            this.labStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.labStatus.Font = new System.Drawing.Font("新細明體", 11F);
            this.labStatus.Location = new System.Drawing.Point(100, 40);
            this.labStatus.Name = "labStatus";
            this.labStatus.Size = new System.Drawing.Size(500, 40);
            this.labStatus.TabIndex = 2;
            this.labStatus.Text = "STATUS";
            this.labStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labNumPer
            // 
            this.labNumPer.Dock = System.Windows.Forms.DockStyle.Right;
            this.labNumPer.Font = new System.Drawing.Font("新細明體", 11F);
            this.labNumPer.Location = new System.Drawing.Point(520, 80);
            this.labNumPer.Name = "labNumPer";
            this.labNumPer.Size = new System.Drawing.Size(80, 20);
            this.labNumPer.TabIndex = 3;
            this.labNumPer.Text = "%";
            this.labNumPer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pro
            // 
            this.pro.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pro.Location = new System.Drawing.Point(100, 80);
            this.pro.Name = "pro";
            this.pro.Size = new System.Drawing.Size(420, 20);
            this.pro.TabIndex = 4;
            // 
            // PIC_DOWNLOAD
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.pro);
            this.Controls.Add(this.labNumPer);
            this.Controls.Add(this.labStatus);
            this.Controls.Add(this.labTitle);
            this.Controls.Add(this.picCover);
            this.Name = "PIC_DOWNLOAD";
            this.Size = new System.Drawing.Size(600, 100);
            this.Load += new System.EventHandler(this.PIC_DOWNLOAD_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picCover)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picCover;
        private System.Windows.Forms.Label labTitle;
        private System.Windows.Forms.Label labStatus;
        private System.Windows.Forms.Label labNumPer;
        private System.Windows.Forms.ProgressBar pro;
    }
}
