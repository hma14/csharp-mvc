namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_isEnterprise_Company_table : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "isEnterprise", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companies", "isEnterprise");
        }
    }
}
