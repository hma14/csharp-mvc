namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_naming_order : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "IsOrderCancelled", c => c.Boolean(nullable: false, defaultValue:false));
            AddColumn("dbo.Orders", "CancelOrderReason", c => c.String());
            AddColumn("dbo.Orders", "DenyCancelOrderReason", c => c.String());
            DropColumn("dbo.Orders", "IsCancelled");
            DropColumn("dbo.Orders", "CancelReason");
            DropColumn("dbo.Orders", "DenyCancelReason");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "DenyCancelReason", c => c.String());
            AddColumn("dbo.Orders", "CancelReason", c => c.String());
            AddColumn("dbo.Orders", "IsCancelled", c => c.Boolean(nullable: false));
            DropColumn("dbo.Orders", "DenyCancelOrderReason");
            DropColumn("dbo.Orders", "CancelOrderReason");
            DropColumn("dbo.Orders", "IsOrderCancelled");
        }
    }
}
