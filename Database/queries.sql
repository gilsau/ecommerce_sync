use techstar_funmodernelectro
go

if not exists(select 1 from category where name = 'computers' and ParentCategoryId = 0)
begin
	insert into category values('Computers', NULL, 1, NULL, NULL, NULL, 0, 1, 6, 1, '6, 3, 9', NULL, 0, 1, 0, 0, 1, 0, 1, getdate(), getdate())
end
else
begin
	update category set deleted = 0 where Name = 'computers'
end

/*
delete category where id in (21,22,23)
update Category set Deleted = 1
*/

select * from category order by name
where parentcategoryid = 0


select COLUMN_NAME, DATA_TYPE
from INFORMATION_SCHEMA.COLUMNS
where TABLE_NAME = 'category'
and IS_NULLABLE = 'NO'

------------------------------------------------------------
select prodid=p.id, product=p.name, category=c.name, * 
from product p
inner join product_category_mapping pcm on pcm.productid = p.id
inner join category c on c.id = pcm.categoryid
order by c.parentcategoryid, c.name desc

select * from product where ID=2

insert into product values(
5,0,1,'Product 1','This is the description for Product 1<hr/>Testing html<hr/>',NULL,NULL,1,0,0,NULL,NULL,NULL,1,5,0,1,0,0,0,'DS_VA3_PC',NULL,NULL,0,0,NULL,0,NULL,0,0,0,0,0,NULL,0,0,0,0,NULL,0,0,0,0,0,0,0,1,0,0,0.0000,0,0,2,0,1,0,0,10000,1,0,0,1,1,0,0,1,10000,NULL,0,0,0,0,0,NULL,0,1259.0000,0.0000,0.0000,NULL,NULL,NULL,0,0.0000,0.0000,0,0.0000,0,0.0000,0,0,NULL,NULL,0,0,7.0000,7.0000,7.0000,7.0000,NULL,NULL,0,1,0,GETDATE(),GETDATE()
)

select COLUMN_NAME, DATA_TYPE
from INFORMATION_SCHEMA.COLUMNS
where TABLE_NAME = 'product'
and IS_NULLABLE = 'NO'


------------------------------------------------------------
select * from Product_Category_Mapping
select COLUMN_NAME, DATA_TYPE
from INFORMATION_SCHEMA.COLUMNS
where TABLE_NAME = 'Product_Category_Mapping'
and IS_NULLABLE = 'NO'

