namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_PreferredCurrency_Product : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "PreferredCurrency", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "PreferredCurrency");
        }
    }
}
