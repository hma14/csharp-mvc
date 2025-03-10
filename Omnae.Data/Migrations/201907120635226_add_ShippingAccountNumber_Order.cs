namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_ShippingAccountNumber_Order : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "ShippingAccountNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "ShippingAccountNumber");
        }
    }
}
