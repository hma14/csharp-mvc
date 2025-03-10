namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDefaultsValuesForUser : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "Email", c => c.String(nullable: false));

            AlterColumn("dbo.AspNetUsers", "EmailConfirmed", c => c.Boolean(nullable: false, defaultValue: false));
            AlterColumn("dbo.AspNetUsers", "PhoneNumberConfirmed", c => c.Boolean(nullable: false, defaultValue: false));
            AlterColumn("dbo.AspNetUsers", "TwoFactorEnabled", c => c.Boolean(nullable: false, defaultValue: false));
            AlterColumn("dbo.AspNetUsers", "LockoutEnabled", c => c.Boolean(nullable: false, defaultValue: false));
            AlterColumn("dbo.AspNetUsers", "AccessFailedCount", c => c.Int(nullable: false, defaultValue: 0));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "Email", c => c.String());
        }
    }
}
