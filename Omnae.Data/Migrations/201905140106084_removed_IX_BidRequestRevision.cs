namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removed_IX_BidRequestRevision : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.BidRequestRevisions", "IX_BidRequestRevision");
        }
        
        public override void Down()
        {
            CreateIndex("dbo.BidRequestRevisions", new[] { "VendorId", "ProductId", "TaskId" }, unique: true, name: "IX_BidRequestRevision");
        }
    }
}
