namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Changed_Decimal_Qty_OmnaeInvoices : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PriceBreaks", "IX_PriceBreak");
            AlterColumn("dbo.OmnaeInvoices", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Orders", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.PriceBreaks", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 3));
            AlterColumn("dbo.RFQQuantities", "Qty1", c => c.Decimal(precision: 18, scale: 3));
            AlterColumn("dbo.RFQQuantities", "Qty2", c => c.Decimal(precision: 18, scale: 3));
            AlterColumn("dbo.RFQQuantities", "Qty3", c => c.Decimal(precision: 18, scale: 3));
            AlterColumn("dbo.RFQQuantities", "Qty4", c => c.Decimal(precision: 18, scale: 3));
            AlterColumn("dbo.RFQQuantities", "Qty5", c => c.Decimal(precision: 18, scale: 3));
            AlterColumn("dbo.RFQQuantities", "Qty6", c => c.Decimal(precision: 18, scale: 3));
            AlterColumn("dbo.RFQQuantities", "Qty7", c => c.Decimal(precision: 18, scale: 3));
            CreateIndex("dbo.PriceBreaks", new[] { "RFQBidId", "ProductId", "Quantity", "TaskId" }, unique: true, name: "IX_PriceBreak");
        }
        
        public override void Down()
        {
            DropIndex("dbo.PriceBreaks", "IX_PriceBreak");
            AlterColumn("dbo.RFQQuantities", "Qty7", c => c.Decimal(precision: 12, scale: 3));
            AlterColumn("dbo.RFQQuantities", "Qty6", c => c.Decimal(precision: 12, scale: 3));
            AlterColumn("dbo.RFQQuantities", "Qty5", c => c.Decimal(precision: 12, scale: 3));
            AlterColumn("dbo.RFQQuantities", "Qty4", c => c.Decimal(precision: 12, scale: 3));
            AlterColumn("dbo.RFQQuantities", "Qty3", c => c.Decimal(precision: 12, scale: 3));
            AlterColumn("dbo.RFQQuantities", "Qty2", c => c.Decimal(precision: 12, scale: 3));
            AlterColumn("dbo.RFQQuantities", "Qty1", c => c.Decimal(precision: 12, scale: 3));
            AlterColumn("dbo.PriceBreaks", "Quantity", c => c.Decimal(nullable: false, precision: 12, scale: 3));
            AlterColumn("dbo.Orders", "Quantity", c => c.Int(nullable: false));
            AlterColumn("dbo.OmnaeInvoices", "Quantity", c => c.Int(nullable: false));
            CreateIndex("dbo.PriceBreaks", new[] { "RFQBidId", "ProductId", "Quantity", "TaskId" }, unique: true, name: "IX_PriceBreak");
        }
    }
}
