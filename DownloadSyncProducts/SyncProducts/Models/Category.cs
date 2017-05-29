using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SyncProducts.Models
{
    public class Category
    {
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