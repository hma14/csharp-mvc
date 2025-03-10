namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_processType_Product : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "ProcessType", c => c.Int());
            AddColumn("dbo.Products", "AnodizingType", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "AnodizingType");
            DropColumn("dbo.Products", "ProcessType");
        }
    }
}
