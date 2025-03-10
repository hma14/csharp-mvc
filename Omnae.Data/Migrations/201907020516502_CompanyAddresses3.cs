namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompanyAddresses3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Addresses", "CompanyId", "dbo.Companies");
            DropIndex("dbo.Addresses", new[] { "CompanyId" });

            AddColumn("dbo.Addresses", "isMailingAddress", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Addresses", "CompanyId", c => c.Int(nullable: true));

            CreateIndex("dbo.Addresses", "CompanyId");
            AddForeignKey("dbo.Addresses", "CompanyId", "dbo.Companies", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Addresses", "CompanyId", "dbo.Companies");
            DropIndex("dbo.Addresses", new[] { "CompanyId" });

            AlterColumn("dbo.Addresses", "CompanyId", c => c.Int(nullable: false));
            DropColumn("dbo.Addresses", "isMailingAddress");

            CreateIndex("dbo.Addresses", "CompanyId");
            AddForeignKey("dbo.Addresses", "CompanyId", "dbo.Companies", "Id", cascadeDelete: true);
        }
    }
}
