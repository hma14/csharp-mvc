namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_cancel_reasons_order : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TaskDatas", "TaskStateBeforeCustomerCancelOrder", c => c.Int(nullable: true));
            AddColumn("dbo.Orders", "CancelReason", c => c.String());
            AddColumn("dbo.Orders", "DenyCancelReason", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "DenyCancelReason");
            DropColumn("dbo.Orders", "CancelReason");
            DropColumn("dbo.TaskDatas", "TaskStateBeforeCustomerCancelOrder");
        }
    }
}
