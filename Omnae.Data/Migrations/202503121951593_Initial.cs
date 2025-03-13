namespace Omnae.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Shippings", "VendorAddressId", "dbo.Addresses");
            DropForeignKey("dbo.Shippings", "VendorCompany_Id", "dbo.Companies");
            DropIndex("dbo.BidRequestRevisions", "IX_BidRequestRevision");
            DropIndex("dbo.Shippings", new[] { "VendorAddressId" });
            DropIndex("dbo.Shippings", new[] { "VendorCompany_Id" });
            DropIndex("dbo.Documents", "IX_Document");
            DropIndex("dbo.Products", "IX_Product");
            DropIndex("dbo.Products", new[] { "VendorId" });
            //DropIndex("dbo.PriceBreaks", "IX_PriceBreak");
            //RenameColumn(table: "dbo.Shippings", name: "CustomerAddressId", newName: "AddressId");
            //RenameColumn(table: "dbo.Shippings", name: "CustomerCompany_Id", newName: "CompanyId");
            //RenameIndex(table: "dbo.Shippings", name: "IX_CustomerCompany_Id", newName: "IX_CompanyId");
            //RenameIndex(table: "dbo.Shippings", name: "IX_CustomerAddressId", newName: "IX_AddressId");
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
            
            CreateTable(
                "dbo.ShippingAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        CompanyId = c.Int(nullable: false),
                        Carrier = c.String(),
                        CarrierType = c.Int(nullable: false),
                        AccountNumber = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        IsDefault = c.Boolean(nullable: false),
                        _updatedAt = c.DateTime(nullable: false),
                        _createdAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: true)
                .Index(t => t.CompanyId);
            
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
                        ShippingAccountId = c.Int(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedByUserId = c.String(maxLength: 128),
                        ModifiedAt = c.DateTime(nullable: false),
                        ModifiedByUserId = c.String(maxLength: 128),
                        IsActive = c.Boolean(nullable: false),
                        IsDefault = c.Boolean(nullable: false),
                        _updatedAt = c.DateTime(nullable: false),
                        _createdAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedByUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedByUserId)
                .ForeignKey("dbo.Shippings", t => t.ShippingId)
                .ForeignKey("dbo.ShippingAccounts", t => t.ShippingAccountId)
                .Index(t => t.ShippingId)
                .Index(t => t.CompanyId)
                .Index(t => t.ShippingAccountId)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.ModifiedByUserId);
            
            //CreateTable(
            //    "dbo.AspNetUsers",
            //    c => new
            //        {
            //            Id = c.String(nullable: false, maxLength: 128),
            //            Email = c.String(nullable: false),
            //            FirstName = c.String(nullable: false),
            //            MiddleName = c.String(),
            //            LastName = c.String(),
            //            PhoneNumber = c.String(),
            //            UserType = c.Int(nullable: false),
            //            Active = c.Boolean(nullable: false),
            //            CompanyId = c.Int(),
            //            IsPrimaryContact = c.Boolean(),
            //            Role = c.String(),
            //            CustomerRole = c.String(),
            //            VendorRole = c.String(),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Companies", t => t.CompanyId)
            //    .Index(t => t.Active)
            //    .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.ApprovedCapabilities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VendorId = c.Int(nullable: false),
                        BuildType = c.Int(nullable: false),
                        MaterialType = c.Int(nullable: false),
                        MetalProcess = c.Int(),
                        PlasticsProcess = c.Int(),
                        ProcessType = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.VendorId, cascadeDelete: true)
                .Index(t => t.VendorId);
            
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
                        _updatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.OrderId, t.InitiateCompanyId }, unique: true, name: "IX_ExpeditedShipmentRequest");
            
            CreateTable(
                "dbo.ProductPriceQuotes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VendorId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        ProductionLeadTime = c.Int(nullable: false),
                        ExpireDate = c.DateTime(nullable: false),
                        QuoteDocUri = c.String(),
                        IsActive = c.Boolean(),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.ProductSharings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SharingCompanyId = c.Int(nullable: false),
                        OwnerCompanyId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        TaskId = c.Int(),
                        CreatedUtc = c.DateTime(),
                        ModifiedUtc = c.DateTime(),
                        HasPermissionToOrder = c.Boolean(),
                        IsRevoked = c.Boolean(),
                        CreatedByUserId = c.String(maxLength: 128),
                        ModifiedByUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedByUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedByUserId)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.Companies", t => t.OwnerCompanyId, cascadeDelete: false)
                .ForeignKey("dbo.Companies", t => t.SharingCompanyId, cascadeDelete: false)
                .ForeignKey("dbo.TaskDatas", t => t.TaskId)
                .Index(t => new { t.SharingCompanyId, t.ProductId }, unique: true, name: "IX_ProductSharing")
                .Index(t => t.OwnerCompanyId)
                .Index(t => t.TaskId)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.ModifiedByUserId);
            
            CreateTable(
                "dbo.RFQActionReasons",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        VendorId = c.Int(nullable: false),
                        ReasonType = c.Int(),
                        Reason = c.String(),
                        Description = c.String(),
                        _updatedAt = c.DateTime(nullable: false),
                        _createdAt = c.DateTime(nullable: false),
                        CreatedByUserId = c.String(maxLength: 128),
                        ModifiedByUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedByUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedByUserId)
                .Index(t => new { t.ProductId, t.VendorId, t.ReasonType }, unique: true, name: "IX_VendorRejectRFQ")
                .Index(t => t.CreatedByUserId)
                .Index(t => t.ModifiedByUserId);
            
            CreateTable(
                "dbo.BidRFQStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        CustomerId = c.Int(nullable: false),
                        StateId = c.Int(nullable: false),
                        TaskId = c.Int(nullable: false),
                        SubmittedVendors = c.Int(nullable: false),
                        TotalVendors = c.Int(nullable: false),
                        KeepCurrentRevisionReason = c.String(),
                        PartRevisionId = c.Int(),
                        RevisionCycle = c.Int(),
                        _updatedAt = c.DateTime(nullable: false),
                        _createdAt = c.DateTime(nullable: false),
                        CreatedByUserId = c.String(maxLength: 128),
                        ModifiedByUserId = c.String(maxLength: 128),
                        RFQActionReasonId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedByUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedByUserId)
                .ForeignKey("dbo.PartRevisions", t => t.PartRevisionId)
                .ForeignKey("dbo.RFQActionReasons", t => t.RFQActionReasonId)
                .Index(t => t.PartRevisionId)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.ModifiedByUserId)
                .Index(t => t.RFQActionReasonId);
            
            CreateTable(
                "dbo.VendorBidRFQStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        VendorId = c.Int(nullable: false),
                        StateId = c.Int(nullable: false),
                        TaskId = c.Int(nullable: false),
                        BidRequestRevisionId = c.Int(),
                        BidRFQStatusId = c.Int(),
                        _updatedAt = c.DateTime(nullable: false),
                        _createdAt = c.DateTime(nullable: false),
                        CreatedByUserId = c.String(maxLength: 128),
                        ModifiedByUserId = c.String(maxLength: 128),
                        RFQActionReasonId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BidRequestRevisions", t => t.BidRequestRevisionId)
                .ForeignKey("dbo.BidRFQStatus", t => t.BidRFQStatusId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedByUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedByUserId)
                .ForeignKey("dbo.RFQActionReasons", t => t.RFQActionReasonId)
                .Index(t => t.BidRequestRevisionId)
                .Index(t => t.BidRFQStatusId)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.ModifiedByUserId)
                .Index(t => t.RFQActionReasonId);
            
            CreateTable(
                "dbo.CompaniesCreditRelationships",
                c => new
                    {
                        CustomerId = c.Int(nullable: false),
                        VendorId = c.Int(nullable: false),
                        isTerm = c.Boolean(nullable: false),
                        TermDays = c.Int(),
                        CreditLimit = c.Decimal(precision: 18, scale: 2),
                        TaxPercentage = c.Int(nullable: false),
                        Currency = c.Int(nullable: false),
                        DiscountDays = c.Int(),
                        Discount = c.Single(),
                        Deposit = c.Int(),
                        ToolingDepositPercentage = c.Int(),
                    })
                .PrimaryKey(t => new { t.CustomerId, t.VendorId })
                .ForeignKey("dbo.Companies", t => t.CustomerId, cascadeDelete: false)
                .ForeignKey("dbo.Companies", t => t.VendorId, cascadeDelete: false)
                .Index(t => t.CustomerId)
                .Index(t => t.VendorId);
            
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
            
            CreateTable(
                "dbo.NCRImages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NCReportId = c.Int(nullable: false),
                        ImageUrl = c.String(maxLength: 8000, unicode: false),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.NCReports", t => t.NCReportId, cascadeDelete: true)
                .Index(t => t.NCReportId);
            
            CreateTable(
                "dbo.StripeQboes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QboId = c.String(),
                        StripeInvoiceId = c.String(),
                        QboInvoiceId = c.String(),
                        QboInvoiceNumber = c.String(),
                        _createdAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Addresses", "isMainAddress", c => c.Boolean(nullable: false));
            AddColumn("dbo.Addresses", "isMailingAddress", c => c.Boolean(nullable: false));
            AddColumn("dbo.Addresses", "CompanyId", c => c.Int());
            AddColumn("dbo.BidRequestRevisions", "CreateDateTime", c => c.DateTime());
            AddColumn("dbo.BidRequestRevisions", "RevisionNumber", c => c.Int());
            AddColumn("dbo.BidRequestRevisions", "RFQActionReasonId", c => c.Int());
            AddColumn("dbo.Companies", "BillAddressId", c => c.Int());
            AddColumn("dbo.Companies", "MainCompanyAddress_Id", c => c.Int());
            AddColumn("dbo.Companies", "CompanyLogoUri", c => c.String());
            AddColumn("dbo.Companies", "CompanyType", c => c.Int(nullable: false));
            AddColumn("dbo.Companies", "isEnterprise", c => c.Boolean(nullable: false));
            AddColumn("dbo.Companies", "isQualified", c => c.Boolean(nullable: false));
            AddColumn("dbo.Companies", "AccountingEmail", c => c.String());
            AddColumn("dbo.Companies", "StripeCustomerId", c => c.String());
            AddColumn("dbo.Companies", "CompanyBankInfoId", c => c.Int());
            AddColumn("dbo.Companies", "_createdAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.Companies", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.Companies", "WasOnboarded", c => c.Boolean(nullable: false));
            AddColumn("dbo.Companies", "OnboardedByCompanyId", c => c.Int());
            AddColumn("dbo.Companies", "InvitedByCompanyId", c => c.Int());
            AddColumn("dbo.Companies", "WasInvited", c => c.Boolean(nullable: false));
            AddColumn("dbo.Companies", "POLegalTerms", c => c.String());
            AddColumn("dbo.Companies", "Currency", c => c.Int(nullable: false));
            AddColumn("dbo.Shippings", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.Documents", "TaskId", c => c.Int());
            AddColumn("dbo.Documents", "CreatedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.Documents", "ModifiedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.Documents", "BidRequestRevisionId", c => c.Int());
            AddColumn("dbo.Documents", "PartRevisionId", c => c.Int());
            AddColumn("dbo.Products", "CreatedDate", c => c.DateTime());
            AddColumn("dbo.Products", "_updatedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.Products", "CreatedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.Products", "ModifiedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.Products", "ProcessType", c => c.Int());
            AddColumn("dbo.Products", "AnodizingType", c => c.Int());
            AddColumn("dbo.Products", "WasOnboarded", c => c.Boolean(nullable: false));
            AddColumn("dbo.Products", "OriginProductId", c => c.Int());
            AddColumn("dbo.Products", "PreferredCurrency", c => c.Int());
            AddColumn("dbo.Products", "BarCode", c => c.String());
            AddColumn("dbo.PartRevisions", "CreatedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.PartRevisions", "ModifiedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.PriceBreaks", "TaskId", c => c.Int());
            AddColumn("dbo.PriceBreaks", "CreatedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.PriceBreaks", "ModifiedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.PriceBreaks", "ProductPriceQuoteId", c => c.Int());
            AddColumn("dbo.PriceBreaks", "UnitOfMeasurement", c => c.Int(nullable: false));
            AddColumn("dbo.ExtraQuantities", "NumberSampleIncluded", c => c.Int());
            AddColumn("dbo.NCReports", "ArbitrateVendorCauseReason", c => c.String());
            AddColumn("dbo.NCReports", "TaskId", c => c.Int());
            AddColumn("dbo.NCReports", "NCRApprovalDate", c => c.DateTime());
            AddColumn("dbo.NCReports", "RootCauseAnalysisDate", c => c.DateTime());
            AddColumn("dbo.NCReports", "_createdAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.NCReports", "_updatedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.OmnaeInvoices", "OrderId", c => c.Int(nullable: false));
            AddColumn("dbo.OmnaeInvoices", "EstimateNumber", c => c.String());
            AddColumn("dbo.OmnaeInvoices", "InvoiceNumber", c => c.String());
            AddColumn("dbo.OmnaeInvoices", "BillNumber", c => c.String());
            AddColumn("dbo.OmnaeInvoices", "PONumber", c => c.String());
            AddColumn("dbo.OmnaeInvoices", "PODocUri", c => c.String(maxLength: 8000, unicode: false));
            AddColumn("dbo.OmnaeInvoices", "PaymentMethod", c => c.String());
            AddColumn("dbo.OmnaeInvoices", "PaymentRefNumber", c => c.String());
            AddColumn("dbo.Orders", "TaskId", c => c.Int());
            AddColumn("dbo.Orders", "CarrierType", c => c.Int());
            AddColumn("dbo.Orders", "ShippingAccountNumber", c => c.String());
            AddColumn("dbo.Orders", "IsReorder", c => c.Boolean(nullable: false));
            AddColumn("dbo.Orders", "Buyer", c => c.String());
            AddColumn("dbo.Orders", "DesireShippingDate", c => c.DateTime());
            AddColumn("dbo.Orders", "EarliestShippingDate", c => c.DateTime());
            AddColumn("dbo.Orders", "CreatedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.Orders", "ModifiedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.Orders", "Notes", c => c.String());
            AddColumn("dbo.Orders", "_updatedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.Orders", "ProductSharingId", c => c.Int());
            AddColumn("dbo.Orders", "OrderCompanyName", c => c.String());
            AddColumn("dbo.Orders", "CustomerId", c => c.Int());
            AddColumn("dbo.Orders", "ExpeditedShipmentRequestId", c => c.Int());
            AddColumn("dbo.Orders", "IsOrderCancelled", c => c.Boolean(nullable: false));
            AddColumn("dbo.Orders", "CancelOrderReason", c => c.String());
            AddColumn("dbo.Orders", "DenyCancelOrderReason", c => c.String());
            AddColumn("dbo.Orders", "OrderCancelledBy", c => c.Int());
            AddColumn("dbo.OrderStateTrackings", "TaskId", c => c.Int());
            AddColumn("dbo.OrderStateTrackings", "NcrId", c => c.Int());
            AddColumn("dbo.OrderStateTrackings", "CreatedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.OrderStateTrackings", "ModifiedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.ProductStateTrackings", "CreatedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.ProductStateTrackings", "ModifiedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.ProductStateTrackings", "NcrId", c => c.Int());
            AddColumn("dbo.ProductStateTrackings", "OrderId", c => c.Int());
            AddColumn("dbo.RFQBids", "QuoteAcceptDate", c => c.DateTime());
            AddColumn("dbo.RFQBids", "HarmonizedCode", c => c.String());
            AddColumn("dbo.RFQBids", "CreatedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.RFQBids", "ModifiedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.RFQBids", "PreferredCurrency", c => c.Int());
            AddColumn("dbo.RFQQuantities", "IsAddedExtraQty", c => c.Boolean(nullable: false));
            AddColumn("dbo.RFQQuantities", "UnitOfMeasurement", c => c.Int(nullable: false));
            AddColumn("dbo.TaskDatas", "isEnterprise", c => c.Boolean(nullable: false));
            AddColumn("dbo.TaskDatas", "CreatedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.TaskDatas", "ModifiedByUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.TaskDatas", "TaskStateBeforeCustomerCancelOrder", c => c.Int());
            AddColumn("dbo.TimerSetups", "TimerStartAt", c => c.DateTime());
            AddColumn("dbo.TimerSetups", "_updatedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.TimerSetups", "_createdAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.TimerSetups", "TimerType", c => c.Int(nullable: false));
            AlterColumn("dbo.Companies", "Name", c => c.String(nullable: false, maxLength: 150));
            AlterColumn("dbo.Products", "ToolingSetupCharges", c => c.Decimal(precision: 18, scale: 3));
            AlterColumn("dbo.Products", "RFQQuantityId", c => c.Int());
            

            DropIndex("dbo.PriceBreaks", "IX_PriceBreak");
            AlterColumn("dbo.PriceBreaks", "RFQBidId", c => c.Int());
            AlterColumn("dbo.PriceBreaks", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 3));
            CreateIndex("dbo.PriceBreaks", new[] { "RFQBidId", "ProductId", "Quantity", "TaskId" }, unique: true, name: "IX_PriceBreak");

            AlterColumn("dbo.PriceBreaks", "UnitPrice", c => c.Decimal(precision: 9, scale: 3));
            AlterColumn("dbo.PriceBreaks", "VendorUnitPrice", c => c.Decimal(nullable: false, precision: 9, scale: 3));
            AlterColumn("dbo.PriceBreaks", "ToolingSetupCharges", c => c.Decimal(precision: 18, scale: 3));
            AlterColumn("dbo.PriceBreaks", "CustomerToolingSetupCharges", c => c.Decimal(precision: 12, scale: 3));
            AlterColumn("dbo.ExtraQuantities", "Qty1", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.ExtraQuantities", "Qty2", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.ExtraQuantities", "Qty3", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.ExtraQuantities", "Qty4", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.ExtraQuantities", "Qty5", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.ExtraQuantities", "Qty6", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.ExtraQuantities", "Qty7", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.OmnaeInvoices", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.OmnaeInvoices", "UnitPrice", c => c.Decimal(nullable: false, precision: 9, scale: 3));
            AlterColumn("dbo.Orders", "SalesPrice", c => c.Decimal(precision: 18, scale: 3));
            AlterColumn("dbo.Orders", "UnitPrice", c => c.Decimal(precision: 9, scale: 3));
            AlterColumn("dbo.Orders", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Orders", "ShippedDate", c => c.DateTime());
            AlterColumn("dbo.ProductStateTrackings", "ProductId", c => c.Int());
            AlterColumn("dbo.RFQBids", "RFQQuantityId", c => c.Int());
            AlterColumn("dbo.RFQQuantities", "Qty1", c => c.Decimal(precision: 18, scale: 3));
            AlterColumn("dbo.RFQQuantities", "Qty2", c => c.Decimal(precision: 18, scale: 3));
            AlterColumn("dbo.RFQQuantities", "Qty3", c => c.Decimal(precision: 18, scale: 3));
            AlterColumn("dbo.RFQQuantities", "Qty4", c => c.Decimal(precision: 18, scale: 3));
            AlterColumn("dbo.RFQQuantities", "Qty5", c => c.Decimal(precision: 18, scale: 3));
            AlterColumn("dbo.RFQQuantities", "Qty6", c => c.Decimal(precision: 18, scale: 3));
            AlterColumn("dbo.RFQQuantities", "Qty7", c => c.Decimal(precision: 18, scale: 3));
            CreateIndex("dbo.Addresses", "CompanyId");
            CreateIndex("dbo.Companies", "BillAddressId");
            CreateIndex("dbo.Companies", "MainCompanyAddress_Id");
            CreateIndex("dbo.Companies", "CompanyBankInfoId");
            CreateIndex("dbo.BidRequestRevisions", "TaskId");
            CreateIndex("dbo.BidRequestRevisions", "RFQActionReasonId");
            CreateIndex("dbo.Documents", new[] { "ProductId", "Name", "Version" }, unique: true, name: "IX_Document");
            CreateIndex("dbo.Documents", "TaskId");
            CreateIndex("dbo.Documents", "CreatedByUserId");
            CreateIndex("dbo.Documents", "ModifiedByUserId");
            CreateIndex("dbo.Documents", "BidRequestRevisionId");
            CreateIndex("dbo.Documents", "PartRevisionId");
            CreateIndex("dbo.PartRevisions", "TaskId");
            CreateIndex("dbo.PartRevisions", "CreatedByUserId");
            CreateIndex("dbo.PartRevisions", "ModifiedByUserId");
            CreateIndex("dbo.TaskDatas", "CreatedByUserId");
            CreateIndex("dbo.TaskDatas", "ModifiedByUserId");
            CreateIndex("dbo.OmnaeInvoices", "TaskId");
            CreateIndex("dbo.OmnaeInvoices", "OrderId");
            CreateIndex("dbo.OmnaeInvoices", "CompanyId");
            CreateIndex("dbo.Orders", "TaskId");
            CreateIndex("dbo.Orders", "CreatedByUserId");
            CreateIndex("dbo.Orders", "ModifiedByUserId");
            CreateIndex("dbo.Orders", "ProductSharingId");
            CreateIndex("dbo.Orders", "CustomerId");
            CreateIndex("dbo.Orders", "ExpeditedShipmentRequestId");
            CreateIndex("dbo.Products", new[] { "CustomerId", "PartNumber", "PartNumberRevision", "VendorId" }, unique: true, name: "IX_Product");
            CreateIndex("dbo.Products", "RFQQuantityId");
            CreateIndex("dbo.Products", "CreatedByUserId");
            CreateIndex("dbo.Products", "ModifiedByUserId");
            //CreateIndex("dbo.PriceBreaks", new[] { "RFQBidId", "ProductId", "Quantity", "TaskId" }, unique: true, name: "IX_PriceBreak");
            CreateIndex("dbo.PriceBreaks", "CreatedByUserId");
            CreateIndex("dbo.PriceBreaks", "ModifiedByUserId");
            CreateIndex("dbo.PriceBreaks", "ProductPriceQuoteId");
            CreateIndex("dbo.RFQBids", "ProductId");
            CreateIndex("dbo.RFQBids", "VendorId");
            CreateIndex("dbo.RFQBids", "CreatedByUserId");
            CreateIndex("dbo.RFQBids", "ModifiedByUserId");
            CreateIndex("dbo.NCReports", "OrderId");
            CreateIndex("dbo.NCReports", "ProductId");
            CreateIndex("dbo.NCReports", "CustomerId");
            CreateIndex("dbo.NCReports", "TaskId");
            CreateIndex("dbo.OrderStateTrackings", "TaskId");
            CreateIndex("dbo.OrderStateTrackings", "CreatedByUserId");
            CreateIndex("dbo.OrderStateTrackings", "ModifiedByUserId");
            CreateIndex("dbo.ProductStateTrackings", "ProductId");
            CreateIndex("dbo.ProductStateTrackings", "CreatedByUserId");
            CreateIndex("dbo.ProductStateTrackings", "ModifiedByUserId");
            AddForeignKey("dbo.Addresses", "CompanyId", "dbo.Companies", "Id");
            AddForeignKey("dbo.Companies", "BillAddressId", "dbo.Addresses", "Id");
            AddForeignKey("dbo.Companies", "CompanyBankInfoId", "dbo.CompanyBankInfoes", "Id");
            AddForeignKey("dbo.Companies", "MainCompanyAddress_Id", "dbo.Addresses", "Id");
            AddForeignKey("dbo.Documents", "BidRequestRevisionId", "dbo.BidRequestRevisions", "Id");
            AddForeignKey("dbo.Documents", "CreatedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Documents", "ModifiedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.PartRevisions", "CreatedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.PartRevisions", "ModifiedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.TaskDatas", "CreatedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.OmnaeInvoices", "CompanyId", "dbo.Companies", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Orders", "CreatedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Orders", "CustomerId", "dbo.Companies", "Id");
            AddForeignKey("dbo.Orders", "ExpeditedShipmentRequestId", "dbo.ExpeditedShipmentRequests", "Id");

            //AddForeignKey("dbo.OmnaeInvoices", "OrderId", "dbo.Orders", "Id", cascadeDelete: false);

            AddForeignKey("dbo.Orders", "ModifiedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Products", "CreatedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Products", "ModifiedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.PriceBreaks", "CreatedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.PriceBreaks", "ModifiedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.PriceBreaks", "ProductPriceQuoteId", "dbo.ProductPriceQuotes", "Id");
            AddForeignKey("dbo.RFQBids", "CreatedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.RFQBids", "ModifiedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.RFQBids", "ProductId", "dbo.Products", "Id", cascadeDelete: true);
            AddForeignKey("dbo.RFQBids", "VendorId", "dbo.Companies", "Id", cascadeDelete: true);

            //AddForeignKey("dbo.PriceBreaks", "RFQBidId", "dbo.RFQBids", "Id");

            //AddForeignKey("dbo.Products", "RFQQuantityId", "dbo.RFQQuantities", "Id");

            AddForeignKey("dbo.Orders", "ProductSharingId", "dbo.ProductSharings", "Id");
            AddForeignKey("dbo.Orders", "TaskId", "dbo.TaskDatas", "TaskId");

            //AddForeignKey("dbo.OmnaeInvoices", "TaskId", "dbo.TaskDatas", "TaskId", cascadeDelete: true);

            AddForeignKey("dbo.TaskDatas", "ModifiedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.NCReports", "CustomerId", "dbo.Companies", "Id");
            AddForeignKey("dbo.NCReports", "OrderId", "dbo.Orders", "Id", cascadeDelete: true);

            AddForeignKey("dbo.NCReports", "ProductId", "dbo.Products", "Id", cascadeDelete: false);

            AddForeignKey("dbo.NCReports", "TaskId", "dbo.TaskDatas", "TaskId");

            //AddForeignKey("dbo.PartRevisions", "TaskId", "dbo.TaskDatas", "TaskId");

            AddForeignKey("dbo.Documents", "PartRevisionId", "dbo.PartRevisions", "Id");
            AddForeignKey("dbo.Documents", "TaskId", "dbo.TaskDatas", "TaskId");
            AddForeignKey("dbo.BidRequestRevisions", "RFQActionReasonId", "dbo.RFQActionReasons", "Id");
            AddForeignKey("dbo.BidRequestRevisions", "TaskId", "dbo.TaskDatas", "TaskId", cascadeDelete: true);
            AddForeignKey("dbo.OrderStateTrackings", "CreatedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.OrderStateTrackings", "ModifiedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.OrderStateTrackings", "TaskId", "dbo.TaskDatas", "TaskId");
            AddForeignKey("dbo.ProductStateTrackings", "CreatedByUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.ProductStateTrackings", "ModifiedByUserId", "dbo.AspNetUsers", "Id");

            //AddForeignKey("dbo.ProductStateTrackings", "ProductId", "dbo.Products", "Id");

            DropColumn("dbo.BidRequestRevisions", "RevisionReason");
            DropColumn("dbo.Companies", "UserId");
            DropColumn("dbo.Companies", "VendorTerm");
            DropColumn("dbo.Shippings", "CustomerId");
            DropColumn("dbo.Shippings", "VendorId");
            DropColumn("dbo.Shippings", "VendorAddressId");
            DropColumn("dbo.Shippings", "VendorCompany_Id");
            DropColumn("dbo.NCReports", "EvidenceImageUrl");
            DropColumn("dbo.NCReports", "CustomerCauseImageRefUrl");
            DropColumn("dbo.NCReports", "RootCauseOnCustomerImageRefUrl");
            DropColumn("dbo.TaskDatas", "OrderId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TaskDatas", "OrderId", c => c.Int());
            AddColumn("dbo.NCReports", "RootCauseOnCustomerImageRefUrl", c => c.String());
            AddColumn("dbo.NCReports", "CustomerCauseImageRefUrl", c => c.String());
            AddColumn("dbo.NCReports", "EvidenceImageUrl", c => c.String());
            AddColumn("dbo.Shippings", "VendorCompany_Id", c => c.Int());
            AddColumn("dbo.Shippings", "VendorAddressId", c => c.Int());
            AddColumn("dbo.Shippings", "VendorId", c => c.Int());
            AddColumn("dbo.Shippings", "CustomerId", c => c.Int());
            AddColumn("dbo.Companies", "VendorTerm", c => c.Int());
            AddColumn("dbo.Companies", "UserId", c => c.String(maxLength: 512));
            AddColumn("dbo.BidRequestRevisions", "RevisionReason", c => c.String());
            DropForeignKey("dbo.ProductStateTrackings", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ProductStateTrackings", "ModifiedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProductStateTrackings", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrderStateTrackings", "TaskId", "dbo.TaskDatas");
            DropForeignKey("dbo.OrderStateTrackings", "ModifiedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrderStateTrackings", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.NCRImages", "NCReportId", "dbo.NCReports");
            DropForeignKey("dbo.CompaniesCreditRelationships", "VendorId", "dbo.Companies");
            DropForeignKey("dbo.CompaniesCreditRelationships", "CustomerId", "dbo.Companies");
            DropForeignKey("dbo.VendorBidRFQStatus", "RFQActionReasonId", "dbo.RFQActionReasons");
            DropForeignKey("dbo.VendorBidRFQStatus", "ModifiedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.VendorBidRFQStatus", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.VendorBidRFQStatus", "BidRFQStatusId", "dbo.BidRFQStatus");
            DropForeignKey("dbo.VendorBidRFQStatus", "BidRequestRevisionId", "dbo.BidRequestRevisions");
            DropForeignKey("dbo.BidRFQStatus", "RFQActionReasonId", "dbo.RFQActionReasons");
            DropForeignKey("dbo.BidRFQStatus", "PartRevisionId", "dbo.PartRevisions");
            DropForeignKey("dbo.BidRFQStatus", "ModifiedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.BidRFQStatus", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.BidRequestRevisions", "TaskId", "dbo.TaskDatas");
            DropForeignKey("dbo.BidRequestRevisions", "RFQActionReasonId", "dbo.RFQActionReasons");
            DropForeignKey("dbo.RFQActionReasons", "ModifiedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.RFQActionReasons", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Documents", "TaskId", "dbo.TaskDatas");
            DropForeignKey("dbo.Documents", "PartRevisionId", "dbo.PartRevisions");
            DropForeignKey("dbo.PartRevisions", "TaskId", "dbo.TaskDatas");
            DropForeignKey("dbo.NCReports", "TaskId", "dbo.TaskDatas");
            DropForeignKey("dbo.NCReports", "ProductId", "dbo.Products");
            DropForeignKey("dbo.NCReports", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.NCReports", "CustomerId", "dbo.Companies");
            DropForeignKey("dbo.TaskDatas", "ModifiedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.OmnaeInvoices", "TaskId", "dbo.TaskDatas");
            DropForeignKey("dbo.Orders", "TaskId", "dbo.TaskDatas");
            DropForeignKey("dbo.Orders", "ProductSharingId", "dbo.ProductSharings");
            DropForeignKey("dbo.ProductSharings", "TaskId", "dbo.TaskDatas");
            DropForeignKey("dbo.ProductSharings", "SharingCompanyId", "dbo.Companies");
            DropForeignKey("dbo.ProductSharings", "OwnerCompanyId", "dbo.Companies");
            DropForeignKey("dbo.ProductSharings", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ProductSharings", "ModifiedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProductSharings", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Products", "RFQQuantityId", "dbo.RFQQuantities");
            DropForeignKey("dbo.PriceBreaks", "RFQBidId", "dbo.RFQBids");
            DropForeignKey("dbo.RFQBids", "VendorId", "dbo.Companies");
            DropForeignKey("dbo.RFQBids", "ProductId", "dbo.Products");
            DropForeignKey("dbo.RFQBids", "ModifiedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.RFQBids", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProductPriceQuotes", "ProductId", "dbo.Products");
            DropForeignKey("dbo.PriceBreaks", "ProductPriceQuoteId", "dbo.ProductPriceQuotes");
            DropForeignKey("dbo.PriceBreaks", "ModifiedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PriceBreaks", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Products", "ModifiedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Products", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Orders", "ModifiedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.OmnaeInvoices", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Orders", "ExpeditedShipmentRequestId", "dbo.ExpeditedShipmentRequests");
            DropForeignKey("dbo.Orders", "CustomerId", "dbo.Companies");
            DropForeignKey("dbo.Orders", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.OmnaeInvoices", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.TaskDatas", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PartRevisions", "ModifiedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PartRevisions", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Documents", "ModifiedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Documents", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Documents", "BidRequestRevisionId", "dbo.BidRequestRevisions");
            DropForeignKey("dbo.ApprovedCapabilities", "VendorId", "dbo.Companies");
            DropForeignKey("dbo.ShippingProfiles", "ShippingAccountId", "dbo.ShippingAccounts");
            DropForeignKey("dbo.ShippingProfiles", "ShippingId", "dbo.Shippings");
            DropForeignKey("dbo.ShippingProfiles", "ModifiedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ShippingProfiles", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.ShippingProfiles", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.ShippingAccounts", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Companies", "MainCompanyAddress_Id", "dbo.Addresses");
            DropForeignKey("dbo.Companies", "CompanyBankInfoId", "dbo.CompanyBankInfoes");
            DropForeignKey("dbo.CompanyBankInfoes", "BankAddressId", "dbo.Addresses");
            DropForeignKey("dbo.Companies", "BillAddressId", "dbo.Addresses");
            DropForeignKey("dbo.Addresses", "CompanyId", "dbo.Companies");
            DropIndex("dbo.ProductStateTrackings", new[] { "ModifiedByUserId" });
            DropIndex("dbo.ProductStateTrackings", new[] { "CreatedByUserId" });
            DropIndex("dbo.ProductStateTrackings", new[] { "ProductId" });
            DropIndex("dbo.OrderStateTrackings", new[] { "ModifiedByUserId" });
            DropIndex("dbo.OrderStateTrackings", new[] { "CreatedByUserId" });
            DropIndex("dbo.OrderStateTrackings", new[] { "TaskId" });
            DropIndex("dbo.NCRImages", new[] { "NCReportId" });
            DropIndex("dbo.CompaniesCreditRelationships", new[] { "VendorId" });
            DropIndex("dbo.CompaniesCreditRelationships", new[] { "CustomerId" });
            DropIndex("dbo.VendorBidRFQStatus", new[] { "RFQActionReasonId" });
            DropIndex("dbo.VendorBidRFQStatus", new[] { "ModifiedByUserId" });
            DropIndex("dbo.VendorBidRFQStatus", new[] { "CreatedByUserId" });
            DropIndex("dbo.VendorBidRFQStatus", new[] { "BidRFQStatusId" });
            DropIndex("dbo.VendorBidRFQStatus", new[] { "BidRequestRevisionId" });
            DropIndex("dbo.BidRFQStatus", new[] { "RFQActionReasonId" });
            DropIndex("dbo.BidRFQStatus", new[] { "ModifiedByUserId" });
            DropIndex("dbo.BidRFQStatus", new[] { "CreatedByUserId" });
            DropIndex("dbo.BidRFQStatus", new[] { "PartRevisionId" });
            DropIndex("dbo.RFQActionReasons", new[] { "ModifiedByUserId" });
            DropIndex("dbo.RFQActionReasons", new[] { "CreatedByUserId" });
            DropIndex("dbo.RFQActionReasons", "IX_VendorRejectRFQ");
            DropIndex("dbo.NCReports", new[] { "TaskId" });
            DropIndex("dbo.NCReports", new[] { "CustomerId" });
            DropIndex("dbo.NCReports", new[] { "ProductId" });
            DropIndex("dbo.NCReports", new[] { "OrderId" });
            DropIndex("dbo.ProductSharings", new[] { "ModifiedByUserId" });
            DropIndex("dbo.ProductSharings", new[] { "CreatedByUserId" });
            DropIndex("dbo.ProductSharings", new[] { "TaskId" });
            DropIndex("dbo.ProductSharings", new[] { "OwnerCompanyId" });
            DropIndex("dbo.ProductSharings", "IX_ProductSharing");
            DropIndex("dbo.RFQBids", new[] { "ModifiedByUserId" });
            DropIndex("dbo.RFQBids", new[] { "CreatedByUserId" });
            DropIndex("dbo.RFQBids", new[] { "VendorId" });
            DropIndex("dbo.RFQBids", new[] { "ProductId" });
            DropIndex("dbo.ProductPriceQuotes", new[] { "ProductId" });
            DropIndex("dbo.PriceBreaks", new[] { "ProductPriceQuoteId" });
            DropIndex("dbo.PriceBreaks", new[] { "ModifiedByUserId" });
            DropIndex("dbo.PriceBreaks", new[] { "CreatedByUserId" });
            DropIndex("dbo.PriceBreaks", "IX_PriceBreak");
            DropIndex("dbo.Products", new[] { "ModifiedByUserId" });
            DropIndex("dbo.Products", new[] { "CreatedByUserId" });
            DropIndex("dbo.Products", new[] { "RFQQuantityId" });
            DropIndex("dbo.Products", "IX_Product");
            DropIndex("dbo.ExpeditedShipmentRequests", "IX_ExpeditedShipmentRequest");
            DropIndex("dbo.Orders", new[] { "ExpeditedShipmentRequestId" });
            DropIndex("dbo.Orders", new[] { "CustomerId" });
            DropIndex("dbo.Orders", new[] { "ProductSharingId" });
            DropIndex("dbo.Orders", new[] { "ModifiedByUserId" });
            DropIndex("dbo.Orders", new[] { "CreatedByUserId" });
            DropIndex("dbo.Orders", new[] { "TaskId" });
            DropIndex("dbo.OmnaeInvoices", new[] { "CompanyId" });
            DropIndex("dbo.OmnaeInvoices", new[] { "OrderId" });
            DropIndex("dbo.OmnaeInvoices", new[] { "TaskId" });
            DropIndex("dbo.TaskDatas", new[] { "ModifiedByUserId" });
            DropIndex("dbo.TaskDatas", new[] { "CreatedByUserId" });
            DropIndex("dbo.PartRevisions", new[] { "ModifiedByUserId" });
            DropIndex("dbo.PartRevisions", new[] { "CreatedByUserId" });
            DropIndex("dbo.PartRevisions", new[] { "TaskId" });
            DropIndex("dbo.Documents", new[] { "PartRevisionId" });
            DropIndex("dbo.Documents", new[] { "BidRequestRevisionId" });
            DropIndex("dbo.Documents", new[] { "ModifiedByUserId" });
            DropIndex("dbo.Documents", new[] { "CreatedByUserId" });
            DropIndex("dbo.Documents", new[] { "TaskId" });
            DropIndex("dbo.Documents", "IX_Document");
            DropIndex("dbo.BidRequestRevisions", new[] { "RFQActionReasonId" });
            DropIndex("dbo.BidRequestRevisions", new[] { "TaskId" });
            DropIndex("dbo.ApprovedCapabilities", new[] { "VendorId" });
            DropIndex("dbo.AspNetUsers", new[] { "CompanyId" });
            DropIndex("dbo.AspNetUsers", new[] { "Active" });
            DropIndex("dbo.ShippingProfiles", new[] { "ModifiedByUserId" });
            DropIndex("dbo.ShippingProfiles", new[] { "CreatedByUserId" });
            DropIndex("dbo.ShippingProfiles", new[] { "ShippingAccountId" });
            DropIndex("dbo.ShippingProfiles", new[] { "CompanyId" });
            DropIndex("dbo.ShippingProfiles", new[] { "ShippingId" });
            DropIndex("dbo.ShippingAccounts", new[] { "CompanyId" });
            DropIndex("dbo.CompanyBankInfoes", new[] { "BankAddressId" });
            DropIndex("dbo.Companies", new[] { "CompanyBankInfoId" });
            DropIndex("dbo.Companies", new[] { "MainCompanyAddress_Id" });
            DropIndex("dbo.Companies", new[] { "BillAddressId" });
            DropIndex("dbo.Addresses", new[] { "CompanyId" });
            AlterColumn("dbo.RFQQuantities", "Qty7", c => c.Int());
            AlterColumn("dbo.RFQQuantities", "Qty6", c => c.Int());
            AlterColumn("dbo.RFQQuantities", "Qty5", c => c.Int());
            AlterColumn("dbo.RFQQuantities", "Qty4", c => c.Int());
            AlterColumn("dbo.RFQQuantities", "Qty3", c => c.Int());
            AlterColumn("dbo.RFQQuantities", "Qty2", c => c.Int());
            AlterColumn("dbo.RFQQuantities", "Qty1", c => c.Int());
            AlterColumn("dbo.RFQBids", "RFQQuantityId", c => c.Int(nullable: false));
            AlterColumn("dbo.ProductStateTrackings", "ProductId", c => c.Int(nullable: false));
            AlterColumn("dbo.Orders", "ShippedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Orders", "Quantity", c => c.Int(nullable: false));
            AlterColumn("dbo.Orders", "UnitPrice", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Orders", "SalesPrice", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.OmnaeInvoices", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.OmnaeInvoices", "Quantity", c => c.Int(nullable: false));
            AlterColumn("dbo.ExtraQuantities", "Qty7", c => c.Int());
            AlterColumn("dbo.ExtraQuantities", "Qty6", c => c.Int());
            AlterColumn("dbo.ExtraQuantities", "Qty5", c => c.Int());
            AlterColumn("dbo.ExtraQuantities", "Qty4", c => c.Int());
            AlterColumn("dbo.ExtraQuantities", "Qty3", c => c.Int());
            AlterColumn("dbo.ExtraQuantities", "Qty2", c => c.Int());
            AlterColumn("dbo.ExtraQuantities", "Qty1", c => c.Int());
            AlterColumn("dbo.PriceBreaks", "CustomerToolingSetupCharges", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.PriceBreaks", "ToolingSetupCharges", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.PriceBreaks", "VendorUnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.PriceBreaks", "UnitPrice", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.PriceBreaks", "Quantity", c => c.Int(nullable: false));
            AlterColumn("dbo.PriceBreaks", "RFQBidId", c => c.Int(nullable: false));
            AlterColumn("dbo.Products", "RFQQuantityId", c => c.Int(nullable: false));
            AlterColumn("dbo.Products", "ToolingSetupCharges", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Companies", "Name", c => c.String(maxLength: 150));
            DropColumn("dbo.TimerSetups", "TimerType");
            DropColumn("dbo.TimerSetups", "_createdAt");
            DropColumn("dbo.TimerSetups", "_updatedAt");
            DropColumn("dbo.TimerSetups", "TimerStartAt");
            DropColumn("dbo.TaskDatas", "TaskStateBeforeCustomerCancelOrder");
            DropColumn("dbo.TaskDatas", "ModifiedByUserId");
            DropColumn("dbo.TaskDatas", "CreatedByUserId");
            DropColumn("dbo.TaskDatas", "isEnterprise");
            DropColumn("dbo.RFQQuantities", "UnitOfMeasurement");
            DropColumn("dbo.RFQQuantities", "IsAddedExtraQty");
            DropColumn("dbo.RFQBids", "PreferredCurrency");
            DropColumn("dbo.RFQBids", "ModifiedByUserId");
            DropColumn("dbo.RFQBids", "CreatedByUserId");
            DropColumn("dbo.RFQBids", "HarmonizedCode");
            DropColumn("dbo.RFQBids", "QuoteAcceptDate");
            DropColumn("dbo.ProductStateTrackings", "OrderId");
            DropColumn("dbo.ProductStateTrackings", "NcrId");
            DropColumn("dbo.ProductStateTrackings", "ModifiedByUserId");
            DropColumn("dbo.ProductStateTrackings", "CreatedByUserId");
            DropColumn("dbo.OrderStateTrackings", "ModifiedByUserId");
            DropColumn("dbo.OrderStateTrackings", "CreatedByUserId");
            DropColumn("dbo.OrderStateTrackings", "NcrId");
            DropColumn("dbo.OrderStateTrackings", "TaskId");
            DropColumn("dbo.Orders", "OrderCancelledBy");
            DropColumn("dbo.Orders", "DenyCancelOrderReason");
            DropColumn("dbo.Orders", "CancelOrderReason");
            DropColumn("dbo.Orders", "IsOrderCancelled");
            DropColumn("dbo.Orders", "ExpeditedShipmentRequestId");
            DropColumn("dbo.Orders", "CustomerId");
            DropColumn("dbo.Orders", "OrderCompanyName");
            DropColumn("dbo.Orders", "ProductSharingId");
            DropColumn("dbo.Orders", "_updatedAt");
            DropColumn("dbo.Orders", "Notes");
            DropColumn("dbo.Orders", "ModifiedByUserId");
            DropColumn("dbo.Orders", "CreatedByUserId");
            DropColumn("dbo.Orders", "EarliestShippingDate");
            DropColumn("dbo.Orders", "DesireShippingDate");
            DropColumn("dbo.Orders", "Buyer");
            DropColumn("dbo.Orders", "IsReorder");
            DropColumn("dbo.Orders", "ShippingAccountNumber");
            DropColumn("dbo.Orders", "CarrierType");
            DropColumn("dbo.Orders", "TaskId");
            DropColumn("dbo.OmnaeInvoices", "PaymentRefNumber");
            DropColumn("dbo.OmnaeInvoices", "PaymentMethod");
            DropColumn("dbo.OmnaeInvoices", "PODocUri");
            DropColumn("dbo.OmnaeInvoices", "PONumber");
            DropColumn("dbo.OmnaeInvoices", "BillNumber");
            DropColumn("dbo.OmnaeInvoices", "InvoiceNumber");
            DropColumn("dbo.OmnaeInvoices", "EstimateNumber");
            DropColumn("dbo.OmnaeInvoices", "OrderId");
            DropColumn("dbo.NCReports", "_updatedAt");
            DropColumn("dbo.NCReports", "_createdAt");
            DropColumn("dbo.NCReports", "RootCauseAnalysisDate");
            DropColumn("dbo.NCReports", "NCRApprovalDate");
            DropColumn("dbo.NCReports", "TaskId");
            DropColumn("dbo.NCReports", "ArbitrateVendorCauseReason");
            DropColumn("dbo.ExtraQuantities", "NumberSampleIncluded");
            DropColumn("dbo.PriceBreaks", "UnitOfMeasurement");
            DropColumn("dbo.PriceBreaks", "ProductPriceQuoteId");
            DropColumn("dbo.PriceBreaks", "ModifiedByUserId");
            DropColumn("dbo.PriceBreaks", "CreatedByUserId");
            DropColumn("dbo.PriceBreaks", "TaskId");
            DropColumn("dbo.PartRevisions", "ModifiedByUserId");
            DropColumn("dbo.PartRevisions", "CreatedByUserId");
            DropColumn("dbo.Products", "BarCode");
            DropColumn("dbo.Products", "PreferredCurrency");
            DropColumn("dbo.Products", "OriginProductId");
            DropColumn("dbo.Products", "WasOnboarded");
            DropColumn("dbo.Products", "AnodizingType");
            DropColumn("dbo.Products", "ProcessType");
            DropColumn("dbo.Products", "ModifiedByUserId");
            DropColumn("dbo.Products", "CreatedByUserId");
            DropColumn("dbo.Products", "_updatedAt");
            DropColumn("dbo.Products", "CreatedDate");
            DropColumn("dbo.Documents", "PartRevisionId");
            DropColumn("dbo.Documents", "BidRequestRevisionId");
            DropColumn("dbo.Documents", "ModifiedByUserId");
            DropColumn("dbo.Documents", "CreatedByUserId");
            DropColumn("dbo.Documents", "TaskId");
            DropColumn("dbo.Shippings", "IsActive");
            DropColumn("dbo.Companies", "Currency");
            DropColumn("dbo.Companies", "POLegalTerms");
            DropColumn("dbo.Companies", "WasInvited");
            DropColumn("dbo.Companies", "InvitedByCompanyId");
            DropColumn("dbo.Companies", "OnboardedByCompanyId");
            DropColumn("dbo.Companies", "WasOnboarded");
            DropColumn("dbo.Companies", "IsActive");
            DropColumn("dbo.Companies", "_createdAt");
            DropColumn("dbo.Companies", "CompanyBankInfoId");
            DropColumn("dbo.Companies", "StripeCustomerId");
            DropColumn("dbo.Companies", "AccountingEmail");
            DropColumn("dbo.Companies", "isQualified");
            DropColumn("dbo.Companies", "isEnterprise");
            DropColumn("dbo.Companies", "CompanyType");
            DropColumn("dbo.Companies", "CompanyLogoUri");
            DropColumn("dbo.Companies", "MainCompanyAddress_Id");
            DropColumn("dbo.Companies", "BillAddressId");
            DropColumn("dbo.BidRequestRevisions", "RFQActionReasonId");
            DropColumn("dbo.BidRequestRevisions", "RevisionNumber");
            DropColumn("dbo.BidRequestRevisions", "CreateDateTime");
            DropColumn("dbo.Addresses", "CompanyId");
            DropColumn("dbo.Addresses", "isMailingAddress");
            DropColumn("dbo.Addresses", "isMainAddress");
            DropTable("dbo.StripeQboes");
            DropTable("dbo.NCRImages");
            DropTable("dbo.HubspotIntegrationSyncControls");
            DropTable("dbo.CompaniesCreditRelationships");
            DropTable("dbo.VendorBidRFQStatus");
            DropTable("dbo.BidRFQStatus");
            DropTable("dbo.RFQActionReasons");
            DropTable("dbo.ProductSharings");
            DropTable("dbo.ProductPriceQuotes");
            DropTable("dbo.ExpeditedShipmentRequests");
            DropTable("dbo.ApprovedCapabilities");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.ShippingProfiles");
            DropTable("dbo.ShippingAccounts");
            DropTable("dbo.CompanyBankInfoes");
            RenameIndex(table: "dbo.Shippings", name: "IX_AddressId", newName: "IX_CustomerAddressId");
            RenameIndex(table: "dbo.Shippings", name: "IX_CompanyId", newName: "IX_CustomerCompany_Id");
            RenameColumn(table: "dbo.Shippings", name: "CompanyId", newName: "CustomerCompany_Id");
            RenameColumn(table: "dbo.Shippings", name: "AddressId", newName: "CustomerAddressId");
            CreateIndex("dbo.PriceBreaks", new[] { "RFQBidId", "ProductId", "Quantity" }, unique: true, name: "IX_PriceBreak");
            CreateIndex("dbo.Products", "VendorId");
            CreateIndex("dbo.Products", new[] { "CustomerId", "PartNumber", "PartNumberRevision" }, unique: true, name: "IX_Product");
            CreateIndex("dbo.Documents", new[] { "ProductId", "Name" }, unique: true, name: "IX_Document");
            CreateIndex("dbo.Shippings", "VendorCompany_Id");
            CreateIndex("dbo.Shippings", "VendorAddressId");
            CreateIndex("dbo.BidRequestRevisions", new[] { "VendorId", "ProductId", "TaskId" }, unique: true, name: "IX_BidRequestRevision");
            AddForeignKey("dbo.Shippings", "VendorCompany_Id", "dbo.Companies", "Id");
            AddForeignKey("dbo.Shippings", "VendorAddressId", "dbo.Addresses", "Id");
        }
    }
}
