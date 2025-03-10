using AutoMapper;
using Common;
using Libs.Notification;
using Microsoft.AspNet.Identity;
using Omnae.BlobStorage;
using Omnae.BusinessLayer;
using Omnae.BusinessLayer.Identity.Model;
using Omnae.BusinessLayer.Models;
using Omnae.BusinessLayer.Services;
using Omnae.Common;
using Omnae.Context;
using Omnae.Data;
using Omnae.Libs.ViewModel;
using Omnae.Model.Context;
using Omnae.Model.Models;
using Omnae.Model.Models.Aspnet;
using Omnae.Service.Service;
using Omnae.Service.Service.Interfaces;
using Omnae.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Omnae.Libs.Notification;
using Serilog;

namespace Omnae.Controllers
{
    public class TaskDatasController : BaseController
    {
        private TaskDatasBL TaskDatasBL { get; }

        private ILogger Log { get; }

        public TaskDatasController(IRFQBidService rfqBidService, ICompanyService companyService, ITaskDataService taskDataService, IPriceBreakService priceBreakService, IOrderService orderService, ILogedUserContext userContext, IProductService productService, IDocumentService documentService, IShippingService shippingService, ICountryService countryService, IAddressService addressService, IStateProvinceService stateProvinceService, IOrderStateTrackingService orderStateTrackingService, IProductStateTrackingService productStateTrackingService, IPartRevisionService partRevisionService, IStoredProcedureService spService, INCReportService ncReportService, IRFQQuantityService rfqQuantityService, IExtraQuantityService extraQuantityService, IBidRequestRevisionService bidRequestRevisionService, ITimerSetupService timerSetupService, IQboTokensService qboTokensService, IOmnaeInvoiceService omnaeInvoiceService, INCRImagesService ncrImagesService, IApprovedCapabilityService approvedCapabilityService, IShippingAccountService shippingAccountService, ApplicationDbContext dbUser, ProductBL productBl, NotificationService notificationService, UserContactService userContactService, TimerTriggerService timerTriggerService, NotificationBL notificationBl, PaymentBL paymentBl, ShipmentBL shipmentBl, ChartBL chartBl, IMapper mapper, NcrBL ncrBL, IDocumentStorageService documentStorageService, IImageStorageService imageStorageService, TaskDatasBL taskDatasBl, ILogger log) : base(rfqBidService, companyService, taskDataService, priceBreakService, orderService, userContext, productService, documentService, shippingService, countryService, addressService, stateProvinceService, orderStateTrackingService, productStateTrackingService, partRevisionService, spService, ncReportService, rfqQuantityService, extraQuantityService, bidRequestRevisionService, timerSetupService, qboTokensService, omnaeInvoiceService, ncrImagesService, approvedCapabilityService, shippingAccountService, dbUser, productBl, notificationService, userContactService, timerTriggerService, notificationBl, paymentBl, shipmentBl, chartBl, mapper, ncrBL, documentStorageService, imageStorageService)
        {
            this.TaskDatasBL = taskDatasBl;
            Log = log;
        }

        // GET: TaskDatas
        public ActionResult Index()
        {
            var company = UserContext.Company;
            var userType = UserContext.UserType;

            var taskDatas = userType == USER_TYPE.Customer
                                        ? TaskDataService.FindTaskDataByCustomerId(company.Id)
                                        : TaskDataService.FindTaskDataByVendorId(company.Id);

            return View(taskDatas);
        }

        // GET: TaskDatas/Details/5
        public ActionResult Details(int id)
        {
            var taskData = TaskDataService.FindById(id);
            return taskData == null ? (ActionResult)HttpNotFound() : View(taskData);
        }

        // GET: TaskDatas/Create
        public ActionResult Create()
        {
            var currentUserId = User.Identity.GetUserId();
            var company = CompanyService.FindCompanyByUserId(currentUserId);
            var products = ProductService.FindProductListByCompanyId(company.Id);
            ViewBag.Products = new SelectList(products, "Id", "Name");
            return View();
        }

