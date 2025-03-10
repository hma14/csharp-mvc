namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modifiedShippingTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Shippings", "CustomerAddressId", "dbo.Addresses");
            DropForeignKey("dbo.Shippings", "CustomerCompany_Id", "dbo.Companies");
            DropForeignKey("dbo.Shippings", "VendorAddressId", "dbo.Addresses");
            DropForeignKey("dbo.Shippings", "VendorCompany_Id", "dbo.Companies");
            DropIndex("dbo.Shippings", new[] { "CustomerAddressId" });
            DropIndex("dbo.Shippings", new[] { "VendorAddressId" });
            DropIndex("dbo.Shippings", new[] { "CustomerCompany_Id" });
            DropIndex("dbo.Shippings", new[] { "VendorCompany_Id" });
            AddColumn("dbo.Shippings", "CompanyId", c => c.Int());
            AddColumn("dbo.Shippings", "AddressId", c => c.Int());
            DropColumn("dbo.Shippings", "CustomerId");
            DropColumn("dbo.Shippings", "VendorId");
            DropColumn("dbo.Shippings", "CustomerAddressId");
            DropColumn("dbo.Shippings", "VendorAddressId");
            DropColumn("dbo.Shippings", "CustomerCompany_Id");
            DropColumn("dbo.Shippings", "VendorCompany_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Shippings", "VendorCompany_Id", c => c.Int());
            AddColumn("dbo.Shippings", "CustomerCompany_Id", c => c.Int());
            AddColumn("dbo.Shippings", "VendorAddressId", c => c.Int());
            AddColumn("dbo.Shippings", "CustomerAddressId", c => c.Int());
            AddColumn("dbo.Shippings", "VendorId", c => c.Int());
            AddColumn("dbo.Shippings", "CustomerId", c => c.Int());
            DropColumn("dbo.Shippings", "AddressId");
            DropColumn("dbo.Shippings", "CompanyId");
            CreateIndex("dbo.Shippings", "VendorCompany_Id");
            CreateIndex("dbo.Shippings", "CustomerCompany_Id");
            CreateIndex("dbo.Shippings", "VendorAddressId");
            CreateIndex("dbo.Shippings", "CustomerAddressId");
            AddForeignKey("dbo.Shippings", "VendorCompany_Id", "dbo.Companies", "Id");
            AddForeignKey("dbo.Shippings", "VendorAddressId", "dbo.Addresses", "Id");
            AddForeignKey("dbo.Shippings", "CustomerCompany_Id", "dbo.Companies", "Id");
            AddForeignKey("dbo.Shippings", "CustomerAddressId", "dbo.Addresses", "Id");
        }
    }
}
