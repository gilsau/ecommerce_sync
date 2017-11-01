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

use gtechapp_ecommerce
go

-- copy products to ecommerce db
if exists(select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME='webproduct')
begin
	drop table webproduct;
end
go
create table WebProduct
(
	Id int not null identity(1,1) primary key,
	Sku nvarchar(50) not null,
	[Name] nvarchar(100) not null,
	[Description] nvarchar(4000) not null,
	SiteParentCategory nvarchar(100) not null,
	SiteSubCategory nvarchar(100) not null,
	WebsiteId int not null,
	[Url] nvarchar(100) not null,
	Created datetime not null
)
go

insert into WebProduct

select p1.sku, p1.name, p1.shortdescription, c2.name, c1.name, WebsiteId=(select id from website where url='http://www.funmodernelectronics.com'), Url='http://www.funmodernelectronics.com', getdate() 
from gtechapp_funmodernelectronics.dbo.product p1
inner join gtechapp_funmodernelectronics.dbo.product_category_mapping pcm1 on pcm1.productid=p1.id
inner join gtechapp_funmodernelectronics.dbo.category c1 on c1.id=pcm1.categoryid
inner join gtechapp_funmodernelectronics.dbo.category c2 on c2.id=c1.parentcategoryid
union
select p1.sku, p1.name, p1.shortdescription, c2.name, c1.name, WebsiteId=(select id from website where url='http://www.outdoorsfungear.com'), Url='http://www.outdoorsfungear.com', getdate() 
from gtechapp_outdoorsfungear.dbo.product p1
inner join gtechapp_outdoorsfungear.dbo.product_category_mapping pcm1 on pcm1.productid=p1.id
inner join gtechapp_outdoorsfungear.dbo.category c1 on c1.id=pcm1.categoryid
inner join gtechapp_outdoorsfungear.dbo.category c2 on c2.id=c1.parentcategoryid
union
select p1.sku, p1.name, p1.shortdescription, c2.name, c1.name, WebsiteId=(select id from website where url='http://www.toysandgamesroom.com'), Url='http://www.toysandgamesroom.com', getdate() 
from gtechapp_toysandgamesroom.dbo.product p1
inner join gtechapp_toysandgamesroom.dbo.product_category_mapping pcm1 on pcm1.productid=p1.id
inner join gtechapp_toysandgamesroom.dbo.category c1 on c1.id=pcm1.categoryid
inner join gtechapp_toysandgamesroom.dbo.category c2 on c2.id=c1.parentcategoryid
union
select p1.sku, p1.name, p1.shortdescription, c2.name, c1.name, WebsiteId=(select id from website where url='http://www.homeandgardenstatues.com'), Url='http://www.homeandgardenstatues.com', getdate() 
from gtechapp_homeandgardenstatues.dbo.product p1
inner join gtechapp_homeandgardenstatues.dbo.product_category_mapping pcm1 on pcm1.productid=p1.id
inner join gtechapp_homeandgardenstatues.dbo.category c1 on c1.id=pcm1.categoryid
inner join gtechapp_homeandgardenstatues.dbo.category c2 on c2.id=c1.parentcategoryid
union
select p1.sku, p1.name, p1.shortdescription, c2.name, c1.name, WebsiteId=(select id from website where url='http://www.hisandherscloset.com'), Url='http://www.hisandherscloset.com', getdate() 
from gtechapp_hisandherscloset.dbo.product p1
inner join gtechapp_hisandherscloset.dbo.product_category_mapping pcm1 on pcm1.productid=p1.id
inner join gtechapp_hisandherscloset.dbo.category c1 on c1.id=pcm1.categoryid
inner join gtechapp_hisandherscloset.dbo.category c2 on c2.id=c1.parentcategoryid
go

select websiteid, url, siteparentcategory, sitesubcategory
from webproduct 
group by url, websiteid, siteparentcategory, sitesubcategory
order by url, websiteid, siteparentcategory, sitesubcategory
