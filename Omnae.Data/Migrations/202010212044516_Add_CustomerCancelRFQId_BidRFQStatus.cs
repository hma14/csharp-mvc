namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_CustomerCancelRFQId_BidRFQStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BidRFQStatus", "CustomerCancelRFQId", c => c.Int());
            CreateIndex("dbo.BidRFQStatus", "CustomerCancelRFQId");
            AddForeignKey("dbo.BidRFQStatus", "CustomerCancelRFQId", "dbo.VendorRejectRFQs", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BidRFQStatus", "CustomerCancelRFQId", "dbo.VendorRejectRFQs");
            DropIndex("dbo.BidRFQStatus", new[] { "CustomerCancelRFQId" });
            DropColumn("dbo.BidRFQStatus", "CustomerCancelRFQId");
        }
    }
}
