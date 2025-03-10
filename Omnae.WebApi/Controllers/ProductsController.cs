using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common;
using Common.Extensions;
using Humanizer;
using Intuit.Ipp.Data;
using Libs;
using Omnae.BlobStorage;
using Omnae.BusinessLayer;
using Omnae.BusinessLayer.Interface;
using Omnae.BusinessLayer.Models;
using Omnae.Common;
using Omnae.Data;
using Omnae.Data.Query;
using Omnae.Libs.ViewModels;
using Omnae.Model.Context;
using Omnae.Model.Extentions;
using Omnae.Model.Models;
using Omnae.Service.Service;
using Omnae.Service.Service.Interfaces;
using Omnae.WebApi.DTO;
using Omnae.WebApi.Models;
using Omnae.WebApi.Util;
using Omnae.WebApi.ViewModels;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using static NMoneys.Currency;
using static Omnae.Data.Query.CompanyQuery;
using static Omnae.Data.Query.NcrQuery;
using static Omnae.Data.Query.OrderQuery;
using static Omnae.Data.Query.TaskDataQuery;




namespace Omnae.WebApi.Controllers
{
    /// <summary>
    /// Core Api for Products 
    /// </summary>
    [EnableCors("*", "*", "*")]
    [RoutePrefix("api")]
    public class ProductsController : BaseApiController
    {
        private IMapper Mapper { get; }
        private readonly CompanyAccountsBL companyBl;
        private readonly IProductService productService;
        private readonly ProductBL productBL;
        private readonly IHomeBL homeBL;
        private readonly ChartBL chartBL;
        private readonly DocumentBL documentBL;
        private readonly ITaskDataService taskDataService;
        private readonly IRFQBidService rfqBidService;
        private readonly TaskDatasBL taskDataBL;
        private readonly DashboardBL dashBoardBL;
        private readonly IBidRequestRevisionService bidRequestRevisionService;
        private readonly IDocumentService documentService;
        private readonly IPriceBreakService priceBreakService;
        private readonly ILogedUserContext userContext;
        private readonly ICompanyService companyService;
        private readonly IProductStateTrackingService productStateTrackingService;
        private readonly IPartRevisionService partRevisionService;
        private readonly IProductSharingService productSharingService;
        private readonly NotificationBL notificationBL;
        private readonly UserContactService userContactService;
        private readonly IImageStorageService imageStorageService;
        private readonly OnBoardingBL onBoardingBL;
        private readonly IApprovedCapabilityService approvedCapabilityService;
        private readonly ITimerSetupService timerSetupService;
        private readonly IBidRFQStatusService bidRFQStatusService;
        private readonly IVendorBidRFQStatusService vendorBidRFQStatusService;
        private readonly DashboardBL dashboardBL;
        private TransactionOptions transOptions;
        private readonly IProductPriceQuoteService productPriceQuoteService;
        private readonly ICompaniesCreditRelationshipService companiesCreditRelationshipService;


        /// <summary>
        /// Product controller constructor
        /// </summary>
        public ProductsController(ILogger log, CompanyAccountsBL companyBl, IProductService productService, ProductBL productBl, IHomeBL homeBl, ChartBL chartBl, DocumentBL documentBl, ITaskDataService taskDataService, IRFQBidService rfqBidService, TaskDatasBL taskDataBl, DashboardBL dashBoardBl, IBidRequestRevisionService bidRequestRevisionService, IDocumentService documentService, IPriceBreakService priceBreakService, ILogedUserContext userContext, ICompanyService companyService, IProductStateTrackingService productStateTrackingService, IPartRevisionService partRevisionService, IMapper mapper, IProductSharingService productSharingService, NotificationBL notificationBL,
            UserContactService userContactService, IImageStorageService imageStorageService, OnBoardingBL onBoardingBL,
            IApprovedCapabilityService approvedCapabilityService,
            ITimerSetupService timerSetupService, IBidRFQStatusService bidRFQStatusService, IVendorBidRFQStatusService vendorBidRFQStatusService,
            DashboardBL dashboardBL, IProductPriceQuoteService productPriceQuoteService,
            ICompaniesCreditRelationshipService companiesCreditRelationshipService) : base(log)
        {
            this.companyBl = companyBl;
            this.productService = productService;
            productBL = productBl;
            homeBL = homeBl;
            chartBL = chartBl;
            documentBL = documentBl;
            this.taskDataService = taskDataService;
            this.rfqBidService = rfqBidService;
            taskDataBL = taskDataBl;
            dashBoardBL = dashBoardBl;
            this.bidRequestRevisionService = bidRequestRevisionService;
            this.documentService = documentService;
            this.priceBreakService = priceBreakService;
            this.userContext = userContext;
            this.companyService = companyService;
            this.productStateTrackingService = productStateTrackingService;
            this.partRevisionService = partRevisionService;
            Mapper = mapper;
            this.productSharingService = productSharingService;
            this.notificationBL = notificationBL;
            this.userContactService = userContactService;
            this.imageStorageService = imageStorageService;
            this.onBoardingBL = onBoardingBL;
            this.approvedCapabilityService = approvedCapabilityService;
            this.timerSetupService = timerSetupService;
            this.bidRFQStatusService = bidRFQStatusService;
            this.vendorBidRFQStatusService = vendorBidRFQStatusService;
            this.dashboardBL = dashboardBL;
            this.productPriceQuoteService = productPriceQuoteService;
            this.companiesCreditRelationshipService = companiesCreditRelationshipService;

            // Transction Options
            transOptions = new TransactionOptions() { Timeout = TimeSpan.FromMinutes(15), IsolationLevel = IsolationLevel.ReadCommitted };
        }

        /// <summary>
        /// List products
        /// </summary>
        /// <param name="companyId">Company Id to filter results by</param>
        /// <param name="mode">User mode (customer or vendor)</param>
        /// <param name="type">Product filter type</param>
        /// <param name="filter">filtered by string</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [Route("products", Name = "ListProducts")]
        [HttpGet]
        public async Task<PagedResultSet<ProductDTO>> ListProducts(int? companyId, TaskDataQuery.UserType mode = TaskDataQuery.UserType.Customer, TaskDataQuery.ProductFilter type = TaskDataQuery.ProductFilter.All, string filter = null, int? page = 1, int pageSize = PageSize, string orderBy = nameof(ProductDTO.ModifiedUtc), bool ascending = false)
        {
            if (companyId == null)
            {
                // TODO: not sure if returned data is correct
                var products = productService.FindProductList();

                return await PageOfResultsSetAsync<Product, ProductDTO>(products, page, pageSize, orderBy, ascending, "ListProducts");
            }
            else
            {
                IQueryable<TaskData> tasks;
                if (type == TaskDataQuery.ProductFilter.Shared)
                {
                    tasks = productSharingService.FindProductSharingTaskDatasByComanyId((int)companyId)
                        .Where(x => x.StateId <= (int)States.ProductionComplete);
                }
                else if (mode == TaskDataQuery.UserType.Customer)
                {
                    tasks = taskDataService.FindTaskDatasByCustomerId((int)companyId)
                        .Union(productSharingService.FindProductSharingTaskDatasByComanyId((int)companyId))
                        .Where(x => x.StateId <= (int)States.ProductionComplete)
                        .FilterBy(type, mode);
                }
                else
                {
                    tasks = taskDataService.FindTaskDatasByVendorId((int)companyId)
                        .Where(x => x.StateId <= (int)States.ProductionComplete)
                        .FilterBy(type, mode);
                }

                var products = SetProductDTO(tasks);
                if (!string.IsNullOrEmpty(filter))
                {
                    // TODO: refactor to service/repo
                    products = SearchProductDTO(products, filter, mode);
                }

                // TODO: to prevent duplicated product, we may need other solution, for now choose the latest product to list
                var prodList = products.ToList();
                prodList = prodList.GroupBy(x => x.Id).Select(x => x.LastOrDefault()).ToList();

                // OE-2223
                //prodList = prodList.GroupBy(x => (x.CustomerId, x.VendorId, x.PartNumber, x.PartNumberRevision)).Select(x => x.LastOrDefault()).ToList();

                return await PageOfResultsSetAsync<ProductDTO>(prodList.AsQueryable<ProductDTO>(), page, pageSize, orderBy, ascending, "ListProducts");
            }
        }



        /// <summary>
        /// Get products of a customer by array of part# and part rev
        /// </summary>
        /// <param name="customerId">Customer Id to filter results by</param>
        /// <param name="parts">array of PartInfoDto (part# and part rev)</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [Route("GetProductsByParts/{customerId:int}", Name = "GetProductsByParts")]
        [HttpPost]
        public async Task<PagedResultSet<ProductDTO>> GetProductsByParts(int customerId, PartInfoDto[] parts, int? page = 1, int pageSize = PageSize, string orderBy = nameof(ProductDTO.ModifiedUtc), bool ascending = false)
        {
            var tasks = taskDataService.FindTaskDatasByCustomerId(customerId)
                        .Union(productSharingService.FindProductSharingTaskDatasByComanyId(customerId))
                        .FilterByParts(parts).AsQueryable<TaskData>(); ;

            var products = SetProductDTO(tasks);
            return await PageOfResultsSetAsync<ProductDTO>(products, page, pageSize, orderBy, ascending, "GetProductsByParts");
        }

        /// <summary>
        /// Show product details 
        /// </summary>
        /// <param name="id">The ID of the product.</param>
        [Route("products/{id:int}")]
        [HttpGet]
        [ResponseType(typeof(ProductDTO))]
        public async Task<IHttpActionResult> Get(int id)
        {
            var product = productService.FindProductById(id);
            if (product == null)
                return BadRequest(IndicatingMessages.ProductNotFound);

            var task = taskDataService
                .FindTaskDataListByProductId(id)
                .LastOrDefault(x => x.StateId != (int)States.RFQBidComplete);

            if (task == null)
                return BadRequest(IndicatingMessages.TaskNotFound);

            var priceBreaks = (await priceBreakService.FindPriceBreaksByTaskId(task.TaskId).Where(x => x.RFQBid.IsActive == true || x.RFQBid == null).OrderBy(x => x.Quantity).ToListAsync()).ToNullIfEmpty()
                               ?? (await priceBreakService.FindPriceBreaksByProductId(id).Where(x => x.RFQBid.IsActive == true || x.RFQBid == null).OrderBy(x => x.Quantity).ToListAsync());

            var prodDto = Mapper.Map<ProductDTO>(task.Product);

            if (prodDto.AvatarUri != null)
            {
                prodDto.AvatarUri = prodDto.AvatarUri.Split(new[] { "%3FAWS" }, StringSplitOptions.None)[0]; //TODo Remove this, Updating the database.
            }

            prodDto.ToolingSetupCharges = priceBreaks.FirstOrDefault()?.ToolingSetupCharges;
            prodDto.CustomerToolingSetupCharges = priceBreaks.FirstOrDefault()?.CustomerToolingSetupCharges;

            var docs = await documentService.FindDocumentsByProductId(id).ToListAsync();
            var documents = Mapper.Map<List<DocumentDTO>>(docs);

            var dto = new PartDetailsDTO
            {
                Product = prodDto,
                PriceBreaks = Mapper.Map<List<PriceBreakDTO>>(priceBreaks), //await priceBreaks.ProjectTo<PriceBreakDTO>().ToListAsync(),
                PartRvision = Mapper.Map<PartRvisionDTO>(product.PartRevision),
                Documents = documents,
                IsEnterprise = task.isEnterprise,
                UnitOfMeasurement = product.RFQQuantityId != null ? product.RFQQuantity.UnitOfMeasurement : 0,
            };

            dto.Product.TaskId = task.TaskId;
            dto.Product.State = (States)task.StateId;
            dto.Product.Quantities = new Quantities
            {
                Qty1 = product.RFQQuantity?.Qty1 ?? priceBreaks.ElementAtOrDefault(0)?.Quantity,
                Qty2 = product.RFQQuantity?.Qty2 ?? priceBreaks.ElementAtOrDefault(1)?.Quantity,
                Qty3 = product.RFQQuantity?.Qty3 ?? priceBreaks.ElementAtOrDefault(2)?.Quantity,
                Qty4 = product.RFQQuantity?.Qty4 ?? priceBreaks.ElementAtOrDefault(3)?.Quantity,
                Qty5 = product.RFQQuantity?.Qty5 ?? priceBreaks.ElementAtOrDefault(4)?.Quantity,
                Qty6 = product.RFQQuantity?.Qty6 ?? priceBreaks.ElementAtOrDefault(5)?.Quantity,
                Qty7 = product.RFQQuantity?.Qty7 ?? priceBreaks.ElementAtOrDefault(6)?.Quantity,
            };

            if (product.RFQQuantity == null && priceBreaks.Count == 0)
            {
                dto.Product.Quantities = null;
            }

            return Ok(dto);
        }

        /// <summary>
        /// Get product detail 
        /// </summary>
        /// <param name="productId">The ID of the product.</param>
        /// <param name="companyId">Company ID</param>
        /// <param name="type">Company type: Customer or Vendor</param>
        [Route("products/get/{productId:int}")]
        [HttpGet]
        [ResponseType(typeof(ProductDTO))]
        public async Task<IHttpActionResult> Get(int productId, int companyId, TaskDataQuery.UserType type = TaskDataQuery.UserType.Customer)
        {
            var product = productService.FindProductById(productId);
            if (product == null)
                return BadRequest(IndicatingMessages.ProductNotFound);

            var task = taskDataService.FindTaskDataListByProductId(productId)
                                      .Where(x => type == TaskDataQuery.UserType.Customer ? x.Product.CustomerId == companyId : x.Product.VendorId == companyId)
                                      .FirstOrDefault();

            if (task == null)
                return BadRequest(IndicatingMessages.TaskNotFound);

            var prodDto = Mapper.Map<ProductDTO>(task.Product);
            if (prodDto.AvatarUri != null)
            {
                prodDto.AvatarUri = prodDto.AvatarUri.Split(new string[] { "%3FAWS" }, StringSplitOptions.None)[0]; //TODo Remove this, Updating the database.
            }


            var priceBreaks = (await priceBreakService.FindPriceBreaksByTaskId(task.TaskId).Where(x => x.RFQBid.IsActive == true || x.RFQBid == null).OrderBy(x => x.Quantity).ToListAsync()).ToNullIfEmpty()
                              ?? (await priceBreakService.FindPriceBreaksByProductId(productId).Where(x => x.RFQBid.IsActive == true || x.RFQBid == null).OrderBy(x => x.Quantity).ToListAsync());
            var pb = priceBreaks.FirstOrDefault();
            prodDto.ToolingSetupCharges = pb?.ToolingSetupCharges;
            prodDto.CustomerToolingSetupCharges = pb?.CustomerToolingSetupCharges;


            var docs = await documentService.FindDocumentsByProductId(productId).ToListAsync();
            if (type == TaskDataQuery.UserType.Vendor)
            {
                docs.RemoveAll(x => x.DocType == (int)DOCUMENT_TYPE.QUOTE_PDF && x.TaskId != task.TaskId);
            }

            var documents = Mapper.Map<List<DocumentDTO>>(docs);

            var dto = new PartDetailsDTO
            {
                Product = prodDto,
                PriceBreaks = Mapper.Map<List<PriceBreakDTO>>(priceBreaks), //await priceBreaks.ProjectTo<PriceBreakDTO>().ToListAsync(),
                PartRvision = Mapper.Map<PartRvisionDTO>(product.PartRevision),
                Documents = documents,
                IsEnterprise = task.isEnterprise,
                UnitOfMeasurement = product.RFQQuantityId != null ? product.RFQQuantity.UnitOfMeasurement : 0,
            };

            dto.Product.TaskId = task.TaskId;
            dto.Product.State = (States)task.StateId;
            dto.Product.Quantities = new Quantities
            {
                Qty1 = product.RFQQuantity?.Qty1 ?? priceBreaks.ElementAtOrDefault(0)?.Quantity,
                Qty2 = product.RFQQuantity?.Qty2 ?? priceBreaks.ElementAtOrDefault(1)?.Quantity,
                Qty3 = product.RFQQuantity?.Qty3 ?? priceBreaks.ElementAtOrDefault(2)?.Quantity,
                Qty4 = product.RFQQuantity?.Qty4 ?? priceBreaks.ElementAtOrDefault(3)?.Quantity,
                Qty5 = product.RFQQuantity?.Qty5 ?? priceBreaks.ElementAtOrDefault(4)?.Quantity,
                Qty6 = product.RFQQuantity?.Qty6 ?? priceBreaks.ElementAtOrDefault(5)?.Quantity,
                Qty7 = product.RFQQuantity?.Qty7 ?? priceBreaks.ElementAtOrDefault(6)?.Quantity,
            };

            if (product.RFQQuantity == null && priceBreaks.Count == 0)
            {
                dto.Product.Quantities = null;
            }

            return Ok(dto);
        }

