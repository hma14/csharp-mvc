namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addback__updatedAt_BidRFQStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BidRFQStatus", "_updatedAt", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BidRFQStatus", "_updatedAt");
        }
    }
}
