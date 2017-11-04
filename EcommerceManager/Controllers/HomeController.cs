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
using EcommerceManager.Helpers;
using EcommerceManager.Models;
using EcommerceManager.Data;
using EcommerceManager.Download;

namespace EcommerceManager.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetDownloadedFiles()
        {
            List<string> files = DataRepository.GetDownloadedFiles();
            return Json(files, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult TruncateTable(string table)
        {
            Result result = new Result();
            DataRepository.TruncateTable(table, out result);
            result.Success = true;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SaveProductsFromFileToDB(string filePath)
        {
            Result result = new Result();
            DataRepository.ImportFileToDB(filePath, out result);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ClearFileSaveDir()
        {
            Result resultClear = new Result();

            Downloader downloader = new Downloader();
            downloader.ClearDirectory(out resultClear);

            return Json(resultClear, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DownloadFileFromUrl(string url, string logo)
        {
            Result resultDownload = new Result();

            Downloader downloader = new Downloader(url, logo);
            downloader.Download(out resultDownload);

            return Json(resultDownload, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult PopulateProductImportTable()
        {
            Result result = new Result();
            DataRepository.PopulateProductImportTable(out result);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CreateCategoriesTable()
        {
            Result result = new Result();
            DataRepository.CreateCategoriesTable(out result);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}