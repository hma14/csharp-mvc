namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Remove__updatedAt_BidRFQStatus : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.BidRFQStatus", "_updatedAt");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BidRFQStatus", "_updatedAt", c => c.DateTime(nullable: false));
        }
    }
}
