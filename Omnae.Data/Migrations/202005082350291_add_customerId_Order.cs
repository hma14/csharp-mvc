namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_customerId_Order : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "CustomerId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "CustomerId");
        }
    }
}
