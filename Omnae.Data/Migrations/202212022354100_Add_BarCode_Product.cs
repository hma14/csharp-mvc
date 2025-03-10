namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_BarCode_Product : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "BarCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "BarCode");
        }
    }
}
