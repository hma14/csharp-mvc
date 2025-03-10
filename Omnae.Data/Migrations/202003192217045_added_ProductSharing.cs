namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_ProductSharing : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductSharings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SharingCompanyId = c.Int(nullable: false),
                        OwnerCompanyId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        CreatedUtc = c.DateTime(),
                        ModifiedUtc = c.DateTime(),
                        HasPermissionToOrder = c.Boolean(),
                        IsRevoked = c.Boolean(),
                        CreatedByUserId = c.String(maxLength: 128),
                        ModifiedByUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedByUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedByUserId)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.Companies", t => t.OwnerCompanyId, cascadeDelete: false)
                .ForeignKey("dbo.Companies", t => t.SharingCompanyId, cascadeDelete: false)
                .Index(t => t.SharingCompanyId)
                .Index(t => t.OwnerCompanyId)
                .Index(t => t.ProductId)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.ModifiedByUserId);
            
            AddColumn("dbo.Orders", "ProductSharingId", c => c.Int());
            CreateIndex("dbo.Orders", "ProductSharingId");
            AddForeignKey("dbo.Orders", "ProductSharingId", "dbo.ProductSharings", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "ProductSharingId", "dbo.ProductSharings");
            DropForeignKey("dbo.ProductSharings", "SharingCompanyId", "dbo.Companies");
            DropForeignKey("dbo.ProductSharings", "OwnerCompanyId", "dbo.Companies");
            DropForeignKey("dbo.ProductSharings", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ProductSharings", "ModifiedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProductSharings", "CreatedByUserId", "dbo.AspNetUsers");
            DropIndex("dbo.ProductSharings", new[] { "ModifiedByUserId" });
            DropIndex("dbo.ProductSharings", new[] { "CreatedByUserId" });
            DropIndex("dbo.ProductSharings", new[] { "ProductId" });
            DropIndex("dbo.ProductSharings", new[] { "OwnerCompanyId" });
            DropIndex("dbo.ProductSharings", new[] { "SharingCompanyId" });
            DropIndex("dbo.Orders", new[] { "ProductSharingId" });
            DropColumn("dbo.Orders", "ProductSharingId");
            DropTable("dbo.ProductSharings");
        }
    }
}
