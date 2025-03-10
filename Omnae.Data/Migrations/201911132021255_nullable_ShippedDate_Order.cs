namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nullable_ShippedDate_Order : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Orders", "ShippedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Orders", "ShippedDate", c => c.DateTime(nullable: false));
        }
    }
}
