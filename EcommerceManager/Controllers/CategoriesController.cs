using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommerceManager.Models;
using EcommerceManager.Data;

namespace EcommerceManager.Controllers
{
    public class CategoriesController : Controller
    {
		[HttpGet]
        public ActionResult Index()
        {
			return View();
        }

		[HttpGet]
		public JsonResult GetWebsites() {
			return Json(DataRepository.GetWebsites(), JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult GetParentCats(int webId)
		{
			List<WebsiteCategory> webCats = DataRepository.GetWebsiteCategories();
			var parentCats = webCats.Where(wc => wc.WebId == webId).Select(wc => new { Id = wc.ParentCatId, Name = wc.ParentCat }).Distinct().ToList();

			return Json(parentCats, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult GetChildCats(int webId, int parentCatId)
		{
			List<WebsiteCategory> webCats = DataRepository.GetWebsiteCategories();
			List<Category> childCats = webCats.Where(wc => wc.WebId == webId && wc.ParentCatId == parentCatId).Select(wc => new Category() { Id = wc.ChildCatId, Name = wc.ChildCat }).ToList();

			return Json(childCats, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult GetProducts(int webId, int childCatId)
		{
			List<SiteProduct> prods = DataRepository.GetProductsForWebsiteByCategory(webId, childCatId);
			return Json(prods, JsonRequestBehavior.AllowGet);
		}
	}
}