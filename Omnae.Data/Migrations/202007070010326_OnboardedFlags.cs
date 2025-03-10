namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OnboardedFlags : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "WasOnboarded", c => c.Boolean(nullable: false, defaultValue: false));
            AddColumn("dbo.Companies", "OnboardedByCompanyId", c => c.Int(nullable: true));
            AddColumn("dbo.Products", "WasOnboarded", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "WasOnboarded");
            DropColumn("dbo.Companies", "OnboardedByCompanyId");
            DropColumn("dbo.Companies", "WasOnboarded");
        }
    }
}
