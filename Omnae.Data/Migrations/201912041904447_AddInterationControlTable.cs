namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInterationControlTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HubspotIntegrationSyncControls",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LastCheckForNewAuthZeroUsers = c.DateTime(),
                        LastCheckForNewAuthZeroCompany = c.DateTime(),
                        LastUpdateInOmnaeUserDatabase = c.DateTime(),
                        LastUpdateInOmnaeCompanyDatabase = c.DateTime(),
                        LastUpdateInHubspotUser = c.DateTime(),
                        LastUpdateInHubspotCompany = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.HubspotIntegrationSyncControls");
        }
    }
}
