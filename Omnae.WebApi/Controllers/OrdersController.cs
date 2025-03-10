using AutoMapper;
using Omnae.BusinessLayer;
using Omnae.BusinessLayer.Interface;
using Omnae.BusinessLayer.Models;
using Omnae.Model.Models;
using Omnae.Service.Service.Interfaces;
using Omnae.WebApi.DTO;
using Omnae.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using AutoMapper.QueryableExtensions;
using Omnae.Common;
using Omnae.Service.Service;
using System.ComponentModel.DataAnnotations;
using Omnae.Data.Query;
using System.Data.Entity;
using Common;
using Common.Extensions;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;
using Omnae.Model.Context;
using Omnae.WebApi.Util;
using Serilog;
using Omnae.Data;
using Omnae.Model.Extentions;
using static Omnae.Data.Query.OrderQuery;

namespace Omnae.WebApi.Controllers
{
    /// <summary>
    /// Core Api for Orders 
    /// </summary>
    [EnableCors("*", "*", "*")]
    [RoutePrefix("api/Orders")]
    public class OrdersController : BaseApiController
    {
        private readonly CompanyAccountsBL companyBl;
        private readonly IOrderService orderService;
        private readonly OrdersBL ordersBL;
        private readonly IHomeBL homeBL;
        private readonly ITaskDataService taskDataService;
        private readonly IDocumentService documentService;
        private readonly IPriceBreakService priceBreakService;
        private readonly IOrderStateTrackingService orderStateTrackingService;
        private readonly ProductBL productBL;
        private readonly ILogedUserContext userContext;
        private readonly TaskSetup taskSetup;
        private readonly DocumentBL documentBL;
        private readonly IProductService productService;
        private readonly ICompaniesCreditRelationshipService companiesCreditRelationshipService;
        private readonly IOmnaeInvoiceService omnaeInvoiceService;
        private readonly UserContactService userContactService;
        private readonly NotificationBL notificationBL;
        private readonly IProductSharingService productSharingService;
        private readonly IExpeditedShipmentRequestService expeditedShipmentRequestService;

        private INCReportService nCReportService { get; }
        private ICompanyService companyService { get; }
        private IMapper Mapper { get; }

        /// <summary>
        /// Constructor for Orders controller 
        /// </summary>
        public OrdersController(ILogger log, CompanyAccountsBL companyBl, IOrderService orderService, OrdersBL ordersBl, IHomeBL homeBl, ITaskDataService taskDataService, IDocumentService documentService, IPriceBreakService priceBreakService, IOrderStateTrackingService orderStateTrackingService, ProductBL productBl, ILogedUserContext userContext, TaskSetup taskSetup, DocumentBL documentBl, IProductService productService, INCReportService nCReportService, ICompanyService companyService, IMapper mapper,
            ICompaniesCreditRelationshipService companiesCreditRelationshipService, IOmnaeInvoiceService omnaeInvoiceService, UserContactService userContactService,
            NotificationBL notificationBL, IProductSharingService productSharingService,
            IExpeditedShipmentRequestService expeditedShipmentRequestService) : base(log)
        {
            this.companyBl = companyBl;
            this.orderService = orderService;
            ordersBL = ordersBl;
            homeBL = homeBl;
            this.taskDataService = taskDataService;
            this.documentService = documentService;
            this.priceBreakService = priceBreakService;
            this.orderStateTrackingService = orderStateTrackingService;
            productBL = productBl;
            this.taskSetup = taskSetup;
            documentBL = documentBl;
            this.productService = productService;
            this.nCReportService = nCReportService;
            this.companyService = companyService;
            Mapper = mapper;
            this.userContext = userContext;
            this.companiesCreditRelationshipService = companiesCreditRelationshipService;
            this.omnaeInvoiceService = omnaeInvoiceService;
            this.userContactService = userContactService;
            this.notificationBL = notificationBL;
            this.productSharingService = productSharingService;
            this.expeditedShipmentRequestService = expeditedShipmentRequestService;
        }

        /// <summary>
        /// Get all orders 
        /// </summary>
        /// <param name="search">key word for searching</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [HttpGet]
        [Route("", Name = "GetOrders")]
        public async Task<PagedResultSet<OrderDTO>> Get(string search = null, int? page = 1, int pageSize = PageSize, string orderBy = nameof(OrderDTO._updatedAt), bool ascending = false)
        {
            var orders = orderService.FindOrderList().Where(x => x.TaskId != null && x.Quantity > x.Product.PriceBreak.NumberSampleIncluded);
            if (search != null)
            {
                orders = SearchOrders(orders, search);
            }
            return await PageOfResultsSetAsync<Order, OrderDTO>(orders, page, pageSize, orderBy, ascending, "GetOrders");
        }

        /// <summary>
        /// List all orders 
        /// </summary>
        /// <param name="companyId">Company ID.</param>
        /// <param name="mode">User mode: Customer/Vendor</param>
        /// <param name="type">Order filter</param>
        /// <param name="filter">key word for searching</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [HttpGet]
        [Route("", Name = "ListOrders")]
        public async Task<PagedResultSet<OrderDTO>> ListOrders(int? companyId,
                                                               OrderQuery.UserMode mode = OrderQuery.UserMode.Customer,
                                                               OrderQuery.OrderFilter type = OrderQuery.OrderFilter.Current,
                                                               string filter = null,
                                                               int? page = 1, int pageSize = PageSize,
                                                               string orderBy = nameof(OrderDTO._updatedAt),
                                                               bool ascending = false)
        {
            IQueryable<Order> orders = Enumerable.Empty<Order>().AsQueryable();
            if (companyId == null)
            {
                orders = orderService.FindOrderList();
            }
            else
            {
                orders = mode == OrderQuery.UserMode.Customer ?
                    orderService.FindOrdersAndSharedByCustomerId(companyId.Value) :
                    orderService.FindOrdersByVendorId(companyId.Value);

                orders = orders.FilterBy(companyId.Value, type, mode, filter);

            }
            return await PageOfResultsSetAsync<Order, OrderDTO>(orders, page, pageSize, orderBy, ascending, "ListOrders");
        }


        /// <summary>
        /// Get order which id = {id}
        /// </summary>
        /// <param name="id">The id of the order.</param>
        /// <param name="mode">Customer or Vendor</param>
        [HttpGet]
        [Route("{id:int}")]
        [ResponseType(typeof(OrderDTO))]
        public async Task<IHttpActionResult> Get(int id, UserMode mode = UserMode.Customer)
        {
            var order = orderService.FindOrderById(id);

            if (order == null)
                return NotFound();

            if (order?.TaskId == null)
                return BadRequest(IndicatingMessages.OrderNotFound);

            var dto = Mapper.Map<OrderDTO>(order);
            dto.RejectReason = order.TaskData.RejectReason;
            dto.State = (States)order.TaskData.StateId;
            dto.IsRiskBuild = order.TaskData.IsRiskBuild;
            var docs = documentService.FindDocumentByProductId(order.ProductId).Where(d => d.TaskId == order.TaskId);

            // Doc Uri
            dto.ProductDrawingUri = docs.Where(d => d.DocType == (int)DOCUMENT_TYPE.PRODUCT_2D_PDF || d.DocType == (int)DOCUMENT_TYPE.PRODUCT_3D_STEP)
                                        .Select(x => x.DocUri).ToArray();
            dto.VendorQuoteUri = docs.Where(d => d.DocType == (int)DOCUMENT_TYPE.QUOTE_PDF && d.TaskData != null && d.TaskData.RFQBidId != null && d.TaskData.RFQBid.IsActive == true)
                                     .Select(x => x.DocUri).ToArray();
            dto.PODocUri = docs.Where(d => d.DocType == (int)DOCUMENT_TYPE.PO_PDF).Select(x => x.DocUri).ToArray();
            dto.ProofDocUri = docs.Where(d => d.DocType == (int)DOCUMENT_TYPE.PROOF_PDF).Select(x => x.DocUri).ToArray();
            dto.ProofRejectDocUri = docs.Where(d => d.DocType == (int)DOCUMENT_TYPE.CORRESPOND_PROOF_REJECT_PDF)
                                        .Select(x => x.DocUri).ToArray();
            dto.SampleRejectDocUri = docs.Where(d => d.DocType == (int)DOCUMENT_TYPE.CORRESPOND_SAMPLE_REJECT_PDF)
                                         .Select(x => x.DocUri).ToArray();
            dto.InspectionReportDocUri = docs.Where(d => d.DocType == (int)DOCUMENT_TYPE.PACKING_SLIP_INSPECTION_REPORT_PDF)
                                             .Select(x => x.DocUri).ToArray();
            dto.PaymentDocUri = docs.Where(d => d.DocType == (int)DOCUMENT_TYPE.PAYMENT_PROOF)
                                        .Select(x => x.DocUri).ToArray();

            dto.PackingSlipDocUri = docs.Where(d => d.DocType == (int)DOCUMENT_TYPE.PACKING_SLIP_PDF)
                                        .Select(x => x.DocUri).ToArray();

            dto.VendorInvoice = docs.Where(d => d.DocType == (int)DOCUMENT_TYPE.ENTERPRISE_VENDOR_INVOICE_PDF)
                                    .Select(x => x.DocUri).ToArray();

            var priceBreaks = (await priceBreakService.FindPriceBreaksByTaskId(order.TaskId.Value).Where(x => x.RFQBid.IsActive == true || x.RFQBid == null).OrderBy(x => x.Quantity).ToListAsync()).ToNullIfEmpty()
                              ?? (await priceBreakService.FindPriceBreaksByProductId(order.ProductId).Where(x => x.RFQBid.IsActive == true || x.RFQBid == null).OrderBy(x => x.Quantity).ToListAsync());
            var pb = priceBreaks.LastOrDefault(x => x.Quantity <= order.Quantity) ?? priceBreaks.FirstOrDefault();

            if (mode == UserMode.Customer)
            {
                dto.ToolingCharge = pb?.CustomerToolingSetupCharges;
            }
            else if (mode == UserMode.Vendor)
            {
                dto.UnitPrice = order.UnitPrice != null ? pb?.VendorUnitPrice : (decimal?)null;
                dto.ToolingCharge = pb?.ToolingSetupCharges;

                if (order.TaskData.isEnterprise == false)
                {
                    double taxRate = 0;
                    var company = order.Product.VendorCompany;
                    Address addr = company.Shipping.Address;
                    if (addr.StateProvince != null)
                    {
                        taxRate = (double)await productBL.GetTaxRateValue(addr.Country.CountryName, addr.StateProvince.Abbreviation);
                        taxRate /= 100;
                    }
                    dto.SalesTax = (decimal)taxRate * (dto.Quantity * (dto.UnitPrice ?? 0) + (dto.ToolingCharge ?? 0));
                    dto.SalesPrice = dto.SalesTax + dto.Quantity * (dto.UnitPrice ?? 0) + (dto.UnitPrice == null ? (dto.ToolingCharge ?? 0) : 0);
                }
            }

            return Ok(dto);
        }

