namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewIndexAtApprovedCapability : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ApprovedCapabilities", new[] { "VendorId" });
            AddColumn("dbo.BidRequestRevisions", "CreateDateTime", c => c.DateTime());
            AddColumn("dbo.ExtraQuantities", "NumberSampleIncluded", c => c.Int());
            AlterColumn("dbo.Companies", "Name", c => c.String(nullable: false, maxLength: 150));
            CreateIndex("dbo.ApprovedCapabilities", "VendorId");
            CreateIndex("dbo.Products", "RFQQuantityId");
            CreateIndex("dbo.NCReports", "OrderId");
            AddForeignKey("dbo.Products", "RFQQuantityId", "dbo.RFQQuantities", "Id", cascadeDelete: true);
            AddForeignKey("dbo.NCReports", "OrderId", "dbo.Orders", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NCReports", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Products", "RFQQuantityId", "dbo.RFQQuantities");
            DropIndex("dbo.NCReports", new[] { "OrderId" });
            DropIndex("dbo.Products", new[] { "RFQQuantityId" });
            DropIndex("dbo.ApprovedCapabilities", new[] { "VendorId" });
            AlterColumn("dbo.Companies", "Name", c => c.String(maxLength: 150));
            DropColumn("dbo.ExtraQuantities", "NumberSampleIncluded");
            DropColumn("dbo.BidRequestRevisions", "CreateDateTime");
            CreateIndex("dbo.ApprovedCapabilities", "VendorId");
        }
    }
}
