namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompanyAddresses : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Addresses", "isMainAddress", c => c.Boolean(nullable: false, defaultValue: false));
            AddColumn("dbo.Addresses", "CompanyId", c => c.Int(nullable: true));
            AddColumn("dbo.Companies", "MainCompanyAddress_Id", c => c.Int());

            Sql(@"
                UPDATE addr
                SET[CompanyId] = CompanyIdsForAddr.New_CompanyID
                FROM[dbo].[Addresses] addr
                    INNER JOIN(SELECT a.[Id] as AddressID, c.ID as New_CompanyID FROM[dbo].[Addresses] a INNER JOIN[dbo].[Companies] c ON(c.AddressId = a.Id OR c.BillAddressId = a.Id)) CompanyIdsForAddr
                        ON addr.[Id] = CompanyIdsForAddr.AddressID;");

            CreateIndex("dbo.Addresses", "CompanyId");
            CreateIndex("dbo.Companies", "MainCompanyAddress_Id");
            AddForeignKey("dbo.Companies", "MainCompanyAddress_Id", "dbo.Addresses", "Id");
            AddForeignKey("dbo.Addresses", "CompanyId", "dbo.Companies", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Addresses", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Companies", "MainCompanyAddress_Id", "dbo.Addresses");
            DropIndex("dbo.Companies", new[] { "MainCompanyAddress_Id" });
            DropIndex("dbo.Addresses", new[] { "CompanyId" });
            DropColumn("dbo.Companies", "MainCompanyAddress_Id");
            DropColumn("dbo.Addresses", "CompanyId");
            DropColumn("dbo.Addresses", "isMainAddress");
        }
    }
}
