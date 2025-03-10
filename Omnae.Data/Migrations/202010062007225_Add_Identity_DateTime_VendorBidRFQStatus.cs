namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Identity_DateTime_VendorBidRFQStatus : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BidRFQStatus", "_createdAt", c => c.DateTime(nullable: false));
            AlterColumn("dbo.VendorBidRFQStatus", "_updatedAt", c => c.DateTime(nullable: false));
            AlterColumn("dbo.VendorBidRFQStatus", "_createdAt", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.VendorBidRFQStatus", "_createdAt", c => c.DateTime());
            AlterColumn("dbo.VendorBidRFQStatus", "_updatedAt", c => c.DateTime());
            AlterColumn("dbo.BidRFQStatus", "_createdAt", c => c.DateTime());
        }
    }
}
