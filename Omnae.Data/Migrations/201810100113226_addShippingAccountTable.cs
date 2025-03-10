namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addShippingAccountTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ShippingAccounts",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    CompanyId = c.Int(nullable: false),
                    Carrier = c.String(),
                    CarrierType = c.Int(nullable: false),                    
                    AccountNumber = c.String(),
                })
                .PrimaryKey(t => t.Id);
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ShippingAccountCarrier",
                c => new
                    {
                        ShippingAccountId = c.Int(nullable: false),
                        ShippingCarrierId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ShippingAccountId, t.ShippingCarrierId });
            
            CreateTable(
                "dbo.ShippingCarriers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CarrierName = c.String(),
                        CarrierType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ShippingAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyId = c.Int(nullable: false),
                        AccountNumber = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.ShippingAccountCarrier", "ShippingCarrierId");
            CreateIndex("dbo.ShippingAccountCarrier", "ShippingAccountId");
            AddForeignKey("dbo.ShippingAccountCarrier", "ShippingCarrierId", "dbo.ShippingCarriers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ShippingAccountCarrier", "ShippingAccountId", "dbo.ShippingAccounts", "Id", cascadeDelete: true);
        }
    }
}
