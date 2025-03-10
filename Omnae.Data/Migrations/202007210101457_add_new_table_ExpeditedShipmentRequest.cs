namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_new_table_ExpeditedShipmentRequest : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExpeditedShipmentRequests",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    OrderId = c.Int(nullable: false),
                    InitiateCompanyId = c.Int(nullable: false),
                    ExpeditedShipmentType = c.Int(nullable: false),
                    IsRequestedByCustomer = c.Boolean(),
                    IsRequestedByVendor = c.Boolean(),
                    NewDesireShippingDate = c.DateTime(nullable: false),
                    IsAccepted = c.Boolean(),
                    _createdAt = c.DateTime(nullable: false),
                    _updatedAt = c.DateTime(),
                })
                .PrimaryKey(t => t.Id);

            AddColumn("dbo.Orders", "ExpeditedShipmentRequestId", c => c.Int(nullable: true));
            CreateIndex("dbo.Orders", "ExpeditedShipmentRequestId");
            AddForeignKey("dbo.Orders", "ExpeditedShipmentRequestId", "dbo.ExpeditedShipmentRequests", "Id");
        }
        
        public override void Down()
        {           
            DropColumn("dbo.Orders", "ExpeditedShipmentRequestId");
            DropTable("dbo.ExpeditedShipmentRequests");
        }
    }
}
