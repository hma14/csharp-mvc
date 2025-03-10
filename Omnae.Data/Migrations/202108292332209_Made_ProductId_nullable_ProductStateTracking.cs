namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Made_ProductId_nullable_ProductStateTracking : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.NCReports", "_createdAt", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ProductStateTrackings", "ProductId", c => c.Int());
            //CreateIndex("dbo.PartRevisions", "TaskId");
            //AddForeignKey("dbo.PartRevisions", "TaskId", "dbo.TaskDatas", "TaskId");
        }
        
        public override void Down()
        {
            //DropForeignKey("dbo.PartRevisions", "TaskId", "dbo.TaskDatas");
            //DropIndex("dbo.PartRevisions", new[] { "TaskId" });
            //AlterColumn("dbo.ProductStateTrackings", "ProductId", c => c.Int(nullable: false));
            //DropColumn("dbo.NCReports", "_createdAt");
        }
    }
}
