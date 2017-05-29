using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using System.Configuration;
using SyncProducts.Models;
using SyncProducts.Helpers;

namespace SyncProducts.Data
{
    public static class DataRepository
    {
        public static List<Website> GetWebsites() {
            Result result = new Result();
            DataProvider.GetDataTable(ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString, "select * from website", out result);
            List<Website> webs = MapWebsites.MapDBToWebsiteList((DataTable)result.ReturnObj);

            return webs;
        }

        public static List<SiteCategoryModel> GetSiteCategories(int websiteId, string supplierCategory)
        {
            Result result = new Result();
            DataProvider.GetDataTable(ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString, string.Format("select * from WebXSuppCatXSiteCat where WebsiteId={0} and SupplierCategoryPath='{1}'", websiteId, supplierCategory), out result);
            List<SiteCategoryModel> siteCats = MapCategories.MapDBToSiteCategoryList((DataTable)result.ReturnObj);

            return siteCats;
        }

        public static void RemoveSiteCategory(int WebCatId, out Result result)
        {
            result = new Result();

            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append(string.Format("delete WebXSuppCatXSiteCat where Id={0};delete CatXProduct where webCatId={0};", WebCatId));

            DataProvider.ExecuteQuery(ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString, sbSQL.ToString(), false, out result);
        }

        public static void AddSiteCategory(SiteCategoryModel model, out Result result) {
            result = new Result();

            //save website/supplier category/site category combo, and get id for combo
            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append(string.Format("insert into WebXSuppCatXSiteCat values({0},'{1}','{2}','{3}'); select @@identity;", model.WebsiteId, model.SupplierCategoryPath, model.SiteCategoryPath, model.SiteCategoryFilter));
            
            DataProvider.ExecuteQuery(ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString, sbSQL.ToString(), true, out result);
        }

        public static void UpdateSiteCategory(SiteCategoryModel model, out Result result)
        {
            result = new Result();

            //save website/supplier category/site category combo, and get id for combo
            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append(string.Format("update WebXSuppCatXSiteCat set SiteCategoryPath='{0}', SiteCategoryFilter='{1}' where Id={2}; select '{2}';", model.SiteCategoryPath, model.SiteCategoryFilter, model.WebCatId));

            DataProvider.ExecuteQuery(ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString, sbSQL.ToString(), true, out result);
        }

        public static void SaveProducts(SaveProductsModel model, out Result result) {
            result = new Result();

            UpdateSiteCategory(new SiteCategoryModel() {
                WebCatId=model.WebCatId,
                WebsiteId=model.WebsiteId,
                SupplierCategoryPath=model.SupplierCategory,
                SiteCategoryPath=model.SiteCategoryPath,
                SiteCategoryFilter=model.SiteCategoryFilter
            }, out result);
            
            //add products
            if (result.Success)
            {
                int webCatId = int.Parse(result.ReturnObj.ToString());
                result = new Result();

                StringBuilder sbSQL = new StringBuilder(string.Format("delete CatXProduct where WebCatId={0};", webCatId));
                foreach (string prodId in model.Products)
                {
                    sbSQL.Append(string.Format(" if not exists(select 1 from CatXProduct where WebCatId={0} and ProductId='{1}') begin insert into CatXProduct values({0}, '{1}'); end ", webCatId, prodId));
                }

                DataProvider.ExecuteQuery(ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString, sbSQL.ToString(), false, out result);
            }
        }

        public static List<Product> GetProductsByCategory(GetProductsByCategoryModel model)
        {
            Result result = new Result();
            string[] suppCatArr = model.SupplierCategoryPath.Split('~');
            string sql = string.Format("select row_number() over(order by pi.productName) as RowNum, pi.*, CatMember=(case when wc.Id is null then 0 else 1 end) from productImport pi left join CatXProduct cp on cp.ProductId = pi.ItemNum left join WebXSuppCatXSiteCat wc on wc.Id = cp.WebCatId and wc.WebsiteId={0} and wc.SupplierCategoryPath='{1}' and wc.SiteCategoryPath='{2}' and wc.SiteCategoryFilter='{3}' where category1='{4}' and category2='{5}' and category3='{6}' ", model.WebsiteId, model.SupplierCategoryPath, model.SiteCategoryPath, model.SiteCategoryFilter, suppCatArr[0].Trim(), suppCatArr[1].Trim(), suppCatArr[2].Trim(), model.WebsiteId);

            DataProvider.GetDataTable(ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString, sql, out result);
            List<Product> prods = MapProducts.MapDBToProdList((DataTable)result.ReturnObj);
            
            return prods;
        }

        public static List<Category> GetLocalCategories() {
            Result result = new Result();

            string sql = "select FullPath=Category1 + ' ~ ' + Category2 + ' ~ ' + Category3, ProductCount=count(ItemNum) from ProductImport where ltrim(rtrim(Category1))<>'' group by Category1, Category2, Category3 order by Category1, Category2, Category3";

            DataProvider.GetDataTable(ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString, sql, out result);
            List<Category> cats = MapCategories.MapDBToCatList((DataTable)result.ReturnObj);

            return cats;
        }

        public static void TruncateTable(string tblName) {
            Result resultClear = new Result();
            DataProvider.ExecuteQuery(ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString, string.Format("truncate table {0}", tblName), false, out resultClear);
        }

        public static void UpdateSiteMemberships(int webId, List<WebsiteMembership> mems) {

            //get website conn string
            Result resultSite = new Result();
            DataProvider.GetDataTable(ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString, string.Format("select * from website where id = {0}", webId), out resultSite);
            DataTable dtSite = (DataTable)resultSite.ReturnObj;
            string siteConnStr = string.Format("server={0};database={1};uid={2};pwd={3};", dtSite.Rows[0]["server"], dtSite.Rows[0]["database"], dtSite.Rows[0]["username"], dtSite.Rows[0]["password"]);
            
            //sync cats with online db
            List<string> newCat1s = mems.Select(c1 => c1.Category1).Distinct().ToList();
            string sqlCat1s = GetSQLToSyncCat1s(newCat1s);

            Result resultCat1 = new Result();
            DataProvider.ExecuteQuery(siteConnStr, sqlCat1s, false, out resultCat1);

            //cat memberships to local db
            string sqlLocWebMem = string.Format("");

        }

        private static string GetSQLToSyncCat1s(List<string> newCats) {
            StringBuilder sb = new StringBuilder("update category set deleted = 1;");

            //sql to add new cats
            foreach (string s in newCats) {
                sb.Append(string.Format("if not exists(select 1 from category where name='{0}' and ParentCategoryId=0) begin insert into category values('{0}', NULL, 1, NULL, NULL, NULL, 0, 1, 6, 1, '6, 3, 9', NULL, 0, 1, 0, 0, 1, 0, 1, getdate(), getdate()) end else begin update category set deleted=0 where Name='{0}' and ParentCategoryId=0 end ", s));
            }

            return sb.ToString();
        }
    }
}