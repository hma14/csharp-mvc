namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Auth0_Roles_AspNetUsers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Role", c => c.String());
            AddColumn("dbo.AspNetUsers", "CustomerRole", c => c.String());
            AddColumn("dbo.AspNetUsers", "VendorRole", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "VendorRole");
            DropColumn("dbo.AspNetUsers", "CustomerRole");
            DropColumn("dbo.AspNetUsers", "Role");
        }
    }
}
