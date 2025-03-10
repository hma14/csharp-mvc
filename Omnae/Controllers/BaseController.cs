using AutoMapper;
using Common;
using Libs.Notification;
using Microsoft.Azure;
using Omnae.BlobStorage;
using Omnae.BusinessLayer;
using Omnae.BusinessLayer.Identity.Model;
using Omnae.BusinessLayer.Models;
using Omnae.BusinessLayer.Services;
using Omnae.Common;
using Omnae.Context;
using Omnae.Model.Models;
using Omnae.Model.Models.Aspnet;
using Omnae.Service.Service;
using Omnae.Service.Service.Interfaces;
using Omnae.ShippingAPI.DHL.Libs;
using Omnae.ShippingAPI.DHL.Models;
using Omnae.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Omnae.Util;
using Omnae.Data;
using Omnae.Libs.Notification;
using Omnae.Model.Context;

namespace Omnae.Controllers
{
    public class BaseController : Controller, IHaveUserContext
    {
        public IRFQBidService RfqBidService { get; }
        public ICompanyService CompanyService { get; }
        public ITaskDataService TaskDataService { get; }
        public IPriceBreakService PriceBreakService { get; }
        public IOrderService OrderService { get; }
        public ILogedUserContext UserContext { get; }

        protected IProductService ProductService { get; }
        protected IDocumentService DocumentService { get; }
        protected IDocumentStorageService DocumentStorageService { get; }
        protected IImageStorageService ImageStorageService { get; }
        protected IShippingService ShippingService { get; }
        protected ICountryService CountryService { get; }
        protected IAddressService AddressService { get; }
        protected IStateProvinceService StateProvinceService { get; }
        protected IOrderStateTrackingService OrderStateTrackingService { get; }
        protected IProductStateTrackingService ProductStateTrackingService { get; }
        protected IPartRevisionService PartRevisionService { get; }
        protected IStoredProcedureService SpService { get; }
        protected INCReportService NcReportService { get; }
        protected IRFQQuantityService RfqQuantityService { get; }
        protected IExtraQuantityService ExtraQuantityService { get; }
        protected IBidRequestRevisionService BidRequestRevisionService { get; }
        protected ITimerSetupService TimerSetupService { get; }
        protected IQboTokensService QboTokensService { get; }
        protected IOmnaeInvoiceService OmnaeInvoiceService { get; }
        protected INCRImagesService NCRImagesService { get; }
        protected IApprovedCapabilityService ApprovedCapabilityService { get; }
        protected IShippingAccountService ShippingAccountService { get; }

        protected ApplicationDbContext DbUser { get; }
        protected ProductBL ProductBl { get; }
        protected NotificationService NotificationService { get; }
        protected UserContactService UserContactService { get; }
        protected TimerTriggerService TimerTriggerService { get; }
        protected NotificationBL NotificationBL { get; }
        protected PaymentBL PaymentBl { get; }
        protected ShipmentBL ShipmentBL { get; }
        protected ChartBL ChartBl { get; }
        protected NcrBL NcrBL { get; }

        protected readonly IMapper mapper;

