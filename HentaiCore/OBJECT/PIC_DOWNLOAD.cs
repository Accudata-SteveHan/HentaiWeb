using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using HentaiCore;

namespace HentaiGetchu2 {
    public partial class PIC_DOWNLOAD : UserControl {
        private delegate void Message(int progress, string msg);

        Message x;

        public PIC_BOOK book = null;
        public string savePath = "";

        public bool done = false;
        public Stream cover = null;
        public string title = "";
        public string status = "";
        public int progress = 0;

        private int maxProgress = 100;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        JOB_CONTROL downloadControl = null;

        public PIC_DOWNLOAD() {
            InitializeComponent();

        }

        public PIC_DOWNLOAD(PIC_BOOK book) {
            this.book = book;

            InitializeComponent();

        }

        private void ShowMessage(int p, string m) {
            this.progress = p;
            this.status = m;
        }

        private void PIC_DOWNLOAD_Load(object sender, EventArgs e) {
            this.timer.Interval = 1000;
            this.timer.Tick += new EventHandler(t_Tick);
            x += ShowMessage;
            

            this.title = book.bookInfo.uni_title != "" ? book.bookInfo.uni_title : book.bookInfo.eng_title;
            this.labNumPer.Text = string.Format("{0}%", this.progress);

            if (book.bookInfo.pic_Stream != null) {
                this.cover = book.bookInfo.pic_Stream;

            }

            this.timer.Start();

            Thread t = new Thread(new ThreadStart(DoDownload));
            t.Start();

        }

        private void DoDownload() {
            try {
                this.ShowMessage(1, "開始");
                //this.Invoke(x, new object[] { 1, "開始" });

                JOB_DL_LIST dlList = new JOB_DL_LIST(book.bookInfo);
                dlList.authUser = Central.authUser;
                //this.Invoke(x, new object[] { 20, "處理頁面分析" });
                this.ShowMessage(20, "處理頁面分析");
                dlList.ProcessDLList();
                //this.Invoke(x, new object[] { 40, "處理深度頁面分析" });
                this.ShowMessage(40, "處理深度頁面分析");
                dlList.ProcessDeepDLList();

                //this.Invoke(x, new object[] { 50, "開始下載" });
                this.ShowMessage(50, "開始下載");

                #region NEW
                List<PIC_INFO> picList = dlList.pic_info;

                this.downloadControl = new JOB_CONTROL();

                foreach (PIC_INFO picInfo in picList) {
                    string dirPath = Central.PathDirector.GetSaveDirName(
                        book.bookInfo.p_key, book.bookInfo.s_key, book.bookInfo.eng_title);

                    JOB_PIC jobPic = new JOB_PIC(picInfo);
                    jobPic.save_path = Path.Combine(savePath, dirPath);
                    jobPic.authUser = Central.authUser;

                    this.downloadControl.AddJob(jobPic);

                }

                Thread t = new Thread(new System.Threading.ThreadStart(this.downloadControl.DoLimitJob));
                t.Start();

                while (t.ThreadState != System.Threading.ThreadState.Stopped) {
                    Thread.Sleep(100);

                }

                #endregion NEW

            } catch (Exception ex) {
                Central.logger.WriteError(string.Format("PIC_DOWNLOAD.DoDownload 錯誤，{0}", ex.ToString()));

            }

        }

        private decimal CalcPicWait(decimal all, decimal process, decimal wait) {

            decimal per = 50 + ((all - wait - process) / all) * 50;

            return per;

        }


        private void t_Tick(object sender, EventArgs e)
        {
            if (this.downloadControl != null) {
                decimal all = Convert.ToDecimal(this.book.bookInfo.total_page);
                decimal wait = Convert.ToDecimal(this.downloadControl.jobList.Count);
                decimal process = Convert.ToDecimal(this.downloadControl.nowJob.Count);

                if (process == 0) {
                    if (wait > 0 && wait < all) {
                        this.progress = 50;
                        this.status = string.Format(
                            "下載準備中：{0}/{1}", wait, all);
                    } else if (wait == 0) {
                        this.progress = Convert.ToInt16(this.CalcPicWait(all, process, wait));
                        this.status = "下載完成";
                        this.timer.Stop();
                    }
                } else {
                    this.progress = Convert.ToInt16(this.CalcPicWait(all, process, wait));
                    this.status = string.Format(
                        "下載中：{0}/{1}",
                        all - (wait + process),
                        all);

                }

            }
            
            this.labTitle.Text = this.title;
            this.labStatus.Text = this.status;
            this.pro.Value = this.progress;
            this.labNumPer.Text = string.Format("{0}%", this.progress);
            if (this.cover != null) {
                this.picCover.Image = Image.FromStream(this.cover)
                    .GetThumbnailImage(this.picCover.Width, this.picCover.Height, null, IntPtr.Zero);
            }

            this.Refresh();

            if (progress >= maxProgress)
            {
                this.done = true;

                if (this.cover != null) {
                    this.cover.Close();
                    this.cover = null;

                }

                //this.timer.Stop();

            }

        }
        
    }

}
