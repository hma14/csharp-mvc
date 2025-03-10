namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MultUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "CompanyId", c => c.Int(nullable: true));
            AddForeignKey("dbo.AspNetUsers", "CompanyId", "dbo.Companies", "Id", cascadeDelete: false);
            
            Sql(@"
               UPDATE u
               SET [CompanyId] = c.[CompanyId]
               FROM (SELECT [Id] as CompanyId, [UserId] FROM [dbo].[Companies] where [UserId] is not null) c
               INNER JOIN [dbo].[AspNetUsers] u ON u.[Id] = c.[UserId];");

            AddColumn("dbo.Companies", "CompanyType", c => c.Int(nullable: true));
            Sql(@"
               UPDATE c
               SET [CompanyType] = case u.UserType when 3 THEN 0 ELSE u.UserType END
               FROM [dbo].[Companies] c
               INNER JOIN [dbo].[AspNetUsers] u ON u.[Id] = c.[UserId];");


            //AddForeignKey("dbo.RFQBids", "ProductId", "dbo.Products", "Id", cascadeDelete: false);
            Sql(@"ALTER TABLE [dbo].[RFQBids] WITH NOCHECK 
                  ADD CONSTRAINT [FK_dbo.RFQBids_dbo.Products_ProductId] 
                  FOREIGN KEY ([ProductId]) 
                  REFERENCES [dbo].[Products] ([Id]);");

            //AddForeignKey("dbo.RFQBids", "VendorId", "dbo.Companies", "Id", cascadeDelete: false);
            Sql(@"ALTER TABLE [dbo].[RFQBids] WITH NOCHECK 
                  ADD CONSTRAINT [FK_dbo.RFQBids_dbo.Companies_VendorId] 
                  FOREIGN KEY ([VendorId]) 
                  REFERENCES [dbo].[Companies] ([Id]);");
            
            DropIndex("dbo.Companies", new[] { "UserId" });
            DropColumn("dbo.Companies", "UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Companies", "UserId", c => c.String(maxLength: 512));
            CreateIndex("dbo.Companies", "UserId");

            DropForeignKey("dbo.RFQBids", "VendorId", "dbo.Companies");
            DropForeignKey("dbo.RFQBids", "ProductId", "dbo.Products");
            
            DropForeignKey("dbo.AspNetUsers", "CompanyId", "dbo.Companies");
            DropIndex("dbo.AspNetUsers", new[] { "CompanyId" });
            
            DropColumn("dbo.Companies", "CompanyType");
            DropColumn("dbo.AspNetUsers", "CompanyId");
        }
    }
}