        public BaseController(IRFQBidService rfqBidService, ICompanyService companyService, ITaskDataService taskDataService, IPriceBreakService priceBreakService, IOrderService orderService, ILogedUserContext userContext, IProductService productService, IDocumentService documentService, IShippingService shippingService, ICountryService countryService, IAddressService addressService, IStateProvinceService stateProvinceService, IOrderStateTrackingService orderStateTrackingService, IProductStateTrackingService productStateTrackingService, IPartRevisionService partRevisionService, IStoredProcedureService spService, INCReportService ncReportService, IRFQQuantityService rfqQuantityService, IExtraQuantityService extraQuantityService, IBidRequestRevisionService bidRequestRevisionService, ITimerSetupService timerSetupService, IQboTokensService qboTokensService, IOmnaeInvoiceService omnaeInvoiceService, INCRImagesService ncrImagesService, IApprovedCapabilityService approvedCapabilityService, IShippingAccountService shippingAccountService, ApplicationDbContext dbUser, ProductBL productBl, NotificationService notificationService, UserContactService userContactService, TimerTriggerService timerTriggerService, NotificationBL notificationBl, PaymentBL paymentBl, ShipmentBL shipmentBl, ChartBL chartBl, IMapper mapper, NcrBL ncrBL, IDocumentStorageService documentStorageService, IImageStorageService imageStorageService)
        {
            RfqBidService = rfqBidService;
            CompanyService = companyService;
            TaskDataService = taskDataService;
            PriceBreakService = priceBreakService;
            OrderService = orderService;
            UserContext = userContext;
            ProductService = productService;
            DocumentService = documentService;
            ShippingService = shippingService;
            CountryService = countryService;
            AddressService = addressService;
            StateProvinceService = stateProvinceService;
            OrderStateTrackingService = orderStateTrackingService;
            ProductStateTrackingService = productStateTrackingService;
            PartRevisionService = partRevisionService;
            SpService = spService;
            NcReportService = ncReportService;
            RfqQuantityService = rfqQuantityService;
            ExtraQuantityService = extraQuantityService;
            BidRequestRevisionService = bidRequestRevisionService;
            TimerSetupService = timerSetupService;
            QboTokensService = qboTokensService;
            OmnaeInvoiceService = omnaeInvoiceService;
            NCRImagesService = ncrImagesService;
            ApprovedCapabilityService = approvedCapabilityService;
            ShippingAccountService = shippingAccountService;
            DbUser = dbUser;
            ProductBl = productBl;
            NotificationService = notificationService;
            UserContactService = userContactService;
            TimerTriggerService = timerTriggerService;
            NotificationBL = notificationBl;
            PaymentBl = paymentBl;
            ShipmentBL = shipmentBl;
            ChartBl = chartBl;
            this.mapper = mapper;
            NcrBL = ncrBL;
            DocumentStorageService = documentStorageService;
            ImageStorageService = imageStorageService;
        }

        private List<Order> GetCutomerOrders(int customerId)
        {
            return OrderService.FindOrderByCustomerId(customerId);
        }

        protected Order GetCustomerOrderByOrderId(int orderId)
        {
            return OrderService.FindOrderById(orderId);
        }

        private List<Document> GetDocumentsByProductId(int productId)
        {
            return DocumentService.FindDocumentByProductId(productId).ToList();
        }

        private List<Document> GetDocumentsByTaskIdDocType(int taskId, DOCUMENT_TYPE type)
        {
            return DocumentService.FindDocumentByTaskIdDocType(taskId, type);
        }

