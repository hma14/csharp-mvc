namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_ForeignKey_ApprovedCapability : DbMigration
    {
        public override void Up()
        {
            AddForeignKey("dbo.ApprovedCapabilities", "VendorId", "dbo.Companies", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApprovedCapabilities", "VendorId", "dbo.Companies");
        }
    }
}
