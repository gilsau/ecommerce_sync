using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Net;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using EcommerceManager.Models;
using EcommerceManager.Helpers;

namespace EcommerceManager.Data
{
	public static class DataRepository
	{
		public static List<string> GetDownloadedFiles()
		{
			string dirPath = HttpContext.Current.Server.MapPath("/downloadedfiles/products/");
			return Directory.GetFiles(dirPath).ToList();
		}

		public static void ClearImgDir(out Result result)
		{
			result = new Result();
			string sDir = HttpContext.Current.Server.MapPath("/productimages/");

			try
			{
				Array.ForEach(Directory.GetFiles(sDir), File.Delete);
				result.Success = true;
				result.ErrForUser = string.Format("Directory: {0} was cleared successfully!", sDir);
			}
			catch (Exception ex)
			{
				result.Success = true;
				result.ErrForUser = string.Format("There was an error clearing Directory: {0}<br/>{1}<hr/>", sDir, ex.Message);
			}
		}

		private static string CleanImageName(string sImgName) {

			sImgName = sImgName.ToLower();

			//parse name out of url
			if (sImgName.IndexOf('/') > -1) {
				sImgName = sImgName.Split('/')[sImgName.Split('/').Length - 1];
			}

			//remove invalid chars
			string[] arrSymbols = new string[] { "c:", ":", ",", "-" };
			foreach (string sSymbol in arrSymbols) {
				sImgName = sImgName.Replace(sSymbol, "");
			}

			return sImgName;
		}

		private static void SaveImageToDB(string productName, string sku, string filePath, int webId, out Result result) {

			result = new Result();

			//get bytes for image file
			Stream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
			BinaryReader br = new BinaryReader(fs);
			Byte[] bytes = br.ReadBytes((Int32)fs.Length);

			//try and save picture to db
			SqlConnection con = new SqlConnection(GetWebsiteConnStr(webId));
			string sql = string.Format("if not exists(select id from product_picture_mapping where productid=(select id from product where sku='{0}')) begin insert into Picture(PictureBinary,MimeType,SeoFilename,AltAttribute,TitleAttribute,isNew) values(@PictureBinary,'image/jpeg',@SeoFilename,null,null,0); insert into product_picture_mapping values((select id from product where sku='{0}'),(select max(id) from picture),1); end ", sku);
			SqlCommand cmd = new SqlCommand(sql);
			cmd.CommandType = CommandType.Text;
			cmd.Parameters.Add("@PictureBinary", SqlDbType.Binary).Value = bytes;
			cmd.Parameters.Add("@SeoFilename", SqlDbType.VarChar).Value = NormalizeSlug(productName);
			cmd.Connection = con;

			try
			{
				con.Open();
				cmd.ExecuteNonQuery();

				result.Success = true;
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.ErrForUser = string.Format("There was a problem saving the image to the database for product: {0}", filePath);

				string innerMsg = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
				result.ErrForLog = string.Format("<HR/>MESSAGE<BR/>{0}<HR/>INNER EXCEPTION MESSAGE<BR/>{1}<HR/>STACK TRACE<BR/>{2}<HR/>SOURCE<BR/>{3}", ex.Message, innerMsg, ex.StackTrace, ex.Source);
			}
			finally
			{
				cmd.Dispose();
				con.Close();
				con.Dispose();
			}
		}

		private static void DownloadImage(string imageUrl, string filename, ImageFormat format, out Result result)
		{
			result = new Result();

			try
			{
				WebClient client = new WebClient();
				Stream stream = client.OpenRead(imageUrl);
				Bitmap bitmap; bitmap = new Bitmap(stream);

				if (bitmap != null)
					bitmap.Save(filename, format);

				stream.Flush();
				stream.Close();
				client.Dispose();

				result.Success = true;
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.ErrForUser = string.Format("There was a problem downloading image for product: {0}", filename);

				string innerMsg = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
				result.ErrForLog = string.Format("<HR/>MESSAGE<BR/>{0}<HR/>INNER EXCEPTION MESSAGE<BR/>{1}<HR/>STACK TRACE<BR/>{2}<HR/>SOURCE<BR/>{3}", ex.Message, innerMsg, ex.StackTrace, ex.Source);
			}
		}

