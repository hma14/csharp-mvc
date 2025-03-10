using System;
using System.Data.Common;
using Omnae.Data.Configuration;
using Omnae.Model.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using Omnae.Model.Models.Aspnet;

namespace Omnae.Data
{
    public class OmnaeContext : DbContext
    {
        public OmnaeContext() : base("name=OmnaeDbContext")
        {
            Database.SetInitializer<OmnaeContext>(null);
            //Database.Log = (str) => Debug.Print(str);
        }
        public OmnaeContext(DbConnection connection) : base(connection, true)
        {
            Database.SetInitializer<OmnaeContext>(null);
            //Database.Log = (str) => Debug.Print(str);
        }

        public DbSet<SimplifiedUser> Users { get; set; }

        //public DbSet<State> States { get; set; }
        public DbSet<PriceBreak> PriceBreaks { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<StateProvince> StateProvinces { get; set; }
        public DbSet<TaskData> TaskDatas { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Order> Orders { get; set; }
        //public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Shipping> Shippings { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<OrderStateTracking> OrderStateTrackings { get; set; }
        public DbSet<ProductStateTracking> ProductStateTrackings { get; set; }
        public DbSet<PartRevision> PartRevisions { get; set; }
        public DbSet<NCReport> NCReports { get; set; }
        public DbSet<RFQBid> RFQBids { get; set; }
        public DbSet<RFQQuantity> RFQQuantities { get; set; }
        public DbSet<BidRequestRevision> BidRequestRevisions { get; set; }
        public DbSet<ExtraQuantity> ExtraQuantities { get; set; }
        public DbSet<TimerSetup> TimerSetups { get; set; }
        public DbSet<QboTokens> QboTokens { get; set; }
        public DbSet<OmnaeInvoice> OmnaeInvoices { get; set; }
        public DbSet<NCRImages> NCRImages { get; set; }
        public DbSet<ShippingAccount> ShippingAccounts { get; set; }
        public DbSet<ApprovedCapability> ApprovedCapabilities { get; set; }
        public DbSet<CompaniesCreditRelationship> CompaniesCreditRelationships { get; set; }
        public DbSet<ShippingProfile> ShippingProfiles { get; set; }
        public DbSet<CompanyBankInfo> CompanyBankInfos { get; set; }
        public DbSet<ProductSharing> ProductSharings { get; set; }
        public DbSet<ExpeditedShipmentRequest> ExpeditedShipmentRequests { get; set; }
        public DbSet<BidRFQStatus> BidRFQStatus { get; set; }
        public DbSet<VendorBidRFQStatus> VendorBidRFQStatus { get; set; }
        public DbSet<RFQActionReason> RFQActionReasons { get; set; }
        public DbSet<StripeQbo> StripeQbos { get; set; }
        public DbSet<ProductPriceQuote> ProductPriceQuotes { get; set; }



        //Internal Control for HubspotIntegration Sync mecanism
        public DbSet<HubspotIntegrationSyncControl> HubspotIntegrationSyncControl { get; set; }

        


        //////////////////////////////////

        public virtual void Commit()
        {
            SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.<SimplifiedUser>(); //This is managed by asp.net indenty.

            // modelBuilder.Configurations.Add(new ShippingConfiguration());
            modelBuilder.Configurations.Add(new OrderConfiguration());
            modelBuilder.Configurations.Add(new TaskDataConfiguration());
            modelBuilder.Configurations.Add(new CompanyConfiguration());
            //modelBuilder.Configurations.Add(new DocumentConfiguration());
            modelBuilder.Configurations.Add(new ProductConfiguration());
            //modelBuilder.Configurations.Add(new PartRevisionConfiguration());
            modelBuilder.Entity<PriceBreak>()
                .Property(p => p.UnitPrice)
                .HasPrecision(9, 3);
            modelBuilder.Entity<PriceBreak>()
                .Property(p => p.VendorUnitPrice)
                .HasPrecision(9, 3);
            modelBuilder.Entity<PriceBreak>()
                .Property(p => p.ToolingSetupCharges)
                .HasPrecision(18, 3);
            modelBuilder.Entity<PriceBreak>()
                .Property(p => p.CustomerToolingSetupCharges)
                .HasPrecision(12, 3);
            modelBuilder.Entity<PriceBreak>()
                .Property(p => p.Quantity)
                .HasPrecision(18, 3);
            modelBuilder.Entity<RFQQuantity>()
                .Property(p => p.Qty1)
                .HasPrecision(18, 3);
            modelBuilder.Entity<RFQQuantity>()
                .Property(p => p.Qty2)
                .HasPrecision(18, 3);
            modelBuilder.Entity<RFQQuantity>()
                .Property(p => p.Qty3)
                .HasPrecision(18, 3);
            modelBuilder.Entity<RFQQuantity>()
                .Property(p => p.Qty4)
                .HasPrecision(18, 3);
            modelBuilder.Entity<RFQQuantity>()
                .Property(p => p.Qty5)
                .HasPrecision(18, 3);
            modelBuilder.Entity<RFQQuantity>()
                .Property(p => p.Qty6)
                .HasPrecision(18, 3);
            modelBuilder.Entity<RFQQuantity>()
                .Property(p => p.Qty7)
                .HasPrecision(18, 3);
            modelBuilder.Entity<OmnaeInvoice>()
                .Property(p => p.UnitPrice)
                .HasPrecision(9, 3);
            modelBuilder.Entity<Order>()
                .Property(p => p.UnitPrice)
                .HasPrecision(9, 3);
            modelBuilder.Entity<Order>()
                .Property(p => p.SalesPrice)
                .HasPrecision(18, 3);
            modelBuilder.Entity<Product>()
                .Property(p => p.ToolingSetupCharges)
                .HasPrecision(18, 3);
            modelBuilder.Entity<ProductPriceQuote>()
                .HasMany<PriceBreak>(p => p.PriceBreaks)
                .WithOptional(b => b.ProductPriceQuote)
                .HasForeignKey<int?>(b => b.ProductPriceQuoteId);
        }

    }
}
