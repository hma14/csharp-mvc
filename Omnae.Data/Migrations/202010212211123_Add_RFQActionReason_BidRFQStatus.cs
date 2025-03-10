namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_RFQActionReason_BidRFQStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BidRFQStatus", "RFQActionReasonId", c => c.Int());
            CreateIndex("dbo.BidRFQStatus", "RFQActionReasonId");
            AddForeignKey("dbo.BidRFQStatus", "RFQActionReasonId", "dbo.RFQActionReasons", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BidRFQStatus", "RFQActionReasonId", "dbo.RFQActionReasons");
            DropIndex("dbo.BidRFQStatus", new[] { "RFQActionReasonId" });
            DropColumn("dbo.BidRFQStatus", "RFQActionReasonId");
        }
    }
}
