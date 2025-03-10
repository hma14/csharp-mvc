namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_UnitOf_Measurement_RFQQuantities : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RFQQuantities", "UnitOfMeasurement", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RFQQuantities", "UnitOfMeasurement");
        }
    }
}
