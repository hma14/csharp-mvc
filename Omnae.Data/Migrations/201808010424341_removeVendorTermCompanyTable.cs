namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeVendorTermCompanyTable : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Companies", "VendorTerm");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Companies", "VendorTerm", c => c.Int());
        }
    }
}
