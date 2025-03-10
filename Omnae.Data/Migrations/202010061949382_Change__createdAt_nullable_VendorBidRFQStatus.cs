namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Change__createdAt_nullable_VendorBidRFQStatus : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.VendorBidRFQStatus", "_updatedAt", c => c.DateTime());
            AlterColumn("dbo.VendorBidRFQStatus", "_createdAt", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.VendorBidRFQStatus", "_createdAt", c => c.DateTime(nullable: false));
            AlterColumn("dbo.VendorBidRFQStatus", "_updatedAt", c => c.DateTime(nullable: false));
        }
    }
}
