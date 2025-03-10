namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOmnaeInvoiceFK : DbMigration
    {
        public override void Up()
        {
            //AddForeignKey("dbo.OmnaeInvoices", "TaskId", "dbo.TaskDatas", "TaskId", cascadeDelete: false); //This is Added in the Live Above. To use NOCHECK to create this FK. Becase have some invalid data in the table right now.
            Sql(@"ALTER TABLE [dbo].[OmnaeInvoices] WITH NOCHECK 
                  ADD CONSTRAINT [FK_dbo.OmnaeInvoices_dbo.TaskDatas_TaskId] 
                  FOREIGN KEY ([TaskId]) 
                  REFERENCES [dbo].[TaskDatas] ([TaskId]);");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OmnaeInvoices", "TaskId", "dbo.TaskDatas");
        }
    }
}
