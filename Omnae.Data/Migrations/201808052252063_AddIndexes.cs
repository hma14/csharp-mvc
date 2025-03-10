namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIndexes : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.OmnaeInvoices", "TaskId");
            CreateIndex("dbo.OmnaeInvoices", "OrderId");
            CreateIndex("dbo.OmnaeInvoices", "CompanyId");
            CreateIndex("dbo.RFQBids", "ProductId");
            CreateIndex("dbo.RFQBids", "VendorId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.RFQBids", new[] { "VendorId" });
            DropIndex("dbo.RFQBids", new[] { "ProductId" });
            DropIndex("dbo.OmnaeInvoices", new[] { "CompanyId" });
            DropIndex("dbo.OmnaeInvoices", new[] { "OrderId" });
            DropIndex("dbo.OmnaeInvoices", new[] { "TaskId" });
        }
    }
}
