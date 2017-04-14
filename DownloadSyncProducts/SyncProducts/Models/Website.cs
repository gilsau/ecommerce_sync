using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SyncProducts.Models
{
    public class Website
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Abbrev { get; set; }
    }

    public class WebsiteMembership
    {
        public int WebId { get; set; }
        public bool IsMember { get; set; }
        public int ProductCount { get; set; }
        public string ProductsUrl { get; set; }
        public string Category1 { get; set; }
        public string Category2 { get; set; }
        public string Category3 { get; set; }
    }
}