        /// <summary>
        /// Get order which id = {id}
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="id">The id of the order.</param>
        /// <param name="mode">Customer or Vendor</param>
        [HttpGet]
        [Route("user/{userId}/Get/{id:int}")]
        [ResponseType(typeof(OrderDTO))]
        public async Task<IHttpActionResult> Get(string userId, int id, UserMode mode = UserMode.Customer)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest(IndicatingMessages.InvalidAccess);

            var order = await orderService.FindOrderByCompanyId(userContext.Company.Id).Where(x => x.Id == id).FirstOrDefaultAsync();
            if (order == null || order.TaskData == null || order.Product == null)
            {
                return BadRequest(IndicatingMessages.OrderNotFound);
            }
            OrderDTO dto = Mapper.Map<OrderDTO>(order);
            var docs = documentService.FindDocumentByProductId(order.ProductId);

            // Doc Uri
            dto.ProductDrawingUri = docs.Where(d => d.DocType == (int)DOCUMENT_TYPE.PRODUCT_2D_PDF || d.DocType == (int)DOCUMENT_TYPE.PRODUCT_3D_STEP)
                                        .Select(x => x.DocUri).ToArray();
            dto.VendorQuoteUri = docs.Where(d => d.DocType == (int)DOCUMENT_TYPE.QUOTE_PDF && d.TaskData != null && d.TaskData.RFQBidId != null && d.TaskData.RFQBid.IsActive == true)
                                     .Select(x => x.DocUri).ToArray();
            dto.PODocUri = docs.Where(d => d.DocType == (int)DOCUMENT_TYPE.PO_PDF).Select(x => x.DocUri).ToArray();
            dto.ProofDocUri = docs.Where(d => d.DocType == (int)DOCUMENT_TYPE.PROOF_PDF).Select(x => x.DocUri).ToArray();
            dto.ProofRejectDocUri = docs.Where(d => d.DocType == (int)DOCUMENT_TYPE.CORRESPOND_PROOF_REJECT_PDF)
                                        .Select(x => x.DocUri).ToArray();
            dto.SampleRejectDocUri = docs.Where(d => d.DocType == (int)DOCUMENT_TYPE.CORRESPOND_SAMPLE_REJECT_PDF)
                                         .Select(x => x.DocUri).ToArray();
            dto.InspectionReportDocUri = docs.Where(d => d.DocType == (int)DOCUMENT_TYPE.PACKING_SLIP_INSPECTION_REPORT_PDF)
                                             .Select(x => x.DocUri).ToArray();
            dto.PaymentDocUri = docs.Where(d => d.DocType == (int)DOCUMENT_TYPE.PAYMENT_PROOF)
                                        .Select(x => x.DocUri).ToArray();

            dto.PackingSlipDocUri = docs.Where(d => d.DocType == (int)DOCUMENT_TYPE.PACKING_SLIP_PDF)
                                        .Select(x => x.DocUri).ToArray();

            dto.VendorInvoice = docs.Where(d => d.DocType == (int)DOCUMENT_TYPE.ENTERPRISE_VENDOR_INVOICE_PDF)
                                    .Select(x => x.DocUri).ToArray();


            var priceBreaks = (await priceBreakService.FindPriceBreaksByTaskId(order.TaskId.Value).Where(x => x.RFQBid.IsActive == true || x.RFQBid == null).OrderBy(x => x.Quantity).ToListAsync()).ToNullIfEmpty()
                              ?? (await priceBreakService.FindPriceBreaksByProductId(order.ProductId).Where(x => x.RFQBid.IsActive == true || x.RFQBid == null).OrderBy(x => x.Quantity).ToListAsync());
            var pb = priceBreaks.LastOrDefault(x => x.Quantity <= order.Quantity) ?? priceBreaks.FirstOrDefault();

            if (mode == UserMode.Customer)
            {
                dto.ToolingCharge = pb?.CustomerToolingSetupCharges;
            }
            else if (mode == UserMode.Vendor)
            {
                dto.UnitPrice = order.UnitPrice != null ? pb?.VendorUnitPrice : (decimal?)null;
                dto.ToolingCharge = pb?.ToolingSetupCharges;

                if (order.TaskData.isEnterprise == false)
                {
                    double taxRate = 0;
                    var company = order.Product.VendorCompany;
                    Address addr = company.Shipping.Address;
                    if (addr.StateProvince != null)
                    {
                        taxRate = (double)await productBL.GetTaxRateValue(addr.Country.CountryName, addr.StateProvince.Abbreviation);
                        taxRate /= 100;
                    }
                    dto.SalesTax = (decimal)taxRate * (dto.Quantity * (dto.UnitPrice ?? 0) + (dto.ToolingCharge ?? 0));
                    dto.SalesPrice = dto.SalesTax + dto.Quantity * (dto.UnitPrice ?? 0) + (dto.UnitPrice == null ? (dto.ToolingCharge ?? 0) : 0);
                }
            }

            return Ok(dto);
        }


        /// <summary>
        /// Get customer's all orders with payment pending 
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="search">key word for searching</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Numbe of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [HttpGet]
        [Route("customer/{userId}/GetCustomerAllPaymentPendingOrders", Name = "GetCustomerAllPaymentPendingOrders")]
        public async Task<PagedResultSet<OrderDTO>> GetCustomerAllPaymentPendingOrders(string userId, string search = null, int? page = 1, int pageSize = PageSize, string orderBy = nameof(OrderDTO._updatedAt), bool ascending = false)
        {
            PagedResultSet<OrderDTO> ordersDTO = new PagedResultSet<OrderDTO>();
            if (userContext.User == null || userContext.Company == null)
                return ordersDTO;

            var orders = orderService.FindOrderByCompanyId(userContext.Company.Id)
                .Where(x => x.TaskId != null && x.Quantity > x.Product.PriceBreak.NumberSampleIncluded && x.TaskData.StateId == (int)States.OrderInitiated);

            if (search != null)
            {
                orders = SearchOrders(orders, search);
            }
            return await PageOfResultsSetAsync<Order, OrderDTO>(orders, page, pageSize, orderBy, ascending, "GetCustomerAllPaymentPendingOrders");
        }

        /// <summary>
        /// Get vendor's all orders with payment need to be confirmed 
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="search">key word for searching</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Numbe of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [HttpGet]
        [Route("vendor/{userId}/GetVendorAllPaymentUnConfirmedOrders", Name = "GetVendorAllPaymentUnConfirmedOrders")]
        public async Task<PagedResultSet<OrderDTO>> GetVendorAllPaymentUnConfirmedOrders(string userId, string search = null, int? page = 1, int pageSize = PageSize, string orderBy = nameof(OrderDTO._updatedAt), bool ascending = false)
        {
            PagedResultSet<OrderDTO> ordersDTO = new PagedResultSet<OrderDTO>();
            if (userContext.User == null || userContext.Company == null)
                return ordersDTO;

            var orders = orderService.FindOrderByCompanyId(userContext.Company.Id)
                    .Where(x => x.TaskId != null && x.Quantity > x.Product.PriceBreak.NumberSampleIncluded && x.TaskData.StateId == (int)States.PaymentMade);

            if (search != null)
            {
                orders = SearchOrders(orders, search);
            }
            return await PageOfResultsSetAsync<Order, OrderDTO>(orders, page, pageSize, orderBy, ascending, "GetVendorAllPaymentUnConfirmedOrders");
        }


        /// <summary>
        /// Get company's orders which companyId = {companyId}
        /// </summary>
        /// <param name="companyId">Company Id.</param>
        /// <param name="search">key word for searching</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Numbe of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [HttpGet]
        [Route("company/{companyId:int}", Name = "GetCompanyOrders")]
        public async Task<PagedResultSet<OrderDTO>> GetCompanyOrders(int companyId, string search = null, int? page = 1, int pageSize = PageSize, string orderBy = nameof(OrderDTO._updatedAt), bool ascending = false)
        {
            var orders = orderService.FindOrderByCompanyId(companyId);
            if (search != null)
            {
                orders = SearchOrders(orders, search);
            }
            return await PageOfResultsSetAsync<Order, OrderDTO>(orders, page, pageSize, orderBy, ascending, "GetCompanyOrders");
        }

        /// <summary>
        /// Get user's orders which user id = {userId}
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="search">key word for searching</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Numbe of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [HttpGet]
        [Route("user/{userId}", Name = "GetUserOrders")]

        public async Task<PagedResultSet<OrderDTO>> GetUserOrders(string userId, string search = null, int? page = 1, int pageSize = PageSize, string orderBy = nameof(OrderDTO._updatedAt), bool ascending = false)
        {
            PagedResultSet<OrderDTO> ordersDTO = new PagedResultSet<OrderDTO>();
            if (userContext.User == null || userContext.Company == null)
                return ordersDTO;

            var orders = orderService.FindOrderByCompanyId(userContext.Company.Id).Where(x => x.TaskId != null && x.PartNumber != null);
            if (search != null)
            {
                orders = SearchOrders(orders, search);
            }
            return await PageOfResultsSetAsync<Order, OrderDTO>(orders, page, pageSize, orderBy, ascending, "GetUserOrders");
        }

        /// <summary>
        /// Get a customer's orders
        /// </summary>
        /// <param name="companyId">Company ID.</param>
        /// <param name="search">key word for searching</param>
        /// <param name="filter">Filter products into first-time order or reorder or non-filter</param>
        /// <param name="type">User type</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Numbe of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [HttpGet]
        [Route("customer/{companyId:int}/Orders", Name = "GetCustomerOrders")]
        public async Task<PagedResultSet<OrderDTO>> GetCustomerOrders([Required] int companyId, string search = null, OrderQuery.OrderFilter filter = OrderQuery.OrderFilter.Current, OrderQuery.UserMode type = OrderQuery.UserMode.Customer, int? page = 1, int pageSize = PageSize, string orderBy = nameof(OrderDTO._updatedAt), bool ascending = false)
        {
            var orders = orderService.FindOrdersAndSharedByCustomerId(companyId).FilterBy(companyId, filter, type, search);
            if (search != null)
            {
                orders = SearchOrders(orders, search);
            }
            return await PageOfResultsSetAsync<Order, OrderDTO>(orders, page, pageSize, orderBy, ascending, "GetCustomerOrders");
        }

        /// <summary>
        /// Get a vendor's orders
        /// </summary>
        /// <param name="companyId">Company ID.</param>
        /// <param name="search">key word for searching</param>
        /// <param name="filter">Filter products into first-time order or reorder or non-filter</param>
        /// <param name="type">User type</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Numbe of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [HttpGet]
        [Route("vendor/{companyId:int}/Orders", Name = "GetVendorOrders")]
        public async Task<PagedResultSet<OrderDTO>> GetVendorOrders([Required] int companyId, string search = null, OrderQuery.OrderFilter filter = OrderQuery.OrderFilter.Current, OrderQuery.UserMode type = OrderQuery.UserMode.Vendor, int? page = 1, int pageSize = PageSize, string orderBy = nameof(OrderDTO._updatedAt), bool ascending = false)
        {
            var orders = orderService.FindOrdersByVendorId(companyId).FilterBy(companyId, filter, type, search);
            if (search != null)
            {
                orders = SearchOrders(orders, search);
            }
            return await PageOfResultsSetAsync<Order, OrderDTO>(orders, page, pageSize, orderBy, ascending, "GetVendorOrders");
        }

