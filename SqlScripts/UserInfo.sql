


SELECT c.name, u.email, u.phonenumber, u.phonenumberconfirmed, a.AddressLine1, a.City  FROM [dbo].[AspNetUsers] u
inner join [dbo].[Companies] c on c.UserId = u.Id
inner join [dbo].[Addresses] a on a.Id = c.AddressId