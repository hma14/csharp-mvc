using Common;
using Intuit.Ipp.Data;
using Intuit.Ipp.OAuth2PlatformClient;
using Libs;
using Libs.Notification;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Azure;
using Microsoft.Owin.Security;
using Omnae.BlobStorage;
using Omnae.BusinessLayer;
using Omnae.BusinessLayer.Identity;
using Omnae.BusinessLayer.Identity.Model;
using Omnae.BusinessLayer.Interface;
using Omnae.BusinessLayer.Models;
using Omnae.BusinessLayer.Services;
using Omnae.Common;
using Omnae.Context;
using Omnae.Data;
using Omnae.Filters;
using Omnae.Libs;
using Omnae.Libs.ViewModels;
using Omnae.Model.Context;
using Omnae.Model.Models;
using Omnae.Models;
using Omnae.QuickBooks.QBO;
using Omnae.QuickBooks.ViewModels;
using Omnae.Service.Service;
using Omnae.Service.Service.Interfaces;
using Omnae.ViewModels;
using Rotativa;
using Stateless;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Azure.Core.Pipeline;
using Azure.Storage.Queues;
using Omnae.BusinessLayer.Util;
using Omnae.Libs.Notification;
using Unity;
using Company = Omnae.Model.Models.Company;
using IMapper = AutoMapper.IMapper;
using Task = System.Threading.Tasks.Task;
using Humanizer;

namespace Omnae.Controllers
{
    [CustomHandleError]
    public class HomeController : BaseController
    {
        private DashboardBL DashboardBL { get; }
        private ApplicationUserManager UserManager { get; }
        private IAuthenticationManager AuthenticationManager { get; }
        private IUnityContainer Container { get; }
        private IHomeBL HomeBl { get; }
        private TaskDatasBL TaskDatasBl { get; }
        private ChartBL ChartBl { get; }
        private DocumentBL DocumentBL { get; }
        private TaskSetup TaskSetup { get; }
        private ICompaniesCreditRelationshipService CreditRelationshipService { get; }
        private OnBoardingBL OnBoardingBl { get; }
        protected ProductBL ProductBl { get; }

        public HomeController(IRFQBidService rfqBidService, ICompanyService companyService, ITaskDataService taskDataService, IPriceBreakService priceBreakService,
                              IOrderService orderService, ILogedUserContext userContext, IProductService productService, IDocumentService documentService,
                              IShippingService shippingService, ICountryService countryService, IAddressService addressService, IStateProvinceService stateProvinceService,
                              IOrderStateTrackingService orderStateTrackingService, IProductStateTrackingService productStateTrackingService,
                              IPartRevisionService partRevisionService, IStoredProcedureService spService, INCReportService ncReportService,
                              IRFQQuantityService rfqQuantityService, IExtraQuantityService extraQuantityService, IBidRequestRevisionService bidRequestRevisionService,
                              ITimerSetupService timerSetupService, IQboTokensService qboTokensService, IOmnaeInvoiceService omnaeInvoiceService,
                              INCRImagesService ncrImagesService, IApprovedCapabilityService approvedCapabilityService, IShippingAccountService shippingAccountService,
                              ApplicationDbContext dbUser, ProductBL productBl, NotificationService notificationService, UserContactService userContactService,
                              TimerTriggerService timerTriggerService, NotificationBL notificationBl, PaymentBL paymentBl, ShipmentBL shipmentBl,
                              ChartBL chartBl, IMapper mapper, NcrBL ncrBL, IDocumentStorageService documentStorageService, IImageStorageService imageStorageService,
                              DashboardBL dashboardBl, ApplicationUserManager userManager, IAuthenticationManager authenticationManager, IUnityContainer container,
                              IHomeBL homeBl, TaskDatasBL taskDatasBl, DocumentBL documentBl, TaskSetup taskSetup, ICompaniesCreditRelationshipService creditRelationshipService,
                              ProductBL ProductBl,
                              OnBoardingBL onBoardingBl) : base(rfqBidService, companyService, taskDataService, priceBreakService, orderService, userContext, productService, documentService, shippingService, countryService, addressService, stateProvinceService, orderStateTrackingService, productStateTrackingService, partRevisionService, spService, ncReportService, rfqQuantityService, extraQuantityService, bidRequestRevisionService, timerSetupService, qboTokensService, omnaeInvoiceService, ncrImagesService, approvedCapabilityService, shippingAccountService, dbUser, productBl, notificationService, userContactService, timerTriggerService, notificationBl, paymentBl, shipmentBl, chartBl, mapper, ncrBL, documentStorageService, imageStorageService)
        {
            DashboardBL = dashboardBl;
            UserManager = userManager;
            AuthenticationManager = authenticationManager;
            Container = container;
            HomeBl = homeBl;
            TaskDatasBl = taskDatasBl;
            ChartBl = chartBl;
            DocumentBL = documentBl;
            TaskSetup = taskSetup;
            CreditRelationshipService = creditRelationshipService;
            OnBoardingBl = onBoardingBl;
            this.ProductBl = ProductBl;
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View();
            }

            if (UserContext?.User?.Confirmed != true)
            {
                ViewBag.Message = "Check your email and confirm your account, you must be confirmed before you can log in.";
                return View("Info");
            }

            if (UserContext.Company == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<TaskData> model = new List<TaskData>();
            try
            {
                model = DashboardBL.GetDashboardData();
            }
            catch (Exception)
            {
                RedirectToAction("Login", "Account");
            }
            return CreatePartialView(UserContext.UserType, model);
        }


        [HttpGet]
        public ActionResult DisplayRFQs()
        {
            if (UserContext.Company == null)
            {
                return RedirectToAction("Index");
            }

            if (UserContext.UserType != USER_TYPE.Vendor)
            {
                return View();
            }

            var model = DashboardBL.GetDisplayRFQs();
            return View(model);
        }


