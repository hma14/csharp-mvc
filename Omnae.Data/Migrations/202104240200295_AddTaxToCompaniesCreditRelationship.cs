namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTaxToCompaniesCreditRelationship : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CompaniesCreditRelationships", "TaxPercentage", c => c.Int(nullable: false, defaultValue: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CompaniesCreditRelationships", "TaxPercentage");
        }
    }
}