        /// <summary>
        /// Calculate tax, subtotal, total on given quantity
        /// </summary>
        /// <param name="pid">Product ID.</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="isReorder">isReorder</param>
        /// <param name="taxRate">Tax rate</param>
        [ResponseType(typeof(CalculateResultViewModel))]
        [HttpGet]
        [Route("customer/{userId}/CalculateTotalOnGivenQuantity/{pid:int}", Name = "CalculateTotalOnGivenQuantity")]
        public async Task<IHttpActionResult> CalculateTotalOnGivenQuantity(int pid, int quantity, bool? isReorder = false, double? taxRate = null)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest("Invalid User or Customer");

            var product = productService.FindProductById(pid);
            if (!productBL.IsUsersProduct(product))
            {
                return BadRequest(IndicatingMessages.InvalidAccess);
            }

            var td = taskDataService.FindTaskDataListByProductId(pid).CanPlaceOrder();
            if (td == null)
            {
                return BadRequest(IndicatingMessages.TaskNotFound);
            }
            var model = await productBL.CalculateAsync(pid, td.TaskId, quantity, isReorder, taxRate, true);
            return Ok(model);
        }

        /// <summary>
        /// Place a new order
        /// </summary>
        [ResponseType(typeof(OrderDTO))]
        [HttpPost]
        [Route("customer/{userId}/placeorder/{pid:int}", Name = "PlaceOrder")]
        public async Task<IHttpActionResult> PlaceOrder(int pid)
        {
            if (userContext.User == null || userContext.Company == null)
            {
                return BadRequest("Invalid User or Customer");
            }
            var product = productService.FindProductById(pid);
            ProductSharing ps = null;
            TaskData td = null;
            if (!productBL.IsUsersProduct(product))
            {
                return BadRequest(IndicatingMessages.InvalidAccess);
            }
            int currentCompanyId = userContext.Company.Id;
            if (product.CustomerId != currentCompanyId)
            {
                ps = productSharingService.FindProductSharingByCompanyIdProductId(currentCompanyId, pid);
                if (ps == null || ps.IsRevoked == true || ps.HasPermissionToOrder == false)
                {
                    return BadRequest(IndicatingMessages.YouHaveNoPermissionToOrderThisPart);
                }
                td = taskDataService.FindById(ps.TaskId.Value);
            }
            else
            {
                td = taskDataService.FindTaskDataListByProductId(pid).CanPlaceOrder();
            }
            if (td == null)
            {
                return BadRequest(IndicatingMessages.TaskNotFound);
            }

            var httpRequest = HttpContext.Current.Request;
            var formDataDic = httpRequest.Form.ToDictionaryOfObjects();
            if (formDataDic.ContainsKey("DesireShippingDate") && !DateTime.TryParse(formDataDic.GetOrNull("DesireShippingDate").ToString(), out var dt1))
            {
                return BadRequest("Invalid DesireShippingDate");
            }
            if (formDataDic.ContainsKey("EarliestShippingDate") && !DateTime.TryParse(formDataDic.GetOrNull("EarliestShippingDate").ToString(), out var dt2))
            {
                return BadRequest("Invalid EarliestShippingDate");
            }
            if (formDataDic.ContainsKey("ShipDate") && !DateTime.TryParse(formDataDic.GetOrNull("ShipDate").ToString(), out var dt3))
            {
                return BadRequest("Invalid ShipDate");
            }

            var model = Slapper.AutoMapper.Map<PlaceOrderViewModel>(formDataDic, false);
            if (model.IsReorder && !td.CanReorder())

            {
                return BadRequest(IndicatingMessages.TaskStateMismatch);
            }
            if (!model.HaveAddrInformation)
            {
                return BadRequest(IndicatingMessages.MissingFormData);
            }

            var files = GetPostedFiles(homeBL);

            try
            {
                model.PurchaseOrder = files.ToArray();
                model.ProductId = pid;
                model.TaskId = td.TaskId;
                model.StateId = td.StateId;
                model.isEnterprise = td.isEnterprise;
                model.ToolingCharges = td.Product.PriceBreak?.ToolingSetupCharges;
                model.IsNewWorkflow = true;

                CalculateResultViewModel res = await productBL.CalculateAsync(pid, td.TaskId, model.Quantity, model.IsReorder, model.TaxRate, model.IsNewWorkflow);
                model.SalesTax = res.SalesTax;
                model.UnitPrice = res.UnitPrice;
                model.ToolingCharges = res.ToolingCharges;
                model.Total = res.Total;
                model.TaxRatePercentage = res.TaxRatePercentage;
                model.OrderCompanyId = currentCompanyId;

                var priceBreak = priceBreakService.FindPriceBreakByProductId(pid).FirstOrDefault(x => x.RFQBidId > 0 && x.RFQBid.VendorId == product.VendorId);
                model.NumberSampleIncluded = priceBreak?.NumberSampleIncluded ?? 1;
                model.IsReorder = ps != null || model.IsReorder;

                if (model.IsForRiskBuild == true)
                {
                    td.IsRiskBuild = true;
                    td.ModifiedUtc = DateTime.UtcNow;
                    td.UpdatedBy = userContext.User.UserName;
                    td.ModifiedByUserId = userContext.UserId;
                    taskDataService.Update(td);
                }
                

                var controller = HomeController.CreateController<HomeController>();
                CreateOrderResult result = await ordersBL.CreateOrder(model, controller.ControllerContext);

                if (!string.IsNullOrWhiteSpace(result.Message))
                {
                    return InternalServerError(new Exception(result.Message));
                }
                if (result.OrderId == 0)
                {
                    return BadRequest(IndicatingMessages.DuplicateOrder);
                }

                var order = orderService.FindOrderById(result.OrderId);
                string error = string.Empty;
                CompaniesCreditRelationship credit = null;
                if (td.isEnterprise)
                {
                    credit = companiesCreditRelationshipService.FindCompaniesCreditRelationshipByCustomerIdVendorId(product.CustomerId.Value, product.VendorId.Value);
                }
                if (order.IsReorder == false)
                {
                    var orders = orderService.FindOrdersByProductId(pid).GroupBy(x => x.TaskId).Select(t => t.FirstOrDefault());
                    foreach (var ord in orders)
                    {
                        if (ord.TaskId != null)
                        {
                            if (credit != null && credit.isTerm)
                            {
                                error = ordersBL.PostPlaceOrderActionWithTerm(ord, order.CustomerPONumber, false);
                            }
                            else
                            {
                                error = ordersBL.PostPlaceOrderAction(ord, order.CustomerPONumber, false);
                            }
                        }
                    }
                    if (error != null)
                    {
                        return CreatedAtRoute("PlaceOrder", new { id = result.OrderId }, error);
                    }
                }
                else
                {
                    if (credit != null && credit.isTerm)
                    {
                        error = ordersBL.PostPlaceOrderActionWithTerm(order, order.CustomerPONumber, true);
                    }
                    else
                    {
                        error = ordersBL.PostPlaceOrderAction(order, order.CustomerPONumber, true);
                    }
                }

                var orderDTO = Mapper.Map<OrderDTO>(order);
                var docs = documentService.FindDocumentByProductId(model.ProductId);
                orderDTO.VendorPODocUri = docs.Where(x => x.DocType == (int)DOCUMENT_TYPE.QBO_PURCHASEORDER_PDF).Select(d => d.DocUri).ToArray();
                orderDTO.PODocUri = docs.Where(x => x.DocType == (int)DOCUMENT_TYPE.PO_PDF).Select(d => d.DocUri).ToArray();

                return CreatedAtRoute("PlaceOrder", new { id = result.OrderId }, orderDTO);
            }
            catch (ValidationException ve)
            {
                return BadRequest(ve.Message);
            }
            finally
            {
                DeleteUploadFiles(files);
            }
        }


        /// <summary>
        /// Start to do Product Run by vendor
        /// </summary>
        [ResponseType(typeof(OrderDTO))]
        [HttpPost]
        [Route("vendor/{userId}/DoProductionRun/{pid:int}", Name = "DoProductionRun")]
        public async Task<IHttpActionResult> DoProductionRun(int pid)
        {
            if (userContext.User == null || userContext.Company == null)
            {
                return BadRequest("Invalid User or Vendor");
            }
            var product = productService.FindProductById(pid);
            ProductSharing ps = null;
            TaskData td = null;
            if (!productBL.IsUsersProduct(product))
            {
                return BadRequest(IndicatingMessages.InvalidAccess);
            }
            if (product.VendorId == null)
            {
                return BadRequest(IndicatingMessages.ProductNotAssignedYet);
            }
            int currentCompanyId = userContext.Company.Id;
            var customerId = (int)product.CustomerId;
            if (product.VendorId != currentCompanyId)
            {
                ps = productSharingService.FindProductSharingByCompanyIdProductId(currentCompanyId, pid);
                if (ps == null || ps.IsRevoked == true || ps.HasPermissionToOrder == false)
                {
                    return BadRequest(IndicatingMessages.YouHaveNoPermissionToOrderThisPart);
                }
                td = taskDataService.FindById(ps.TaskId.Value);
            }
            else
            {
                td = taskDataService.FindTaskDataListByProductId(pid).CanPlaceOrder();
            }
            if (td == null)
            {
                return BadRequest(IndicatingMessages.TaskNotFound);
            }

            
            

            var httpRequest = HttpContext.Current.Request;
            var formDataDic = httpRequest.Form.ToDictionaryOfObjects();
            bool isInStock = false;
            if (formDataDic.ContainsKey("isInStock"))
            {
                isInStock = bool.Parse(formDataDic.GetOrNull("isInStock").ToString());
            }

            // check if the product to be ordered is alread in stock
            if (isInStock && td.isEnterprise == true)
            {
                td.StateId = (int)States.ProductionComplete;
                td.ModifiedUtc = DateTime.UtcNow;
                td.UpdatedBy = userContext.User.UserName;
                td.ModifiedByUserId = userContext.UserId;
                taskDataService.Update(td);

            }
      

            if (formDataDic.ContainsKey("DesireShippingDate") && !DateTime.TryParse(formDataDic.GetOrNull("DesireShippingDate").ToString(), out var dt1))
            {
                return BadRequest("Invalid DesireShippingDate");
            }
            if (formDataDic.ContainsKey("EarliestShippingDate") && !DateTime.TryParse(formDataDic.GetOrNull("EarliestShippingDate").ToString(), out var dt2))
            {
                return BadRequest("Invalid EarliestShippingDate");
            }
            if (formDataDic.ContainsKey("ShipDate") && !DateTime.TryParse(formDataDic.GetOrNull("ShipDate").ToString(), out var dt3))
            {
                return BadRequest("Invalid ShipDate");
            }

            var model = Slapper.AutoMapper.Map<PlaceOrderViewModel>(formDataDic, false);
            if (model.IsReorder && !td.CanReorder())

            {
                return BadRequest(IndicatingMessages.TaskStateMismatch);
            }
            if (!model.HaveAddrInformation)
            {
                return BadRequest(IndicatingMessages.MissingFormData);
            }

            var files = GetPostedFiles(homeBL);

            try
            {
                model.PurchaseOrder = files.ToArray();
                model.ProductId = pid;
                model.TaskId = td.TaskId;
                model.StateId = td.StateId;
                model.isEnterprise = true; // td.isEnterprise;
                model.ToolingCharges = td.Product.PriceBreak?.ToolingSetupCharges;

                CalculateResultViewModel res = await productBL.CalculateAsync(pid, td.TaskId, model.Quantity, model.IsReorder, model.TaxRate, true);
                model.SalesTax = res.SalesTax;
                model.UnitPrice = res.UnitPrice;
                model.ToolingCharges = res.ToolingCharges;
                model.Total = res.Total;
                model.TaxRatePercentage = res.TaxRatePercentage;
                model.OrderCompanyId = customerId;

                var priceBreak = priceBreakService.FindPriceBreakByProductId(pid).FirstOrDefault();
                if (priceBreak == null)
                    return BadRequest(IndicatingMessages.PriceBreaksNotFound);

                model.NumberSampleIncluded = priceBreak?.NumberSampleIncluded ?? 1;
                model.IsReorder = ps != null || model.IsReorder;

                if (model.IsForRiskBuild == true)
                {
                    td.IsRiskBuild = true;
                    td.ModifiedUtc = DateTime.UtcNow;
                    td.UpdatedBy = userContext.User.UserName;
                    td.ModifiedByUserId = userContext.UserId;
                    taskDataService.Update(td);
                }
                model.IsNewWorkflow = true;

                var controller = HomeController.CreateController<HomeController>();
                CreateOrderResult result = await ordersBL.CreateOrder(model, controller.ControllerContext);

                if (!string.IsNullOrWhiteSpace(result.Message))
                {
                    return InternalServerError(new Exception(result.Message));
                }
                if (result.OrderId == 0)
                {
                    return BadRequest(IndicatingMessages.DuplicateOrder);
                }

                var order = orderService.FindOrderById(result.OrderId);
                string error = string.Empty;

                List<Order> orderList = new List<Order>();
                if (order.IsReorder == false)
                {
                    var orders = orderService.FindOrdersByProductId(pid);
                    foreach (var ord in orders)
                    {
                        if (ord.TaskId != null)
                        {
                            ordersBL.PostDoProductionRun(ord, order.CustomerPONumber, false, isInStock);
                            orderList.Add(ord);
                        }
                    }
                }
                else
                {
                    ordersBL.PostDoProductionRun(order, order.CustomerPONumber, true, isInStock);
                    orderList.Add(order);
                }


                var orderDtoList = Mapper.Map<List<OrderDTO>>(orderList);
                foreach (var orderDto in orderDtoList)
                {
                    var docs = documentService.FindDocumentByProductId(model.ProductId);
                    orderDto.VendorPODocUri = docs.Where(x => x.DocType == (int)DOCUMENT_TYPE.QBO_PURCHASEORDER_PDF).Select(d => d.DocUri).ToArray();
                    orderDto.PODocUri = docs.Where(x => x.DocType == (int)DOCUMENT_TYPE.PO_PDF).Select(d => d.DocUri).ToArray();
                }

                return CreatedAtRoute("DoProductionRun", new { id = result.OrderId }, orderDtoList);
            }
            catch (ValidationException ve)
            {
                return BadRequest(ve.Message);
            }
            finally
            {
                DeleteUploadFiles(files);
            }
        }