		public static void DownloadImages(PushToSiteModel model, out Result result)
		{
			result = new Result();

			//connection string
			string webConnStr = GetWebsiteConnStr(model.webId);
			string mainConnStr = ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString;

			//get products' info
			result = new Result();
			string prodNums = string.Join("','", model.productNums);
			string sql = string.Format("select itemNum, [name], imageUrl from productimport where itemnum in ('{0}')", prodNums);
			DataProvider.GetDataTable(mainConnStr, sql, out result);

			//loop thru product list
			StringBuilder sbMapImgToProd = new StringBuilder();
			DataRowCollection prodRows = ((DataTable)result.ReturnObj).Rows;
			foreach (DataRow dr in prodRows)
			{
				//download image
				string sku = dr[0].ToString();
				string prodName = dr[1].ToString();
				string imgUrl = dr[2].ToString();
				string imgName = CleanImageName(imgUrl);
				string filePath = string.Format("{0}{1}", HttpContext.Current.Server.MapPath("/productimages/"), imgName);

				result = new Result();
				DownloadImage(imgUrl, filePath, ImageFormat.Jpeg, out result);

				//save img to db
				if (result.Success) {
					result = new Result();
					SaveImageToDB(prodName, sku, filePath, model.webId, out result);
				}
				else
				{
					string sqlDelProd = string.Format("delete product where sku='{0}'", sku);
					DataProvider.ExecuteQuery(webConnStr, sqlDelProd, false, out result);
				}
			}

			//persist img for related categories, use random product pic
			string prodSKUForCatPics = prodRows[0][0].ToString();
			int parentCatPicId = 0;
			int parentCatId = 0;
			int subCatPicId = 0;
			int subCatId = 0;
			if (result.Success)
			{
				result = new Result();
				string sqlGetCatPics = string.Format("select ParentCatId=pc.id, SubCatId=sc.id, ppm.pictureid from product p left join product_category_mapping pcm on pcm.productid = p.id left join category sc on sc.id = pcm.categoryid left join category pc on pc.id = sc.parentcategoryid left join product_picture_mapping ppm on ppm.productid = p.id where sku = '{0}' ", prodSKUForCatPics);
				DataProvider.GetDataTable(webConnStr, sqlGetCatPics, out result);
				DataRowCollection catPicRows = ((DataTable)result.ReturnObj).Rows;
				parentCatPicId = int.Parse(catPicRows[0][2].ToString());
				parentCatId = int.Parse(catPicRows[0][0].ToString());
				subCatPicId = int.Parse(catPicRows[0][2].ToString());
				subCatId = int.Parse(catPicRows[0][1].ToString());
			}

			if (result.Success)
			{
				result = new Result();
				string sqlCatPic = string.Format(" update category set pictureId={0} where id={1}; update category set pictureId={2} where id={3} ", parentCatPicId, parentCatId, subCatPicId, subCatId);
				DataProvider.ExecuteQuery(webConnStr, sqlCatPic, false, out result);
			}

			if (result.Success)
			{
				result.ErrForUser = "Images were downloaded successfuly!";
			}
		}

		public static void AddImagesToDB(PushToSiteModel model, out Result result)
		{
			result = new Result();

			//connection string
			string webConnStr = GetWebsiteConnStr(model.webId);
			string mainConnStr = ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString;

			//get products' info
			result = new Result();
			string prodNums = string.Join("','", model.productNums);
			string sql = string.Format("select itemNum, name, imageUrl from productimport where itemnum in ('{0}')", prodNums);
			DataProvider.GetDataTable(mainConnStr, sql, out result);

			//loop thru product list
			StringBuilder sbMapImgToProd = new StringBuilder();
			foreach (DataRow dr in ((DataTable)result.ReturnObj).Rows)
			{
				string sku = dr[0].ToString();
				string prodName = NormalizeSlug(dr[1].ToString());
				string imgUrl = CleanImageName(dr[2].ToString());
				string filePath = string.Format("{0}{1}", HttpContext.Current.Server.MapPath("/productimages/"), imgUrl);

				//save image to db
				Result resultSI = new Result();
				//SaveImageToDB(prodName, sku, filePath, model.webId, out resultSI);

				if (resultSI.Success)
				{
					//string to map image to product in db
					int picId = int.Parse(resultSI.ReturnObj.ToString());
					string sMapProdImg = string.Format(" if not exists(select id from Product_Picture_Mapping where productid=(select id from product where sku='{0}') and pictureid={1}) begin insert into Product_Picture_Mapping values((select id from product where sku = '{0}'), {1}, 1); end ", sku, picId);
					sbMapImgToProd.Append(sMapProdImg);
				}
			}

			//run sql to map image to product on website's db
			DataProvider.ExecuteQuery(webConnStr, sbMapImgToProd.ToString(), false, out result);
			if (result.Success)
			{
				result.ErrForUser = "Images were mapped to products in database, for website successfully!";
			}
		}


		private static string NormalizeSlug(string slug)
		{
			Regex rgx = new Regex("[^a-zA-Z0-9 -]");
			slug = rgx.Replace(slug, "");
			slug = slug.Replace(" ", "-");
			return slug;
		}

		public static void PopulateProductImportTable(out Result result)
		{
			result = new Result();

			StringBuilder sb = new StringBuilder("insert into ProductImport");
			sb.Append(" select pi1.ItemNum,");
			sb.Append(" [Name] = SUBSTRING(ProductName, 1, 100),");
			sb.Append(" [Description] = convert(nvarchar(4000), pi2.ProductDescriptionOriginalHtml),");
			sb.Append(" Wholesale = cast(pi2.WholesalePriceBeforeMarkup as money),");
			sb.Append(" Retail = cast(pi2.RetailPrice as money),");
			sb.Append(" Qty = cast(pi2.QtyInStock as int),");
			sb.Append(" ImageUrl = pi2.URLToLargeImage,");
			sb.Append(" [Length] = iif(ISNUMERIC(pi2.shippinglength) = 1, cast(pi2.shippinglength as decimal), 0),");
			sb.Append(" Height = iif(ISNUMERIC(pi2.shippingheight) = 1, cast(pi2.shippingheight as decimal), 0),");
			sb.Append(" Width = iif(ISNUMERIC(pi2.shippingwidth) = 1, cast(pi2.shippingwidth as decimal), 0),");
			sb.Append(" [Weight] = iif(ISNUMERIC(pi2.[Weight]) = 1, cast(pi2.[Weight] as decimal), 0),");
			sb.Append(" Category1 = SUBSTRING(pi2.category1, 1, 100),");
			sb.Append(" Category2 = SUBSTRING(pi2.category2, 1, 100),");
			sb.Append(" Category3 = SUBSTRING(pi2.category3, 1, 100)");
			sb.Append(" from(select ItemNum, RetailProdName = min(convert(nvarchar(10), RetailPrice) + ProductName + SupplierNameItemNumber)");
			sb.Append(" from ProductImportPre");
			sb.Append(" where ltrim(rtrim(ItemNum)) <> ''");
			sb.Append(" and ltrim(rtrim(ProductName)) <> ''");
			sb.Append(" and isnumeric(WholesalePriceBeforeMarkup) = 1");
			sb.Append(" and ISNUMERIC(RetailPrice) = 1");
			sb.Append(" and ltrim(rtrim(Category1)) <> ''");
			sb.Append(" group by ItemNum) as pi1");
			sb.Append(" inner join(select ItemNum,");
			sb.Append(" RetailProdName = convert(nvarchar(10), RetailPrice) + ProductName + SupplierNameItemNumber,");
			sb.Append(" ProductName,");
			sb.Append(" ProductDescriptionOriginalHtml,");
			sb.Append(" WholesalePriceBeforeMarkup,");
			sb.Append(" RetailPrice,");
			sb.Append(" QtyInStock,");
			sb.Append(" URLToLargeImage,");
			sb.Append(" ShippingLength,");
			sb.Append(" ShippingHeight,");
			sb.Append(" ShippingWidth,");
			sb.Append(" [Weight],");
			sb.Append(" Category1,");
			sb.Append(" Category2,");
			sb.Append(" Category3");
			sb.Append(" from ProductImportPre) as pi2");
			sb.Append(" on pi2.ItemNum = pi1.ItemNum and pi2.RetailProdName = pi1.RetailProdName");

			DataProvider.ExecuteQuery(ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString, sb.ToString(), false, out result);

			if (result.Success)
			{
				result.ErrForUser = "Table: ProductImport was populated successfully!";
			}
		}

