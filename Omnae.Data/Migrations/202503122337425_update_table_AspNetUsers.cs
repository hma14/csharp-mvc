namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_table_AspNetUsers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Active", c => c.Boolean());
            AddColumn("dbo.AspNetUsers", "CompanyId", c => c.Int());
            AddColumn("dbo.AspNetUsers", "IsPrimaryContact", c => c.Boolean());
            AddColumn("dbo.AspNetUsers", "Role", c => c.String());
            AddColumn("dbo.AspNetUsers", "CustomerRole", c => c.String());
            AddColumn("dbo.AspNetUsers", "VendorRole", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Active");
            DropColumn("dbo.AspNetUsers", "CompanyId");
            DropColumn("dbo.AspNetUsers", "IsPrimaryContact");
            DropColumn("dbo.AspNetUsers", "Role");
            DropColumn("dbo.AspNetUsers", "CustomerRole");
            DropColumn("dbo.AspNetUsers", "VendorRole");
        }
    }
}
