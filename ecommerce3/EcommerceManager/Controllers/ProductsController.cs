using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using EcommerceManager.Helpers;
using EcommerceManager.Models;
using EcommerceManager.Data;
using EcommerceManager.Download;

namespace EcommerceManager.Controllers
{
    public class ProductsController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Websites = DataRepository.GetWebsites();
            ViewBag.Categories = DataRepository.GetLocalCategories();

            return View();
        }

        [HttpGet]
        public JsonResult GetCats()
        {
            List<Category> cats = DataRepository.GetLocalCategories();
            return Json(cats, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetWebsites()
        {
            List<Website> webs = DataRepository.GetWebsites();
            return Json(webs, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetProducts(string category, int pageSize, int? page)
        {
            int pg = page == null ? 1 : int.Parse(page.ToString());
            category = HttpUtility.UrlDecode(category);

            List<Product> prods = DataRepository.GetProductsByCategory(category, pageSize, pg);
            int prodCnt = prods.Count() > 0 ? prods[0].ProductCount : 0;

            var result = Json(new ProdSearchBag() { Products = prods, Found = prodCnt }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;

            return result;
        }

        [HttpGet]
        public JsonResult ProductSearch(string keyword, int pageSize, int? page)
        {
            int pg = page == null ? 1 : int.Parse(page.ToString());
            keyword = HttpUtility.UrlDecode(keyword);

            List<Product> prods = DataRepository.GetProductsByKeyword(keyword, pageSize, pg);
            int prodCnt = prods.Count() > 0 ? prods[0].ProductCount : 0;

            var result = Json(new ProdSearchBag() { Products = prods, Found = prodCnt }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;

            return result;
        }

        [HttpGet]
        public JsonResult GetProductsForSite(int webId, string category, int pageSize, int? page)
        {
            int pg = page == null ? 1 : int.Parse(page.ToString());
            category = HttpUtility.UrlDecode(category);

            //List 1 - get local products for selected supplier category
            List<Product> prods1 = DataRepository.GetProductsByCategory(category, pageSize, pg);
            int prodCnt = prods1.Count() > 0 ? prods1[0].ProductCount : 0;

            //List 2 - get website products, in the corresponding category on site, that match the local product skus found in List 1
            List<Product> prods2 = DataRepository.GetProductsFromWebsiteByCategory(webId, category, prods1.Select(p1 => p1.ItemNum).ToList());

            //add products to List 2, from List 1 that don't match List 2
            List<Product> prodsNonWeb = prods1.Where(p => !(prods2.Any(p2 => p2.ItemNum == p.ItemNum))).ToList();
            List<Product> prods = prods2.Union(prodsNonWeb).ToList();

            //Website Category with list of products
            ProdSearchBag prodSearchBag = new ProdSearchBag();
            prodSearchBag.Products = prods;
            prodSearchBag.Found = prodCnt;
            prodSearchBag.WebsiteCategory = DataRepository.GetWebsiteCategory(webId, category);

            //JSON result sent back to client-side
            var result = Json(prodSearchBag, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;

            return result;
        }

        [HttpPost]
        public JsonResult CategoryPersist(PushToSiteModel model)
        {
            Result result = new Result();
            DataRepository.CategoryPersist(model, out result);
            return Json(result);
        }

        [HttpPost]
        public JsonResult ProductsPersist(PushToSiteModel model)
        {
            Result result = new Result();
            DataRepository.ProductsPersist(model, out result);
            return Json(result);
        }

        [HttpGet]
        public JsonResult ClearImgDir()
        {
            Result result = new Result();
            DataRepository.ClearImgDir(out result);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DownloadImages(PushToSiteModel model)
        {
            Result result = new Result();
            DataRepository.DownloadImages(model, out result);
            return Json(result);
        }

        [HttpPost]
        public JsonResult AddImagesToDB(PushToSiteModel model)
        {
            Result result = new Result();
            //DataRepository.AddImagesToDB(model, out result);
            return Json(result);
        }

    }

    public class ProdSearchBag
    {
        public List<Product> Products { get; set; }
        public int Found { get; set; }
        public string WebsiteCategory { get; set; }
    }
}