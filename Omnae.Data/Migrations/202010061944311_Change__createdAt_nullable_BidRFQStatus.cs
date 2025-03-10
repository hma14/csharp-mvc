namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Change__createdAt_nullable_BidRFQStatus : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BidRFQStatus", "_createdAt", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BidRFQStatus", "_createdAt", c => c.DateTime(nullable: false));
        }
    }
}
