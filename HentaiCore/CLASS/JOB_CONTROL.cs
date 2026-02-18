using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HentaiCore {
    public class JOB_CONTROL {
        public Queue<JOB> jobList = new Queue<JOB>();
        public List<Thread> nowJob = null;
        private int limit = 3;

        public void DoJob() {
            this.nowJob = new List<Thread>();

            while (this.jobList.Count > 0) {
                JOB job = this.jobList.Dequeue();

                this.Create_JOB_WEB(job);
                this.Create_JOB_PAGE(job);
                this.Create_JOB_THREAD(job);
                this.Create_JOB_DL_LIST(job);
                this.Create_JOB_PIC(job);

            }

            this.MoniJob();

        }

        public void DoLimitJob() {
            this.nowJob = new List<Thread>();

            while (this.jobList.Count > 0) {
                if (this.nowJob.Count <= limit) {
                    JOB job = this.jobList.Dequeue();

                    this.Create_JOB_WEB(job);
                    this.Create_JOB_PAGE(job);
                    this.Create_JOB_THREAD(job);
                    this.Create_JOB_DL_LIST(job);
                    this.Create_JOB_PIC(job);

                }

                this.MoniLimitJob();

                Thread.Sleep(50);

            }

            this.MoniJob();

        }

        private void MoniLimitJob() {
            for (int i = this.nowJob.Count - 1 ; i >= 0 ; i--) {
                if (this.nowJob[i].ThreadState == ThreadState.Stopped) {
                    this.nowJob.RemoveAt(i);

                }

            }

        }

        public void AddJob(JOB job) {
            jobList.Enqueue(job);

        }

        private void MoniJob() {
            while (this.nowJob.Count > 0) {
                try {
                    for (int i = this.nowJob.Count - 1 ; i >= 0 ; i--) {
                        if (i <= this.nowJob.Count - 1 &&
                            this.nowJob[i].ThreadState == ThreadState.Stopped) {
                            this.nowJob.RemoveAt(i);

                        }

                    }

                } catch (Exception ex) {

                }

                Thread.Sleep(50);

            }

        }

        #region Create
        private void Create_JOB_WEB(JOB job) {
            JOB_WEB j = job as JOB_WEB;
            if (j == null) return;

            Thread newT = new Thread(j.ProcessSite);
            nowJob.Add(newT);

            newT.Start();

        }

        private void Create_JOB_PAGE(JOB job) {
            JOB_PAGE j = job as JOB_PAGE;
            if (j == null) return;

            Thread newT = new Thread(j.GetThreadList);
            nowJob.Add(newT);

            newT.Start();

        }

        private void Create_JOB_THREAD(JOB job) {
            JOB_THREAD j = job as JOB_THREAD;
            if (j == null) return;

            Thread newT = new Thread(j.GetBookInfo);
            nowJob.Add(newT);

            newT.Start();

        }

        private void Create_JOB_DL_LIST(JOB job) {
            JOB_DL_LIST j = job as JOB_DL_LIST;
            if (j == null) return;

            Thread newT = new Thread(j.ProcessDeepDLList);
            nowJob.Add(newT);

            newT.Start();

        }

        private void Create_JOB_PIC(JOB job) {
            JOB_PIC j = job as JOB_PIC;
            if (j == null) return;

            Thread newT = new Thread(j.ProcessDownloadPic);
            nowJob.Add(newT);

            newT.Start();

        }

        #endregion Create

    }

}
