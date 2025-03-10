namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_IX_PriceBreak_TaskId_PriceBreak : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PriceBreaks", "IX_PriceBreak");
            CreateIndex("dbo.PriceBreaks", new[] { "RFQBidId", "ProductId", "Quantity", "TaskId" }, unique: true, name: "IX_PriceBreak");
        }
        
        public override void Down()
        {
            DropIndex("dbo.PriceBreaks", "IX_PriceBreak");
            CreateIndex("dbo.PriceBreaks", new[] { "RFQBidId", "ProductId", "Quantity" }, unique: true, name: "IX_PriceBreak");
        }
    }
}
