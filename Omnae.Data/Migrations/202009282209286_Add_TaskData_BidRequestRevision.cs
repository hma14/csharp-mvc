namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_TaskData_BidRequestRevision : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.BidRequestRevisions", "TaskId");
            AddForeignKey("dbo.BidRequestRevisions", "TaskId", "dbo.TaskDatas", "TaskId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BidRequestRevisions", "TaskId", "dbo.TaskDatas");
            DropIndex("dbo.BidRequestRevisions", new[] { "TaskId" });
        }
    }
}
