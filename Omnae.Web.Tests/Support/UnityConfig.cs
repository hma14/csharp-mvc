using System;
using System.Linq;
using Libs.Notification;
using Omnae.Context;
using Omnae.Models;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Omnae.BlobStorage;
using Omnae.Data;
using Omnae.Data.Abstracts;
using Omnae.Data.Repositories;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Data.StoredProcedures;
using Omnae.Service.Service;
using Omnae.Service.Service.Interfaces;
using Moq;
using Omnae.BusinessLayer.Identity;
using Omnae.BusinessLayer.Identity.Model;
using Omnae.Libs;
using Omnae.Libs.Notification;
using Unity.AspNet.Mvc;
using Omnae.Web.Tests.Fakes;

namespace Omnae.Web.Tests.Support
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityTestConfig
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
            //Register Asp.Net Identity
            container.RegisterType<ApplicationDbContext>(new HierarchicalLifetimeManager(), new InjectionFactory(c => GetFakeDbContextUser()));
            container.RegisterType<ApplicationSignInManager>(new HierarchicalLifetimeManager());
            container.RegisterType<ApplicationUserManager>(new HierarchicalLifetimeManager());
            container.RegisterType<EmailService>(new HierarchicalLifetimeManager());
            container.RegisterType<SmsService>(new HierarchicalLifetimeManager());
            container.RegisterType<IAuthenticationManager>(new HierarchicalLifetimeManager(), new InjectionFactory(c => GetAuthenticationManagerMock()));
            container.RegisterType<IUserStore<ApplicationUser>>(new HierarchicalLifetimeManager(), new InjectionFactory(c => new OmnaeUserStore( c.Resolve<ApplicationDbContext>() )));

            ////////////////////////////////////////////////////////
            
            container.RegisterType<NotificationService>(new PerRequestLifetimeManager());
            container.RegisterType<IEmailSender, EmailSenderFake>(new PerRequestLifetimeManager());
            container.RegisterType<ISmsSender, SmsSenderFake>(new PerRequestLifetimeManager());
            
            ////////////////////////////////////////////////////////
            // Omnae Registrations
            container.RegisterType<OmnaeContext>(new HierarchicalLifetimeManager(), new InjectionFactory(c => GetFakeDbContext()));

            //container.RegisterType<IDbFactory, DbFactory>(new HierarchicalLifetimeManager());
            container.RegisterType<IUnitOfWork, UnitOfWork>(new HierarchicalLifetimeManager());

            //container.RegisterType<IMapper, Mapper>();
            container.RegisterType<IStorageService>(new HierarchicalLifetimeManager(), new InjectionFactory(c => GetBlobStorageServiceMock()));

            container.RegisterType<ITaskDataRepository, TaskDataRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<ITaskDataService, TaskDataService>(new HierarchicalLifetimeManager());

            container.RegisterType<IStateProvinceRepository, StateProvinceRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IStateProvinceService, StateProvinceService>(new HierarchicalLifetimeManager());

            container.RegisterType<ICountryRepository, CountryRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<ICountryService, CountryService>(new HierarchicalLifetimeManager());

            container.RegisterType<IAddressRepository, AddressRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IAddressService, AddressService>(new HierarchicalLifetimeManager());

            container.RegisterType<ICompanyRepository, CompanyRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<ICompanyService, CompanyService>(new HierarchicalLifetimeManager());

            container.RegisterType<IShippingRepository, ShippingRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IShippingService, ShippingService>(new HierarchicalLifetimeManager());

            container.RegisterType<IProductRepository, ProductRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IProductService, ProductService>(new HierarchicalLifetimeManager());

            container.RegisterType<IDocumentRepository, DocumentRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IDocumentService, DocumentService>(new HierarchicalLifetimeManager());

            container.RegisterType<IPriceBreakRepository, PriceBreakRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IPriceBreakService, PriceBreakService>(new HierarchicalLifetimeManager());

            container.RegisterType<IOrderRepository, OrderRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IOrderService, OrderService>(new HierarchicalLifetimeManager());

            container.RegisterType<IOrderStateTrackingRepository, OrderStateTrackingRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IOrderStateTrackingService, OrderStateTrackingService>(new HierarchicalLifetimeManager());

            container.RegisterType<IProductStateTrackingRepository, ProductStateTrackingRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IProductStateTrackingService, ProductStateTrackingService>(new HierarchicalLifetimeManager());

            container.RegisterType<IPartRevisionRepository, PartRevisionRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IPartRevisionService, PartRevisionService>(new HierarchicalLifetimeManager());

            container.RegisterType<INCReportRepository, NCReportRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<INCReportService, NCReportService>(new HierarchicalLifetimeManager());

            container.RegisterType<IRFQBidRepository, RFQBidRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IRFQBidService, RFQBidService>(new HierarchicalLifetimeManager());

            container.RegisterType<IRFQQuantityRepository, RFQQuantityRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IRFQQuantityService, RFQQuantityService>(new HierarchicalLifetimeManager());

            container.RegisterType<IExtraQuantityRepository, ExtraQuantityRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IExtraQuantityService, ExtraQuantityService>(new HierarchicalLifetimeManager());

            container.RegisterType<IBidRequestRevisionRepository, BidRequestRevisionRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IBidRequestRevisionService, BidRequestRevisionService>(new HierarchicalLifetimeManager());

            container.RegisterType<ITimerSetupRepository, TimerSetupRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<ITimerSetupService, TimerSetupService>(new HierarchicalLifetimeManager());

            container.RegisterType<IQboTokensRepository, QboTokensRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IQboTokensService, QboTokensService>(new HierarchicalLifetimeManager());

            container.RegisterType<IOmnaeInvoiceRepository, OmnaeInvoiceRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IOmnaeInvoiceService, OmnaeInvoiceService>(new HierarchicalLifetimeManager());

            container.RegisterType<INCRImagesRepository, NCRImagesRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<INCRImagesService, NCRImagesService>(new HierarchicalLifetimeManager());
            
            container.RegisterType<IStoredProcedure, StoredProcedure>(new HierarchicalLifetimeManager());
            container.RegisterType<IStoredProcedureService, StoredProcedureService>(new HierarchicalLifetimeManager());


            container.RegisterType<LogedUserContext>(new TransientLifetimeManager());
        }

        private static ApplicationDbContext GetFakeDbContextUser()
        {
            var loader = new Effort.DataLoaders.CsvDataLoader(@".\Data\");
            var fakeConnection = Effort.DbConnectionFactory.CreateTransient(loader);
            var dbContext = new ApplicationDbContext(fakeConnection);

            return dbContext;
        }

        private static OmnaeContext GetFakeDbContext()
        {
            var loader = new Effort.DataLoaders.CsvDataLoader(@".\Data\");
            var fakeConnection = Effort.DbConnectionFactory.CreateTransient(loader);
            var dbContext = new OmnaeContext(fakeConnection);
            dbContext.Database.CreateIfNotExists();

            return dbContext;
        }

        private static IAuthenticationManager GetAuthenticationManagerMock()
        {
            var mockManager = new Mock<IAuthenticationManager>();
            return mockManager.Object;
        }

        private static IStorageService GetBlobStorageServiceMock()
        {
            var mockManager = new Mock<IStorageService>();
            return mockManager.Object;
        }
    }
}
