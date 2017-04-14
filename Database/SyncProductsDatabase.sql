use Ecommerce
go

-- drop all foreign keys for all tables
if exists(select 1 from INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS)
begin
	declare @tblProd table(rowNum int, constraint_name nvarchar(50), table_name nvarchar(50))

	-- get all constraints	
	insert into @tblProd(rowNum, constraint_name, table_name)
	select  ROW_NUMBER() OVER(order by rc.constraint_name) rownumber, rc.CONSTRAINT_NAME, ctu.TABLE_NAME
	from INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS rc
	inner join INFORMATION_SCHEMA.CONSTRAINT_TABLE_USAGE ctu on ctu.CONSTRAINT_NAME = rc.CONSTRAINT_NAME

	-- loop thru constraints and drop them
	declare @nextRow int, @nextConstraintName nvarchar(50), @nextTableName nvarchar(50), @sql nvarchar(max)
	select top 1 @nextRow=rowNum, @nextConstraintName=constraint_name, @nextTableName=table_name from @tblProd order by rowNum
	while(@nextRow <= (select COUNT(*) from @tblProd))
	begin
		select @nextConstraintName=constraint_name, @nextTableName=table_name from @tblProd where rowNum=@nextRow
		exec('alter table ' + @nextTableName + ' drop constraint ' + @nextConstraintName)
		set @nextRow = @nextRow + 1
	end
end
go

if exists(select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME = N'Website')
begin
	drop table Website
end
go

create table Website
(
	Id int identity(1,1) not null primary key,
	Name nvarchar(100) not null,
	Url nvarchar(100) not null,
	Abbrev nvarchar(5) not null,
	[Server] nvarchar(50) not null,
	[Database] nvarchar(50) not null,
	Username nvarchar(50) not null,
	[Password] nvarchar(50) not null
)
go
insert into Website values('Fun Modern Electronics', 'http://www.FunModernElectronics.com', 'FME', 'xdbs5.dailyrazor.com', 'techstar_funmodernelectro', 'techstar_admin', 'Xmod350!')
insert into Website values('His & Hers Closet', 'http://www.HisAndHersCloset.com', 'HHC', 'xdbs5.dailyrazor.com', 'techstar_hishers', 'techstar_admin', 'Xmod350!')
insert into Website values('Home & Garden Statues', 'http://www.HomeAndGardenStatues.com', 'HGS', 'xdbs5.dailyrazor.com', 'techstar_homegarden', 'techstar_admin', 'Xmod350!')
insert into Website values('Outdoors Fun Gear', 'http://www.OutdoorsFunGear.com', 'OFG', 'xdbs5.dailyrazor.com', 'techstar_outdoors', 'techstar_admin', 'Xmod350!')
insert into Website values('Toys & Games Room', 'http://www.ToysAndGamesRoom.com', 'TGR', 'xdbs5.dailyrazor.com', 'techstar_toysgames', 'techstar_admin', 'Xmod350!')
go

if exists(select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME = N'Product')
begin
	drop table Product
end
go

create table Product
(
	ItemNum nvarchar(50) not null primary key,
	ProductName nvarchar(250) null,	
	ProductDescription text null,	
	ProductDescriptionOrignalHtml text null,		
	Brand nvarchar(500) null,
	Category1 nvarchar(500) null,	
	Category2 nvarchar(500) null,	
	Category3 nvarchar(500) null,	
	Category4 nvarchar(500) null,
	RetailPrice nvarchar(50) null,
	WholesalePriceBeforeMarkup nvarchar(50) null,
	MAPPrice nvarchar(50) null,
	SupplierHandlingFee nvarchar(50) null,
	[Weight] nvarchar(50) null,
	QtyInStock nvarchar(50) null,
	InStock nvarchar(50) null,
	UPC nvarchar(500) null,
	ImageName nvarchar(500) null,
	URLToLargeImage nvarchar(500) null,
	URLToThumbImage nvarchar(500) null,
	RefurbishedFlag nvarchar(5) null,
	AttributeList nvarchar(max) null,
	ShippingForUSA nvarchar(50) null,
	ManufacturerProdId nvarchar(500) null,
	ShippingLength nvarchar(50) null,
	ShippingWidth nvarchar(50) null,
	ShippingHeight nvarchar(50) null,
	SalesTaxPct nvarchar(50) null,
	SalesTaxState nvarchar(50) null,
	Manufacturer nvarchar(500) null,
	ShippingForCanada nvarchar(50) null,
	ShippingForInternational nvarchar(50) null,
	W2bHandlingFee nvarchar(500) null,
	W2B3PctTransactionFeeUSA nvarchar(50) null,
	W2B3PctTransactionFeeCanada nvarchar(50) null,
	W2B3PctTransactionFeeInternational nvarchar(50) null,
	USAZONE nvarchar(50) null,
	CANADAZONE nvarchar(50) null,
	Attributes nvarchar(max) null,
	ExtraImg1 nvarchar(500) null,
	ExtraImg2 nvarchar(500) null,
	ExtraImg3 nvarchar(500) null,
	ExtraImg4 nvarchar(500) null,
	ExtraImg5 nvarchar(500) null,
	ExtraImg6 nvarchar(500) null,
	DescriptionNoHtml text null,
	SupplierNameItemNumber nvarchar(50) null,
	SupplierCategory1 nvarchar(500) null,
	SupplierCategory2 nvarchar(500) null,
	SupplierCategory3 nvarchar(500) null,
	SupplierCategory4 nvarchar(500) null
)
go

