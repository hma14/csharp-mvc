namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addCreateDateToProduct_addQuoteAcceptDateToRFQBid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "CreatedDate", c => c.DateTime());
            AddColumn("dbo.RFQBids", "QuoteAcceptDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RFQBids", "QuoteAcceptDate");
            DropColumn("dbo.Products", "CreatedDate");
        }
    }
}
