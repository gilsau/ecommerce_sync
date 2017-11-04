using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using EcommerceManager.Models;

namespace EcommerceManager.Helpers
{
    public static class MapWebsites
    {
        public static List<Website> MapDBToWebsiteList(DataTable dt)
        {
            List<Website> webs = new List<Website>();
            foreach (DataRow dr in dt.Rows)
            {
                Website web = new Website();
                web.Id = int.Parse(dr[0].ToString());
                web.Name = dr[1].ToString();
                web.Url = dr[2].ToString();
                web.Abbrev = dr[3].ToString();
                web.Server = dr[4].ToString();
                web.Database = dr[5].ToString();
                web.Username = dr[6].ToString();
                web.Password = dr[7].ToString();

                if (dr.ItemArray.Count() > 9)
                {
                    web.CategoryMappings = new List<CategoryMapping>() {
                        new CategoryMapping(){
                            SupplierCategory = dr[8].ToString(),
                            WebsiteCategory = dr[9].ToString()
                        }
                    };
                }

                webs.Add(web);
            }

            return webs;
        }
    }
}