        // POST: TaskDatas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TaskDataViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var taskData = mapper.Map<TaskData>(model);
            taskData.UpdatedBy = User.Identity.Name;
            taskData.CreatedUtc = taskData.ModifiedUtc = DateTime.UtcNow;
            taskData.CreatedByUserId = UserContext.UserId;
            taskData.ModifiedByUserId = UserContext.UserId;

            TaskDataService.AddTaskData(taskData);

            return RedirectToAction("Index");
        }

        // GET: TaskDatas/UpdateState/5
        public ActionResult UpdateState(int? id)
        {
            if (id == null)
            {
                HandleErrorInfo info = new HandleErrorInfo(new Exception("TaskId is null"), "TaskDatas", "UpdateState");
                return View("Error", info);
            }
            TaskData taskData = TaskDataService.FindById(id.Value);
            if (taskData == null)
            {
                HandleErrorInfo info = new HandleErrorInfo(new Exception("TaskData is null"), "TaskDatas", "UpdateState");
                return View("Error", info);
            }
            TempData["TaskData"] = taskData;

            TaskDataViewModel model = new TaskDataViewModel
            {
                StateId = (States)taskData.StateId,
                VendorId = taskData.Product.VendorId.Value,
                TaskId = taskData.TaskId
            };
            return View(model);
        }

        // POST: TaskDatas/UpdateState/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateState(TaskDataViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var taskData = (TaskData)TempData["TaskData"];
            taskData.StateId = (int)model.StateId;
            taskData.UpdatedBy = User.Identity.Name;
            taskData.ModifiedUtc = DateTime.UtcNow;
            taskData.ModifiedByUserId = UserContext.UserId;

            TaskDataService.Update(taskData);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public bool UpdateTag(int? taskId)
        {
            if (!ModelState.IsValid)
            {
                return false;
            }

            var taskData = TaskDataService.FindById(taskId.Value);
            taskData.isTagged = taskData.isTagged == null ? true : !taskData.isTagged;
            TaskDataService.Update(taskData);
            return true;
        }


