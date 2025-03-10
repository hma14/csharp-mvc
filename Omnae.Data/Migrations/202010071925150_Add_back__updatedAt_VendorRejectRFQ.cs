namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_back__updatedAt_VendorRejectRFQ : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VendorRejectRFQs", "_updatedAt", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VendorRejectRFQs", "_updatedAt");
        }
    }
}
