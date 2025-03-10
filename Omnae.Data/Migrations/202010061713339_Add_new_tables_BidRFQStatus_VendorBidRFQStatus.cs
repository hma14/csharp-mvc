namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_new_tables_BidRFQStatus_VendorBidRFQStatus : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BidRFQStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        CustomerId = c.Int(nullable: false),
                        StateId = c.Int(nullable: false),
                        TaskId = c.Int(nullable: false),
                        SubmittedVendors = c.Int(nullable: false),
                        TotalVendors = c.Int(nullable: false),
                        PartRevisionId = c.Int(),
                        RevisionCycle = c.Int(),
                        _updatedAt = c.DateTime(nullable: false),
                        _createdAt = c.DateTime(nullable: false),
                        CreatedByUserId = c.String(maxLength: 128),
                        ModifiedByUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedByUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedByUserId)
                .ForeignKey("dbo.PartRevisions", t => t.PartRevisionId)
                .Index(t => t.PartRevisionId)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.ModifiedByUserId);
            
            CreateTable(
                "dbo.VendorBidRFQStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        VendorId = c.Int(nullable: false),
                        StateId = c.Int(nullable: false),
                        TaskId = c.Int(nullable: false),
                        RejectRFQReason = c.String(),
                        BidRequestRevisionId = c.Int(),
                        BidRFQStatusId = c.Int(),
                        _updatedAt = c.DateTime(nullable: false),
                        _createdAt = c.DateTime(nullable: false),
                        CreatedByUserId = c.String(maxLength: 128),
                        ModifiedByUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BidRequestRevisions", t => t.BidRequestRevisionId)
                .ForeignKey("dbo.BidRFQStatus", t => t.BidRFQStatusId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedByUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedByUserId)
                .Index(t => t.BidRequestRevisionId)
                .Index(t => t.BidRFQStatusId)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.ModifiedByUserId);
            
            AddColumn("dbo.Documents", "BidRequestRevisionId", c => c.Int());
            AddColumn("dbo.Documents", "PartRevisionId", c => c.Int());
            //AddColumn("dbo.Documents", "BidRFQStatus_Id", c => c.Int());
            CreateIndex("dbo.Documents", "BidRequestRevisionId");
            CreateIndex("dbo.Documents", "PartRevisionId");
            //CreateIndex("dbo.Documents", "BidRFQStatus_Id");
            AddForeignKey("dbo.Documents", "BidRequestRevisionId", "dbo.BidRequestRevisions", "Id");
            AddForeignKey("dbo.Documents", "PartRevisionId", "dbo.PartRevisions", "Id");
            //AddForeignKey("dbo.Documents", "BidRFQStatus_Id", "dbo.BidRFQStatus", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VendorBidRFQStatus", "ModifiedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.VendorBidRFQStatus", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.VendorBidRFQStatus", "BidRFQStatusId", "dbo.BidRFQStatus");
            DropForeignKey("dbo.VendorBidRFQStatus", "BidRequestRevisionId", "dbo.BidRequestRevisions");
            DropForeignKey("dbo.Documents", "BidRFQStatus_Id", "dbo.BidRFQStatus");
            DropForeignKey("dbo.BidRFQStatus", "PartRevisionId", "dbo.PartRevisions");
            DropForeignKey("dbo.BidRFQStatus", "ModifiedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.BidRFQStatus", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Documents", "PartRevisionId", "dbo.PartRevisions");
            DropForeignKey("dbo.Documents", "BidRequestRevisionId", "dbo.BidRequestRevisions");
            DropIndex("dbo.VendorBidRFQStatus", new[] { "ModifiedByUserId" });
            DropIndex("dbo.VendorBidRFQStatus", new[] { "CreatedByUserId" });
            DropIndex("dbo.VendorBidRFQStatus", new[] { "BidRFQStatusId" });
            DropIndex("dbo.VendorBidRFQStatus", new[] { "BidRequestRevisionId" });
            DropIndex("dbo.BidRFQStatus", new[] { "ModifiedByUserId" });
            DropIndex("dbo.BidRFQStatus", new[] { "CreatedByUserId" });
            DropIndex("dbo.BidRFQStatus", new[] { "PartRevisionId" });
            DropIndex("dbo.Documents", new[] { "BidRFQStatus_Id" });
            DropIndex("dbo.Documents", new[] { "PartRevisionId" });
            DropIndex("dbo.Documents", new[] { "BidRequestRevisionId" });
            DropColumn("dbo.Documents", "BidRFQStatus_Id");
            DropColumn("dbo.Documents", "PartRevisionId");
            DropColumn("dbo.Documents", "BidRequestRevisionId");
            DropTable("dbo.VendorBidRFQStatus");
            DropTable("dbo.BidRFQStatus");
        }
    }
}
