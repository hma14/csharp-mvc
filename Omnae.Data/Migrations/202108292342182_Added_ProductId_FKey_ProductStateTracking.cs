namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_ProductId_FKey_ProductStateTracking : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.ProductStateTrackings", "ProductId");
            AddForeignKey("dbo.ProductStateTrackings", "ProductId", "dbo.Products", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductStateTrackings", "ProductId", "dbo.Products");
            DropIndex("dbo.ProductStateTrackings", new[] { "ProductId" });
        }
    }
}
