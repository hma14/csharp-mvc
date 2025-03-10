namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_ProductPriceQuote_table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductPriceQuotes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VendorId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        ProductionLeadTime = c.Int(nullable: false),
                        ExpireDate = c.DateTime(nullable: false),
                        QuoteDocUri = c.String(),
                        IsActive = c.Boolean(),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
            AddColumn("dbo.PriceBreaks", "ProductPriceQuoteId", c => c.Int());
            CreateIndex("dbo.PriceBreaks", "ProductPriceQuoteId");
            AddForeignKey("dbo.PriceBreaks", "ProductPriceQuoteId", "dbo.ProductPriceQuotes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductPriceQuotes", "ProductId", "dbo.Products");
            DropForeignKey("dbo.PriceBreaks", "ProductPriceQuoteId", "dbo.ProductPriceQuotes");
            DropIndex("dbo.ProductPriceQuotes", new[] { "ProductId" });
            DropIndex("dbo.PriceBreaks", new[] { "ProductPriceQuoteId" });
            DropColumn("dbo.PriceBreaks", "ProductPriceQuoteId");
            DropTable("dbo.ProductPriceQuotes");
        }
    }
}
