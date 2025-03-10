namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCompanyLogoUriCompanyTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "CompanyLogoUri", c => c.String());
            AddColumn("dbo.TaskDatas", "isEnterprise", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TaskDatas", "isEnterprise");
            DropColumn("dbo.Companies", "CompanyLogoUri");
        }
    }
}
