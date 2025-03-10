namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInvitation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "InvitedByCompanyId", c => c.Int(nullable: true));
            AddColumn("dbo.Companies", "WasInvited", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companies", "WasInvited");
            DropColumn("dbo.Companies", "InvitedByCompanyId");
        }
    }
}
