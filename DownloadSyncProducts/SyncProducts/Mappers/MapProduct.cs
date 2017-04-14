using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using SyncProducts.Models;

namespace SyncProducts.Helpers
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
                prod.ProductName = dr[2].ToString();
                prod.ProductDescription = dr[3].ToString();
                prod.ProductDescriptionOrignalHtml = dr[4].ToString();
                prod.Brand = dr[5].ToString();
                prod.Category1 = dr[6].ToString();
                prod.Category2 = dr[7].ToString();
                prod.Category3 = dr[8].ToString();
                prod.Category4 = dr[9].ToString();
                prod.RetailPrice = dr[10].ToString();
                prod.WholesalePriceBeforeMarkup = dr[11].ToString();
                prod.MAPPrice = dr[12].ToString();
                prod.SupplierHandlingFee = dr[13].ToString();
                prod.Weight = dr[14].ToString();
                prod.QtyInStock = dr[15].ToString();
                prod.InStock = dr[16].ToString();
                prod.UPC = dr[17].ToString();
                prod.ImageName = dr[18].ToString();
                prod.URLToLargeImage = dr[19].ToString();
                prod.URLToThumbImage = dr[20].ToString();
                prod.RefurbishedFlag = dr[21].ToString();
                prod.AttributeList = dr[22].ToString();
                prod.ShippingForUSA = dr[23].ToString();
                prod.ManufacturerProdId = dr[24].ToString();
                prod.ShippingLength = dr[25].ToString();
                prod.ShippingWidth = dr[26].ToString();
                prod.ShippingHeight = dr[27].ToString();
                prod.SalesTaxPct = dr[28].ToString();
                prod.SalesTaxState = dr[29].ToString();
                prod.Manufacturer = dr[30].ToString();
                prod.ShippingForCanada = dr[31].ToString();
                prod.ShippingForInternational = dr[32].ToString();
                prod.W2bHandlingFee = dr[33].ToString();
                prod.W2B3PctTransactionFeeUSA = dr[34].ToString();
                prod.W2B3PctTransactionFeeCanada = dr[35].ToString();
                prod.W2B3PctTransactionFeeInternational = dr[36].ToString();
                prod.USAZONE = dr[37].ToString();
                prod.CANADAZONE = dr[38].ToString();
                prod.Attributes = dr[39].ToString();
                prod.ExtraImg1 = dr[40].ToString();
                prod.ExtraImg2 = dr[41].ToString();
                prod.ExtraImg3 = dr[42].ToString();
                prod.ExtraImg4 = dr[43].ToString();
                prod.ExtraImg5 = dr[44].ToString();
                prod.ExtraImg6 = dr[45].ToString();
                prod.DescriptionNoHtml = dr[46].ToString();
                prod.SupplierNameItemNumber = dr[47].ToString();
                prod.SupplierCategory1 = dr[48].ToString();
                prod.SupplierCategory2 = dr[49].ToString();
                prod.SupplierCategory3 = dr[50].ToString();
                prod.SupplierCategory4 = dr[51].ToString();

                prods.Add(prod);
            }
            
            return prods;
        }
    }
}