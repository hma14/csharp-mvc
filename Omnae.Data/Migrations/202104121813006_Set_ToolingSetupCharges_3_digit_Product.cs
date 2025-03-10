namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Set_ToolingSetupCharges_3_digit_Product : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Products", "IX_Product");
            DropIndex("dbo.Products", new[] { "VendorId" });
            AlterColumn("dbo.Products", "ToolingSetupCharges", c => c.Decimal(precision: 18, scale: 3));
            CreateIndex("dbo.Products", new[] { "CustomerId", "PartNumber", "PartNumberRevision", "VendorId" }, unique: true, name: "IX_Product");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Products", "IX_Product");
            AlterColumn("dbo.Products", "ToolingSetupCharges", c => c.Decimal(precision: 18, scale: 2));
            CreateIndex("dbo.Products", "VendorId");
            CreateIndex("dbo.Products", new[] { "CustomerId", "PartNumber", "PartNumberRevision" }, unique: true, name: "IX_Product");
        }
    }
}
