namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_new_tables_BidRFQStatus_VendorBidRFQStatus : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Documents", "BidRFQStatus_Id", "dbo.BidRFQStatus");
            DropIndex("dbo.Documents", new[] { "BidRFQStatus_Id" });
            DropColumn("dbo.Documents", "BidRFQStatus_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Documents", "BidRFQStatus_Id", c => c.Int());
            CreateIndex("dbo.Documents", "BidRFQStatus_Id");
            AddForeignKey("dbo.Documents", "BidRFQStatus_Id", "dbo.BidRFQStatus", "Id");
        }
    }
}
