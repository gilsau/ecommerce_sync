/*
use gtechapp_toysandgamesroom
go

use gtechapp_hisandherscloset
go

use gtechapp_funmodernelectronics
go

use gtechapp_outdoorsfungear
go

use gtechapp_homeandgardenstatues
go

*/

select wp1.sku, wp1.imgUrl, wp1.[name], wp1.[description], categories = STUFF((SELECT ',' + wp2.[Url] + '<br/>' + wp2.SiteParentCategory + '~' + wp2.SiteSubCategory + '<hr/>' FROM webproduct wp2 WHERE wp2.sku = wp1.sku FOR XML PATH('')), 1, 1, '') from webproduct wp1 where SiteSubCategoryId=1 and WebsiteId=62 group by wp1.sku, wp1.imgUrl, wp1.[name], wp1.[description]




use gtechapp_ecommerce
go

-- copy products to ecommerce db
print '--------------------------------------------------'
print 'Drop/Create Table: WebProduct'
if exists(select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME='webproduct')
begin
	drop table webproduct;
end
go
create table WebProduct
(
	Id int not null identity(1,1) primary key,
	ImgUrl nvarchar(100) not null,
	Sku nvarchar(50) not null,
	[Name] nvarchar(100) not null,
	[Description] nvarchar(4000) not null,
	SiteParentCategoryId int not null,
	SiteParentCategory nvarchar(100) not null,
	SiteSubCategoryId int not null,
	SiteSubCategory nvarchar(100) not null,
	WebsiteId int not null,
	[Url] nvarchar(100) not null,
	Created datetime not null
)
go

print '--------------------------------------------------'
print 'Populate Table: Web Product'
insert into WebProduct

select pi.imageUrl, p1.sku, p1.name, p1.shortdescription, c2.id, c2.name, c1.id, c1.name, WebsiteId=(select id from website where url='http://www.funmodernelectronics.com'), Url='http://www.funmodernelectronics.com', getdate() 
from gtechapp_funmodernelectronics.dbo.product p1
inner join gtechapp_ecommerce.gtechapp_admin.productimport pi on pi.itemnum=p1.sku
inner join gtechapp_funmodernelectronics.dbo.product_category_mapping pcm1 on pcm1.productid=p1.id
inner join gtechapp_funmodernelectronics.dbo.category c1 on c1.id=pcm1.categoryid
inner join gtechapp_funmodernelectronics.dbo.category c2 on c2.id=c1.parentcategoryid
union
select pi.imageUrl, p1.sku, p1.name, p1.shortdescription, c2.id, c2.name, c1.id, c1.name, WebsiteId=(select id from website where url='http://www.outdoorsfungear.com'), Url='http://www.outdoorsfungear.com', getdate() 
from gtechapp_outdoorsfungear.dbo.product p1
inner join gtechapp_ecommerce.gtechapp_admin.productimport pi on pi.itemnum=p1.sku
inner join gtechapp_outdoorsfungear.dbo.product_category_mapping pcm1 on pcm1.productid=p1.id
inner join gtechapp_outdoorsfungear.dbo.category c1 on c1.id=pcm1.categoryid
inner join gtechapp_outdoorsfungear.dbo.category c2 on c2.id=c1.parentcategoryid
union
select pi.imageUrl, p1.sku, p1.name, p1.shortdescription, c2.id, c2.name, c1.id, c1.name, WebsiteId=(select id from website where url='http://www.toysandgamesroom.com'), Url='http://www.toysandgamesroom.com', getdate() 
from gtechapp_toysandgamesroom.dbo.product p1
inner join gtechapp_ecommerce.gtechapp_admin.productimport pi on pi.itemnum=p1.sku
inner join gtechapp_toysandgamesroom.dbo.product_category_mapping pcm1 on pcm1.productid=p1.id
inner join gtechapp_toysandgamesroom.dbo.category c1 on c1.id=pcm1.categoryid
inner join gtechapp_toysandgamesroom.dbo.category c2 on c2.id=c1.parentcategoryid
union
select pi.imageUrl, p1.sku, p1.name, p1.shortdescription, c2.id, c2.name, c1.id, c1.name, WebsiteId=(select id from website where url='http://www.homeandgardenstatues.com'), Url='http://www.homeandgardenstatues.com', getdate() 
from gtechapp_homeandgardenstatues.dbo.product p1
inner join gtechapp_ecommerce.gtechapp_admin.productimport pi on pi.itemnum=p1.sku
inner join gtechapp_homeandgardenstatues.dbo.product_category_mapping pcm1 on pcm1.productid=p1.id
inner join gtechapp_homeandgardenstatues.dbo.category c1 on c1.id=pcm1.categoryid
inner join gtechapp_homeandgardenstatues.dbo.category c2 on c2.id=c1.parentcategoryid
union
select pi.imageUrl, p1.sku, p1.name, p1.shortdescription, c2.id, c2.name, c1.id, c1.name, WebsiteId=(select id from website where url='http://www.hisandherscloset.com'), Url='http://www.hisandherscloset.com', getdate() 
from gtechapp_hisandherscloset.dbo.product p1
inner join gtechapp_ecommerce.gtechapp_admin.productimport pi on pi.itemnum=p1.sku
inner join gtechapp_hisandherscloset.dbo.product_category_mapping pcm1 on pcm1.productid=p1.id
inner join gtechapp_hisandherscloset.dbo.category c1 on c1.id=pcm1.categoryid
inner join gtechapp_hisandherscloset.dbo.category c2 on c2.id=c1.parentcategoryid
go
