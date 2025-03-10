namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_QuantityTemp_QuantityUnit : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PriceBreaks", "IX_PriceBreak");
            AddColumn("dbo.PriceBreaks", "UnitOfMeasurement", c => c.Int(nullable: false));
            AlterColumn("dbo.PriceBreaks", "Quantity", c => c.Decimal(nullable: false, precision: 12, scale: 3));
            CreateIndex("dbo.PriceBreaks", new[] { "RFQBidId", "ProductId", "Quantity", "TaskId" }, unique: true, name: "IX_PriceBreak");
        }
        
        public override void Down()
        {
            DropIndex("dbo.PriceBreaks", "IX_PriceBreak");
            AlterColumn("dbo.PriceBreaks", "Quantity", c => c.Int(nullable: false));
            DropColumn("dbo.PriceBreaks", "UnitOfMeasurement");
            CreateIndex("dbo.PriceBreaks", new[] { "RFQBidId", "ProductId", "Quantity", "TaskId" }, unique: true, name: "IX_PriceBreak");
        }
    }
}
