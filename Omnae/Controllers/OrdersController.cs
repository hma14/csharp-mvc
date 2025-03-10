using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.Azure;
using Omnae.BlobStorage;
using Omnae.Common;
using Omnae.Context;
using Omnae.Model.Models;
using Omnae.QuickBooks.ViewModels;
using Omnae.Service.Service.Interfaces;
using Omnae.ShippingAPI.DHL.Models;
using Omnae.ViewModels;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using Common;
using Libs.Notification;
using Omnae.BusinessLayer;
using Omnae.BusinessLayer.Identity.Model;
using Omnae.BusinessLayer.Models;
using Omnae.BusinessLayer.Services;
using Omnae.Libs.Notification;
using Omnae.QuickBooks.QBO;
using Omnae.Service.Service;
using Omnae.Model.Context;

namespace Omnae.Controllers
{
    public class OrdersController : BaseController
    {
        private OrdersBL OrdersBL { get; }
        private TaskDataCustomerBL TaskDataCustomerBl { get; }

        public OrdersController(IRFQBidService rfqBidService, ICompanyService companyService, ITaskDataService taskDataService, IPriceBreakService priceBreakService, IOrderService orderService, ILogedUserContext userContext, IProductService productService, IDocumentService documentService, IShippingService shippingService, ICountryService countryService, IAddressService addressService, IStateProvinceService stateProvinceService, IOrderStateTrackingService orderStateTrackingService, IProductStateTrackingService productStateTrackingService, IPartRevisionService partRevisionService, IStoredProcedureService spService, INCReportService ncReportService, IRFQQuantityService rfqQuantityService, IExtraQuantityService extraQuantityService, IBidRequestRevisionService bidRequestRevisionService, ITimerSetupService timerSetupService, IQboTokensService qboTokensService, IOmnaeInvoiceService omnaeInvoiceService, INCRImagesService ncrImagesService, IApprovedCapabilityService approvedCapabilityService, IShippingAccountService shippingAccountService, ApplicationDbContext dbUser, ProductBL productBl, NotificationService notificationService, UserContactService userContactService, TimerTriggerService timerTriggerService, NotificationBL notificationBl, PaymentBL paymentBl, ShipmentBL shipmentBl, ChartBL chartBl, IMapper mapper, NcrBL ncrBL, IDocumentStorageService documentStorageService, IImageStorageService imageStorageService, OrdersBL ordersBl, TaskDataCustomerBL taskDataCustomerBl) : base(rfqBidService, companyService, taskDataService, priceBreakService, orderService, userContext, productService, documentService, shippingService, countryService, addressService, stateProvinceService, orderStateTrackingService, productStateTrackingService, partRevisionService, spService, ncReportService, rfqQuantityService, extraQuantityService, bidRequestRevisionService, timerSetupService, qboTokensService, omnaeInvoiceService, ncrImagesService, approvedCapabilityService, shippingAccountService, dbUser, productBl, notificationService, userContactService, timerTriggerService, notificationBl, paymentBl, shipmentBl, chartBl, mapper, ncrBL, documentStorageService, imageStorageService)
        {
            OrdersBL = ordersBl;
            TaskDataCustomerBl = taskDataCustomerBl;
        }

        // GET: Orders
        public ActionResult Index()
        {
            if (UserContext.Company == null)
                return RedirectToAction("Index", "Home");

            var model = OrdersBL.GetOrders();
            return View(model);
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            ViewBag.ChargeError = null;
            if (id == null)
            {
                HandleErrorInfo info = new HandleErrorInfo(new Exception(IndicatingMessages.TaskNotFound), "Details", "Orders");
                return View("Error", info);
            }

            var model = OrdersBL.Details((int)id);
            if (model == null)
            {
                HandleErrorInfo info = new HandleErrorInfo(new Exception(IndicatingMessages.InvalidAccess), "Details", "Orders");
                return View("Error", info);
            }

            return View(model);
        }

