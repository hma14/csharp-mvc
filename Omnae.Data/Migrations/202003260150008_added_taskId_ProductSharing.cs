namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_taskId_ProductSharing : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductSharings", "TaskId", c => c.Int(nullable: true));
            CreateIndex("dbo.ProductSharings", "TaskId");
            AddForeignKey("dbo.ProductSharings", "TaskId", "dbo.TaskDatas", "TaskId", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductSharings", "TaskId", "dbo.TaskDatas");
            DropIndex("dbo.ProductSharings", new[] { "TaskId" });
            DropColumn("dbo.ProductSharings", "TaskId");
        }
    }
}