        // GET: TaskDatas/Edit/5
        public ActionResult PopulateRFQToVendors(int? id)
        {
            if (id == null)
            {
                HandleErrorInfo info = new HandleErrorInfo(new Exception(IndicatingMessages.ParameterIsNull), "TaskDatas", "PopulateRFQToVendors");
                return View("Error", info);
            }

            TaskData taskData = TaskDataService.FindById(id.Value);
            if (taskData == null)
            {
                HandleErrorInfo info = new HandleErrorInfo(new Exception(IndicatingMessages.TaskNotFound), "TaskDatas", "PopulateRFQToVendors");
                return View("Error", info);
            }

            TempData["TaskData"] = taskData;
            Product product = null;
            if (taskData.ProductId != null)
            {
                product = ProductService.FindProductById(taskData.ProductId.Value);
            }
            else
            {
                HandleErrorInfo info = new HandleErrorInfo(new Exception(IndicatingMessages.ProductNotFound), "TaskDatas", "PopulateRFQToVendors");
                return View("Error", info);
            }

            var vendors = DbUser.Users.Where(u => u.UserType == USER_TYPE.Vendor).OrderBy(x => x.UserName);

            var model = new PopulateRFQToVendorsViewModel
            {
                ProductId = product.Id,
                isEnterprise = taskData.isEnterprise,
            };

            ViewBag.VendorIdList = new MultiSelectList(GetUserIds(vendors), "CompanyId", "UserName");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PopulateRFQToVendors(PopulateRFQToVendorsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Home");
            }

            Product product = ProductService.FindProductById(model.ProductId);
            TaskData taskData = null;
            bool selected = false;

            foreach (var vid in model.VendorIds)
            {
                try
                {
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


                    // Create a new TaskData for each Vendor to bid RFQ, leave the ProductId unset,
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
                        ModifiedByUserId = UserContext.UserId,
                        isEnterprise = model.isEnterprise,
                    };
                    TaskDataService.AddTaskData(taskData);

                    // Notify
                    var company = CompanyService.FindCompanyById(vid);
                    var users = company.Users.Where(u => u.Active == true);
                    foreach (var user in users)
                    {
                        var destination = user.Email;
                        var destinationSms = user.PhoneNumber;

                        string subject = $"Notify you the RFQ {product.PartNumber}, {product.Description} is ready for bid for the vendor: {destination}";
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
                            string msg = ex.RetrieveErrorMessage();
                            if (msg.Equals(IndicatingMessages.SmsWarningMsg) || msg.Equals(IndicatingMessages.NoCellPhoneWarningMsg))
                            {
                                continue;
                            }
                            throw;
                        }
                    }
                }
                catch (Exception ex)
                {
                    string msg = ex.RetrieveErrorMessage();
                    if (msg.Equals(IndicatingMessages.SmsWarningMsg) || msg.Equals(IndicatingMessages.NoCellPhoneWarningMsg))
                    {
                        continue;
                    }
                    TempData["ErrorMessage"] = ex.RetrieveErrorMessage();
                    return RedirectToAction("Index", "Home");
                }
            }
            if (selected == false && taskData != null)
            {
                TempData["TaskData"] = taskData;
                return RedirectToAction("SetupTimer", "TaskDatas", new { @productId = model.ProductId });
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult SetupTimer(int productId)
        {
            var td = TaskDataService.FindTaskDataByProductId(productId);
            SetupTimerViewModel entity = new SetupTimerViewModel()
            {
                TaskId = td.TaskId,
                ProductDetails = SetupProductDetailsVM(td),
                ProductId = productId,
                isEnterprise = td.isEnterprise,
            };

            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetupTimer(SetupTimerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return SetupTimer(model.ProductId);
            }

            var error = TaskDatasBL.SetupTimer_Reseller(model);
            if (error != null)
            {
                TempData["ErrorMessage"] = error;
                return RedirectToAction("SetupTimer", "TaskDatas", new { productId = model.ProductId });
            }
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public async Task CheckTimeoutForBidRevisionRequest() //ATrigger Event/Callback endpoint.
        {
            Log.Information("Atrigger callback to {Callback}", "CheckTimeoutForBidRevisionRequest");

            if (int.TryParse(Request.Form["productId"], out var productId) && productId == 0)
            {
                Log.Warning("Invalid Request from Atrigger, Invalid productId:{productId} - {Callback}", productId, "CheckTimeoutForBidRevisionRequest");
                return;
            }
            Log.Information("ProductId:{productId}", productId);

            var vendorIds = RfqBidService.FindRFQBidListByProductId(productId).GroupBy(g => g.VendorId).Select(x => x.First().VendorId).ToList();
            var error = await TaskDatasBL.RevisionRequestTimeoutHandler(productId, vendorIds);
            Log.Information("RevisionRequestTimeoutHandler:{error}", error);
        }
        
        [AllowAnonymous]
        public async Task CheckTimeoutForBid() //ATrigger Event/Callback endpoint.
        {
            Log.Information("Atrigger callback to {Callback}", "CheckTimeoutForBid");

            if (int.TryParse(Request.Form["productId"], out var productId) && productId == 0)
            {
                Log.Warning("Invalid Request from Atrigger, Invalid productId:{productId} - {Callback}", productId, "CheckTimeoutForBid");
                return;
            }
            Log.Information("ProductId:{productId}", productId);
            
            await TaskDatasBL.BidTimeoutHandler(productId);
        }

        [AllowAnonymous]
        public async Task CheckTimeoutAlertBidWillExpire() //ATrigger Event/Callback endpoint.
        {
            Log.Information("Atrigger callback to {Callback}", "CheckTimeoutAlertBidWillExpire");

            if (int.TryParse(Request.Form["productId"], out var productId) && productId == 0)
            {
                Log.Warning("Invalid Request from Atrigger, Invalid productId:{productId} - {Callback}", productId, "CheckTimeoutAlertBidWillExpire");
                return;
            }
            Log.Information("ProductId:{productId}", productId);

            await TaskDatasBL.BidWillExpireSoonHandler(productId);
        }

        // GET: TaskDatas/Edit/5
        public ActionResult AssignRFQ(int? id)
        {
            if (id == null)
            {
                HandleErrorInfo info = new HandleErrorInfo(new Exception("Task Id is null"), "TaskDatas", "AssignRFQ");
                return View("Error", info);
            }

            TaskData taskData = TaskDataService.FindById(id.Value);

            if (taskData == null)
            {
                HandleErrorInfo info = new HandleErrorInfo(new Exception("TaskData is null"), "TaskDatas", "AssignRFQ");
                return View("Error", info);
            }

            TempData["TaskData"] = taskData;
            var vendors = DbUser.Users.Where(u => u.UserType == USER_TYPE.Vendor);
            TaskDataViewModel model = new TaskDataViewModel
            {
                StateId = (States)taskData.StateId,
                Vendors = GetVendorList(vendors)
            };

            return View(model);
        }
        private IList<SelectListItem> GetVendorList(IQueryable<ApplicationUser> vendors)
        {
            IList<SelectListItem> items = new List<SelectListItem>();
            foreach (var ven in vendors)
            {
                var company = CompanyService.FindCompanyByUserId(ven.Id);
                if (company != null)
                {
                    var item = new SelectListItem { Text = ven.UserName, Value = company.Id.ToString() };
                    items.Add(item);
                }
            }
            return items;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignRFQ(TaskDataViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var vendors = DbUser.Users.Where(u => u.UserType == USER_TYPE.Vendor);
                model.Vendors = GetVendorList(vendors);
                return RedirectToAction("Index", "Home");
            }

            // Must populate dropdown list again, otherwise it will be null
            var vendors2 = DbUser.Users.Where(u => u.UserType == USER_TYPE.Vendor);
            model.Vendors = GetVendorList(vendors2);

            TaskData taskData = (TaskData)TempData["TaskData"];

            Product product = ProductService.FindProductById(taskData.ProductId.Value);
            product.AdminId = User.Identity.GetUserId();
            //product.VendorId = model.VendorId;
            product.ModifiedByUserId = UserContext.UserId;

            ProductService.UpdateProduct(product);

            //taskData.StateId = (int)model.StateId;
            taskData.StateId = (int)States.OutForRFQ;
            taskData.UpdatedBy = User.Identity.Name;
            taskData.ModifiedUtc = DateTime.UtcNow;
            taskData.ModifiedByUserId = UserContext.UserId;

            TaskDataService.Update(taskData);

            // Notify
            var users = taskData.Product.VendorCompany.Users.Where(u => u.Active == true); ;
            foreach (var user in users)
            {
                var destination = user.Email;
                var destinationSms = user.PhoneNumber;

                string subject = $"Omnae.com RFQ {taskData.Product.PartNumber}, {taskData.Product.Description} has been assigned to vendor: {destination}";
                string url = ConfigurationManager.AppSettings["URL"]; 
                var entity = new BaseViewModel
                {
                    UserName = destination,
                    PartNumber = taskData.Product.PartNumber,
                    Description = taskData.Product.Description,
                    Url = url
                };
                try
                {
                    NotificationService.NotifyStateChange((States)taskData.StateId, subject, destination, destinationSms, entity);
                }
                catch (Exception ex)
                {
                    string msg = ex.RetrieveErrorMessage();
                    if (msg.Equals(IndicatingMessages.SmsWarningMsg) || msg.Equals(IndicatingMessages.NoCellPhoneWarningMsg))
                    {
                        continue;
                    }
                    throw;
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Reorder(int? Id)
        {
            if (Id == null)
            {
                HandleErrorInfo info = new HandleErrorInfo(new Exception(IndicatingMessages.ParameterIsNull), "TaskDatas", "Reorder");
                return View("Error", info);
            }
            var product = ProductService.FindProductById(Id.Value);
            if (product == null)
            {
                HandleErrorInfo info = new HandleErrorInfo(new Exception(IndicatingMessages.ProductNotFound), "TaskDatas", "Reorder");
                return View("Error", info);
            }
            return RedirectToAction("OrderDetails", "Products", new { @id = Id.Value });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reorder(int productId)
        {
            TaskData taskData = new TaskData();
            taskData.StateId = (int)States.ProductionStarted;
            taskData.ProductId = productId;
            taskData.UpdatedBy = User.Identity.Name;
            taskData.CreatedUtc = taskData.ModifiedUtc = DateTime.UtcNow;
            taskData.CreatedByUserId = UserContext.UserId;
            taskData.ModifiedByUserId = UserContext.UserId;

            TaskDataService.AddTaskData(taskData);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult SetupMarkupsForAddQty(int id, int? rfqBidId = null)
        {
            var task = TaskDataService.FindById(id);

            if (task.ProductId == null)
            {
                TempData["ErrorMsg"] = "Product id was not found for the task.";
                return RedirectToAction("Index", "Home");
            }


            int productId = task.ProductId.Value;
            List<PriceBreak> priceBreaks = PriceBreakService.FindPriceBreakByTaskId(id)
                .Where(x => x.VendorUnitPrice > 0).OrderBy(x => x.Quantity).ToList();

            var extraQtyId = task.Product.ExtraQuantityId;
            if (extraQtyId == null)
            {
                TempData["ErrorMsg"] = "Couldn't find added extra quantities.";
                return RedirectToAction("Index", "Home");
            }
            var extraQty = ExtraQuantityService.FindExtraQuantityById(extraQtyId.Value);
            var extraPBs = PriceBreakService.FindPriceBreakByTaskId(id).Where(x => x.Quantity >= extraQty.Qty1).ToList();

            if (extraPBs.Count == 0)
            {
                task.StateId = (int)States.QuoteAccepted;
                task.ModifiedUtc = DateTime.UtcNow;
                task.UpdatedBy = User.Identity.Name;
                task.ModifiedByUserId = UserContext.UserId;

                TaskDataService.Update(task);
                TempData["ErrorMsg"] = "Couldn't find Price Breaks for added extra quantities.";
                return RedirectToAction("Index", "Home");
            }

            var priceBreak = PriceBreakService.FindPriceBreakByTaskId(id).FirstOrDefault();
            var model = new SetupMarkupsViewModel
            {
                TaskId = id,
                ProductId = productId,
                RfqBidId = rfqBidId,
                QtyMarks = extraPBs.Select(pb => new QtyMarkup
                {
                    Quantity = pb.Quantity,
                    VendorUnitPrice = pb.VendorUnitPrice,
                    Markup = null,
                    ShipUnitPrice = pb.ShippingUnitPrice ?? 0m,
                }).ToList(),
                VendorToolingCharges = extraPBs.First().ToolingSetupCharges,
                isAddQty = true,
                NumberSampleIncluded = priceBreak?.NumberSampleIncluded,
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult SetupMarkups(int id)
        {
            var task = TaskDataService.FindById(id);

            if (task.ProductId == null)
            {
                TempData["ErrorMessage"] = IndicatingMessages.ProductNotFound;
                return RedirectToAction("Index", "Home");
            }

            int productId = task.ProductId.Value;
            List<PriceBreak> priceBreaks = new List<PriceBreak>();
            if (task.RFQBidId != null)
            {
                priceBreaks = PriceBreakService.FindPriceBreakByTaskIdProductId(task.TaskId, productId).Where(x => x.VendorUnitPrice > 0).ToList();
                if (priceBreaks.Count == 0)
                {
                    var partRevision = PartRevisionService.FindPartRevisionByTaskId(task.TaskId);
                    if (partRevision != null)
                    {
                        productId = partRevision.OriginProductId;
                        priceBreaks = PriceBreakService.FindPriceBreakByProductIdRFQBidId(productId, task.RFQBidId.Value).Where(x => x.VendorUnitPrice > 0).ToList();

                    }
                }
            }
            else
            {
                priceBreaks = PriceBreakService.FindPriceBreakByProductId(productId).Where(x => x.VendorUnitPrice > 0 && x.UnitPrice == null).ToList();
                if (priceBreaks == null || priceBreaks.Count == 0)
                {
                    var partRevision = PartRevisionService.FindPartRevisionByTaskId(task.TaskId);
                    if (partRevision != null)
                    {
                        productId = partRevision.OriginProductId;
                        priceBreaks = PriceBreakService.FindPriceBreakByProductId(productId).Where(x => x.VendorUnitPrice > 0).ToList();

                    }
                }
            }
            List<QtyMarkup> markups = new List<QtyMarkup>();
            foreach (var pb in priceBreaks)
            {
                QtyMarkup qm = new QtyMarkup
                {
                    Quantity = pb.Quantity,
                    VendorUnitPrice = pb.VendorUnitPrice,

                    Markup = null,
                    ShipUnitPrice = pb.ShippingUnitPrice ?? 0m,
                };
                markups.Add(qm);
            }
            SetupMarkupsViewModel model = new SetupMarkupsViewModel
            {
                TaskId = id,
                ProductId = productId,
                RfqBidId = task.RFQBidId ?? 0,
                QtyMarks = markups,
                VendorToolingCharges = priceBreaks.Any() ? priceBreaks[0].ToolingSetupCharges : null,
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult SetupMarkupsForAddQty(SetupMarkupsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model.TaskId);
            }

            using (var trans = AsyncTransactionScope.StartNew())
            {
                var product = ProductService.FindProductById(model.ProductId);
                decimal? customerToolingSetupCharges = null;

                foreach (var ent in model.QtyMarks)
                {
                    var pb = PriceBreakService.FindPriceBreakByTaskIdProductIdQty(model.TaskId, model.ProductId, ent.Quantity);
                    {
                        if (customerToolingSetupCharges == null)
                        {
                            customerToolingSetupCharges = pb.ToolingSetupCharges * (decimal)model.ToolingMarkup;
                        }

                        pb.CustomerToolingSetupCharges = customerToolingSetupCharges;
                        pb.Markup = ent.Markup;
                        pb.UnitPrice = ent.UnitPrice;
                        pb.RFQBidId = model.RfqBidId;
                        pb.NumberSampleIncluded = model.NumberSampleIncluded;
                        PriceBreakService.ModifyPriceBreak(pb);
                    }
                }

                // Update ExtraQuantity table
                if (product.ExtraQuantityId != null)
                {
                    ExtraQuantity exQty = ExtraQuantityService.FindExtraQuantityById(product.ExtraQuantityId.Value);
                    exQty.CustomerToolingSetupCharges = customerToolingSetupCharges;
                    exQty.NumberSampleIncluded = model.NumberSampleIncluded;
                    ExtraQuantityService.UpdateExtraQuantity(exQty);
                }
                TaskData td = TaskDataService.FindById(model.TaskId);
                td.StateId = (int)States.QuoteAccepted;
                td.UpdatedBy = User.Identity.Name;
                td.ModifiedUtc = DateTime.UtcNow;
                td.ModifiedByUserId = UserContext.UserId;

                TaskDataService.Update(td);

                trans.Complete();
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<ActionResult> SetupMarkups(SetupMarkupsViewModel model)
        {
            if (ModelState.IsValid)
            {
                foreach (var ent in model.QtyMarks)
                {
                    PriceBreak pb = PriceBreakService.FindPriceBreakByTaskIdProductIdQty(model.TaskId, model.ProductId, ent.Quantity);
                    if (pb == null)
                    {
                        pb = PriceBreakService.FindPriceBreakByProductIdQty(model.ProductId, ent.Quantity);
                    }
                    if (pb != null)
                    {
                        pb.CustomerToolingSetupCharges = pb.ToolingSetupCharges * (decimal)model.ToolingMarkup;
                        pb.Markup = ent.Markup;
                        pb.UnitPrice = ent.UnitPrice;
                        PriceBreakService.ModifyPriceBreak(pb);
                    }
                }
                if (model.isAddQty == true)
                {
                    var product = ProductService.FindProductById(model.ProductId);
                    var taskData = TaskDataService.FindById(model.TaskId);
                    var userType = UserContext.UserType;
                    var userIds = new List<SimplifiedUser>();

                    userIds.AddRange(taskData.Product.CustomerCompany.Users.Where(u => u.Active == true));
                    foreach (var user in userIds)
                    {
                        var destination = user.Email;
                        var destinationSms = user.PhoneNumber;
                        try
                        {
                            await SendNotifications(taskData, destination, destinationSms);
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

                return RedirectToAction("BidForReview", new { @id = model.TaskId });
            }
            return View(model.TaskId);
        }

        [HttpGet]
        public ActionResult BidForReview(int id)
        {
            TaskData td = TaskDataService.FindById(id);
            if (td?.ProductId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (td.RFQBidId == null && !td.isEnterprise)
            {
                // Workaround code for cases that parts are not through Bid process
                td.StateId = (int)States.QuoteAccepted;
                td.UpdatedBy = User.Identity.Name;
                td.ModifiedUtc = DateTime.UtcNow;
                td.ModifiedByUserId = UserContext.UserId;

                TaskDataService.Update(td);

                return RedirectToAction("Index", "Home");
            }
            var productId = td.ProductId.Value;
            var totalBids = RfqBidService.FindRFQBidListByProductId(productId).Where(x => x.ProductLeadTime != null && x.ToolingCharge != null).ToList();
            List<RFQBid> bids = totalBids.GroupBy(g => g.VendorId)
                .Select(x => x.LastOrDefault())
                .Where(x => x.IsActive == null).ToList();

            if (bids.Count == 0)
            {
                var alreadyBid = totalBids.Where(x => x.IsActive == true).ToList();
                if (alreadyBid.Count > 0)
                {
                    // Check and remove old task
                    var otd = TaskDataService.FindTaskDataListByProductId(productId)
                        .FirstOrDefault(x => x.RFQBidId == null && x.StateId == (int)States.PendingRFQRevision);
                    if (otd == null)
                    {
                        otd = TaskDataService.FindTaskDataListByProductId(productId)
                        .FirstOrDefault(x => x.RFQBidId == null);
                    }
                    if (otd != null && (otd.Orders == null || otd.Orders.Count == 0))
                    {
                        if (otd.NCReports == null || otd.NCReports.Count == 0)
                        {
                            TaskDataService.DeleteTaskData(otd);
                        }
                    }
                    return RedirectToAction("Index", "Home");
                }
                //  No more tasks that participating the Bid
                TempData["ErrorMessage"] = "Nobody is participating the bidding.";
                return RedirectToAction("Index", "Home");
            }

            Dictionary<int, List<PriceBreak>> dic = new Dictionary<int, List<PriceBreak>>();
            Dictionary<int, RFQBid> dicBid = new Dictionary<int, RFQBid>();
            Dictionary<int, string> dicVendorReasons = new Dictionary<int, string>();
            Dictionary<int, SelectList> dicReasons = new Dictionary<int, SelectList>();
            Dictionary<int, bool> dicSelectVendor = new Dictionary<int, bool>();

            List<SelectListItem> list = new List<SelectListItem>();

            for (int i = 0; i < BidFailedReason.Reasons.Length; i++)
            {
                SelectListItem item = new SelectListItem() { Value = i.ToString(), Text = BidFailedReason.Reasons[i] };
                list.Add(item);
            }
            SelectList slist = new SelectList(list, "Value", "Text");

            List<Company> VendorList = new List<Company>();
            List<int> vendorIds = new List<int>();
            List<ChartTypeViewModel> charts = new List<ChartTypeViewModel>();
            List<int> bidFailedReasonIndex = new List<int>();
            bids = bids.OrderBy(x => x.Id).ToList();
            int idx = 0;
            foreach (var bd in bids)
            {
                TaskData taskData = TaskDataService.FindTaskDataByRFQBidId(bd.Id);
                if (taskData == null)
                {
                    // only happened when come back from NCR
                    taskData = td;
                }
                var pricebreaks = PriceBreakService.FindPriceBreakByTaskIdProductId(taskData.TaskId, bd.ProductId);
                if (pricebreaks == null || pricebreaks.Count == 0)
                {
                    var partRevision = PartRevisionService.FindPartRevisionByTaskId(taskData.TaskId);
                    if (partRevision != null)
                    {
                        pricebreaks = PriceBreakService.FindPriceBreakByProductIdRFQBidId(partRevision.OriginProductId, bd.Id);
                    }
                }

                if (!taskData.isEnterprise)
                {
                    // calculate new unit prices with shipping unit price and markup
                    foreach (var pb in pricebreaks)
                    {
                        pb.UnitPrice = (pb.VendorUnitPrice + pb.ShippingUnitPrice) * (decimal)(pb.Markup ?? COMMON_MAX.DEFAULT_MARKUP);
                        PriceBreakService.ModifyPriceBreak(pb);
                    }
                }
                Company vendor = CompanyService.FindCompanyById(bd.VendorId);
                VendorList.Add(vendor);
                if (!dic.ContainsKey(bd.VendorId))
                {
                    dic.Add(bd.VendorId, pricebreaks);
                }
                if (!dicBid.ContainsKey(bd.VendorId))
                {
                    dicBid.Add(bd.VendorId, bd);
                }
                if (!dicVendorReasons.ContainsKey(bd.VendorId))
                {
                    dicVendorReasons.Add(bd.VendorId, string.Empty);
                }
                if (!dicSelectVendor.ContainsKey(bd.VendorId))
                {
                    dicSelectVendor.Add(bd.VendorId, false);
                }
                if (!dicReasons.ContainsKey(bd.VendorId))
                {
                    dicReasons.Add(bd.VendorId, slist);
                }
                vendorIds.Add(bd.VendorId);
                charts.Add(ChartBl.GetVendorQAChart(bd.VendorId, idx++));
            }

            Product product = ProductService.FindProductById(productId);
            var isAnyActive = totalBids.FirstOrDefault(x => x.IsActive == true);
            BidReviewViewModel model = new BidReviewViewModel()
            {
                BidId = td.RFQBidId ?? 0,
                ProductId = productId,
                ProductName = product.Name,
                Vendors = VendorList,
                VendorIds = vendorIds,
                BidIdPriceBreaksDic = dic,
                BidDic = dicBid,
                VendorReason = dicVendorReasons,
                ReasonsDic = dicReasons,
                SelectVendor = dicSelectVendor,
                ToShowCheckbox = isAnyActive == null || (td.NCReports != null && td.NCReports.Any()),
                CustomerPriority = product.CustomerPriority,
                isEnterprise = td.isEnterprise,
                Charts = charts.ToArray(),
                //BidFailedReasonIndex = bidFailedReasonIndex,
            };
            return View(model);
        }


        [HttpPost]
        public ActionResult ReviewBid(int? bidVendorId, int vendorId, int productId, int SampleLeadTime, int ProductLeadTime, int? ReasonIndex)
        {
            string error = TaskDatasBL.ReviewBid(bidVendorId == vendorId, vendorId, productId, ReasonIndex);
            if (error != null)
            {
                TempData["ErrorMessage"] = error;
                return RedirectToAction("Index", "Home");
            }
            return new EmptyResult();
        }

        public async Task<ActionResult> CancelRFQ(int Id)
        {
            TempData["ErrorMessage"] = await TaskDatasBL.CancelRFQ(Id);
            return RedirectToAction("Index", "Home");
        }
    }
}
