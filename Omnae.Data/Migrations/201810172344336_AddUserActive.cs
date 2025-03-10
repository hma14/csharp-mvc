namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserActive : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Active", c => c.Boolean(nullable: false, defaultValue: true));
            CreateIndex("dbo.AspNetUsers", "Active");
        }
        
        public override void Down()
        {
            DropIndex("dbo.AspNetUsers", new[] { "Active" });
            DropColumn("dbo.AspNetUsers", "Active");
        }
    }
}
