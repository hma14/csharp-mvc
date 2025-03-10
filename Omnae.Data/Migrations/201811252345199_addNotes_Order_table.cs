namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addNotes_Order_table : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "Notes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "Notes");
        }
    }
}