        [HttpGet]
        public ActionResult DisplayOrders()
        {
            if (UserContext.Company == null)
            {
                return RedirectToAction("Index");
            }

            if (UserContext.UserType != USER_TYPE.Vendor)
            {
                return View();
            }

            var model = DashboardBL.GetDisplayOrders();
            return View(model);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// TODO Refactor and Remove
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [OutputCache(Duration = 300, VaryByParam = "User", Location = OutputCacheLocation.Client)]
        public PartialViewResult PartialReorders()
        {
            var model = DashboardBL.GetDashboardDataCustomer().ReOrders; //TODO: Can improve performance here
            return PartialView("_CustomerReorders", model);
        }

        [OutputCache(Duration = 300, VaryByParam = "User", Location = OutputCacheLocation.Client)]
        public PartialViewResult PartialVendorCompleteProducts()
        {
            var model = DashboardBL.GetDashboardDataVendor().Products; //TODO: Can improve performance here

            var rows = int.Parse(ConfigurationManager.AppSettings["MaxIconsInGrid"]); //TODO: Put this in Configuration
            model = model.Take(rows).ToList();

            return PartialView("_VendorCompleteProducts", model);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private ActionResult CreatePartialView(USER_TYPE userType, List<TaskData> list, bool isForSearch = false)
        {
            if (list == null)
                throw new Exception(IndicatingMessages.TaskNotFound);

            switch (userType)
            {
                case USER_TYPE.Customer:
                    {
                        var model = DashboardBL.ProcessDashboardCustomer(list);
                        model.OrderTrackings = model.OrderTrackings.Where(x => (x.IsForSearch = isForSearch)).ToList();

                        //TODO: Remove this TempData and the Session
                        TempData["CustomerNewlyUpdates"] = model.NewlyUpdates;
                        TempData["CustomerOrderTrackings"] = model.OrderTrackings;
                        TempData["CustomerFirstOrders"] = model.FirstOrders;
                        TempData["ReOrders"] = model.ReOrders;
                        TempData.Keep();

                        return PartialView("_CustomerLanding", model);
                    }
                // Admin //TODO Refactor order to improve performance.
                case USER_TYPE.Vendor:
                    {
                        var model = DashboardBL.ProcessDashboardVendorData(list);

                        //TODO: Remove this TempData and the Session
                        Session["VendorProducts"] = model.Products;

                        return PartialView("_VendorLanding", model);
                    }
                case USER_TYPE.Admin:
                    {
                        var tasks = TaskDataService.GetTasksThatNeedAdminAtention();
                        List<TaskViewModel> tdvmList = new List<TaskViewModel>();

                        foreach (var td in tasks)
                        {
                            if (td.Product != null)
                            {
                                td.Product.AdminId = UserContext.UserId;
                                Order order = td.Orders?.LastOrDefault();
                                if (order == null)
                                {
                                    //order = OrderService.FindOrdersByProductId(td.ProductId.Value).LastOrDefault();
                                    var ncr = NcReportService.FindNCReportByTaskId(td.TaskId);
                                    if (ncr != null)
                                    {
                                        order = ncr.Order;
                                    }
                                }
                                TaskViewModel tvm = new TaskViewModel
                                {
                                    UserType = UserContext.UserType,
                                    TaskData = td,
                                    Order = order,
                                    VendorName = GetVendorName(td),
                                    ProductDetailsVM = td.Product != null ? SetupProductDetailsVM(td, order?.Id) : null,
                                    ProductFileVM = td.Product != null ? SetupProductFilesVM(td) : null,
                                    NcrDescriptionVM = TaskSetup.SetupNcrDescriptionViewModel(td),
                                    EnumName = Omnae.Common.Extensions.StringExtensions.SplitCamelCase(Enum.GetName(typeof(States), td.StateId)),
                                    MyFunc = () => TaskSetup.CheckPreconditions(td.ProductId.Value, td.TaskId),
                                };

                                tdvmList.Add(tvm);
                            }
                        }
                        return PartialView("_AdminLanding", tdvmList);
                    }
                default: throw new InvalidOperationException("Unknown User Type");
            }
        }

        ///////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////

        [HttpGet]
        public ActionResult Search(string search)
        {
            if (UserContext.Company != null)
            {
                var model = HomeBl.Search(search, UserContext.Company.Id).ToList();
                return CreatePartialView(UserContext.UserType, model, true);
            }
            else
            {
                return null;
            }
        }

        [HttpGet]
        public ActionResult Filter(FILTERS filter)
        {
            var model = new List<TaskData>();
            if (UserContext.Company == null)
            {
                return CreatePartialView(UserContext.UserType, model);
            }

            var taskDatas = UserContext.UserType == USER_TYPE.Customer
                ? TaskDataService.FindTaskDataByCustomerId(UserContext.Company.Id)
                : TaskDataService.FindTaskDataByVendorId(UserContext.Company.Id);

            if (filter == FILTERS.Tagged)
            {
                model = taskDatas.Where(x => x.isTagged == true).OrderByDescending(x => x.ModifiedUtc).ToList();
            }
            else if (filter == FILTERS.Alert && UserContext.UserType == USER_TYPE.Customer)
            {
                model = taskDatas.Where(x => x.StateId == (int)States.OrderInitiated ||
                                             x.StateId == (int)States.ReOrderInitiated ||
                                             x.StateId == (int)States.ProofingComplete ||
                                             x.StateId == (int)States.SampleComplete ||
                                             x.StateId == (int)States.QuoteAccepted ||
                                             x.StateId == (int)States.NCRCustomerApproval ||
                                             x.StateId == (int)States.NCRCustomerCorrectivePartsReceived ||
                                             x.StateId == (int)States.NCRRootCauseDisputes ||
                                             x.StateId == (int)States.NCRVendorCorrectivePartsComplete ||
                                             x.StateId == (int)States.NCRCustomerRevisionNeeded
                    )
                    .OrderBy(x => x.Product.PartNumber).ToList();
            }
            else if (filter == FILTERS.Alert && UserContext.UserType == USER_TYPE.Vendor)
            {
                model = taskDatas.Where(x => x.StateId == (int)States.OutForRFQ ||
                                             x.StateId == (int)States.RFQRevision ||
                                             x.StateId == (int)States.OrderPaid ||
                                             x.StateId == (int)States.ProofingStarted ||
                                             x.StateId == (int)States.ProofRejected ||
                                             x.StateId == (int)States.ReOrderPaid ||
                                             x.StateId == (int)States.OrderPaid ||
                                             x.StateId == (int)States.ProofApproved ||
                                             x.StateId == (int)States.ToolingStarted ||
                                             x.StateId == (int)States.SampleRejected ||
                                             x.StateId == (int)States.SampleApproved ||
                                             x.StateId == (int)States.NCRClosed ||
                                             x.StateId == (int)States.ProductionStarted ||
                                             x.StateId == (int)States.NCRCustomerStarted ||
                                             x.StateId == (int)States.NCRVendorRootCauseAnalysis ||
                                             x.StateId == (int)States.NCRVendorCorrectivePartsInProduction
                    )
                    .OrderBy(x => x.Product.PartNumber).ToList();
            }
            else if (filter == FILTERS.RevisonRequired)
            {
                model = taskDatas.Where(x => x.StateId == (int)States.RFQRevision).OrderBy(x => x.Product.PartNumber).ToList();
            }
            else if (filter == FILTERS.Proofing)
            {
                model = taskDatas.Where(x => x.StateId >= (int)States.ProofingStarted && x.StateId <= (int)States.ProofRejected).OrderBy(x => x.Product.PartNumber).ToList();
            }
            else if (filter == FILTERS.Sampling)
            {
                model = taskDatas.Where(x => x.StateId >= (int)States.SampleStarted && x.StateId <= (int)States.SampleRejected).OrderBy(x => x.Product.PartNumber).ToList();
            }
            else if (filter == FILTERS.InProduction)
            {
                model = taskDatas.Where(x => x.StateId == (int)States.ProductionStarted).OrderBy(x => x.Product.PartNumber).ToList();
            }
            else if (filter == FILTERS.RemoveFilter)
            {
                model = taskDatas;
            }

            return CreatePartialView(UserContext.UserType, model);
        }

        public ActionResult Alerts()
        {
            if (UserContext.Company == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var taskDatas = UserContext.UserType == USER_TYPE.Customer
                ? TaskDataService.FindTaskDataByCustomerId(UserContext.Company.Id)
                : TaskDataService.FindTaskDataByVendorId(UserContext.Company.Id);

            var model = taskDatas
                .Where(x => x.StateId == (int)States.BackFromRFQ ||
                            x.StateId == (int)States.BidReview && x.isEnterprise == true ||
                            x.StateId == (int)States.OrderInitiated ||
                            x.StateId == (int)States.ReOrderInitiated ||
                            x.StateId == (int)States.ProofingComplete ||
                            x.StateId == (int)States.SampleComplete ||
                            x.StateId == (int)States.NCRCustomerCorrectivePartsAccepted && x.isEnterprise == true ||
                            x.StateId == (int)States.QuoteAccepted)
                .OrderByDescending(x => x.ModifiedUtc).ToList();

            var taskVMList = new List<TaskViewModel>();
            foreach (var td in model)
            {
                DocumentService.UpdateDocUrlWithSecurityToken(td?.Product?.Documents);

                Order order = td.Orders?.LastOrDefault();
                if (order == null)
                {
                    order = OrderService.FindOrdersByProductId(td.ProductId.Value).LastOrDefault();
                }
                TaskViewModel tvm = new TaskViewModel
                {
                    UserType = UserContext.UserType,
                    TaskData = td,
                    Order = order,
                    EnumName = Omnae.Common.Extensions.StringExtensions.SplitCamelCase(Enum.GetName(typeof(States), td.StateId)),
                    ChkPreconditions = td.StateId == (int)States.ProductionComplete ? TaskSetup.CheckPreconditions(td) : (bool?)null,
                    NcrDescriptionVM = TaskSetup.SetupNcrDescriptionViewModel(td),
                    ProductFileVM = td.Product != null ? SetupProductFilesVM(td) : null,
                    Docs = td.Product.Documents.ToList(),
                };
                taskVMList.Add(tvm);
            }
            TempData["CustomerNewlyUpdates"] = taskVMList;
            return View("_Alerts", taskVMList);
        }

        [HttpGet]
        public ActionResult ContinueRegistration(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            int usaCode = (int)STATE_PROVINCE_CODE.US_STATE_CODE;
            int canadaCode = (int)STATE_PROVINCE_CODE.CA_PROVINCE_CODE;
            int selectedProvince = 59;    // 59: BC

            ViewBag.CountryList = new SelectList(CountryService.FindAllCountries(), "Id", "CountryName", (int)COUNTRY_ID.CA);
            ViewBag.States = new SelectList(StateProvinceService.FindStateProvinceByCode(usaCode), "Id", "Name");
            ViewBag.Provinces = new SelectList(StateProvinceService.FindStateProvinceByCode(canadaCode), "Id", "Name", selectedProvince);

            var model = new ContinueRegistrationViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ContinueRegistration(ContinueRegistrationViewModel model, bool? isBilling, bool? isShipping)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMsg"] = "ModelState is invalid";
                return RedirectToAction("ContinueRegistration");
            }

            try
            {
                using (var trans = AsyncTransactionScope.StartNew())
                {
                    var userEntity = DbUser.Users.FirstOrDefault(u => u.Id == UserContext.UserId);
                    if (userEntity == null)
                    {
                        TempData["ErrorMsg"] = "Internal Error, user not found";
                        return RedirectToAction("ContinueRegistration");
                    }

                    var companies = CompanyService.FindAllCompanies().Where(x => x.Name == model.CompanyName); //TODO Security Problem - Don't use the Name from Model.
                                                                                                               //TODO Better Logic to see if the Company exists.
                    if (companies?.Any() == true)
                    {
                        TempData["ErrorMsg"] = $"'{model.CompanyName}' already exists, please choose another company name!";
                        return RedirectToAction("ContinueRegistration");
                    }

                    int? stateProvinceId = null;
                    if (model.CountryId == (int)COUNTRY_ID.US)
                    {
                        stateProvinceId = model.StateId;
                    }
                    else if (model.CountryId == (int)COUNTRY_ID.CA)
                    {
                        stateProvinceId = model.ProvinceId;
                    }

                    // Address
                    var address = new Omnae.Model.Models.Address()
                    {
                        AddressLine1 = model.AddressLine1,
                        AddressLine2 = model.AddressLine2,
                        City = model.City,
                        CountryId = model.CountryId,
                        StateProvinceId = stateProvinceId,
                        ZipCode = model.ZipCode,
                        PostalCode = model.PostalCode,
                        isBilling = model.IsBilling,
                        isShipping = model.IsShipping,
                    };

                    // Save address to DB
                    int addressId = AddressService.AddAddress(address);

                    // BillAddress
                    var billAddressId = model.IsBilling ? addressId : (int?)null;
                    if (billAddressId == null)
                    {
                        var billAddress = new Omnae.Model.Models.Address()
                        {
                            AddressLine1 = model.Bill_AddressLine1,
                            AddressLine2 = model.Bill_AddressLine2,
                            City = model.Bill_City,
                            CountryId = model.Bill_CountryId,
                            StateProvinceId = stateProvinceId,
                            ZipCode = model.Bill_ZipCode,
                            PostalCode = model.Bill_PostalCode,
                        };

                        // Save address to DB
                        billAddressId = AddressService.AddAddress(billAddress);
                    }

                    // BillAddress
                    var shippingAddressId = model.IsShipping ? addressId : (int?)null;
                    if (shippingAddressId == null)
                    {
                        var shipAddress = new Omnae.Model.Models.Address()
                        {
                            AddressLine1 = model.Shipping_AddressLine1,
                            AddressLine2 = model.Shipping_AddressLine2,
                            City = model.Shipping_City,
                            CountryId = model.Shipping_CountryId,
                            StateProvinceId = stateProvinceId,
                            ZipCode = model.Shipping_ZipCode,
                            PostalCode = model.Shipping_PostalCode,
                        };

                        // Save address to DB
                        shippingAddressId = AddressService.AddAddress(shipAddress);
                    }

                    // Company   
                    string logoName = $"Logo_{model.CompanyName}";
                    var company = new Company
                    {
                        Name = model.CompanyName,
                        AddressId = addressId,
                        BillAddressId = billAddressId,
                        CompanyType = model.UserType == USER_TYPE.Customer ? CompanyType.Customer
                                    : model.UserType == USER_TYPE.Vendor ? CompanyType.Vendor
                                    : CompanyType.None,
                        CompanyLogoUri = model.CompanyLogo != null ? ImageStorageService.Upload(model.CompanyLogo, logoName) : null,
                        isEnterprise = false,
                        Currency = CurrencyCodes.USD,
                    };

                    // Save Company to DB with shippingId being null, will insert later
                    var companyId = CompanyService.AddCompany(company);

                    //Save the User Company
                    userEntity.CompanyId = companyId;
                    await DbUser.SaveChangesAsync();


                    Session["CompanyName"] = company.Name;

                    // Shipping
                    var shipping = new Shipping()
                    {
                        Attention_FreeText = model.Attention,
                        CompanyId = companyId,
                        AddressId = shippingAddressId,
                        PhoneNumber = userEntity.PhoneNumber,
                        EmailAddress = userEntity.Email
                    };

                    // Save Shipping to DB
                    int shippingId = ShippingService.AddShipping(shipping);

                    // Insert shippingId to Companies table and Update the Addr table with CompanyID
                    CompanyService.UpdateCompanyShippingId(companyId, shippingId);
                    AddressService.UpdateAddressesForTheCompany(shippingAddressId, companyId, AddressType.Shipping);
                    AddressService.UpdateAddressesForTheCompany(billAddressId, companyId, AddressType.Billing);
                    AddressService.UpdateAddressesForTheCompany(addressId, companyId, AddressType.Mailing);


                    // Add new company account to QBO
                    var register = new RegisterViewModel()
                    {
                        Email = userEntity.Email,
                        PhoneNumber = userEntity.PhoneNumber,
                        FirstName = userEntity.FirstName,
                        MiddleName = userEntity.MiddleName,
                        LastName = userEntity.LastName,
                    };
                    var entity = new CreateAccountForCustomerViewModel
                    {
                        Resgister = register,
                        ContinueRegistration = model,
                    };


                    if (company.isEnterprise == false)
                    {
                        var qboInfo = CreateQboAccountInfo(entity);
                        var qboApi = new QboApi(QboTokensService);
                        company.QboId = await qboApi.CreateCustomer(qboInfo);
                    }

                    CompanyService.UpdateCompany(company);


                    ViewBag.CompanyId = companyId;

                    RemoveUserContextFromMemory();
                    trans.Complete();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = ex.RetrieveErrorMessage();
            }
            return RedirectToAction("ContinueRegistration");
        }

        private void RemoveUserContextFromMemory()
        {
            foreach (var registration in Container.Registrations.Where(p => p.RegisteredType == typeof(ILogedUserContext) || p.RegisteredType == typeof(LogedUserContext)))
            {
                registration.LifetimeManager.RemoveValue();
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TaskStateHandler(StateTransitionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Home");
            }
            List<HttpPostedFileBase> files = new List<HttpPostedFileBase>();
            if (Request.Files != null && Request.Files.Count > 0)
            {
                //foreach (HttpPostedFileBase file in Request.Files)
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    if (file.ContentLength > 0)
                    {
                        files.Add(file);
                    }
                }
            }
            var msg = await HomeBl.TaskStateHandler(model, files);
            if (msg != null)
            {
                if (msg.Equals(IndicatingMessages.SmsWarningMsg) || msg.Equals(IndicatingMessages.NoCellPhoneWarningMsg))
                {
                    TempData["Warning"] = msg;
                }
                else
                {
                    TempData["ErrorMessage"] = msg;
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult AssignTerm()
        {
            var companies = GetCustomers();
            ViewBag.Companies = new SelectList(companies, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignTerm(AssignTermViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var company = CompanyService.FindCompanyById(model.Id); //TODO: Check if this ID can be thusted or we need to use from the logged user.
            company.Term = model.Term == 0 ? null : model.Term;
            if (model.CreditLimit != null)
            {
                company.CreditLimit = model.CreditLimit.Value;
            }

            CompanyService.UpdateCompany(company);

            return Redirect("Index");
        }

        [HttpGet]
        public ActionResult AssignTermCreditLimit()
        {
            var customers = GetCustomers();
            var vendors = GetVendors();

            CompaniesCreditRelationshipViewModel model = new CompaniesCreditRelationshipViewModel
            {
                Customers = new SelectList(customers, "Id", "Name"),
                Vendors = new SelectList(vendors, "Id", "Name"),
                Currencies = new SelectList(GetCurrencyList(), "Value", "Text"),

            };
            return View(model);
        }

        private List<ListItem> GetCurrencyList()
        {
            Array values = Enum.GetValues(typeof(CurrencyCodes));
            List<ListItem> items = new List<ListItem>(values.Length);

            foreach (CurrencyCodes i in values)
            {
                string currencyName = WorldCurrency.Get(i).EnglishName ?? WorldCurrency.Get(CurrencyCodes.EUR).EnglishName;
                items.Add(new ListItem
                {
                    Text = Enum.GetName(typeof(CurrencyCodes), i) + $" - {currencyName}",
                    Value = ((int)i).ToString(),
                });
            }
            return items;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignTermCreditLimit(CompaniesCreditRelationshipViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = IndicatingMessages.MissingFormData;
                return RedirectToAction("AssignTermCreditLimit");
            }
            var error = HomeBl.AssignTermCreditLimit(model);
            if (error != null)
            {
                TempData["ErrorMessage"] = error;
            }
            else
            {
                TempData["Message"] = IndicatingMessages.Success;
            }
            return RedirectToAction("AssignTermCreditLimit");
        }

        [HttpGet]
        public ActionResult RemoveTermCreditLimit()
        {
            var customers = GetCustomers();
            var vendors = GetVendors();

            RemoveCreditRelationshipViewModel model = new RemoveCreditRelationshipViewModel
            {
                Customers = new SelectList(customers, "Id", "Name"),
                Vendors = new SelectList(vendors, "Id", "Name"),
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveTermCreditLimit(RemoveCreditRelationshipViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = IndicatingMessages.MissingFormData;
                return RedirectToAction("RemoveTermCreditLimit");
            }
            var error = HomeBl.RemoveTermCreditLimit(model);
            if (error != null)
            {
                TempData["ErrorMessage"] = error;
            }
            else
            {
                TempData["Message"] = IndicatingMessages.Success;
            }
            return RedirectToAction("RemoveTermCreditLimit");
        }

        [HttpGet]
        public ActionResult AssignTermToVendor()
        {
            var companies = GetVendors();
            ViewBag.Companies = new SelectList(companies, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignTermToVendor(AssignTermViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("AssignTermToVendor");
            }

            var company = CompanyService.FindCompanyById(model.Id);
            company.Term = model.Term;

            CompanyService.UpdateCompany(company);

            TempData["Result"] = $"'{company.Name}' granted {company.Term} days Term to Omnae successfully.";

            return RedirectToAction("AssignTermToVendor");
        }


        [HttpGet]
        public ActionResult AssignVendor()
        {
            var companies = CompanyService.FindAllCompanies()
                .Where(x => x.CompanyType == CompanyType.Customer)
                .GroupBy(x => x.Id).Select(y => y.FirstOrDefault()).OrderBy(n => n.Name).ToList();
            ViewBag.Companies = new SelectList(companies, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AssignVendor(int Id)
        {
            var company = CompanyService.FindCompanyById(Id); //TODO: Check if this ID can be thusted or we need to use from the logged user.
            company.CompanyType = CompanyType.Vendor;

            foreach (var othersUser in company.Users)
            {
                othersUser.UserType = USER_TYPE.Vendor;
            }

            CompanyService.UpdateCompany(company);//TODO: Test if this will change all the Users Logic.

            var users = await UserManager.FindAllByCompanyIdAsync(company.Id); //TODO: Check if this ID can be thusted or we need to use from the logged user.
            foreach (var user in users)
            {
                user.UserType = USER_TYPE.Vendor;
                var result = UserManager.Update(user);
                if (!result.Succeeded)
                {
                    ViewBag.ErrorMessage = "Assign user to vendor was failed. ";
                    if (result.Errors != null && result.Errors.Any())
                    {
                        ViewBag.ErrorMessage += result.Errors.ToArray()[0];
                    }
                    return View();
                }
            }
            TempData["Message"] = "Success";
            return RedirectToAction("AssignVendor");
        }

        public PartialViewResult CheckStates(TaskData td, StateMachine<States, Triggers> stTransition)
        {
            var stateTransition = new StateTransitionViewModel();

            if (td.StateId == (int)States.BackFromRFQ ||
                td.StateId == (int)States.OrderInitiated ||
                td.StateId == (int)States.ReOrderInitiated ||
                td.StateId == (int)States.ProofingComplete ||
                td.StateId == (int)States.SampleComplete)
            {
                stateTransition = new StateTransitionViewModel
                {
                    TaskData = td,
                    StTransition = stTransition,
                };
            }

            return PartialView("_StateTransition", stateTransition);
        }

        public ActionResult RedirectToFullView(int num)
        {
            //TODO: Check if this is Working

            List<TaskViewModel> model = new List<TaskViewModel>();

            switch (num)
            {
                case 1:
                    model = TempData["CustomerNewlyUpdates"] as List<TaskViewModel>;
                    return View("_CustomerLandingPartial1_Full", model);
                case 2:
                    model = TempData["CustomerOrderTrackings"] as List<TaskViewModel>;
                    return View("_CustomerLandingPartial2_Full", model);
                case 3:
                    model = TempData["CustomerFirstOrders"] as List<TaskViewModel>;
                    return View("_CustomerLandingPartial3_Full", model);
                case 4:
                    model = Session["ReOrders"] as List<TaskViewModel>;
                    if (model != null)
                    {
                        return View("_CustomerLandingPartial4_Full", model);
                    }
                    break;
                case 5:
                    model = Session["VendorProducts"] as List<TaskViewModel>;
                    return View("_VendorCompleteProducts_Full", model);
                case 6:
                    model = TempData["CustomerNewlyUpdates"] as List<TaskViewModel>;
                    return View("_Alerts", model);

            }
            TempData["ErrorMessage"] = "Something wrong";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult CreatePartsFromExcelDataFile()
        {
            var companies = CompanyService.FindAllCompanies().Where(x => x.CompanyType != null && x.CompanyType.Value == CompanyType.Customer).OrderBy(x => x.Name);
            ViewBag.Companies = new SelectList(companies, "Id", "Name");
            if (TempData["ExcelDataFile"] != null)
            {
                TempData.Keep();
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePartsFromExcelDataFile(int Id)
        {
            if (Request.Files == null || Request.Files.Count == 0 || Request.Files[0] == null)
            {
                TempData["ErrorMessage"] = "Please select a file to upload!";
                return RedirectToAction("CreatePartsFromExcelDataFile", "Home");
            }

            string inputFileName = Path.GetFileName(Request.Files[0].FileName);
            string inputFilePath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(@"~/Docs/"), inputFileName);
            Request.Files[0].SaveAs(inputFilePath);

            List<FileDataViewModel> inputList = new List<FileDataViewModel>();
            try
            {
                inputList = ExcelDataParser.GetFileStream(inputFilePath);

                foreach (var row in inputList)
                {
                    if (string.IsNullOrEmpty(row.PartNumber))
                    {
                        throw new Exception("PartNumber is required but it is null");
                    }
                    if (string.IsNullOrEmpty(row.PartNumberRevision))
                    {
                        throw new Exception("PartNumberRevision is required but it is null");
                    }
                    var prod = ProductService.FindProductByPartNumberOfACustomer(Id, row.PartNumber, row.PartNumberRevision);
                    if (prod != null) // This part number already exists in database, to avoid duplicating, stop creating this part now
                    {
                        continue;
                    }

                    // Save data to tables
                    // Save to Product table
                    Product product = new Product()
                    {
                        Name = row.Name,
                        Description = row.Description,
                        AdminId = row.AdminId,
                        CustomerId = Id,
                        VendorId = row.VendorId,
                        PartNumber = row.PartNumber,
                        PartNumberRevision = row.PartNumberRevision,
                        BuildType = (BUILD_TYPE)row.BuildType,
                        Material = (MATERIALS_TYPE)row.Material,
                        PrecisionMetal = (Precision_Metal?)row.PrecisionMetal,
                        MetalsProcesses = (Metals_Processes?)row.MetalsProcesses,
                        MetalType = (Metal_Type?)row.MetalType,
                        MetalsSurfaceFinish = (Metals_Surface_Finish?)row.MetalsSurfaceFinish,
                        PrecisionPlastics = (Precision_Plastics?)row.PrecisionPlastics,
                        PlasticsProcesses = (Plastics_Processes?)row.PlasticsProcesses,
                        MembraneSwitches = (Switches_Type?)row.MembraneSwitches,
                        MembraneSwitchesAttributes = (Print_Type?)row.MembraneSwitchesAttributes,
                        MembraneSwitchesAttributesWaterproof = (Membrane_Switches_Attributes_Waterproof?)row.MembraneSwitchesAttributesWaterproof,
                        MembraneSwitchesAttributesEmbossing = (Membrane_Switches_Attributes_Embossing?)row.MembraneSwitchesAttributesEmbossing,
                        MembraneSwitchesAttributesLEDLighting = (Membrane_Switches_Attributes_LEDLighting?)row.MembraneSwitchesAttributesLEDLighting,
                        MembraneSwitchesAttributesLED_EL_Backlighting = (Membrane_Switches_Attributes_LED_EL_Backlighting?)row.MembraneSwitchesAttributesLED_EL_Backlighting,
                        GraphicOverlaysAttributes = (Print_Type?)row.GraphicOverlaysAttributes,
                        GraphicOverlaysAttributesEmbossing = (Graphic_Overlays_Attributes_Embossing?)row.GraphicOverlaysAttributesEmbossing,
                        GraphicOverlaysAttributesSelectiveTexture = (Graphic_Overlays_Attributes_SelectiveTexture?)row.GraphicOverlaysAttributesSelectiveTexture,
                        Elastomers = row.Elastomers,
                        Labels = row.Labels,
                        MilledStone = row.MilledStone,
                        MilledWood = row.MilledWood,
                        FlexCircuits = row.FlexCircuits,
                        CableAssemblies = row.CableAssemblies,
                        Others = row.Others,
                        MetalType_FreeText = row.MetalType_FreeText,
                        SurfaceFinish_FreeText = row.SurfaceFinish_FreeText,
                        PlasticType_FreeText = row.PlasticType_FreeText,
                        ToolingLeadTime = row.ToolingLeadTime,
                        SampleLeadTime = row.SampleLeadTime,
                        ProductionLeadTime = row.ProductionLeadTime,
                        ToolingSetupCharges = row.ToolingSetupCharges,
                        HarmonizedCode = row.HarmonizedCode,
                        CreatedDate = DateTime.UtcNow,
                        CreatedByUserId = UserContext.UserId,
                    };

                    // avatar image
                    string fileName = Path.GetFileName(row.AvatarFilePath);
                    var path = row.AvatarFilePath;
                    string filePath = Path.Combine(Server.MapPath(@"~/Docs/"), path);

                    using (var hpf = new MemoryFile(filePath, "image/jpg"))
                    {
                        // Upload Avatar image to Azure Blob Storage and then return a Uri
                        var imageUri = ImageStorageService.Upload(hpf, fileName);

                        product.AvatarUri = imageUri;
                    }

                    var productId = ProductService.AddProduct(product);
                    product = ProductService.FindProductById(productId);

                    // Create a new TaskData from here
                    int daysToExpire = Convert.ToInt32(ConfigurationManager.AppSettings["DaysToExpire"]);
                    DateTime pastDays = DateTime.UtcNow.AddDays(-daysToExpire);
                    TaskData taskData = new TaskData()
                    {
                        StateId = row.StateId,
                        ProductId = productId,
                        CreatedUtc = pastDays,
                        ModifiedUtc = pastDays,
                        UpdatedBy = User.Identity.Name,
                        CreatedByUserId = UserContext.UserId,
                        ModifiedByUserId = UserContext.UserId,
                        IsRiskBuild = false,
                    };
                    var taskId = TaskDataService.AddTaskData(taskData);

                    // Create PartRevision table
                    var partRevsion = new PartRevision()
                    {
                        OriginProductId = productId,
                        StateId = (States)taskData.StateId,
                        TaskId = taskId,
                        Name = product.PartNumberRevision,
                        Description = "Initial Part Revision",
                        CreatedBy = User.Identity.Name,
                        CreatedUtc = DateTime.UtcNow,
                        CreatedByUserId = UserContext.UserId,
                        ModifiedByUserId = UserContext.UserId,
                    };
                    product.PartRevisionId = PartRevisionService.AddPartRevision(partRevsion);

                    // Save 2D, 3D and Proof documents into Documents table
                    // 2D pdf document

                    fileName = Path.GetFileName(row.Doc2D);
                    path = row.Doc2D;
                    filePath = Path.Combine(HostingEnvironment.MapPath(@"~/Docs/"), path);

                    int i = 1;

                    using (var hpf = new MemoryFile(filePath, "application/pdf"))
                    {
                        string ext = Path.GetExtension(fileName);
                        string fileNewName = $"prod_{productId}_{i++}{ext}";
                        var doc2DUri = DocumentStorageService.Upload(hpf, fileNewName);

                        var doc = new Document()
                        {
                            TaskId = taskId,
                            Name = fileNewName,
                            Version = 1,
                            DocUri = doc2DUri,
                            DocType = (int)DOCUMENT_TYPE.PRODUCT_2D_PDF,
                            IsLocked = false,
                            ProductId = productId,
                            CreatedUtc = pastDays,
                            ModifiedUtc = pastDays,
                        };

                        DocumentService.AddDocument(doc);
                    }

                    // 3D Step document

                    if (!string.IsNullOrWhiteSpace(row.Doc3D))
                    {
                        fileName = Path.GetFileName(row.Doc3D);
                        path = row.Doc3D;
                        filePath = Path.Combine(HostingEnvironment.MapPath(@"~/Docs/"), path);

                        using (var hpf = new MemoryFile(filePath, "application/octet-stream"))
                        {
                            var ext = Path.GetExtension(fileName);
                            var fileNewName = $"prod_{productId}_{i++}{ext}";
                            var doc3DUri = DocumentStorageService.Upload(hpf, fileNewName);

                            var doc = new Document()
                            {
                                TaskId = taskId,
                                Name = fileNewName,
                                Version = 1,
                                DocUri = doc3DUri,
                                DocType = (int)DOCUMENT_TYPE.PRODUCT_3D_STEP,
                                IsLocked = false,
                                ProductId = productId,
                                CreatedUtc = pastDays,
                                ModifiedUtc = pastDays,
                            };

                            DocumentService.AddDocument(doc);
                        }
                    }

                    // Proof document

                    if (!string.IsNullOrWhiteSpace(row.DocProof))
                    {
                        fileName = Path.GetFileName(row.DocProof);
                        path = row.DocProof;
                        filePath = Path.Combine(HostingEnvironment.MapPath(@"~/Docs/"), path);

                        using (var hpf = new MemoryFile(filePath, "application/pdf"))
                        {
                            var ext = Path.GetExtension(fileName);
                            var fileNewName = $"prod_{productId}_{i}{ext}";
                            var docProofUri = DocumentStorageService.Upload(hpf, fileNewName);
                            var doc = new Document()
                            {
                                TaskId = taskId,
                                Name = fileNewName,
                                Version = 1,
                                DocUri = docProofUri,
                                DocType = (int)DOCUMENT_TYPE.PROOF_PDF,
                                IsLocked = false,
                                ProductId = productId,
                                CreatedUtc = pastDays,
                                ModifiedUtc = pastDays,
                            };

                            DocumentService.AddDocument(doc);
                        }
                    }

                    // Save PriceBreaks to PriceBreaks table
                    RFQQuantity rfqQty = new RFQQuantity();
                    PriceBreak pb = new PriceBreak()
                    {
                        ProductId = productId,
                        TaskId = taskId,
                        Quantity = row.PriceBreak_Amount1,
                        UnitPrice = row.PriceBreak_UnitPrice1,
                        VendorUnitPrice = row.Vendor_PriceBreak_UnitPrice1
                    };
                    PriceBreakService.AddPriceBreak(pb);
                    rfqQty.Qty1 = row.PriceBreak_Amount1;

                    if (row.PriceBreak_Amount2 != null && row.Vendor_PriceBreak_UnitPrice2 != null)
                    {
                        PriceBreak pb2 = new PriceBreak()
                        {
                            ProductId = productId,
                            TaskId = taskId,
                            Quantity = (int)row.PriceBreak_Amount2,
                            UnitPrice = (decimal)row.PriceBreak_UnitPrice2,
                            VendorUnitPrice = (decimal)row.Vendor_PriceBreak_UnitPrice2
                        };
                        PriceBreakService.AddPriceBreak(pb2);
                        rfqQty.Qty2 = row.PriceBreak_Amount2.Value;
                    }

                    if (row.PriceBreak_Amount3 != null && row.Vendor_PriceBreak_UnitPrice3 != null)
                    {
                        PriceBreak pb3 = new PriceBreak()
                        {
                            ProductId = productId,
                            TaskId = taskId,
                            Quantity = (int)row.PriceBreak_Amount3,
                            UnitPrice = (decimal)row.PriceBreak_UnitPrice3,
                            VendorUnitPrice = (int)row.Vendor_PriceBreak_UnitPrice3
                        };
                        PriceBreakService.AddPriceBreak(pb3);
                        rfqQty.Qty3 = row.PriceBreak_Amount3.Value;
                    }

                    if (row.PriceBreak_Amount4 != null && row.Vendor_PriceBreak_UnitPrice4 != null)
                    {
                        PriceBreak pb4 = new PriceBreak()
                        {
                            ProductId = productId,
                            TaskId = taskId,
                            Quantity = (int)row.PriceBreak_Amount4,
                            UnitPrice = (decimal)row.PriceBreak_UnitPrice4,
                            VendorUnitPrice = (int)row.Vendor_PriceBreak_UnitPrice4
                        };
                        PriceBreakService.AddPriceBreak(pb4);
                        rfqQty.Qty4 = row.PriceBreak_Amount4.Value;
                    }

                    if (row.PriceBreak_Amount5 != null && row.Vendor_PriceBreak_UnitPrice5 != null)
                    {
                        PriceBreak pb5 = new PriceBreak()
                        {
                            ProductId = productId,
                            TaskId = taskId,
                            Quantity = (int)row.PriceBreak_Amount5,
                            UnitPrice = (decimal)row.PriceBreak_UnitPrice5,
                            VendorUnitPrice = (int)row.Vendor_PriceBreak_UnitPrice5
                        };
                        PriceBreakService.AddPriceBreak(pb5);
                        rfqQty.Qty5 = row.PriceBreak_Amount5.Value;
                    }

                    if (row.PriceBreak_Amount6 != null && row.Vendor_PriceBreak_UnitPrice6 != null)
                    {
                        PriceBreak pb6 = new PriceBreak()
                        {
                            ProductId = productId,
                            TaskId = taskId,
                            Quantity = (int)row.PriceBreak_Amount6,
                            UnitPrice = (decimal)row.PriceBreak_UnitPrice6,
                            VendorUnitPrice = (int)row.Vendor_PriceBreak_UnitPrice6
                        };
                        PriceBreakService.AddPriceBreak(pb6);
                        rfqQty.Qty6 = row.PriceBreak_Amount6.Value;
                    }

                    if (row.PriceBreak_Amount7 != null && row.Vendor_PriceBreak_UnitPrice7 != null)
                    {
                        PriceBreak pb7 = new PriceBreak()
                        {
                            ProductId = productId,
                            TaskId = taskId,
                            Quantity = (int)row.PriceBreak_Amount7,
                            UnitPrice = (int)row.PriceBreak_UnitPrice7,
                            VendorUnitPrice = (int)row.Vendor_PriceBreak_UnitPrice7
                        };
                        PriceBreakService.AddPriceBreak(pb7);
                        rfqQty.Qty7 = row.PriceBreak_Amount7.Value;
                    }

                    product.PriceBreakId = PriceBreakService.FindPriceBreakByProductId(productId).OrderBy(x => x.UnitPrice).FirstOrDefault().Id;

                    product.RFQQuantityId = RfqQuantityService.AddRFQQuantity(rfqQty);

                    ProductService.UpdateProduct(product);
                }
            }
            catch (Exception ex)
            {
                TempData["Result"] = ex.RetrieveErrorMessage();
                return RedirectToAction("CreatePartsFromExcelDataFile", "Home");
            }

            // uploaded input data Excel file to Azure Blobs Documents container
            using (var hpfb = new MemoryFile(inputFilePath, "application/application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
            {
                TempData["ExcelDataFile"] = DocumentStorageService.Upload(hpfb, inputFileName);
                TempData["Result"] = "File data input succeeded!";
            }

            return RedirectToAction("CreatePartsFromExcelDataFile", "Home");
        }

        [HttpGet]
        public ActionResult ChangePartState()
        {
            var products = ProductService.FindProductList().Where(x => x.CustomerCompany != null).GroupBy(x => x.PartNumber).Select(y => y.FirstOrDefault()).OrderBy(x => x.CustomerCompany.Name);
            var ddl = new List<SelectListItem>();
            foreach (var p in products)
            {
                var orders = OrderService.FindOrdersByProductId(p.Id);
                var item = new SelectListItem()
                {
                    Text = $"Pt#: {p.PartNumber} ( PO#: {(orders != null && orders.Any() ? orders.FirstOrDefault().CustomerPONumber : null)}, Pt: {p.Name}, Rev: {p.PartNumberRevision}, Customer: {CompanyService.FindCompanyById(p.CustomerId.Value).Name} )",
                    Value = p.Id.ToString()
                };
                ddl.Add(item);
            }

            ViewBag.ProductList = new SelectList(ddl, "Value", "Text");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePartState(ChangePartStateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var taskData = TaskDataService.FindTaskDataByProductId(model.ProductId);
            if (taskData.StateId >= (int)States.QuoteAccepted && taskData.StateId <= (int)States.ProductionComplete)
            {
                taskData.StateId = (int)model.StateId;
                TaskDataService.Update(taskData);
            }
            else
            {
                TempData["ErrorMessage"] = "You cannot set state prior to the state of Order Paid, or after production started";
                return RedirectToAction("ChangePartState");
            }

            return RedirectToAction("Index");
        }

        private List<SelectListItem> CreateAssignPartsToVendorProductList()
        {
            var prods = ProductService.FindProductList();
            var companies = CompanyService.FindAllCompanies();

            var ddl = (from c in companies
                       join p in prods
                       on c.Id equals p.CustomerId into companiesProds
                       from cp in companiesProds.DefaultIfEmpty()
                       where cp != null
                       orderby cp.PartNumber
                       group cp by new { cp.PartNumber, cp.Name, cp.PartNumberRevision } into g
                       select new SelectListItem
                       {
                           Text = g.FirstOrDefault().PartNumber + " (" + g.FirstOrDefault().Name + ", Rev. " + g.FirstOrDefault().PartNumberRevision + ")",
                           Value = g.FirstOrDefault().Id.ToString(),
                       }).ToList();

            return ddl;
        }

        private List<SelectListItem> CreateAssignPartsToVendorVendorList()
        {
            var companies = CompanyService.FindAllCompanies(CompanyType.Vendor);
            var result = from cv in companies
                         where cv != null
                         select new SelectListItem
                         {
                             Text = cv.Name,
                             Value = cv.Id.ToString()
                         };

            return result.ToList();
        }

        public ActionResult AssignPartsToVendor()
        {
            TempData["ProductList"] = new SelectList(CreateAssignPartsToVendorProductList(), "Value", "Text");
            TempData["VendorList"] = new SelectList(CreateAssignPartsToVendorVendorList(), "Value", "Text");
            TempData.Keep();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignPartsToVendor(AssignPartsToVendorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("AssignPartsToVendor");
            }

            var product = ProductService.FindProductById(model.ProductId);
            var rfqbid = RfqBidService.FindRFQBidListByProductId(model.ProductId).FirstOrDefault();
            var invoices = OmnaeInvoiceService.FindOmnaeInvoiceByProductId(model.ProductId).Where(x => x.UserType == (int)USER_TYPE.Vendor).ToList();

            if (product != null && invoices != null)
            {
                try
                {
                    product.VendorId = model.VendorId;
                    ProductService.UpdateProduct(product);
                    if (rfqbid != null)
                    {
                        rfqbid.VendorId = model.VendorId;
                        RfqBidService.UpdateRFQBid(rfqbid);
                    }
                    if (invoices.Any())
                    {
                        foreach (var inv in invoices)
                        {
                            inv.CompanyId = model.VendorId;
                            OmnaeInvoiceService.UpdateOmnaeInvoice(inv);
                        }
                    }

                    var vendor = CompanyService.FindCompanyById(model.VendorId);

                    TempData["Result"] = $"Assign {product.Name} to {vendor.Name} succeeded!";
                    TempData["ErrorMessage"] = null;
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex.RetrieveErrorMessage();
                }
            }
            else
            {
                TempData["ErrorMessage"] = IndicatingMessages.ProductNotFound;
            }

            return RedirectToAction("AssignPartsToVendor");
        }

        [HttpGet]
        public ActionResult GetPOByProductId()
        {
            var prods = ProductService.FindProductList().ToList();
            var docs = DocumentService.FindDocumentList().Where(x => x.DocType == (int)DOCUMENT_TYPE.PO_PDF);
            var products = (from d in docs
                            join p in prods
                            on d.ProductId equals p.Id into docs_prods
                            from dp in docs_prods.DefaultIfEmpty()
                            where dp != null
                            orderby dp.PartNumber
                            group dp by dp.PartNumber into g
                            select new SelectListItem
                            {
                                Text = $"Part# {g.Key}, Rev. {g.First().PartNumberRevision}",
                                Value = g.First().Id.ToString(),
                            }).OrderBy(x => x.Text).ToList();

            Session["ProductList"] = new SelectList(products, "Value", "Text");
            return View();
        }

        [HttpPost]
        public ActionResult GetPOByProductId(int productId)
        {
            var model = new GetPOByProductIdViewModel
            {
                DocList = DocumentService.FindDocumentByProductId(productId).Where(x => x.DocType == (int)DOCUMENT_TYPE.PO_PDF).ToList()
            };

            if (model.DocList != null)
            {
                return PartialView("_PODocList", model);
            }

            if (Session["ProductList"] == null)
            {
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpGet]
        public ActionResult GetQuoteDocByVendor()
        {
            var custs = GetCustomers();
            var prods = ProductService.FindProductList().Where(x => x.CustomerId != null);
            var docs = DocumentService.FindDocumentList().Where(x => x.DocType == (int)DOCUMENT_TYPE.QUOTE_PDF).ToList();

            var products = (from d in docs
                            join p in prods
                            on d.ProductId equals p.Id into prods_docs
                            from pd in prods_docs.DefaultIfEmpty()
                            where pd?.CustomerId != null
                            select new
                            {
                                CustomerId = pd.CustomerId,
                                ProdId = pd.Id,

                            });

            var customers = (from p in products
                             join c in custs
                             on p.CustomerId equals c.Id into prods_customers
                             from pc in prods_customers.DefaultIfEmpty()
                             where pc != null
                             select new SelectListItem
                             {
                                 Text = pc.Name,
                                 Value = pc.Id.ToString(),
                             }).GroupBy(x => x.Value).Select(x => x.First());

            var model = new GetQuoteDocByVendorViewModel
            {
                Customers = new SelectList(customers, "Value", "Text"),
            };

            return View(model);
        }

        public ActionResult GetCustomerProducts(int custId)
        {
            TempData["Result"] = null;
            TempData["ErrorMessage"] = null;

            var products = ProductService.FindProductListByCustomerId(custId);
            var docs = DocumentService.FindDocumentList().Where(d => d.DocType == (int)DOCUMENT_TYPE.QUOTE_PDF).ToList();
            if (products != null && products.Any())
            {
                var productList = (from dc in docs
                                   join pd in products
                                   on dc.ProductId equals pd.Id into pd_dc
                                   from dp in pd_dc.DefaultIfEmpty()
                                   where dp != null
                                   orderby dp.PartNumber
                                   group dp by (dp.PartNumber, dp.PartNumberRevision) into g
                                   select new SelectListItem
                                   {
                                       Text = $"Part #: {g.First().PartNumber}, Rev: {g.First().PartNumberRevision}",
                                       Value = g.First().Id.ToString(),
                                   }).ToList();

                var model = new GetQuoteDocByVendorViewModel
                {
                    ProductList = new SelectList(productList, "Value", "Text"),
                };
                return PartialView("_CustomerProductDropdown", model);
            }

            TempData["ErrorMessage"] = "This customer has no parts yet!";
            return PartialView("_CustomerProductDropdown", null);
        }

        public ActionResult GetVendorProducts(int vendorId)
        {
            TempData["Result"] = null;
            TempData["ErrorMessage"] = null;

            var products = ProductService.FindProductListByVendorId(vendorId);
            var docs = DocumentService.FindDocumentList().Where(d => d.DocType == (int)DOCUMENT_TYPE.QUOTE_PDF).ToList();
            if (products != null && products.Any())
            {
                var productList = (from dc in docs
                                   join pd in products
                                   on dc.ProductId equals pd.Id into pd_dc
                                   from dp in pd_dc.DefaultIfEmpty()
                                   where dp != null
                                   orderby dp.PartNumber
                                   group dp by (dp.PartNumber, dp.PartNumberRevision) into g
                                   select new SelectListItem
                                   {
                                       Text = $"Part #: {g.First().PartNumber}, Rev: {g.First().PartNumberRevision}",
                                       Value = g.First().Id.ToString(),
                                   }).ToList();

                var model = new GetQuoteDocByVendorViewModel
                {
                    VendorId = vendorId,
                    ProductList = new SelectList(productList, "Value", "Text"),
                };
                return PartialView("_VendorProductDropdown", model);
            }

            TempData["ErrorMessage"] = "This vendor has no parts yet!";
            return PartialView("_VendorProductDropdown", null);
        }

        [HttpPost]
        public ActionResult GetQuoteDocs(int? productId)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_GetQuoteDocByVendor", null);
            }

            if (productId == null)
            {
                return PartialView("_GetQuoteDocByVendor", null);
            }

            var docs = DocumentService.FindDocumentByProductId(productId.Value).Where(x => x.DocType == (int)DOCUMENT_TYPE.QUOTE_PDF).ToList();
            if (docs.Count == 0)
            {
                TempData["ErrorMessage"] = "Couldn't find Quote documents for this vendor and part";
                return PartialView("_VendorProductDropdown", null);
            }

            List<Document> quotes = null;
            if (docs.First().UserType != null)
            {
                quotes = docs.Where(d => d.UserType == (int)USER_TYPE.Vendor).ToList();
            }
            else
            {
                var product = ProductService.FindProductById(productId.Value);
                var vendorId = product.VendorId;
                quotes = docs.Where(d => d.Name.Contains(vendorId.ToString())).ToList();
            }
            var model = new GetQuoteDocByVendorViewModel
            {
                QuoteDocs = docs,
            };
            if (quotes.Count > 0)
            {
                model = new GetQuoteDocByVendorViewModel
                {
                    QuoteDocs = quotes,
                };
            }

            return PartialView("_GetQuoteDocByVendor", model);
        }

        [HttpGet]
        public ActionResult CreateAccountForCustomer()
        {
            int usaCode = (int)STATE_PROVINCE_CODE.US_STATE_CODE;
            int canadaCode = (int)STATE_PROVINCE_CODE.CA_PROVINCE_CODE;
            int selectedProvince = 59; // 59: BC

            ViewBag.CountryList = new SelectList(CountryService.FindAllCountries(), "Id", "CountryName", (int)COUNTRY_ID.CA);
            ViewBag.States = new SelectList(StateProvinceService.FindStateProvinceByCode(usaCode), "Id", "Name");
            ViewBag.Provinces = new SelectList(StateProvinceService.FindStateProvinceByCode(canadaCode), "Id", "Name", selectedProvince);

            var model = new CreateAccountForCustomerViewModel
            {
                Resgister = new RegisterViewModel(),
                ContinueRegistration = new ContinueRegistrationViewModel(),

            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAccountForCustomer(CreateAccountForCustomerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Entered data is invalid, please try again.";
                return RedirectToAction("CreateAccountForCustomer", "Home");
            }

            using (var trans = AsyncTransactionScope.StartNew())
            {

                // Address
                int? stateProvinceId = null;
                if (model.ContinueRegistration.CountryId == (int)COUNTRY_ID.US)
                {
                    stateProvinceId = model.ContinueRegistration.StateId;
                }
                else if (model.ContinueRegistration.CountryId == (int)COUNTRY_ID.CA)
                {
                    stateProvinceId = model.ContinueRegistration.ProvinceId;
                }
                var address = new Omnae.Model.Models.Address()
                {
                    AddressLine1 = model.ContinueRegistration.AddressLine1,
                    AddressLine2 = model.ContinueRegistration.AddressLine2,
                    City = model.ContinueRegistration.City,
                    CountryId = model.ContinueRegistration.CountryId,
                    StateProvinceId = stateProvinceId,
                    ZipCode = model.ContinueRegistration.ZipCode,
                    PostalCode = model.ContinueRegistration.PostalCode ?? model.ContinueRegistration.AreaCode,
                    isBilling = model.ContinueRegistration.IsBilling,
                    isShipping = model.ContinueRegistration.IsShipping,
                };

                // Save address to DB
                int addressId = AddressService.AddAddress(address);

                // Company
                var company = new Model.Models.Company()
                {
                    Name = model.ContinueRegistration.CompanyName,
                    AddressId = addressId,
                    CompanyType = model.ContinueRegistration.UserType == USER_TYPE.Customer ? CompanyType.Customer
                                : model.ContinueRegistration.UserType == USER_TYPE.Vendor ? CompanyType.Vendor
                                : CompanyType.None,
                    isEnterprise = model.ContinueRegistration.isEnterprise,
                };
                // Save Company to DB with shippingId being null, will insert later
                var companyId = CompanyService.AddCompany(company);

                //Create User
                // Store user basic info
                var user = new ApplicationUser
                {
                    UserName = model.Resgister.Email,
                    Email = model.Resgister.Email,
                    UserType = model.ContinueRegistration.UserType,
                    PhoneNumber = model.Resgister.PhoneNumber,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    FirstName = model.Resgister.FirstName,
                    MiddleName = model.Resgister.MiddleName,
                    LastName = model.Resgister.LastName,
                    Active = true,
                    CompanyId = companyId,
                };
                var result = await UserManager.CreateAsync(user, model.Resgister.Password);
                if (!result.Succeeded)
                {
                    TempData["ErrorMessage"] = result.Errors.FirstOrDefault();
                    return RedirectToAction("CreateAccountForCustomer", "Home");
                }

                //Add default Role
                await UserManager.AddToRoleAsync(user.Id, Omnae.Model.Security.Roles.CompanyAdmin);

                // Shipping
                Shipping shipping = ShippingService.FindShippingByUserId(companyId);
                if (shipping == null)
                {
                    shipping = new Shipping()
                    {
                        Attention_FreeText = model.ContinueRegistration.Attention,
                        CompanyId = company.Id,
                        AddressId = addressId,
                        PhoneNumber = user.PhoneNumber,
                        EmailAddress = user.Email
                    };

                    try
                    {
                        // Save Shipping to DB
                        int shippingId = ShippingService.AddShipping(shipping);

                        // Insert shippingId to Companies table
                        CompanyService.UpdateCompanyShippingId(companyId, shippingId);
                    }
                    catch (DbEntityValidationException ex)
                    {
                        TempData["ErrorMessage"] = RetrieveDbEntityValidationException(ex);
                        return RedirectToAction("CreateAccountForCustomer");
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = ex.RetrieveErrorMessage();
                        return RedirectToAction("CreateAccountForCustomer");
                    }
                }

                Session["UserType"] = model.ContinueRegistration.UserType;

                // Add this account into QBO
                CustomerInfo qboInfo = CreateQboAccountInfo(model);
                QboApi qboApi = new QboApi(QboTokensService);
                string qboId = string.Empty;

                if (model.ContinueRegistration.UserType == USER_TYPE.Customer)
                {
                    try
                    {
                        qboId = await qboApi.CreateCustomer(qboInfo);
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = ex.RetrieveErrorMessage();
                        return RedirectToAction("CreateAccountForCustomer");
                    }
                }
                else if (model.ContinueRegistration.UserType == USER_TYPE.Vendor)
                {
                    try
                    {
                        qboId = await qboApi.CreateVendor(qboInfo);
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = ex.RetrieveErrorMessage();
                        return RedirectToAction("CreateAccountForCustomer");
                    }
                }
                company.QboId = qboId;
                CompanyService.UpdateCompany(company);

                trans.Complete();
                return RedirectToAction("Index", "Home");
            }
        }

        private CustomerInfo CreateQboAccountInfo(CreateAccountForCustomerViewModel model)
        {
            var accountInfo = new CustomerInfo();

            accountInfo.Taxable = true;
            accountInfo.CompanyName = model.ContinueRegistration.CompanyName;
            accountInfo.GivenName = model.Resgister.FirstName;
            accountInfo.MiddleName = model.Resgister.MiddleName;
            accountInfo.FamilyName = model.Resgister.LastName;
            accountInfo.DisplayName = model.ContinueRegistration.CompanyName;

            var phone = new TelephoneNumber
            {
                FreeFormNumber = model.Resgister.PhoneNumber,
            };

            var email = new EmailAddress
            {
                Address = model.Resgister.Email,
                Default = true,
                DefaultSpecified = true,
            };

            accountInfo.primaryPhone = phone;
            accountInfo.alternatePhone = phone;
            accountInfo.mobile = phone;
            accountInfo.primaryEmailAddr = email;

            int? stateProvinceId = null;
            if (model.ContinueRegistration.CountryId == (int)COUNTRY_ID.US)
            {
                stateProvinceId = model.ContinueRegistration.StateId;
            }
            else if (model.ContinueRegistration.CountryId == (int)COUNTRY_ID.CA)
            {
                stateProvinceId = model.ContinueRegistration.ProvinceId;
            }

            StateProvince state = null;
            if (stateProvinceId != null)
            {
                state = StateProvinceService.FindStateProvinceById(stateProvinceId.Value);
            }
            Country country = CountryService.FindCountryById(model.ContinueRegistration.CountryId);
            var addr = new Intuit.Ipp.Data.PhysicalAddress
            {
                Line1 = model.ContinueRegistration.AddressLine1,
                Line2 = model.ContinueRegistration.AddressLine2,
                City = model.ContinueRegistration.City,
                Country = country.CountryName,
                CountryCode = model.ContinueRegistration.CountryId.ToString(),
                PostalCode = model.ContinueRegistration.PostalCode ?? model.ContinueRegistration.ZipCode,
                CountrySubDivisionCode = state?.Abbreviation,
            };
            accountInfo.BillAddr = addr;

            if (model.ContinueRegistration.IsBilling)
            {
                accountInfo.BillAddr = addr;
            }
            if (model.ContinueRegistration.IsShipping)
            {
                accountInfo.ShipAddr = addr;
            }

            return accountInfo;
        }

        [HttpGet]
        public ActionResult AddExistingCustomersToQBO()
        {
            var customers = DbUser.Users.Where(u => u.UserType == USER_TYPE.Customer);
            AddExistingUsersToQBOViewModel model = new AddExistingUsersToQBOViewModel
            {
                DdlCustomers = new MultiSelectList(GetUserIdList(customers).OrderBy(x => x.UserName).ToList(), "CompanyId", "UserName"),
            };

            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddExistingCustomersToQBO(AddExistingUsersToQBOViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index");
            }
            using (var trans = AsyncTransactionScope.StartNew())
            {
                foreach (var id in model.UserIds)
                {
                    QboApi qboApi = new QboApi(QboTokensService);
                    try
                    {
                        // create Customer on QBO
                        CustomerInfo customerInfo = HomeBl.CreateUsersForQBO(id);
                        string qboId = await qboApi.CreateCustomer(customerInfo);

                        // update Company table by insert this qbo id
                        var company = CompanyService.FindCompanyById(id);
                        company.QboId = qboId;
                        CompanyService.UpdateCompany(company);
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = ex.RetrieveErrorMessage();
                        TempData["Result"] = null;
                        var vends = DbUser.Users.Where(u => u.UserType == USER_TYPE.Vendor);
                        model.DdlCustomers = new MultiSelectList(GetUserIdList(vends).OrderBy(x => x.UserName).ToList(), "CompanyId", "UserName");
                        return View(model);
                    }
                }
                var customers = DbUser.Users.Where(u => u.UserType == USER_TYPE.Customer);
                model.DdlCustomers = new MultiSelectList(GetUserIdList(customers).OrderBy(x => x.UserName).ToList(), "CompanyId", "UserName");

                TempData["Result"] = "Add Customer(s) to QuickBooks Online succeeded!";
                TempData["ErrorMessage"] = null;

                trans.Complete();
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult AddExistingVendorsToQBO()
        {
            var vendors = DbUser.Users.Where(u => u.UserType == USER_TYPE.Vendor);
            AddExistingUsersToQBOViewModel model = new AddExistingUsersToQBOViewModel
            {
                DdlCustomers = new MultiSelectList(GetUserIdList(vendors).OrderBy(x => x.UserName).ToList(), "CompanyId", "UserName"),
            };
            TempData["Result"] = null;
            TempData["ErrorMessage"] = null;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddExistingVendorsToQBO(AddExistingUsersToQBOViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index");
            }

            using (var trans = AsyncTransactionScope.StartNew())
            {
                foreach (var id in model.UserIds)
                {
                    QboApi qboApi = new QboApi(QboTokensService);
                    try
                    {
                        var company = CompanyService.FindCompanyById(id);

                        CustomerInfo vendorInfo = HomeBl.CreateUsersForQBO(id);
                        string qboId = await qboApi.CreateVendor(vendorInfo);

                        // update Company table by insert this qbo id

                        company.QboId = qboId;
                        CompanyService.UpdateCompany(company);
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = ex.RetrieveErrorMessage();
                        TempData["Result"] = null;
                        var vends = DbUser.Users.Where(u => u.UserType == USER_TYPE.Vendor);
                        model.DdlCustomers = new MultiSelectList(GetUserIdList(vends).OrderBy(x => x.UserName).ToList(), "CompanyId", "UserName");
                        return View(model);
                    }
                }
                var vendors = DbUser.Users.Where(u => u.UserType == USER_TYPE.Vendor);
                model.DdlCustomers = new MultiSelectList(GetUserIdList(vendors).OrderBy(x => x.UserName).ToList(), "CompanyId", "UserName");

                TempData["Result"] = "Add Vendor(s) to QuickBooks Online succeeded!";
                TempData["ErrorMessage"] = null;

                trans.Complete();
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult CompanyNCR(ChartType chartType = ChartType.BarChart, NCR_FILTERS? filter = null, int? val = null)
        {
            if (UserContext.Company == null)
            {
                TempData["ErrorMessage"] = "Company could not be found";
                return RedirectToAction("Index", "Home");
            }
            var model = ChartBl.NCRCharts(chartType, filter, val);
            Session["NCR_FILTER"] = filter;
            Session["NCR_FILTER_VAL"] = val;

            var products = model.Products.Where(x => x.CustomerCompany.isEnterprise == false);
            var orders = model.Orders;

            if (UserContext.UserType == USER_TYPE.Customer)
            {
                ViewBag.AllProducts = (filter == NCR_FILTERS.Vendor ? products.Where(x => x.VendorId == val).ToList() : products);
            }
            else if (UserContext.UserType == USER_TYPE.Vendor)
            {
                ViewBag.AllProducts = (filter == NCR_FILTERS.Customer ? products.Where(x => x.CustomerId == val).ToList() : products);
            }
            else if (UserContext.UserType == USER_TYPE.Admin)
            {
                ViewBag.AllProducts = products;
            }


            ViewBag.Years = orders?.Select(x => x.OrderDate.Year).OrderBy(x => x).Distinct().ToList();
            ViewBag.Customers = products?
                .Select(x => x.CustomerCompany)
                .GroupBy(x => x.Id)
                .Select(y => y.First())
                .OrderBy(x => x.Name).ToList();

            ViewBag.Vendors = products?
                .Where(x => x.VendorCompany != null)?
                .Select(x => x.VendorCompany)
                .GroupBy(x => x.Id)
                .Select(y => y.First())
                .OrderBy(x => x.Name).ToList();

            if (model.ChartType.DicFilter != null && model.ChartType.DicFilter.Count() > 0)
            {
                return PartialView("_CompanyNCR", model);
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult SwitchCharts(ChartType? id)
        {
            NCR_FILTERS? filter = (NCR_FILTERS?)Session["NCR_FILTER"];
            int? val = (int?)Session["NCR_FILTER_VAL"];
            NCRViewModel model;
            if (id != null)
            {
                model = ChartBl.NCRCharts(id.Value, filter, val);
                return PartialView("_Charts", model.ChartType);
            }
            model = ChartBl.NCRCharts(ChartType.BarChart, filter, val);

            var products = model.Products;
            var orders = model.Orders;

            if (UserContext.UserType == USER_TYPE.Customer)
            {
                ViewBag.AllProducts = (filter == NCR_FILTERS.Vendor ? products.Where(x => x.VendorId == val).ToList() : products);
            }
            else if (UserContext.UserType == USER_TYPE.Vendor)
            {
                ViewBag.AllProducts = (filter == NCR_FILTERS.Customer ? products.Where(x => x.CustomerId == val).ToList() : products);
            }
            else if (UserContext.UserType == USER_TYPE.Admin)
            {
                ViewBag.AllProducts = products;
            }


            ViewBag.Years = orders.Select(x => x.OrderDate.Year).Distinct().ToList();
            ViewBag.Customers = products
                .Select(x => x.CustomerCompany)
                .GroupBy(x => x.Id)
                .Select(y => y.First())
                .OrderBy(x => x.Name).ToList();

            ViewBag.Vendors = products
                .Where(x => x.VendorCompany != null)
                .Select(x => x.VendorCompany)
                .GroupBy(x => x.Id)
                .Select(y => y.First())
                .OrderBy(x => x.Name).ToList();

            return PartialView("_Charts", model.ChartType);
        }


        public ActionResult NcrDetails(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = IndicatingMessages.ParameterIsNull;
                return RedirectToAction("Index");
            }

            NcrDescriptionViewModel model = new NcrDescriptionViewModel();
            string errors = HomeBl.NcrDetails(id.Value, ref model);
            if (errors != null)
            {
                TempData["ErrorMessage"] = errors;
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult AdminOrderDetails()
        {
            var tasks = TaskDataService.GetTaskDataAll().Where(x => x.StateId >= (int)States.OrderPaid &&
                                                               x.StateId <= (int)States.ProductionComplete ||
                                                               x.StateId == (int)States.VendorPendingInvoice).ToList();

            var orders = OrderService.FindOrderList();

            var totalOrders = (from task in tasks
                               join order in orders
                               on task.TaskId equals order.TaskId into tasks_orders
                               from to in tasks_orders.DefaultIfEmpty()
                               where to != null && to.CustomerPONumber != null
                               orderby to.CustomerPONumber
                               select new SelectListItem
                               {
                                   Text = to.CustomerPONumber,
                                   Value = to.Id.ToString(),
                               }).ToList();

            SelectListItem item = new SelectListItem() { Text = "Select Follwoing PO Number for Displaying Order Details", Value = "0" };
            totalOrders.Insert(0, item);
            AdminOrderDetailsViewModel model = new AdminOrderDetailsViewModel();
            model.DdlOrders = new SelectList(totalOrders, "Value", "Text");
            return View(model);
        }

        [HttpPost]
        public ActionResult AdminOrderDetails(int OrderId)
        {
            if (OrderId > 0)
            {
                var order = OrderService.FindOrderById(OrderId);
                var taskData = order.TaskData;
                var product = order.Product;
                List<ProductStateTracking> productStates = ProductStateTrackingService.FindProductStateTrackingListByProductId(product.Id).OrderBy(x => x.ModifiedUtc).ToList();

                var orderStates = OrderStateTrackingService.FindOrderStateTrackingListByOrderId(order.Id).OrderBy(x => x.ModifiedUtc).ToList();

                StateLastUpdatedViewModel stateLastUpdated = new StateLastUpdatedViewModel
                {
                    StateId = (States)taskData.StateId,
                    LastUpdated = taskData.ModifiedUtc,
                };

                var shippingResp = ShipmentBL.ShipmentTracking(order.TrackingNumber);

                StateTrackingViewModel model = new StateTrackingViewModel()
                {
                    OrderId = order.Id,
                    TaskId = taskData.TaskId,
                    StateId = (States)taskData.StateId,
                    OrderStateTrackings = orderStates,
                    ProductStateTrackings = productStates,
                    Order = order,
                    Product = product,
                    IsTagged = taskData.isTagged,
                    LastUpdated = stateLastUpdated,
                    DHLResponse = shippingResp,
                    isEnterprise = taskData.isEnterprise,
                    NumberSampleIncluded = GetNumberSampleIncluded(taskData.TaskId, product.Id),
                };

                return PartialView("_OrderDetails", model);
            }
            else if (OrderId == 0)
            {
                return RedirectToAction("AdminOrderDetails");
            }
            else
            {
                TempData["ErrorMessage"] = "Inputs are not valid.";
                return View();
            }
        }


        // AdminProductDetails

        [HttpGet]
        public ActionResult GetAdminProducts()
        {
            var tasks = TaskDataService.GetTaskDataAll().Where(x => (x.StateId >= (int)States.QuoteAccepted &&
                                                                     x.StateId <= (int)States.NCRClosed) ||
                                                                    (x.StateId >= (int)States.AddExtraQuantities &&
                                                                     x.StateId <= (int)States.NCRDamagedByCustomer));

            var products = ProductService.FindProductList().ToList();
#if true
            var totalProducts = (from product in products
                                 join task in tasks
                                 on product.Id equals task.ProductId into tasks_products
                                 from tp in tasks_products.DefaultIfEmpty()
                                 where tp != null && tp.Product != null
                                 orderby tp.Product.CustomerCompany.Name
                                 group tp by new { tp.Product.PartNumber, tp.Product.PartNumberRevision } into g
                                 select new SelectListItem
                                 {
                                     Text = $"Csutomer: {g.First().Product.CustomerCompany.Name}, Part #: {g.Key.PartNumber}, Rev.: {g.Key.PartNumberRevision}",
                                     Value = g.First().TaskId.ToString(),
                                 }).ToList();

#else
            var totalProducts = (from task in tasks
                                 join product in products
                                 on task.ProductId equals product.Id into tasks_products
                                 from tp in tasks_products.DefaultIfEmpty()
                                 where tp != null && tp.PartNumber != null
                                 orderby tp.CustomerCompany.Name
                                 group tp by new { task.TaskId,  tp.PartNumber, tp.PartNumberRevision } into g
                                 select new SelectListItem
                                 {
                                     Text = $"Csutomer: {g.First().CustomerCompany?.Name}, Part #: {g.Key.PartNumber}, Rev.: {g.Key.PartNumberRevision}",
                                     Value = g.Key.TaskId.ToString(),
                                 }).ToList();
#endif

            SelectListItem item = new SelectListItem() { Text = "Select Follwoing Part # for Displaying Product Details", Value = "0" };
            totalProducts.Insert(0, item);
            var model = new AdminProductDetailsViewModel() { DdlProducts = new SelectList(totalProducts, "Value", "Text") };
            return View(model);
        }



        [HttpGet]
        public PartialViewResult AdminProductDetails(int taskId)
        {
            //Product product = ProductService.FindProductById(taskId);
            //if (product == null)
            //{
            //    HandleErrorInfo info = new HandleErrorInfo(new Exception(IndicatingMessages.ProductNotFound), "Products", "Details");
            //    return View("Error", info);
            //}

            TaskDataModalViewModel tvm = GetTaskViewModel(taskId);
            return PartialView("_AdminProductDetails", tvm);

        }



        [HttpPost]
        public ActionResult ModifyNCCauseReasons(string pk, string name, string value)
        {
            int id = pk != null ? int.Parse(pk) : 0;

            if (name == "RevisingSuggestion")
            {
                var td = TaskDataService.FindById(id);
                td.RevisingReason = value;
                td.UpdatedBy = User.Identity.Name;
                td.ModifiedUtc = DateTime.UtcNow;
                td.ModifiedByUserId = UserContext.UserId;

                TaskDataService.Update(td);
            }
            else
            {
                var entity = NcReportService.FindNCReportById(id);
                if (entity == null)
                {
                    HandleErrorInfo info = new HandleErrorInfo(new Exception("NCReport is not found"), "Home", "ModifyNCCauseReasons");
                    return View("Error", info);
                }
                if (name == "ArbitrateCustomerCauseReason")
                {
                    entity.ArbitrateCustomerCauseReason = value;
                }
                else if (name == "ArbitrateVendorCauseReason")
                {
                    entity.ArbitrateVendorCauseReason = value;
                }
                else if (name == "RejectCorrectivePartsReason")
                {
                    entity.RejectCorrectivePartsReason = value;
                }
                else if (name == "RejectRootCauseReason")
                {
                    entity.RejectRootCauseReason = value;
                }
                else if (name == "RejectCorrectiveActionReason")
                {
                    entity.RejectCorrectiveActionReason = value;
                }
                else if (name == "RootCauseOnCustomerReason")
                {
                    entity.RootCauseOnCustomerReason = value;
                }

                NcReportService.UpdateNCReport(entity);
            }

            return Json(new { newValue = value });
        }

        [HttpPost]
        public ActionResult AddNotes(string pk, string name, string value)
        {
            int id = pk != null ? int.Parse(pk) : 0;

            if (name == "notes")
            {
                var order = OrderService.FindOrderById(id);
                order.Notes = value;
                order.ModifiedByUserId = UserContext.UserId;
                OrderService.UpdateOrder(order);
            }
            return Json(new { newValue = value });
        }

        // One time function for updating existing tables
        [HttpGet]
        public ActionResult UpdateExistingDatabase()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateExistingDatabase(int id = 1)
        {
            try
            {
                // Get all products
                var products = ProductService.FindProductList().Where(p => p.PartRevisionId == null).OrderBy(x => x.Id);
                foreach (var product in products)
                {
                    var taskData = TaskDataService.FindTaskDataByProductId(product.Id);
                    // create a new PartRevision record
                    PartRevision pr = new PartRevision()
                    {
                        Name = product.PartNumberRevision,
                        OriginProductId = product.Id,
                        TaskId = taskData.TaskId,
                        StateId = (States)taskData.StateId,
                        CreatedBy = "dlionello@padtech.com",
                        CreatedUtc = DateTime.UtcNow,
                        Description = "Initial Part Revision",
                    };
                    product.PartRevisionId = PartRevisionService.AddPartRevision(pr);

                    ProductService.UpdateProduct(product);
                }

                TempData["Result"] = "Database updating succeeded!";
            }
            catch (Exception ex)
            {
                TempData["Result"] = ex.Message;
            }

            return View();
        }

        private List<GetInspectionReportsViewModel> GetVendorInspectionReports(int companyId)
        {
            var prods = ProductService.FindProductList().Where(x => x.VendorId != null && x.VendorId == companyId).ToList();

            var docs = DocumentService.FindDocuments().Where(d => d.DocType == (int)DOCUMENT_TYPE.PACKING_SLIP_INSPECTION_REPORT_PDF).ToList();
            DocumentService.UpdateDocUrlWithSecurityToken(docs);

            var model = (from p in prods
                         join d in docs
                         on p.Id equals d.ProductId into doc_prod
                         from dp in doc_prod.DefaultIfEmpty()
                         where dp != null
                         orderby dp.CreatedUtc

                         select new GetInspectionReportsViewModel
                         {
                             ProductName = p.Name,
                             ProductDescription = p.Description,
                             PartNumber = p.PartNumber,
                             PartNumberRev = p.PartNumberRevision,
                             Doc = new Document
                             {
                                 Id = dp.Id,
                                 Name = dp.Name,
                                 DocType = dp.DocType,
                                 CreatedUtc = dp.CreatedUtc,
                                 ModifiedUtc = dp.ModifiedUtc,
                                 ProductId = dp.ProductId,
                                 UpdatedBy = dp.UpdatedBy,
                                 Version = dp.Version,
                                 DocUri = dp.DocUri,
                                 IsLocked = dp.IsLocked,
                             },
                         }).ToList();

            return model;
        }

        [HttpGet]
        public ActionResult RetrieveVendorInspectionReports()
        {
            var vendors = CreateVendorList();
            var prods = ProductService.FindProductList();

            var vend = (from p in prods.ToList()
                        join v in vendors
                        on p.VendorId equals v.Id into prods_vendors
                        from pv in prods_vendors.DefaultIfEmpty()
                        where pv != null
                        group pv by new { ProdsVednorId = pv.Id, ProductId = p.Id } into g
                        select new
                        {
                            CompanyName = g.First().Name,
                            CompanyId = g.Key.ProdsVednorId.ToString(),
                            ProdId = g.Key.ProductId,
                        });

            var docs = DocumentService.FindDocumentList().Where(d => d.DocType == (int)DOCUMENT_TYPE.PACKING_SLIP_INSPECTION_REPORT_PDF);

            var vends = (from d in docs
                         join v in vend.ToList()
                         on d.ProductId equals v.ProdId into doc_vendor
                         from dv in doc_vendor.DefaultIfEmpty()
                         where dv != null
                         select new SelectListItem
                         {
                             Text = dv.CompanyName,
                             Value = dv.CompanyId,
                         }).GroupBy(g => g.Value).Select(x => x.FirstOrDefault());


            InspectionReportViewModel model = new InspectionReportViewModel
            {
                Vendors = new SelectList(vends, "Value", "Text"),
            };

            return View(model);
        }

        public ActionResult GetInspectionReports(int vendorId)
        {
            var model = GetVendorInspectionReports(vendorId);
            return PartialView("_GetInspectionReports", model);
        }

        public ActionResult Help()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Contact()
        {
            var location = new LocationViewModel
            {
                latitude = "49.200240",
                longitude = "-122.911785",
            };

            return View(location);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(string sender, string email, string subject, string content)
        {
            try
            {
                NotificationService.NotifyContact(sender, email, subject, content);
                TempData["Result"] = "Email has been successfully send to Omnae Team";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.RetrieveErrorMessage();
                return RedirectToAction("Contact");
            }
            return RedirectToAction("Contact");
        }

        // Chart js
        [HttpPost]
        public JsonResult MultiBarChartData(string[] DateRange, int[] TotalQuantity, int[] TotalCustomerNcrs, int[] TotalVendorNcrs)
        {
            return ChartBl.MultiBarChartData(DateRange, TotalQuantity, TotalCustomerNcrs, TotalVendorNcrs);
        }


        public JsonResult MultiLineChartData(string[] DateRange, int[] TotalQuantity, int[] TotalCustomerNcrs, int[] TotalVendorNcrs)
        {
            return ChartBl.MultiLineChartData(DateRange, TotalQuantity, TotalCustomerNcrs, TotalVendorNcrs);
        }


        public JsonResult MultiPieChartData(string[] DateRange, int[] TotalQuantity, int[] TotalCustomerNcrs, int[] TotalVendorNcrs)
        {
            return ChartBl.MultiPieChartData(DateRange, TotalQuantity, TotalCustomerNcrs, TotalVendorNcrs);
        }

        [OutputCache(Duration = 10)]
        public PartialViewResult ShowModal(int? id, int? taskId, int? orderId = null)
        {

            var tvm = CreateTaskViewModel(taskId, orderId);
            return PartialView("_ModalActionView", tvm);
        }

        [OutputCache(Duration = 10)]
        public TaskDataModalViewModel GetTaskViewModel(int taskId, int? orderId = null)
        {
            var tvm = CreateTaskViewModel(taskId, orderId);
            TaskDataModalViewModel model = new TaskDataModalViewModel { TaskVM = tvm };
            return model;
        }



        public async Task<ActionResult> PerformConnectQBOAuth()
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                Session.Clear();
                Session.Abandon();
                AuthenticationManager.SignOut("Cookies");  //TODO: Check Why have this code.

                //Intialize DiscoverPolicy
                DiscoveryPolicy dpolicy = new DiscoveryPolicy();
                dpolicy.RequireHttps = true;
                dpolicy.ValidateIssuerName = true;


                //Assign the Sandbox Discovery url for the Apps' Dev clientid and clientsecret that you use
                //Or
                //Assign the Production Discovery url for the Apps' Production clientid and clientsecret that you use

                string discoveryUrl = ConfigurationManager.AppSettings["DiscoveryUrl"];
                DiscoveryClient discoveryClient;


                if (discoveryUrl != null && AppConfig.ClientId != null && AppConfig.ClientSecret != null)
                {
                    discoveryClient = new DiscoveryClient(discoveryUrl);
                }
                else
                {
                    Exception ex = new Exception("Discovery Url missing!");
                    throw ex;
                }
                DiscoveryResponse discoveryResponse = await discoveryClient.GetAsync();

                if (discoveryResponse.StatusCode == HttpStatusCode.OK)
                {
                    //Authorize endpoint
                    AppConfig.AuthorizeUrl = discoveryResponse.AuthorizeEndpoint;

                    //Token endpoint
                    AppConfig.TokenEndpoint = discoveryResponse.TokenEndpoint;

                    //Token Revocation enpoint
                    AppConfig.RevocationEndpoint = discoveryResponse.RevocationEndpoint;

                    //UserInfo endpoint
                    AppConfig.UserInfoEndpoint = discoveryResponse.UserInfoEndpoint;

                    //Issuer endpoint
                    AppConfig.IssuerEndpoint = discoveryResponse.Issuer;

                    //JWKS Keys
                    AppConfig.Keys = discoveryResponse.KeySet.Keys;
                }

                //Get mod and exponent value
                foreach (var key in AppConfig.Keys)
                {
                    if (key.N != null)
                    {
                        //Mod
                        AppConfig.Mod = key.N;
                    }
                    if (key.E != null)
                    {
                        //Exponent
                        AppConfig.Expo = key.E;
                    }
                }

                return GetAppNow();
            }
            catch (Exception ex)
            {
                HandleErrorInfo info = new HandleErrorInfo(ex, "Home", "PerformConnectQBOAuth");
                return View("Error", info);
            }
        }

        private async Task<OmnaeInvoice> SyncWithQBOInvoice(OmnaeInvoice invoice)
        {
            QboApi qbo = new QboApi(QboTokensService);
            Invoice inv = await qbo.GetInvoice(invoice.InvoiceId);

            if (inv != null && inv.Balance == 0 && inv.LinkedTxn != null && inv.LinkedTxn.Count() > 0)
            {
                Payment payment = await qbo.GetPayment(inv.LinkedTxn.Last().TxnId);
                if (payment != null && payment.PaymentMethodRef != null)
                {
                    PaymentMethod paymentMethod = await qbo.GetPaymentMethod(payment.PaymentMethodRef.Value);
                    invoice.IsOpen = false;
                    invoice.CloseDate = inv.MetaData.LastUpdatedTime;
                    invoice.PaymentMethod = paymentMethod.Name;
                    invoice.PaymentRefNumber = payment.PaymentRefNum;
                    OmnaeInvoiceService.UpdateOmnaeInvoice(invoice);
                }
            }
            return invoice;
        }

        private async Task<OmnaeInvoice> SyncWithQBOBill(OmnaeInvoice invoice)
        {
            QboApi qbo = new QboApi(QboTokensService);
            Bill bill = await qbo.GetBill(invoice.BillId);
            if (bill != null && bill.Balance == 0 && bill.LinkedTxn != null && bill.LinkedTxn.Count() > 0)
            {
                BillPayment billPayment = await qbo.GetBillPayment(bill.LinkedTxn[0].TxnId);
                if (billPayment.PayType == BillPaymentTypeEnum.Check)
                {
                    BillPaymentCheck billPaymentCheck = (BillPaymentCheck)billPayment.AnyIntuitObject;
                    invoice.PaymentMethod = billPaymentCheck.BankAccountRef.name;
                    invoice.PaymentRefNumber = billPaymentCheck.BankAccountRef.Value;
                }
                else
                {
                    BillPaymentCreditCard billPaymentCreditCard = (BillPaymentCreditCard)billPayment.AnyIntuitObject;
                    invoice.PaymentMethod = billPaymentCreditCard.CCAccountRef.name;
                    invoice.PaymentRefNumber = billPaymentCreditCard.CCAccountRef.Value;
                }

                invoice.IsOpen = false;
                invoice.CloseDate = bill.MetaData.LastUpdatedTime;
                OmnaeInvoiceService.UpdateOmnaeInvoice(invoice);
            }
            return invoice;
        }

        public ActionResult UpdateInvoiceNavigationBar()
        {
            TempData.Keep();
            return PartialView("_InvoiceNavigation");

        }

        public async Task<GetInvoicesViewModel> GetCompanyInvoices(int? companyId, INVOICE_FILTERS? filter = null, int? val = null)
        {
            companyId = companyId ?? UserContext.Company.Id;

            var invoices = new List<OmnaeInvoice>();
            USER_TYPE userType = UserContext.UserType;

            Model.Models.Company company = CompanyService.FindCompanyById(companyId.Value);
            if (company == null)
            {
                TempData["ErrorMessage"] = "Company could not be found";
                return null;
            }

            var invs = OmnaeInvoiceService.FindOmnaeInvoiceListByCompanyId(companyId.Value)
                .Where(x => x.InvoiceNumber != null || x.BillNumber != null)
                .GroupBy(x => x.Id).Select(x => x.First()).ToList();

            if (invs.Count == 0)
            {
                TempData["ErrorMessage"] = "No invoices could be found";
                return null;
            }


            if (filter == null)
            {
                invoices = invs;
            }
            else if (filter == INVOICE_FILTERS.Product && val != null && val > 0)
            {
                invoices = invs.Where(x => x.ProductId == val.Value).ToList();
            }
            else if (filter == INVOICE_FILTERS.Year && val != null && val > 0)
            {
                invoices = invs.Where(x => x.InvoiceDate.Year == val.Value).ToList();
            }

            var allProducts = invs.GroupBy(g => g.ProductId).Select(x => ProductService.FindProductById(x.First().ProductId)).ToList();
            TempData["AllProducts"] = allProducts;
            TempData["Years"] = invs.Select(x => x.InvoiceDate.Year).Distinct().ToList();
            TempData.Keep();


            List<OmnaeInvoiceViewModel> openInvoices = new List<OmnaeInvoiceViewModel>();
            List<OmnaeInvoiceViewModel> closedInvoices = new List<OmnaeInvoiceViewModel>();
            TempData["ErrorMessage"] = null;
            foreach (var inv in invoices)
            {
                if (inv == null)
                {
                    break;
                }

                OmnaeInvoice invoice = inv;
                try
                {
                    if (inv.UserType == (int)USER_TYPE.Customer && invoice.InvoiceId != null)
                    {
                        invoice = await SyncWithQBOInvoice(invoice);
                    }
                    else if (inv.UserType == (int)USER_TYPE.Vendor && invoice.BillId != null)
                    {
                        invoice = await SyncWithQBOBill(invoice);
                    }
                    OmnaeInvoiceViewModel mod = GetOmnaeInvoiceViewModel(invoice, company);
                    if (mod == null)
                    {
                        continue;
                    }
                    if (invoice.IsOpen == true)
                    {
                        openInvoices.Add(mod);
                    }
                    else
                    {
                        closedInvoices.Add(mod);
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "QBO Get Invoice/Bill failed: " + ex.RetrieveErrorMessage();
                    return null;
                }
            }

            string reportName = QBOReport.CUSTOMER_BALANCE;
            if (userType == USER_TYPE.Vendor)
            {
                reportName = QBOReport.VENDOR_BALANCE;
            }
            decimal balance = await GetQboUserBalance(reportName, company.QboId, company.Name);
            decimal exchangeRate = await GetCurrentExchangeRate(DateTime.UtcNow);

            // Convert to USD
            balance /= exchangeRate;

            decimal wip = GetWip(company.Id);
            decimal creditLimit = company.CreditLimit;

            var model = new GetInvoicesViewModel
            {
                Balance = balance,
                Wip = wip,
                CreditLimit = creditLimit,
                AvailableCredit = creditLimit - balance - wip,
                OpenInvoices = openInvoices.OrderBy(x => x.InvoiceDate).ToList(),
                ClosedInvoices = closedInvoices.OrderBy(x => x.InvoiceDate).ToList(),
                UserType = userType,
                Term = company.Term,
            };

            return model;
        }

        [HttpGet]
        public async Task<ActionResult> GetInvoices(INVOICE_FILTERS? filter = null, int? val = null)
        {
            var model = await GetCompanyInvoices(UserContext.Company.Id, filter, val);
            if (model == null)
            {
                TempData.Keep();
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult GetCustomerInvoices()
        {
            var customers = GetCustomers();
            var invoices = OmnaeInvoiceService.FindOmnaeInvoiceList();

            var custs = (from inv in invoices
                         join cust in customers
                         on inv.CompanyId equals cust.Id into inv_cust
                         from ic in inv_cust.DefaultIfEmpty()
                         where inv.InvoiceNumber != null && ic != null && ic.Id > 0
                         select new SelectListItem
                         {
                             Value = ic.Id.ToString(),
                             Text = ic.Name,
                         })
                        .GroupBy(x => x.Value)
                        .Select(x => x.First())
                        .ToList();

            var model = new GetCustomerInvoicesViewModel
            {
                Customers = new SelectList(custs, "Value", "Text"),
            };

            return View(model);
        }





        [HttpGet]
        public async Task<ActionResult> CustomerInvoices(int companyId, INVOICE_FILTERS? filter = null, int? val = null)
        {
            TempData["companyId"] = companyId;
            TempData.Keep();
            GetInvoicesViewModel model = await GetCompanyInvoices(companyId, filter, val);
            if (model == null)
            {
                return RedirectToAction("GetInvoices", "Home");
            }

            return PartialView("_GetInvoices", model);
        }

        [AllowAnonymous]
        public ActionResult InvoiceDetailPdf(OmnaeInvoiceViewModel model)
        {
            if (model.Id == 0)
            {
                return View();
            }

            var invoice = OmnaeInvoiceService.FindOmnaeInvoiceById(model.Id);
            var company = CompanyService.FindCompanyById(model.CompanyId);

            OmnaeInvoiceViewModel mod = GetOmnaeInvoiceViewModel(invoice, company);
            return View(mod);
        }

        [HttpGet]
        public ActionResult ExportInvoiceToPdf(int id, int companyId)
        {
            var invoice = OmnaeInvoiceService.FindOmnaeInvoiceById(id);
            var company = CompanyService.FindCompanyById(companyId);
            if (invoice == null || company == null)
            {
                TempData["ErrorMessage"] = "Invoice or company was not found!";
                TempData.Keep();
                //return RedirectToAction("GetInvoices");
                return RedirectToAction("InvoiceDetailPdf");
            }
            OmnaeInvoiceViewModel model = GetOmnaeInvoiceViewModel(invoice, company);

            var pdf = new ActionAsPdf("InvoiceDetailPdf", model)
            {
                PageOrientation = Rotativa.Options.Orientation.Landscape,
                PageMargins = { Left = 5, Right = 5 },
                FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName,
            };
            return pdf;
        }

        private decimal GetWip(int userId)
        {
            var invoice = OmnaeInvoiceService.FindOmnaeInvoiceListByCompanyId(userId)
                                .Where(x => x.Task != null && x.Task.StateId >= (int)States.OrderPaid && x.Task.StateId < (int)States.ProductionComplete);

            decimal result = invoice != null ? invoice.Sum(x => x.Quantity * x.UnitPrice + x.ToolingSetupCharges) : 0m;
            return result;
        }

        private async Task<decimal> GetQboUserBalance(string reportName, string companyId, string companyName)
        {
            decimal result = 0m;

            QboApi qbo = new QboApi(QboTokensService);
            Report report = await qbo.GetReport(reportName);
            if (report != null)
            {
                foreach (var row in report.Rows)
                {
                    {
                        foreach (var ob in row.AnyIntuitObjects)
                        {
                            ColData[] arr = ob as ColData[];
                            if (companyId != null && arr != null && arr[0].id != null)
                            {
                                if (arr[0].id == companyId)
                                {
                                    result = decimal.Parse(arr[1].value);
                                    return result;
                                }
                            }
                            else if (arr != null && arr[0].value == companyName)
                            {
                                result = decimal.Parse(arr[1].value);
                                return result;
                            }
                        }
                    }
                }
            }

            return result;
        }

        [HttpGet]
        public ActionResult AdminUpdateUserEmail()
        {
            List<ApplicationUser> users = DbUser.Users.OrderBy(x => x.Email).ToList();
            ViewBag.DdlUsers = new SelectList(users, "Id", "Email");

            AdminUpdateUserEmailViewModel model = new AdminUpdateUserEmailViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> AdminUpdateUserEmail(AdminUpdateUserEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = UserManager.FindById(model.Id);
                string oldEmail = user.Email;

                user.Email = model.Email;
                user.UserName = model.Email;
                var result = await UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["Message"] = string.Format("Admin successfully updated user email from {0} to {1}", oldEmail, model.Email);
                }
                else
                {
                    TempData["ErrorMessage"] = "Admin update user email failed";
                    RedirectToAction("AdminUpdateUserEmail");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Some inputs are invalid";
            }

            return RedirectToAction("AdminUpdateUserEmail");
        }

        [HttpGet]
        public ActionResult ChangeShippingDatesForCustomer()
        {
            var customers = GetCustomers();
            var prods = ProductService.FindProductList();

            var custs = (from p in prods.ToList()
                         join c in customers
                         on p.CustomerId equals c.Id into prods_customers
                         from pc in prods_customers.DefaultIfEmpty()
                         where pc != null
                         group pc by new { ProdsCustomersId = pc.Id, ProductId = p.Id } into g
                         select new
                         {
                             CompanyName = g.First().Name,
                             CompanyId = g.Key.ProdsCustomersId.ToString(),
                             ProdId = g.Key.ProductId,
                         });

            var orders = OrderService.FindOrderList();
            var custList = (from o in orders.ToList()
                            join c in custs
                            on o.ProductId equals c.ProdId into order_customers
                            from oc in order_customers.DefaultIfEmpty()
                            where oc != null && o.ShippedDate != null
                            select new SelectListItem
                            {
                                Text = oc.CompanyName,
                                Value = oc.CompanyId,
                            }).GroupBy(x => x.Value).Select(x => x.First()).OrderBy(x => x.Text).ToList(); ;

            ViewBag.Customers = new SelectList(custList, "Value", "Text");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeShippingDatesForCustomer(ChangeShippingDatesForCustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.OrderId != null)
                {
                    var order = OrderService.FindOrderById(model.OrderId.Value);
                    if (order != null)
                    {
                        order.ShippedDate = model.DateToChange;
                        OrderService.UpdateOrder(order);
                        TempData["Result"] = "The order shipping date has been successfully changed";
                        return RedirectToAction("ChangeShippingDatesForCustomer");
                    }
                }
            }
            TempData["ErrorMessage"] = "Change shipping date failed, please try again";
            return View();
        }

        private List<Model.Models.Company> GetCustomers()
        {
            var companies = CompanyService.FindAllCompanies(CompanyType.Customer).OrderBy(x => x.Name).ToList();
            return companies;
        }

        private List<Model.Models.Company> GetVendors()
        {
            var companies = CompanyService.FindAllCompanies(CompanyType.Vendor).OrderBy(x => x.Name).ToList();
            return companies;
        }

        [HttpGet]
        public ActionResult ChangeEstDatesForCustomer()
        {
            var customers = GetCustomers();
            var prods = ProductService.FindProductList();

            var custs = (from p in prods.ToList()
                         join c in customers
                         on p.CustomerId equals c.Id into prods_customers
                         from pc in prods_customers.DefaultIfEmpty()
                         where pc != null
                         group pc by new { ProductCustomerId = pc.Id, ProductId = p.Id } into g
                         select new
                         {
                             CompanyName = g.First().Name,
                             CompanyId = g.Key.ProductCustomerId,
                             ProdId = g.Key.ProductId,
                         });

            var orders = OrderService.FindOrderList().ToList();
            var custList = (from o in orders
                            join c in custs
                            on o.ProductId equals c.ProdId into order_customers
                            from oc in order_customers.DefaultIfEmpty()
                            where oc != null && o.EstimateCompletionDate != null
                            select new SelectListItem
                            {
                                Text = oc.CompanyName,
                                Value = oc.CompanyId.ToString(),
                            }).GroupBy(x => x.Value).Select(x => x.First()).OrderBy(x => x.Text).ToList();

            ViewBag.Customers = new SelectList(custList, "Value", "Text");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeEstDatesForCustomer(ChangeEstForCustomerViewModel model)
        {
            if (ModelState.IsValid && model.OrderId != null)
            {
                var order = OrderService.FindOrderById(model.OrderId.Value);
                if (order != null)
                {
                    order.EstimateCompletionDate = model.DateToChange;
                    OrderService.UpdateOrder(order);
                    TempData["Result"] = "The order shipping date has been successfully changed";
                    return RedirectToAction("ChangeEstDatesForCustomer");
                }
            }
            TempData["ErrorMessage"] = "Change shipping date failed, please try again";
            return View();
        }

        public ActionResult GetOrderEstCompletionDates(int companyId)
        {
            var orders = OrderService.FindOrderByCustomerId(companyId).GroupBy(x => x.Id).Select(x => x.First()).ToList();
            var products = ProductService.FindProductListByCompanyId(companyId);
            if (orders.Any())
            {
                var orderList = (from od in orders
                                 join pd in products on od.ProductId equals pd.Id into order_product
                                 from op in order_product.DefaultIfEmpty()
                                 where od.EstimateCompletionDate != null
                                 orderby op.PartNumber
                                 select new SelectListItem
                                 {
                                     Text = $"Pt#: {op.PartNumber} ( PO# {od.CustomerPONumber}, Est. Compl. Date: {od.EstimateCompletionDate.Value.ToString("MM-dd-yyyy")} ) ",
                                     Value = od.Id.ToString(),
                                 }
                    ).GroupBy(x => x.Value).Select(y => y.First()).ToList();

                var model = new ChangeEstForCustomerViewModel
                {
                    CustomerId = companyId,
                    OrderId = null,
                    OrderList = new SelectList(orderList, "Value", "Text"),
                };
                return PartialView("_OrderEstimates", model);
            }

            TempData["ErrorMessage"] = "This customer has no order yet!";
            return PartialView("_OrderList", null);
        }
        public ActionResult GetOrderList(int companyId)
        {
            var orders = OrderService.FindOrderByCustomerId(companyId);
            var products = ProductService.FindProductListByCompanyId(companyId);
            if (orders.Any())
            {
                var orderList = (from od in orders
                                 join pd in products
                                 on od.ProductId equals pd.Id into order_product
                                 from op in order_product.DefaultIfEmpty()
                                 where od.ShippedDate != null
                                 orderby op.PartNumber
                                 select new SelectListItem
                                 {
                                     Text = $"Pt#: {op.PartNumber} ( PO#: {od.CustomerPONumber}, Ship Date: {od.ShippedDate} )",
                                     Value = od.Id.ToString(),
                                 }
                    ).GroupBy(x => x.Value).Select(y => y.First()).ToList();

                ChangeShippingDatesForCustomerViewModel model = new ChangeShippingDatesForCustomerViewModel
                {
                    CustomerId = companyId,
                    OrderId = null,
                };
                TempData["OrderList"] = new SelectList(orderList, "Value", "Text");
                return PartialView("_OrderList", model);
            }
            else
            {
                TempData["ErrorMessage"] = "This customer has no order yet!";
                return PartialView("_OrderList", null);
            }
        }

        [HttpGet]
        public ActionResult GetQtyUnitPrices(int prodId)
        {
            var pbs = PriceBreakService.FindPriceBreakByProductId(prodId);
            if (pbs == null || pbs.Count == 0)
            {
                TempData["ErrorMessage"] = "No Unit Prices were found for this part! Please choose another part.";
                return PartialView("_GetQtyUnitPrices", null);
            }

            var model = new ModifyUnitPricesViewModel
            {
                UnitPriceList = new List<GetQtyUnitPrices>(),
            };
            foreach (var pb in pbs)
            {
                if (pb.RFQBidId > 0)
                {
                    var rfqbid = RfqBidService.FindRFQBidById(pb.RFQBidId.Value);
                    if (rfqbid.ProductId != prodId && (rfqbid.IsActive == null || rfqbid.IsActive == false))
                    {
                        continue;
                    }
                }
                var qup = new GetQtyUnitPrices
                {
                    PriceBreakId = pb.Id,
                    Quantity = pb.Quantity,
                    CustomerUnitPrice = pb.UnitPrice ?? 0m,
                    VendorUnitPrice = pb.VendorUnitPrice,
                    CustomerToolingCharge = pb.CustomerToolingSetupCharges ?? 0m,
                    VendorToolingCharge = pb.ToolingSetupCharges ?? 0m,
                };
                model.UnitPriceList.Add(qup);
            }
            var product = ProductService.FindProductById(prodId);
            model.VendorName = product.VendorCompany != null ? product.VendorCompany.Name : null;
            return PartialView("_GetQtyUnitPrices", model);
        }

        [HttpGet]
        public ActionResult ModifyUnitPrices()
        {
            var model = new ModifyUnitPricesViewModel
            {
                Customers = new SelectList(CreateCustomerList(), "Value", "Text"),
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ModifyUnitPrices(ModifyUnitPricesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Data validation failed!";
                return RedirectToAction("ModifyUnitPrices");
            }

            foreach (var ele in model.UnitPriceList)
            {
                var pb = PriceBreakService.FindPriceBreakById(ele.PriceBreakId);
                pb.Quantity = ele.Quantity;
                pb.UnitPrice = ele.CustomerUnitPrice;
                pb.VendorUnitPrice = ele.VendorUnitPrice;
                pb.CustomerToolingSetupCharges = ele.CustomerToolingCharge;
                pb.ToolingSetupCharges = ele.VendorToolingCharge;
                try
                {
                    PriceBreakService.ModifyPriceBreak(pb);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Exceptin thrown: {ex.InnerException?.InnerException?.Message ?? (ex.InnerException?.Message ?? ex.Message)}";
                    return RedirectToAction("ModifyUnitPrices");
                }
            }

            TempData["Result"] = "Successfully updated unit prices";

            return RedirectToAction("ModifyUnitPrices");
        }

        [HttpGet]
        public ActionResult GetProductList(int? companyId)
        {
            TempData["Result"] = null;
            TempData["ErrorMessage"] = null;

            if (companyId == null)
            {
                TempData["ErrorMessage"] = "This customer has no parts yet! Please choose another part.";
                return PartialView("_ProductDropdown", null);
            }
            var products = ProductService.FindProductListByCustomerId(companyId.Value);
            if (products?.Any() != true)
            {
                TempData["ErrorMessage"] = "This customer has no parts yet!";
                return PartialView("_ProductDropdown", null);
            }

            var productList = (from pd in products
                               orderby pd.PartNumber
                               group pd by (pd.PartNumber, pd.PartNumberRevision)
                into g
                               select new SelectListItem
                               {
                                   Text = $"Part/No: {g.First().PartNumber}, Part/Rev: {g.First().PartNumberRevision}",
                                   Value = g.First().Id.ToString(),
                               }).ToList();

            var model = new ModifyUnitPricesViewModel
            {
                CustomerId = companyId.Value,
                Products = new SelectList(productList, "Value", "Text"),
            };

            TempData["ProductList"] = model;

            return PartialView("_ProductDropdown", model);
        }

        public ActionResult MarginReport()
        {
            DateRangeViewModel model = new DateRangeViewModel();
            return View(model);
        }

        public async Task<List<TransactionViewModel>> GetMarginReport(DateTime start, DateTime end)
        {
            end = end.AddHours(23).AddMinutes(59).AddSeconds(59);

            var tasks = TaskDataService.GetTaskDataAll();
            var taskList = tasks.Where(x => x.StateId >= (int)States.OrderInitiated);

            var orderList = OrderService.FindOrderList();

            var orders = (from td in taskList
                          join od in orderList
                          on td.TaskId equals od.TaskId into taskdata_order
                          from to in taskdata_order.DefaultIfEmpty()
                          where to != null && to.Quantity > 0 && to.ShippedDate >= start && to.ShippedDate <= end
                          select new Order
                          {
                              Id = to.Id,
                              ProductId = to.ProductId,
                              Quantity = to.Quantity,
                              SalesTax = to.SalesTax,
                              OrderDate = to.OrderDate,
                              ShippedDate = to.ShippedDate,
                          }).ToList();

            var priceBreakList = PriceBreakService.FindPriceBreakList();

            var pbs = (from od in orders
                       join pb in priceBreakList
                       on od.ProductId equals pb.ProductId into order_pricebreak
                       from ppb in order_pricebreak.DefaultIfEmpty()
                       where ppb != null && ppb.Quantity > 0 && ppb.RFQBidId > 0 && ppb.ShippingUnitPrice != null
                       select new PriceBreak
                       {
                           Id = ppb.Id,
                           ProductId = ppb.ProductId,
                           RFQBidId = ppb.RFQBidId,
                           Quantity = ppb.Quantity,
                           VendorUnitPrice = ppb.VendorUnitPrice,
                           ShippingUnitPrice = ppb.ShippingUnitPrice
                       }).ToList();

            List<TransactionViewModel> marginReport = new List<TransactionViewModel>();

            foreach (var order in orders)
            {
                decimal exchangeRate = await GetCurrentExchangeRate(order.OrderDate);
                var productId = order.ProductId;
                var product = ProductService.FindProductById(productId);
                var customer = CompanyService.FindCompanyById(product.CustomerId.Value);
                var vendor = CompanyService.FindCompanyById(product.VendorId.Value);

                var inv = OmnaeInvoiceService.FindOmnaeInvoiceByCompanyIdByOrderId(customer.Id, order.Id);
                var vinv = OmnaeInvoiceService.FindOmnaeInvoiceByCompanyIdByOrderId(vendor.Id, order.Id);
                if ((inv == null || vinv == null))
                {
                    continue;
                }

                string pn = product.PartNumber + " " + product.PartNumberRevision;
                PriceBreak pb = null;

                if (order.Quantity > 1)
                {
                    pb = pbs.Where(x => x.Quantity <= order.Quantity).OrderBy(x => x.Quantity).LastOrDefault();
                    if (pb == null)
                    {
                        continue;
                    }
                }

                // Sales
                decimal total1 = inv.Quantity * inv.UnitPrice;
                decimal total2 = total1 + inv.ToolingSetupCharges;
                double taxRate = ShipmentBL.GetSalesTax(customer.Address);
                decimal salesTax = inv.SalesTax; //(decimal)taxRate * total2;
                decimal netTotalCAD = (total2 + salesTax) * exchangeRate;

                // Purchase
                decimal vendorTotal1 = vinv.Quantity * vinv.UnitPrice;
                decimal vendorTotal2 = vendorTotal1 + vinv.ToolingSetupCharges;
                decimal vendorTotalCAD = vendorTotal2 * exchangeRate;
                decimal shippingCAD = 0m;
                if (pb != null && pb.ShippingUnitPrice != null)
                {
                    shippingCAD = pb.ShippingUnitPrice.Value * vinv.Quantity * exchangeRate;
                }
                decimal vendorNetTotalCAD = vendorTotalCAD + shippingCAD;
                decimal profitLossCAD = netTotalCAD - vendorNetTotalCAD;


                TransactionViewModel trans = new TransactionViewModel
                {
                    CustomerName = customer.Name,
                    PNRev = pn,
                    Quantity = pb.Quantity,
                    ExchangeRate = exchangeRate,
                    OrderDate = order.OrderDate,
                    ShipDate = order.ShippedDate,

                    // Sales
                    EstimateNumber = inv.EstimateNumber,
                    CustomerTotal1 = total1,
                    CustomerToolingSetup = inv.ToolingSetupCharges,
                    CustomerTotal2 = total2,
                    SalesTax = salesTax,
                    CustomerNetTotalCAD = netTotalCAD,

                    // Purchase
                    VendorName = vendor.Name,
                    PONumber = vinv.PONumber.ToString(),
                    POUri = vinv.PODocUri,
                    VendorTotal1 = vendorTotal1,
                    VendorToolingSetup = vinv.ToolingSetupCharges,
                    VendorTotal2 = vendorTotal2,
                    VendorTotalCAD = vendorTotalCAD,
                    ShippingCAD = shippingCAD,
                    VendorNetTotalCAD = vendorNetTotalCAD,

                    // Calculation 
                    ProfitLossCAD = profitLossCAD,
                    GrossProfitMargin = (double)(profitLossCAD / netTotalCAD) * 100d,
                    InvoiceNumber = inv.InvoiceNumber,
                };
                marginReport.Add(trans);
            }
            return marginReport.OrderBy(x => x.ShipDate).ToList();
        }

        [HttpPost]
        public async Task<ActionResult> ChooseMarginReportDateRange(DateTime? start, DateTime? end)
        {
            var model = await GetMarginReport(start.Value, end.Value);
            return PartialView("_MarginReport", model);
        }

        [HttpGet]
        public IList<WipStatusReportViewModel> GetWipStatusReport(int? customerId = null, int? vendorId = null, DateTime? start = null, DateTime? end = null)
        {
            var tasks = TaskDataService.GetTaskDataAll();
            var taskList = tasks.Where(x => x.StateId >= (int)States.OrderInitiated && x.StateId < (int)States.ProductionComplete ||
                                            x.StateId == (int)States.AddExtraQuantities || x.StateId == (int)States.VendorPendingInvoice);

            if (customerId != null)
            {
                taskList = taskList.Where(x => x.Product != null && x.Product.CustomerId == customerId).ToList();
            }
            if (vendorId != null)
            {
                taskList = taskList.Where(x => x.Product != null && x.Product.VendorId == vendorId).ToList();
            }

            var orderList = OrderService.FindOrderList();

            var orders = (from td in taskList
                          join od in orderList
                          on td.TaskId equals od.TaskId into taskdata_order
                          from to in taskdata_order.DefaultIfEmpty()
                          where to != null && (to.Quantity > 0 || to.IsForToolingOnly == true) && to.ProductId == td.ProductId
                          select new OrderTaskDataViewModel
                          {
                              OrderId = to.Id,
                              TaskId = to.TaskId,
                              ProductId = to.ProductId,
                              Product = to.Product,
                              OrderDate = to.OrderDate,
                              Quantity = to.Quantity,
                              ShippedDate = to.ShippedDate,
                              CarrierName = to.CarrierName,
                              TrackingNumber = to.TrackingNumber,
                              EstimateCompletionDate = to.EstimateCompletionDate,
                              PartNumber = to.PartNumber,
                              CustomerPONumber = to.CustomerPONumber,
                              StateId = td.StateId,
                              Notes = to.Notes,
                              RequestedShipDate = to.DesireShippingDate,
                          }).GroupBy(x => x.OrderId).Select(x => x).ToList();

            if (start != null)
            {
                orders = orders.Where(x => x.First() != null && x.First().OrderDate >= start).ToList();
            }
            if (end != null)
            {
                end = end.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                orders = orders.Where(x => x.First() != null && x.First().OrderDate <= end).ToList();
            }

            List<WipStatusReportViewModel> wipStatusReport = new List<WipStatusReportViewModel>();

            foreach (var order in orders)
            {
                var product = order.First().Product;
                if (product == null)
                {
                    continue;
                }

                var inv = OmnaeInvoiceService.FindOmnaeInvoiceByUserTypeByOrderId(USER_TYPE.Customer, order.Key);
                var vinv = OmnaeInvoiceService.FindOmnaeInvoiceByUserTypeByOrderId(USER_TYPE.Vendor, order.Key);
                if ((inv == null || vinv == null))
                {
                    continue;
                }

                WipStatusReportViewModel trans = new WipStatusReportViewModel
                {
                    OrderId = order.First().OrderId,
                    PartNumber = product.PartNumber,
                    PartNumberRevision = product.PartNumberRevision,
                    Qty = order.First().Quantity,
                    OrderDate = order.First().OrderDate,
                    EstimatedCompletionDate = order.Last().EstimateCompletionDate != null ? order.Last().EstimateCompletionDate : null,
                    EstimateNumber = inv.EstimateNumber,
                    CustomerId = inv.CompanyId,
                    CustomerName = CompanyService.FindCompanyById(inv.CompanyId).Name,
                    VendorId = vinv.CompanyId,
                    VendorName = CompanyService.FindCompanyById(vinv.CompanyId).Name,
                    CustomerPONumber = inv.PONumber ?? order.First().CustomerPONumber,
                    CustomerPOUri = inv.PODocUri,
                    VendorPONumber = vinv.PONumber,
                    VendorPOUri = vinv.PODocUri,
                    State = (States)order.First().StateId,
                    Notes = order.First().Notes,
                    RequestedShipDate = order.Last().RequestedShipDate,
                };
                wipStatusReport.Add(trans);
            }
            return wipStatusReport.OrderBy(x => x.OrderDate).ToList();
        }

        [HttpGet]
        public IList<OrderHistoryViewModel> GetOrderHistory(int? customerId = null, int? vendorId = null, DateTime? start = null, DateTime? end = null)
        {
            var query = from ohvm in OrderService.GetOrderHistory(customerId, vendorId, start, end)
                        let order = ohvm.Order
                        let product = order.Product
                        let inv = ohvm.CustomerInvoice
                        let vinv = ohvm.VendorInvoice
                        let customerName = ohvm.CustomerName
                        let vendorName = ohvm.VendorName
                        orderby order.OrderDate
                        select new OrderHistoryViewModel
                        {
                            OrderId = order.Id,
                            PartNumber = product.PartNumber,
                            PartNumberRevision = product.PartNumberRevision,
                            OrderDate = order.OrderDate,
                            EstimateNumber = inv.EstimateNumber,
                            CustomerId = inv.CompanyId,
                            CustomerName = customerName,
                            VendorId = vinv.CompanyId,
                            VendorName = vendorName,
                            CustomerPONumber = inv.PONumber ?? order.CustomerPONumber,
                            CustomerPOUri = inv.PODocUriFromBd,
                            VendorPONumber = vinv.PONumber,
                            VendorPOUri = vinv.PODocUriFromBd,
                            ShippedDate = order.ShippedDate,
                            CarrierName = order.CarrierName,
                            TrackingNumber = order.TrackingNumber,
                            Notes = order.Notes,
                            RequestedShipDate = order.DesireShippingDate,
                        };

            var result = query.ToList();

            foreach (var model in result)
            {
                model.CustomerPOUri = DocumentStorageService.AddSecurityTokenToUrl(model.CustomerPOUri);
                model.VendorPOUri = DocumentStorageService.AddSecurityTokenToUrl(model.VendorPOUri);
            }

            return result;
        }

        [HttpGet]
        public ActionResult WipStatusReport()
        {
            IEnumerable<WipStatusReportViewModel> model = GetWipStatusReport();
            var customers = model.GroupBy(y => (y.CustomerId, y.CustomerName)).Select(x => x.First()).OrderBy(x => x.CustomerName);
            var vendors = model.GroupBy(y => (y.VendorId, y.VendorName)).Select(x => x.First()).OrderBy(x => x.VendorName);
            TempData["Customers"] = new SelectList(customers, "CustomerId", "CustomerName");
            TempData["Vendors"] = new SelectList(vendors, "VendorId", "VendorName");
            return View(model);
        }

        [HttpGet]
        public ActionResult OrderHistory()
        {
            IEnumerable<OrderHistoryViewModel> model = GetOrderHistory();
            var customers = model.GroupBy(y => (y.CustomerId, y.CustomerName)).Select(x => x.First()).OrderBy(x => x.CustomerName);
            var vendors = model.GroupBy(y => (y.VendorId, y.VendorName)).Select(x => x.First()).OrderBy(x => x.VendorName);
            TempData["Customers"] = new SelectList(customers, "CustomerId", "CustomerName");
            TempData["Vendors"] = new SelectList(vendors, "VendorId", "VendorName");
            return View(model);
        }

        [HttpPost]
        public ActionResult GetWipStatusReportByFilters(int? customerId = null, int? vendorId = null, DateTime? start = null, DateTime? end = null)
        {
            IEnumerable<WipStatusReportViewModel> model = GetWipStatusReport(customerId, vendorId, start, end);
            return PartialView("_WipStatusReport", model);
        }

        [HttpPost]
        public ActionResult GetOrderHistoryByFilters(int? customerId = null, int? vendorId = null, DateTime? start = null, DateTime? end = null)
        {
            IEnumerable<OrderHistoryViewModel> model = GetOrderHistory(customerId, vendorId, start, end);
            return PartialView("_OrderHistory", model);
        }

        public async Task<ActionResult> ExportFileExcel(DateTime MarginReportStartDate, DateTime MarginReportEndDate)
        {
            Response.AddHeader("content-disposition", "attachment;filename=MarginReport.xls");
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");

            var model = await GetMarginReport(MarginReportStartDate, MarginReportEndDate);
            return PartialView("_MarginReport", model);
        }

        private List<SelectListItem> CreateCustomerList()
        {
            var custs = ProductService.FindProductList()
                                      .Where(x => x.CustomerCompany != null && x.CustomerId != null)
                                      .Select(x => new SelectListItem { Text = x.CustomerCompany.Name, Value = x.CustomerId.ToString() })
                                      .GroupBy(x => x.Value).Select(y => y.FirstOrDefault()).OrderBy(x => x.Text).ToList();
            return custs;
        }

        private List<Model.Models.Company> CreateVendorList()
        {
            var companies = CompanyService.FindAllCompanies(CompanyType.Vendor);
            var result = from cc in companies.ToList()
                         select new Model.Models.Company
                         {
                             Id = cc.Id,
                             Name = cc.Name,
                         };
            return result.ToList();
        }

        [AllowAnonymous]
        public ActionResult PackingSlip(PackingSlipViewModel model)
        {
            return View(model);
        }




        [HttpGet]
        public ActionResult CreatePackingSlip(int orderId)
        {
            try
            {
                var order = OrderService.FindOrderById(orderId);
                var model = HomeBl.CreatePackingSlip(order.TaskData, this.ControllerContext, order);
                return PartialView("_PackingSlipUri", model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.RetrieveErrorMessage();
                return RedirectToAction("CreatePackingSlip", orderId);
            }
        }

        [HttpGet]
        public ActionResult RFQLog()
        {
            var taskDatas = TaskDataService.GetTaskDataAll().Where(x => x.RFQBidId != null).ToList();

            var taskDatas2 = TaskDataService.GetTaskDataAll()
                .Where(x => x.StateId == (int)States.PendingRFQ)
                .GroupBy(x => x.TaskId)
                .Select(g => new
                {
                    ProductId = g.First().ProductId,
                    StateId = g.First().StateId,
                    BidDateTime = DateTime.MinValue,
                    TaskId = g.First().TaskId,
                    QuoteAcceptedDate = (DateTime?)null,
                    CreatedUTCDate = g.First().CreatedUtc,
                }).ToList();

            var products = ProductService.FindProductList().ToList();
            var rfqBids = RfqBidService.FindRFQBidList();


            var list1 = (from rfq in rfqBids
                         join td in taskDatas
                         on rfq.Id equals td.RFQBidId into rfq_td
                         from rt in rfq_td.DefaultIfEmpty()
                         where rt != null
                         select new
                         {
                             ProductId = rt.ProductId,
                             StateId = rt.StateId,
                             BidDateTime = rfq.BidDatetime,
                             TaskId = rt.TaskId,
                             QuoteAcceptedDate = rfq.QuoteAcceptDate,
                             CreatedUTCDate = rt.CreatedUtc,
                         }).ToList();

            taskDatas2.AddRange(list1);

            var model = (from pd in products
                         join lst in taskDatas2
                         on pd.Id equals lst.ProductId into pd_lst
                         from pl in pd_lst.DefaultIfEmpty()
                         where pl != null
                         select new RFQLogViewModel
                         {
                             RFQCreatedDate = pl.CreatedUTCDate,
                             CustomerName = CompanyService.FindCompanyById(pd.CustomerId.Value).Name,
                             ProductType = pd.Material,
                             PartNumber = pd.PartNumber,
                             RevisionNumber = pd.PartNumberRevision,
                             QuoteAcceptDate = pl.QuoteAcceptedDate != null ? pl.QuoteAcceptedDate.Value : (Nullable<DateTime>)null,
                             SelectedVendorName = pd.VendorId != null ? CompanyService.FindCompanyById(pd.VendorId.Value).Name : null,
                             CurrentState = (States)pl.StateId,
                         }).ToList();

            return View(model);
        }

        [HttpGet]
        public ActionResult UploadVendors()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UploadVendors(UploadVendorsProductsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Result"] = "Validation Error";
                return View();
            }

            using (var trans = AsyncTransactionScope.StartNew())
            {
                TempData["ErrorMessage"] = null;
                TempData["Result"] = null;

                if (model.InputVendorsExcel.ContentLength == 0)
                {
                    TempData["ErrorMessage"] = "Selected input file is empty";
                    return RedirectToAction("UploadVendors", "Home");
                }

                string inputFileName = Path.GetFileName(model.InputVendorsExcel.FileName);
                string inputFilePath = Path.Combine(HostingEnvironment.MapPath(@"~/Docs/"), inputFileName);  //TODO: Do Better
                model.InputVendorsExcel.SaveAs(inputFilePath);

                try
                {
                    await OnBoardingBl.ImportVendorAsync(inputFilePath, isNewSystem: false);
                }
                catch (DbEntityValidationException ex)
                {
                    TempData["ErrorMessage"] = RetrieveDbEntityValidationException(ex);
                    return RedirectToAction("UploadVendors", "Home");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex.RetrieveErrorMessage();
                    return RedirectToAction("UploadVendors", "Home");
                }

                TempData["Result"] = "Succeeded uploading vendors to OMNAE";
                trans.Complete();
                return View();
            }
        }



        [HttpGet]
        public ActionResult UploadVendorProducts()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadVendorProducts(UploadVendorsProductsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Validation Error";
                return View();
            }

            TempData["ErrorMessage"] = null;
            TempData["Result"] = null;

            if (model.InputProductsExcel.ContentLength == 0)
            {
                TempData["ErrorMessage"] = "Selected input file is empty";
                return RedirectToAction("UploadVendorProducts", "Home");
            }

            string inputFileName = Path.GetFileName(model.InputProductsExcel.FileName);
            string inputFilePath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(@"~/Docs/"), inputFileName);  //TODO: Do Better
            model.InputProductsExcel.SaveAs(inputFilePath);

            var inputList = new List<VendorProductDataViewModel>();
            try
            {
                var hasDupe = OnBoardingBl.ImportPartsAsync(inputFilePath);

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.RetrieveErrorMessage();
                return RedirectToAction("UploadVendorProducts", "Home");
            }

            TempData["Result"] = "Uploading Products to OMNAE Succeeds";
            return View();
        }



        [HttpGet]
        public ActionResult AddCompanyLogo()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCompanyLogo(AddCompanyLogoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Home");
            }

            TempData["ErrorMessage"] = null;
            TempData["Result"] = null;

            var company = CompanyService.FindCompanyByUserId(UserContext.UserId);

            if (company == null)
            {
                TempData["ErrorMessage"] = "Your company cannot be found";
                return RedirectToAction("AddCompanyLogo", "Home");
            }
            string logoName = $"Logo_{company.Name}";

            company.CompanyLogoUri = ImageStorageService.Upload(model.CompanyLogo, logoName);
            CompanyService.UpdateCompany(company);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult AddShippingAccount(int? companyId = null)
        {
            if (IsCurrentUserAdmin())
            {
                var companies = GetCustomers();
                var model = new ShippingAccountViewModel
                {
                    Companies = new SelectList(companies, "Id", "Name"),
                };
                if (companyId != null)
                {
                    model.ShippingAccountList = ShippingAccountService.FindShippingAccountByCompanyId(companyId.Value);
                }
                return View(model);
            }
            else
            {
                var company = CompanyService.FindCompanyByUserId(UserContext.UserId);
                if (company == null)
                {
                    TempData["ErrorMessage"] = "Your company is not existing";
                    return RedirectToAction("AddShippingAccount");
                }
                var shippingAccounts = ShippingAccountService.FindShippingAccountByCompanyId(company.Id);
                var model = new ShippingAccountViewModel
                {
                    ShippingAccountList = shippingAccounts,
                };
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult GetShippingAccount(int companyId)
        {
            var shippingAccounts = ShippingAccountService.FindShippingAccountByCompanyId(companyId);
            var model = new ShippingAccountViewModel
            {
                ShippingAccountList = shippingAccounts,
            };
            return PartialView("_ShippingAccount", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddShippingAccount(ShippingAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                TempData["ErrorMessage"] = null;

                var company = model.CompanyId > 0 ? CompanyService.FindCompanyById(model.CompanyId)
                                                  : CompanyService.FindCompanyByUserId(UserContext.UserId);
                var shippingAccount = new ShippingAccount
                {
                    Name = model.Name,
                    AccountNumber = model.AccountNumber,
                    CompanyId = company.Id,
                    Carrier = Enum.GetName(typeof(SHIPPING_CARRIER), model.Carrier),
                    CarrierType = model.CarrierType,
                };
                ShippingAccountService.AddShippingAccount(shippingAccount);
            }
            else
            {
                TempData["ErrorMessage"] = "Input data is missing or invalid";
            }
            return RedirectToAction("AddShippingAccount", "Home", new { @companyId = model.CompanyId });


        }

        [HttpGet]
        public ActionResult DeleteShippingAccount(int id)
        {
            var entity = ShippingAccountService.FindShippingAccountById(id);
            var companyId = entity.CompanyId;
            try
            {
                ShippingAccountService.RemoveShippingAccount(entity);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.RetrieveErrorMessage();
                return RedirectToAction("AddShippingAccount", "Home");
            }
            var shippingAccounts = ShippingAccountService.FindShippingAccountByCompanyId(companyId);
            var model = new ShippingAccountViewModel
            {
                ShippingAccountList = shippingAccounts,
            };
            return PartialView("_ShippingAccount", model);
        }

        [NonAction]
        public bool IsCurrentUserAdmin()
        {
            return UserContext.UserId.Equals(ConfigurationManager.AppSettings["AdminId"], StringComparison.CurrentCultureIgnoreCase);
        }

        [HttpGet]
        public ActionResult AssignCustomerEnterprise()
        {
            var companies = GetCustomers();
            var model = new AssignCustomerEnterpriseViewModel
            {
                Companies = new SelectList(companies, "Id", "Name"),
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignCustomerEnterprise(AssignCustomerEnterpriseViewModel model)
        {
            if (ModelState.IsValid)
            {
                var company = CompanyService.FindCompanyById(model.CompanyId);
                if (company == null)
                {
                    TempData["ErrorMessage"] = "Company could not be found";
                    return RedirectToAction("AssignCustomerEnterprise", "Home");
                }
                company.isEnterprise = model.CustomerType == CUSTOMER_TYPE.Subscriber;
                CompanyService.UpdateCompany(company);

                TempData["Result"] = $"'{company.Name}' has been successfully assigned to be a {Enum.GetName(typeof(CUSTOMER_TYPE), model.CustomerType)}.";
                TempData["ErrorMessage"] = null;
            }
            else
            {
                TempData["ErrorMessage"] = "Input data is missing or invalid";
                TempData["Result"] = null;
            }
            return RedirectToAction("AssignCustomerEnterprise", "Home");
        }

        [HttpGet]
        public ActionResult AddApprovedCapabilityToVendor()
        {
            var vendors = (from v in CompanyService.FindAllCompanies()
                           orderby v.Name
                           select new SelectListItem
                           {
                               Value = v.Id.ToString(),
                               Text = v.Name,
                           }).ToList();

            var model = new AddApprovedCapabilityToVendorViewModel
            {
                VendorList = new SelectList(vendors, "Value", "Text"),
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult AddApprovedCapabilityToVendor(AddApprovedCapabilityToVendorViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApprovedCapability ac = mapper.Map<ApprovedCapability>(model);
                try
                {
                    ApprovedCapabilityService.AddApprovedCapability(ac);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex.RetrieveErrorMessage();
                }
                TempData["Result"] = "Success!";
            }
            //return RedirectToAction("AddApprovedCapabilityToVendor", "Home");
            return PartialView("_ExistingCapabilities", model);
        }

        [HttpPost]
        public ActionResult AddApprovedCapabilities(int vendorId,
                                                    BUILD_TYPE buildType,
                                                    MATERIALS_TYPE materialType,
                                                    Metals_Processes? metalProcess,
                                                    Plastics_Processes? plasticsProcess,
                                                    Process_Type? processType)
        {
            if (ModelState.IsValid)
            {
                var ac = new ApprovedCapability
                {
                    VendorId = vendorId,
                    BuildType = buildType,
                    MaterialType = materialType,
                    MetalProcess = metalProcess,
                    PlasticsProcess = plasticsProcess,
                    ProcessType = processType,
                };

                try
                {
                    ApprovedCapabilityService.AddApprovedCapability(ac);
                    TempData["Result"] = "Success!";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex.RetrieveErrorMessage();
                }
            }

            var caps = ApprovedCapabilityService.FindApprovedCapabilityByVendorId(vendorId);
            var model = new AddApprovedCapabilityToVendorViewModel
            {
                ExistingApprovedCapabilities = caps,
            };

            return PartialView("_ExistingCapabilities", model);
        }

        [HttpGet]
        public ActionResult GetVendorExistingApprovedCapabilities(int vendorId)
        {
            var ap = ApprovedCapabilityService.FindApprovedCapabilityByVendorId(vendorId);
            var model = new AddApprovedCapabilityToVendorViewModel
            {
                ExistingApprovedCapabilities = ap,
            };
            return PartialView("_ExistingCapabilities", model);
        }

        [HttpGet]
        public ActionResult DeleteApprovedCapabilities(int id)
        {
            var entity = ApprovedCapabilityService.FindApprovedCapabilityById(id);
            var vendorId = entity.VendorId;
            try
            {
                ApprovedCapabilityService.DeleteApprovedCapability(entity);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.RetrieveErrorMessage();
                return RedirectToAction("AddApprovedCapabilityToVendor", "Home");
            }

            var ac = ApprovedCapabilityService.FindApprovedCapabilityByVendorId(vendorId);
            var model = new AddApprovedCapabilityToVendorViewModel
            {
                ExistingApprovedCapabilities = ac,
            };
            return PartialView("_ExistingCapabilities", model);
        }

        public void ExportVendorBillToPdf(BillViewModel model)
        {
            if (model == null)
            {
                return;
            }

            // vendor bill
            string fileName = $"vendor_bill_vid-{model.VendorId.Value}_tid-{model.TaskId}_pid-{model.ProductId}_orderid-{model.OrderId}.pdf";
            var pdf = new ActionAsPdf("ExportVendorBill", model)
            {
                PageOrientation = Rotativa.Options.Orientation.Landscape,
                PageMargins = { Left = 5, Right = 5 },
                FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName,
            };

            byte[] pdfBytes = pdf.BuildFile(this.ControllerContext);
            MemoryStream ms = new MemoryStream(null); //BUG: ???? Check better
            var docUri = DocumentStorageService.Upload(ms, fileName);

            var doc = new Document()
            {
                TaskId = model.TaskId,
                Version = 1,
                Name = fileName,
                DocUri = docUri,
                DocType = (int)DOCUMENT_TYPE.QBO_BILL_PDF,
                ProductId = model.ProductId,
                UpdatedBy = User.Identity.Name,
                CreatedUtc = DateTime.UtcNow,
                CreatedByUserId = UserContext.UserId,
                ModifiedUtc = DateTime.UtcNow
            };
            var d = DocumentService.FindDocumentByProductId(model.ProductId).FirstOrDefault(x => x.Name == fileName);
            if (d == null)
            {
                DocumentService.AddDocument(null);
            }

            // vendor attached invoice
            if (model.isEnterprise && model.AttachInvoice != null)
            {
                fileName = $"vendor_invoice_vid-{model.VendorId.Value}_tid-{model.TaskId}_pid-{model.ProductId}_orderid-{model.OrderId}.pdf";
                docUri = DocumentStorageService.Upload(model.AttachInvoice, fileName);
                doc = new Document()
                {
                    TaskId = model.TaskId,
                    Version = 1,
                    Name = fileName,
                    DocUri = docUri,
                    DocType = (int)DOCUMENT_TYPE.ENTERPRISE_VENDOR_INVOICE_PDF,
                    ProductId = model.ProductId,
                    UpdatedBy = User.Identity.Name,
                    CreatedUtc = DateTime.UtcNow,
                    CreatedByUserId = UserContext.UserId,
                    ModifiedUtc = DateTime.UtcNow
                };
                d = DocumentService.FindDocumentByProductId(model.ProductId).FirstOrDefault(x => x.Name == fileName);
                if (d == null)
                {
                    DocumentService.AddDocument(null);
                }
            }
        }

        [HttpGet]
        public ActionResult CustomerUploadMissingFiles(int? productId = null)
        {
            var products = ProductService.FindProductListByCustomerId(UserContext.Company.Id);
            if (products != null && products.Any())
            {
                var productDDL = products
                    .GroupBy(x => (x.PartNumber, x.PartNumberRevision))
                    .Select(g => new SelectListItem
                    {
                        Text = $"Part #: {g.First().PartNumber}, Rev: {g.First().PartNumberRevision}",
                        Value = g.First().Id.ToString(),

                    }).ToList();
                IList<Document> docs = null;
                if (productId != null)
                {
                    docs = HomeBl.GetDocumentsByProductId(productId.Value);
                }
                var model = new CustomerUploadMissingFilesViewModel
                {
                    ProductDDL = new SelectList(productDDL, "Value", "Text", productId),
                    Documents = docs,
                };

                return View(model);
            }
            return View();
        }

        [HttpGet]
        public ActionResult GetDocumentsByProductId(int productId)
        {
            var docs = HomeBl.GetDocumentsByProductId(productId);
            return PartialView("_Documents", docs);
        }

        [HttpGet]
        public ActionResult GetNcrImagesByNcrId(int? ncrId)
        {
            List<NCRImages> ncrimages = new List<NCRImages>();
            var imgs = ncrId != null ? NCRImagesService.FindNCRImagesByNCReportId(ncrId.Value) : ncrimages;
            return PartialView("_NcrFiles", imgs);
        }

        [HttpPost]
        public ActionResult DeleteDoc(int docId, int productId)
        {
            DocumentBL.Delete(docId);
            var docs = HomeBl.GetDocumentsByProductId(productId);
            return PartialView("_Documents", docs);
        }

        [HttpPost]
        public ActionResult DeleteNcrImage(int imageId, int ncrId)
        {
            var entity = NCRImagesService.FindNCRImagesById(imageId);
            if (entity != null)
            {
                NCRImagesService.DeleteNCRImages(entity);
            }
            var imgs = NCRImagesService.FindNCRImagesByNCReportId(ncrId);
            return PartialView("_NcrFiles", imgs);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CustomerUploadMissingFiles(CustomerUploadMissingFilesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMsg"] = "ModelState is invalid";
                return RedirectToAction("CustomerUploadMissingFiles");
            }

            using (var trans = AsyncTransactionScope.StartNew())
            {
                IList<Document> docs = null;
                if (Request.Files == null || Request.Files.Count == 0 || Request.Files[0] == null)
                {
                    TempData["ErrorMessage"] = IndicatingMessages.ForgotUploadFile;
                    return PartialView("_Documents", docs);
                }

                int productId = model.ProductId;
                CUSTOMER_MISSING_DOCUMENT_TYPE docType = model.DocType;
                var td = TaskDataService.FindTaskDataListByProductId(productId).Where(x => x.RFQBidId == null).FirstOrDefault();
                if (td == null)
                {
                    td = TaskDataService.FindTaskDataListByProductId(productId).LastOrDefault(x => x.StateId != (int)States.RFQBidComplete);
                }
                List<HttpPostedFileBase> fileBases = new List<HttpPostedFileBase>();
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    fileBases.Add(Request.Files[i]);
                }
                try
                {
                    int? orderId = null;
                    if (docType == CUSTOMER_MISSING_DOCUMENT_TYPE.PRODUCT_AVATAR)
                    {
                        docs = DocumentBL.UploadMissingFiles(fileBases, docType, td);
                        td.Product.AvatarUri = docs.Last().DocUri;
                        ProductService.UpdateProduct(td.Product);
                    }
                    else if (docType == CUSTOMER_MISSING_DOCUMENT_TYPE.PRODUCT_2D_PDF || docType == CUSTOMER_MISSING_DOCUMENT_TYPE.PRODUCT_3D_STEP)
                    {
                        docs = DocumentBL.UploadMissingFiles(fileBases, docType, td);
                    }
                    else if (td.StateId > (int)States.QuoteAccepted && td.StateId < (int)States.BidForRFQ && td.StateId > (int)States.BidTimeout)
                    {
                        orderId = OrderService.FindOrderByTaskId(td.TaskId).LastOrDefault()?.Id;
                        if (orderId == null)
                        {
                            throw new Exception(IndicatingMessages.OrderNotFound);
                        }
                        docs = DocumentBL.UploadMissingFiles(fileBases, docType, td, orderId);
                    }
                    else
                    {
                        throw new Exception(IndicatingMessages.UploadDocOnInvalidState);
                    }

                }
                //catch (StorageException ex)
                //{
                //    TempData["ErrorMessage"] = ex.RetrieveErrorMessage();
                //    return RedirectToAction("CustomerUploadMissingFiles", new { @productId = productId });
                //}
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex.RetrieveErrorMessage();
                    return RedirectToAction("CustomerUploadMissingFiles", new { @productId = productId });
                }
                trans.Complete();
                return RedirectToAction("CustomerUploadMissingFiles", new { @productId = productId });
            }

        }

        [HttpGet]
        public ActionResult UploadMissingNcrImages(int? ncrId = null)
        {
            var companyType = UserContext.Company.CompanyType;
            var ncrs = companyType == CompanyType.Customer ? NcReportService.FindNCReportByCustomerId(UserContext.Company.Id)
                                                           : NcReportService.FindNCReportByVendorId(UserContext.Company.Id);
            if (ncrs != null && ncrs.Any())
            {
                var ncrDDL = ncrs.ToList()
                    //.GroupBy(x => (x.OrderId, x.VendorId.Value))
                    .Select(g => new SelectListItem
                    {
                        Text = $"Desc: {g.NCDescription}, State: {Enum.GetName(typeof(States), g.StateId)}",
                        Value = g.Id.ToString(),
                    }).ToList();

                IList<NCRImages> ncrImages = null;
                if (ncrId != null)
                {
                    ncrImages = NCRImagesService.FindNCRImagesByNCReportId(ncrId.Value);
                }
                var model = new UploadMissingNcrImagesViewModel
                {
                    NcrDDL = new SelectList(ncrDDL, "Value", "Text", ncrId),
                    NcrImages = ncrImages,
                };

                return View(model);
            }
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadMissingNcrImages(UploadMissingNcrImagesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMsg"] = "ModelState is invalid";
                return RedirectToAction("CustomerUploadMissingFiles");
            }

            using (var trans = AsyncTransactionScope.StartNew())
            {
                IList<NCRImages> imgs = null;
                if (Request.Files == null || Request.Files.Count == 0 || Request.Files[0] == null)
                {
                    TempData["ErrorMessage"] = IndicatingMessages.ForgotUploadFile;
                    return PartialView("_NcrFiles", model);
                }

                NCR_IMAGE_TYPE docType = model.DocType;

                List<HttpPostedFileBase> fileBases = new List<HttpPostedFileBase>();
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    fileBases.Add(Request.Files[i]);
                }
                try
                {
                    int? orderId = null;
                    var ncr = NcReportService.FindNCReportById(model.NcrId);
                    orderId = ncr.OrderId;
                    var td = ncr.TaskId != null ? ncr.Task : TaskDataService.FindTaskDataByProductId(ncr.ProductId);
                    if (td.StateId >= (int)States.NCRCustomerStarted && td.StateId <= (int)States.NCRClosed ||
                        td.StateId == (int)States.NCRCustomerCorrectivePartsAccepted ||
                        td.StateId == (int)States.NCRDamagedByCustomer)
                    {
                        orderId = ncr.OrderId;
                        if (orderId == null)
                        {
                            throw new Exception(IndicatingMessages.OrderNotFound);
                        }
                    }
                    imgs = DocumentBL.UploadMissingNcrImages(fileBases, docType, model.NcrId, orderId.Value);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex.RetrieveErrorMessage();
                    return RedirectToAction("UploadMissingNcrImages", new { @ncrId = model.NcrId });
                }
                trans.Complete();
                return RedirectToAction("UploadMissingNcrImages", new { @ncrId = model.NcrId });
            }

        }


        [AllowAnonymous]
        public ActionResult ExportVendorBill(BillViewModel model)
        {
            return View(model);
        }


        [HttpGet]
        [Authorize(Roles = Omnae.Model.Security.Roles.CompanyAdmin)]
        public ActionResult DemoRestore()
        {
            return View();
        }

        [HttpPost, ActionName("DemoRestore"), ValidateAntiForgeryToken]
        [Authorize(Roles = Omnae.Model.Security.Roles.CompanyAdmin)]
        public ActionResult DemoRestorePost()
        {
            //Send Request to Restore the Database to the Queue.
            var storageConnectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
            var queue = new QueueClient(storageConnectionString, "demo-restore-db");
            queue.CreateIfNotExists();

            var base64Msg = Convert.ToBase64String(Encoding.UTF8.GetBytes("restoreDB"));

            var peekedMessage = queue.PeekMessages()?.Value?.FirstOrDefault();
            if (peekedMessage == null || peekedMessage.MessageText != base64Msg)
            {
                queue.SendMessage(base64Msg);
            }

            return RedirectToAction("Index");
        }

        ////////////////////////////////////////////////////////////////////
        // Private methods
        ////////////////////////////////////////////////////////////////////

        private ActionResult GetAppNow()
        {
            string scope = OidcScopes.Accounting.GetStringValue() + " "
                 //+ OidcScopes.Payment.GetStringValue() + " "
                 + OidcScopes.OpenId.GetStringValue() + " "
                 + OidcScopes.Address.GetStringValue()
                 + " " + OidcScopes.Email.GetStringValue() + " "
                 + OidcScopes.Phone.GetStringValue()
                 + " " + OidcScopes.Profile.GetStringValue();

            string authorizeUrl = GetAuthorizeUrl(scope);
            // perform the redirect here.
            return Redirect(authorizeUrl);
        }

        private string GetAuthorizeUrl(string scope)
        {
            var state = Guid.NewGuid().ToString("N");

            SetTempState(state);

            //Make Authorization request
            var request = new AuthorizeRequest(AppConfig.AuthorizeUrl);

            string url = request.CreateAuthorizeUrl(
               clientId: AppConfig.ClientId,
               responseType: OidcConstants.AuthorizeResponse.Code,
               scope: scope,
               redirectUri: AppConfig.RedirectUrl,
               state: state);

            return url;
        }

        private void SetTempState(string state)
        {
            var tempId = new ClaimsIdentity("TempState");
            tempId.AddClaim(new Claim("state", state));

            AuthenticationManager.SignIn(tempId); //TODO: Check this SignIn - What this is doing.
            Session.Abandon();
        }


        private static string RetrieveDbEntityValidationException(DbEntityValidationException ex)
        {
            return ex.RetrieveDbEntityValidationException();
        }

        private async Task<decimal> GetCurrentExchangeRate(DateTime dat)
        {
            QboApi qboApi = new QboApi(QboTokensService);
            decimal? exchangeRate = await qboApi.GetExchangeRate(dat);
            if (exchangeRate == null)
                throw new Exception("exchange rate is null");
            return exchangeRate.Value;
        }


        private TaskViewModel CreateTaskViewModel(int? taskId, int? orderId)
        {
            if (taskId == null && orderId == null)
                throw new ApplicationException("Invalid IDs");

            TaskData td = TaskDataService.FindByIdWithExtraData(taskId.Value);
            Order order;

            if (orderId != null)
            {
                order = OrderService.FindOrderById(orderId.Value);
            }
            else if (td.Orders.Any())
            {
                order = td.Orders.FirstOrDefault();
            }
            else
            {
                order = OrderService.FindOrdersByProductId(td.ProductId.Value).LastOrDefault();
            }

            DocumentService.UpdateDocUrlWithSecurityToken(td?.Product?.Documents);
            DocumentService.UpdateDocUrlWithSecurityToken(order?.Product?.Documents);

            TaskViewModel tvm = new TaskViewModel
            {
                UserType = UserContext.UserType,
                TaskData = td,
                Order = order,
                EnumName = Omnae.Common.Extensions.StringExtensions.SplitCamelCase(Enum.GetName(typeof(States), td.StateId)),
                //VendorUnitPrice = td.Invoices?.LastOrDefault(x => x.UserType == (int)USER_TYPE.Vendor && x.Quantity > 1)?.UnitPrice ?? 0m,
                //VendorPONumber = td.Invoices?.FirstOrDefault(x => x.UserType == (int)USER_TYPE.Vendor)?.PONumber ?? string.Empty,
                ChkPreconditions = td.StateId == (int)States.ProductionComplete ? TaskSetup.CheckPreconditions(td) : (bool?)null,
                MyFunc = () => TaskSetup.CheckPreconditions(td.ProductId.Value, td.TaskId),
                ChangeRevisionReason = td.Product.ParentPartRevisionId != null ? GetChangePartRevisionReason(td.TaskId) : null,
                ProductDetailsVM = td.Product != null ? SetupProductDetailsVM(td, order?.Id) : null,
                ProductFileVM = td.Product != null ? SetupProductFilesVM(td) : null,
                Docs = td.Product?.Documents.ToList(),
            };

            if ((tvm.UserType == USER_TYPE.Customer || tvm.UserType == USER_TYPE.Vendor) &&
                (td.StateId == (int)States.BackFromRFQ ||
                 td.StateId == (int)States.OutForRFQ ||
                 td.StateId == (int)States.RFQRevision ||
                 td.StateId == (int)States.BidForRFQ))
            {
                tvm.NcrDescriptionVM = TaskSetup.SetupNcrDescriptionViewModel(td);
            }
            else if (td.StateId > (int)States.NCRCustomerStarted && td.StateId < (int)States.NCRClosed ||
                    td.StateId == (int)States.NCRCustomerCorrectivePartsAccepted ||
                    td.StateId == (int)States.NCRDamagedByCustomer)
            {
                tvm.NcrDescriptionVM = TaskSetup.SetupNcrDescriptionViewModel(td);
            }
            else if (td.StateId == (int)States.SampleStarted || td.StateId == (int)States.ProductionStarted)
            {
                tvm.CarrierFromOrder = order.CarrierName;
            }


            if (tvm.UserType == USER_TYPE.Vendor || tvm.UserType == USER_TYPE.Admin)
            {
                if (td.StateId == (int)States.OutForRFQ ||
                    td.StateId == (int)States.RFQRevision ||
                    td.StateId == (int)States.BidForRFQ)
                {
                    tvm.RFQVM = DashboardBL.SetupRFQBidVM(td);
                }

                if (td.StateId == (int)States.AddExtraQuantities)
                {
                    tvm.NcrDescriptionVM = TaskSetup.SetupNcrDescriptionViewModel(td);
                    tvm.RFQVM = DashboardBL.SetupExtraQuantity(td);
                }
                if (td.StateId == (int)States.RFQBidComplete && td.RFQBidId != null)
                {
                    tvm.BidFailedReason = DashboardBL.GetBidFailedReason(td.RFQBidId.Value);
                }

                if (td.RejectReason != null && td.StateId == (int)States.SampleRejected)
                {
                    tvm.SampleRejectDocs = TaskSetup.GetDocumentsByProductIdDocType(td.ProductId.Value, DOCUMENT_TYPE.CORRESPOND_SAMPLE_REJECT_PDF);
                }
            }

            if (td.StateId == (int)States.VendorPendingInvoice && order != null)
            {
                tvm.VendorInvoiceVM = DashboardBL.SetupVendorCreateInvoiceViewModel(td.ProductId.Value, td.TaskId, new List<Order> { order });
            }

            if (td.StateId == (int)States.NCRVendorCorrectivePartsComplete)
            {
                tvm.NCReport = TaskSetup.GetNCReport(td.ProductId.Value, order.Id);
            }
            return tvm;
        }


    }
}
