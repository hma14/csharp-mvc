namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class replace_IsActive_with_IsCancelled_Order : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "IsCancelled", c => c.Boolean(nullable: false));
            DropColumn("dbo.Orders", "IsActive");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "IsActive", c => c.Boolean());
            DropColumn("dbo.Orders", "IsCancelled");
        }
    }
}
