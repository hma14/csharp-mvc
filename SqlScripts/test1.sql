/****** Script for SelectTopNRows command from SSMS  ******/
SELECT * FROM [dbo].[TaskDatas] where stateid=7
SELECT * FROM [dbo].[TaskDatas] where taskid=18
SELECT * FROM [dbo].[TaskDatas] where productid=4

update [dbo].[TaskDatas] set stateid=8 where stateid=10


update [dbo].[TaskDatas] set stateid=8 where productid=4


select * from [dbo].[Products] where id=4

select * from [dbo].[Shippings]

select * from [dbo].[Orders] where id=116

select * from [dbo].[Documents] where productid=20

select * from [dbo].[StateProvinces] 


-- Get address

select p.Name, ad.City, co.CountryName, ad.PostalCode, ad.ZipCode from [dbo].[Products] p
inner join [dbo].[Companies] c on p.CustomerId = c.id
inner join [dbo].[Shippings] s on s.CustomerId = c.id
inner join [dbo].[Addresses] ad on ad.id = s.CustomerAddressId
inner join [dbo].[Countries] co on ad.CountryId = co.Id
where p.id = 57