namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCompanyAccountingEmail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "AccountingEmail", c => c.String(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companies", "AccountingEmail");
        }
    }
}
