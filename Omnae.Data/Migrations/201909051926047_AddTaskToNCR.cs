namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTaskToNCR : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NCReports", "TaskId", c => c.Int(nullable: true));
            CreateIndex("dbo.NCReports", "TaskId");
            AddForeignKey("dbo.NCReports", "TaskId", "dbo.TaskDatas", "TaskId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NCReports", "TaskId", "dbo.TaskDatas");
            DropIndex("dbo.NCReports", new[] { "TaskId" });
            DropColumn("dbo.NCReports", "TaskId");
        }
    }
}
