using Libs.Notification;
using Omnae.BlobStorage;
using Omnae.BusinessLayer;
using Omnae.BusinessLayer.Identity;
using Omnae.BusinessLayer.Identity.Model;
using Omnae.BusinessLayer.Interface;
using Omnae.BusinessLayer.Services;
using Omnae.Data;
using Omnae.Data.Abstracts;
using Omnae.Data.Repositories;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Model.Context;
using Omnae.Service.Service;
using Omnae.Service.Service.Interfaces;
using Omnae.WebApi.Context;
using System;
using System.Net;
using System.Web;
using System.Web.Http;
using AutoMapper;
using Omnae.WebApi.App_Start.Mapping;
using Unity;
using Unity.AspNet.Mvc;
using Unity.Injection;
using Unity.Lifetime;
using Unity.WebApi;
using Omnae.Data.StoredProcedures;
using Omnae.Libs.Notification;
using Omnae.Notification;
using Omnae.WebApi.Services;
using Serilog;
using Omnae.Hubspot;

namespace Omnae.WebApi
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();
            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            RegisterTypes(container);
            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
        }
        
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
            //container.RegisterType<ApplicationSignInManager>(new PerRequestLifetimeManager());
            //container.RegisterType<ApplicationUserManager>(new PerRequestLifetimeManager());
            //container.RegisterType<EmailService>(new PerRequestLifetimeManager());
            //container.RegisterType<SmsService>(new PerRequestLifetimeManager());
            //container.RegisterType<AuthenticationManager>(new PerRequestLifetimeManager(), new InjectionFactory(c => HttpContext.Current.GetOwinContext().Authentication));
            //container.RegisterType<IUserStore<ApplicationUser>>(new PerRequestLifetimeManager(), new InjectionFactory(c => new OmnaeUserStore(c.Resolve<ApplicationDbContext>())));

            ////////////////////////////////////////////////////////

            container.RegisterType<NotificationService>(new PerRequestLifetimeManager());
            container.RegisterType<IEmailSender, BackgroundEmailSender>(new PerRequestLifetimeManager());
            container.RegisterType<ISmsSender, SmsSender>(new PerRequestLifetimeManager());

            ////////////////////////////////////////////////////////

            container.RegisterType<IMapper>(new ContainerControlledLifetimeManager(), new InjectionFactory(c => AutoMapperConfig.CreateMapper()));

            ////////////////////////////////////////////////////////
            // Omnae Registrations
            container.RegisterType<OmnaeContext>(new PerRequestLifetimeManager(), new InjectionFactory(c => new OmnaeContext()));

            //container.RegisterType<IDbFactory, DbFactory>(new PerRequestLifetimeManager());
            container.RegisterType<IUnitOfWork, UnitOfWork>(new PerRequestLifetimeManager());

            //container.RegisterType<IMapper, Mapper>();
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

            container.RegisterType<ILogedUserContext, LogedUserApiContext>(new PerRequestLifetimeManager());
            container.RegisterType<AuthZeroManager>(new PerRequestLifetimeManager());
            container.RegisterType<IHomeBL, HomeBL>(new PerRequestLifetimeManager());

            container.RegisterType<NcrBL>(new PerRequestLifetimeManager());

            container.RegisterType<ICompaniesCreditRelationshipRepository, CompaniesCreditRelationshipRepository>(new PerRequestLifetimeManager());
            container.RegisterType<ICompaniesCreditRelationshipService, CompaniesCreditRelationshipService>(new PerRequestLifetimeManager());

            container.RegisterType<IShippingProfileService, ShippingProfileService>(new PerRequestLifetimeManager());
            container.RegisterType<IShippingProfileRepository, ShippingProfileRepository>(new PerRequestLifetimeManager());

            container.RegisterType<IAuthZeroService, AuthZeroManager>(new PerRequestLifetimeManager());

            container.RegisterType<ICompanyBankInfoService, CompanyBankInfoService>(new PerRequestLifetimeManager());
            container.RegisterType<ICompanyBankInfoRepository, CompanyBankInfoRepository>(new PerRequestLifetimeManager());

            container.RegisterType<IProductSharingService, ProductSharingService>(new PerRequestLifetimeManager());
            container.RegisterType<IProductSharingRepository, ProductSharingRepository>(new PerRequestLifetimeManager());

            container.RegisterType<DocumentStorageService>(new PerRequestLifetimeManager());

            container.RegisterType<IExpeditedShipmentRequestService, ExpeditedShipmentRequestService>(new PerRequestLifetimeManager());
            container.RegisterType<IExpeditedShipmentRequestRepository, ExpeditedShipmentRequestRepository>(new PerRequestLifetimeManager());


            container.RegisterType<IBidRFQStatusService, BidRFQStatusService>(new PerRequestLifetimeManager());
            container.RegisterType<IBidRFQStatusRepository, BidRFQStatusRepository>(new PerRequestLifetimeManager());

            container.RegisterType<IVendorBidRFQStatusService, VendorBidRFQStatusService>(new PerRequestLifetimeManager());
            container.RegisterType<IVendorBidRFQStatusRepository, VendorBidRFQStatusRepository>(new PerRequestLifetimeManager());


            container.RegisterType<IRFQActionReasonService, RFQActionReasonService>(new PerRequestLifetimeManager());
            container.RegisterType<IRFQActionReasonRepository, RFQActionReasonRepository>(new PerRequestLifetimeManager());

            container.RegisterType<CompanySyncService, CompanySyncService>(new PerRequestLifetimeManager());

            
            container.RegisterType<IProductPriceQuoteService, ProductPriceQuoteService>(new PerRequestLifetimeManager());
            container.RegisterType<IProductPriceQuoteRepository, ProductPriceQuoteRepository>(new PerRequestLifetimeManager());



        }
    }
}