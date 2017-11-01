using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceManager.Models
{
    public class PushToSiteModel
    {
        public List<string> productNums { get; set; }
        public int webId { get; set; }
        public string supplierCat { get; set; }
        public string siteCat { get; set; }
    }
}