        // GET: Orders/Details/5
        public ActionResult OrderDetails(int? id)
        {
            ViewBag.ChargeError = null;
            if (id == null)
            {
                TempData["ErrorMessage"] = IndicatingMessages.TaskNotFound;
                return RedirectToAction("Index", "Orders");
            }
            var taskData = TaskDataService.FindById(id.Value); //TODO: Check if this user have permittion to see this TaskData.
            if (taskData == null)
            {
                TempData["ErrorMessage"] = IndicatingMessages.TaskNotFound;
                return RedirectToAction("Index", "Orders");
            }

            Product product = taskData.Product; //TODO: 
            if (this.UserContext.UserType == USER_TYPE.Customer)
            {
                var prods = TaskDataCustomerBl.GetCustomerProductsOnTask(taskData);
                if (prods == null)
                {
                    TempData["ErrorMessage"] = IndicatingMessages.InvalidAccess;
                    return RedirectToAction("Index", "Orders");
                }
                product = prods.FirstOrDefault(x => x.Id == taskData.ProductId);
            }

            var orders = OrderService.FindOrderByTaskId(taskData.TaskId);
            if (orders == null || orders.Count == 0)
            {
                orders = OrderService.FindOrdersByProductId(taskData.ProductId.Value);
                if (orders == null || orders.Count == 0)
                {
                    TempData["ErrorMessage"] = "No order could be found related to this task";
                    return RedirectToAction("Index", "Orders");
                }
            }
            var order = orders.Last();

            List<ProductStateTracking> productStates = ProductStateTrackingService.FindProductStateTrackingListByProductId(product.Id).OrderBy(x => x.ModifiedUtc).ToList();

            var orderStates = OrderStateTrackingService.FindOrderStateTrackingListByOrderId(order.Id).OrderBy(x => x.ModifiedUtc).ToList();

            StateLastUpdatedViewModel stateLastUpdated = new StateLastUpdatedViewModel
            {
                StateId = (States)taskData.StateId,
                LastUpdated = taskData.ModifiedUtc,
            };

            DHLResponse shippingResp = new DHLResponse();
            if (order.TrackingNumber != null)
            {
                shippingResp = ShipmentBL.ShipmentTracking(order.TrackingNumber.Contains(" ") ? order.TrackingNumber.Replace(" ", string.Empty) : order.TrackingNumber);
            }

            DocumentService.UpdateDocUrlWithSecurityToken(product?.Documents);

            StateTrackingViewModel model = new StateTrackingViewModel()
            {
                OrderId = order.Id,
                TaskId = id.Value,
                StateId = (States)taskData.StateId,
                OrderStateTrackings = orderStates,
                ProductStateTrackings = productStates,
                Order = order,
                Product = product,
                IsTagged = taskData.isTagged,
                LastUpdated = stateLastUpdated,
                DHLResponse = shippingResp,
                isEnterprise = taskData.isEnterprise,
                UserType = UserContext.UserType,
            };

            TempData["ReturnUrl"] = this.Url.Action("OrderDetails", "Orders", new { id = id.Value }, this.Request.Url.Scheme);
            TempData.Keep();

            return View(model);
        }


