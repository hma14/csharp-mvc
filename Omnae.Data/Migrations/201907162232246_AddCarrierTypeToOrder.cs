namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCarrierTypeToOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "CarrierType", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "CarrierType");
        }
    }
}