if exists(select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME = N'WebsiteProduct')
begin
	drop table WebsiteProduct
end
go

create table WebsiteProduct
(
	Id int identity(1,1) not null primary key,
	WebsiteId int not null constraint fk_WebsiteProduct_Website references Website(Id),
	ProductId nvarchar(50) not null constraint fk_WebsiteProduct_Product references Product(ItemNum)
)
go

if exists(select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME = N'ProductImport')
begin
	drop table ProductImport
end
go

create table ProductImport
(
	ItemNum nvarchar(50) not null primary key,
	ProductName nvarchar(250) null,	
	ProductDescription text null,	
	ProductDescriptionOrignalHtml text null,		
	Brand nvarchar(500) null,
	Category1 nvarchar(500) null,	
	Category2 nvarchar(500) null,	
	Category3 nvarchar(500) null,	
	Category4 nvarchar(500) null,
	RetailPrice nvarchar(50) null,
	WholesalePriceBeforeMarkup nvarchar(50) null,
	MAPPrice nvarchar(50) null,
	SupplierHandlingFee nvarchar(50) null,
	[Weight] nvarchar(50) null,
	QtyInStock nvarchar(50) null,
	InStock nvarchar(50) null,
	UPC nvarchar(500) null,
	ImageName nvarchar(500) null,
	URLToLargeImage nvarchar(500) null,
	URLToThumbImage nvarchar(500) null,
	RefurbishedFlag nvarchar(5) null,
	AttributeList nvarchar(max) null,
	ShippingForUSA nvarchar(50) null,
	ManufacturerProdId nvarchar(500) null,
	ShippingLength nvarchar(50) null,
	ShippingWidth nvarchar(50) null,
	ShippingHeight nvarchar(50) null,
	SalesTaxPct nvarchar(50) null,
	SalesTaxState nvarchar(50) null,
	Manufacturer nvarchar(500) null,
	ShippingForCanada nvarchar(50) null,
	ShippingForInternational nvarchar(50) null,
	W2bHandlingFee nvarchar(500) null,
	W2B3PctTransactionFeeUSA nvarchar(50) null,
	W2B3PctTransactionFeeCanada nvarchar(50) null,
	W2B3PctTransactionFeeInternational nvarchar(50) null,
	USAZONE nvarchar(50) null,
	CANADAZONE nvarchar(50) null,
	Attributes nvarchar(max) null,
	ExtraImg1 nvarchar(500) null,
	ExtraImg2 nvarchar(500) null,
	ExtraImg3 nvarchar(500) null,
	ExtraImg4 nvarchar(500) null,
	ExtraImg5 nvarchar(500) null,
	ExtraImg6 nvarchar(500) null,
	DescriptionNoHtml text null,
	SupplierNameItemNumber nvarchar(50) null,
	SupplierCategory1 nvarchar(500) null,
	SupplierCategory2 nvarchar(500) null,
	SupplierCategory3 nvarchar(500) null,
	SupplierCategory4 nvarchar(500) null
)
go


sp_tables



select ROW_NUMBER() over (order by Category1, Category2, Category3) as RowNum, Category1, Category2, Category3, Products=COUNT(productname) 
from productimport 
where ltrim(rtrim(suppliercategory1)) <> '' 
group by Category1, Category2, Category3 
order by Category1, Category2, Category3 


select * from website where id = 1

/*
select *
from websiteproduct wp
inner join Website w on w.Id = wp.WebsiteId
inner join Product p on p.ItemNum = wp.ProductId

-- TODO: add WEBSITES to select list, with web memberships and product counts
select 
ROW_NUMBER() over (order by Category1, Category2, Category3) as RowNum, 
Websites = (SELECT stuff((
    SELECT top 10 ', ' + cast(itemnum as varchar(max))
    FROM productimport
    FOR XML PATH('')
    ), 1, 2, '')
),
Category1, 
Category2, 
Category3, 
Products=COUNT(productname) 
from productimport 
where ltrim(rtrim(suppliercategory1)) <> '' 
group by Category1, Category2, Category3 
order by Category1, Category2, Category3 

select * from product

*/