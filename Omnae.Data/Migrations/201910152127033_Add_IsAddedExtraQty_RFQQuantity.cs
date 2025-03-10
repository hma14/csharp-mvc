namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_IsAddedExtraQty_RFQQuantity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RFQQuantities", "IsAddedExtraQty", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RFQQuantities", "IsAddedExtraQty");
        }
    }
}
