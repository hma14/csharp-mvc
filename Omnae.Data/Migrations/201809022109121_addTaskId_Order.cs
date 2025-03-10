namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class addTaskId_Order : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "TaskId", c => c.Int(nullable: true));
            CreateIndex("dbo.Orders", "TaskId");
            AddForeignKey("dbo.Orders", "TaskId", "dbo.TaskDatas", "TaskId");

            Sql(@"
               UPDATE o
               SET [TaskId] = t.[TaskId]
               FROM (SELECT [TaskId], [OrderId] FROM [dbo].[TaskDatas] where [OrderId] is not null) t
	             INNER JOIN [dbo].[Orders] o ON t.[OrderId] = o.[Id];");

            DropForeignKey("dbo.TaskDatas", "OrderId", "dbo.Orders");
            DropIndex("dbo.TaskDatas", new[] { "OrderId" });
            DropColumn("dbo.TaskDatas", "OrderId");
        }

        public override void Down()
        {
            throw new InvalidOperationException("Down is Disable in this Migration");
            //AddColumn("dbo.TaskDatas", "OrderId", c => c.Int(nullable: true));
            //DropForeignKey("dbo.Orders", "TaskId", "dbo.TaskDatas");
            //DropIndex("dbo.Orders", new[] { "TaskId" });
            //DropColumn("dbo.Orders", "TaskId");
            //CreateIndex("dbo.TaskDatas", "OrderId");
            //AddForeignKey("dbo.TaskDatas", "OrderId", "dbo.Orders", "Id");
        }
    }
}
