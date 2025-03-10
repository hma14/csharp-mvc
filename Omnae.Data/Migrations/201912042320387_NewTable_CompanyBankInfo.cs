namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewTable_CompanyBankInfo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompanyBankInfoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BankName = c.String(),
                        BankAddressId = c.Int(nullable: false),
                        TransitNumber = c.String(),
                        InstitutionNumber = c.String(),
                        AccountNumber = c.String(),
                        BeneficiaryBankSwiftNumber = c.String(),
                        IntermediaryBank = c.String(),
                        IntermediaryBankSwiftNumber = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Addresses", t => t.BankAddressId, cascadeDelete: true)
                .Index(t => t.BankAddressId);
            
            AddColumn("dbo.Companies", "CompanyBankInfoId", c => c.Int());
            CreateIndex("dbo.Companies", "CompanyBankInfoId");
            AddForeignKey("dbo.Companies", "CompanyBankInfoId", "dbo.CompanyBankInfoes", "Id");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.HubspotIntegrationSyncControls",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LastCheckForNewAuthZeroUsers = c.DateTime(),
                        LastCheckForNewAuthZeroCompany = c.DateTime(),
                        LastUpdateInOmnaeUserDatabase = c.DateTime(),
                        LastUpdateInOmnaeCompanyDatabase = c.DateTime(),
                        LastUpdateInHubspotUser = c.DateTime(),
                        LastUpdateInHubspotCompany = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.Companies", "CompanyBankInfoId", "dbo.CompanyBankInfoes");
            DropForeignKey("dbo.CompanyBankInfoes", "BankAddressId", "dbo.Addresses");
            DropIndex("dbo.CompanyBankInfoes", new[] { "BankAddressId" });
            DropIndex("dbo.Companies", new[] { "CompanyBankInfoId" });
            DropColumn("dbo.Companies", "CompanyBankInfoId");
            DropTable("dbo.CompanyBankInfoes");
        }
    }
}
