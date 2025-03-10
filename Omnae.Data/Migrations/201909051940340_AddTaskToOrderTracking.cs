namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTaskToOrderTracking : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderStateTrackings", "TaskId", c => c.Int());
            CreateIndex("dbo.OrderStateTrackings", "TaskId");
            AddForeignKey("dbo.OrderStateTrackings", "TaskId", "dbo.TaskDatas", "TaskId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderStateTrackings", "TaskId", "dbo.TaskDatas");
            DropIndex("dbo.OrderStateTrackings", new[] { "TaskId" });
            DropColumn("dbo.OrderStateTrackings", "TaskId");
        }
    }
}
