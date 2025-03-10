namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddActiveToCompany : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "IsActive", c => c.Boolean(nullable: false, defaultValue: true));
            CreateIndex("dbo.Orders", "CustomerId");
            AddForeignKey("dbo.Orders", "CustomerId", "dbo.Companies", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "CustomerId", "dbo.Companies");
            DropIndex("dbo.Orders", new[] { "CustomerId" });
            DropColumn("dbo.Companies", "IsActive");
        }
    }
}
