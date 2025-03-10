namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Remove_Identity__createdAt_VendorRejectRFQ : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.VendorRejectRFQs", "_createdAt", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.VendorRejectRFQs", "_createdAt", c => c.DateTime(nullable: false));
        }
    }
}
