namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_ApprovedCapability_table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApprovedCapabilities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VendorId = c.Int(nullable: false),
                        BuildType = c.Int(nullable: false),
                        MaterialType = c.Int(nullable: false),
                        MetalProcess = c.Int(),
                        PlasticsProcess = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.VendorId, t.BuildType, t.MaterialType }, unique: true, name: "IX_ApprovedCapability");
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.ApprovedCapabilities", "IX_ApprovedCapability");
            DropTable("dbo.ApprovedCapabilities");
        }
    }
}
