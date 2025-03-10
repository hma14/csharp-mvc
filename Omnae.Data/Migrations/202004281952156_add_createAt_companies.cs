namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_createAt_companies : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.NCReports", "CustomerId");
            AddForeignKey("dbo.NCReports", "CustomerId", "dbo.Companies", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NCReports", "CustomerId", "dbo.Companies");
            DropIndex("dbo.NCReports", new[] { "CustomerId" });
        }
    }
}