        /// <summary>
        /// Get RFQ details by id = {id} 
        /// </summary>
        /// <param name="id">The ID of the product.</param>
        /// <param name="companyId">Company Id.</param>
        /// <param name="type">Company type is customer or vendor.</param>
        [HttpGet]
        [Route("rfqs/{id:int}")]
        [ResponseType(typeof(ProductDTO))]
        public async Task<IHttpActionResult> GetRFQDetails(int id, int companyId, TaskDataQuery.UserType type = TaskDataQuery.UserType.Customer)
        {
            var td = taskDataService.FindTaskDataListByProductId(id)
                        .Where(x => type == UserType.Vendor ?
                               (x.RFQBidId != null && x.RFQBid.VendorId == companyId || x.Product?.VendorId == companyId) :
                               (x.RFQBidId == null && x.Product.CustomerId == companyId ||
                                x.Product.CustomerId == companyId && x.Product.VendorId != null))
                        .LastOrDefault();

            if (td == null)
                return NotFound();

            Product originProduct = null;
            ICollection<PriceBreak> priceBreaks = null;
            if (td.StateId == (int)States.OutForRFQ && td.Product.ParentPartRevisionId != null)
            {
                var originProductId = partRevisionService.FindPartRevisionById(td.Product.ParentPartRevisionId.Value).OriginProductId;
                originProduct = productService.FindProductById(originProductId);
                priceBreaks = await priceBreakService.FindPriceBreaksByProductId(originProductId).Where(x => x.RFQBid.IsActive == true || x.RFQBid == null).OrderBy(x => x.Quantity).ToListAsync();
            }

            priceBreaks = priceBreaks?.ToNullIfEmpty()
                              ?? (await priceBreakService.FindPriceBreaksByTaskId(td.TaskId).Where(x => x.RFQBid.IsActive == true || x.RFQBid == null).OrderBy(x => x.Quantity).ToListAsync()).ToNullIfEmpty()
                              ?? (await priceBreakService.FindPriceBreaksByProductId(td.Product.Id).Where(x => x.RFQBid.IsActive == true || x.RFQBid == null).OrderBy(x => x.Quantity).ToListAsync());
            var pb = priceBreaks.FirstOrDefault();

            var prodDto = Mapper.Map<ProductDTO>(td.Product);
            prodDto.AvatarUri = prodDto?.AvatarUri != null ? prodDto.AvatarUri.Split(new[] { "%3FAWS" }, StringSplitOptions.None)[0] : null;
            prodDto.HarmonizedCode = td.RFQBid?.HarmonizedCode;

            prodDto.ToolingSetupCharges = pb?.ToolingSetupCharges;
            prodDto.CustomerToolingSetupCharges = pb?.CustomerToolingSetupCharges;

            prodDto.TaskId = td.TaskId;
            prodDto.State = (States)td.StateId;

            var product = (originProduct != null && originProduct.RFQQuantityId > 0) ? originProduct : td.Product;

            prodDto.Quantities = new Quantities
            {
                Qty1 = product?.RFQQuantity?.Qty1 ?? priceBreaks.ElementAtOrDefault(0)?.Quantity,
                Qty2 = product?.RFQQuantity?.Qty2 ?? priceBreaks.ElementAtOrDefault(1)?.Quantity,
                Qty3 = product?.RFQQuantity?.Qty3 ?? priceBreaks.ElementAtOrDefault(2)?.Quantity,
                Qty4 = product?.RFQQuantity?.Qty4 ?? priceBreaks.ElementAtOrDefault(3)?.Quantity,
                Qty5 = product?.RFQQuantity?.Qty5 ?? priceBreaks.ElementAtOrDefault(4)?.Quantity,
                Qty6 = product?.RFQQuantity?.Qty6 ?? priceBreaks.ElementAtOrDefault(5)?.Quantity,
                Qty7 = product?.RFQQuantity?.Qty7 ?? priceBreaks.ElementAtOrDefault(6)?.Quantity,
            };

            if (product.RFQQuantityId == null && priceBreaks.Count == 0)
            {
                prodDto.Quantities = null;
            }

            var docs = await documentService.FindDocumentsByTaskId(td.TaskId).ToListAsync();
            var documents = Mapper.Map<List<DocumentDTO>>(docs);

            var dto = new PartDetailsDTO
            {
                Product = prodDto,
                PriceBreaks = Mapper.Map<List<PriceBreakDTO>>(priceBreaks), //await priceBreaks.ProjectTo<PriceBreakDTO>().ToListAsync(),
                PartRvision = Mapper.Map<PartRvisionDTO>(td.Product.PartRevision),
                Documents = documents,
                IsEnterprise = td.isEnterprise,
                UnitOfMeasurement = product.RFQQuantityId != null ? product.RFQQuantity.UnitOfMeasurement : 0,
            };

            return Ok(dto);
        }

        /// <summary>
        /// Get a customer's RFQs
        /// </summary>
        /// <param name="companyId">Customer ID.</param>
        /// <param name="search">key word for searching</param>
        /// <param name="filter">Filter products into first-time order or reorder or non-filter</param>
        /// <param name="type">User type</param> 
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Numbe of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [HttpGet]
        [Route("companies/{companyId:int}/customer/RFQs", Name = "GetCustomerRFQs")]
        public async Task<PagedResultSet<ProductDTO>> GetCustomerRFQs(int companyId, string search = null, TaskDataQuery.RFQFilter filter = TaskDataQuery.RFQFilter.All, TaskDataQuery.UserType type = TaskDataQuery.UserType.Customer, int? page = 1, int pageSize = PageSize, string orderBy = nameof(ProductDTO.ModifiedUtc), bool ascending = false)
        {
            IQueryable<TaskData> tasks = Enumerable.Empty<TaskData>().AsQueryable();
            tasks = taskDataService.FindTaskDatasByCustomerId(companyId)
                .Where(x => x.RFQBidId == null || x.Product.VendorId != null);

            // check if there are any sharing parts by this company and display them as well
            var psTasks = productSharingService
                .FindProductSharingTaskDatasByComanyId(companyId);
            if (psTasks != null && psTasks.Any())
            {
                tasks = tasks.Union(psTasks);
            }
            tasks = tasks.FilterBy(filter, type);

            var requiredActionStates = filter == RFQFilter.All ? tasks.FilterBy(RFQFilter.ActionRequired, type).Select(x => x.StateId) :
                                                   (filter == RFQFilter.ActionRequired ? tasks.Select(x => x.StateId) : Enumerable.Empty<int>().AsQueryable());

            var rfqs = SetProductDTO(tasks, requiredActionStates).GroupBy(x => x.Id).Select(x => x.FirstOrDefault());
            if (!string.IsNullOrEmpty(search))
            {
                rfqs = SearchProductDTO(rfqs, search, type);
            }

            return await PageOfResultsSetAsync<ProductDTO>(rfqs, page, pageSize, orderBy, ascending, "GetCustomerRFQs");
        }

        /// <summary>
        /// Get a vendor's RFQs
        /// </summary>
        /// <param name="companyId">vendor ID.</param>
        /// <param name="search">key word for searching</param>
        /// <param name="filter">Filter products into first-time order or reorder or non-filter</param>
        /// <param name="type">User type</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Numbe of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [HttpGet]
        [Route("companies/{companyId:int}/vendor/rfqs", Name = "GetVendorRFQs")]
        public async Task<PagedResultSet<ProductDTO>> GetVendorRFQs(int companyId, string search = null, TaskDataQuery.RFQFilter filter = TaskDataQuery.RFQFilter.All, TaskDataQuery.UserType type = TaskDataQuery.UserType.Vendor, int? page = 1, int pageSize = PageSize, string orderBy = nameof(ProductDTO.ModifiedUtc), bool ascending = false)
        {
            var tasks = taskDataService.FindTaskDatas()
                .Where(x => x.RFQBidId != null && x.RFQBid.VendorId == companyId ||
                       (x.StateId == (int)States.OutForRFQ || x.StateId == (int)States.BidForRFQ) && x.Product.VendorId == companyId ||
                       x.StateId == (int)States.NCRCustomerRevisionNeeded && x.Product.VendorId == companyId)
                .FilterBy(filter, type);

            var requiredActionStates = filter == RFQFilter.All ? tasks.FilterBy(RFQFilter.ActionRequired, type).Select(x => x.StateId) :
                                                   (filter == RFQFilter.ActionRequired ? tasks.Select(x => x.StateId) : Enumerable.Empty<int>().AsQueryable());

            var rfqs = SetProductDTO(tasks, requiredActionStates).GroupBy(x => x.Id).Select(x => x.FirstOrDefault());
            if (!string.IsNullOrEmpty(search))
            {
                rfqs = SearchProductDTO(rfqs, search, type);
            }
            return await PageOfResultsSetAsync<ProductDTO>(rfqs, page, pageSize, orderBy, ascending, "GetVendorRFQs");
        }

        /// <summary>
        /// Get a customer's/vendor's Active RFQs
        /// </summary>
        /// <param name="companyId">vendor ID.</param>
        /// <param name="type">User type</param>
        /// <param name="search">key word for searching</param>
        /// <param name="filter">Filter products into first-time order or reorder or non-filter</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Numbe of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [HttpGet]
        [Route("companies/{companyId:int}/GetActiveRFQs", Name = "GetActiveRFQs")]
        public async Task<PagedResultSet<ProductDTO>> GetActiveRFQs(int companyId, TaskDataQuery.UserType type = TaskDataQuery.UserType.Vendor, string search = null, TaskDataQuery.RFQFilter filter = TaskDataQuery.RFQFilter.All,  int? page = 1, int pageSize = PageSize, string orderBy = nameof(ProductDTO.ModifiedUtc), bool ascending = false)
        {
            IQueryable<TaskData> tasks = taskDataService.FindTaskDatas().ActiveRFQs();
            if (type == UserType.Customer)
            {

                tasks = tasks.Where(x => x.ProductId != null && x.Product.CustomerId == companyId)                           
                             .FilterBy(filter, type);
            }
            else
            {
                tasks = tasks.Where(x => x.RFQBidId != null && x.RFQBid.VendorId == companyId)
                             .FilterBy(filter, type);

            }

            var requiredActionStates = filter == RFQFilter.All ? tasks.FilterBy(RFQFilter.ActionRequired, type).Select(x => x.StateId) :
                                                   (filter == RFQFilter.ActionRequired ? tasks.Select(x => x.StateId) : Enumerable.Empty<int>().AsQueryable());

            var rfqs = SetProductDTO(tasks, requiredActionStates).GroupBy(x => x.Id).Select(x => x.FirstOrDefault());
            if (!string.IsNullOrEmpty(search))
            {
                rfqs = SearchProductDTO(rfqs, search, type);
            }
            return await PageOfResultsSetAsync<ProductDTO>(rfqs, page, pageSize, orderBy, ascending, "GetActiveRFQs");
        }

        /// <summary>
        /// Get a customer's/vendor's RFQs in Bidding stage
        /// </summary>
        /// <param name="companyId">vendor ID.</param>
        /// <param name="type">User type</param>
        /// <param name="search">key word for searching</param>
        /// <param name="filter">Filter products into first-time order or reorder or non-filter</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Numbe of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [HttpGet]
        [Route("companies/{companyId:int}/GetBiddingRFQs", Name = "GetBiddingRFQs")]
        public async Task<PagedResultSet<ProductDTO>> GetBiddingRFQs(int companyId, TaskDataQuery.UserType type = TaskDataQuery.UserType.Vendor, string search = null, TaskDataQuery.RFQFilter filter = TaskDataQuery.RFQFilter.All, int? page = 1, int pageSize = PageSize, string orderBy = nameof(ProductDTO.ModifiedUtc), bool ascending = false)
        {
            IQueryable<TaskData> tasks = taskDataService.FindTaskDatas().BiddingRFQs();
            if (type == UserType.Customer)
            {

                tasks = tasks.Where(x => x.ProductId != null && x.Product.CustomerId == companyId)
                             .FilterBy(filter, type);
            }
            else
            {
                tasks = tasks.Where(x => x.RFQBidId != null && x.RFQBid.VendorId == companyId)
                             .FilterBy(filter, type);

            }

            var requiredActionStates = filter == RFQFilter.All ? tasks.FilterBy(RFQFilter.ActionRequired, type).Select(x => x.StateId) :
                                                   (filter == RFQFilter.ActionRequired ? tasks.Select(x => x.StateId) : Enumerable.Empty<int>().AsQueryable());

            var rfqs = SetProductDTO(tasks, requiredActionStates).GroupBy(x => x.Id).Select(x => x.FirstOrDefault());
            if (!string.IsNullOrEmpty(search))
            {
                rfqs = SearchProductDTO(rfqs, search, type);
            }
            return await PageOfResultsSetAsync<ProductDTO>(rfqs, page, pageSize, orderBy, ascending, "GetBiddingRFQs");
        }


        /// <summary>
        /// Get a customer's/vendor's RFQ Statitics
        /// </summary>
        /// <param name="companyId">Company ID.</param>
        /// <param name="mode">User type</param>
        [HttpGet]
        [Route("companies/{companyId:int}/GetRFQStatistics", Name = "GetRFQStatistics")]
        public ActiveRFQsStatisticsDTO GetRFQStatistics(int companyId, TaskDataQuery.UserType mode = TaskDataQuery.UserType.Vendor)
        {
            IQueryable<TaskData> tasks = taskDataService.FindTaskDatas().WhereIsRFQs().ActiveRFQStatistics();
            int? totalCustomers = null;
            int? totalVendors = null;
            int numberOfActionableReviewRFQs = 0;
            int numberOfActionableBiddingRFQs = 0;

            if (mode == UserType.Customer)
            {
                tasks = tasks.Where(x => x.ProductId != null && x.Product.CustomerId == companyId && x.RFQBidId != null);
                totalVendors = tasks
                    .Where(x => x.RFQBidId != null)
                    .GroupBy(x => x.RFQBid.VendorId)
                    .Select(x => x.FirstOrDefault().RFQBid.VendorId)
                    .Count();
                numberOfActionableReviewRFQs = tasks.CustomerActionableReviewRFQs().Count();
                numberOfActionableBiddingRFQs = tasks.CustomerActionableBiddingRFQs().Count();
            }
            else
            {
                tasks = tasks.Where(x => x.RFQBidId != null && x.RFQBid.VendorId == companyId);
                totalCustomers = tasks.GroupBy(x => x.Product.CustomerId)
                                      .Select(x => x.FirstOrDefault().Product.CustomerId)                                      
                                      .Count();

                numberOfActionableReviewRFQs = tasks.VendorActionableReviewRFQs().Count();
                numberOfActionableBiddingRFQs = tasks.VendorActionableBiddingRFQs().Count();
            }

            ActiveRFQsStatisticsDTO stats = new ActiveRFQsStatisticsDTO
            {
                NumberOfAllActiveRFQs = tasks.Count(),
                UniqueVendorsOrCustomersInvolvedInActiveRFQs = mode == UserType.Customer? totalVendors : totalCustomers,
                NumberOfReviewRFQs = tasks.ActiveRFQs().Count(),
                NumberOfActionableReviewRFQs = numberOfActionableReviewRFQs,
                NumberOfBiddingRFQs = tasks.BiddingRFQs().Count(),
                NumberOfActionableBiddingRFQs = numberOfActionableBiddingRFQs,
            };

            return stats;
        }