		public static void ImportFileToDB(string filePath, out Result result)
		{
			result = new Result();
			string[] filePathArr = new string[] { };
			DataTable dtCSVData = new DataTable();

			if (!string.IsNullOrEmpty(filePath))
			{
				//read CSV file
				using (var reader = new CsvHelper.CsvReader(new StreamReader(filePath)))
				{
					//add columns to data table
					reader.ReadHeader();
					for (int c = 0; c < 55; c++)
					{
						dtCSVData.Columns.Add();
					}

					//add rows to data table
					while (reader.Read())
					{
						dtCSVData.Rows.Add(reader.CurrentRecord);
					}
				}

				//insert data into db table
				int prevProdCount = int.Parse(HttpContext.Current.Application["ProductImportPreCount"].ToString());
				int postProdCount = 0;
				using (SqlConnection dbConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString))
				{
					dbConnection.Open();
					using (SqlBulkCopy s = new SqlBulkCopy(dbConnection))
					{
						s.BatchSize = 5000;
						s.DestinationTableName = "ProductImportPre";

						try
						{
							s.WriteToServer(dtCSVData);
							result.Success = true;
						}
						catch (Exception ex)
						{
							result.Success = false;
							result.ErrForUser = ex.Message;
							Logger.LogError(ex, new List<string>() { result.ErrForUser });
						}
						finally
						{
							HttpContext.Current.Application["ProductImportPreCount"] = GetProductCount();
							postProdCount = int.Parse(HttpContext.Current.Application["ProductImportPreCount"].ToString());
						}
					}
				}

				if (result.Success)
				{
					filePathArr = filePath.Split('\\');
					result.ErrForUser = string.Format("File: {0}<br/>Products Added: {1}<hr/>", filePathArr[filePathArr.Length - 1], postProdCount - prevProdCount);
				}
				else
				{
					result.ErrForUser = string.Format("Failed to import ALL DATA from file: {0}, but was able to import {1} of {2}<br/><b>ERROR: {3}</b><hr/>", filePath, postProdCount - prevProdCount, dtCSVData.Rows.Count, result.ErrForUser);
				}
			}
		}

		public static List<Product> GetProductsByKeyword(string keyword, int pageSize, int page) {
			Result result = new Result();

			string sql = string.Format("select row_number() over (order by name) as RowNum, *, CatMember=0, SiteCategory='', Count=COUNT(*) OVER() from productimport where itemnum like '%{0}%' or [name] like '%{0}%' or [description] like '%{0}%' or category1 like '%{0}%' or category2 like '%{0}%' or category3 like '%{0}%' order by RowNum OFFSET {1} * ({2} - 1) ROWS FETCH NEXT {1} ROWS ONLY OPTION(RECOMPILE);", keyword, pageSize, page);
			DataProvider.GetDataTable(ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString, sql, out result);
			DataTable dt = (DataTable)result.ReturnObj;
			List<Product> prods = MapProducts.MapDBToProdList((DataTable)result.ReturnObj);

			return prods;
		}

		public static List<Product> GetProductsFromWebsiteByKeyword(int webId, string keyword, int pageSize, int page)
		{
			List<Product> webProds = new List<Product>();
			Result result = new Result();
			string mainConnStr = ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString;
			string webConnStr = GetWebsiteConnStr(webId);

			//get local skus for keyword
			List<string> localSkus = GetProductsByKeyword(keyword, pageSize, page).Select(lp => lp.ItemNum).ToList();

			//website skus for keyword
			result = new Result();
			string sql = string.Format("select p.sku from product where itemnum in ('{0}')", string.Join("','", localSkus));
			DataProvider.GetDataTable(webConnStr, sql, out result);
			string webSkus = string.Join("','", ((DataTable)result.ReturnObj).AsEnumerable().Select(dr => dr[0].ToString()).ToList());

			//get local product info for skus found
			sql = string.Format("select row_number() over(order by pi.Name) as RowNum, pi.*, CatMember=1, SiteCategory='', Count=COUNT(*) OVER() from productimport pi where itemnum in ('{0}')", webSkus);
			DataProvider.GetDataTable(mainConnStr, sql, out result);
			webProds = MapProducts.MapDBToProdList((DataTable)result.ReturnObj);

			return webProds;
		}

