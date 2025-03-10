using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.Azure;
using Omnae.BlobStorage;
using Omnae.Common;
using Omnae.Context;
using Omnae.Libs.ViewModel;
using Omnae.Model.Models;
using Omnae.Service.Service;
using Omnae.Service.Service.Interfaces;
using Omnae.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Common;
using Libs.Notification;
using Omnae.BusinessLayer;
using Omnae.BusinessLayer.Identity.Model;
using Omnae.BusinessLayer.Models;
using Omnae.BusinessLayer.Services;
using Omnae.Libs.Notification;
using Omnae.Model.Context;

namespace Omnae.Controllers
{
    public class ProductsController : BaseController
    {
        private TaskDataCustomerBL TaskDataCustomerBL { get; }
        private ProductBL ProductBL { get; }
        private TaskSetup TaskSetup { get; }


        public ProductsController(IRFQBidService rfqBidService, ICompanyService companyService, ITaskDataService taskDataService, IPriceBreakService priceBreakService, IOrderService orderService, ILogedUserContext userContext, IProductService productService, IDocumentService documentService, IShippingService shippingService, ICountryService countryService, IAddressService addressService, IStateProvinceService stateProvinceService, IOrderStateTrackingService orderStateTrackingService, IProductStateTrackingService productStateTrackingService, IPartRevisionService partRevisionService, IStoredProcedureService spService, INCReportService ncReportService, IRFQQuantityService rfqQuantityService, IExtraQuantityService extraQuantityService, IBidRequestRevisionService bidRequestRevisionService, ITimerSetupService timerSetupService, IQboTokensService qboTokensService, IOmnaeInvoiceService omnaeInvoiceService, INCRImagesService ncrImagesService, IApprovedCapabilityService approvedCapabilityService, IShippingAccountService shippingAccountService, ApplicationDbContext dbUser, ProductBL productBl, NotificationService notificationService, UserContactService userContactService, TimerTriggerService timerTriggerService, NotificationBL notificationBl, PaymentBL paymentBl, ShipmentBL shipmentBl, ChartBL chartBl, IMapper mapper, NcrBL ncrBL, IDocumentStorageService documentStorageService, IImageStorageService imageStorageService, TaskDataCustomerBL taskDataCustomerBl, TaskSetup taskSetup) : base(rfqBidService, companyService, taskDataService, priceBreakService, orderService, userContext, productService, documentService, shippingService, countryService, addressService, stateProvinceService, orderStateTrackingService, productStateTrackingService, partRevisionService, spService, ncReportService, rfqQuantityService, extraQuantityService, bidRequestRevisionService, timerSetupService, qboTokensService, omnaeInvoiceService, ncrImagesService, approvedCapabilityService, shippingAccountService, dbUser, productBl, notificationService, userContactService, timerTriggerService, notificationBl, paymentBl, shipmentBl, chartBl, mapper, ncrBL, documentStorageService, imageStorageService)
        {
            TaskDataCustomerBL = taskDataCustomerBl;
            ProductBL = productBl;
            TaskSetup = taskSetup;
        }

