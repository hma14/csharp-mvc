namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_OrderCancelledBy_Order : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "OrderCancelledBy", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "OrderCancelledBy");
        }
    }
}
