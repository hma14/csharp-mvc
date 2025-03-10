namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class created_index_unique_ProductSharing : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ProductSharings", new[] { "SharingCompanyId" });
            DropIndex("dbo.ProductSharings", new[] { "ProductId" });
            //AddColumn("dbo.Products", "_updatedAt", c => c.DateTime());
            CreateIndex("dbo.ProductSharings", new[] { "SharingCompanyId", "ProductId" }, unique: true, name: "IX_ProductSharing");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ProductSharings", "IX_ProductSharing");
            //DropColumn("dbo.Products", "_updatedAt");
            CreateIndex("dbo.ProductSharings", "ProductId");
            CreateIndex("dbo.ProductSharings", "SharingCompanyId");
        }
    }
}
