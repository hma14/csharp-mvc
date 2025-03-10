namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIndexToAspNetIdentity : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.AspNetUserClaims", "UserId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
        }
    }
}