#if false
        /// <summary>
        /// Pay With Cheque
        /// </summary>
        /// <param name="orderId">Order Id</param>
        [HttpPost]
        [Route("customer/{userId}/PayWithNonTerm/{orderId:int}", Name = "PayWithNonTerm")]
        public IHttpActionResult PayWithNonTerm(int orderId)
        {
            var order = orderService.FindOrderById(orderId);
            TaskData td = order.TaskData;
            if (td.StateId != (int)States.QuoteAccepted && td.StateId != (int)States.ProductionComplete)
            {
                return BadRequest(IndicatingMessages.TaskNotFound);
            }
            if (order == null)
            {
                return BadRequest(IndicatingMessages.OrderNotFound);
            }
            if (order.TaskId == null)
            {
                return BadRequest(IndicatingMessages.TaskNotFound);
            }
            try
            {
                List<HttpPostedFileBase> files = GetPostedFiles(homeBL);
                string error = ordersBL.PostPlaceOrderAction(order.TaskId.Value, order.CustomerPONumber, order.IsReorder);
                if (error != null)
                {
                    return BadRequest(error);
                }
                DeleteUploadFiles(files);
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.RetrieveErrorMessage());
            }
        }
#endif

        /// <summary>
        /// Made pending payment
        /// </summary>
        /// <param name="orderId">Order Id</param>
        [HttpPost]
        [Route("customer/{userId}/PendingPaymentMade/{orderId:int}", Name = "PendingPaymentMade")]
        public async Task<IHttpActionResult> PendingPaymentMade(int orderId)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest("Invalid User or Customer");


            var order = orderService.FindOrderById(orderId);
            if (order == null)
            {
                return BadRequest(IndicatingMessages.OrderNotFound);
            }
            if (!productBL.IsUsersProduct(order.Product))
            {
                return BadRequest(IndicatingMessages.InvalidAccess);
            }
            var orders = orderService.FindOrdersByProductId(order.ProductId);
            if (orders.Last().IsForToolingOnly == false && orders.Last().Id == orderId && orders.First().TaskData.StateId < (int)States.PaymentMade)
            {
                return BadRequest("You must pay sample order first");
            }

            TaskData td = order.TaskData;
            if (td.StateId != (int)States.OrderInitiated && td.StateId != (int)States.ReOrderInitiated)
            {
                return BadRequest(IndicatingMessages.TaskNotFound);
            }
            var files = GetPostedFiles(homeBL);
            if (files == null || files.Count == 0)
            {
                return BadRequest(IndicatingMessages.ForgotUploadFile);
            }
            StateTransitionViewModel model = new StateTransitionViewModel
            {
                group = td.StateId == (int)States.OrderInitiated ? Triggers.PendingPaymentMade.ToString() : Triggers.PendingReorderPaymentMade.ToString(),
                TaskData = td,
                UserType = USER_TYPE.Customer,
            };
            return await ProcessRequest(model, files);
        }

        /// <summary>
        /// Vendor rejected payment
        /// </summary>
        /// <param name="orderId">Order Id</param>
        /// <param name="rejectReason">Reject payment reason</param>
        [HttpPost]
        [Route("vendor/{userId}/RejectPayment/{orderId:int}", Name = "RejectPayment")]
        public async Task<IHttpActionResult> RejectPayment(int orderId, string rejectReason = null)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest("Invalid User or Customer");

            var order = orderService.FindOrderById(orderId);
            if (order == null)
            {
                return BadRequest(IndicatingMessages.OrderNotFound);
            }
            if (!productBL.IsUsersProduct(order.Product))
            {
                return BadRequest(IndicatingMessages.InvalidAccess);
            }
            TaskData td = order.TaskData;
            if (td.StateId != (int)States.PaymentMade && td.StateId != (int)States.ReOrderPaymentMade)
            {
                return BadRequest(IndicatingMessages.TaskNotFound);
            }
            using (var ts = AsyncTransactionScope.StartNew())
            {
                td.StateId = td.StateId == (int)States.PaymentMade ? (int)States.OrderInitiated : (int)States.ReOrderInitiated;
                td.RejectReason = rejectReason;
                taskDataService.Update(td);

                await SendEmailAysnc(td, USER_TYPE.Vendor);

                ts.Complete();
                return Ok();
            }
        }


        /// <summary>
        /// Order Paid
        /// </summary>
        /// <param name="orderId">Order Id</param>
        [HttpPost]
        [Route("vendor/{userId}/OrderPaid/{orderId:int}", Name = "OrderPaid")]
        public async Task<IHttpActionResult> OrderPaid(int orderId)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest("Invalid User or Customer");

            var order = orderService.FindOrderById(orderId);
            if (order == null)
            {
                return BadRequest(IndicatingMessages.OrderNotFound);
            }
            if (!productBL.IsUsersProduct(order.Product))
            {
                return BadRequest(IndicatingMessages.InvalidAccess);
            }
            TaskData td = order.TaskData;
            if (td.StateId != (int)States.PaymentMade && td.StateId != (int)States.ReOrderPaymentMade)
            {
                return BadRequest(IndicatingMessages.TaskNotFound);
            }


            //if (order.Quantity)
            StateTransitionViewModel model = new StateTransitionViewModel
            {
                group = td.StateId == (int)States.PaymentMade ? Triggers.OrderPaid.ToString() : Triggers.PaidReOrder.ToString(),
                TaskData = td,
                UserType = USER_TYPE.Vendor,
            };
            var result = await ProcessRequest(model);

            using (var trans = AsyncTransactionScope.StartNew())
            {
                if (order.IsReorder == false && order.IsForToolingOnly == false)
                {
                    // Find the order pairs. 
                    var orders = orderService.FindOrdersByProductId(order.ProductId).OrderBy(x => x.Quantity);
                    if (order.Quantity == orders.Last().Quantity && order.UnitPrice != null)
                    {
                        // current order is a quantity order, else do nothing
                        // Associate this order's task to the task of sample order
                        order.TaskId = orders.First().TaskId;
                        orderService.UpdateOrder(order);
                        var invoices = omnaeInvoiceService.FindOmnaeInvoiceByTaskId(td.TaskId);
                        foreach (var inv in invoices)
                        {
                            omnaeInvoiceService.DeleteOmnaeInvoice(inv);
                        }
                        taskDataService.DeleteTaskData(td);
                    }
                }
                trans.Complete();
            }
            return result;
        }


        /// <summary>
        /// Get price break on given quantity
        /// </summary>
        /// <param name="pid">Product Id</param>
        /// <param name="quantity">Quantity</param>
        [HttpGet]
        [Route("GetUnitPriceOnQty/{pid:int}", Name = "GetUnitPriceOnQty")]
        public IHttpActionResult GetUnitPriceOnQty(int pid, int quantity)
        {
            try
            {
                var moq = priceBreakService.FindMinimumOrderQuantity(pid);
                if (moq == null)
                    return BadRequest(IndicatingMessages.PriceBreaksNotFound);

                PriceBreak pb = null;
                if (quantity <= moq)
                {
                    pb = priceBreakService.FindPriceBreakByProductIdQty(pid, moq.Value);
                }
                else
                {
                    pb = priceBreakService.FindPriceBreakByProductIdQty(pid, quantity);
                }
                return Ok(pb?.UnitPrice);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.RetrieveErrorMessage());
            }
        }

        /// <summary>
        /// Get Order Details.
        /// </summary>
        /// <param name="id">Order Id</param>
        [HttpGet]
        [Route("user/{userId}/getorderdetails/{id}", Name = "GetOrderDetails")]
        public async Task<IHttpActionResult> GetOrderDetails(int id)
        {
            var order = orderService.FindOrderById(id);
            if (order == null)
            {
                return BadRequest(IndicatingMessages.OrderNotFound);
            }
            if (!productBL.IsUsersProduct(order.Product))
            {
                return BadRequest(IndicatingMessages.InvalidAccess);
            }
            var td = taskDataService.FindById(order.TaskId.Value);
            if (td == null)
            {
                return BadRequest(IndicatingMessages.TaskNotFound);
            }
            var docs = await documentService.FindDocumentsByTaskId(td.TaskId).ToListAsync();
            var stateTrackingList = orderStateTrackingService.FindOrderStateTrackingsByOrderId(id);
            var ncrReports = nCReportService.FindNCReportsByOrderId(id);

            OrderDetailsDTO dto = new OrderDetailsDTO
            {
                Order = Mapper.Map<OrderDTO>(order),
                Product = Mapper.Map<ProductDTO>(td.Product),
                PartRvision = Mapper.Map<PartRvisionDTO>(td.Product.PartRevision),
                Documents = Mapper.Map<List<DocumentDTO>>(docs),
                OrderStateTrackings = await stateTrackingList.ProjectTo<OrderStateTrackingDTO>(stateTrackingList).ToListAsync(),
                NCReports = await ncrReports.ProjectTo<NCReportDTO>(ncrReports).ToListAsync(),
            };
            dto.Order.State = dto.Product.State = (States)td.StateId;
            return Ok(dto);
        }


        /// <summary>
        /// Delete an existing order which id = {id}
        /// </summary>
        /// <param name="id">The id of the order.</param>
        [ResponseType(typeof(void))]
        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult Delete(int id)
        {
            var order = orderService.FindOrderById(id);
            if (order == null)
                return NotFound();
            if (!productBL.IsUsersProduct(order.Product))
            {
                return BadRequest(IndicatingMessages.InvalidAccess);
            }
            orderService.RemoveOrder(order);

            return Ok();
        }

        private bool OrderExists(int id)
        {
            return orderService.FindOrderById(id) != null;
        }

        /// <summary>
        /// Get All NCR History from a order which id = {id}
        /// </summary>
        /// <param name="id">The id of the order.</param>
        [HttpGet]
        [Route("{id:int}/ncr/")]
        [ResponseType(typeof(IList<NcrInfoViewModel>))]
        public IHttpActionResult GetNcrHistory(int id)
        {
            if (id < 0)
                return NotFound();

            var order = orderService.FindOrderList()
                .Where(x => x.Id == id)
                .Select(o => new { o.Id, o.ProductId })
                .SingleOrDefault();

            if (order == null)
                return NotFound();

            var ncrs = nCReportService.FindNCReportByProductIdOrderId(order.ProductId, order.Id);
            var mapped = ncrs.Select(g => new NcrInfoViewModel
            {
                Id = g.Id,
                NCRNumber = g.NCRNumber,
                DateInitiated = g.NCDetectedDate,
                RootCause = g.RootCause != null ? Enum.GetName(typeof(NC_ROOT_CAUSE), g.RootCause) : null,
                Vendor = g.VendorId != null ? companyService.FindCompanyById(g.VendorId.Value).Name : null,
                Cost = g.Cost,
                DateClosed = g.DateNcrClosed
            }).ToList();

            return Ok(mapped);
        }

