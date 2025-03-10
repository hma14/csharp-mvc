namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixDocumentIndexAndMergeLastChanges : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Documents", "IX_Document");
            //CreateTable(
            //    "dbo.HubspotIntegrationSyncControls",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            LastCheckForNewAuthZeroUsers = c.DateTime(),
            //            LastCheckForNewAuthZeroCompany = c.DateTime(),
            //            LastUpdateInOmnaeUserDatabase = c.DateTime(),
            //            LastUpdateInOmnaeCompanyDatabase = c.DateTime(),
            //            LastUpdateInHubspotUser = c.DateTime(),
            //            LastUpdateInHubspotCompany = c.DateTime(),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //AddColumn("dbo.Orders", "_updatedAt", c => c.DateTime());
            //AddColumn("dbo.NCReports", "_updatedAt", c => c.DateTime());
            CreateIndex("dbo.Documents", new[] { "ProductId", "Name", "Version" }, unique: true, name: "IX_Document");
            CreateIndex("dbo.NCReports", "ProductId");
            //AddForeignKey("dbo.PriceBreaks", "RFQBidId", "dbo.RFQBids", "Id", cascadeDelete: false);
            //AddForeignKey("dbo.NCReports", "ProductId", "dbo.Products", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NCReports", "ProductId", "dbo.Products");
            //DropForeignKey("dbo.PriceBreaks", "RFQBidId", "dbo.RFQBids");
            DropIndex("dbo.NCReports", new[] { "ProductId" });
            DropIndex("dbo.Documents", "IX_Document");
            //DropColumn("dbo.NCReports", "_updatedAt");
            //DropColumn("dbo.Orders", "_updatedAt");
            //DropTable("dbo.HubspotIntegrationSyncControls");
            CreateIndex("dbo.Documents", new[] { "ProductId", "Name" }, unique: true, name: "IX_Document");
        }
    }
}
