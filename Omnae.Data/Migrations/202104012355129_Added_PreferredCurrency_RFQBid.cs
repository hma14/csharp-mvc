namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_PreferredCurrency_RFQBid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RFQBids", "PreferredCurrency", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RFQBids", "PreferredCurrency");
        }
    }
}
