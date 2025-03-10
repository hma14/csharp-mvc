namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDefaultShippingProfile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShippingProfiles", "IsDefault", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShippingProfiles", "IsDefault");
        }
    }
}