        /// <summary>
        /// Assign RFQ to vendors
        /// </summary>
        /// <param name="pid">Product ID.</param>
        /// <param name="vendors">List of vendors chosen to bid this RFQ</param>     
        [ResponseType(typeof(AssignRFQResult))]
        [HttpPost]
        [Route("rfqs/user/{userId}/assignrfqtovendors/{pid:int}", Name = "AssignRFQToVendors")]
        public IHttpActionResult AssignRFQToVendors(int pid, [FromUri] int[] vendors)
        {
            try
            {
                var res = productBL.AssignRFQToVendors(pid, vendors);
                if (res.alreadySelected)
                {
                    return BadRequest(IndicatingMessages.VendorHaseBeenSelected);
                }
                return CreatedAtRoute("AssignRFQToVendors", new { id = res.productId }, res);
            }
            catch (Exception ex)
            {
                return BadRequest("AssignRFQToVendors: " + ex.RetrieveErrorMessage());
            }
        }

        private string SetupTimer(int pid, TypeOfTimers type, string revisionRequestInterval, string bidInterval)
        {
            var td = taskDataService.FindTaskDataListByProductId(pid).FirstOrDefault(x => x.RFQBidId == null);

            var setupTimerVM = new SetupTimerViewModel
            {
                ProductId = pid,
                TaskId = td.TaskId,
                isEnterprise = td.isEnterprise,
                RevisionRequestTimerInterval = revisionRequestInterval,
                BidTimerInterval = bidInterval,
                TimerType = type,
            };

            Log.Information("SetupTimer - Type:{type} {@Data}", type, setupTimerVM);
            return taskDataBL.SetupTimer(setupTimerVM);

        }



        /// <summary>
        /// Setup Bid Timer or Revision Request Timer, also change existing timer
        /// </summary>
        /// <param name="pid">Product ID.</param>
        /// <param name="timerType">Type of the timer</param>
        /// <param name="revisionRequestInterval">Revision Request Interval</param>
        /// <param name="bidInterval">Bid Interval</param>
        [HttpPost]
        [Route("rfqs/user/{userId}/SetupTimers/{pid:int}", Name = "SetupTimers_1")]
        public IHttpActionResult SetupTimers(int pid, string revisionRequestInterval, string bidInterval, TypeOfTimers timerType = TypeOfTimers.RFQRevisionTimer)
        {
            try
            {
                var error = SetupTimer(pid, timerType, revisionRequestInterval, bidInterval);
                if (error != null)
                {
                    return BadRequest("SetupTimers_1: " + error);
                }
                return CreatedAtRoute("SetupTimers_1", new { id = pid }, "Success");
            }
            catch (Exception ex)
            {
                return BadRequest("SetupTimers_1: " + ex.RetrieveErrorMessage());
            }
        }


        /// <summary>
        /// Setup Bid Timer or Revision Request Timer, also change existing timer
        /// </summary>
        /// <param name="pid">Product ID.</param>
        /// <param name="interval">Timer interval in days</param>
        /// <param name="timerType">Type of the timer</param>
        /// <param name="flag">Action of either Extend time limit or Asisgn new vendors</param>
        [HttpPost]
        [Route("rfqs/user/{userId}/SetupTimers/{pid:int}", Name = "SetupTimers")]
        public IHttpActionResult SetupTimers(int pid, string interval, TypeOfTimers timerType = TypeOfTimers.RFQRevisionTimer, ActionFlag? flag = null)
        {
            try
            {
                var error = taskDataBL.SetupTimer(pid, interval, timerType, flag);
                if (error != null)
                {
                    return BadRequest("SetupTimers: " + error);
                }
                return CreatedAtRoute("SetupTimers", new { id = pid }, "Success");
            }
            catch (Exception ex)
            {
                return BadRequest("SetupTimers: " + ex.RetrieveErrorMessage());
            }
        }



        /// <summary>
        /// Get Bid and RFQ Revision Timer Intervals
        /// </summary>
        /// <param name="pid">Product ID.</param>
        [HttpGet]
        [Route("rfqs/user/{userId}/GetTimerIntervals/{pid:int}", Name = "GetTimerIntervals")]
        public IHttpActionResult GetTimerIntervals(int pid)
        {
            var timer = timerSetupService.FindAllTimerSetupsByProductId(pid).ToList();

            List<TimerSetup> list = new List<TimerSetup>
            {
                timer.Where(x => x.TimerType == TypeOfTimers.RFQRevisionTimer).LastOrDefault(),
                timer.Where(x => x.TimerType == TypeOfTimers.BidTimer).LastOrDefault(),
            };

            var dto = Mapper.Map<List<GetTimerIntervalsDTO>>(list);

            return Ok(dto);
        }


        /// <summary>
        /// Create a RFQ (new product) 
        /// </summary>
        [ResponseType(typeof(ProductDTO))]
        [HttpPost]
        [Route("rfqs/user/{userId}/createrfq", Name = "CreateRFQ")]
        public async Task<IHttpActionResult> CreateRFQ()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            List<HttpPostedFileBase> files = new List<HttpPostedFileBase>();
            string root = HttpContext.Current.Server.MapPath(@"~/Docs");
            if (!System.IO.Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }

            var httpRequest = HttpContext.Current.Request;
            var formDataDic = httpRequest.Form.ToDictionaryOfObjects();

            // Check in case build type is Process and material is Process Type
            if (formDataDic.ContainsKey("processType") && Process_Type.TryParse(formDataDic["processType"].ToString(), out Process_Type processType))
            {
                if (processType == Process_Type.Anodizing &&
                    !(formDataDic.ContainsKey("anodizingType") && Process_Type.TryParse(formDataDic["anodizingType"].ToString(), out Anodizing_Type anodizing)))
                {
                    return BadRequest(IndicatingMessages.MissingFormData);
                }
            }
            if (formDataDic.ContainsKey("partNumber") == false || formDataDic.ContainsKey("partNumberRevision") == false)
                return BadRequest(IndicatingMessages.MissingFormData);

            var customerId = userContext.Company.Id;
            var partNumber = formDataDic["partNumber"].ToString();
            var parNumberRevision = formDataDic["partNumberRevision"].ToString();
            var res = IsUniqueRFQOn3Factors(productService, customerId, partNumber, parNumberRevision);
            if (res == false)
                return BadRequest(IndicatingMessages.DuplicatedRFQ);

            var mod = Slapper.AutoMapper.Map<ProductApiViewModel>(formDataDic, false);

            var model = Mapper.Map<ProductViewModel>(mod);
            model.QuantityList = new List<decimal?>();
            if (mod.Qty1 != null)
            {
                model.QuantityList.Add(mod.Qty1.Value);
            }
            if (mod.Qty2 != null)
            {
                model.QuantityList.Add(mod.Qty2.Value);
            }
            if (mod.Qty3 != null)
            {
                model.QuantityList.Add(mod.Qty3.Value);
            }
            if (mod.Qty4 != null)
            {
                model.QuantityList.Add(mod.Qty4.Value);
            }
            if (mod.Qty5 != null)
            {
                model.QuantityList.Add(mod.Qty5.Value);
            }
            if (mod.Qty6 != null)
            {
                model.QuantityList.Add(mod.Qty6.Value);
            }
            if (mod.Qty7 != null)
            {
                model.QuantityList.Add(mod.Qty7.Value);
            }

            model.QuantityList.Sort();

            var request = HttpContext.Current.Request;
            if (request.Files == null || request.Files.Count == 0 || request.Files[0] == null)
            {
                return BadRequest(IndicatingMessages.ForgotUploadFile);
            }
            for (int i = 0; i < request.Files.Count; i++)
            {
                string path = request.Files[i].FileName;
                var filePath = Path.Combine(root, path);

                if (!System.IO.File.Exists(filePath))
                {
                    request.Files[i].SaveAs(filePath);
                }

                var postedFileBase = new MemoryFile(filePath, request.Files[i].ContentType);
                if (request.Files[i].ContentType.Contains("image/") == true)
                {
                    model.AvatarFile = postedFileBase;
                    continue;
                }
                files.Add(postedFileBase);
            }

            using (var trans = AsyncTransactionScope.StartNew(transOptions))
            {
                try
                {
                    Product product = Mapper.Map<Product>(model);
                    product.CustomerId = customerId;
                    var result = await productBL.CreateAsync(model, product);
                    var td = taskDataService.FindTaskDataByProductId(result.PartResult.NewProductId);
                    var newProductDTO = Mapper.Map<ProductDTO>(td.Product);
                    newProductDTO.TaskId = td.TaskId;
                    newProductDTO.State = (States)td.StateId;
                    newProductDTO.Quantities = Mapper.Map<Quantities>(td.Product.RFQQuantity);
                    newProductDTO.OriginProductId = model.OriginProductId;
                    newProductDTO.UnitOfMeasurement = product.RFQQuantityId != null ? product.RFQQuantity.UnitOfMeasurement : 0; 

                    string result2 = documentBL.UploadProductDocs(files, product.Id, result.PartResult.NewTaskId);
                    if (result2 != null)
                    {
                        return BadRequest(result2);
                    }

                    newProductDTO.Product2DDocUri = documentService.FindDocumentByProductIdDocType(product.Id, DOCUMENT_TYPE.PRODUCT_2D_PDF).Select(x => x.DocUri).ToArray();
                    newProductDTO.Product3DDocUri = documentService.FindDocumentByProductIdDocType(product.Id, DOCUMENT_TYPE.PRODUCT_3D_STEP).Select(x => x.DocUri).ToArray();
                    files.Add(model.AvatarFile);
                    trans.Complete();
                    return CreatedAtRoute("CreateRFQ", new { id = result.PartResult.NewProductId }, newProductDTO);
                }
                catch (Exception ex)
                {
                    return BadRequest("CreateAsync: " + ex.RetrieveErrorMessage());
                }
                finally
                {
                    DeleteUploadFiles(files);
                }
            }
        }


        /// <summary>
        /// Get a list of qualified vendors by product id 
        /// </summary>
        /// <param name="productId">The ID of the product.</param>
        /// <param name="search">search string to be matched or contained</param>
        /// <param name="vendorType">Vendor type: Myself, My Vendors or Network Vendors</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Numbe of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [HttpGet]
        [Route("rfqs/getqualifiedvendors/{productId:int}", Name = "GetQualifiedVendors")]
        public PagedResultSet<VendorStatsViewModel> GetQualifiedVendors(int productId,
                                                                        string search = null,
                                                                        VENDOR_TYPE vendorType = VENDOR_TYPE.NetworkVendors,
                                                                        int? page = 1, int pageSize = 10, string orderBy = null, bool ascending = false)
        {
            var result = productBL.GetQualifiedVendors(productId, vendorType, search);
            return PageOfResultsSet<VendorStatsViewModel>(result, page, pageSize, orderBy, ascending, "GetQualifiedVendors");
        }

        /// <summary>
        /// Get a list of qualified vendors by build type, material type, process type(optional) 
        /// </summary>
        /// <param name="processType">Process Type</param>
        /// <param name="buildType">RFQ build type</param>
        /// <param name="materialType">RFQ material type</param>
        /// <param name="vendorType">Vendor type: Myself, My Vendors or Network Vendors</param>
        /// <param name="search">search string to be matched or contained</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Numbe of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [HttpGet]
        [Route("rfqs/customer/{userId}/GetQualifiedVendorsByParam", Name = "GetQualifiedVendorsByParam")]
        public PagedResultSet<VendorStatsViewModel> GetQualifiedVendorsByParam(
                                                                                BUILD_TYPE buildType,
                                                                                VENDOR_TYPE vendorType = VENDOR_TYPE.NetworkVendors,
                                                                                MATERIALS_TYPE? materialType = null,
                                                                                int? processType = null,
                                                                                string search = null,
                                                                                int? page = 1, int pageSize = 10,
                                                                                string orderBy = nameof(VendorStatsViewModel.CompletedOrders),
                                                                                bool ascending = false)
        {
            var customerId = userContext.Company.Id;
            var result = productBL.GetQualifiedVendors(customerId, buildType, vendorType, materialType, processType, search);
            return PageOfResultsSet<VendorStatsViewModel>(result, page, pageSize, orderBy, ascending, "GetQualifiedVendorsByParam");
        }


        /// <summary>
        /// Get Chart Data by company id
        /// </summary>
        /// <param name="id">Company id</param>
        /// <param name="isVendorMode">Is in Vendor/Customer mode</param>
        /// <param name="filter">NCR Chart Filter</param>
        /// <param name="filterValue">Filter value</param>
        [ResponseType(typeof(RFQChartDataViewModel))]
        [HttpGet]
        [Route("products/getchartdata/{id:int}")]
        public IHttpActionResult GetChartData(int id, bool isVendorMode, NCR_CHART_FILTERS? filter = null, int? filterValue = null, DateTime? StartDatetime = null, DateTime? EndDatetime = null)
        {
            UserMode mode = isVendorMode == true ? UserMode.Vendor : UserMode.Customer;
            var result = chartBL.GetChartData(id, mode, filter, filterValue, null, StartDatetime, EndDatetime);
            return Ok(result);
        }

        /// <summary>
        /// Validate if RFQ aleady exists, if it is unique return true else false.
        /// </summary>
        /// <param name="customerId">Customer Id</param>
        /// <param name="partNumber">Part Number</param>
        /// <param name="partNumberRevision">Part Number Revision</param>
        [ResponseType(typeof(bool))]
        [HttpGet]
        [Route("rfqs/IsUniqueRFQ")]
        public IHttpActionResult IsUniqueRFQ(int customerId, string partNumber, string partNumberRevision)
        {
            var prod = productService.FindProductByPartNumberOfACustomer(customerId, partNumber, partNumberRevision);
            return Ok(prod == null);
        }


        /// <summary>
        /// Check if the RFQ is unique in combination of 3 factors of customerId, part Number and revision number, 
        /// among all RFQs during RFQ stage
        /// if it has unique combination return true else false.
        /// </summary>
        /// <param name="customerId">Customer Id</param>
        /// <param name="partNumber">Part Number</param>
        /// <param name="partNumberRevision">Part Number Revision</param>
        [ResponseType(typeof(bool))]
        [HttpGet]
        [Route("rfqs/IsUniqueRFQOn3Factors")]
        public IHttpActionResult IsUniqueRFQOn3Factors(int customerId, string partNumber, string partNumberRevision)
        {
            var result = IsUniqueRFQOn3Factors(productService, customerId, partNumber, partNumberRevision);
            return Ok(result);
        }

        /// <summary>
        ///  Check if the product is unique in combination of 3 factors of customerId, part Number and revision number
        ///  among all products, 
        /// </summary>
        /// <param name="customerId">Customer Id</param>
        /// <param name="partNumber">Part Number</param>
        /// <param name="partNumberRevision">Part Number Revision</param>
        [ResponseType(typeof(bool))]
        [HttpGet]
        [Route("products/IsUniqueProductOn3Factors")]
        public IHttpActionResult IsUniqueProductOn3Factors(int customerId, string partNumber, string partNumberRevision)
        {
            var result = IsUniqueProductOn3Factors(productService, customerId, partNumber, partNumberRevision);
            return Ok(result);
        }

        /// <summary>
        ///  Check if the product is unique in combination of 4 factors of customerId, vendorid, part Number and revision number 
        ///  among all products, 
        /// </summary>
        /// <param name="customerId">Customer Id</param>
        /// <param name="vendorId">Vendor Id</param>
        /// <param name="partNumber">Part Number</param>
        /// <param name="partNumberRevision">Part Number Revision</param>
        [ResponseType(typeof(bool))]
        [HttpGet]
        [Route("products/IsUniqueProductOn4Factors")]
        public IHttpActionResult IsUniqueProductOn4Factors(int customerId, int vendorId, string partNumber, string partNumberRevision)
        {
            var result = IsUniqueProductOn4Factors(productService, customerId, vendorId, partNumber, partNumberRevision);
            return Ok(result);
        }

        /// <summary>
        /// Vendor Bid for RFQ.
        /// </summary>
        /// <param name="pid">Product/RFQ Id</param>
        [HttpPost]
        [Route("rfqs/user/{userId}/vendorbidforrfq/{pid:int}", Name = "VendorBidForRFQ")]
        public async Task<IHttpActionResult> VendorBidForRFQ(int pid)
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            var vendorId = userContext.Company.Id;
            var tds = taskDataService.FindTaskDataListByProductId(pid);
            TaskData td = tds.LastOrDefault(t => t.RFQBidId != null && t.RFQBid.VendorId == vendorId &&
                                                 (t.StateId == (int)States.OutForRFQ || t.StateId == (int)States.RFQRevision ||
                                                  t.StateId == (int)States.BidForRFQ || t.StateId == (int)States.RFQBidUpdateQuantity) ||
                                                 t.RFQBidId == null &&
                                                 (t.StateId == (int)States.OutForRFQ || t.StateId == (int)States.RFQRevision ||
                                                 t.StateId == (int)States.BidForRFQ || t.StateId == (int)States.RFQBidUpdateQuantity));

