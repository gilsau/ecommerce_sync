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
                cat.RowNum = int.Parse(dr[0].ToString());
                cat.Category1 = dr[2].ToString();
                cat.Category2 = dr[3].ToString();
                cat.Category3 = dr[4].ToString();
                cat.ProductCount = int.Parse(dr[5].ToString());

                //add website memberships
                cat.WebMemberships = new List<WebsiteMembership>();
                foreach (string webInfo in dr[1].ToString().Split(','))
                {
                    string[] arrWebMem = webInfo.Split('|');
                    cat.WebMemberships.Add(new WebsiteMembership() { WebId = int.Parse(arrWebMem[0]), IsMember = bool.Parse(arrWebMem[1]), ProductCount = int.Parse(arrWebMem[2]), ProductsUrl = arrWebMem[3], Category1 = cat.Category1, Category2 = cat.Category2, Category3 = cat.Category3, });
                }

                cats.Add(cat);
            }
            
            return cats;
        }
    }
}