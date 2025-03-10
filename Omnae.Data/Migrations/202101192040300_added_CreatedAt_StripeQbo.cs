namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_CreatedAt_StripeQbo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StripeQboes", "_createdAt", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StripeQboes", "_createdAt");
        }
    }
}
