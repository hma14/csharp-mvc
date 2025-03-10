namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modified_TimerSetup_2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TimerSetups", "TimerStatus", c => c.Int(nullable: false));
            DropColumn("dbo.TimerSetups", "IsExpired");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TimerSetups", "IsExpired", c => c.Boolean());
            DropColumn("dbo.TimerSetups", "TimerStatus");
        }
    }
}