#if false

        /// <summary>
        /// Vendor trigger OrderPaid.
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="pid">Product/RFQ Id</param>
        [HttpPost]
        [Route("vendor/{userId}/OrderPaid/{pid:int}")]
        public async Task<IHttpActionResult> OrderPaid(int pid)
        {
            TaskData td = taskDataService.FindTaskDataListByProductId(pid)
                .Where(x => x.StateId == (int)States.PaymentMade || x.StateId == (int)States.ReOrderPaymentMade)
                .LastOrDefault();

            if (td == null)
            {
                return BadRequest(IndicatingMessages.TaskNotFound);
            }
            StateTransitionViewModel model = new StateTransitionViewModel
            {
                group = Triggers.OrderPaid.ToString(),
                TaskData = td,
                UserType = USER_TYPE.Vendor,
            };
            return await ProcessRequest(model);
        }



        /// <summary>
        /// Vendor trigger StartedProof.
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="pid">Product/RFQ Id</param>
        [HttpPost]
        [Route("vendor/{userId}/StartedProof/{pid:int}")]
        public async Task<IHttpActionResult> StartedProof(string userId, int pid)
        {
            TaskData td = taskDataService.FindTaskDataListByProductId(pid)
                .Where(x => x.StateId == (int)States.OrderPaid || x.StateId == (int)States.ProofRejected)
                .LastOrDefault();

            if (td == null)
            {
                return BadRequest(IndicatingMessages.TaskNotFound);
            }
            StateTransitionViewModel model = new StateTransitionViewModel
            {
                group = Triggers.StartedProof.ToString(),
                TaskData = td,
                UserType = USER_TYPE.Vendor,
            };
            return await ProcessRequest(model);
        }
#endif

        /// <summary>
        /// Vendor trigger ProofApproval.
        /// </summary>
        /// <param name="orderId">Order Id</param>
        [HttpPost]
        [Route("vendor/{userId}/ProofApproval/{orderId:int}")]
        public async Task<IHttpActionResult> ProofApproval(int orderId)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest("Invalid User or Customer");

            var order = orderService.FindOrderById(orderId);
            if (order == null)
            {
                return BadRequest(IndicatingMessages.OrderNotFound);
            }
            if (!productBL.IsUsersProduct(order.Product))
            {
                return BadRequest(IndicatingMessages.InvalidAccess);
            }
            TaskData td = order.TaskData;
            if (!(td.StateId == (int)States.OrderPaid || td.StateId == (int)States.ProofingStarted || td.StateId == (int)States.ProofRejected))
            {
                return BadRequest(IndicatingMessages.TaskNotFound);
            }
            int pid = order.ProductId;
            if (!taskSetup.CheckPreconditions(pid, td.TaskId))
            {
                var files = GetPostedFiles(homeBL);
                try
                {
                    var result = documentBL.UploadProofingDoc(files, pid, td.TaskId);
                    if (!result)
                        return BadRequest(IndicatingMessages.UploadFileFailed);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.RetrieveErrorMessage());
                }
                finally
                {
                    DeleteUploadFiles(files);
                }
            }
            td.StateId = (int)States.ProofingStarted;
            StateTransitionViewModel model = new StateTransitionViewModel
            {
                group = Triggers.ProofApproval.ToString(),
                TaskData = td,
                UserType = USER_TYPE.Vendor,
            };

            return await ProcessRequest(model);
        }

        /// <summary>
        /// Customer trigger ApprovedProof.
        /// </summary>
        /// <param name="orderId">Order Id</param>
        [HttpPost]
        [Route("customer/{userId}/ApprovedProof/{orderId:int}")]
        public async Task<IHttpActionResult> ApprovedProof(int orderId)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest("Invalid User or Customer");

            var order = orderService.FindOrderById(orderId);
            if (order == null)
            {
                return BadRequest(IndicatingMessages.OrderNotFound);
            }
            if (!productBL.IsUsersProduct(order.Product))
            {
                return BadRequest(IndicatingMessages.InvalidAccess);
            }
            TaskData td = order.TaskData;
            if (td.StateId != (int)States.ProofingComplete)
            {
                return BadRequest(IndicatingMessages.TaskNotFound);
            }
            if (td.IsRiskBuild == true)
            {
                var orders = orderService.FindOrdersByProductId(order.ProductId);
                foreach (var ord in orders)
                {
                    if (ord.Id == orderId)
                        continue;
                    if (ord.TaskData.StateId < (int)States.OrderPaid)
                    {
                        return BadRequest(IndicatingMessages.QuantityOrderHasNotBeenPaid);
                    }
                }
            }
            StateTransitionViewModel model = new StateTransitionViewModel
            {
                group = Triggers.ApprovedProof.ToString(),
                TaskData = td,
                UserType = USER_TYPE.Customer,
            };
            return await ProcessRequest(model);
        }


        /// <summary>
        /// Customer trigger ProofRejected.
        /// </summary>
        /// <param name="orderId">Order Id</param>
        [HttpPost]
        [Route("customer/{userId}/ProofRejected/{orderId:int}")]
        public async Task<IHttpActionResult> ProofRejected(int orderId)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest("Invalid User or Customer");

            var order = orderService.FindOrderById(orderId);
            if (order == null)
            {
                return BadRequest(IndicatingMessages.OrderNotFound);
            }
            if (!productBL.IsUsersProduct(order.Product))
            {
                return BadRequest(IndicatingMessages.InvalidAccess);
            }
            TaskData td = order.TaskData;
            if (td.StateId != (int)States.ProofingComplete)
            {
                return BadRequest(IndicatingMessages.TaskNotFound);
            }
            var files = GetPostedFiles(homeBL, false);

            try
            {
                var formDataDic = HttpContext.Current.Request.Form.ToDictionary();
                if (!formDataDic.ContainsKey("rejectReason") || string.IsNullOrEmpty(formDataDic["rejectReason"]))
                {
                    return BadRequest(IndicatingMessages.MissingFormData);
                }
                td.RejectReason = formDataDic["rejectReason"];
                StateTransitionViewModel model = new StateTransitionViewModel
                {
                    group = Triggers.ProofRejected.ToString(),
                    TaskData = td,
                    UserType = USER_TYPE.Customer,
                    RejectReasonDoc = files.Any() ? files : null,
                };
                return await ProcessRequest(model, files);
            }
            finally
            {
                DeleteUploadFiles(files);
            }
        }

#if false
        /// <summary>
        /// Vendor trigger ReadyForTooling.
        /// </summary>
        /// <param name="orderId">Order Id</param>
        [HttpPost]
        [Route("vendor/{userId}/ReadyForTooling/{orderId:int}")]
        public async Task<IHttpActionResult> ReadyForTooling(int orderId)
        {           
            var order = orderService.FindOrderById(orderId);
            TaskData td = order.TaskData;
            if (td.StateId != (int)States.ProofApproved)
            {
                return BadRequest(IndicatingMessages.TaskNotFound);
            }
            StateTransitionViewModel model = new StateTransitionViewModel
            {
                group = Triggers.ReadyForTooling.ToString(),
                TaskData = td,
                UserType = USER_TYPE.Vendor,
            };
            return await ProcessRequest(model);
        }
