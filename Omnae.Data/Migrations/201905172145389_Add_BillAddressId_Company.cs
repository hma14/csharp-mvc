namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_BillAddressId_Company : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "BillAddressId", c => c.Int());
            CreateIndex("dbo.Companies", "BillAddressId");
            AddForeignKey("dbo.Companies", "BillAddressId", "dbo.Addresses", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Companies", "BillAddressId", "dbo.Addresses");
            DropIndex("dbo.Companies", new[] { "BillAddressId" });
            DropColumn("dbo.Companies", "BillAddressId");
        }
    }
}