		public static List<Product> GetProductsByCategory(string category, int pageSize, int page)
		{
			Result result = new Result();
			string[] suppCatArr = category.Split('~');
			string cat1 = suppCatArr.Length > 0 ? suppCatArr[0].Trim().Replace("'", "''") : string.Empty;
			string cat2 = suppCatArr.Length > 1 ? suppCatArr[1].Trim().Replace("'", "''") : string.Empty;
			string cat3 = suppCatArr.Length > 2 ? suppCatArr[2].Trim().Replace("'", "''") : string.Empty;

			string sql = string.Format("select row_number() over(order by pi.Name) as RowNum, pi.*, CatMember=0, SiteCategory='', Count=COUNT(*) OVER() from productImport pi where rtrim(ltrim(category1)) = '{0}' and rtrim(ltrim(category2)) = '{1}' and rtrim(ltrim(category3)) = '{2}' order by RowNum OFFSET {3} * ({4} - 1) ROWS FETCH NEXT {3} ROWS ONLY OPTION(RECOMPILE);", cat1, cat2, cat3, pageSize, page);

			DataProvider.GetDataTable(ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString, sql, out result);
			List<Product> prods = MapProducts.MapDBToProdList((DataTable)result.ReturnObj);

			return prods;
		}

		public static List<Product> GetProductsFromWebsiteByCategory(int webId, string supplierCategory, List<string> lstItemNums)
		{
			List<Product> webProds = new List<Product>();
			Result result = new Result();
			string mainConnStr = ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString;
			string webConnStr = GetWebsiteConnStr(webId);
			string webCat = GetWebsiteCategory(webId, supplierCategory);
			webCat = supplierCategory.Split('~')[1].Trim();
			int? webCatId = GetWebsiteCategoryId(webId, webCat);

			if (webCatId != null)
			{
				//get products from website's database for this category
				result = new Result();
				string strSKUs = string.Join("','", lstItemNums);
				string sql = string.Format("select p.sku from product p inner join Product_Category_Mapping pcm on pcm.ProductId = p.Id inner join Category c on c.Id = pcm.CategoryId where c.Id={0} and p.deleted=0 and sku in ('{1}')", webCatId, strSKUs);
				DataProvider.GetDataTable(webConnStr, sql, out result);
				string skusFoundOnSiteForCat = string.Join("','", ((DataTable)result.ReturnObj).AsEnumerable().Select(dr => dr[0].ToString()).ToList());

				//get product info for skus found
				sql = string.Format("select row_number() over(order by pi.Name) as RowNum, pi.*, CatMember=1, SiteCategory='{0}', Count=COUNT(*) OVER() from productimport pi where itemnum in ('{1}')", webCat, skusFoundOnSiteForCat);
				DataProvider.GetDataTable(mainConnStr, sql, out result);
				webProds = MapProducts.MapDBToProdList((DataTable)result.ReturnObj);
			}

			return webProds;
		}

		public static int GetProductCount() {
			return GetProductCount(ProdCountType.ProductImportPreAll, null, null);
		}

		public static int GetProductCount(ProdCountType prodCountType, string keyword, string category)
		{
			int cnt = 0;
			string sql = string.Empty;

			if (prodCountType == ProdCountType.Keyword)
			{
				sql = string.Format("select count(itemnum) from productimport where [name] like '%{0}%' or[description] like '%{0}%' or category1 like '%{0}%' or category2 like '%{0}%' or category3 like '%{0}%'", keyword);
			}
			else if (prodCountType == ProdCountType.Category)
			{
				string[] suppCatArr = category.Split('~');
				string cat1 = suppCatArr.Length > 0 ? suppCatArr[0].Trim().Replace("'", "''") : string.Empty;
				string cat2 = suppCatArr.Length > 1 ? suppCatArr[1].Trim().Replace("'", "''") : string.Empty;
				string cat3 = suppCatArr.Length > 2 ? suppCatArr[2].Trim().Replace("'", "''") : string.Empty;

				sql = string.Format("select count(itemnum) from productImport pi where rtrim(ltrim(category1)) = '{0}' and rtrim(ltrim(category2)) = '{1}' and rtrim(ltrim(category3)) = '{2}'", cat1, cat2, cat3);
			}
			else if (prodCountType == ProdCountType.ProductImportPreAll) {
				sql = "select count(itemnum) from productimportpre";
			}

			Result result = new Result();
			DataProvider.ExecuteQuery(ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString, sql, true, out result);
			cnt = int.Parse(result.ReturnObj.ToString());

			return cnt;
		}

