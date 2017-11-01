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
			List<Category> parentCats = webCats.Select(wc => new Category() { Id = wc.ParentCatId, Name = wc.ParentCat }).ToList();

			return Json(parentCats, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult GetChildCats(int webId, int parentCatId)
		{
			List<WebsiteCategory> webCats = DataRepository.GetWebsiteCategories();
			List<Category> childCats = webCats.Select(wc => new Category() { Id = wc.ChildCatId, Name = wc.ChildCat }).ToList();

			return Json(childCats, JsonRequestBehavior.AllowGet);
		}
	}
}