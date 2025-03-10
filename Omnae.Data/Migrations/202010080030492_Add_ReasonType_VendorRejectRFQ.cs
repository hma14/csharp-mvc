namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_ReasonType_VendorRejectRFQ : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.VendorRejectRFQs", "IX_VendorRejectRFQ");
            AddColumn("dbo.VendorRejectRFQs", "ReasonType", c => c.Int());
            CreateIndex("dbo.VendorRejectRFQs", new[] { "ProductId", "VendorId", "ReasonType" }, unique: true, name: "IX_VendorRejectRFQ");
        }
        
        public override void Down()
        {
            DropIndex("dbo.VendorRejectRFQs", "IX_VendorRejectRFQ");
            DropColumn("dbo.VendorRejectRFQs", "ReasonType");
            CreateIndex("dbo.VendorRejectRFQs", new[] { "ProductId", "VendorId" }, unique: true, name: "IX_VendorRejectRFQ");
        }
    }
}
