namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_column_Currency_Company : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "Currency", c => c.Int(nullable: false, defaultValue:840));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companies", "Currency");
        }
    }
}
