using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SyncProducts.Models
{
    public class SaveProductsModel {
        public int WebCatId { get; set; }
        public int WebsiteId { get; set; }
        public string SupplierCategory { get; set; }
        public string SiteCategoryPath { get; set; }
        public string SiteCategoryFilter { get; set; }
        public List<string> Products { get; set; }
    }

    public class SiteCategoryProducts
    {
        public string SiteCategory { get; set; }
        public string SiteCategoryFilter { get; set; }
        public List<string> Products { get; set; }
    }

    public class Product
    {
        public int RowNum { get; set; }
        public string ItemNum { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductDescriptionOrignalHtml { get; set; }
        public string Brand { get; set; }
        public string Category1 { get; set; }
        public string Category2 { get; set; }
        public string Category3 { get; set; }
        public string Category4 { get; set; }
        public string RetailPrice { get; set; }
        public string WholesalePriceBeforeMarkup { get; set; }
        public string MAPPrice { get; set; }
        public string SupplierHandlingFee { get; set; }
        public string Weight { get; set; }
        public string QtyInStock { get; set; }
        public string InStock { get; set; }
        public string UPC { get; set; }
        public string ImageName { get; set; }
        public string URLToLargeImage { get; set; }
        public string URLToThumbImage { get; set; }
        public string RefurbishedFlag { get; set; }
        public string AttributeList { get; set; }
        public string ShippingForUSA { get; set; }
        public string ManufacturerProdId { get; set; }
        public string ShippingLength { get; set; }
        public string ShippingWidth { get; set; }
        public string ShippingHeight { get; set; }
        public string SalesTaxPct { get; set; }
        public string SalesTaxState { get; set; }
        public string Manufacturer { get; set; }
        public string ShippingForCanada { get; set; }
        public string ShippingForInternational { get; set; }
        public string W2bHandlingFee { get; set; }
        public string W2B3PctTransactionFeeUSA { get; set; }
        public string W2B3PctTransactionFeeCanada { get; set; }
        public string W2B3PctTransactionFeeInternational { get; set; }
        public string USAZONE { get; set; }
        public string CANADAZONE { get; set; }
        public string Attributes { get; set; }
        public string ExtraImg1 { get; set; }
        public string ExtraImg2 { get; set; }
        public string ExtraImg3 { get; set; }
        public string ExtraImg4 { get; set; }
        public string ExtraImg5 { get; set; }
        public string ExtraImg6 { get; set; }
        public string DescriptionNoHtml { get; set; }
        public string SupplierNameItemNumber { get; set; }
        public string SupplierCategory1 { get; set; }
        public string SupplierCategory2 { get; set; }
        public string SupplierCategory3 { get; set; }
        public string SupplierCategory4 { get; set; }
        public bool CatMember { get; set; }
    }
}