#endif

        /// <summary>
        /// Vendor trigger CorrectingProof.
        /// </summary>
        /// <param name="orderId">Order Id</param>
        [HttpPost]
        [Route("vendor/{userId}/CorrectingProof/{orderId:int}")]
        public async Task<IHttpActionResult> CorrectingProof(int orderId)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest("Invalid User or Customer");

            var order = orderService.FindOrderById(orderId);
            if (order == null)
            {
                return BadRequest(IndicatingMessages.OrderNotFound);
            }
            if (!productBL.IsUsersProduct(order.Product))
            {
                return BadRequest(IndicatingMessages.InvalidAccess);
            }
            TaskData td = order.TaskData;
            if (td.StateId != (int)States.ProofRejected)
            {
                return BadRequest(IndicatingMessages.TaskNotFound);
            }
            StateTransitionViewModel model = new StateTransitionViewModel
            {
                group = Triggers.CorrectingProof.ToString(),
                TaskData = td,
                UserType = USER_TYPE.Vendor,
            };
            return await ProcessRequest(model);
        }


        /// <summary>
        /// Vendor trigger ToolingComplete.
        /// </summary>
        /// <param name="orderId">Order Id</param>
        [HttpPost]
        [Route("vendor/{userId}/ToolingComplete/{orderId:int}")]
        public async Task<IHttpActionResult> ToolingComplete(int orderId)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest("Invalid User or Customer");

            var order = orderService.FindOrderById(orderId);
            if (order == null)
            {
                return BadRequest(IndicatingMessages.OrderNotFound);
            }
            if (!productBL.IsUsersProduct(order.Product))
            {
                return BadRequest(IndicatingMessages.InvalidAccess);
            }
            TaskData td = order.TaskData;
            if (td.StateId != (int)States.ProofApproved && td.StateId != (int)States.SampleRejected)
            {
                return BadRequest(IndicatingMessages.TaskNotFound);
            }

            // Get form data for 
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var formDataDic = HttpContext.Current.Request.Form.ToDictionary();
            if (!formDataDic.ContainsKey("estimateCompletionDate"))
                return BadRequest(IndicatingMessages.MissingFormData);

            DateTime estimateCompletionDate = DateTime.Parse(formDataDic["estimateCompletionDate"]);
            td.StateId = (int)States.ToolingStarted;
            StateTransitionViewModel model = new StateTransitionViewModel
            {
                group = Triggers.ToolingComplete.ToString(),
                TaskData = td,
                UserType = USER_TYPE.Vendor,
                EstimateCompletionDate = estimateCompletionDate,
            };
            return await ProcessRequest(model);
        }

        /// <summary>
        /// Get Generated TrackingSlip Doc.
        /// </summary>
        /// <param name="orderId">Order Id</param>
        [HttpGet]
        [ResponseType(typeof(PackingSlipUriViewModel))]
        [Route("vendor/{userId}/GetTrackingSlip/{orderId:int}")]
        public IHttpActionResult GetTrackingSlip(int orderId)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest("Invalid User or Customer");

            var order = orderService.FindOrderById(orderId);
            if (order == null)
            {
                return BadRequest(IndicatingMessages.OrderNotFound);
            }
            if (!productBL.IsUsersProduct(order.Product))
            {
                return BadRequest(IndicatingMessages.InvalidAccess);
            }
            TaskData td = order.TaskData;
            if (td.StateId != (int)States.SampleComplete && td.StateId != (int)States.ProductionComplete)
            {
                return BadRequest(IndicatingMessages.TaskStateMismatch);
            }

            var controller = HomeController.CreateController<HomeController>();
            var model = homeBL.CreatePackingSlip(td, controller.ControllerContext, order);

            return Ok(model);
        }


        /// <summary>
        /// Vendor trigger CompleteSample.
        /// </summary>
        /// <param name="orderId">Order Id</param>
        [HttpPost]
        [Route("vendor/{userId}/CompleteSample/{orderId:int}")]
        public async Task<IHttpActionResult> CompleteSample(int orderId)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest("Invalid User or Customer");

            var order = orderService.FindOrderById(orderId);
            if (order == null)
            {
                return BadRequest(IndicatingMessages.OrderNotFound);
            }
            if (!productBL.IsUsersProduct(order.Product))
            {
                return BadRequest(IndicatingMessages.InvalidAccess);
            }
            TaskData td = order.TaskData;
            if (td.StateId != (int)States.SampleStarted && td.StateId != (int)States.SampleComplete && td.StateId != (int)States.SampleRejected)
            {
                return BadRequest(IndicatingMessages.TaskNotFound);
            }
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var formDataDic = HttpContext.Current.Request.Form.ToDictionary();

            if (!formDataDic.ContainsKey("trackingNumber"))
                return BadRequest(IndicatingMessages.MissingFormData);

            string trackingNumber = formDataDic["trackingNumber"];
            string carrierName = formDataDic["carrierName"];
            string accountNumber = formDataDic["accountNumber"];

            SHIPPING_CARRIER_TYPE carrierType;
            bool res = SHIPPING_CARRIER_TYPE.TryParse(formDataDic["carrierType"], out carrierType);
            td.StateId = (int)States.SampleStarted;

            // Upload Inspection Report file
            var files = GetPostedFiles(homeBL);
            try
            {
                StateTransitionViewModel model = new StateTransitionViewModel
                {
                    group = Triggers.CompleteSample.ToString(),
                    TaskData = td,
                    UserType = USER_TYPE.Vendor,
                    TrackingNumber = trackingNumber,
                    CarrierName = carrierName,
                    ShippingAccountNumber = accountNumber,
                    CarrierType = res == true ? carrierType : SHIPPING_CARRIER_TYPE.Air,
                };

                return await ProcessRequest(model, files);
            }
            finally
            {
                DeleteUploadFiles(files);
            }
        }

        /// <summary>
        /// Customer trigger ApproveSample.
        /// </summary>
        /// <param name="orderId">Order Id</param>
        [HttpPost]
        [Route("Customer/{userId}/ApproveSample/{orderId:int}")]
        public async Task<IHttpActionResult> ApproveSample(int orderId)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest("Invalid User or Customer");

            var order = orderService.FindOrderById(orderId);
            if (order == null)
            {
                return BadRequest(IndicatingMessages.OrderNotFound);
            }
            if (!productBL.IsUsersProduct(order.Product))
            {
                return BadRequest(IndicatingMessages.InvalidAccess);
            }
            TaskData td = order.TaskData;
            if (td.StateId != (int)States.SampleComplete)
            {
                return BadRequest(IndicatingMessages.TaskNotFound);
            }
            if (order.IsForToolingOnly == false)
            {
                var orders = orderService.FindOrdersByProductId(order.ProductId);
                foreach (var ord in orders)
                {
                    if (ord.Id == orderId)
                        continue;
                    if (ord.TaskData.StateId < (int)States.OrderPaid)
                    {
                        return BadRequest(IndicatingMessages.QuantityOrderHasNotBeenPaid);
                    }
                }
            }

            StateTransitionViewModel model = new StateTransitionViewModel
            {
                group = Triggers.ApproveSample.ToString(),
                TaskData = td,
                UserType = USER_TYPE.Customer,
            };
            return await ProcessRequest(model);
        }

        /// <summary>
        /// Customer trigger ApproveSample to approve sample
        /// </summary>
        /// <param name="orderId">Order Id</param>
        [HttpPost]
        [Route("Customer/{userId}/SampleApproved/{orderId:int}")]
        public async Task<IHttpActionResult> SampleApproved(int orderId)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest("Invalid User or Customer");

            var order = orderService.FindOrderById(orderId);
            if (order == null)
            {
                return BadRequest(IndicatingMessages.OrderNotFound);
            }
            if (!productBL.IsUsersProduct(order.Product))
            {
                return BadRequest(IndicatingMessages.InvalidAccess);
            }
            TaskData td = order.TaskData;
            if (td.StateId != (int)States.SampleComplete)
            {
                return BadRequest(IndicatingMessages.TaskNotFound);
            }

            StateTransitionViewModel model = new StateTransitionViewModel
            {
                group = Triggers.ApproveSample.ToString(),
                TaskData = td,
                UserType = USER_TYPE.Customer,
            };

            var res = await ProcessRequest(model);
            if (order.IsForToolingOnly == false)
            {
                var orders = orderService.FindOrdersByProductId(order.ProductId);
                foreach (var ord in orders)
                {
                    if (ord.Id == orderId)
                        continue;
                    ord.TaskData.StateId = order.TaskData.StateId;
                    orderService.UpdateOrder(ord);
                }
            }
            return res;
        }

        /// <summary>
        /// Customer trigger RejectedSample.
        /// </summary>
        /// <param name="orderId">Order Id</param>
        [HttpPost]
        [Route("Customer/{userId}/RejectedSample/{orderId:int}")]
        public async Task<IHttpActionResult> RejectedSample(int orderId)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest("Invalid User or Customer");

            var order = orderService.FindOrderById(orderId);
            if (order == null)
            {
                return BadRequest(IndicatingMessages.OrderNotFound);
            }
            if (!productBL.IsUsersProduct(order.Product))
            {
                return BadRequest(IndicatingMessages.InvalidAccess);
            }
            TaskData td = order.TaskData;
            if (td.StateId != (int)States.SampleComplete)
            {
                return BadRequest(IndicatingMessages.TaskNotFound);
            }

            var files = GetPostedFiles(homeBL, false);
            try
            {
                var formDataDic = HttpContext.Current.Request.Form.ToDictionary();
                if (!formDataDic.ContainsKey("rejectReason") || string.IsNullOrEmpty(formDataDic["rejectReason"]))
                {
                    return BadRequest(IndicatingMessages.MissingFormData);
                }
                td.RejectReason = formDataDic["rejectReason"];
                StateTransitionViewModel model = new StateTransitionViewModel
                {
                    group = Triggers.RejectedSample.ToString(),
                    TaskData = td,
                    UserType = USER_TYPE.Customer,
                    RejectReasonDoc = files.Any() ? files : null,
                };
                return await ProcessRequest(model, files);
            }
            finally
            {
                DeleteUploadFiles(files);
            }
        }

        /// <summary>
        /// Customer trigger CorrectingSample.
        /// </summary>
        /// <param name="orderId">Order Id</param>
        [HttpPost]
        [Route("vendor/{userId}/CorrectingSample/{orderId:int}")]
        public async Task<IHttpActionResult> CorrectingSample(int orderId)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest("Invalid User or Customer");

            var order = orderService.FindOrderById(orderId);
            if (order == null)
            {
                return BadRequest(IndicatingMessages.OrderNotFound);
            }
            if (!productBL.IsUsersProduct(order.Product))
            {
                return BadRequest(IndicatingMessages.InvalidAccess);
            }
            TaskData td = order.TaskData;
            if (td.StateId != (int)States.SampleRejected)
            {
                return BadRequest(IndicatingMessages.TaskNotFound);
            }
            StateTransitionViewModel model = new StateTransitionViewModel
            {
                group = Triggers.CorrectingSample.ToString(),
                TaskData = td,
                UserType = USER_TYPE.Vendor,
            };
            return await ProcessRequest(model);
        }

        /// <summary>
        /// Vendor trigger InProduction.
        /// </summary>
        /// <param name="orderId">Order Id</param>
        [HttpPost]
        [Route("vendor/{userId}/InProduction/{orderId:int}")]
        public async Task<IHttpActionResult> InProduction(int orderId)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest("Invalid User or Customer");

            var order = orderService.FindOrderById(orderId);
            if (order == null)
            {
                return BadRequest(IndicatingMessages.OrderNotFound);
            }
            if (!productBL.IsUsersProduct(order.Product))
            {
                return BadRequest(IndicatingMessages.InvalidAccess);
            }
            TaskData td = order.TaskData;
            if (td.StateId != (int)States.SampleApproved && td.StateId != (int)States.ReOrderPaid)
            {
                return BadRequest(IndicatingMessages.TaskNotFound);
            }
            var formDataDic = HttpContext.Current.Request.Form.ToDictionary();
            if (!formDataDic.ContainsKey("estimateCompletionDate"))
            {
                return BadRequest(IndicatingMessages.MissingFormData);
            }
            var estimateCompletionDate = DateTime.Parse(formDataDic["estimateCompletionDate"]);
            StateTransitionViewModel model = new StateTransitionViewModel
            {
                group = Triggers.InProduction.ToString(),
                TaskData = td,
                UserType = USER_TYPE.Vendor,
                EstimateCompletionDate = estimateCompletionDate,
            };
            return await ProcessRequest(model);
        }


        /// <summary>
        /// Vendor trigger CompleteProduction.
        /// </summary>
        /// <param name="orderId">Order Id</param>
        [HttpPost]
        [Route("vendor/{userId}/CompleteProduction/{orderId:int}")]
        public async Task<IHttpActionResult> CompleteProduction(int orderId)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest("Invalid User or Customer");

            var order = orderService.FindOrderById(orderId);
            if (order == null)
            {
                return BadRequest(IndicatingMessages.OrderNotFound);
            }
            if (!productBL.IsUsersProduct(order.Product))
            {
                return BadRequest(IndicatingMessages.InvalidAccess);
            }
            TaskData td = order.TaskData;
            if (td.StateId != (int)States.ProductionStarted)
            {
                return BadRequest(IndicatingMessages.TaskNotFound);
            }
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var files = GetPostedFiles(homeBL);
            try
            {
                var formDataDic = HttpContext.Current.Request.Form.ToDictionary();

                if (!formDataDic.ContainsKey("trackingNumber"))
                    return BadRequest(IndicatingMessages.MissingFormData);

                string trackingNumber = formDataDic["trackingNumber"];
                string carrierName = formDataDic.ContainsKey("carrierName") ? formDataDic["carrierName"] : null;
                string accountNumber = formDataDic.ContainsKey("accountNumber") ? formDataDic["accountNumber"] : null;

                SHIPPING_CARRIER_TYPE carrierType;
                bool res = SHIPPING_CARRIER_TYPE.TryParse(formDataDic["carrierType"], out carrierType);

                string trigger = Triggers.CompleteProduction.ToString();
                if (td.isEnterprise == false)
                {
                    trigger = Triggers.CompleteInvoiceForVendor.ToString();
                    td.StateId = (int)States.VendorPendingInvoice;
                }

                // Upload Inspection Report file
                StateTransitionViewModel model = new StateTransitionViewModel
                {
                    group = trigger,
                    TaskData = td,
                    UserType = USER_TYPE.Vendor,
                    TrackingNumber = trackingNumber,
                    CarrierName = carrierName,
                    ShippingAccountNumber = accountNumber,
                    CarrierType = res == true ? carrierType : SHIPPING_CARRIER_TYPE.Air,
                };
                return await ProcessRequest(model, files);
            }
            finally
            {
                DeleteUploadFiles(files);
            }
        }


        /// <summary>
        /// Vendor trigger CompleteProduction without need for shipping info.
        /// </summary>
        /// <param name="orderId">Order Id</param>
        [HttpPost]
        [Route("vendor/{userId}/CompleteProductionV2/{orderId:int}")]
        public async Task<IHttpActionResult> CompleteProductionV2(int orderId)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest("Invalid User or Customer");

            var order = orderService.FindOrderById(orderId);
            if (order == null)
            {
                return BadRequest(IndicatingMessages.OrderNotFound);
            }
            if (!productBL.IsUsersProduct(order.Product))
            {
                return BadRequest(IndicatingMessages.InvalidAccess);
            }
            TaskData td = order.TaskData;
            if (td.StateId != (int)States.ProductionStarted)
            {
                return BadRequest(IndicatingMessages.TaskNotFound);
            }
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var files = GetPostedFiles(homeBL);
            try
            {
                string trigger = Triggers.CompleteProduction.ToString();
                if (td.isEnterprise == false)
                {
                    trigger = Triggers.CompleteInvoiceForVendor.ToString();
                    td.StateId = (int)States.VendorPendingInvoice;
                }

                // Upload Inspection Report file
                StateTransitionViewModel model = new StateTransitionViewModel
                {
                    group = trigger,
                    TaskData = td,
                    UserType = USER_TYPE.Vendor,
                    RequireShippingInfo = false,
                };
                return await ProcessRequest(model, files);
            }
            finally
            {
                DeleteUploadFiles(files);
            }
        }

