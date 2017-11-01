using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceManager.Models
{
	public class WebsiteCategoryPost
	{
		public int WebId { get; set; }
		public string WebUrl { get; set; }
		public int ParentCatId { get; set; }
		public string ParentCat { get; set; }
		public int ChildCatId { get; set; }
		public string ChildCat { get; set; }
		public int WebIdTgt { get; set; }
		public string WebTitleTgt { get; set; }
		public int ParentCatIdTgt { get; set; }
		public string ParentCatTgt { get; set; }
		public int ChildCatIdTgt { get; set; }
		public string ChildCatTgt { get; set; }
	}
}