        // GET: Products
        public ActionResult Index()
        {
            if (UserContext.Company == null)
                return RedirectToAction("Index", "Home"); //TODO: Send the user

            var taskVMList = new List<TaskViewModel>();
            List<TaskData> taskDatas = new List<TaskData>();
            if (UserContext.UserType == USER_TYPE.Customer)
            {
                taskDatas = TaskDataService.FindTaskDataByCustomerId(UserContext.Company.Id)
                                            .Where(x => x.StateId >= (int)States.ProductionStarted && x.StateId < (int)States.NCRCustomerStarted)
                                            .OrderByDescending(t => t.ModifiedUtc)
                                            .GroupBy(p => p.ProductId)
                                            .Select(g => g.First()).ToList();
            }
            else if (UserContext.UserType == USER_TYPE.Vendor)
            {
                taskDatas = TaskDataService.FindTaskDataByVendorId(UserContext.Company.Id)
                                            .Where(x => x.StateId >= (int)States.ProductionStarted && x.StateId < (int)States.NCRCustomerStarted)
                                            .GroupBy(x => x.ProductId)
                                            .Select(y => y.First())
                                            .ToList();
            }

            foreach (var td in taskDatas)
            {
                Order order = td.Orders?.LastOrDefault();
                if (order == null)
                {
                    order = OrderService.FindOrdersByProductId(td.ProductId.Value).LastOrDefault();
                }
                OmnaeInvoiceService.UpdateDocUrlWithSecurityToken(td.Invoices);
                TaskViewModel tvm = new TaskViewModel
                {
                    TaskData = td,
                    Order = order,
                    EnumName = Omnae.Common.Extensions.StringExtensions.SplitCamelCase(Enum.GetName(typeof(States), td.StateId)),
                    ProductUnitPrice = td.Product.PriceBreak?.UnitPrice, 
                    VendorUnitPrice = td.Invoices?.LastOrDefault(x => x.UserType == (int)USER_TYPE.Vendor && x.Quantity > 1)?.UnitPrice ?? 0m,
                    VendorPONumber = td.Invoices?.LastOrDefault(x => x.UserType == (int)USER_TYPE.Vendor)?.PONumber ?? null,
                    VendorPODocUri = td.Invoices?.LastOrDefault(x => x.UserType == (int)USER_TYPE.Vendor)?.PODocUri ?? null,
                };
                if (tvm.VendorPONumber == null)
                {
                    tvm.VendorPONumber = OmnaeInvoiceService.FindOmnaeInvoiceByProductId(td.ProductId.Value).FirstOrDefault(x => x.UserType == (int)USER_TYPE.Vendor)?.PONumber ?? string.Empty;
                }
                if (td.StateId == (int)States.ProductionComplete)
                {
                    tvm.ChkPreconditions = TaskSetup.CheckPreconditions(td);
                }
                taskVMList.Add(tvm);
            }
            if (UserContext.UserType == USER_TYPE.Vendor)
            {
                return View("DisplayVendorProducts", taskVMList);
            }
            return View(taskVMList);
        }

        // GET: Products
        public ActionResult NewOrderList()
        {
            var prodList = ProductBL.NewOrderList();

            TempData["UserType"] = UserContext.UserType;
            return View("_Products_NewOrder", prodList);
        }

