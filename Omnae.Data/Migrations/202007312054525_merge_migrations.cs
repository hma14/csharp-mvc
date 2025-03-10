namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class merge_migrations : DbMigration
    {
        public override void Up()
        {
            //AlterColumn("dbo.Products", "_updatedAt", c => c.DateTime(nullable: false));
            //AlterColumn("dbo.OmnaeInvoices", "PODocUri", c => c.String(maxLength: 8000, unicode: false));
            //AlterColumn("dbo.Orders", "_updatedAt", c => c.DateTime(nullable: false));
            //AlterColumn("dbo.ExpeditedShipmentRequests", "_createdAt", c => c.DateTime(nullable: false));
            //AlterColumn("dbo.ExpeditedShipmentRequests", "_updatedAt", c => c.DateTime(nullable: false));
            //AlterColumn("dbo.NCReports", "_updatedAt", c => c.DateTime(nullable: false));
            //AlterColumn("dbo.NCRImages", "ImageUrl", c => c.String(maxLength: 8000, unicode: false));
        }
        
        public override void Down()
        {
            //AlterColumn("dbo.NCRImages", "ImageUrl", c => c.String());
            //AlterColumn("dbo.NCReports", "_updatedAt", c => c.DateTime());
            //AlterColumn("dbo.ExpeditedShipmentRequests", "_updatedAt", c => c.DateTime());
            //AlterColumn("dbo.ExpeditedShipmentRequests", "_createdAt", c => c.DateTime(nullable: false));
            //AlterColumn("dbo.Orders", "_updatedAt", c => c.DateTime());
            //AlterColumn("dbo.OmnaeInvoices", "PODocUri", c => c.String());
            //AlterColumn("dbo.Products", "_updatedAt", c => c.DateTime());
        }
    }
}
