namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Removed_TimerStatus_TimerSetup : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.TimerSetups", "TimerStatus");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TimerSetups", "TimerStatus", c => c.Int(nullable: false));
        }
    }
}
