namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_POLegalTerms_Company : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "POLegalTerms", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companies", "POLegalTerms");
        }
    }
}
