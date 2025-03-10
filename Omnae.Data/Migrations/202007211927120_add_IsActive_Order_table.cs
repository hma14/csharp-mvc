namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_IsActive_Order_table : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "IsActive", c => c.Boolean(nullable:true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "IsActive");
        }
    }
}
