namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modified_TimerSetup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TimerSetups", "TimerStartAt", c => c.DateTime());
            AddColumn("dbo.TimerSetups", "IsExpired", c => c.Boolean());
            AddColumn("dbo.TimerSetups", "TimerType", c => c.Int(nullable: false));
            DropColumn("dbo.TimerSetups", "BidTimerInterval");
            DropColumn("dbo.TimerSetups", "BidTimerStartAt");
            DropColumn("dbo.TimerSetups", "RevisionTimerStartAt");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TimerSetups", "RevisionTimerStartAt", c => c.DateTime());
            AddColumn("dbo.TimerSetups", "BidTimerStartAt", c => c.DateTime());
            AddColumn("dbo.TimerSetups", "BidTimerInterval", c => c.String());
            DropColumn("dbo.TimerSetups", "TimerType");
            DropColumn("dbo.TimerSetups", "IsExpired");
            DropColumn("dbo.TimerSetups", "TimerStartAt");
        }
    }
}
