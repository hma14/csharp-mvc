namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_VendorRejectRFQId_BidRequestRevision : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BidRequestRevisions", "VendorRejectRFQId", c => c.Int());
            CreateIndex("dbo.BidRequestRevisions", "VendorRejectRFQId");
            AddForeignKey("dbo.BidRequestRevisions", "VendorRejectRFQId", "dbo.VendorRejectRFQs", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BidRequestRevisions", "VendorRejectRFQId", "dbo.VendorRejectRFQs");
            DropIndex("dbo.BidRequestRevisions", new[] { "VendorRejectRFQId" });
            DropColumn("dbo.BidRequestRevisions", "VendorRejectRFQId");
        }
    }
}