		public static void ProductsPersist(PushToSiteModel model, out Result result)
		{
			result = new Result();

			//connection strings
			string webConnStr = GetWebsiteConnStr(model.webId);
			string mainConnStr = ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString;

			//get products' info
			result = new Result();
			string prodNums = string.Join("','", model.productNums);
			string sql = string.Format("select row_number() over (order by ItemNum) as Id, * from productimport where itemnum in ('{0}')", prodNums);
			DataProvider.GetDataTable(mainConnStr, sql, out result);

			//get website's category id to map to products
			string webParentCat = model.siteCat.Split('-')[0].Trim();
			int? webParentCatId = GetWebsiteCategoryId(model.webId, webParentCat);
			string webCat = model.siteCat.Split('-')[1].Trim();
			int? webCatId = GetWebsiteCategoryId(model.webId, webCat);
			string sWebUrl = GetWebsiteUrl(model.webId);
			if (webCatId != null && result.Success)
			{
				//create sql statements to add products
				DataTable dtProds = (DataTable)result.ReturnObj;
				StringBuilder sbProduct = new StringBuilder();
				StringBuilder sbProductEcomm = new StringBuilder();
				string sNewProdDesc = string.Empty;
				foreach (DataRow dr in dtProds.Rows)
				{
					//string to insert product in db
					string sProdName = CleanProductName(Utilities.ScrubHtml(dr[2].ToString()).Replace("'", "''"));
					string sProdDesc = dr[3].ToString().Replace("'", "''");
					string slugProd = NormalizeSlug(string.Format("{0}-{1}", webParentCat, sProdName));
					string sInsProd = string.Format(" if exists(select {0} from product where sku='{1}') begin update product set Deleted=0, name='{2}', shortDescription='{3}', fullDescription=null, productCost={4}, price={5}, stockQuantity={6}, length={8}, height={9}, width={10}, weight={11} where name='{2}'; end else begin insert into product values(5, 0, 1, '{2}', '{3}', NULL, NULL, 1, 0, 0, NULL, NULL, NULL, 1, 0, 0, 0, 0, 0, 0, '{1}', NULL, NULL, 0, 0, NULL, 0, NULL, 0, 0, 0, 0, 0, NULL, 0, 0, 0, 0, NULL, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0.0000, 0, 0, 2, 0, 1, 0, 0, 0, {6}, 1, 0, 0, 1, 1, 0, 0, 1, 10000, NULL, 0, 0, 0, 0, 0, NULL, 0, {5}, 0.0000, {4}, 0, 0.0000, 0.0000, 0, 0.0000, 0, 0.0000, 0, 0, NULL, NULL, 0, 0, {11}, {8}, {10}, {9}, NULL, NULL, 0, 1, 0, getdate(), getdate()); end ", dr[0].ToString(), dr[1].ToString(), sProdName, sProdDesc, dr[4].ToString(), dr[5].ToString(), dr[6].ToString(), dr[7].ToString(), dr[8].ToString(), dr[9].ToString(), dr[10].ToString(), dr[11].ToString(), dr[12].ToString());
					sbProduct.Append(sInsProd);

					//string to map product to category in db
					string sMapProdCat = string.Format(" if not exists(select 1 from Product_Category_Mapping where ProductId=(select top 1 id from product where sku='{0}') and CategoryId={1}) begin insert into Product_Category_Mapping values((select top 1 id from product where sku='{0}'), {1}, 0, 1); end ", dr[1].ToString(), webCatId);
					sbProduct.Append(sMapProdCat);

					//string to insert url record for product in db
					string sInsUrlProd = string.Format(" if exists(select id from UrlRecord where EntityName='Product' and EntityId=(select id from product where sku='{0}')) begin update UrlRecord set IsActive=1 where EntityName='Product' and EntityId=(select id from product where sku='{0}'); end else begin insert into UrlRecord values((select id from product where sku='{0}'), 'Product', '{1}', 1, 0); end ", dr[1].ToString(), slugProd);
					sbProduct.Append(sInsUrlProd);

					//if clothing, add sizes (product attributes) for product
					if (ProductIsClothing(sProdName))
					{
						//persist productAttribute
						sbProduct.Append(" if not exists(select 1 from productAttribute where [name]='Size') begin insert into productAttribute values('Size', null) end ");

						//map product with productAttribute
						sbProduct.Append(string.Format(" if not exists(select 1 from Product_ProductAttribute_Mapping where productid=(select id from product where sku='{0}') and productAttributeId=(select id from productAttribute where [name]='Size')) begin insert into product_productAttribute_Mapping values((select id from product where sku='{0}'), (select id from productAttribute where [name]='Size'), NULL, 1, 1, 1, NULL, NULL, NULL, NULL, NULL, NULL) end ", dr[1].ToString()));

						//provide values for productAttribute
						int displayOrder = 0;
						List<string> lstSizes = GetSizes(sProdDesc, out sNewProdDesc);
						foreach (string sz in lstSizes)
						{
							displayOrder++;
							sbProduct.Append(string.Format(" if not exists(select 1 from ProductAttributeValue where productAttributeMappingId=(select id from product_productAttribute_Mapping where productId=(select id from product where sku='{0}') and productAttributeId=(select id from productAttribute where [name]='Size')) and [Name]='{1}') begin insert into productAttributeValue values((select id from product_productAttribute_Mapping where productId = (select id from product where sku = '{0}') and productAttributeId = (select id from productAttribute where [name] = 'Size')),0,0,'{1}',NULL,0,0.0000,0.0000,0.0000,0,0,0,{2},0) end ", dr[1].ToString(), sz, displayOrder));
						}

						//update product description
						if (!string.IsNullOrEmpty(sNewProdDesc))
						{
							sProdDesc = sNewProdDesc;
							sbProduct.Append(string.Format(" update product set shortdescription='{0}' where sku='{1}' ", sNewProdDesc, dr[1].ToString()));
						}
					}

					//save product to ecommerce db
					sbProductEcomm.Append(string.Format(" if not exists(select 1 from webproduct where websiteId={0} and sku='{1}' and lower(siteParentCategory)='{2}' and lower(SiteSubCategory)='{3}') begin insert into webproduct values('{1}', '{4}', '{5}', '{2}', '{3}', {0}, '{6}', getdate()) end ", model.webId, dr[1].ToString(), webParentCat.ToLower(), webCat.ToLower(), sProdName, sProdDesc, sWebUrl));
				}

				//run sql to insert products on website's db
				if (result.Success)
				{
					result = new Result();
					DataProvider.ExecuteQuery(webConnStr, sbProduct.ToString(), false, out result);
				}

				//run sql to insert products on local db
				if (result.Success)
				{
					result = new Result();
					DataProvider.ExecuteQuery(mainConnStr, sbProductEcomm.ToString(), false, out result);
				}

				//run sql to update global settings
				if (result.Success)
				{
					string webName = GetWebsiteName(model.webId);

					result = new Result();
					StringBuilder sbGlobal = new StringBuilder();
					sbGlobal.Append(string.Format(" update setting set value = '{0}' where name = 'seosettings.defaulttitle'; ", webName));
					sbGlobal.Append(" update topic set published = 0 where systemName = 'homepagetext'; ");
					sbGlobal.Append(" update category set ShowOnHomePage = 1 where ParentCategoryId = 0; ");
					sbGlobal.Append(" update setting set value = 'True' where name = 'catalogsettings.searchpageproductsperpage'; ");
					sbGlobal.Append(" update setting set value = '100,50,20' where name = 'catalogsettings.searchpagepagesizeoptions'; ");
					sbGlobal.Append(" update setting set value = '100' where name = 'forumsettings.searchresultspagesize'; ");
					sbGlobal.Append(" update setting set value = 'True' where name = 'catalogsettings.showcategoryproductnumber' or name = 'catalogsettings.showcategoryproductnumberincludingsubcategories' or name = 'catalogsettings.includefeaturedproductsinnormallists'; ");
					sbGlobal.Append(" update setting set value = 'False' where name = 'displaydefaultmenuitemsettings.displayblogmenuitem' or name = 'displaydefaultmenuitemsettings.displaycontactusmenuitem' or name = 'displaydefaultmenuitemsettings.displayforumsmenuitem' or name = 'displaydefaultmenuitemsettings.displayhomepagemenuitem' or name = 'displaydefaultmenuitemsettings.displayproductsearchmenuitem' or name = 'newssettings.shownewsonmainpage'; ");
					sbGlobal.Append(" update setting set value = '40' where name = 'catalogsettings.newproductsnumber'; ");
					sbGlobal.Append(" update product set showonhomepage=0; update product set showonhomepage=1 where id in (SELECT TOP 20 id FROM product ORDER BY NEWID()); ");
					sbGlobal.Append(" update product set MarkAsNew = 0; update product set MarkAsNew = 1 where id in (SELECT TOP 40 id FROM product ORDER BY NEWID()); ");
					DataProvider.ExecuteQuery(webConnStr, sbGlobal.ToString(), false, out result);
				}

				if (result.Success)
				{
					result.ErrForUser = string.Format("{0} products persisted to website successfully!", model.productNums.Count());
				}
			}
		}

