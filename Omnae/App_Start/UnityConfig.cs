using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Omnae.BlobStorage;
using Omnae.Context;
using Omnae.Data;
using Omnae.Data.Abstracts;
using Omnae.Data.Repositories;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Data.StoredProcedures;
using Omnae.Service.Service;
using Omnae.Service.Service.Interfaces;
using System;
using System.Web;
using Libs.Notification;
using Omnae.BusinessLayer;
using Unity;
using Unity.AspNet.Mvc;
using Unity.Injection;
using Unity.Lifetime;
using Omnae.BusinessLayer.Services;
using Omnae.Model.Context;
using Omnae.BusinessLayer.Identity.Model;
using Omnae.BusinessLayer.Identity;
using Omnae.BusinessLayer.Models;
using Omnae.BusinessLayer.Interface;
using Omnae.Libs.Notification;
using Omnae.Notification;
using Serilog;

namespace Omnae.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            ////////////////////////////////////////////////////////
            //Log
            container.RegisterType<ILogger>(new SingletonLifetimeManager(), new InjectionFactory(c => SerilogConfig.InitializeLog()));
            
            ////////////////////////////////////////////////////////
            //Register Asp.Net Identity
            container.RegisterType<ApplicationDbContext>(new PerRequestLifetimeManager(), new InjectionFactory(c => new ApplicationDbContext()));
            container.RegisterType<ApplicationSignInManager>(new PerRequestLifetimeManager());
            container.RegisterType<ApplicationUserManager>(new PerRequestLifetimeManager());
            container.RegisterType<EmailService>(new PerRequestLifetimeManager());
            container.RegisterType<SmsService>(new PerRequestLifetimeManager());
            container.RegisterType<IAuthenticationManager>(new PerRequestLifetimeManager(), new InjectionFactory(c => HttpContext.Current.GetOwinContext().Authentication));
            container.RegisterType<IUserStore<ApplicationUser>>(new PerRequestLifetimeManager(), new InjectionFactory(c => new OmnaeUserStore( c.Resolve<ApplicationDbContext>() )));

            ////////////////////////////////////////////////////////
            
            container.RegisterType<NotificationService>(new PerRequestLifetimeManager());
            container.RegisterType<IEmailSender, BackgroundEmailSender>(new PerRequestLifetimeManager());
            container.RegisterType<ISmsSender, SmsSender>(new PerRequestLifetimeManager());
            
            ////////////////////////////////////////////////////////
            // Omnae Registrations
            container.RegisterType<OmnaeContext>(new PerRequestLifetimeManager(), new InjectionFactory(c => new OmnaeContext()));

            //container.RegisterType<IDbFactory, DbFactory>(new PerRequestLifetimeManager());
            container.RegisterType<IUnitOfWork, UnitOfWork>(new PerRequestLifetimeManager());

            container.RegisterType<IMapper>(new ContainerControlledLifetimeManager(), new InjectionFactory(c => AutoMapperConfig.CreateMapper()));
            container.RegisterType<IDocumentStorageService, DocumentStorageService>(new PerRequestLifetimeManager());
            container.RegisterType<IImageStorageService, ImagesStorageService>(new PerRequestLifetimeManager());

            container.RegisterType<ITaskDataRepository, TaskDataRepository>(new PerRequestLifetimeManager());
            container.RegisterType<ITaskDataService, TaskDataService>(new PerRequestLifetimeManager());

            container.RegisterType<IStateProvinceRepository, StateProvinceRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IStateProvinceService, StateProvinceService>(new PerRequestLifetimeManager());

            container.RegisterType<ICountryRepository, CountryRepository>(new PerRequestLifetimeManager());
            container.RegisterType<ICountryService, CountryService>(new PerRequestLifetimeManager());

            container.RegisterType<IAddressRepository, AddressRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IAddressService, AddressService>(new PerRequestLifetimeManager());

            container.RegisterType<ICompanyRepository, CompanyRepository>(new PerRequestLifetimeManager());
            container.RegisterType<ICompanyService, CompanyService>(new PerRequestLifetimeManager());

            container.RegisterType<IShippingRepository, ShippingRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IShippingService, ShippingService>(new PerRequestLifetimeManager());

            container.RegisterType<IProductRepository, ProductRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IProductService, ProductService>(new PerRequestLifetimeManager());

            container.RegisterType<IDocumentRepository, DocumentRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IDocumentService, DocumentService>(new PerRequestLifetimeManager());

            container.RegisterType<IPriceBreakRepository, PriceBreakRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IPriceBreakService, PriceBreakService>(new PerRequestLifetimeManager());

            container.RegisterType<IOrderRepository, OrderRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IOrderService, OrderService>(new PerRequestLifetimeManager());

            container.RegisterType<IOrderStateTrackingRepository, OrderStateTrackingRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IOrderStateTrackingService, OrderStateTrackingService>(new PerRequestLifetimeManager());

            container.RegisterType<IProductStateTrackingRepository, ProductStateTrackingRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IProductStateTrackingService, ProductStateTrackingService>(new PerRequestLifetimeManager());

            container.RegisterType<IPartRevisionRepository, PartRevisionRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IPartRevisionService, PartRevisionService>(new PerRequestLifetimeManager());

            container.RegisterType<INCReportRepository, NCReportRepository>(new PerRequestLifetimeManager());
            container.RegisterType<INCReportService, NCReportService>(new PerRequestLifetimeManager());

            container.RegisterType<IRFQBidRepository, RFQBidRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IRFQBidService, RFQBidService>(new PerRequestLifetimeManager());

            container.RegisterType<IRFQQuantityRepository, RFQQuantityRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IRFQQuantityService, RFQQuantityService>(new PerRequestLifetimeManager());

            container.RegisterType<IExtraQuantityRepository, ExtraQuantityRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IExtraQuantityService, ExtraQuantityService>(new PerRequestLifetimeManager());

            container.RegisterType<IBidRequestRevisionRepository, BidRequestRevisionRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IBidRequestRevisionService, BidRequestRevisionService>(new PerRequestLifetimeManager());

            container.RegisterType<ITimerSetupRepository, TimerSetupRepository>(new PerRequestLifetimeManager());
            container.RegisterType<ITimerSetupService, TimerSetupService>(new PerRequestLifetimeManager());

            container.RegisterType<IQboTokensRepository, QboTokensRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IQboTokensService, QboTokensService>(new PerRequestLifetimeManager());

            container.RegisterType<IOmnaeInvoiceRepository, OmnaeInvoiceRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IOmnaeInvoiceService, OmnaeInvoiceService>(new PerRequestLifetimeManager());

            container.RegisterType<INCRImagesRepository, NCRImagesRepository>(new PerRequestLifetimeManager());
            container.RegisterType<INCRImagesService, NCRImagesService>(new PerRequestLifetimeManager());

            container.RegisterType<IShippingAccountRepository, ShippingAccountRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IShippingAccountService, ShippingAccountService>(new PerRequestLifetimeManager());

            container.RegisterType<IApprovedCapabilityRepository, ApprovedCapabilityRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IApprovedCapabilityService, ApprovedCapabilityService>(new PerRequestLifetimeManager());

            container.RegisterType<IStoredProcedure, StoredProcedure>(new PerRequestLifetimeManager());
            container.RegisterType<IStoredProcedureService, StoredProcedureService>(new PerRequestLifetimeManager());

            container.RegisterType<TimerTriggerService, TimerTriggerService>(new PerRequestLifetimeManager());
            container.RegisterType<DashboardBL, DashboardBL>(new PerRequestLifetimeManager());
            container.RegisterType<OrdersBL, OrdersBL>(new PerRequestLifetimeManager());
            container.RegisterType<DocumentBL, DocumentBL>(new PerRequestLifetimeManager());
            container.RegisterType<TaskDataCustomerBL, TaskDataCustomerBL>(new PerRequestLifetimeManager());


            container.RegisterType<ChartBL, ChartBL>(new PerRequestLifetimeManager());
            container.RegisterType<ProductBL, ProductBL>(new PerRequestLifetimeManager());
            container.RegisterType<ShipmentBL, ShipmentBL>(new PerRequestLifetimeManager());
            container.RegisterType<PaymentBL, PaymentBL>(new PerRequestLifetimeManager());
            container.RegisterType<IHomeBL, HomeBL>(new PerRequestLifetimeManager());
            
            container.RegisterType<NcrBL>(new PerRequestLifetimeManager());

            container.RegisterType<ILogedUserContext, LogedUserContext>(new SessionLifetimeManager());
            container.RegisterType<LogedUserContext, LogedUserContext>(new SessionLifetimeManager());


            container.RegisterType<ICompaniesCreditRelationshipRepository, CompaniesCreditRelationshipRepository>(new PerRequestLifetimeManager());
            container.RegisterType<ICompaniesCreditRelationshipService, CompaniesCreditRelationshipService>(new PerRequestLifetimeManager());

            container.RegisterType<IShippingProfileService, ShippingProfileService>(new PerRequestLifetimeManager());
            container.RegisterType<IShippingProfileRepository, ShippingProfileRepository>(new PerRequestLifetimeManager());

            container.RegisterType<IAuthZeroService, NoImpAuthZeroService>(new PerRequestLifetimeManager());

            container.RegisterType<IProductSharingService, ProductSharingService>(new PerRequestLifetimeManager());
            container.RegisterType<IProductSharingRepository, ProductSharingRepository>(new PerRequestLifetimeManager());

            container.RegisterType<DocumentStorageService>(new PerRequestLifetimeManager());

            container.RegisterType<IBidRFQStatusService, BidRFQStatusService>(new PerRequestLifetimeManager());
            container.RegisterType<IBidRFQStatusRepository, BidRFQStatusRepository>(new PerRequestLifetimeManager());

            container.RegisterType<IVendorBidRFQStatusService, VendorBidRFQStatusService>(new PerRequestLifetimeManager());
            container.RegisterType<IVendorBidRFQStatusRepository, VendorBidRFQStatusRepository>(new PerRequestLifetimeManager());

            container.RegisterType<IRFQActionReasonService, RFQActionReasonService>(new PerRequestLifetimeManager());
            container.RegisterType<IRFQActionReasonRepository, RFQActionReasonRepository>(new PerRequestLifetimeManager());

            container.RegisterType<IStripeQboService, StripeQboService>(new PerRequestLifetimeManager());
            container.RegisterType<IStripeQboRepository, StripeQboRepository>(new PerRequestLifetimeManager());

            container.RegisterType<IProductPriceQuoteService, ProductPriceQuoteService>(new PerRequestLifetimeManager());
            container.RegisterType<IProductPriceQuoteRepository, ProductPriceQuoteRepository>(new PerRequestLifetimeManager());

        }
    }

    public class SessionLifetimeManager : LifetimeManager
    {
        private string _key = Guid.NewGuid().ToString();

        public override object GetValue(ILifetimeContainer container = null)
        {
            return HttpContext.Current.Session[_key];
        }

        public override void SetValue(object newValue, ILifetimeContainer container = null)
        {
            HttpContext.Current.Session[_key] = newValue;
        }

        public override void RemoveValue(ILifetimeContainer container = null)
        {
            (this.GetValue(null) as IDisposable)?.Dispose();
            HttpContext.Current.Session.Remove(_key);
        }

        protected override LifetimeManager OnCreateLifetimeManager()
        {
            return (LifetimeManager) new SessionLifetimeManager();
        }
    }
}
