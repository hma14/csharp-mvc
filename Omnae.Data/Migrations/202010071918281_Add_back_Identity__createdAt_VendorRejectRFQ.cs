namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_back_Identity__createdAt_VendorRejectRFQ : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.VendorRejectRFQs", "_createdAt", c => c.DateTime());
            DropColumn("dbo.VendorRejectRFQs", "_updatedAt");
        }
        
        public override void Down()
        {
            AddColumn("dbo.VendorRejectRFQs", "_updatedAt", c => c.DateTime());
            AlterColumn("dbo.VendorRejectRFQs", "_createdAt", c => c.DateTime());
        }
    }
}