		private static string CleanProductName(string sProdName) {
			string sNewProdName = string.Format("{0} ", sProdName.ToUpper());
			List<string> lstWords = new List<string>() { " XS ", " S ", " M ", " L ", " XL ", " XXL ", " XXXL ", " 1XL ", " 2XL ", " 3XL ", " 4XL ", " 5XL ", " 6XL ", " 7XL ", " 30 ", " 31 ", " 32 ", " 33 ", " 34 ", " 35 ", " 36 ", " 37 ", " 38 ", " 39 ", " 40 ", " 41 ", " 42 " };

			foreach (string word in lstWords) {
				sNewProdName = sNewProdName.Replace(word, "");
			}

			return sNewProdName;
		}

		private static List<string> GetSizes(string sProdDesc, out string sNewProdDesc) {
			List<string> lstSizes = new List<string>();
			sNewProdDesc = string.Empty;

			//check if there's a table in description with sizes
			int iStartIdx = sProdDesc.IndexOf("<table");
			int iEndIdx = sProdDesc.IndexOf("</table>");

			if (iStartIdx > -1 && iEndIdx > -1)
			{
				string sTblHtml = sProdDesc.Substring(iStartIdx, iEndIdx - iStartIdx) + "</table>";

				XmlDocument xDoc = new XmlDocument();
				xDoc.LoadXml("<?xml version=\"1.0\" encoding=\"UTF - 8\"?>" + sTblHtml);
				XmlNodeList nodeList = xDoc.SelectNodes("//tr/td[1]");

				//save sizes provided
				if (nodeList.Count > 0)
				{
					foreach (XmlNode node in nodeList)
					{
						lstSizes.Add(node.InnerText);
					}
				}
			}

			//check if there's a JSON string with the sizes
			iStartIdx = -1;
			iEndIdx = -1;

			if (lstSizes.Count < 1 &&
				sProdDesc.ToLower().IndexOf("size") > -1 &&
				sProdDesc.ToLower().IndexOf("{") > -1 &&
				sProdDesc.ToLower().IndexOf("}") > -1)
			{
				//check if there's a table in description with sizes
				iStartIdx = sProdDesc.IndexOf("{");
				iEndIdx = sProdDesc.IndexOf("]}", iStartIdx) + 2;

				if (iStartIdx > -1 && iEndIdx > -1)
				{
					string sJSON = sProdDesc.Substring(iStartIdx, iEndIdx - iStartIdx);
					sJSON = sJSON.Replace("''", "\"");
					ProdSizes sizes = new JavaScriptSerializer().Deserialize<ProdSizes>(sJSON);

					//build reference html table for sizes
					StringBuilder sbTbl = new StringBuilder("<table style=\"width:100%;\" border=\"1\" border-spacing=\"5\">");
					sbTbl.Append("<tr>");
					foreach (string sHead in sizes.title) {
						sbTbl.Append(string.Format("<th>{0}</th>", sHead));
					}
					sbTbl.Append("</tr>");

					foreach (List<string> lstSize in sizes.data) {

						//add row of sizes to html table
						sbTbl.Append("<tr>");
						foreach (string sCol in lstSize) {
							sbTbl.Append(string.Format("<td>{0}</td>", sCol));
						}
						sbTbl.Append("</tr>");

						//add to list of sizes
						lstSizes.Add(lstSize.ElementAt(0));
					}
					sbTbl.Append("</table>");

					//modify description, replace json string with html table
					sNewProdDesc = sProdDesc.Substring(0, iStartIdx - 1);
					sNewProdDesc = string.Format("{0}<br/><br/>{1}", sNewProdDesc, sbTbl.ToString());
				}
			}

			//default values sent back
			if (lstSizes.Count < 1)
			{
				lstSizes = new List<string>() { "S", "M", "L", "XL", "XXL" };
			}

			return lstSizes;
		}

