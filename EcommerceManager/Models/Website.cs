using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceManager.Models
{
    public class Website
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Abbrev { get; set; }
        public string Server { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<CategoryMapping> CategoryMappings { get; set; }
    }

    public class CategoryMapping {
        public string SupplierCategory { get; set; }
        public string WebsiteCategory { get; set; }
    }
}