        protected ProductFileViewModel SetupProductFilesVM(TaskData td)
        {
            if (td.ProductId == null)
            {
                return null;
            }

            var allDocs = GetDocumentsByProductId(td.ProductId.Value);

            RFQBid rfqBid = null;
            if (td.RFQBidId != null)
            {
                rfqBid = RfqBidService.FindRFQBidById(td.RFQBidId.Value);
            }

            var model = new ProductFileViewModel
            {
                TaskId = td.TaskId,
                ProductId = td.ProductId.Value,
                StateId = (States)td.StateId,
                Doc2Ds = allDocs.Where(d => d.DocType == (int)DOCUMENT_TYPE.PRODUCT_2D_PDF).ToList(),
                Doc3Ds = allDocs.Where(d => d.DocType == (int)DOCUMENT_TYPE.PRODUCT_3D_STEP).ToList(),
                QuoteDocs = allDocs.Where(d => d.DocType == (int)DOCUMENT_TYPE.QUOTE_PDF).ToList(),
                QuoteDocumentForRFQBid = DocumentService.FindDocumentByTaskIdDocType(td.TaskId, DOCUMENT_TYPE.QUOTE_PDF).FirstOrDefault(),
                RevisingDocs = allDocs.Where(d => d.DocType == (int)DOCUMENT_TYPE.REVISING_DOCS).ToList(),
                ProofDocs = allDocs.Where(d => d.DocType == (int)DOCUMENT_TYPE.PROOF_PDF).ToList(),
                PackingDocs = allDocs.Where(d => d.DocType == (int)DOCUMENT_TYPE.PACKING_SLIP_PDF).ToList(),
                InspectionReportDocs = allDocs.Where(d => d.DocType == (int)DOCUMENT_TYPE.PACKING_SLIP_INSPECTION_REPORT_PDF).ToList(),
                CustInvoiceDocs = allDocs.Where(d => d.DocType == (int)DOCUMENT_TYPE.QBO_INVOICE_PDF).ToList(),
                UserType = UserContext.UserType,
                PaymentProofDocs = allDocs.Where(d => d.DocType == (int)DOCUMENT_TYPE.PAYMENT_PROOF).ToList(),
            };

            return model;
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateNCR(NcrDescriptionViewModel model)
        {
            if (!ModelState.IsValid) 
                return View(model.ProductId);
            using (var trans = AsyncTransactionScope.StartNew())
            {
                await NcrBL.CreateNCReport(model, Request.Files.ToFileList());

                trans.Complete();
                return RedirectToAction("Index", "Home");
            }
        }

        [NonAction]
        public ExtraQuantityPartDetailsViewModel GetExtraQuantityPartDetails(Product product)
        {
            if (product.ExtraQuantityId == null)
            {
                return null;
            }

            var extraQty = ExtraQuantityService.FindExtraQuantityById(product.ExtraQuantityId.Value);
            if (extraQty == null)
            {
                return null;
            }

            var model = new ExtraQuantityPartDetailsViewModel()
            {
                ToolingLeadTime = extraQty.ToolingLeadTime,
                ProductionLeadTime = extraQty.ProductionLeadTime,
                SampleLeadTime = extraQty.SampleLeadTime,
                ToolingSetupCharges = extraQty.ToolingSetupCharges,
                CustomerToolingSetupCharges = extraQty.CustomerToolingSetupCharges,
                HarmonizedCode = extraQty.HarmonizedCode,
            };
            return model;
        }

        protected bool IsAllVendorsResponsed(int productId, List<int> vendorIds)
        {
            var pb = PriceBreakService.FindPriceBreakByProductId(productId).GroupBy(g => g.RFQBidId).Select(x => x.First());
            return (vendorIds.Count == pb.Count());
        }

        protected Task SendNotifications(TaskData taskData, string destination, string destinationSms, bool isAdmin = false, List<byte[]> attachmentData = null, List<string> VendorInvoicesNumbers = null)
        {
            return NotificationBL.SendNotificationsAsync(taskData, destination, destinationSms, isAdmin, attachmentData, VendorInvoicesNumbers);
        }



        protected List<UserNameCompanyId> GetUserIdList(IQueryable<ApplicationUser> users)
        {
            var items = new List<UserNameCompanyId>();
            foreach (var user in users)
            {
                var company = CompanyService.FindCompanyByUserId(user.Id);
                if (company != null)
                {
                    var item = new UserNameCompanyId { CompanyId = company.Id, UserName = company.Name };
                    items.Add(item);
                }
            }
            return items.OrderBy(x => x.UserName).GroupBy(g => g.CompanyId).Select(g => g.First()).ToList();
        }

        protected List<UserNameCompanyId> GetUserIds(IQueryable<ApplicationUser> users)
        {
            var items = new List<UserNameCompanyId>();
            foreach (var user in users)
            {
                var company = CompanyService.FindCompanyByUserId(user.Id);
                if (company != null)
                {
                    var item = new UserNameCompanyId { CompanyId = company.Id, UserName = company.Name };
                    items.Add(item);
                }
            }
            return items.OrderBy(x => x.UserName).GroupBy(g => g.CompanyId).Select(g => g.First()).ToList();
        }

        [NonAction] //Used in Views
        public string GetChangePartRevisionReason(int taskId)
        {
            var partRevision = PartRevisionService.FindPartRevisionByTaskId(taskId);
            return partRevision?.Description;
        }

        protected OmnaeInvoiceViewModel GetOmnaeInvoiceViewModel(OmnaeInvoice invoice, Company company)
        {
            var product = ProductService.FindProductById(invoice.ProductId);
            if (product == null)
            {
                return null;
            }

            var order = OrderService.FindOrderById(invoice.OrderId);

            // Get Customer info for invoice            
            Model.Models.Address address = null;
            string companyName = string.Empty;
            SimplifiedUser user = null;

            if (invoice.UserType == (int)USER_TYPE.Customer)
            {
                address = AddressService.FindAddressById(company.AddressId.Value);
                companyName = company.Name;
                user = company.Users.FirstOrDefault(); //TODO: Change this lohic to add All Users
                // user = dbUser.Users.Where(u => u.Id == company.UserId).FirstOrDefault();
            }
            else if (invoice.UserType == (int)USER_TYPE.Vendor)
            {
                var admin = CompanyService.FindAllCompanies().FirstOrDefault(x => x.Name == "Administrator");
                if (admin == null)
                {
                    return null;
                }

                address = AddressService.FindAddressById(admin.AddressId.Value);
                companyName = Administrator_Account.Name; // admin.Name;
                user = admin.Users.FirstOrDefault(); //TODO: Change this logic to add All Users
                // user = dbUser.Users.Where(u => u.Id == admin.UserId).FirstOrDefault();
            }

            var totalAmount = invoice.Quantity * invoice.UnitPrice + invoice.ToolingSetupCharges + invoice.SalesTax;
            var model = new OmnaeInvoiceViewModel
            {
                Id = invoice.Id,
                ProductId = product.Id,
                ProductName = product.Name,
                PartNumber = product.PartNumber,
                PartRevision = product.PartRevision?.Name,
                ProductDescription = product.Description,
                Quantity = invoice.Quantity,
                UnitPrice = invoice.UnitPrice,
                Amount = totalAmount,
                SalesTax = invoice.SalesTax,
                ToolingSetupCharges = invoice.ToolingSetupCharges,
                InvoiceDate = invoice.InvoiceDate,
                DueDate = invoice.DueDate,
                BillAddr = address,
                CompanyId = invoice.CompanyId,
                CompanyName = companyName,
                Email = user?.Email,
                ShippingDate = order?.ShippedDate,
                ShipVia = order?.CarrierName,
                TrackingNo = order?.TrackingNumber,
                Term = company.Term,
                IsOpen = invoice.IsOpen,
                ClosedDate = invoice.CloseDate,
                PaymentMethod = invoice.PaymentMethod,
                PaymentRefNumber = invoice.PaymentRefNumber,
                InvoiceNumber = invoice.InvoiceNumber,
                CustomerPONumber = order != null ? order.CustomerPONumber : invoice.PONumber,
                BillNumber = invoice.BillNumber,
            };

            return model;
        }

        public ActionResult InvoiceDetail(int Id)
        {
            var invoice = OmnaeInvoiceService.FindOmnaeInvoiceById(Id);
            if (invoice == null)
            {
                return RedirectToAction("GetInvoices", "Home", new { @userTyp = USER_TYPE.Customer });
            }

            var company = CompanyService.FindCompanyById(invoice.CompanyId);
            if (company == null)
            {
                return RedirectToAction("GetInvoices", "Home", new { @userTyp = USER_TYPE.Customer });
            }

            OmnaeInvoiceViewModel model = GetOmnaeInvoiceViewModel(invoice, company);
            return PartialView("_InvoiceDetails", model);
        }

        [NonAction] //Used in Views
        public string GetVendorName(TaskData td)
        {
            if (td.RFQBidId == null)
            {
                return null;
            }

            var rfqbid = RfqBidService.FindRFQBidById(td.RFQBidId.Value);
            if (rfqbid == null || rfqbid.VendorId == 0)
            {
                return null;
            }

            return CompanyService.FindCompanyById(rfqbid.VendorId).Name;
        }

        [NonAction] //Used in Views
        public string GetVendorName(int tid)
        {
            TaskData td = TaskDataService.FindById(tid);
            return GetVendorName(td);
        }

        [NonAction] //Used in Views
        public string GetCompanyIdFromQuoteName(string quoteDoc)
        {
            // example: quote_457_company-134.pdf
            string[] arr = quoteDoc.Split('-');
            if (arr.Length <= 1)
            {
                return null;
            }

            string[] arr2 = arr[1].Split('.');
            return arr2[0];  // return "134"
        }

        [NonAction] //Used in Views
        public ProductDetailsViewModel SetupProductDetailsVM(TaskData td, int? orderId = null)
        {
            return ProductBl.SetupProductDetailsVM(td, orderId);
        }

        [NonAction] //Used in Views
        public int? GetNumberSampleIncluded(int taskId, int productId)
        {
            var pb = PriceBreakService.FindPriceBreakByTaskIdProductId(taskId, productId).FirstOrDefault(x => x.NumberSampleIncluded != null);
            return pb?.NumberSampleIncluded;
        }
    }
}