            if (td == null)
            {
                return BadRequest(IndicatingMessages.TaskNotFound);
            }

            if (td.StateId == (int)States.PendingRFQRevision || td.StateId == (int)States.BackFromRFQ)
            {
                return BadRequest(IndicatingMessages.YouMustWaitUntilCustomerCompleteRevision);
            }
            var files = GetPostedFiles(homeBL);

            try
            {
                var httpRequest = HttpContext.Current.Request;
                var formDataDic = httpRequest.Form.ToDictionaryOfObjects();
                RFQApiViewModel mod = Slapper.AutoMapper.Map<RFQApiViewModel>(formDataDic, false);

                RFQViewModel model = Mapper.Map<RFQViewModel>(mod);
                RFQViewModel tmp = dashBoardBL.SetupRFQBidVM(td);

                if (model == null)
                {
                    return BadRequest(IndicatingMessages.MissingShppingAddress);
                }
                var product = productService.FindProductById(pid);
                var unitOfMeasurement = product.RFQQuantityId != null ? product.RFQQuantity.UnitOfMeasurement : 0;
                model.Id = pid;
                model.PriceBreakVM = tmp.PriceBreakVM;
                model.ShippingQuoteVM = tmp.ShippingQuoteVM;
                if (!model.PriceBreakVM.PriceBreakList.Any())
                {
                    return BadRequest(IndicatingMessages.PriceBreaksNotFound);
                }
                model.isEnterprise = true;
                model.TaskId = td.TaskId;
                model.PriceBreakVM.PriceBreakList[0].Quantity = mod.Qty1;
                model.PriceBreakVM.PriceBreakList[0].VendorUnitPrice = mod.UnitPrice1;
                model.PriceBreakVM.PriceBreakList[0].UnitOfMeasurement = unitOfMeasurement;

                if (mod.Qty2 != null && mod.UnitPrice2 != null)
                {
                    model.PriceBreakVM.PriceBreakList[1].Quantity = mod.Qty2.Value;
                    model.PriceBreakVM.PriceBreakList[1].VendorUnitPrice = mod.UnitPrice2.Value;
                    model.PriceBreakVM.PriceBreakList[1].UnitOfMeasurement = unitOfMeasurement;
                }
                if (mod.Qty3 != null && mod.UnitPrice3 != null)
                {
                    model.PriceBreakVM.PriceBreakList[2].Quantity = mod.Qty3.Value;
                    model.PriceBreakVM.PriceBreakList[2].VendorUnitPrice = mod.UnitPrice3.Value;
                    model.PriceBreakVM.PriceBreakList[2].UnitOfMeasurement = unitOfMeasurement;
                }
                if (mod.Qty4 != null && mod.UnitPrice4 != null)
                {
                    model.PriceBreakVM.PriceBreakList[3].Quantity = mod.Qty4.Value;
                    model.PriceBreakVM.PriceBreakList[3].VendorUnitPrice = mod.UnitPrice4.Value;
                    model.PriceBreakVM.PriceBreakList[3].UnitOfMeasurement = unitOfMeasurement;
                }
                if (mod.Qty5 != null && mod.UnitPrice5 != null)
                {
                    model.PriceBreakVM.PriceBreakList[4].Quantity = mod.Qty5.Value;
                    model.PriceBreakVM.PriceBreakList[4].VendorUnitPrice = mod.UnitPrice5.Value;
                    model.PriceBreakVM.PriceBreakList[4].UnitOfMeasurement = unitOfMeasurement;
                }
                if (mod.Qty6 != null && mod.UnitPrice6 != null)
                {
                    model.PriceBreakVM.PriceBreakList[5].Quantity = mod.Qty6.Value;
                    model.PriceBreakVM.PriceBreakList[5].VendorUnitPrice = mod.UnitPrice6.Value;
                    model.PriceBreakVM.PriceBreakList[5].UnitOfMeasurement = unitOfMeasurement;
                }
                if (mod.Qty7 != null && mod.UnitPrice7 != null)
                {
                    model.PriceBreakVM.PriceBreakList[6].Quantity = mod.Qty7.Value;
                    model.PriceBreakVM.PriceBreakList[6].VendorUnitPrice = mod.UnitPrice7.Value;
                    model.PriceBreakVM.PriceBreakList[6].UnitOfMeasurement = unitOfMeasurement;
                }

                model.ShippingQuoteVM.BkgDetails.DimensionUnit = mod.DimensionUnit;
                model.ShippingQuoteVM.BkgDetails.WeightUnit = mod.WeightUnit;
                model.ShippingQuoteVM.BkgDetails.Pieces[0].Height = mod.Height;
                model.ShippingQuoteVM.BkgDetails.Pieces[0].Depth = mod.Depth;
                model.ShippingQuoteVM.BkgDetails.Pieces[0].Width = mod.Width;
                model.ShippingQuoteVM.BkgDetails.Pieces[0].Weight = mod.Weight;
                model.ShippingQuoteVM.BkgDetails.NumberPartsPerPiece = mod.NumberPartsPerPiece;


                var result = await homeBL.RFQBidReview(model, files);

                return CreatedAtRoute("VendorBidForRFQ", new { id = result }, result == 0 ? "Success" : "Revision timer timeout, you may continue");
            }
            catch (Exception ex)
            {
                return BadRequest("RFQBidReview: " + ex.RetrieveErrorMessage());
            }
            finally
            {
                DeleteUploadFiles(files);
            }
        }

        /// <summary>
        /// Get Vendor Bid RFQ Submission Summary.
        /// </summary>
        /// <param name="pid">Product/RFQ Id</param>
        [ResponseType(typeof(VendorSubmittedRFQ))]
        [HttpGet]
        [Route("rfqs/vendor/{userId}/getbidrfqsummary/{pid:int}", Name = "GetBidRFQSummary")]
        public async Task<IHttpActionResult> GetBidRFQSummary(int pid)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest("Invalid User or Customer");

            var td = taskDataService.FindTaskDataListByProductId(pid)
                                    .Where(x => x.RFQBid?.VendorId == userContext.Company.Id || x.Product?.VendorId == userContext.Company.Id)
                                    .FirstOrDefault();
            if (td == null)
                return BadRequest(IndicatingMessages.TaskNotFound);

            var product = td.Product;

            documentService.UpdateDocUrlWithSecurityToken(product?.Documents);
            var docs = product.Documents.Where(x => x.DocType == (int)DOCUMENT_TYPE.QUOTE_PDF && x.TaskId == td.TaskId).Select(x => x.DocUri).ToList();


            var priceBreaks = priceBreakService.FindPriceBreaksByTaskId(td.TaskId);
            var pbDtoList = priceBreaks != null ? await priceBreaks.ProjectTo<PriceBreakDTO>().ToListAsync() : null;
            var vendorId = td.RFQBid?.VendorId ?? product.VendorId.Value;
            var model = new VendorSubmittedRFQ
            {
                TaskId = td.TaskId,
                VendorId = vendorId,
                State = (States)td.StateId,
                SampleLeadTime = td.RFQBid?.SampleLeadTime ?? product.SampleLeadTime.Value,
                ProductionLeadTime = td.RFQBid?.ProductLeadTime ?? product.ProductionLeadTime.Value,
                NumberSampleIncluded = priceBreaks != null && priceBreaks.Any() ? priceBreaks.First().NumberSampleIncluded.Value : 0,
                HarmonizedCode = td.RFQBid?.HarmonizedCode ?? product.HarmonizedCode,
                QuoteDocUri = docs,
                PriceBreaks = pbDtoList,
                PreferredCurrency = td.RFQBid?.PreferredCurrency ?? product.VendorCompany?.Currency,
            };
            return Ok(model);
        }

        /// <summary>
        /// Get Vendor Bid RFQ Submission Summary.
        /// </summary>
        /// <param name="pid">Product/RFQ Id</param>
        /// <param name="vid">Vendor Id</param>
        [ResponseType(typeof(VendorSubmittedRFQ))]
        [HttpGet]
        [Route("rfqs/customer/{userId}/GetVendorBidDetails/{pid:int}", Name = "GetVendorBidDetails")]
        public async Task<IHttpActionResult> GetVendorBidDetails(int pid, int vid)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest("Invalid User or Customer");

            var td = taskDataService.FindTaskDataListByProductId(pid).Where(x => x.RFQBid?.VendorId == vid).FirstOrDefault();
            if (td == null)
                return Ok(new VendorSubmittedRFQ());

            var product = td.Product;

            documentService.UpdateDocUrlWithSecurityToken(td?.Product?.Documents);
            var docs = product.Documents.Where(x => x.DocType == (int)DOCUMENT_TYPE.QUOTE_PDF && x.TaskId == td.TaskId).Select(x => x.DocUri).ToList(); //TODO: 

            var priceBreaks = priceBreakService.FindPriceBreaksByTaskId(td.TaskId);
            var pbDtoList = await priceBreaks.ProjectTo<PriceBreakDTO>().ToListAsync();

            VendorSubmittedRFQ model = new VendorSubmittedRFQ
            {
                TaskId = td.TaskId,
                VendorId = vid,
                State = (States)td.StateId,
                SampleLeadTime = product.SampleLeadTime.Value,
                ProductionLeadTime = product.ProductionLeadTime.Value,
                NumberSampleIncluded = priceBreaks != null && priceBreaks.Any() ? priceBreaks.First().NumberSampleIncluded.Value : 0,
                HarmonizedCode = td.RFQBid?.HarmonizedCode,
                QuoteDocUri = docs,
                PriceBreaks = pbDtoList,
            };

            return Ok(model);
        }

        /// <summary>
        /// Customer Get Bid Summary List from Vendors.
        /// </summary>
        /// <param name="pid">Product/RFQ Id</param>
        [ResponseType(typeof(RFQBidSummary))]
        [HttpGet]
        [Route("rfqs/customer/{userId}/GetBidRFQSummaryList/{pid:int}", Name = "GetBidRFQSummaryList")]
        public async Task<IHttpActionResult> GetBidRFQSummaryList(int pid)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest("Invalid User or Customer");

            var prod = productService.FindProductById(pid);
            if (!productBL.IsUsersProduct(prod))
            {
                return BadRequest(IndicatingMessages.InvalidAccess);
            }

            var originalTask = taskDataService.FindTaskDataByProductId(pid);

            if (originalTask == null)
            {
                //return BadRequest(IndicatingMessages.TaskNotFound);
                return Ok(new RFQBidSummary());
            }
            var bidStatus = bidRFQStatusService.FindBidRFQStatusByProductId(pid)
                .Where(x => x.StateId == originalTask.StateId)
                .ToList()
                .LastOrDefault();

            RFQBidSummary model = new RFQBidSummary
            {
                TaskId = originalTask.TaskId,
                State = (States)originalTask.StateId,
                KeepCurrentRevisionReason = bidStatus?.KeepCurrentRevisionReason,
                CancelRFQReason = bidStatus?.RFQActionReason?.Reason,
                CancelRFQDescription = bidStatus?.RFQActionReason?.Description,
            };
            var lowestPriceBreak = priceBreakService.FindPriceBreakByProductId(pid).Where(x => x.UnitPrice > 0).OrderBy(x => x.UnitPrice).FirstOrDefault();
            PartDetails partDetails = new PartDetails
            {
                PartName = prod.Name,
                BuildType = prod.BuildType,
                MaterialType = prod.Material,
                SampleLeadTime = prod.SampleLeadTime,
                ProductionLeadTime = prod.ProductionLeadTime,
                PartNumber = prod.PartNumber,
                RevisionNumber = prod.PartRevision.Name,


                //NumberSampleIncluded = lowestPriceBreak?.NumberSampleIncluded,
                //UnitPrice = lowestPriceBreak?.UnitPrice,
                //Quantity = lowestPriceBreak?.Quantity,
            };

            var tds = taskDataService.FindTaskDataListByProductId(pid)
                                    .Where(x => x.RFQBidId != null &&
                                    !(x.StateId == (int)States.VendorRejectedRFQ ||
                                        x.StateId == (int)States.RFQBidComplete ||
                                        x.StateId == (int)States.BidTimeout) ||

                                        // in case it is new production revision
                                        x.RFQBidId == null && x.StateId == (int)States.BidForRFQ && x.Product.VendorId != null);
            List<BidDetails> bidDetailsList = new List<BidDetails>();
            foreach (var td in tds)
            {
                var product = td.Product;
                var priceBreaks = priceBreakService.FindPriceBreaksByTaskId(td.TaskId);
                var pbDtoList = await priceBreaks.ProjectTo<PriceBreakDTO>().ToListAsync();
                PriceBreak pb = null;
                UserPerformance vendorPerformance = new UserPerformance();

                var vendorId = td.RFQBid?.VendorId ?? product.VendorId.Value;
                UserPerformanceViewModel vp = homeBL.GetUserPerformance(vendorId, CompanyType.Vendor);
                
                if (vp != null)
                {
                    vendorPerformance = Mapper.Map<UserPerformance>(vp);
                }
                if (priceBreaks.ToList().Any())
                {
                    pb = priceBreaks.OrderByDescending(x => x.ShippingUnitPrice).ToList().Last();
                }
                vendorPerformance.UserName = td.RFQBid?.VendorCompany?.Name ?? product.VendorCompany?.Name;
                vendorPerformance.UserLocation = Mapper.Map<AddressDTO>(td.RFQBid?.VendorCompany?.Address ?? product.VendorCompany?.Address);

                var quoteExpireDate = productPriceQuoteService.FindProductPriceQuotes(pid, vendorId).FirstOrDefault();

                var bidDetails = new BidDetails
                {
                    VendorId = vendorId,
                    VendorStatus = (States)td.StateId,
                    VendorName = td.RFQBid?.VendorCompany?.Name ?? product.VendorCompany?.Name,
                    //QuoteDocUri = docs,
                    //PriceBreaks = pbDtoList,
                    HarmonizedCode = td.RFQBid?.HarmonizedCode,

                    SampleTime = td.RFQBid?.SampleLeadTime ?? product.SampleLeadTime,
                    ProductionLeadTime = td.RFQBid?.ProductLeadTime ?? product.ProductionLeadTime,

                    NumberSampleIncluded = pb?.NumberSampleIncluded,
                    EstShippingTime = pb?.ShippingDays,
                    UnitShipping = pb?.ShippingUnitPrice,
                    AvrShipping = priceBreaks?.Average(x => x.ShippingUnitPrice),
                    ToolingCharge = pb?.ToolingSetupCharges,

                    QuoteDocUri = documentService.FindDocumentByTaskIdDocType(td.TaskId, DOCUMENT_TYPE.QUOTE_PDF).Select(d => d.DocUri).ToList(),
                    Quantities = new Quantities
                    {
                        Qty1 = product.RFQQuantity.Qty1 ?? null,
                        Qty2 = product.RFQQuantity.Qty2 ?? null,
                        Qty3 = product.RFQQuantity.Qty3 ?? null,
                        Qty4 = product.RFQQuantity.Qty4 ?? null,
                        Qty5 = product.RFQQuantity.Qty5 ?? null,
                        Qty6 = product.RFQQuantity.Qty6 ?? null,
                        Qty7 = product.RFQQuantity.Qty7 ?? null,
                    },
                    VendorPerformance = vendorPerformance,
                    PreferredCurrency = td.RFQBid?.PreferredCurrency,
                    QuoteExpireDate = quoteExpireDate?.ExpireDate,
                };

                foreach (var p in pbDtoList)
                {
                    bidDetails.UnitPrices.Add(p.UnitPrice.Value);
                }
                bidDetailsList.Add(bidDetails);

            }
            model.PartDetails = partDetails;
            model.BidDetails = bidDetailsList;

            return Ok(model);
        }

        /// <summary>
        /// List all vendors who are participating the bid.
        /// </summary>
        /// <param name="pid">Product/RFQ Id</param>
        [ResponseType(typeof(RFQBidSummary))]
        [HttpGet]
        [Route("rfqs/customer/{userId}/GetRFQVendors/{pid:int}", Name = "GetRFQVendors")]
        public async Task<IHttpActionResult> GetRFQVendors(int pid)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest("Invalid User or Customer");

            var prod = productService.FindProductById(pid);
            if (!productBL.IsUsersProduct(prod))
            {
                return BadRequest(IndicatingMessages.InvalidAccess);
            }

            var originalTask = taskDataService.FindTaskDataByProductId(pid);

            if (originalTask == null)
            {
                //return BadRequest(IndicatingMessages.TaskNotFound);
                return Ok(new RFQBidSummary());
            }

            RFQBidSummary model = new RFQBidSummary
            {
                TaskId = originalTask.TaskId,
                State = (States)originalTask.StateId,
            };
            var lowestPriceBreak = priceBreakService.FindPriceBreakByProductId(pid).Where(x => x.UnitPrice > 0).OrderBy(x => x.UnitPrice).FirstOrDefault();
            PartDetails partDetails = new PartDetails
            {
                PartName = prod.Name,
                BuildType = prod.BuildType,
                MaterialType = prod.Material,
                SampleLeadTime = prod.SampleLeadTime,
                ProductionLeadTime = prod.ProductionLeadTime,
                PartNumber = prod.PartNumber,
                RevisionNumber = prod.PartRevision.Name,

                //NumberSampleIncluded = lowestPriceBreak?.NumberSampleIncluded,
                //UnitPrice = lowestPriceBreak?.UnitPrice,
                //Quantity = lowestPriceBreak?.Quantity,
            };

            var tds = taskDataService.FindTaskDataListByProductId(pid).Where(x => x.RFQBidId != null);
            List<BidDetails> bidDetailsList = new List<BidDetails>();
            foreach (var td in tds)
            {
                var product = td.Product;
                var priceBreaks = priceBreakService.FindPriceBreaksByTaskId(td.TaskId);
                var pbDtoList = await priceBreaks.ProjectTo<PriceBreakDTO>().ToListAsync();
                PriceBreak pb = null;
                UserPerformance vendorPerformance = new UserPerformance();
                UserPerformanceViewModel vp = homeBL.GetUserPerformance(td.RFQBid.VendorId, CompanyType.Vendor);
                if (vp != null)
                {
                    vendorPerformance = Mapper.Map<UserPerformance>(vp);
                }
                if (priceBreaks.Any())
                {
                    pb = priceBreaks.OrderByDescending(x => x.ShippingUnitPrice).ToList().Last();
                }
                vendorPerformance.UserName = td.RFQBid.VendorCompany.Name;
                vendorPerformance.UserLocation = Mapper.Map<AddressDTO>(td.RFQBid.VendorCompany.Address);

                var bidDetails = new BidDetails
                {
                    VendorId = td.RFQBid.VendorId,
                    VendorStatus = (States)td.StateId,
                    VendorName = td.RFQBid.VendorCompany.Name,
                    //QuoteDocUri = docs,
                    //PriceBreaks = pbDtoList,
                    HarmonizedCode = td.RFQBid?.HarmonizedCode,
                    SampleTime = td.RFQBid.SampleLeadTime,
                    ProductionLeadTime = td.RFQBid.ProductLeadTime,
                    EstShippingTime = pb?.ShippingDays,
                    UnitShipping = pb?.ShippingUnitPrice,
                    AvrShipping = priceBreaks?.Average(x => x.ShippingUnitPrice),
                    ToolingCharge = pb?.ToolingSetupCharges,
                    QuoteDocUri = documentService.FindDocumentByTaskIdDocType(td.TaskId, DOCUMENT_TYPE.QUOTE_PDF).Select(d => d.DocUri).ToList(),
                    Quantities = new Quantities
                    {
                        Qty1 = product.RFQQuantity.Qty1 ?? null,
                        Qty2 = product.RFQQuantity.Qty2 ?? null,
                        Qty3 = product.RFQQuantity.Qty3 ?? null,
                        Qty4 = product.RFQQuantity.Qty4 ?? null,
                        Qty5 = product.RFQQuantity.Qty5 ?? null,
                        Qty6 = product.RFQQuantity.Qty6 ?? null,
                        Qty7 = product.RFQQuantity.Qty7 ?? null,
                    },
                    VendorPerformance = vendorPerformance,
                    NumberSampleIncluded = pb?.NumberSampleIncluded
                };

                foreach (var p in pbDtoList)
                {
                    bidDetails.UnitPrices.Add(p.UnitPrice.Value);
                }
                bidDetailsList.Add(bidDetails);

            }
            model.PartDetails = partDetails;
            model.BidDetails = bidDetailsList;

            return Ok(model);
        }

        /// <summary>
        /// Vendor Revising RFQ.
        /// </summary>
        /// <param name="pid">Product/RFQ Id</param>
        [HttpPost]
        [Route("rfqs/vendor/{userId}/RevisingRFQ/{pid:int}", Name = "RevisingRFQ")]
        public async Task<IHttpActionResult> RevisingRFQ(int pid)
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            if (userContext.User == null || userContext.Company == null)
                return BadRequest("Invalid User or Customer");

            var vendorId = userContext.Company.Id;

            var files = GetPostedFiles(homeBL);
            try
            {
                var formDataDic = HttpContext.Current.Request.Form.ToDictionary();
                if (!formDataDic.ContainsKey("revisionReason"))
                    return BadRequest(IndicatingMessages.ForgotRevisionReason);

                string revisionReason = formDataDic["revisionReason"];
                TaskData td = null;
                var rfqBids = rfqBidService.FindRFQBidByVendorIdProductId(vendorId, pid);
                if (rfqBids != null)
                {

                    td = taskDataService.FindTaskDataListByProductId(pid)
                        .Where(x => (x.StateId == (int)States.BidForRFQ ||
                                     x.StateId == (int)States.RFQRevision) &&
                                     x.RFQBidId == rfqBids.Id)
                        .LastOrDefault();
                }
                else
                {
                    td = taskDataService.FindTaskDataByProductId(pid);
                }

                if (td == null)
                {
                    return BadRequest(IndicatingMessages.TaskNotFound);
                }

                td.RevisingReason += revisionReason;
                StateTransitionViewModel model = new StateTransitionViewModel
                {
                    group = Triggers.RevisionRequired.ToString(),
                    TaskData = td,
                    UserType = USER_TYPE.Vendor,
                };
                taskDataService.Update(td);

                var result = await homeBL.TaskStateHandler(model, files);
                if (result != null)
                {
                    return BadRequest(result);
                }

                return CreatedAtRoute("RevisingRFQ", new { id = td.StateId }, td.RevisingReason);
            }
            catch (Exception ex)
            {
                return BadRequest("TaskStateHandler: " + ex.RetrieveErrorMessage());
            }
            finally
            {
                DeleteUploadFiles(files);
            }
        }

        /// <summary>
        /// Vendor Request RFQ Revision
        /// </summary>
        /// <param name="pid">Product/RFQ Id</param>
        [HttpPost]
        [Route("rfqs/vendor/{userId}/VendorRequestRFQRevision/{pid:int}", Name = "VendorRequestRFQRevision")]
        public async Task<IHttpActionResult> VendorRequestRFQRevision(int pid)
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            if (userContext.User == null || userContext.Company == null)
                return BadRequest(IndicatingMessages.InvalidAccess);

            var vendorId = userContext.Company.Id;

            var files = GetPostedFiles(homeBL);
            try
            {
                var formDataDic = HttpContext.Current.Request.Form.ToDictionary();
                if (!formDataDic.ContainsKey("revisionReason") || !formDataDic.ContainsKey("revisionDescription"))
                    return BadRequest(IndicatingMessages.ForgotRevisionReason);

                string revisionReason = formDataDic["revisionReason"];
                string revsionDescription = formDataDic["revisionDescription"];
                var td = taskDataService.FindTaskDataListByProductId(pid)
                        .Where(x => (x.RFQBid != null && x.RFQBid.VendorId == vendorId &&
                                     (x.StateId == (int)States.ReviewRFQ || x.StateId == (int)States.RFQReviewUpdateQuantity)))
                        .FirstOrDefault();

                if (td == null)
                {
                    return BadRequest(IndicatingMessages.TaskNotFound);
                }

                try
                {
                    productBL.SetRFQActionReasonTable(pid, vendorId, revisionReason, revsionDescription, REASON_TYPE.RFQ_REVISION_REQUEST);
                }
                catch
                {
                    throw;
                }
                StateTransitionViewModel model = new StateTransitionViewModel
                {
                    group = Triggers.RevisionRequired.ToString(),
                    TaskData = td,
                    UserType = USER_TYPE.Vendor,
                };
                taskDataService.Update(td);

                var result = await homeBL.TaskStateHandler(model, files);
                if (result != null)
                {
                    return BadRequest(result);
                }
                td = taskDataService.FindById(td.TaskId);
                return CreatedAtRoute("VendorRequestRFQRevision", new { id = td.StateId }, Enum.GetName(typeof(States), td.StateId));
            }
            catch (Exception ex)
            {
                return BadRequest("TaskStateHandler: " + ex.RetrieveErrorMessage());
            }
            finally
            {
                DeleteUploadFiles(files);
            }
        }


        /// <summary>
        /// Vendor Accepted RFQ
        /// </summary>
        /// <param name="pid">Product/RFQ Id</param>
        [HttpPost]
        [Route("rfqs/vendor/{userId}/VendorAcceptedRFQ/{pid:int}", Name = "VendorAcceptedRFQ")]
        public async Task<IHttpActionResult> VendorAcceptedRFQ(int pid)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest(IndicatingMessages.InvalidAccess);

            var vendorId = userContext.Company.Id;

            try
            {
                var td = taskDataService.FindTaskDataListByProductId(pid)
                        .Where(x => (x.RFQBid != null && x.RFQBid.VendorId == vendorId &&
                                     (x.StateId == (int)States.ReviewRFQ ||
                                     x.StateId == (int)States.RFQReviewUpdateQuantity)))
                        .FirstOrDefault();

                if (td == null)
                {
                    return BadRequest(IndicatingMessages.TaskNotFound);
                }

                StateTransitionViewModel model = new StateTransitionViewModel
                {
                    group = Triggers.VendorAcceptRFQ.ToString(),
                    TaskData = td,
                    UserType = USER_TYPE.Vendor,
                };

                var result = await homeBL.TaskStateHandler(model);
                if (result != null)
                {
                    return BadRequest(result);
                }
                td = taskDataService.FindById(td.TaskId);
                return CreatedAtRoute("VendorAcceptedRFQ", new { id = td.StateId }, Enum.GetName(typeof(States), td.StateId));
            }
            catch (Exception ex)
            {
                return BadRequest("TaskStateHandler: " + ex.RetrieveErrorMessage());
            }
        }


        /// <summary>
        /// Send a revision to vendors assigned to RFQ by customer
        /// </summary>
        /// <param name="pid">ProductID</param>
        /// <returns></returns>
        //https://nautilusdigital.atlassian.net/browse/OM-149 //Create endpoint for customer to send a revision to vendors assigned to RFQ
        [HttpPost]
        [Route("rfqs/customer/{userId}/SendCustomerRevisionRFQ/{pid:int}", Name = "SendCustomerRevisionRFQ")]
        public IHttpActionResult SendCustomerRevisionRFQ(int pid)
        {
            // Read the form data and return an async task.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            if (userContext.User == null || userContext.Company == null)
                return BadRequest("Invalid User or Customer");

            TaskData td = taskDataService.FindTaskDataListByProductId(pid)
                .Where(x => x.StateId < (int)States.QuoteAccepted ||
                            x.StateId == (int)States.PendingRFQRevision ||
                            x.StateId == (int)States.BidForRFQ ||
                            x.StateId == (int)States.BidReview ||
                            x.StateId == (int)States.RFQBidUpdateQuantity ||
                            x.StateId == (int)States.RFQReviewUpdateQuantity ||
                            x.StateId == (int)States.ReviewRFQ)
                .FirstOrDefault();

            if (td == null)
            {
                return BadRequest(IndicatingMessages.TaskNotFound);
            }

            var files = GetPostedFiles(homeBL);
            var docs = new List<HttpPostedFileBase>();
            HttpPostedFileBase newAvatar = null;
            foreach (var file in files)
            {
                if (file.ContentType.Contains("image/") == true)
                {
                    newAvatar = file;
                    continue;
                }
                docs.Add(file);
            }
            try
            {
                using (var trans = AsyncTransactionScope.StartNew(transOptions))
                {
                    var formDataDic = HttpContext.Current.Request.Form.ToDictionary();
                    if (!formDataDic.ContainsKey("responseRevisionRequest"))
                        return BadRequest(IndicatingMessages.ForgotRevisionReason);

                    if (!formDataDic.ContainsKey("newPartNumberRevision"))
                        return BadRequest(IndicatingMessages.MissingFormData);

                    string responseRevisionRequest = formDataDic["responseRevisionRequest"];
                    string newRevisionNumber = formDataDic["newPartNumberRevision"];
                    var bidRequestRevision = bidRequestRevisionService.FindBidRequestRevisionListByProductCustomerIdTaskId(pid, td.TaskId).LastOrDefault();
                    CreatePartRevisionViewModel model = new CreatePartRevisionViewModel
                    {
                        ProductId = pid,
                        TaskId = td.TaskId,
                        PartRevision = newRevisionNumber,
                        PartRevisionDesc = responseRevisionRequest,
                        NewAvatar = newAvatar,
                        VendorId = td.Product?.VendorId,
                        BidRequestRevisionId = bidRequestRevision?.Id,
                    };
                    var result = productBL.CreateRFQRevision(model);
                    string error = documentBL.UploadProductDocs(docs, result.NewProductId, result.NewTaskId, result.PartRevisionId);

                    if (!string.IsNullOrEmpty(error))
                    {
                        return BadRequest("UploadProductDocs: " + error);
                    }
                    var prod = productService.FindProductById(result.NewProductId);
                    var dto = Mapper.Map<ProductDTO>(prod);
                    dto.TaskId = result.NewTaskId;
                    dto.State = result.State;

                    trans.Complete();
                    return CreatedAtRoute("SendCustomerRevisionRFQ", new { id = result.NewProductId }, dto);
                }
            }
            catch (Exception ex)
            {
                return BadRequest("TaskStateHandler: " + ex.RetrieveErrorMessage());
            }
            finally
            {
                DeleteUploadFiles(files);
            }
        }


        /// <summary>
        /// Vendor Bid for RFQ.
        /// </summary>
        /// <param name="pid">Product/RFQ Id</param>
        [HttpGet]
        [Route("rfqs/vendor/{userId}/GetRevisions/{pid:int}", Name = "GetRevisions")]
        public IHttpActionResult GetRevisions(int pid)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest("Invalid User or Customer");

            List<Document> doc = new List<Document>();
            List<BidRevisingRequest> list = new List<BidRevisingRequest>();
            var td = taskDataService.FindTaskDataByProductId(pid);
            if (td != null)
            {
                var rrs = bidRequestRevisionService.FindBidRequestRevisionListByProductCustomerIdTaskId(pid, td.TaskId);
                var rr = rrs.LastOrDefault();
                if (rr != null)
                {
                    BidRevisingRequest brr = new BidRevisingRequest
                    {
                        Id = rr.Id,
                        VendorId = rr.VendorId,
                        VendorName = companyService.FindCompanyById(rr.VendorId).Name,
                        Description = td.RevisingReason,
                        CreateDatetime = rr.CreateDateTime,
                        RevisionNumber = rr.RevisionNumber.Value,
                    };

                    list.Add(brr);
                    doc = documentService.FindDocumentByTaskIdDocType(rr.TaskId, DOCUMENT_TYPE.REVISING_DOCS);
                }
            }

            RFQRevisionDTO dto = new RFQRevisionDTO
            {
                Descriptions = list,
                Files = doc?.Select(x => x.DocUri.Split('%')[0]).ToArray(),
            };
            return Ok(dto);
        }

        /// <summary>
        /// Customer gets Revision .
        /// </summary>
        /// <param name="pid">Product/RFQ Id</param>
        [HttpGet]
        [Route("rfqs/customer/{userId}/GetRevisionRequests/{pid:int}", Name = "GetRevisionRequests")]
        public IHttpActionResult GetRevisionRequests(int pid)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest("Invalid User or Customer");

            var doc = documentService.FindDocumentByProductIdDocType(pid, DOCUMENT_TYPE.REVISING_DOCS);
            List<BidRevisingRequest> list = new List<BidRevisingRequest>();
            TaskData td = taskDataService.FindTaskDataByProductId(pid);
            if (td != null)
            {
                var rr = bidRequestRevisionService.FindBidRequestRevisionListByProductCustomerIdTaskId(pid, td.TaskId);
                foreach (var r in rr)
                {
                    var vendor = companyService.FindCompanyById(r.VendorId);
                    BidRevisingRequest brr = new BidRevisingRequest
                    {
                        Id = r.Id,
                        VendorId = r.VendorId,
                        VendorName = vendor?.Name,
                        Description = r.RFQActionReason?.Description,
                        CreateDatetime = r.CreateDateTime,
                        RevisionNumber = r.RevisionNumber.Value,
                    };
                    list.Add(brr);
                }
            }
            RFQRevisionDTO dto = new RFQRevisionDTO
            {
                Descriptions = list,
                Files = doc?.Select(x => x.DocUri.Split('%')[0]).ToArray(),
            };
            return Ok(dto);
        }

        /// <summary>
        /// Get documents related this product.
        /// </summary>
        /// <param name="pid">Product/RFQ Id</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Numbe of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [HttpGet]
        [Route("products/user/{userId}/GetAllFiles/{pid:int}", Name = "GetAllFiles")]
        public async Task<PagedResultSet<DocumentDTO>> GetAllFiles(int pid, int? page = 1, int pageSize = PageSize, string orderBy = nameof(ProductDTO.Id), bool ascending = false)
        {
            if (userContext.User == null || userContext.Company == null)
                return null;

            var docs = documentService.FindDocumentsByCompanyId(userContext.Company.Id).Where(d => d.ProductId == pid);
            return await PageOfResultsSetAlternativeMapperAsync<Document, DocumentDTO>(docs, page, pageSize, orderBy, ascending, "GetAllFiles");
        }

        /// <summary>
        /// Get vendor's documents related this product.
        /// </summary>
        /// <param name="pid">Product/RFQ Id</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Numbe of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [HttpGet]
        [Route("products/user/{userId}/GetVendorAllFiles/{pid:int}", Name = "GetVendorAllFiles")]
        public async Task<PagedResultSet<DocumentDTO>> GetVendorAllFiles(int pid, int? page = 1, int pageSize = PageSize, string orderBy = nameof(ProductDTO.Id), bool ascending = false)
        {
            if (userContext.User == null || userContext.Company == null)
                return null;

            var docs = documentService.FindDocumentsByVendorId(userContext.Company.Id)
                                      .Where(d => d.ProductId == pid);

            return await PageOfResultsSetAlternativeMapperAsync<Document, DocumentDTO>(docs, page, pageSize, orderBy, ascending, "GetVendorAllFiles");
        }

        /// <summary>
        /// Send a revision to vendors assigned to RFQ for a customer
        /// </summary>
        /// <param name="pid">Product ID</param>
        /// <returns></returns>
        [HttpPost]
        [Route("rfqs/customer/{userId}/CustomerReviewBid/{pid:int}", Name = "CustomerReviewBid")]
        public IHttpActionResult CustomerReviewBid(int pid)
        {
            if (userContext.User == null || userContext.Company == null)
                return null;

            var formDataDic = HttpContext.Current.Request.Form.ToDictionary();
            if (!formDataDic.ContainsKey("selected") || !formDataDic.ContainsKey("vendorId"))
                return BadRequest(IndicatingMessages.MissingFormData);

            bool selected = bool.Parse(formDataDic["selected"]);
            int vendorId = int.Parse(formDataDic["vendorId"]);
            int? reasonIndex = null;
            if (formDataDic.ContainsKey("reasonIndex"))
            {
                int temp;
                bool res = int.TryParse(formDataDic["reasonIndex"], out temp);
                if (res == true)
                    reasonIndex = temp;
            }

            Product product = productService.FindProductById(pid);
            if (product == null)
            {
                return BadRequest(IndicatingMessages.ProductNotFound);
            }
            try
            {

                if (selected == false && reasonIndex == null)
                {
                    return BadRequest("The value of reasonIndex must be provided");
                }
                if (reasonIndex != null && (reasonIndex >= BidFailedReason.Reasons.Length || reasonIndex < 1))
                {
                    return BadRequest($"reasonIndex value is out of range: {reasonIndex}, the value range: 1 - {BidFailedReason.Reasons.Length - 1}");
                }

                if (product.VendorId == null)
                {
                    if (IsUniqueProductOn4Factors(productService, product.CustomerId.Value, vendorId, product.PartNumber, product.PartNumberRevision) == false)
                        return BadRequest(IndicatingMessages.DuplicatedProduct);
                }

                var result = taskDataBL.ReviewBid(selected, vendorId, product.Id, reasonIndex);
                return CreatedAtRoute("CustomerReviewBid", new { id = pid }, "Success");
            }
            catch (Exception ex)
            {
                return BadRequest("ReviewBid: " + ex.RetrieveErrorMessage());
            }
        }


        /// <summary>
        /// Create a new revision for a product by customer
        /// </summary>
        /// <param name="pid">Product ID</param>
        /// <returns></returns>
        [HttpPost]
        [Route("products/user/{userId}/CreatePartRevision/{pid:int}", Name = "CreatePartRevision")]
        public IHttpActionResult CreatePartRevision(int pid)
        {
            if (userContext.User == null || userContext.Company == null)
                return null;

            Product product = productService.FindProductById(pid);
            if (product == null)
            {
                return BadRequest(IndicatingMessages.ProductNotFound);
            }
            if (product.CustomerId != userContext.Company.Id)
            {
                return BadRequest(IndicatingMessages.YouCannotCreateRevision);
            }
            if (!productBL.IsUsersProduct(product))
            {
                return BadRequest(IndicatingMessages.InvalidAccess);
            }
            var formDataDic = HttpContext.Current.Request.Form.ToDictionary();
            if (!formDataDic.ContainsKey("newRevisionNumber") || !formDataDic.ContainsKey("description"))
                return BadRequest(IndicatingMessages.MissingFormData);

            string newRevisionNumber = formDataDic["newRevisionNumber"];
            string description = formDataDic["description"];

            var files = GetPostedFiles(homeBL);
            var docs = new List<HttpPostedFileBase>();

            try
            {

                HttpPostedFileBase newAvatar = null;
                foreach (var file in files)
                {
                    if (file.ContentType.Contains("image/") == true)
                    {
                        newAvatar = file;
                        continue;
                    }
                    docs.Add(file);
                }
                CreatePartRevisionViewModel model = new CreatePartRevisionViewModel
                {
                    ProductId = pid,
                    PartRevision = newRevisionNumber,
                    PartRevisionDesc = description,
                    NewAvatar = newAvatar,
                    VendorId = product.VendorId != null ? product.VendorId.Value : product.VendorId,
                };

                var result = productBL.CreatePartRevision(model);
                string error = documentBL.UploadProductDocs(docs, result.NewProductId, result.NewTaskId);
                if (!string.IsNullOrEmpty(error))
                {
                    return BadRequest("UploadProductDocs: " + error);
                }
                var newProduct = productService.FindProductById(result.NewProductId);
                var newProductDTO = Mapper.Map<ProductDTO>(newProduct);
                newProductDTO.TaskId = result.NewTaskId;
                newProductDTO.State = result.State;

                return CreatedAtRoute("CreatePartRevision", new { id = result.NewProductId }, newProductDTO);
            }
            catch (Exception ex)
            {
                return BadRequest("CreatePartRevision: " + ex.RetrieveErrorMessage());
            }
            finally
            {
                DeleteUploadFiles(files);
                DeleteUploadFiles(docs);
            }
        }

        /// <summary>
        /// Get part revison list by product ID from which these revisions created
        /// </summary>
        /// <param name="pid">Original Product ID</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Numbe of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        /// <returns></returns>
        [HttpGet]
        [Route("products/user/{userId}/GetPartRevisionList/{pid:int}", Name = "GetPartRevisionList")]
        public async Task<PagedResultSet<PartRevisionDTO>> GetPartRevisionList(int pid, int? page = 1, int pageSize = PageSize, string orderBy = nameof(ProductDTO.Id), bool ascending = false)
        {
            if (userContext.User == null || userContext.Company == null)
                return null;

            var product = productService.FindProductById(pid);
            var partRevision = partRevisionService
                .FindPartRevisionById(product.PartRevisionId.Value);

            var partRevisions = partRevisionService
                .FindPartRevisionsByProductId(partRevision.OriginProductId)
                .GroupBy(x => new { x.Name, x.TaskId })
                .Select(x => x.FirstOrDefault());


            if (partRevisions == null || !partRevisions.Any())
                return null;

            var dtos = await PageOfResultsSetAsync<PartRevision, PartRevisionDTO>(partRevisions, page, pageSize, orderBy, ascending, "GetPartRevisionList");

            return dtos;
        }

        /// <summary>
        /// Cancel a RFQ bid by vendor
        /// </summary>
        /// <param name="pid">Product ID</param>
        /// <returns></returns>
        [HttpPost]
        [Route("rfqs/user/{userId}/CancelRFQ/{pid:int}", Name = "CancelRFQ")]
        public async Task<IHttpActionResult> CancelRFQ(int pid)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest("Invalid User or Customer");

            var td = taskDataService.FindTaskDataListByProductId(pid)
                .Where(x => x.RFQBidId != null && x.RFQBid.VendorId == userContext.Company.Id || x.RFQBidId == null)
                .FirstOrDefault();
            if (td == null)
            {
                return BadRequest(IndicatingMessages.TaskNotFound);
            }
            var error = await taskDataBL.CancelRFQ(td.TaskId);
            if (error != null)
            {
                return BadRequest(error);
            }
            return CreatedAtRoute("CancelRFQ", new { id = pid }, "Success");
        }

        /// <summary>
        /// Customer cancels a RFQ during Bid process
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <returns></returns>
        [HttpPost]
        [Route("rfqs/user/{userId}/CancelRFQByCustomer/{productId:int}", Name = "CancelRFQByCustomer")]
        public async Task<IHttpActionResult> CancelRFQByCustomer(int productId)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest(IndicatingMessages.CustomerNotFound);

            var formDataDic = HttpContext.Current.Request.Form.ToDictionary();
            if (!(formDataDic.ContainsKey("reason") || formDataDic.ContainsKey("description")))
                return BadRequest(IndicatingMessages.ForgotRejectReason);

            string cancelReason = formDataDic["reason"];
            string cancelDescription = formDataDic["description"];

            var tds = taskDataService.FindTaskDataListByProductId(productId);
            var product = productService.FindProductById(productId);
            int? rfqActionReasonId = null;
            int customerId = userContext.Company.Id;

            if (tds == null || tds.Count == 0)
            {
                return BadRequest(IndicatingMessages.TaskNotFound);
            }

            using (var trans = AsyncTransactionScope.StartNew())
            {
                try
                {
                    rfqActionReasonId = productBL.SetRFQActionReasonTable(productId, customerId, cancelReason, cancelDescription, REASON_TYPE.CUSTOMER_CANCEL_RFQ);
                }
                catch
                {
                    throw;
                }
                States newState = States.CustomerCancelledRFQ;
                foreach (var td in tds)
                {
                    td.StateId = (int)newState;
                    td.UpdatedBy = userContext.User.UserName;
                    td.ModifiedByUserId = userContext.UserId;
                    taskDataService.Update(td);

                    // Notify vendors of canceling of the RFQ
                    if (td.RFQBidId != null)
                    {
                        var vendor = td.RFQBid.VendorCompany;
                        var userContacts = userContactService.GetAllActiveUserConnectFromCompany(vendor);
                        foreach (var contact in userContacts)
                        {
                            var destination = contact.Email;
                            var destinationSms = contact.PhoneNumber;
                            await homeBL.SendNotifications(td, destination, destinationSms);
                        }
                    }
                }
                // Update state in [BidRFQStatus] table
                var bidRFQStatus = bidRFQStatusService.FindBidRFQStatusListByProductId(productId).LastOrDefault();
                var customerTask = tds.Where(x => x.RFQBidId == null).FirstOrDefault();
                if (bidRFQStatus == null)
                {
                    bidRFQStatus = new BidRFQStatus
                    {
                        ProductId = productId,
                        CustomerId = customerId,
                        StateId = (int)newState,
                        TaskId = customerTask != null ? customerTask.TaskId : 0,
                        TotalVendors = tds.Count - 1,
                        RevisionCycle = 0,
                        CreatedByUserId = userContext.UserId,
                        RFQActionReasonId = rfqActionReasonId,
                    };

                    var bidRFQStatusId = bidRFQStatusService.AddBidRFQStatus(bidRFQStatus);
                }
                else
                {
                    bidRFQStatus.StateId = (int)newState;
                    bidRFQStatus.ModifiedByUserId = userContext.UserId;
                    bidRFQStatus.RFQActionReasonId = rfqActionReasonId;
                    bidRFQStatusService.UpdateBidRFQStatus(bidRFQStatus);
                }
                trans.Complete();
                return Ok("Success");
            }
        }


        /// <summary>
        /// Get Product state history
        /// </summary>
        /// <param name="pid">Product ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("products/user/{userId}/GetPartStateTracking/{pid:int}", Name = "GetPartStateTracking")]
        public IHttpActionResult GetPartStateTracking(int pid)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest("Invalid User or Customer");

            var companyId = userContext.Company.Id;
            var product = productService.FindProductById(pid);
            if (product == null)
            {
                return BadRequest(IndicatingMessages.ProductNotFound);
            }
            if (!productBL.IsUsersProduct(product))
            {
                return BadRequest(IndicatingMessages.InvalidAccess);
            }
            var trackings = productStateTrackingService.FindProductStateTrackingListByProductId(pid);
            ProductStateTrackingDTO dto = new ProductStateTrackingDTO
            {
                ProductId = pid,
                StateDateTime = trackings.Select(x => new StateDatetimePair { State = (States)x.StateId, ModifiedUtc = x.ModifiedUtc }).ToList(),
            };

            return CreatedAtRoute("GetPartStateTracking", new { id = pid }, dto);
        }

        /////////////// Product Sharing  /////////////////////

        /// <summary>
        /// Shares a product with a company
        /// </summary>
        /// <param name="productId">Product ID to be shared by the company</param>
        /// <param name="vm">Type of CreateProductShareViewModel</param>
        /// <returns></returns>
        [Route("products/{productId:int}/shares", Name = "CreateProductShare")]
        [HttpPost]
        public IHttpActionResult CreateProductShare(int productId, [FromBody] CreateProductShareViewModel vm)
        {
            // view model validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // entities validation
            var product = productService.FindProductById(productId);
            if (product == null)
            {
                return BadRequest(IndicatingMessages.ProductNotFound);
            }

            if (product.CustomerId != vm.SharerCompanyId)
            {
                return BadRequest(IndicatingMessages.CompanyNotAuthorized);
            }

            // set ignoreIsRevoked to true, so that we still get entity even it is revoked
            var productShare = productSharingService.FindProductSharingByCompanyIdProductId(vm.ShareeCompanyId, productId, true);
            var currentUtc = DateTime.UtcNow;
            if (productShare != null)
            {
                if (productShare.IsRevoked != true) return BadRequest(IndicatingMessages.ProductAlreadyShared);

                using (var trans = AsyncTransactionScope.StartNew())
                {
                    productShare.TaskData.ModifiedUtc = currentUtc;
                    taskDataService.Update(productShare.TaskData);
                    productShare.IsRevoked = false;
                    productShare.HasPermissionToOrder = true;
                    productShare.ModifiedByUserId = vm.CreatedByUserId;
                    productSharingService.UpdateProductSharing(productShare);
                    var dto = Mapper.Map<ProductShareDto>(productShare);
                    trans.Complete();

                    return CreatedAtRoute("CreateProductShare", new { id = productId }, dto);
                }
            }

            using (var transaction = AsyncTransactionScope.StartNew())
            {
                var task = taskDataService.FindTaskDataListByProductId(productId).PartCanBeShared();
                if (task == null)
                {
                    return BadRequest(IndicatingMessages.ThisProductIsNotInCorrectStateToAllowSharing);
                }
                task.ModifiedUtc = currentUtc;
                taskDataService.Update(task);
                var model = new ProductSharing
                {
                    ProductId = productId,
                    OwnerCompanyId = vm.SharerCompanyId,
                    SharingCompanyId = vm.ShareeCompanyId,
                    TaskId = task.TaskId,
                    HasPermissionToOrder = true,
                    CreatedUtc = currentUtc,
                    ModifiedUtc = currentUtc,
                    CreatedByUserId = vm.CreatedByUserId,
                };
                var productShareId = productSharingService.AddProductSharing(model);
                productShare = productSharingService.Find(productShareId);
                var dto = Mapper.Map<ProductShareDto>(productShare);

                // Create Credit Relations between sharer and sharee
                var vendorId = product.VendorId;
                if (vendorId == null)
                    throw new Exception($"Couldn't find customer for this product{productId}");
                var companiesCreditRelationship = companiesCreditRelationshipService.FindCompaniesCreditRelationshipByCustomerIdVendorId(vm.ShareeCompanyId, vendorId.Value);
                var shareeCompany = companyService.FindCompanyById(vm.ShareeCompanyId);

                if (companiesCreditRelationship == null)
                {
                    var newCreditRelation = new CompaniesCreditRelationship
                    {
                        CustomerId = vm.ShareeCompanyId,
                        VendorId = vendorId.Value,
                        isTerm = false,
                        Currency = shareeCompany?.Currency ?? CurrencyCodes.USD,
                    };
                    companiesCreditRelationshipService.AddCompaniesCreditRelationship(newCreditRelation);
                }

#if false
                // Notify sharee
                ProductsharingViewModel psVM = new ProductsharingViewModel
                {
                    ShareeCompanyName = productShare.ProductSharingCompany.Name,
                    SharerCompanyName = productShare.ProductOwnerCompany.Name,
                    PartNumber = productShare.Product.PartNumber,
                    ProductId = productShare.ProductId,
                    PartDescription = productShare.Product.Description,
                };
                var userContacts = userContactService.GetAllActiveUserConnectFromCompany(productShare.SharingCompanyId);
                var vendors = userContactService.GetAllActiveUserConnectFromCompany(productShare.Product.VendorId.Value);
                userContacts = userContacts.Union(vendors);
                var subject = "This is notified that you are allowing to share a product";
                foreach (var user in userContacts)
                {
                    notificationBL.NotifyCreateProductSharing(subject, user.Email, null, psVM);
                }
#endif

                transaction.Complete();

                return CreatedAtRoute("CreateProductShare", new { id = productId }, dto);
            }
        }

        /// <summary>
        /// List product shares
        /// </summary>
        /// <param name="productId">product ID.</param>
        /// <param name="sharerCompanyId">Sharer company ID.</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Numbe of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [Route("products/{productId:int}/shares", Name = "ListProductShares")]
        [HttpGet]
        public async Task<PagedResultSet<ProductShareDto>> ListProductShares(int productId, int? sharerCompanyId = null,
                                                                             int? page = 1, int pageSize = PageSize,
                                                                             string orderBy = nameof(ProductSharing.CreatedUtc),
                                                                             bool ascending = false)
        {
            // TODO: return NotFound if product is not found
            var productShares = productSharingService.QueryProductShares(productId);
            if (sharerCompanyId != null)
            {
                productShares = productShares.Where(x => x.OwnerCompanyId == sharerCompanyId);
            }

            return await PageOfResultsSetAsync<ProductSharing, ProductShareDto>(productShares, page, pageSize, orderBy, ascending, "ListProductShares");
        }

        /// <summary>
        /// Revoke a part sharing relationship between sharee company and a product
        /// </summary>
        /// <param name="productId">shared part ID.</param>
        /// <param name="shareId">Product sharing ID.</param>
        /// <param name="userId">User ID.</param>
        [Route("products/{productId:int}/shares/{shareId:int}", Name = "RemoveProductShare")]
        [HttpDelete]
        public IHttpActionResult RemoveProductShare(int productId, int shareId, string userId = null)
        {
            // view model validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // entities validation
            var product = productService.FindProductById(productId);
            if (product == null)
            {
                return BadRequest(IndicatingMessages.ProductNotFound);
            }

            var productShare = productSharingService.Find(shareId);
            if (productShare == null)
            {
                return BadRequest(IndicatingMessages.NotFound);
            }

            using (var trans = AsyncTransactionScope.StartNew())
            {
                try
                {
                    productShare.IsRevoked = true;
                    productShare.ModifiedByUserId = userId;
                    productShare.ModifiedUtc = DateTime.UtcNow;

                    productSharingService.UpdateProductSharing(productShare);
                    trans.Complete();

                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.RetrieveErrorMessage());
                }
            }
        }

        /// <summary>
        /// List sharing companies who can make order of the shared product
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Numbe of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        /// <returns></returns>
        [HttpGet]
        [Route("products/{productId:int}/customers", Name = "ListCompaniesCanOrder")]
        public async Task<PagedResultSet<ProductCustomerDto>> ListCompaniesCanOrder(int productId,
                                                                                   int? page = 1,
                                                                                   int pageSize = PageSize,
                                                                                   string orderBy = nameof(ProductSharing.CreatedUtc),
                                                                                   bool ascending = false)
        {
            var shares = productSharingService.FindSharingsByProductId(productId);
            var result = await PageOfResultsSetAsync<ProductSharing, ProductCustomerDto>(shares, page, pageSize, orderBy, ascending, "ListCompaniesCanOrder");

            // Find sharer and concat its info to the result list
            var ps = shares.FirstOrDefault();
            if (ps == null)
                return result;

            var psDto = new ProductCustomerDto
            {
                Id = ps.OwnerCompanyId,
                Name = ps.ProductOwnerCompany.Name,
                Email = ps.ProductOwnerCompany.Shipping?.EmailAddress,
                Logo = ps.ProductOwnerCompany.CompanyLogoUri,
                BecameCustomerAt = ps.CreatedUtc.Value,
            };
            result.Results = result.Results.Concat<ProductCustomerDto>(new List<ProductCustomerDto>() { psDto });
            return result;
        }


        /// <summary>
        /// Add product draw documents or add/update a product avatar to an exisint product which was uploaded by onboarding process
        /// </summary>
        [ResponseType(typeof(ProductDTO))]
        [HttpPost]
        [Route("products/user/{userId}/AddProductFiles", Name = "AddProductFiles")]
        public IHttpActionResult AddProductFiles(int productId)
        {
            var product = productService.FindProductById(productId);
            if (!productBL.IsUsersProduct(product))
            {
                return BadRequest(IndicatingMessages.InvalidAccess);
            }

            var files = GetPostedFiles(homeBL);
            List<HttpPostedFileBase> postedFiles = new List<HttpPostedFileBase>();
            using (var trans = AsyncTransactionScope.StartNew())
            {
                try
                {
                    foreach (var file in files)
                    {
                        if (file.ContentType.Contains("image/") == true)
                        {
                            var fileName = FileUtil.MakeValidFileName($"Avatar-Customer-{product.CustomerId}-{Guid.NewGuid()}{Path.GetExtension(file.FileName)}");
                            product.AvatarUri = imageStorageService.Upload(file, fileName);
                            productService.UpdateProduct(product);
                        }
                        else
                        {
                            postedFiles.Add(file);
                        }
                    }

                    if (postedFiles.Count > 0)
                    {
                        var td = taskDataService.FindTaskDataByProductId(productId);
                        string result = documentBL.UploadProductDocs(postedFiles, productId, td.TaskId);
                        if (result != null)
                        {
                            return BadRequest(result);
                        }
                    }
                    trans.Complete();
                }
                catch (Exception ex)
                {
                    throw new Exception("AddProductFiles: " + ex.RetrieveErrorMessage());
                }
                finally
                {
                    DeleteUploadFiles(files);
                }
            }
            return Ok("Success");
        }

        /// <summary>
        /// Add an existing product to my products
        /// </summary>
        [ResponseType(typeof(ProductDTO))]
        [HttpPost]
        [Route("products/user/{userId}/AddExistingProduct", Name = "AddExistingProduct")]
        public IHttpActionResult AddExistingProduct()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var httpRequest = HttpContext.Current.Request;
            var formDataDic = httpRequest.Form.ToDictionaryOfObjects();
            var model = Slapper.AutoMapper.Map<AddExistingProductViewModel>(formDataDic, false);
            if (companyService.FindCompanyById(model.CustomerId) == null || companyService.FindCompanyById(model.VendorId) == null)
                return BadRequest(IndicatingMessages.CompanyNotFound);

            if (IsUniqueProductOn4Factors(productService, model.CustomerId, model.VendorId, model.PartNumber, model.PartRevision) == false)
                return BadRequest(IndicatingMessages.DuplicatedProduct);

            using (var trans = AsyncTransactionScope.StartNew(transOptions))
            {
                try
                {
                    var product = onBoardingBL.AddExistingProduct(model);
                    var dto = Mapper.Map<ProductDTO>(product);
                    var td = taskDataService.FindTaskDataByProductId(product.Id);
                    dto.TaskId = td.TaskId;
                    dto.State = (States)td.StateId;

                    trans.Complete();
                    return Ok(dto);
                }
                catch
                {
                    throw;
                }               
            }
        }

        /// <summary>
        /// Check if vendors available to match the selected capabilities when creating a new RFQ
        /// </summary>
        /// <param name="buildType">RFQ Build type</param>
        /// <param name="materialType">RFQ Material type</param>
        /// <param name="process"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("products/CheckVendorCapability", Name = "CheckVendorCapability")]
        public bool CheckVendorCapability(BUILD_TYPE buildType, int materialType, int? process = null)
        {
            List<ApprovedCapability> capaList = new List<ApprovedCapability>();
            if (process != null)
            {
                capaList = approvedCapabilityService.FindApprovedCapabilitiesByParams(buildType, (MATERIALS_TYPE)materialType, process.Value);
            }
            else
            {
                if (buildType != BUILD_TYPE.Process)
                {
                    capaList = approvedCapabilityService.FindApprovedCapabilitiesByParams(buildType, (MATERIALS_TYPE)materialType);

                }
                else
                {
                    capaList = approvedCapabilityService.FindApprovedCapabilitiesByParams(buildType, (Process_Type)materialType);
                }
            }
            return capaList.Count > 0;
        }

        /// <summary>
        /// Reset a RFQ
        /// </summary>
        /// <param name="productId">Product Id</param>
        [ResponseType(typeof(ProductDTO))]
        [HttpPost]
        [Route("rfqs/customer/{userId}/ResetRFQ/{productId:int}", Name = "ResetRFQ")]
        public IHttpActionResult ResetRFQ(int productId)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest(IndicatingMessages.CustomerNotFound);

            var td = taskDataService.FindOriginalTaskDataByProductId(productId);
            if (td == null)
                return BadRequest(IndicatingMessages.TaskNotFound);

            var error = taskDataBL.ResetRFQ(productId, td, userContext.User.UserName, userContext.UserId);
            if (error != null)
                return BadRequest(error);

            return Ok("Success");
        }

        /// <summary>
        /// Vendor rejects a RFQ (new product) 
        /// </summary>
        /// <param name="productId">Product Id</param>
        [ResponseType(typeof(ProductDTO))]
        [HttpPost]
        [Route("rfqs/vendor/{userId}/VendorRejectRFQ/{productId:int}", Name = "VendorRejectRFQ")]
        public async Task<IHttpActionResult> VendorRejectRFQ(int productId)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest(IndicatingMessages.CustomerNotFound);

            var formDataDic = HttpContext.Current.Request.Form.ToDictionary();
            if (!(formDataDic.ContainsKey("reason") || formDataDic.ContainsKey("description")))
                return BadRequest(IndicatingMessages.ForgotRejectReason);

            string rejectReason = formDataDic["reason"];
            string rejectDescription = formDataDic["description"];

            using (var trans = AsyncTransactionScope.StartNew())
            {
                var vendorId = userContext.Company.Id;
                var td = taskDataService.FindTaskDataListByProductId(productId)
                                        .Where(x => x.RFQBidId != null && x.RFQBid.VendorId == vendorId)
                                        .FirstOrDefault();

                if (td == null)
                {
                    return BadRequest(IndicatingMessages.TaskNotFound);
                }
                if (!(td.StateId == (int)States.ReviewRFQ ||
                      td.StateId == (int)States.BidForRFQ ||
                      td.StateId == (int)States.RFQReviewUpdateQuantity ||
                      td.StateId == (int)States.RFQBidUpdateQuantity))
                    return BadRequest(IndicatingMessages.TaskStateMismatch);

                try
                {
                    productBL.SetRFQActionReasonTable(productId, vendorId, rejectReason, rejectDescription, REASON_TYPE.REJECT_RFQ);
                }
                catch
                {
                    throw;
                }

                StateTransitionViewModel model = new StateTransitionViewModel
                {
                    group = Triggers.VendorRejectRFQ.ToString(),
                    TaskData = td,
                    UserType = USER_TYPE.Vendor,
                };

                var result = await homeBL.TaskStateHandler(model);
                if (result != null)
                {
                    return BadRequest(result);
                }
                trans.Complete();
                return Ok(td.RejectReason);
            }
        }

        /// <summary>
        /// Get remaining BID time. (In Days)
        /// </summary>
        /// <param name="productId">Product Id</param>      
        [HttpGet]
        [Route("rfqs/GetRemainingBidTime/{productId:int}", Name = "GetRemainingBidTime")]
        public int? GetRemainingBidTime(int productId)
        {
            var timer = timerSetupService.FindTimerSetupByProductIdTimerType(productId, TypeOfTimers.BidTimer);
            if (timer?.TimerStartAt == null || timer.Interval == null)
                return null;

            //TODO, Fix when the Unit is not Days.

            var toBeExpired = timer.TimerStartAt.Value.AddDays(int.Parse(timer.Interval));
            var interval = (toBeExpired - DateTime.UtcNow).Days;
            return interval;
        }


        /// <summary>
        /// Get Review RFQ Status by RFQ ID
        /// </summary>
        /// <param name="productId">Product Id</param>      
        /// <param name="type">Product Id</param>   
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [HttpGet]
        [Route("rfqs/user/{userId}/GetReviewRFQStatus/{productId:int}", Name = "GetReviewRFQStatus")]
        public async Task<PagedResultSet<ReviewRFQStatusDTO>> GetReviewRFQStatus(int productId, UserType type,
                                                                                int? page = 1, int pageSize = PageSize,
                                                                                string orderBy = nameof(ReviewRFQStatusDTO.TimeStamp),
                                                                                bool ascending = true)
        {
            var newProductDocs = documentService.FindDocumentByProductIdDocType(productId, DOCUMENT_TYPE.PRODUCT_2D_PDF);
            var newProduct3DDocs = documentService.FindDocumentByProductIdDocType(productId, DOCUMENT_TYPE.PRODUCT_3D_STEP);
            newProductDocs.AddRange(newProduct3DDocs);
            var revisionDocs = documentService.FindDocumentByProductIdDocType(productId, DOCUMENT_TYPE.REVISING_DOCS);
            List<ReviewRFQStatusDTO> dtoList = new List<ReviewRFQStatusDTO>();

            var vendorBidRFQStatuses = vendorBidRFQStatusService.FindVendorBidRFQStatusByProductId(productId);
            var bidRFQStatuses = bidRFQStatusService.FindBidRFQStatusByProductId(productId);
            if (type == UserType.Vendor)
            {
                var vendorId = userContext.Company.Id;
                vendorBidRFQStatuses = vendorBidRFQStatusService.FindVendorBidRFQStatusByProductIdVendorId(productId, vendorId);
            }
            var vendorBidRFQStatuseDTOList = Mapper.Map<List<VendorBidRFQStatusDTO>>(vendorBidRFQStatuses.ToList());
            foreach (var status in bidRFQStatuses)
            {
                var vendorRFQStatus = vendorBidRFQStatuseDTOList.Where(x => x.BidRFQStatusId == status.Id).ToList();
                foreach (var vst in vendorRFQStatus)
                {
                    if (vst.BidRequestRevisionId == null) continue;
                    vst.BidRequestRevision = Mapper.Map<BidRequestRevisionDTO>(bidRequestRevisionService
                        .FindBidRequestRevisionById(vst.BidRequestRevisionId.Value));

                    vst.BidRequestRevision.RevisionDocsUri = revisionDocs
                        .Where(x => x.BidRequestRevisionId == vst.BidRequestRevisionId)
                        .Select(d => d.DocUri).ToList();
                }

                ReviewRFQStatusDTO dto = new ReviewRFQStatusDTO
                {
                    RevisionCycle = status.RevisionCycle ?? 0,
                    CusotmerId = status.CustomerId,
                    State = status.StateId,
                    TaskId = status.TaskId,
                    ProductId = status.ProductId,
                    Type = type,
                    SubmittedVendors = type == UserType.Customer ? status.SubmittedVendors : (int?)null,
                    TotalVendors = type == UserType.Customer ? status.TotalVendors : (int?)null,
                    ProductDocsUri = status.PartRevisionId != null ?
                                     newProductDocs.Where(x => x.PartRevisionId == status.PartRevisionId).Select(x => x.DocUri).ToList() :
                                     newProductDocs.Where(x => x.PartRevisionId == null).Select(x => x.DocUri).ToList(),
                    PartRevisionDescription = status?.PartRevision?.Description,
                    PartRevisionName = status?.PartRevision?.Name,
                    KeepCurrentRevisionReason = status?.KeepCurrentRevisionReason,
                    CancelRFQReason = status.RFQActionReason?.Reason,
                    CancelRFQDescription = status.RFQActionReason?.Description,
                    TimeStamp = status._createdAt.Value,
                    VendorRFQStatus = vendorRFQStatus,
                };
                dtoList.Add(dto);
            }
            return await PageOfResultsSetAsync<ReviewRFQStatusDTO>(dtoList.AsQueryable<ReviewRFQStatusDTO>(), page, pageSize, orderBy, ascending, "GetReviewRFQStatus");
        }



        /// <summary>
        /// Customer wants to keep current revision
        /// </summary>
        /// <param name="productId">Product/RFQ Id</param>
        [HttpPost]
        [Route("rfqs/customer/{userId}/CustomerKeepCurrentRevision/{productId:int}", Name = "CustomerKeepCurrentRevision")]
        public IHttpActionResult CustomerKeepCurrentRevision(int productId)
        {
            if (userContext.User == null || userContext.Company == null)
                return BadRequest(IndicatingMessages.InvalidAccess);

            var formDataDic = HttpContext.Current.Request.Form.ToDictionary();
            if (!formDataDic.ContainsKey("keepCurrentRevisionReason"))
                return BadRequest(IndicatingMessages.MissingFormData);

            var customerTask = taskDataService.FindTaskDataListByProductId(productId).FirstOrDefault(x => x.RFQBidId == null);
            if (customerTask.StateId != (int)States.PendingRFQRevision)
                return BadRequest(IndicatingMessages.TaskStateMismatch);

            var keepCurrentRevisionReason = formDataDic["keepCurrentRevisionReason"].ToString();
            using (var trans = AsyncTransactionScope.StartNew())
            {
                var bidRFQStatus = bidRFQStatusService.FindBidRFQStatusListByProductId(productId).LastOrDefault();
                var tasks = taskDataService.FindTaskDatasByProductId(productId);
                var partRevision = partRevisionService.FindPartRevisionByProductId(productId).LastOrDefault();
                try
                {
                    // Add new RFQ Cycle
                    customerTask.StateId = (int)States.KeepCurrentRFQRevision;
                    customerTask.ModifiedUtc = DateTime.UtcNow;
                    customerTask.ModifiedByUserId = userContext.UserId;
                    taskDataService.Update(customerTask);
                    productBL.AddRFQCycle(customerTask, tasks, partRevision?.Id);
                }
                catch (Exception ex)
                {
                    return BadRequest("CustomerKeepCurrentRevision: " + ex.RetrieveErrorMessage());
                }

                // Start Bid Timer here
                var timer = timerSetupService.FindTimerSetupByProductIdTimerType(productId, TypeOfTimers.BidTimer);
                var bidInterval = timer?.Interval;
                if (bidInterval != null)
                {
                    var error = taskDataBL.SetupTimer(productId, bidInterval, TypeOfTimers.BidTimer);
                    if (error != null)
                    {
                        return BadRequest("SetupTimers: " + error);
                    }
                }
                trans.Complete();
                return Ok("Success");
            }
        }

        /// <summary>
        /// Get TimerSetup histoy by product Id
        /// </summary>
        /// <param name="productId">Product Id</param>      
        /// <param name="type">Type of Timers</param>   
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [HttpGet]
        [Route("rfqs/GetTimerSetupHistory/{productId:int}", Name = "GetTimerSetupHistory")]
        public async Task<PagedResultSet<TimerSetupDTO>> GetTimerSetupHistory(int productId, TypeOfTimers? type = null,
                                                                                int? page = 1, int pageSize = PageSize,
                                                                                string orderBy = nameof(TimerSetup._updatedAt),
                                                                                bool ascending = true)
        {
            var timers = timerSetupService.FindAllTimerSetupsByProductIdTimerType(productId, type);
            return await PageOfResultsSetAsync<TimerSetup, TimerSetupDTO>(timers, page, pageSize, orderBy, ascending, "GetTimerSetupHistory");
        }


        /// <summary>
        /// Customer updates RFQ quantities during RFQ bid
        /// </summary>
        /// <param name="productId">Product/RFQ Id</param>
        /// <param name="qtys">Array of quantities</param>
        [HttpPost]
        [Route("rfqs/customer/{userId}/UpdateRFQQuantities/{productId:int}", Name = "UpdateRFQQuantities")]
        public IHttpActionResult UpdateRFQQuantities(int productId, [FromUri] int[] qtys)
        {
            if (qtys == null || qtys.Length == 0)
                return BadRequest(IndicatingMessages.MissingFormData);

            var td = taskDataService.FindTaskDataByProductId(productId);
            if (td == null)
                return BadRequest(IndicatingMessages.TaskNotFound);

            var res = productBL.UpdateRFQQuantities(productId, qtys.ToList());
            if (res != null)
            {
                return BadRequest(res);
            }

            return Ok("Success");
        }


        /// <summary>
        /// Customer adds extra quantities after state of QuoteAccepted
        /// </summary>
        /// <param name="productId">Product/RFQ Id</param>
        [HttpPost]
        [Route("rfqs/userId/{userId}/CustomerAddExtraQuantities/{productId:int}", Name = "CustomerAddExtraQuantities")]
        public async Task<IHttpActionResult> CustomerAddExtraQuantities(int productId)
        {
            using (var trans = AsyncTransactionScope.StartNew())
            {
                var td = taskDataService.FindTaskDataByProductId(productId);
                if (td == null)
                    return BadRequest(IndicatingMessages.TaskNotFound);

                if (td.CanReorder() == false)
                    return BadRequest(IndicatingMessages.TaskStateMismatch);

                var rfqvm = dashboardBL.SetupExtraQuantity(td);
                StateTransitionViewModel model = new StateTransitionViewModel
                {
                    group = Triggers.SetupUnitPricesForExtraQuantities.ToString(),
                    TaskData = td,
                    UserType = USER_TYPE.Vendor,
                    RFQVM = rfqvm,
                };
                var result = await homeBL.TaskStateHandler(model);
                if (result != null)
                {
                    return BadRequest(result);
                }
                trans.Complete();
                return Ok("Success");
            }
        }

        /// <summary>
        /// Get vendor stats by company ID 
        /// </summary>
        /// <param name="companyId">Company Id</param>

        [HttpGet]
        [Route("rfqs/GetVendorStatsByCompanyId/{companyId:int}", Name = "GetVendorStatsByCompanyId")]
        public VendorStatsViewModel GetVendorStatsByCompanyId(int companyId)
        {

            return productBL.GetVendorStatsByCompanyId(companyId);
        }


        ///<summary>
        /// Get a customer's vendor's all products
        /// </summary>
        /// <param name="vendorId">vendor ID.</param>
        /// <param name="exclude">exclude vendor ids</param>
        /// <param name="filter">filtered by string</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Numbe of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [HttpGet]
        [Route("products/customer/{userId}/products/{vendorId:int}", Name = "GetCustomerVendorProducts")]
        public async Task<PagedResultSet<ProductDTO>> GetCustomerVendorProducts(int vendorId, [FromUri] int[] exclude = null, string filter = null,
                                                                                int? page = 1, int pageSize = PageSize,
                                                                                string orderBy = nameof(Product.CreatedDate),
                                                                                bool ascending = false)
        {
            var customerId = userContext.Company.Id;
            var products = productService.FindProductsByCustomerId(customerId).Where(x => x.VendorId == vendorId && !exclude.Contains(x.Id));

            // vendors from sharing products
            var sharing = productSharingService.FindProductSharingsByComanyId(customerId)
                .Where(x => x.Product.VendorId == vendorId)
                .Select(x => x.Product);

            products = products.Union(sharing);

            // Get records from [ProductPriceQuotes] 
            DateTime now = DateTime.UtcNow;
            var priceQuotesProducts = productPriceQuoteService.FindProductPriceQuotesByVendorId(vendorId).Where(x => x.ExpireDate > now).Select(x => x.ProductId);
            products = products.Where(x => priceQuotesProducts.Contains(x.Id));

            if (filter != null)
            {
                filter = filter.Trim().ToUpper();
                products = products.Where(x => x.Name.ToUpper().Contains(filter)
                                               || x.PartNumber.ToUpper().Contains(filter)
                                               || x.PartNumberRevision.ToUpper().Contains(filter));
            }
            return await PageOfResultsSetAlternativeMapperAsync<Product, ProductDTO>(products, page, pageSize, orderBy, ascending, "GetCustomerVendorProducts");
        }

        ///<summary>
        /// Get a vendor's customer's all products
        /// </summary>
        /// <param name="customerId">customer ID.</param>
        /// <param name="exclude">exclude vendor ids</param>
        /// <param name="filter">filtered by string</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Numbe of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [HttpGet]
        [Route("products/vendor/{userId}/products/{customerId:int}", Name = "GetVendorCustomerProducts")]
        public async Task<PagedResultSet<ProductDTO>> GetVendorCustomerProducts(int customerId, [FromUri] int[] exclude = null, string filter = null,
                                                                                int? page = 1, int pageSize = PageSize,
                                                                                string orderBy = nameof(Product.CreatedDate),
                                                                                bool ascending = false)
        {
            var vendorId = userContext.Company.Id;
            var products = productService.FindProductsByVendorId(vendorId).Where(x => x.CustomerId == customerId && !exclude.Contains(x.Id));

            // customers from sharing products
            var sharing = productSharingService.FindProductSharingsByComanyId(vendorId)
                .Where(x => x.Product.CustomerId == customerId)
                .Select(x => x.Product);

            products = products.Union(sharing);

            // Get records from [ProductPriceQuotes] 
            DateTime now = DateTime.UtcNow;
            var priceQuotesProducts = productPriceQuoteService.FindProductPriceQuotesByVendorId(vendorId).Where(x => x.ExpireDate > now).Select(x => x.ProductId);
            products = products.Where(x => priceQuotesProducts.Contains(x.Id));

            if (filter != null)
            {
                filter = filter.Trim().ToUpper();
                products = products.Where(x => x.Name.ToUpper().Contains(filter)
                                               || x.PartNumber.ToUpper().Contains(filter)
                                               || x.PartNumberRevision.ToUpper().Contains(filter));
            }
            return await PageOfResultsSetAlternativeMapperAsync<Product, ProductDTO>(products, page, pageSize, orderBy, ascending, "GetCustomerVendorProducts");
        }

        ///<summary>
        /// Get product price quotes by product ID
        /// </summary>
        /// <param name="id">Product ID.</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Numbe of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [HttpGet]
        [Route("products/{id:int}/GetProductPriceQuotes", Name = "GetProductPriceQuotes")]
        public async Task<PagedResultSet<ProductPriceQuoteDTO>> GetProductPriceQuotes([FromUri] int id,
                                                                            int? page = 1, int pageSize = PageSize,
                                                                            string orderBy = nameof(ProductPriceQuote.CreatedAt),
                                                                            bool ascending = false)
        {
            var productPriceQuotes = productPriceQuoteService.FindProductPriceQuotesByProductId(id);

            return await PageOfResultsSetAlternativeMapperAsync<ProductPriceQuote, ProductPriceQuoteDTO>(productPriceQuotes, page, pageSize, orderBy, ascending, "GetProductPriceQuotes");
        }

        ///<summary>
        /// Get latest product price quote by product ID and vendor ID
        /// </summary>
        /// <param name="id">Product ID.</param>
        /// <param name="vendorId">Vendor ID.</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Numbe of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [HttpGet]
        [Route("products/{id:int}/GetProductPriceQuotesByVendor/{vendorId:int}", Name = "GetProductPriceQuotesByVendor")]
        public async Task<PagedResultSet<ProductPriceQuoteDTO>> GetProductPriceQuotesByVendor([FromUri] int id, [FromUri] int vendorId,
                                                                            int? page = 1, int pageSize = PageSize,
                                                                            string orderBy = nameof(ProductPriceQuote.CreatedAt),
                                                                            bool ascending = false)
        {
            var productPriceQuotes = productPriceQuoteService.FindProductPriceQuotes(id, vendorId);

            return await PageOfResultsSetAlternativeMapperAsync<ProductPriceQuote, ProductPriceQuoteDTO>(productPriceQuotes, page, pageSize, orderBy, ascending, "GetProductPriceQuotesByVendor");
        }

