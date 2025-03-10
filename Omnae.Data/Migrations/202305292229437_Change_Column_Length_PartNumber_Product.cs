namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Change_Column_Length_PartNumber_Product : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Products", "IX_Product");
            AlterColumn("dbo.Products", "PartNumber", c => c.String(maxLength: 150));
            AlterColumn("dbo.Products", "PartNumberRevision", c => c.String(maxLength: 150));
            AlterColumn("dbo.Products", "ParentPartNumberRevision", c => c.String(maxLength: 150));
            CreateIndex("dbo.Products", new[] { "CustomerId", "PartNumber", "PartNumberRevision", "VendorId" }, unique: true, name: "IX_Product");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Products", "IX_Product");
            AlterColumn("dbo.Products", "ParentPartNumberRevision", c => c.String(maxLength: 50));
            AlterColumn("dbo.Products", "PartNumberRevision", c => c.String(maxLength: 50));
            AlterColumn("dbo.Products", "PartNumber", c => c.String(maxLength: 50));
            CreateIndex("dbo.Products", new[] { "CustomerId", "PartNumber", "PartNumberRevision", "VendorId" }, unique: true, name: "IX_Product");
        }
    }
}