		private static bool ProductIsClothing(string sProdName) {
			bool isClothProd = false;

			List<string> lstKeywords = new List<string>() { "pant", "shirt", "sleeve", "hooded", "jean", "shorts", "hoodie", "jacket", "coat", "sweater" };
			foreach (string word in lstKeywords)
			{
				if (sProdName.ToLower().Contains(word))
				{
					isClothProd = true;
				}
			}

			return isClothProd;
		}

		public class ProdSizes
		{
			public string id { get; set; }
			public List<string> title { get; set; }
			public List<List<string>> data { get; set; }
		}

		public enum ProdCountType
		{
			Keyword,
			Category,
			ProductImportPreAll
		}


		public static List<WebsiteCategory> GetWebsiteCategories() {
			List<WebsiteCategory> Cats = new List<WebsiteCategory>();
			Result result = new Result();
			string sql = "select websiteid, url, siteparentcategory, sitesubcategory from webproduct group by url, websiteid, siteparentcategory, sitesubcategory order by url, websiteid, siteparentcategory, sitesubcategory";
			DataProvider.GetDataTable(ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString, sql, out result);

			DataRowCollection rows = null;
			if (result.Success) {
				rows = (DataRowCollection)result.ReturnObj;
			}

			if (rows != null)
			{
				foreach (DataRow dr in rows)
				{
					WebsiteCategory cat = new WebsiteCategory();
					cat.WebId = int.Parse(dr[0].ToString());
					cat.WebUrl = dr[1].ToString();
					cat.ParentCatId = int.Parse(dr[2].ToString());
					cat.ParentCat = dr[3].ToString();
					cat.ChildCatId = int.Parse(dr[4].ToString());
					cat.ChildCat = dr[5].ToString();
					Cats.Add(cat);
				}
			}

			return Cats;
		}

        public static void CategoryPersist(PushToSiteModel model, out Result result) {

            result = new Result();
            
            string webConnStr = GetWebsiteConnStr(model.webId);
            string mainConnStr = ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString;

            //save category mapping for website
            if (!string.IsNullOrEmpty(model.supplierCat))
            {
                model.supplierCat = model.supplierCat.Replace("'", "''");
                string sqlWebCat = string.Format("if not exists(select 1 from WebsiteCategory where WebsiteId={0} and SupplierCategory='{1}' and WebsiteCategory='{2}') begin insert into WebsiteCategory values({0}, '{1}', '{2}'); end", model.webId, model.supplierCat, model.siteCat);
                DataProvider.ExecuteQuery(mainConnStr, sqlWebCat, false, out result);
            }
            else {
                result.Success = true;
            }

			//categories
			string sqlCat = string.Empty;
			string[] arrWebCat = model.siteCat.Split('-');
			string webCat1 = arrWebCat[0].Trim();
			string webCat2 = arrWebCat[1].Trim();
			int? webCat1Id = GetWebsiteCategoryId(model.webId, webCat1);
			int? webCat2Id = GetWebsiteCategoryId(model.webId, webCat2);

			//insert category on website
			if (result.Success)
			{
				if (webCat1Id == 0)
				{
					//add/update parent category on website
					result = new Result();
					sqlCat = string.Format(" insert into Category values('{0}', NULL, 1, NULL, NULL, NULL, 0, 1, 100, 1, '100,50,20', NULL, 1, 1, 0, 0, 1, 0, 1, getdate(), getdate()) ", webCat1);
					DataProvider.ExecuteQuery(webConnStr, sqlCat, false, out result);

					//get id
					webCat1Id = GetWebsiteCategoryId(model.webId, webCat1);
				}
			}

			//add/update UrlRecord for parent category
			if (result.Success)
			{
				string slug1 = webCat1.ToLower().Replace(" ", "-");
				result = new Result();
				sqlCat = string.Format("if exists(select id from UrlRecord where EntityName='Category' and EntityId={0}) begin update UrlRecord set IsActive=1 where EntityName='Category' and EntityId={0}; end else begin insert into UrlRecord values({0}, 'Category', '{1}', 1, 0); end", webCat1Id, slug1);
				DataProvider.ExecuteQuery(webConnStr, sqlCat, false, out result);
			}

			//add/update child category on website
			if (result.Success)
            {
				if (webCat2Id == 0)
				{
					result = new Result();
					sqlCat = string.Format(" insert into Category values('{1}', NULL, 1, NULL, NULL, NULL, (select id from category where name = '{0}'), 1, 100, 1, '100,50,20', NULL, 0, 1, 0, 0, 1, 0, 1, getdate(), getdate()); ", webCat1, webCat2);
					DataProvider.ExecuteQuery(webConnStr, sqlCat, false, out result);

					//get id
					webCat2Id = GetWebsiteCategoryId(model.webId, webCat2);
				}
            }

			//add/update UrlRecord for child category
			if (result.Success)
            {
                string slug = webCat2.ToLower().Replace(" ", "-");
                result = new Result();
                sqlCat = string.Format("if exists(select id from UrlRecord where EntityName='Category' and EntityId={0}) begin update UrlRecord set IsActive=1 where EntityName='Category' and EntityId={0}; end else begin insert into UrlRecord values({0}, 'Category', '{1}', 1, 0); end", webCat2Id, slug);
                DataProvider.ExecuteQuery(webConnStr, sqlCat, false, out result);
            }

			if (result.Success) {
				result.ErrForUser = string.Format("Category: \"{0}\", persisted on website successfully!", model.siteCat);
			}
        }

