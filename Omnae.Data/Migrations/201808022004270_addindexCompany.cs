namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addindexCompany : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Companies", "UserId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Companies", new[] { "UserId" });
        }
    }
}
