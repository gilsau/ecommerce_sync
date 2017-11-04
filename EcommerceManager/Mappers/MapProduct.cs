using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using EcommerceManager.Models;

namespace EcommerceManager.Helpers
{
    public static class MapProducts
    {
        public static List<Product> MapDBToProdList(DataTable dt)
        {
            List<Product> prods = new List<Product>();
            foreach (DataRow dr in dt.Rows)
            {
                Product prod = new Product();
                prod.RowNum = int.Parse(dr[0].ToString());
                prod.ItemNum = dr[1].ToString();
                prod.Name = dr[2].ToString();
                prod.Description = dr[3].ToString();
                prod.Wholesale = decimal.Parse(dr[4].ToString());
                prod.Retail = decimal.Parse(dr[5].ToString());
                prod.Qty = int.Parse(dr[6].ToString());
                prod.ImageUrl = dr[7].ToString();
                prod.Length = decimal.Parse(dr[8].ToString());
                prod.Height = decimal.Parse(dr[9].ToString());
                prod.Width = decimal.Parse(dr[10].ToString());
                prod.Weight = decimal.Parse(dr[11].ToString());
                prod.Category1 = dr[12].ToString();
                prod.Category2 = dr[13].ToString();
                prod.Category3 = dr[14].ToString();
                prod.CatMember = bool.Parse((dr[15].ToString() == "0" ? "false" : "true"));
                prod.SiteCategory = dr[16].ToString();
                prod.ProductCount = int.Parse(dr[17].ToString());
                prods.Add(prod);
            }
            
            return prods;
        }

		public static List<SiteProduct> MapDBToSiteProdList(DataTable dt)
		{
			List<SiteProduct> prods = new List<SiteProduct>();
			foreach (DataRow dr in dt.Rows)
			{
				SiteProduct prod = new SiteProduct();
				prod.Sku = dr[0].ToString();
				prod.ImgUrl = dr[1].ToString();
				prod.Name = dr[2].ToString();
				prod.Description = dr[3].ToString();
				prod.Categories = dr[4].ToString();
				prods.Add(prod);
			}

			return prods;
		}
	}
}