namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_isQualified_Company : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "isQualified", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companies", "isQualified");
        }
    }
}
