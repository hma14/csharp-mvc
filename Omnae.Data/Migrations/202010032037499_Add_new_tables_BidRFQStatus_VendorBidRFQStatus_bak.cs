namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_new_tables_BidRFQStatus_VendorBidRFQStatus : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Documents", name: "BidRFQStatusId", newName: "BidRFQStatus_Id");
            RenameIndex(table: "dbo.Documents", name: "IX_BidRFQStatusId", newName: "IX_BidRFQStatus_Id");
            AddColumn("dbo.BidRFQStatus", "_updatedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.BidRFQStatus", "_createdAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.BidRFQStatus", "CreatedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.BidRFQStatus", "ModifiedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.VendorBidRFQStatus", "_updatedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.VendorBidRFQStatus", "_createdAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.VendorBidRFQStatus", "CreatedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.VendorBidRFQStatus", "ModifiedByUserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.BidRFQStatus", "CreatedByUserId");
            CreateIndex("dbo.BidRFQStatus", "ModifiedByUserId");
            CreateIndex("dbo.VendorBidRFQStatus", "CreatedByUserId");
            CreateIndex("dbo.VendorBidRFQStatus", "ModifiedByUserId");
            AddForeignKey("dbo.BidRFQStatus", "CreatedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.BidRFQStatus", "ModifiedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.VendorBidRFQStatus", "CreatedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.VendorBidRFQStatus", "ModifiedByUserId", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.BidRFQStatus", "TimeStamp");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BidRFQStatus", "TimeStamp", c => c.DateTime(nullable: false));
            DropForeignKey("dbo.VendorBidRFQStatus", "ModifiedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.VendorBidRFQStatus", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.BidRFQStatus", "ModifiedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.BidRFQStatus", "CreatedByUserId", "dbo.AspNetUsers");
            DropIndex("dbo.VendorBidRFQStatus", new[] { "ModifiedByUserId" });
            DropIndex("dbo.VendorBidRFQStatus", new[] { "CreatedByUserId" });
            DropIndex("dbo.BidRFQStatus", new[] { "ModifiedByUserId" });
            DropIndex("dbo.BidRFQStatus", new[] { "CreatedByUserId" });
            DropColumn("dbo.VendorBidRFQStatus", "ModifiedByUserId");
            DropColumn("dbo.VendorBidRFQStatus", "CreatedByUserId");
            DropColumn("dbo.VendorBidRFQStatus", "_createdAt");
            DropColumn("dbo.VendorBidRFQStatus", "_updatedAt");
            DropColumn("dbo.BidRFQStatus", "ModifiedByUserId");
            DropColumn("dbo.BidRFQStatus", "CreatedByUserId");
            DropColumn("dbo.BidRFQStatus", "_createdAt");
            DropColumn("dbo.BidRFQStatus", "_updatedAt");
            RenameIndex(table: "dbo.Documents", name: "IX_BidRFQStatus_Id", newName: "IX_BidRFQStatusId");
            RenameColumn(table: "dbo.Documents", name: "BidRFQStatus_Id", newName: "BidRFQStatusId");
        }
    }
}
