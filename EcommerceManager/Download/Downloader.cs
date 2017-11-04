using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Web;
using System.Configuration;
using System.IO.Compression;
using EcommerceManager.Helpers;

namespace EcommerceManager.Download
{
    public class Downloader
    {
        public string UrlForDownload { get; set; }
        public string LogoForDownload { get; set; }
        private string _FileSavePath { get;set; }
        private List<FileInfo> _ZipFiles { get; set; }
        private List<FileInfo> _CSVJPGFiles { get; set; }
        private string lastCSVFile { get; set; }

        //constructor
        public Downloader() {
            _FileSavePath = string.Format("{0}\\{1}", HttpContext.Current.Server.MapPath("~\\DownloadedFiles"), "\\Products");
        }
        public Downloader(string url, string logo) {
            _ZipFiles = new List<FileInfo>();
            _CSVJPGFiles = new List<FileInfo>();
            UrlForDownload = url;
            LogoForDownload = logo;

            _FileSavePath = string.Format("{0}\\Products", HttpContext.Current.Server.MapPath("/DownloadedFiles/"));
        }

        public void Download(out Result result) {
            result = new Result();

            try
            {
                //make sure directory exists
                if (!Directory.Exists(_FileSavePath))
                {
                    Directory.CreateDirectory(_FileSavePath);
                }
                
                WebClient wc = new WebClient();
                
                string company = string.IsNullOrEmpty(LogoForDownload) ? "null" : LogoForDownload;
                string fileName = string.Format("{0:yyyy_MM_dd_HH_mm_ss_fff}_{1}.zip", DateTime.Now, company);
                string fullFilePath = string.Format("{0}\\{1}", _FileSavePath, fileName);

                //download file from url
                wc.DownloadFile(new Uri(UrlForDownload), fullFilePath);
                var fileType = wc.ResponseHeaders["Content-Type"];

                //add zip files to global collection, to unzip later
                if (fileType.IndexOf("zip") > -1)
                {
                    _ZipFiles.Add(new FileInfo(fullFilePath));
                }

                //if not a zip file, it should be a txt file, rename it to csv
                else {
                    string newCSVFile = fullFilePath.Replace("zip", "csv");
                    File.Move(fullFilePath, newCSVFile);
                    lastCSVFile = newCSVFile;
                }

                result.ErrForUser = string.Format("{0}<hr class='nomarg pad5' />File was downloaded successfully!<br/>{1}", result.ErrForUser, fullFilePath);
                
                Result resultUnzip = new Result();
                UnzipAllFiles(out resultUnzip);
                DeleteZipFiles();

                //check operation status
                if (resultUnzip.Success)
                {
                    result.Success = true;
                    result.ErrForUser = string.Format("{0}<hr class='nomarg pad5' />{1}", result.ErrForUser, resultUnzip.ErrForUser);
                    result.ReturnObj = lastCSVFile;
                }
                else {
                    result.Success = false;
                    result.ErrForUser = string.Format("{0}<hr/>{1}", result.ErrForUser, resultUnzip.ErrForUser);
                    result.ErrForLog = string.Format("{0}<hr/>{1}", result.ErrForLog, resultUnzip.ErrForLog);
                }
            }
            catch (Exception ex) {
                result.Success = false;
                result.ErrForUser = ex.Message;
                result.ErrForLog = string.Format("{0}{1}{2}{3}{4}{5}{6}", ex.Message, "<br/>", "<br/>", ex.InnerException, "<br/>", "<br/>", ex.StackTrace);
            }
        }

        private void UnzipAllFiles(out Result result) {
            result = new Result();

            //make sure directory exists
            if (!Directory.Exists(_FileSavePath))
            {
                Directory.CreateDirectory(_FileSavePath);
            }

            //unzip files
            string zipFullFileName = string.Empty;
            string endFullFileName = string.Empty;

            try
            {
                foreach (FileInfo file in _ZipFiles)
                {
                        zipFullFileName = file.FullName;
                        endFullFileName = file.FullName.ToLower().Replace(".zip", ".csv");

                        using (ZipArchive archive = ZipFile.OpenRead(zipFullFileName))
                        {
                            foreach (ZipArchiveEntry entry in archive.Entries)
                            {
                                if (entry.FullName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase) || entry.FullName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                                {
                                    entry.ExtractToFile(endFullFileName, true);
                                    lastCSVFile = endFullFileName;
                                }
                            }
                        }
                        result.ErrForUser = string.Format("{0}<hr class='nomarg pad5' />File was unzipped successfully!<br/>{1}", result.ErrForUser, endFullFileName);
                        _CSVJPGFiles.Add(new FileInfo(endFullFileName));
                    
                }


                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrForUser = ex.Message;
                result.ErrForLog = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}", "<br/>", zipFullFileName, "<br/>", endFullFileName, ex.Message, "<br/>", "<br/>", ex.InnerException, "<br/>", "<br/>", ex.StackTrace);
            }
        }

        private void DeleteZipFiles() {
            foreach (FileInfo file in _ZipFiles) {
                file.Delete();
            }
        }

        public void ClearDirectory(out Result result) {
            result = new Result();

            try
            {
                foreach (FileInfo file in new DirectoryInfo(_FileSavePath).GetFiles())
                {
                    file.Delete();
                }

                result.Success = true;
                result.ErrForUser = string.Format("Directory: {0} was cleared!", _FileSavePath);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrForUser = ex.Message;
                result.ErrForLog = string.Format("{0}{1}{2}{3}{4}{5}{6}", ex.Message, "<br/>", "<br/>", ex.InnerException, "<br/>", "<br/>", ex.StackTrace);
            }
        }
    }
}
