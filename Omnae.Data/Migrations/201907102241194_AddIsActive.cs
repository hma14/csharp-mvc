namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsActive : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shippings", "IsActive", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("dbo.ShippingAccounts", "IsActive", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("dbo.ShippingProfiles", "IsActive", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShippingProfiles", "IsActive");
            DropColumn("dbo.ShippingAccounts", "IsActive");
            DropColumn("dbo.Shippings", "IsActive");
        }
    }
}
