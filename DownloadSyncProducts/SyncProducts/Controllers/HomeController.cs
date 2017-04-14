using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using CsvHelper;
using CsvHelper.Configuration;
using SyncProducts.Helpers;
using SyncProducts.Models;
using SyncProducts.Data;
using SyncProducts.Download;

namespace SyncProducts.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            List<Website> webs = DataRepository.GetWebsites();
            return View(webs);
        }

        [HttpGet]
        public JsonResult GetProducts(Category cat)
        {
            List<Product> prods = DataRepository.GetLocalProducts(cat);
            return Json(prods, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCats()
        {
            List<Category> cats = DataRepository.GetLocalCategories();
            return Json(cats, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateWebMemsAndProdCounts(List<WebsiteMembership> webMems) {

            //apply memberships to sites
            List<int> siteIds = webMems.Select(s => s.WebId).Distinct().ToList();
            foreach (int s in siteIds) {
                DataRepository.UpdateSiteMemberships(s, webMems.Where(sm => sm.WebId == s && sm.IsMember).ToList());
            }

            return GetCats();
        }

        [HttpGet]
        public JsonResult TruncateProductImportTable()
        {
            Result result = new Result();
            DataRepository.TruncateTable("productimport");
            result.Success = true;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SaveProductsFromFileToDB(string filePath)
        {
            Result result = new Result();
            string[] filePathArr = new string[] { };
            DataTable dtCSVData = new DataTable();
            
            //read CSV file
            using (var reader = new CsvHelper.CsvReader(new StreamReader(filePath)))
            {
                //add columns to data table
                reader.ReadHeader();
                for (int c = 0; c < reader.FieldHeaders.Count(); c++)
                {
                    dtCSVData.Columns.Add();
                }
                
                //add rows to data table
                while (reader.Read())
                {
                    dtCSVData.Rows.Add(reader.CurrentRecord);
                }
            }

            //insert data into db table
            using (SqlConnection dbConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString))
            {
                dbConnection.Open();
                using (SqlBulkCopy s = new SqlBulkCopy(dbConnection))
                {
                    s.BatchSize = 5000;
                    s.DestinationTableName = "ProductImport";

                    try
                    {
                        s.WriteToServer(dtCSVData);
                    }
                    catch { }
                }
            }

            filePathArr = filePath.Split('\\');
            result.Success = true;
            result.ErrForUser = string.Format("<hr class='nomarg nopad'/>File: {0}<br/>Products Added: {1}", filePathArr[filePathArr.Length-1], dtCSVData.Rows.Count);
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ClearFileSaveDir(int urlType)
        {
            Result resultClear = new Result();

            Downloader downloader = new Downloader(urlType);
            downloader.ClearDirectory(out resultClear);

            return Json(resultClear);
        }

        [HttpPost]
        public JsonResult DownloadFileFromUrl(string url, int urlType) {
            Result resultDownload = new Result();

            Downloader downloader = new Downloader(url, urlType);
            downloader.Download(out resultDownload);

            return Json(resultDownload, JsonRequestBehavior.AllowGet);
        }
    }
}