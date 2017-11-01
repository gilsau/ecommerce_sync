using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceManager.Models
{
	public class WebsiteCategory {
		public int WebId { get; set; }
		public string WebUrl { get; set; }
		public int ParentCatId { get; set; }
		public string ParentCat { get; set; }
		public int ChildCatId { get; set; }
		public string ChildCat { get; set; }
	}

	public class Category
    {
		public int Id { get; set; }
		public string Name { get; set; }
		public string FullPath { get; set; }
        public int ProductCount { get; set; }
    }

    public class GetProductsByCategoryModel
    {
        public int WebsiteId { get; set; }
        public string SupplierCategoryPath { get; set; }
        public string SiteCategoryPath { get; set; }
        public string SiteCategoryFilter { get; set; }
    }

    public class GetSiteCategoriesModel {
        public int WebsiteId { get; set; }
        public string SupplierCategoryPath { get; set; }
    }

    public class SiteCategoryModel
    {
        public int WebCatId { get; set; }
        public int WebsiteId { get; set; }
        public string SupplierCategoryPath { get; set; }
        public string SiteCategoryPath { get; set; }
        public string SiteCategoryFilter { get; set; }
    }
}