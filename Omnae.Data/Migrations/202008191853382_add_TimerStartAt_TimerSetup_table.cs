namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_TimerStartAt_TimerSetup_table : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TimerSetups", "BidTimerStartAt", c => c.DateTime());
            AddColumn("dbo.TimerSetups", "RevisionTimerStartAt", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TimerSetups", "RevisionTimerStartAt");
            DropColumn("dbo.TimerSetups", "BidTimerStartAt");
        }
    }
}
