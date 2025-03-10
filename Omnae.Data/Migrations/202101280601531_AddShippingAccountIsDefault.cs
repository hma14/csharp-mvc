namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddShippingAccountIsDefault : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShippingAccounts", "IsDefault", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShippingAccounts", "IsDefault");
        }
    }
}
