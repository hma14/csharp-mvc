namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_KeepCurrentRevisionReason_BidRFQStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BidRFQStatus", "KeepCurrentRevisionReason", c => c.String());
            DropColumn("dbo.BidRequestRevisions", "RevisionReason");
            DropColumn("dbo.VendorBidRFQStatus", "RejectRFQReason");
        }
        
        public override void Down()
        {
            AddColumn("dbo.VendorBidRFQStatus", "RejectRFQReason", c => c.String());
            AddColumn("dbo.BidRequestRevisions", "RevisionReason", c => c.String());
            DropColumn("dbo.BidRFQStatus", "KeepCurrentRevisionReason");
        }
    }
}
