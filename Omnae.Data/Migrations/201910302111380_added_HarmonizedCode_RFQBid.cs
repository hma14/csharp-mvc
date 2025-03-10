namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_HarmonizedCode_RFQBid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RFQBids", "HarmonizedCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RFQBids", "HarmonizedCode");
        }
    }
}
