using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SyncProducts.Models
{
    public class Category
    {
        public int RowNum { get; set; }
        public List<WebsiteMembership> WebMemberships { get; set; }
        public string Category1 { get; set; }
        public string Category2 { get; set; }
        public string Category3 { get; set; }
        public int ProductCount { get; set; }
    }

    public class SiteCategory {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ParentCategoryId { get; set; }
    }
}