        // GET: Products
        public ActionResult ReOrderList()
        {
            var prodList = ProductBL.ReOrderList();

            TempData["UserType"] = UserContext.UserType;
            return View("_Products_Reorder", prodList);
        }


        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                HandleErrorInfo info = new HandleErrorInfo(new Exception(IndicatingMessages.ParameterIsNull), "Products", "Details");
                return View("Error", info);
            }
            Product product = ProductService.FindProductById(id.Value);
            if (product == null)
            {
                HandleErrorInfo info = new HandleErrorInfo(new Exception(IndicatingMessages.ProductNotFound), "Products", "Details");
                return View("Error", info);
            }
            if (!ProductService.IsUsersProduct(product, UserContext, CompanyService))
            {
                HandleErrorInfo info = new HandleErrorInfo(new Exception(IndicatingMessages.InvalidAccess), "Products", "Details");
                return View("Error", info);
            }
            //return PartialView("_Details", product);
            return View(product);
        }



        [HttpGet]
        public ActionResult CheckPlaceOrderType(bool chx)
        {
            PlaceOrderViewModel model = Session["PlaceOrderViewModel"] as PlaceOrderViewModel;
            model.IsForOrderTooling = chx;
            int productId = model.Product != null ? model.Product.Id : 0;
            if (chx == true)
            {
                TaskData td = TaskDataService.FindById(model.TaskId);
                var pb = PriceBreakService.FindPriceBreakByTaskId(td.TaskId).FirstOrDefault();
                
                if (pb != null)
                {
                    model.ToolingCharges = pb.CustomerToolingSetupCharges;
                    model.Total = pb.CustomerToolingSetupCharges;
                    model.NumberSampleIncluded = pb.NumberSampleIncluded != null ? pb.NumberSampleIncluded.Value : 1;
                }
                return PartialView("_PlaceTooling", model);
            }
            else
            {
                return PartialView("_PlaceOrder", model);
            }
        }

        [HttpGet]
        // GET: Products/PlaceOrder 
        // Override Products/PlaceOder
        public async Task<ActionResult> PlaceOrder(int? Id, int? revisionId = null)
        {
            return await StartOrder(Id, false, revisionId);
        }

        // GET: Products/PlaceOrder
        public async Task<ActionResult> ReOrder(int? Id, int? revisionId = null)
        {
            return await StartOrder(Id, true, revisionId);
        }

        private async Task<ActionResult> StartOrder(int? Id, bool isReorder, int? revisionId = null)
        {
            try
            {
                var model = await ProductBL.StartOrderAsync(Id, isReorder, revisionId);

                var entity = DocumentService.FindDocumentByProductId(model.ProductId).Where(d => d.DocType == (int)DOCUMENT_TYPE.QUOTE_PDF).ToList();
                if (entity?.Any() == true)
                {
                    Document doc = entity.LastOrDefault();
                    WebClient webClient = new WebClient();
                    try
                    {
                        ViewData["PDF"] = webClient.DownloadData(doc.DocUri);
                    }
                    catch (Exception ex)
                    {
                        //HandleAzureExceptions(ex, $"{doc.DocUri} could not be found in AZURE");
                        TempData["AzureException"] = ex.RetrieveErrorMessage() + $" {doc.DocUri} could not be found in AZURE";
                    }
                }

                if (TempData["AzureException"] == null)
                {
                    TempData["AzureException"] = null;
                }

                Session["PlaceOrderViewModel"] = model;

                return View("PlaceOrder", model);
            }
            catch (BLException e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Index", "Home");
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = ex.RetrieveErrorMessage();
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Products/OrderDetails/5
        public ActionResult OrderDetails(int? id)
        {
            if (id == null)
            {
                HandleErrorInfo info = new HandleErrorInfo(new Exception(IndicatingMessages.ParameterIsNull), "Products", "OrderDetails");
                return View("Error", info);
            }
            Product product = ProductService.FindProductById(id.Value);
            if (product == null)
            {
                HandleErrorInfo info = new HandleErrorInfo(new Exception(IndicatingMessages.ProductNotFound), "Products", "OrderDetails");
                return View("Error", info);
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            var list = new List<SelectListItem>();
            for (var i = 0; i < CustomerBidPreference.Preferences.Length; i++)
            {
                var item = new SelectListItem() { Value = i.ToString(), Text = CustomerBidPreference.Preferences[i] };
                list.Add(item);
            }

            int length = int.Parse(ConfigurationManager.AppSettings["QuantityLength"]);
            decimal?[] quatities = new decimal?[length];

            var model = new ProductViewModel()
            {
                CustomerPriorityDdl = new SelectList(list, "Value", "Text"),
                QuantityList = new List<decimal?>(quatities),
            };

            return View(model);
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProductViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Create", "Products");

            try
            {               
                Product product = Mapper.Map<Product>(model);
                product.CustomerId = UserContext.Company.Id;
                var result = await ProductBL.CreateAsync(model, product);

                TempData["Warning"] = string.IsNullOrWhiteSpace(result.WarningMsg) ? null : result.WarningMsg;                
                return RedirectToAction("Create", "Documents", new {@id = result.PartResult.NewProductId, @taskId = result.PartResult.NewTaskId});
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.RetrieveErrorMessage();
                return RedirectToAction("Create", "Products");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignRFQToVendors(AssignRFQToVendorsViewModel model)
        {
            if (ModelState.IsValid)
            {
                Product product = ProductService.FindProductById(model.ProductId);
                int taskId = 0;
                TaskData taskData = null;
                bool selected = false;
                int length = model.VendorIds.Length > 5 ? 5 : model.VendorIds.Length;
                for (int i = 0; i < length; i++)
                {
                    if (model.isChosen[i] == true)
                    {
                        var vid = model.VendorIds[i];
                        var rfqbid = RfqBidService.FindRFQBidByVendorIdProductId(vid, model.ProductId);
                        if (rfqbid != null)
                        {
                            selected = true;
                            continue;
                        }

                        selected = false;
                        DateTime utcNow = DateTime.UtcNow;
                        RFQBid bid = new RFQBid()
                        {
                            ProductId = model.ProductId,
                            VendorId = vid,
                            RFQQuantityId = product.RFQQuantityId,
                            BidDatetime = utcNow,
                            BidFailedReason = null,
                            CreatedByUserId = UserContext.UserId,
                        };

                        int bidId = RfqBidService.AddRFQBid(bid);

                        // Add bid to RFQBid table for each Vendor
                        // Create a new TaskData for each Vendor to bid RFQ, leave the VendorId unset,
                        // Once a vendor win the bid that: 
                        // 1. set this product.vendor to the winning vendor
                        // 2. then set this productId to the product TaskData's ProductId

                        taskData = new TaskData()
                        {
                            StateId = (int)States.BidForRFQ,
                            ProductId = model.ProductId,
                            RFQBidId = bidId,
                            CreatedUtc = utcNow,
                            ModifiedUtc = utcNow,
                            UpdatedBy = User.Identity.Name,
                            CreatedByUserId = UserContext.UserId,
                            isEnterprise = model.isEnterprise,
                        };
                        taskId = TaskDataService.AddTaskData(taskData);


                        // Notify
                        var company = CompanyService.FindCompanyById(vid);
                        var users = company.Users.Where(u => u.Active == true);
                        foreach (var user in users)
                        {
                            var destination = user.Email;
                            var destinationSms = user.PhoneNumber;

                            string subject = string.Format("Notify you the RFQ {0}, {1} is ready for bid for the vendor: {2}", product.PartNumber, product.Description, destination);
                            string url = ConfigurationManager.AppSettings["URL"];
                            var entity = new BaseViewModel
                            {
                                UserName = destination,
                                PartNumber = product.PartNumber,
                                Description = product.Description,
                                Url = url
                            };
                            try
                            {
                                NotificationService.NotifyStateChange((States)taskData.StateId, subject, destination, destinationSms, entity);
                            }
                            catch (Exception ex)
                            {
                                string error = ex.RetrieveErrorMessage();
                                if (error.Equals(IndicatingMessages.SmsWarningMsg) || error.Equals(IndicatingMessages.NoCellPhoneWarningMsg))
                                {
                                    continue;
                                }

                                throw;
                            }
                        }

                    }

                }
                if (selected == false && taskData != null)
                {
                    return RedirectToAction("SetupTimer", "TaskDatas", new { @productId = model.ProductId });
                }
            }
            TempData["ErrorMessage"] = "Couldn't find vendors.";
            return RedirectToAction("Create");
        }

        public ActionResult FindDocuments(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = IndicatingMessages.ParameterIsNull;
                return RedirectToAction("Index", "Home");
            }
            TaskData td = TaskDataService.FindById(id.Value);
            if (td == null)
            {
                TempData["ErrorMessage"] = IndicatingMessages.TaskNotFound;
                return RedirectToAction("Index", "Home");
            }
            int quoteDocId = -1;
            RFQBid bid = null;
            if (td.RFQBidId != null)
            {
                bid = td.RFQBid;

                if (bid != null)
                {
                    quoteDocId = bid.QuoteDocId ?? -1;
                }
            }
            if (td.ProductId != null)
            {
                Product product = td.Product;

                if (product == null)
                {
                    TempData["ErrorMessage"] = IndicatingMessages.ProductNotFound;
                    return RedirectToAction("Index", "Home");

                }
                if (!ProductService.IsUsersProduct(product, UserContext, CompanyService, bid))
                {
                    TempData["ErrorMessage"] = IndicatingMessages.InvalidAccess;
                    return RedirectToAction("Index", "Home");
                }

                DocumentService.UpdateDocUrlWithSecurityToken(product?.Documents);

                List<Document> finalDocs = new List<Document>();
                foreach (var doc in product.Documents)
                {
                    if (doc.DocType != (int)DOCUMENT_TYPE.QUOTE_PDF || doc.DocType == (int)DOCUMENT_TYPE.QUOTE_PDF && doc.Id == quoteDocId)
                    {
                        finalDocs.Add(doc);
                    }
                }
                if (finalDocs.Count > 0)
                {

                    return View(finalDocs);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            Product product = ProductService.FindProductById(id.Value);

            ProductViewModel model = mapper.Map<ProductViewModel>(product);

            if (product == null)
                return HttpNotFound();
            
            //ViewBag.PriceBreakId = new SelectList(db.PriceBreaks, "Id", "Id", product.PriceBreakId);
            TempData["avatarUri"] = model.AvatarUri;
            return View(model);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductViewModel model)
        {
            if (!ModelState.IsValid) 
                return View(model);
            
            if (model.AvatarUri == null)
            {
                model.AvatarUri = (string)TempData["avatarUri"];
            }

            Product product = mapper.Map<Product>(model);

            var company = CompanyService.FindCompanyByUserId(User.Identity.GetUserId());
            product.CustomerId = company.Id;

            ProductService.UpdateProduct(product);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult MaterialDetails(int id)
        {
            var product = ProductService.FindProductById(id);
            return View("MaterialDetails", product);
        }

        [HttpGet]
        public async Task<ActionResult> Calculate(int? Id, int? TaskId, decimal? Quantity, bool? isReorder)
        {
            try
            {
                var model = await ProductBL.CalculateAsync(Id, TaskId, Quantity, isReorder);
                return PartialView("_CalculateResult", model);
            }
            catch (Exception e)
            {
                ViewBag.Msg = e.RetrieveErrorMessage();
                return PartialView("_CalculateResult", null);
            }
        }

        [HttpGet]
        public ActionResult CreatePartRevision(int? prodId)
        {
            if (prodId == null)
            {
                HandleErrorInfo info = new HandleErrorInfo(new Exception(IndicatingMessages.ParameterIsNull), "Products", "CreatePartRevision");
                return View("Error", info);
            }
            var prod = GetCustomerProductById(prodId.Value);
            if (prod == null)
            {
                HandleErrorInfo info = new HandleErrorInfo(new Exception(IndicatingMessages.InvalidAccess), "Products", "CreatePartRevision");
                return View("Error", info);
            }
            var model = new CreatePartRevisionViewModel
            {
                ProductId = prod.Id,
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePartRevision(CreatePartRevisionViewModel model)
        {
            if (!ModelState.IsValid) 
                return View();

            try
            {
                var result = ProductBL.CreatePartRevision(model);
                
                // Documents related to this product
                if (Request.Files == null || Request.Files.Count == 0 || Request.Files[0] == null)
                {
                    HandleErrorInfo info = new HandleErrorInfo(new Exception("Please select a file to upload!"), "Products", "CreatePartRevision");
                    return View("Error", info);
                }
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase postedFile;
                    if (model.NewAvatar != null && i == 0)
                    {
                        continue;
                    }
                    postedFile = Request.Files[i];
                    if (!string.IsNullOrEmpty(postedFile.FileName))
                    {
                        string ext = Path.GetExtension(postedFile.FileName);

                        DOCUMENT_TYPE docType = DOCUMENT_TYPE.PRODUCT_2D_PDF;
                        if (ext.ToUpper() == ".STEP" || ext.ToUpper() == ".DXF")
                        {
                            docType = DOCUMENT_TYPE.PRODUCT_3D_STEP;
                        }
                        else if (ext.ToUpper() != ".PDF" && ext.ToUpper() != ".AI" && ext.ToUpper() != ".EPS")
                        {
                            throw new BLException("Wrong file format, please use *.STEP or *.XDF for 3D file and *.PDF, *.AI or *.EPS for 2D files");
                        }
                        string fileNewName = $"prod_{result.NewProductId}_{model.PartRevision}_{i + 1}{ext}";
                        var docUri = DocumentStorageService.Upload(Request.Files[i], fileNewName);

                        var doc = new Document()
                        {
                            TaskId = result.NewTaskId,
                            Version = 1,
                            Name = fileNewName,
                            DocType = (int)docType,
                            DocUri = docUri,
                            ProductId = result.NewProductId,
                            UpdatedBy = UserContext.User.UserName,
                            CreatedUtc = DateTime.UtcNow,
                            ModifiedUtc = DateTime.UtcNow,
                            CreatedByUserId = UserContext.UserId,
                            ModifiedByUserId = UserContext.UserId,
                        };

                        try
                        {
                            DocumentService.AddDocument(doc);
                        }
                        catch (Exception ex)
                        {
                            throw new BLException($"Tried to upload same document twice! Duplicating document name: {doc.Name}. Exception: {ex.RetrieveErrorMessage()}");
                        }
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            catch (BLException e)
            {
                TempData["ErrorMessage"] = e.RetrieveErrorMessage();
                return RedirectToAction("CreatePartRevision", "Products", new { @prodId = model.ProductId });
            }
        }

        [HttpGet]
        public ActionResult ChooseOrderForNCR(int? id, int taskId)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = IndicatingMessages.ParameterIsNull;
                return RedirectToAction("Index", "Home");
            }
            var orders = OrderService.FindOrdersByProductId(id.Value).Where(x => x.Quantity > 1).ToList();
            if (orders.Count == 0)
            {
                TempData["ErrorMessage"] = IndicatingMessages.PartHasNotBeenOrderedYet;
                return RedirectToAction("Index", "Home");
            }
            OrderViewModel model = new OrderViewModel();
            //var config = new MapperConfiguration(cfg =>
            //{
            //    cfg.CreateMap<Order, OrderViewModel>();
            //});

            //var mapper = config.CreateMapper();
            List<OrderViewModel> orderList = mapper.Map<List<OrderViewModel>>(orders);
            Product product = ProductService.FindProductById(id.Value);
            orderList = orderList.Select(x => { x.PartNumber = product.PartNumber; x.TaskId = taskId; return x; }).ToList();

            return View(orderList);
        }

        [HttpGet]
        public ActionResult CreateNCR(int? productId, int? orderId, int taskId)
        {
            if (productId == null || orderId == null || taskId == 0)
            {
                HandleErrorInfo info = new HandleErrorInfo(new Exception(IndicatingMessages.ParameterIsNull), "Products", "CreateNCR");
                return View("Error", info);
            }
            var prod = GetCustomerProductById(productId.Value);
            var order = GetCustomerOrderByOrderId(orderId.Value);
            if (prod == null || order == null)
            {
                HandleErrorInfo info = new HandleErrorInfo(new Exception(IndicatingMessages.InvalidAccess), "Products", "CreateNCR");
                return View("Error", info);
            }

            var model = new NcrDescriptionViewModel()
            {
                ProductId = productId.Value,
                TaskId = taskId,
                OrderId = order.Id,
                StateId = States.NCRVendorRootCauseAnalysis,
                CustomerId = prod.CustomerCompany.Id,
                VendorId = prod.VendorCompany.Id,
                Customer = prod.CustomerCompany.Name,
                Vendor = prod.VendorCompany.Name,
                ProductName = prod.Name,
                ProductAvatarUri = prod.AvatarUri,
                ProductPartNo = prod.PartNumber,
                PartRevisionNo = prod.PartNumberRevision,
                ProductDescription = prod.Description,
                NCDetectedby = DETECTED_BY.CUSTOMER,
                NCDetectedDate = DateTime.UtcNow,
                PONumber = order.CustomerPONumber,
                NCRNumber = NcReportService.FindNCReportsYearlySequence(prod.CustomerId.Value),
                NCRNumberForVendor = NcReportService.FindNCReportsYearlySequenceForVendor(prod.VendorId.Value),
                TotalProductQuantity = order.Quantity,
            };

            return View(model);
        }
        
        [HttpGet]
        public ActionResult SetupWholesales(int? id)
        {
            if (id == null)
            {
                HandleErrorInfo info = new HandleErrorInfo(new Exception(IndicatingMessages.ParameterIsNull), "Products", "SetupWholesales");
                return View("Error", info);
            }
            var product = ProductService.FindProductById(id.Value);
            if (!ProductService.IsUsersProduct(product, UserContext, CompanyService))
            {
                HandleErrorInfo info = new HandleErrorInfo(new Exception(IndicatingMessages.InvalidAccess), "Products", "SetupWholesales");
                return View("Error", info);
            }

            // find all revisions related to this product
            var revisions = PartRevisionService.FindPartRevisionByProductId(id.Value);

            SetupWholesalesViewModel model = new SetupWholesalesViewModel()
            {
                productId = id.Value,
                PartRevisionList = new SelectList(revisions, "Id", "Name")
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetupWholesales(SetupWholesalesViewModel model)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageMarkUp(int? id, int? revisionId)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TurnOnOffWholesales(int? id, int? revisionId)
        {
            return View();
        }

        [HttpGet]
        public ActionResult NcrHistory(int? id)
        {
            if (id == null)
            {
                HandleErrorInfo info = new HandleErrorInfo(new Exception(IndicatingMessages.ParameterIsNull), "Products", "NcrHistory");
                return View("Error", info);
            }
            NCRViewModel model = new NCRViewModel();
            var orders = OrderService.FindOrdersByProductId(id.Value);
            var ncrs = NcReportService.FindNCReportsByProductId(id.Value);
            model.NcrInfoList = ncrs.Select(g => new NcrInfoViewModel
            {
                Id = g.Id,
                NCRNumber = g.NCRNumber,
                DateInitiated = g.NCDetectedDate,
                RootCause = g.RootCause != null ? Enum.GetName(typeof(NC_ROOT_CAUSE), g.RootCause) : null,
                Vendor = ProductService.FindProductById(g.ProductId).Name,
                Cost = g.Cost,
                DateClosed = g.DateNcrClosed
            }).ToList();

            return View(model);
        }

        [HttpGet]
        public ActionResult AddQuantity(int? id, int? taskId)
        {
            if (id == null || taskId == null)
            {
                HandleErrorInfo info = new HandleErrorInfo(new Exception(IndicatingMessages.ParameterIsNull), "Products", "AddQuantity");
                return View("Error", info);
            }
            Product product = GetCustomerProductById(id.Value);

            List<SelectListItem> list = new List<SelectListItem>();
            for (int i = 0; i < CustomerBidPreference.Preferences.Length; i++)
            {
                SelectListItem item = new SelectListItem() { Value = i.ToString(), Text = CustomerBidPreference.Preferences[i] };
                list.Add(item);
            }

            int length = int.Parse(ConfigurationManager.AppSettings["QuantityLength"]);
            int?[] quatities = new int?[length];
            TaskData taskData = TaskDataService.FindById(taskId.Value);

            AddQuantityViewModel model = new AddQuantityViewModel()
            {
                ProductId = id.Value,
                Product = product,
                task = taskData,
                QuantityList = new List<int?>(quatities),
                ProductDetailsVM = ProductBl.SetupProductDetailsVM(taskData),
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult AssignRFQToVendors(int id)
        {
            var model = ProductBL.AssignRFQToValidVendors(id);
            return View(model);
        }

        //[HttpPost]
        //public ActionResult AssignRFQToValidVendors(int pid)
        //{
        //    var model = ProductBL.AssignRFQToValidVendors(pid);
        //    return View(model);
        //}


        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddQuantity(AddQuantityViewModel model)
        {          
            if (ModelState.IsValid)
            {
                TaskData td = TaskDataService.FindById(model.TaskId);
                if (td == null)
                {
                    TempData["ErrorMessage"] = IndicatingMessages.TaskNotFound;
                    return RedirectToAction("Index", "Home");
                }
                if (td.StateId != (int)States.QuoteAccepted && td.StateId != (int)States.ProductionComplete)
                {
                    TempData["Warning"] = "Add Quantity function is not available at current state: " + Enum.GetName(typeof(States), td.StateId);
                    return RedirectToAction("Index", "Home");
                }

                Product product = GetCustomerProductById(model.ProductId);
                int taskId = model.TaskId;

                // Create RFQQuantity table based on user inputs
                var qtys = model.QuantityList.Where(x => x != null).Select(q => q.Value).ToList();
                ExtraQuantity extraQty = null;
                if (product == null)
                {
                    TempData["ErrorMessage"] = IndicatingMessages.ProductNotFound;
                    return RedirectToAction("Index", "Home");
                }

                extraQty = new ExtraQuantity();
                for (int i = 0; i < qtys.Count; i++)
                {
                    if (i == 0 && qtys[i] > 0)
                    {
                        extraQty.Qty1 = qtys[i];
                    }
                    if (i == 1 && qtys[i] > 0)
                    {
                        extraQty.Qty2 = qtys[i];
                    }
                    if (i == 2 && qtys[i] > 0)
                    {
                        extraQty.Qty3 = qtys[i];
                    }
                    if (i == 3 && qtys[i] > 0)
                    {
                        extraQty.Qty4 = qtys[i];
                    }
                    if (i == 4 && qtys[i] > 0)
                    {
                        extraQty.Qty5 = qtys[i];
                    }
                    if (i == 5 && qtys[i] > 0)
                    {
                        extraQty.Qty6 = qtys[i];
                    }
                    if (i == 6 && qtys[i] > 0)
                    {
                        extraQty.Qty7 = qtys[i];
                    }
                }


                try
                {
                    product.ExtraQuantityId = ExtraQuantityService.AddExtraQuantity(extraQty);
                    ProductService.UpdateProduct(product);

                    td.StateId = (int)States.AddExtraQuantities;
                    td.ModifiedUtc = DateTime.UtcNow;
                    td.UpdatedBy = User.Identity.Name;
                    td.ModifiedByUserId = UserContext.UserId;

                    TaskDataService.Update(td);

                    // set this task id to PartRevision
                    if (product.PartRevisionId != null)
                    {
                        var partRevision = PartRevisionService.FindPartRevisionById(product.PartRevisionId.Value);
                        partRevision.TaskId = taskId;
                        partRevision.StateId = (States)td.StateId;
                        partRevision.ModifiedByUserId = UserContext.UserId;

                        PartRevisionService.UpdatePartRevision(partRevision);

                        // Fill in ProductStateTracking table with product id, state id ...
                        ProductStateTracking productState = new ProductStateTracking()
                        {
                            ProductId = product.Id,
                            StateId = td.StateId,
                            UpdatedBy = td.UpdatedBy,
                            ModifiedUtc = td.ModifiedUtc,
                            CreatedByUserId = UserContext.UserId,
                            ModifiedByUserId = UserContext.UserId,
                        };
                        ProductStateTrackingService.AddProductStateTracking(productState);

                        // Notify Vendor with email and sms
                        if (product.VendorId != null)
                        {
                            string subject = $"Added Extra Quantities for {product.PartNumber}, {product.Description}";

                            var users = product.VendorCompany.Users.Where(u => u.Active == true);
                            foreach (var user in users)
                            {
                                var entity = new CreateNewPartViewModel()
                                {
                                    UserName = user.Email,
                                    PartNumber = product.PartNumber,
                                    Description = product.Description,
                                };
                                try
                                {
                                    await SendNotifications(td, user.Email, user.PhoneNumber);
                                    TempData["Warning"] = null;
                                }
                                catch (Exception ex)
                                {
                                    string errorMsg = ex.RetrieveErrorMessage();
                                    if (errorMsg.Equals(IndicatingMessages.SmsWarningMsg))
                                    {
                                        TempData["Warning"] = errorMsg;
                                    }
                                    else
                                    {
                                        TempData["ErrorMessage"] = errorMsg;
                                    }
                                }
                            }

                        }                      
                    }
                }
                catch (DbEntityValidationException dbEx)
                {
                    TempData["ErrorMessage"] = dbEx.RetrieveErrorMessage();
                }               
            }
            return RedirectToAction("Index", "Home");
        }

        private Product GetCustomerProductById(int productId)
        {
            var currentUserId = User.Identity.GetUserId();
            var company = CompanyService.FindCompanyByUserId(currentUserId);
            if (company == null)
                return null;

            var prod = ProductService.FindProductListByCustomerId(company.Id).FirstOrDefault(x => x.Id == productId);
            DocumentService.UpdateDocUrlWithSecurityToken(prod?.Documents);

            return prod;
        }

        //private Order GetCustomerOrderByOrderId(int orderId)
        //{
        //    return OrderService.FindOrderById(orderId);
        //}

        //[NonAction] //Used in Views
        //public ProductDetailsViewModel SetupProductDetailsVM(TaskData td)
        //{
        //    return ProductBl.SetupProductDetailsVM(td);
        //}

       
    }
}
