namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameVendorRejectRFQ : DbMigration
    {
        public override void Up()
        {
            
            RenameTable(name: "dbo.VendorRejectRFQs", newName: "RFQActionReasons");

            Sql(@"CREATE TRIGGER [dbo].[trg_RFQActionReasons_UpdateTimeOnUpdate] 
                  ON[dbo].[RFQActionReasons]
                  AFTER UPDATE
                  AS
                  BEGIN
                  UPDATE[dbo].[RFQActionReasons]
                      SET _updatedAt = GetUtcDate()
                      WHERE ID IN(SELECT DISTINCT ID FROM Inserted)
                  END;");


            RenameColumn(table: "dbo.BidRequestRevisions", name: "VendorRejectRFQId", newName: "RFQActionReasonId");
            RenameColumn(table: "dbo.VendorBidRFQStatus", name: "VendorRejectRFQId", newName: "RFQActionReasonId");
            RenameIndex(table: "dbo.BidRequestRevisions", name: "IX_VendorRejectRFQId", newName: "IX_RFQActionReasonId");
            RenameIndex(table: "dbo.VendorBidRFQStatus", name: "IX_VendorRejectRFQId", newName: "IX_RFQActionReasonId");
        }
        
        public override void Down()
        {
        }
    }
}
