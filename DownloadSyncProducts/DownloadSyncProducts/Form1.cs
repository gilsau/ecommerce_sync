using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows.Forms;
using DownloadSyncProducts.Download;
using DownloadSyncProducts.Helpers;

namespace DownloadSyncProducts
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();

            //set element sizes
            txtFeedbackDwn.Width = tcMain.Width / 2;
        }

        private void btnDownloadAll_Click(object sender, EventArgs e)
        {
            Downloader dr = new Downloader();
            
            Result dwnResult = new Result();
            dr.Download(out dwnResult, txtFeedbackDwn);

            Result unzipResult = new Result();
            dr.UnzipAllFiles(out unzipResult, txtFeedbackDwn);
            dr.DeleteZipFiles();

            Result dbResult = new Result();
            dr.SaveFilesToDB(out dbResult, txtFeedbackDwn);

            if (dwnResult.Success && unzipResult.Success)
            {
                Feedback.WriteLine("Successfully Completed!", txtFeedbackDwn);
            }
            else
            {
                string err = string.Format("Error!{0}{1}{2}{3}", Environment.NewLine, dwnResult.ErrForLog, Environment.NewLine, unzipResult.ErrForLog);
                Feedback.WriteLine(err, txtFeedbackDwn);
            }
        }
    }
}
