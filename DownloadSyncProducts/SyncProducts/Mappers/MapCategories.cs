using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using SyncProducts.Models;

namespace SyncProducts.Helpers
{
    public static class MapCategories
    {
        public static List<Category> MapDBToCatList(DataTable dt) {
            List<Category> cats = new List<Category>();
            foreach(DataRow dr in dt.Rows)
            {
                Category cat = new Category();
                cat.FullPath = dr[0].ToString();
                cat.ProductCount = int.Parse(dr[1].ToString());
                cats.Add(cat);
            }
            
            return cats;
        }

        public static List<SiteCategoryModel> MapDBToSiteCategoryList(DataTable dt)
        {
            List<SiteCategoryModel> cats = new List<SiteCategoryModel>();
            foreach (DataRow dr in dt.Rows)
            {
                SiteCategoryModel cat = new SiteCategoryModel();
                cat.WebCatId = int.Parse(dr[0].ToString());
                cat.WebsiteId = int.Parse(dr[1].ToString());
                cat.SupplierCategoryPath = dr[2].ToString();
                cat.SiteCategoryPath = dr[3].ToString();
                cat.SiteCategoryFilter = dr[4].ToString();
                cats.Add(cat);
            }

            return cats;
        }
    }
}