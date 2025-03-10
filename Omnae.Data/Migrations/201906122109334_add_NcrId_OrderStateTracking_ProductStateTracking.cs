namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_NcrId_OrderStateTracking_ProductStateTracking : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NCReports", "RootCauseAnalysisDate", c => c.DateTime());
            AddColumn("dbo.NCReports", "NCRApprovalDate", c => c.DateTime());
            AddColumn("dbo.OrderStateTrackings", "NcrId", c => c.Int());
            AddColumn("dbo.ProductStateTrackings", "NcrId", c => c.Int());
            AddColumn("dbo.ProductStateTrackings", "OrderId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductStateTrackings", "OrderId");
            DropColumn("dbo.ProductStateTrackings", "NcrId");
            DropColumn("dbo.OrderStateTrackings", "NcrId");
            DropColumn("dbo.NCReports", "NCRApprovalDate");
            DropColumn("dbo.NCReports", "RootCauseAnalysisDate");
        }
    }
}
