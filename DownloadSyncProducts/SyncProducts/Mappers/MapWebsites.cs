using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using SyncProducts.Models;

namespace SyncProducts.Helpers
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

                webs.Add(web);
            }

            return webs;
        }
    }
}