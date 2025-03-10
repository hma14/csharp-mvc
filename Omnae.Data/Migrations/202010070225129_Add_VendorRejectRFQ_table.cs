namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_VendorRejectRFQ_table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VendorRejectRFQs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        VendorId = c.Int(nullable: false),
                        Reason = c.String(),
                        Description = c.String(),
                        _updatedAt = c.DateTime(nullable: false),
                        _createdAt = c.DateTime(nullable: false),
                        CreatedByUserId = c.String(maxLength: 128),
                        ModifiedByUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedByUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedByUserId)
                .Index(t => new { t.ProductId, t.VendorId }, unique: true, name: "IX_VendorRejectRFQ")
                .Index(t => t.CreatedByUserId)
                .Index(t => t.ModifiedByUserId);
            
            AddColumn("dbo.VendorBidRFQStatus", "VendorRejectRFQId", c => c.Int());
            CreateIndex("dbo.VendorBidRFQStatus", "VendorRejectRFQId");
            AddForeignKey("dbo.VendorBidRFQStatus", "VendorRejectRFQId", "dbo.VendorRejectRFQs", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VendorBidRFQStatus", "VendorRejectRFQId", "dbo.VendorRejectRFQs");
            DropForeignKey("dbo.VendorRejectRFQs", "ModifiedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.VendorRejectRFQs", "CreatedByUserId", "dbo.AspNetUsers");
            DropIndex("dbo.VendorRejectRFQs", new[] { "ModifiedByUserId" });
            DropIndex("dbo.VendorRejectRFQs", new[] { "CreatedByUserId" });
            DropIndex("dbo.VendorRejectRFQs", "IX_VendorRejectRFQ");
            DropIndex("dbo.VendorBidRFQStatus", new[] { "VendorRejectRFQId" });
            DropColumn("dbo.VendorBidRFQStatus", "VendorRejectRFQId");
            DropTable("dbo.VendorRejectRFQs");
        }
    }
}