        public static string GetWebsiteCategory(int webId, string supplierCategory)
        {
            supplierCategory = supplierCategory.Replace("'", "''");
            string mainConnStr = ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString;
            Result result = new Result();

            //get website category mapped to this supplier category
            string sqlWebCat = string.Format("select WebsiteCategory from WebsiteCategory where WebsiteId={0} and SupplierCategory='{1}'", webId, supplierCategory);
            DataProvider.GetDataTable(mainConnStr, sqlWebCat, out result);
            DataTable dt = (DataTable)result.ReturnObj;
            string webCat = dt.Rows.Count > 0 ? dt.Rows[0][0].ToString() : string.Empty;

            return webCat;
        }

        public static int? GetWebsiteCategoryId(int webId, string websiteCategory)
        {
            int? webCatId = null;
            websiteCategory = websiteCategory.Replace("'", "''");

            if (!string.IsNullOrEmpty(websiteCategory))
            {
                string webConnStr = GetWebsiteConnStr(webId);
                Result result = new Result();
                
                //get website category id
                string sqlWebCat = string.Format("select Id from Category where name='{0}'", websiteCategory);
                DataProvider.GetDataTable(webConnStr, sqlWebCat, out result);
                DataTable dt = (DataTable)result.ReturnObj;
                webCatId = dt.Rows.Count > 0 ? int.Parse(dt.Rows[0][0].ToString()) : 0;
            }

            return webCatId;
        }

        public static List<Category> GetLocalCategories()
        {
            Result result = new Result();

            string sql = "select * from categories order by fullpath";

            DataProvider.GetDataTable(ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString, sql, out result);
            List<Category> cats = MapCategories.MapDBToCatList((DataTable)result.ReturnObj);

            return cats;
        }

        public static void CreateCategoriesTable(out Result result)
        {
            result = new Result();

            DataProvider.ExecuteQuery(ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString, "if exists(select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME='categories') begin drop table categories; end select FullPath = Category1 + ' ~ ' + Category2 + ' ~ ' + Category3, ProductCount = count(ItemNum) into Categories from ProductImportPre where ltrim(rtrim(Category1)) <> '' group by Category1, Category2, Category3 order by Category1, Category2, Category3 ", false, out result);

            if (result.Success)
            {
                result.ErrForUser = "Table: Categories was created successfully!";
            }
        }


        public static List<Website> GetWebsites()
        {
            Result result = new Result();
            DataProvider.GetDataTable(ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString, "select * from website", out result);
            List<Website> webs = MapWebsites.MapDBToWebsiteList((DataTable)result.ReturnObj);

            return webs;
        }

        public static string GetWebsiteConnStr(int websiteId) {
            string connStr = string.Empty;

            //get website info
            string sql = string.Format("select * from website where id={0}; ", websiteId);
            Result result = new Result();
            DataProvider.GetDataTable(ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString, sql, out result);
            DataTable dtWeb = (DataTable)result.ReturnObj;
            List<Website> webs = MapWebsites.MapDBToWebsiteList((DataTable)result.ReturnObj);

            connStr = string.Format("server={0};database={1};uid={2};pwd={3}", webs[0].Server, webs[0].Database, webs[0].Username, webs[0].Password);

            return connStr;
        }

		private static string GetWebsiteName(int webId) {
			string webName = string.Empty;
			Result result = new Result();

			string mainConnStr = ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString;
			string sql = string.Format("select name from website where id={0}", webId);
			DataProvider.ExecuteQuery(mainConnStr, sql, true, out result);
			
			if (result.Success) {
				webName = result.ReturnObj.ToString();
			}

			return webName;
		}

		private static string GetWebsiteUrl(int webId)
		{
			string webUrl = string.Empty;
			Result result = new Result();

			string mainConnStr = ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString;
			string sql = string.Format("select url from website where id={0}", webId);
			DataProvider.ExecuteQuery(mainConnStr, sql, true, out result);

			if (result.Success)
			{
				webUrl = result.ReturnObj.ToString();
			}

			return webUrl;
		}

		public static void TruncateTable(string table, out Result resultClear) {
            resultClear = new Result();
            DataProvider.ExecuteQuery(ConfigurationManager.ConnectionStrings["MainConnStr"].ConnectionString, string.Format("truncate table {0};", table), false, out resultClear);

            if (resultClear.Success)
            {
                resultClear.ErrForUser = string.Format("Table: {0} was truncated successfully!", table);
            }
            else {
                resultClear.Success = false;
                resultClear.ErrForUser = string.Format("There was an error truncating Table: {0}.<br/>{1}<hr/>", table, resultClear.ErrForUser);
            }
        }
    }
}