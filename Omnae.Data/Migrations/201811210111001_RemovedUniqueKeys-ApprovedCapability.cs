namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedUniqueKeysApprovedCapability : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ApprovedCapabilities", "IX_ApprovedCapability");
            CreateIndex("dbo.ApprovedCapabilities", "VendorId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ApprovedCapabilities", new[] { "VendorId" });
            CreateIndex("dbo.ApprovedCapabilities", new[] { "VendorId", "BuildType", "MaterialType", "MetalProcess", "PlasticsProcess" }, unique: true, name: "IX_ApprovedCapability");
        }
    }
}