#if STAGING

        /// <summary>
        /// This endpoint is only for QA to simulate RevisionRequest Timer timed out (no need to wait actual days to expire)
        /// </summary>
        /// <param name="productId">RFQ Id</param>
        /// <param name="vendors">array of vendor Ids</param>

        [HttpPost]
        [Route("rfqs/RevisionRequestTimeoutHandler/{productId:int}", Name = "RevisionRequestTimeoutHandler")]
        public async Task<IHttpActionResult> RevisionRequestTimeoutHandler(int productId, [FromUri] int[] vendors)
        {
            using (var trans = AsyncTransactionScope.StartNew(transOptions))
            {
                await taskDataBL.RevisionRequestTimeoutHandler(productId, vendors.ToList());
                trans.Complete();
                return Ok("Success");
            }
        }

        /// <summary>
        /// This endpoint is only for QA to simulate Bid Timer timed out (no need to wait actual days to expire)
        /// </summary>
        /// <param name="productId">RFQ Id</param>

        [HttpPost]
        [Route("rfqs/BidTimeoutHandler/{productId:int}", Name = "BidTimeoutHandler")]
        public async Task<IHttpActionResult> BidTimeoutHandler(int productId)
        {
            using (var trans = AsyncTransactionScope.StartNew(transOptions))
            {
                await taskDataBL.BidTimeoutHandler(productId);
                trans.Complete();
                return Ok("Success");
            }
        }

        /// <summary>
        /// This endpoint is only for QA to simulate Bid will expire soon Timer timed out (no need to wait actual days to expire)
        /// </summary>
        /// <param name="productId">RFQ Id</param>

        [HttpPost]
        [Route("rfqs/BidWillExpireSoonHandler/{productId:int}", Name = "BidWillExpireSoonHandler")]
        public async Task<IHttpActionResult> BidWillExpireSoonHandler(int productId)
        {
            using (var trans = AsyncTransactionScope.StartNew(transOptions))
            {
                await taskDataBL.BidWillExpireSoonHandler(productId, false);
                trans.Complete();
                return Ok("Success");
            }
        }

#endif

        private IQueryable<ProductDTO> SearchProductDTO(IQueryable<ProductDTO> products, string search, UserType type)
        {
            search = search.Trim().ToUpper();
            return products.Where(x => x.PartNumber != null && x.PartNumber.ToUpper().Contains(search)
                                                    || x.Name != null && x.Name.ToUpper().Contains(search)
                                                    || type == UserType.Customer && x.VendorName.ToUpper().Contains(search)
                                                    || type == UserType.Vendor && x.CustomerName.ToUpper().Contains(search));
        }


    }
}

