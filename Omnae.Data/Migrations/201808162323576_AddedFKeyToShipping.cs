namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedFKeyToShipping : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Shippings", "AddressId");
            AddForeignKey("dbo.Shippings", "AddressId", "dbo.Addresses", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Shippings", "AddressId", "dbo.Addresses");
            DropIndex("dbo.Shippings", new[] { "AddressId" });
        }
    }
}
