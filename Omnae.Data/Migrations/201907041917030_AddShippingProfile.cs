namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddShippingProfile : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ShippingProfiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileName = c.String(nullable: false),
                        DestinationCompanyName = c.String(),
                        ShippingId = c.Int(),
                        Description = c.String(),
                        CompanyId = c.Int(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedByUserId = c.String(maxLength: 128),
                        ModifiedAt = c.DateTime(nullable: false),
                        ModifiedByUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedByUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedByUserId)
                .ForeignKey("dbo.Shippings", t => t.ShippingId)
                .Index(t => t.ShippingId)
                .Index(t => t.CompanyId)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.ModifiedByUserId);
            
            Sql(@"
                UPDATE [dbo].[Shippings]
                   SET [CompanyId] = null
                 WHERE Id in (SELECT h.[Id] FROM [dbo].[Shippings] h LEFT JOIN [dbo].[Companies] c ON h.CompanyId = c.Id WHERE C.Id is Null)");

            CreateIndex("dbo.Shippings", "CompanyId");
            AddForeignKey("dbo.Shippings", "CompanyId", "dbo.Companies", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShippingProfiles", "ShippingId", "dbo.Shippings");
            DropForeignKey("dbo.ShippingProfiles", "ModifiedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ShippingProfiles", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ShippingProfiles", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Shippings", "CompanyId", "dbo.Companies");
            DropIndex("dbo.ShippingProfiles", new[] { "ModifiedByUserId" });
            DropIndex("dbo.ShippingProfiles", new[] { "CreatedByUserId" });
            DropIndex("dbo.ShippingProfiles", new[] { "CompanyId" });
            DropIndex("dbo.ShippingProfiles", new[] { "ShippingId" });
            DropIndex("dbo.Shippings", new[] { "CompanyId" });
            DropTable("dbo.ShippingProfiles");
        }
    }
}
