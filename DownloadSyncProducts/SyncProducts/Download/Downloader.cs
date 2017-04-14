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
using SyncProducts.Helpers;

namespace SyncProducts.Download
{
    public class Downloader
    {
        public List<string> UrlsForDownload { get; set; }
        private int _UrlType { get; }
        private string _FileSavePath { get; }
        private List<FileInfo> _ZipFiles { get; set; }
        private List<FileInfo> _CSVJPGFiles { get; set; }

        //constructor
        public Downloader(int urlType) {
            _UrlType = urlType;
            if (urlType == 1)
            {
                _FileSavePath = string.Format("{0}\\{1}", HttpContext.Current.Server.MapPath("~\\DownloadedFiles"), "\\Products");
            }
            else if (urlType == 2)
            {
                _FileSavePath = string.Format("{0}\\{1}", HttpContext.Current.Server.MapPath("~\\DownloadedFiles"), "\\Images");
            }
        }
        public Downloader(string url, int urlType) {
            _ZipFiles = new List<FileInfo>();
            _CSVJPGFiles = new List<FileInfo>();
            UrlsForDownload = new List<string>() { url };

            _UrlType = urlType;
            if (urlType == 1)
            {
                _FileSavePath = string.Format("{0}\\{1}", HttpContext.Current.Server.MapPath("~\\DownloadedFiles"), "\\Products");
            }
            else if (urlType == 2)
            {
                _FileSavePath = string.Format("{0}\\{1}", HttpContext.Current.Server.MapPath("~\\DownloadedFiles"), "\\Images");
            }
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
                
                //loop thru url to download
                WebClient wc = new WebClient();
                foreach (string url in UrlsForDownload)
                {
                    string fileName = string.Format("{0:yyyy_MM_dd_HH_mm_ss_fff}.zip", DateTime.Now);
                    string fullFilePath = string.Format("{0}\\{1}", _FileSavePath, fileName);

                    //download file from url
                    wc.DownloadFile(new Uri(url), fullFilePath);

                    //add file to global list of files downloaded
                    _ZipFiles.Add(new FileInfo(fullFilePath));

                    result.ErrForUser = string.Format("{0}<hr class='nomarg pad5' />File was downloaded successfully!<br/>{1}", result.ErrForUser, fullFilePath);
                }

                Result resultUnzip = new Result();
                UnzipAllFiles(out resultUnzip);
                DeleteZipFiles();

                //check operation status
                if (resultUnzip.Success)
                {
                    result.Success = true;
                    result.ErrForUser = string.Format("{0}<hr class='nomarg pad5' />{1}", result.ErrForUser, resultUnzip.ErrForUser);
                    result.ReturnObj = _CSVJPGFiles[_CSVJPGFiles.Count - 1].FullName;
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
                    if (_UrlType == 1)
                    {
                        zipFullFileName = file.FullName;
                        endFullFileName = file.FullName.ToLower().Replace(".zip", ".csv");

                        using (ZipArchive archive = ZipFile.OpenRead(zipFullFileName))
                        {
                            foreach (ZipArchiveEntry entry in archive.Entries)
                            {
                                if (entry.FullName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                                {
                                    entry.ExtractToFile(endFullFileName, true);
                                }
                            }
                        }
                        result.ErrForUser = string.Format("{0}<hr class='nomarg pad5' />File was unzipped successfully!<br/>{1}", result.ErrForUser, endFullFileName);
                        _CSVJPGFiles.Add(new FileInfo(endFullFileName));
                    }
                    else if(_UrlType == 2) {
                        ZipFile.ExtractToDirectory(file.FullName, _FileSavePath);
                    }
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
