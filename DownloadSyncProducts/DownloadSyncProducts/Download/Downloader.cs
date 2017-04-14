using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Configuration;
using System.IO.Compression;
using System.Windows.Forms;
using DownloadSyncProducts.Helpers;

namespace DownloadSyncProducts.Download
{
    public class Downloader
    {
        public string FileSavePath { get; }
        public List<string> UrlsForDownload { get; set; }
        private List<FileInfo> _ZipFiles { get; set; }
        private List<FileInfo> _CSVFiles { get; set; }

        //constructor
        public Downloader() {
            _ZipFiles = new List<FileInfo>();
            _CSVFiles = new List<FileInfo>();
            UrlsForDownload = ConfigurationManager.AppSettings["DownloadProductUrls"].Split(',').Select(u => u.Trim()).ToList();
            FileSavePath = ConfigurationManager.AppSettings["FileSavePath"];
        }

        public void Download(out Result result, TextBox txtBox) {
            result = new Result();

            try
            {
                //make sure directory exists
                Feedback.WriteLine(string.Format("Verifying Directory...{0}{1}", Environment.NewLine, FileSavePath), txtBox);
                if (!Directory.Exists(FileSavePath))
                {
                    Directory.CreateDirectory(FileSavePath);
                }
                ClearDirectory(FileSavePath);

                //loop thru url to download
                WebClient wc = new WebClient();
                foreach (string url in UrlsForDownload)
                {
                    string fileName = string.Format("{0:yyyy_MM_dd_HH_mm_ss_fff}.zip", DateTime.Now);
                    string fullFilePath = string.Format("{0}\\{1}", FileSavePath, fileName);

                    //download file from url
                    Feedback.WriteLine(string.Format("Downloading...{0}{1}", Environment.NewLine, url), txtBox);
                    wc.DownloadFile(new Uri(url), fullFilePath);

                    //add file to global list of files downloaded
                    Feedback.WriteLine(string.Format("File Downloaded...{0}{1}", Environment.NewLine, fullFilePath), txtBox);
                    _ZipFiles.Add(new FileInfo(fullFilePath));

                }
                result.Success = true;
            }
            catch (Exception ex) {
                result.ErrForUser = ex.Message;
                result.ErrForLog = string.Format("{0}{1}{2}{3}{4}{5}{6}", ex.Message, Environment.NewLine, Environment.NewLine, ex.InnerException, Environment.NewLine, Environment.NewLine, ex.StackTrace);
            }
        }

        public void UnzipAllFiles(out Result result, TextBox txtBox) {
            result = new Result();

            //make sure directory exists
            if (!Directory.Exists(FileSavePath))
            {
                Directory.CreateDirectory(FileSavePath);
            }

            //unzip files
            string zipFullFileName = string.Empty;
            string csvFullFileName = string.Empty;

            try
            {
                foreach (FileInfo file in _ZipFiles)
                {
                    zipFullFileName = file.FullName;
                    csvFullFileName = file.FullName.ToLower().Replace(".zip", ".csv");

                    using (ZipArchive archive = ZipFile.OpenRead(zipFullFileName))
                    {
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {   
                            if (entry.FullName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                            {
                                entry.ExtractToFile(csvFullFileName, true);
                            }
                        }
                    }

                    Feedback.WriteLine(string.Format("Unzipped CSV File...{0}{1}", Environment.NewLine, csvFullFileName), txtBox);
                    _CSVFiles.Add(new FileInfo(csvFullFileName));
                }
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.ErrForUser = ex.Message;
                result.ErrForLog = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}", Environment.NewLine, zipFullFileName, Environment.NewLine, csvFullFileName, ex.Message, Environment.NewLine, Environment.NewLine, ex.InnerException, Environment.NewLine, Environment.NewLine, ex.StackTrace);
            }
        }

        public void DeleteZipFiles() {
            foreach (FileInfo file in _ZipFiles) {
                file.Delete();
            }
        }

        public void SaveFilesToDB(out Result result, TextBox txtBox)
        {
            result = new Result();


            //open files


            //add to local database, table products, table categories


        }

        private void ClearDirectory(string dirPath) {
            foreach (FileInfo file in new DirectoryInfo(dirPath).GetFiles()) {
                file.Delete();
            }
        }
    }
}
