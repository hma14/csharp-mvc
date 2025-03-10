namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_table_CompaniesCreditRelationships : DbMigration
    {
        
        
        public override void Up()
        {
            CreateTable(
                "dbo.CompaniesCreditRelationships",
                c => new
                    {
                        CustomerId = c.Int(nullable: false),
                        VendorId = c.Int(nullable: false),
                        isTerm = c.Boolean(nullable: false),
                        TermDays = c.Int(),
                        CreditLimit = c.Decimal(precision: 18, scale: 2),
                        DiscountDays = c.Int(),
                        Discount = c.Single(),
                        Deposit = c.Int(),
                    })
                .PrimaryKey(t => new { t.CustomerId, t.VendorId });
            
            CreateIndex("dbo.CompaniesCreditRelationships", "VendorId");
            CreateIndex("dbo.CompaniesCreditRelationships", "CustomerId");
            AddForeignKey("dbo.CompaniesCreditRelationships", "VendorId", "dbo.Companies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.CompaniesCreditRelationships", "CustomerId", "dbo.Companies", "Id", cascadeDelete: false);
        }

        public override void Down()
        {
            DropForeignKey("dbo.CompaniesCreditRelationships", "CustomerId", "dbo.Companies");
            DropForeignKey("dbo.CompaniesCreditRelationships", "VendorId", "dbo.Companies");
            DropIndex("dbo.CompaniesCreditRelationships", new[] { "CustomerId" });
            DropIndex("dbo.CompaniesCreditRelationships", new[] { "VendorId" });
            DropTable("dbo.CompaniesCreditRelationships");
        }
    }
}
