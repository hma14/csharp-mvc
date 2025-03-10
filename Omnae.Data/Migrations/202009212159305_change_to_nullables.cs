namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class change_to_nullables : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Products", "RFQQuantityId", "dbo.RFQQuantities");
            DropForeignKey("dbo.PriceBreaks", "RFQBidId", "dbo.RFQBids");
            DropIndex("dbo.Products", new[] { "RFQQuantityId" });
            DropIndex("dbo.PriceBreaks", "IX_PriceBreak");
            AlterColumn("dbo.Products", "RFQQuantityId", c => c.Int());
            AlterColumn("dbo.PriceBreaks", "RFQBidId", c => c.Int());
            AlterColumn("dbo.RFQBids", "RFQQuantityId", c => c.Int());
            CreateIndex("dbo.Products", "RFQQuantityId");
            CreateIndex("dbo.PriceBreaks", new[] { "RFQBidId", "ProductId", "Quantity", "TaskId" }, unique: true, name: "IX_PriceBreak");
            Sql(@"UPDATE [dbo].[Products] SET [RFQQuantityId] = null WHERE [RFQQuantityId] = 0;");
            Sql(@"UPDATE [dbo].[PriceBreaks] SET [RFQBidId] = null WHERE [RFQBidId] = 0;");
            AddForeignKey("dbo.Products", "RFQQuantityId", "dbo.RFQQuantities", "Id");
            AddForeignKey("dbo.PriceBreaks", "RFQBidId", "dbo.RFQBids", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PriceBreaks", "RFQBidId", "dbo.RFQBids");
            DropForeignKey("dbo.Products", "RFQQuantityId", "dbo.RFQQuantities");
            DropIndex("dbo.PriceBreaks", "IX_PriceBreak");
            DropIndex("dbo.Products", new[] { "RFQQuantityId" });
            AlterColumn("dbo.RFQBids", "RFQQuantityId", c => c.Int(nullable: false));
            AlterColumn("dbo.PriceBreaks", "RFQBidId", c => c.Int(nullable: false));
            AlterColumn("dbo.Products", "RFQQuantityId", c => c.Int(nullable: false));
            CreateIndex("dbo.PriceBreaks", new[] { "RFQBidId", "ProductId", "Quantity", "TaskId" }, unique: true, name: "IX_PriceBreak");
            CreateIndex("dbo.Products", "RFQQuantityId");
            AddForeignKey("dbo.PriceBreaks", "RFQBidId", "dbo.RFQBids", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Products", "RFQQuantityId", "dbo.RFQQuantities", "Id", cascadeDelete: true);
        }
    }
}
