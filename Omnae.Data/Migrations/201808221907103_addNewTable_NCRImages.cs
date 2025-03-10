namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addNewTable_NCRImages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NCRImages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NCReportId = c.Int(nullable: false),
                        ImageUrl = c.String(),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.NCReports", t => t.NCReportId, cascadeDelete: true)
                .Index(t => t.NCReportId);
            
            DropColumn("dbo.NCReports", "EvidenceImageUrl");
            DropColumn("dbo.NCReports", "CustomerCauseImageRefUrl");
            DropColumn("dbo.NCReports", "VendorCauseImageRefUrl");
            DropColumn("dbo.NCReports", "RootCauseOnCustomerImageRefUrl");
        }
        
        public override void Down()
        {
            AddColumn("dbo.NCReports", "RootCauseOnCustomerImageRefUrl", c => c.String());
            AddColumn("dbo.NCReports", "VendorCauseImageRefUrl", c => c.String());
            AddColumn("dbo.NCReports", "CustomerCauseImageRefUrl", c => c.String());
            AddColumn("dbo.NCReports", "EvidenceImageUrl", c => c.String());
            DropForeignKey("dbo.NCRImages", "NCReportId", "dbo.NCReports");
            DropIndex("dbo.NCRImages", new[] { "NCReportId" });
            DropTable("dbo.NCRImages");
        }
    }
}
