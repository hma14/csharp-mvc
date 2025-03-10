namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShippingAccountAndProfile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShippingProfiles", "ShippingAccountId", c => c.Int());
            CreateIndex("dbo.ShippingAccounts", "CompanyId");
            CreateIndex("dbo.ShippingProfiles", "ShippingAccountId");

            AddForeignKey("dbo.ShippingAccounts", "CompanyId", "dbo.Companies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.ShippingProfiles", "ShippingAccountId", "dbo.ShippingAccounts", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShippingProfiles", "ShippingAccountId", "dbo.ShippingAccounts");
            DropForeignKey("dbo.ShippingAccounts", "CompanyId", "dbo.Companies");

            DropIndex("dbo.ShippingProfiles", new[] { "ShippingAccountId" });
            DropIndex("dbo.ShippingAccounts", new[] { "CompanyId" });
            DropColumn("dbo.ShippingProfiles", "ShippingAccountId");
        }
    }
}
