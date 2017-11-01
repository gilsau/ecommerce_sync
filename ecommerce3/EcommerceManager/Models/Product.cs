using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceManager.Models
{
    public class SaveProductsModel {
        public int WebCatId { get; set; }
        public int WebsiteId { get; set; }
        public string SupplierCategory { get; set; }
        public string SiteCategoryPath { get; set; }
        public string SiteCategoryFilter { get; set; }
        public List<string> Products { get; set; }
    }

    public class SiteCategoryProducts
    {
        public string SiteCategory { get; set; }
        public string SiteCategoryFilter { get; set; }
        public List<string> Products { get; set; }
    }

    public class Product
    {
        public int RowNum { get; set; }
        public string ItemNum { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Wholesale { get; set; }
        public decimal Retail { get; set; }
        public int Qty { get; set; }
        public string ImageUrl { get; set; }
        public decimal Length { get; set; }
        public decimal Height { get; set; }
        public decimal Width { get; set; }
        public decimal Weight { get; set; }
        public string Category1 { get; set; }
        public string Category2 { get; set; }
        public string Category3 { get; set; }
        public bool CatMember { get; set; }
        public string SiteCategory { get; set; }
        public int ProductCount { get; set; }
    }
}