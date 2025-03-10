namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_Currency_ToolingDepositPercentage_table_CompaniesCreditRelationship : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CompaniesCreditRelationships", "Currency", c => c.Int(nullable: false));
            AddColumn("dbo.CompaniesCreditRelationships", "ToolingDepositPercentage", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CompaniesCreditRelationships", "ToolingDepositPercentage");
            DropColumn("dbo.CompaniesCreditRelationships", "Currency");
        }
    }
}