        [HttpPost] //TODO: Check if this is used.
        public void UploadPODoc(int productId)
        {
            if (!ModelState.IsValid)
                return;
            if (Request.Files == null || Request.Files.Count == 0)
                return;

            var postedFile = Request.Files[0];
            Document po = null;
            if (postedFile.FileName != "")
            {
                if (postedFile.ContentType.Contains("application/pdf"))
                {
                    string fileName = string.Format("po_{0}{1}", productId, Path.GetExtension(postedFile.FileName));
                    var docUri = DocumentStorageService.Upload(postedFile, fileName);
                    po = new Document()
                    {
                        Version = 1,
                        Name = fileName,
                        DocUri = docUri,
                        DocType = (int)DOCUMENT_TYPE.PO_PDF,
                        ProductId = productId,
                        UpdatedBy = User.Identity.Name,
                        CreatedUtc = DateTime.UtcNow,
                        ModifiedUtc = DateTime.UtcNow
                    };
                    try
                    {
                        DocumentService.AddDocument(po);
                    }
                    catch (Exception ex)
                    {
                        TempData["AzureException"] = "PO document uplading failed - " + ex.Message;
                    }
                }
                else
                {
                    TempData["AzureException"] = "Invalid file type - must be PDF file!";
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PlaceOrderViewModel model)
        {
            if (!ModelState.IsValid)
                return View();

            CreateOrderResult result = await OrdersBL.CreateOrder(model, this.ControllerContext);

            if (string.IsNullOrEmpty(result.Message))
            {

                switch (model.PaymentMethod)
                {
                    case (int)PAYMENT_METHODS.CreditCard:
                        return RedirectToAction("Charge", "Orders", new { @Id = result.OrderId, @isReorder = model.IsReorder });

                    case (int)PAYMENT_METHODS.Term:

                        if (result.Term > 0)
                        {
                            PayWithTermViewModel terms = new PayWithTermViewModel
                            {
                                TaskId = result.TaskId,
                                OrderId = result.OrderId,
                                CompanyName = result.CustomerName,
                                Amount = model.Total,
                                Term = result.Term,
                                IsReorder = model.IsReorder,
                                CustomerPONumber = model.PONumber,
                            };
                            return RedirectToAction("PayWithTerm", "Orders", terms);
                        }

                        TempData["PlaceOrderError"] = "You are not assigned as a Term customer. Please choose other payment method.";
                        return RedirectToAction("PlaceOrder", "Products", new { @id = model.TaskId });

                    case (int)PAYMENT_METHODS.Cheque:
                        PayWithOthersViewModel cheque = new PayWithOthersViewModel
                        {
                            TaskId = result.TaskId,
                            OrderId = result.OrderId,
                            CompanyName = result.CustomerName,
                            IsReorder = model.IsReorder,
                            CustomerPONumber = model.PONumber,
                            Amount = model.Total,
                        };
                        return RedirectToAction("PayWithCheque", "Orders", cheque);

                    case (int)PAYMENT_METHODS.Wire:
                        PayWithOthersViewModel wire = new PayWithOthersViewModel
                        {
                            TaskId = result.TaskId,
                            OrderId = result.OrderId,
                            CompanyName = result.CustomerName,
                            IsReorder = model.IsReorder,
                            CustomerPONumber = model.PONumber,
                            Amount = model.Total,
                        };
                        return RedirectToAction("PayWithWire", "Orders", wire);

                    case (int)PAYMENT_METHODS.Others:
                        PayWithOthersViewModel others = new PayWithOthersViewModel
                        {
                            TaskId = result.TaskId,
                            OrderId = result.OrderId,
                            CompanyName = result.CustomerName,
                            IsReorder = model.IsReorder,
                            CustomerPONumber = model.PONumber,
                            Amount = model.Total,
                        };
                        return RedirectToAction("PayWithOthers", "Orders", others);

                    default:
                        TempData["PlaceOrderError"] = "Payment method couldn't be found";
                        return RedirectToAction("PlaceOrder", "Products", new { @id = model.TaskId });

                }
            }
            else
            {
                TempData["PlaceOrderError"] = result.Message;
                return RedirectToAction("PlaceOrder", "Products", new { @id = model.TaskId });
            }
        }

        [AllowAnonymous]
        public ActionResult CreateVendorPurchaseOrder(PurchaseOrderViewModel model) // PDF generation PO
        {
            return View(model);
        }

        [HttpGet]
        public ActionResult PayWithTerm(PayWithTermViewModel entity)
        {
            Order order = OrderService.FindOrderById(entity.OrderId);
            if (order == null)
            {
                TempData["PlaceOrderError"] = IndicatingMessages.OrderNotFound;
                return RedirectToAction("PlaceOrder", "Products", new { @id = entity.TaskId });
            }
            string error = OrdersBL.PostPlaceOrderActionWithTerm(order, entity.CustomerPONumber, entity.IsReorder);
            if (error != null)
            {
                TempData["PlaceOrderError"] = error;
                return RedirectToAction("PlaceOrder", "Products", new { @id = entity.TaskId });
            }
            return View(entity);
        }

        [HttpGet]
        public ActionResult PayWithCheque(PayWithOthersViewModel entity)
        {
            Order order = OrderService.FindOrderById(entity.OrderId);
            if (order == null)
            {
                TempData["PlaceOrderError"] = IndicatingMessages.OrderNotFound;
                return RedirectToAction("PlaceOrder", "Products", new { @id = entity.TaskId });
            }
            string error = OrdersBL.PostPlaceOrderAction(order, entity.CustomerPONumber, entity.IsReorder);
            if (error != null)
            {
                TempData["PlaceOrderError"] = error;
                return RedirectToAction("PlaceOrder", "Products", new { @id = entity.TaskId });
            }
            return View(entity);
        }

        [HttpGet]
        public ActionResult PayWithWire(PayWithOthersViewModel entity)
        {
            Order order = OrderService.FindOrderById(entity.OrderId);
            if (order == null)
            {
                TempData["PlaceOrderError"] = IndicatingMessages.OrderNotFound;
                return RedirectToAction("PlaceOrder", "Products", new { @id = entity.TaskId });
            }
            string error = OrdersBL.PostPlaceOrderAction(order, entity.CustomerPONumber, entity.IsReorder);
            if (error != null)
            {
                TempData["PlaceOrderError"] = error;
                return RedirectToAction("PlaceOrder", "Products", new { @id = entity.TaskId });
            }
            return View(entity);
        }

        [HttpGet]
        public ActionResult PayWithOthers(PayWithOthersViewModel entity)
        {
            Order order = OrderService.FindOrderById(entity.OrderId);
            if (order == null)
            {
                TempData["PlaceOrderError"] = IndicatingMessages.OrderNotFound;
                return RedirectToAction("PlaceOrder", "Products", new { @id = entity.TaskId });
            }
            string error = OrdersBL.PostPlaceOrderAction(order, entity.CustomerPONumber, entity.IsReorder);
            if (error != null)
            {
                TempData["PlaceOrderError"] = error;
                return RedirectToAction("PlaceOrder", "Products", new { @id = entity.TaskId });
            }
            return View(entity);
        }

        [HttpGet]
        public ActionResult TrackingShippingStatusPage(string id)
        {
            var resp = ShipmentBL.ShipmentTracking(id);
            return View(resp);
        }

        public ActionResult TrackingShippingStatus(string id)
        {
            var resp = ShipmentBL.ShipmentTracking(id);
            return PartialView("_TrackingShippingStatus", resp);
        }

        [HttpGet]
        public ActionResult CreateNCR(int? id, int taskId)
        {
            if (id == null || taskId == 0) // id: OrderId
            {
                HandleErrorInfo info = new HandleErrorInfo(new Exception(IndicatingMessages.ParameterIsNull), "Orders", "CreateNCR");
                return View("Error", info);
            }
            var order = OrderService.FindOrderById(id.Value);
            if (order == null)
            {
                HandleErrorInfo info = new HandleErrorInfo(new Exception(IndicatingMessages.InvalidAccess), "Orders", "CreateNCR");
                return View("Error", info);
            }

            var prod = ProductService.FindProductById(order.ProductId);
            Company customer = prod.CustomerCompany;
            Company vendor = prod.VendorCompany;
            NcrDescriptionViewModel model = new NcrDescriptionViewModel()
            {
                ProductId = prod.Id,
                TaskId = taskId,
                OrderId = order.Id,
                StateId = States.NCRVendorRootCauseAnalysis,
                CustomerId = prod.CustomerId,
                VendorId = prod.VendorId,
                Customer = customer?.Name,
                Vendor = vendor?.Name,
                ProductName = prod.Name,
                ProductAvatarUri = prod.AvatarUri,
                ProductPartNo = prod.PartNumber,
                PartRevisionNo = prod.PartNumberRevision,
                ProductDescription = prod.Description,
                NCDetectedby = DETECTED_BY.CUSTOMER,
                NCDetectedDate = DateTime.UtcNow,
                PONumber = order.CustomerPONumber,
                NCRNumber = NcReportService.FindNCReportsYearlySequence(prod.CustomerCompany.Id),
                NCRNumberForVendor = NcReportService.FindNCReportsYearlySequenceForVendor(prod.VendorId.Value),
                TotalProductQuantity = OrderService.FindOrderByCustomerId(prod.CustomerId.Value)
                    .Where(o => o.ProductId == prod.Id)
                    .Sum(q => q.Quantity),
            };
            return View(model);
        }

        [HttpGet]
        public ActionResult NcrHistory(int? id)
        {
            if (id == null)
            {
                HandleErrorInfo info = new HandleErrorInfo(new Exception(IndicatingMessages.ParameterIsNull), "Orders", "NcrHistory");
                return View("Error", info);
            }

            NCRViewModel model = new NCRViewModel();
            var order = OrderService.FindOrderById(id.Value);
            var ncrs = NcReportService.FindNCReportByProductIdOrderId(order.ProductId, order.Id);
            model.NcrInfoList = ncrs.Select(g => new NcrInfoViewModel
            {
                Id = g.Id,
                NCRNumber = g.NCRNumber,
                DateInitiated = g.NCDetectedDate,
                RootCause = g.RootCause != null ? Enum.GetName(typeof(NC_ROOT_CAUSE), g.RootCause) : null,
                Vendor = g.VendorId != null ? CompanyService.FindCompanyById(g.VendorId.Value).Name : null,
                Cost = g.Cost,
                DateClosed = g.DateNcrClosed,
                UserType = UserContext.UserType,
            }).ToList();

            return View(model);
        }

        private async Task<Tuple<string, string>> CallQBOCreateEstimate(PurchaseOrderViewModel model)
        {
            QboApi qboApi = new QboApi(QboTokensService);

            byte[] data = await qboApi.CreateEstimate(model);

            if (data != null && model.CustomerId != null)
            {
                string fileName = $"estimate_companyid_{model.CustomerId.Value}_productid_{model.ProductId}.pdf";

                var docUri = DocumentStorageService.Upload(data, fileName);
                var doc = new Document()
                {
                    TaskId = model.TaskId,
                    Version = 1,
                    Name = fileName,
                    DocUri = docUri,
                    DocType = (int)DOCUMENT_TYPE.QBO_ESTIMATE_PDF,
                    ProductId = model.ProductId,
                    UpdatedBy = User.Identity.Name,
                    CreatedUtc = DateTime.UtcNow,
                    ModifiedUtc = DateTime.UtcNow
                };
                var d = DocumentService.FindDocumentByProductId(model.ProductId).FirstOrDefault(x => x.Name == fileName);
                if (d == null)
                {
                    DocumentService.AddDocument(doc);
                }
            }
            return new Tuple<string, string>(model.EstimateId, model.EstimateNumber);
        }


    }
}
