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

        public static List<Product> GetLocalProducts(Category cat)
        {
            Result result = new Result();

            //TODO: add WEBSITES in select list
            string sql = string.Format("select ROW_NUMBER() over (order by Category1, Category2, Category3) as RowNum, * from productimport where category1 = '{0}' and category2 = '{1}' and category3 = '{2}' order by Category1, Category2, Category3", cat.Category1, cat.Category2, cat.Category3);

            DataProvider.GetDataTable(ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString, sql, out result);
            List<Product> prods = MapProducts.MapDBToProdList((DataTable)result.ReturnObj);

            return prods;
        }

        public static List<Category> GetLocalCategories() {
            Result result = new Result();

            //TODO: add WEBSITES in select list
            string sql = "select ROW_NUMBER() over (order by Category1, Category2, Category3) as RowNum, Websites='1|true|300|http://www.funmodernelectronics.com,2|false|500|http://www.hisandherscloset.com,3|true|700|http://www.homeandgardenstatues.com,4|false|900|http://www.outdoorsfungear.com,5|true|1100|http://www.toysandgamesroom.com', Category1, Category2, Category3, Products=COUNT(productname) from productimport where ltrim(rtrim(suppliercategory1)) <> '' group by Category1, Category2, Category3 order by Category1, Category2, Category3 ";

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