namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_OriginProductId_Product : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "OriginProductId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "OriginProductId");
        }
    }
}