#if false
        /// <summary>
        /// Vendor trigger CreateInvoiceForVendor.
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="pid">Product Id</param>
        [HttpPost]
        [Route("vendor/{userId}/CreateInvoiceForVendor/{pid:int}")]
        public async Task<IHttpActionResult> CreateInvoiceForVendor(string userId, int pid)
        {
            TaskData td = taskDataService.FindTaskDataListByProductId(pid)
                .Where(x => x.StateId == (int)States.ProductionComplete)
                .LastOrDefault();

            if (td == null)
            {
                return BadRequest(IndicatingMessages.TaskNotFound);
            }
            StateTransitionViewModel model = new StateTransitionViewModel
            {
                group = Triggers.CreateInvoiceForVendor.ToString(),
                TaskData = td,
                UserType = USER_TYPE.Vendor,
            };
            return await ProcessRequest(model);
        }
#endif

        /// <summary>
        /// Vendor trigger CompleteInvoiceForVendor.
        /// </summary>
        /// <param name="orderId">Order Id</param>
        [HttpPost]
        [Route("vendor/{userId}/CompleteInvoiceForVendor/{orderId:int}")]
        public async Task<IHttpActionResult> CompleteInvoiceForVendor(int orderId)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest("Invalid User or Customer");

            var order = orderService.FindOrderById(orderId);
            if (order == null)
            {
                return BadRequest(IndicatingMessages.OrderNotFound);
            }
            if (!productBL.IsUsersProduct(order.Product))
            {
                return BadRequest(IndicatingMessages.InvalidAccess);
            }
            TaskData td = order.TaskData;
            if (td.StateId != (int)States.VendorPendingInvoice)
            {
                return BadRequest(IndicatingMessages.TaskNotFound);
            }

            var files = GetPostedFiles(homeBL);

            try
            {
                if (files.Count == 0)
                {
                    return BadRequest(IndicatingMessages.ForgotUploadFile);
                }
                string vendorAttachedInviceNumberForTooling = string.Empty;
                string vendorAttachedInvoiceNumber = string.Empty;
                var formDataDic = HttpContext.Current.Request.Form.ToDictionary();
                if (order.IsReorder || order.IsForToolingOnly == true)
                {
                    if (!formDataDic.ContainsKey("vendorAttachedInvoiceNumber"))
                        return BadRequest(IndicatingMessages.MissingFormData);

                    vendorAttachedInvoiceNumber = formDataDic["vendorAttachedInvoiceNumber"];
                }
                else
                {
                    if (!formDataDic.ContainsKey("vendorAttachedInviceNumberForTooling") ||
                        !formDataDic.ContainsKey("vendorAttachedInvoiceNumber") ||
                        files.Count != 2)
                    {
                        return BadRequest(IndicatingMessages.MissingFormData);
                    }
                    vendorAttachedInviceNumberForTooling = formDataDic["vendorAttachedInviceNumberForTooling"];
                    vendorAttachedInvoiceNumber = formDataDic["vendorAttachedInvoiceNumber"];
                }

                StateTransitionViewModel model = new StateTransitionViewModel
                {
                    group = Triggers.CompleteInvoiceForVendor.ToString(),
                    TaskData = td,
                    UserType = USER_TYPE.Vendor,
                    VendorInvoiceVM = new VendorInvoiceViewModel
                    {
                        AttachInvoiceForTooling = files.Count > 1 ? files[1] : null,
                        VendorAttachedInviceNumberForTooling = vendorAttachedInviceNumberForTooling,
                        AttachInvoice = files[0],
                        VendorAttachedInvoiceNumber = vendorAttachedInvoiceNumber,
                    },
                };
                return await ProcessRequest(model, files);
            }
            finally
            {
                DeleteUploadFiles(files);
            }
        }

        // Request/Response for expedited shipment


        /// <summary>
        /// Send request for expedited shipment to vendor or customer by customer or vendor
        /// </summary>
        /// <param name="orderId">Order Id</param>
        /// <param name="newShippingDate">New desired shipping date</param>
        /// <param name="mode">Customer or Vendor</param>
        [HttpPost]
        [Route("user/{userId}/SendExpeditedShipmentRequest/{orderId:int}", Name = "SendExpeditedShipmentRequest")]
        public IHttpActionResult SendExpeditedShipmentRequest(int orderId,
                                                                DateTime newShippingDate,
                                                                OrderQuery.UserMode mode = OrderQuery.UserMode.Customer)
        {
            using (var trans = AsyncTransactionScope.StartNew())
            {
                var order = orderService.FindOrderById(orderId);
                if (order == null)
                    return BadRequest(IndicatingMessages.OrderNotFound);



                EXPEDITED_SHIPMENT_TYPE expeditedType = newShippingDate < order.DesireShippingDate ?
                                                        EXPEDITED_SHIPMENT_TYPE.PRIOR_TO :
                                                        (newShippingDate == order.DesireShippingDate ?
                                                         EXPEDITED_SHIPMENT_TYPE.ON_TIME :
                                                         EXPEDITED_SHIPMENT_TYPE.DELAYED);

                int companyId = userContext.Company.Id;
                if (!productBL.IsUsersProduct(order.Product))
                {
                    return BadRequest(IndicatingMessages.InvalidAccess);
                }

                var entity = expeditedShipmentRequestService.FindExpeditedShipmentRequestByParams(orderId, companyId);
                try
                {
                    if (entity != null)
                    {
                        entity.NewDesireShippingDate = newShippingDate;
                        entity.IsRequestedByCustomer = mode == OrderQuery.UserMode.Customer;
                        entity.IsRequestedByVendor = mode == OrderQuery.UserMode.Vendor;
                        expeditedShipmentRequestService.UpdateExpeditedShipmentRequest(entity);
                    }
                    else
                    {
                        entity = new ExpeditedShipmentRequest
                        {
                            OrderId = order.Id,
                            InitiateCompanyId = companyId,
                            ExpeditedShipmentType = expeditedType,
                            IsRequestedByCustomer = mode == OrderQuery.UserMode.Customer,
                            IsRequestedByVendor = mode == OrderQuery.UserMode.Vendor,
                            NewDesireShippingDate = newShippingDate,
                        };
                        order.ExpeditedShipmentRequestId = expeditedShipmentRequestService.AddExpeditedShipmentRequest(entity);
                        orderService.UpdateOrder(order);
                    }

                    var dto = Mapper.Map<OrderDTO>(order);

                    trans.Complete();
                    return Ok(dto);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Send response for expedited shipment to vendor or customer by customer or vendor
        /// </summary>
        /// <param name="orderId">Order Id</param>
        /// <param name="isAccepted">Accept request or not</param>
        /// <param name="mode">Customer or Vendor</param>

        [HttpPost]
        [Route("user/{userId}/SendExpeditedShipmentResponse/{orderId:int}", Name = "SendExpeditedShipmentResponse")]
        public IHttpActionResult SendExpeditedShipmentResponse(int orderId,
                                                                EXPEDITED_SHIPMENT_RESPONSE isAccepted,
                                                                OrderQuery.UserMode mode = OrderQuery.UserMode.Vendor)
        {
            using (var trans = AsyncTransactionScope.StartNew())
            {
                var order = orderService.FindOrderById(orderId);
                if (order == null)
                    return BadRequest(IndicatingMessages.OrderNotFound);
                if (order.ExpeditedShipmentRequestId == null)
                    return BadRequest(IndicatingMessages.NoExpeditedShipmentRequested);

                if (!productBL.IsUsersProduct(order.Product))
                {
                    return BadRequest(IndicatingMessages.InvalidAccess);
                }

                var entity = expeditedShipmentRequestService.FindExpeditedShipmentRequest(order.ExpeditedShipmentRequestId.Value);

                entity.IsAccepted = isAccepted == EXPEDITED_SHIPMENT_RESPONSE.ACCEPT;
                expeditedShipmentRequestService.UpdateExpeditedShipmentRequest(entity);

                if (entity.IsAccepted == true)
                {
                    order.DesireShippingDate = entity.NewDesireShippingDate;
                    orderService.UpdateOrder(order);
                }

                var dto = Mapper.Map<OrderDTO>(order);

                trans.Complete();
                return Ok(dto);
            }
        }

        /// <summary>
        /// Customer sending cancel order request to vendor
        /// </summary>
        /// <param name="orderId">Order Id</param>
        /// <param name="cancelOrderReason">Cancel order reason</param>
        [HttpPost]
        [Route("customer/{userId}/CustomerCancelOrderRequest/{orderId:int}", Name = "CustomerCancelOrderRequest")]
        public IHttpActionResult CustomerCancelOrderRequest(int orderId, string cancelOrderReason)
        {
            var order = orderService.FindOrderById(orderId);
            if (order == null)
            {
                return BadRequest(IndicatingMessages.OrderNotFound);
            }
            if (order.CustomerId != null && order.CustomerId != userContext.Company.Id)
            {
                return BadRequest(IndicatingMessages.InvalidAccess);
            }
            TaskData td = order.TaskData;
            if (td.StateId < (int)States.OrderInitiated || td.StateId > (int)States.ProductionStarted)
            {
                return BadRequest(IndicatingMessages.TaskStateMismatch);
            }

            td.TaskStateBeforeCustomerCancelOrder = td.StateId;
            td.StateId = (int)States.CustomerCancelOrder;
            taskDataService.Update(td);
            order.CancelOrderReason = cancelOrderReason;
            orderService.UpdateOrder(order);

            var dto = Mapper.Map<OrderDTO>(order);
            return Ok(dto);
        }

        /// <summary>
        /// Vendor sending cancel order reponse to customer
        /// </summary>
        /// <param name="orderId">Order Id</param>
        /// <param name="agreeToCancel">Vendor agrees to cancel or not</param>
        /// <param name="denyCancelReason">Vendor deny cancel reason</param>
        [HttpPost]
        [Route("vendor/{userId}/VendorCancelOrderResponse/{orderId:int}", Name = "VendorCancelOrderResponse")]
        public IHttpActionResult VendorCancelOrderResponse(int orderId, bool agreeToCancel, string denyCancelReason = null)
        {
            var order = orderService.FindOrderById(orderId);
            if (order == null)
            {
                return BadRequest(IndicatingMessages.OrderNotFound);
            }
            if (!productBL.IsUsersProduct(order.Product))
            {
                return BadRequest(IndicatingMessages.InvalidAccess);
            }
            TaskData td = order.TaskData;
            if (td.StateId != (int)States.CustomerCancelOrder)
            {
                return BadRequest(IndicatingMessages.TaskStateMismatch);
            }

            if (agreeToCancel == true)
            {
                td.StateId = (int)States.OrderCancelled;
                taskDataService.Update(td);

                var orders = orderService.FindOrderByTaskId(td.TaskId);
                foreach (var ord in orders)
                {
                    ord.IsOrderCancelled = true;
                    ord.DenyCancelOrderReason = null;
                    ord.OrderCancelledBy = (int)OrderCancelledBy.Customer;
                    orderService.UpdateOrder(ord);
                }
            }
            else
            {
                td.StateId = (int)td.TaskStateBeforeCustomerCancelOrder;
                taskDataService.Update(td);
                order.DenyCancelOrderReason = denyCancelReason;
                orderService.UpdateOrder(order);
            }
            var dto = Mapper.Map<OrderDTO>(order);
            return Ok(dto);
        }

        /// <summary>
        /// Vendor rejects an order
        /// </summary>
        /// <param name="orderId">Order Id</param>
        /// <param name="rejectlReason">Vendor deny cancel reason</param>
        [HttpPost]
        [Route("vendor/{userId}/VendorRejectOrder/{orderId:int}", Name = "VendorRejectOrder")]
        public IHttpActionResult VendorRejectOrder(int orderId, string rejectlReason)
        {
            var order = orderService.FindOrderById(orderId);
            if (order == null)
            {
                return BadRequest(IndicatingMessages.OrderNotFound);
            }
            if (!productBL.IsUsersProduct(order.Product))
            {
                return BadRequest(IndicatingMessages.InvalidAccess);
            }
            TaskData td = order.TaskData;
            if (td.StateId < (int)States.OrderInitiated || td.StateId > (int)States.ProductionStarted)
            {
                return BadRequest(IndicatingMessages.TaskStateMismatch);
            }
            td.StateId = (int)States.OrderCancelled;
            taskDataService.Update(td);

            var orders = orderService.FindOrderByTaskId(td.TaskId);
            foreach (var ord in orders)
            {
                ord.IsOrderCancelled = true;
                ord.CancelOrderReason = rejectlReason;
                ord.ModifiedByUserId = userContext.UserId;
                ord.OrderCancelledBy = (int)OrderCancelledBy.Vendor;
                orderService.UpdateOrder(ord);
            }


            var dto = Mapper.Map<OrderDTO>(order);
            return Ok(dto);
        }


        /// <summary>
        /// Customer/Vendor wants to change the quantity of the order before task state reaches to ProductionComplete 
        /// and after order placed
        /// </summary>
        /// <param name="orderId">Order Id</param>
        /// <param name="newQty">New quantity to be changed to</param>
        [HttpPost]
        [Route("user/{userId}/ChangeOrderQuantity/{orderId:int}", Name = "ChangeOrderQuantity")]
        public IHttpActionResult ChangeOrderQuantity(int orderId, int newQty)
        {
            var order = orderService.FindOrderById(orderId);
            if (order == null)
                return BadRequest(IndicatingMessages.OrderNotFound);

            if (!productBL.IsUsersProduct(order.Product))
                return BadRequest(IndicatingMessages.InvalidAccess);

            TaskData td = order.TaskData;
            if (td.StateId < (int)States.OrderPaid || td.StateId >= (int)States.ProductionComplete)
                return BadRequest(IndicatingMessages.TaskStateMismatch);

            order.Quantity = newQty;
            order.ModifiedByUserId = userContext.UserId;
            orderService.UpdateOrder(order);

            var dto = Mapper.Map<OrderDTO>(order);
            return Ok(dto);
        }

        /// <summary>
        /// Remove a not started order in order to redefine a new Production Run
        /// </summary>
        /// <param name="orderId">Order Id</param>
        [HttpPost]
        [Route("user/{userId}/RemoveOrder/{orderId:int}", Name = "RemoveOrder")]
        public IHttpActionResult RemoveOrder(int orderId)
        {
            var order = orderService.FindOrderById(orderId);
            if (order == null)
                return BadRequest(IndicatingMessages.OrderNotFound);

            if (!productBL.IsUsersProduct(order.Product))
                return BadRequest(IndicatingMessages.InvalidAccess);

            using (var ts = AsyncTransactionScope.StartNew())
            {
                var invoices = omnaeInvoiceService.FindOmnaeInvoicesByOrderId(order.Id);
                var td = order.TaskData;
                foreach (var inv in invoices)
                {
                    omnaeInvoiceService.DeleteOmnaeInvoice(inv);
                }
                orderService.RemoveOrder(order);
                if (order.IsReorder == true)
                {
                    taskDataService.DeleteTaskData(td);
                }
                else
                {
                    td.StateId = (int)States.QuoteAccepted;
                    td.UpdatedBy = userContext.User.UserName;
                    td.ModifiedByUserId = userContext.UserId;
                    taskDataService.Update(td);
                }
                
                ts.Complete();
                return Ok("Success");
            }
        }


        ///////////////////////  Private methods  ///////////////////////

        private async Task<IHttpActionResult> ProcessRequest(StateTransitionViewModel model, List<HttpPostedFileBase> files = null)
        {
            var td = model.TaskData;
            taskDataService.Update(td);
            var request = new HttpRequestWrapper(HttpContext.Current.Request);

            try
            {
                var result = await homeBL.TaskStateHandler(model, files);
                if (result != null)
                {
                    if (!result.Equals(IndicatingMessages.SmsWarningMsg) && !result.Equals(IndicatingMessages.NoCellPhoneWarningMsg))
                    {
                        return BadRequest(result);
                    }
                }
                td = taskDataService.FindById(td.TaskId);

                return Ok(td.StateId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DeleteUploadFiles(files);
            }
        }


        private IQueryable<Order> SearchOrders(IQueryable<Order> orders, string search)
        {
            search = search.Trim().ToUpper();
            return orders.Where(x => x.Product.PartNumber != null && x.Product.PartNumber.ToUpper().Contains(search)
                                || x.Product.Name != null && x.Product.Name.ToUpper().Contains(search)
                                || x.Product.VendorCompany.Name != null && x.Product.VendorCompany.Name.ToUpper().Contains(search)
                                || x.CustomerPONumber != null && x.CustomerPONumber.ToUpper().Contains(search));
        }


        private async Task SendEmailAysnc(TaskData td, USER_TYPE userType)
        {
            var company = userType == USER_TYPE.Customer ? td.Product?.VendorCompany : td.Product.CustomerCompany;
            if (company != null)
            {
                var users = userContactService.GetAllActiveUserConnectFromCompany(company);
                foreach (var contactInformation in users)
                {
                    await notificationBL.SendNotificationsAsync(td, contactInformation.Email, contactInformation.PhoneNumber);
                }
            }
        }
    }

}