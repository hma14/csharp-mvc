namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_BidTimerInterval_to_TimerSetup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TimerSetups", "BidTimerInterval", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TimerSetups", "BidTimerInterval");
        }
    }
}
