using System;
using System.Collections.Generic;
using AutoMapper;
using Omnae.BusinessLayer;
using Omnae.BusinessLayer.Interface;
using Omnae.Model.Models;
using Omnae.Service.Service.Interfaces;
using Omnae.WebApi.DTO;
using Omnae.WebApi.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using Omnae.BlobStorage;
using Omnae.BusinessLayer.Models;
using Omnae.Common;
using Omnae.Data;
using Omnae.Data.Query;
using Omnae.Model.Context;
using Omnae.WebApi.Util;
using Common;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;
using Omnae.Model.Extentions;
using Serilog;
using static Omnae.Data.Query.NcrQuery;
using static Omnae.Data.Query.TaskDataQuery;

namespace Omnae.WebApi.Controllers
{
    /// <summary>
    /// Core Api for NCR 
    /// </summary>
    [EnableCors("*", "*", "*")]
    [RoutePrefix("api")]
    public class NcrController : BaseApiController
    {
        private CompanyAccountsBL CompanyBl { get; }
        private IOrderService OrderService { get; }
        private OrdersBL OrdersBL { get; }
        private IHomeBL HomeBL { get; }
        private INCReportService NcReportService { get; }
        private ICompanyService CompanyService { get; }
        private IMapper Mapper { get; }
        private IOrderStateTrackingService OrderStateTrackingService { get; }
        private ITaskDataService TaskDataService { get; }
        private IProductStateTrackingService ProductStateTrackingService { get; }
        private INCRImagesService NCRImagesService { get; }
        private ILogedUserContext UserContext { get; }
        private IProductService ProductService { get; }
        private ProductBL ProductBL { get; }
        private NcrBL NcrBL { get; }
        private IPriceBreakService priceBreakService { get; }
        

        /// <summary>
        /// NCR controller constructor
        /// </summary>
        public NcrController(ILogger log, CompanyAccountsBL companyBl, IOrderService orderService, OrdersBL ordersBl, IHomeBL homeBl, INCReportService ncReportService, ICompanyService companyService, IMapper mapper, IOrderStateTrackingService orderStateTrackingService, ITaskDataService taskDataService, IProductStateTrackingService productStateTrackingService, INCRImagesService ncrImagesService, ILogedUserContext userContext, IProductService productService, ProductBL productBl, NcrBL ncrBl, IPriceBreakService priceBreakService) : base(log)
        {
            CompanyBl = companyBl;
            OrderService = orderService;
            OrdersBL = ordersBl;
            HomeBL = homeBl;
            NcReportService = ncReportService;
            CompanyService = companyService;
            Mapper = mapper;
            OrderStateTrackingService = orderStateTrackingService;
            TaskDataService = taskDataService;
            ProductStateTrackingService = productStateTrackingService;
            NCRImagesService = ncrImagesService;
            UserContext = userContext;
            ProductService = productService;
            ProductBL = productBl;
            NcrBL = ncrBl;
            this.priceBreakService = priceBreakService;
        }

        /// <summary>
        /// List NCRs
        /// </summary>
        /// <param name="companyId">Company Id to filter results by</param>
        /// <param name="mode">User mode (customer or vendor)</param>
        /// <param name="type">Product filter type</param>
        /// <param name="filter">filtered by string</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of records a page can hold</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="ascending">Asc/Desc</param>
        [Route("ncrs", Name = "ListNCRs")]
        [HttpGet]
        public async Task<PagedResultSet<NCReportDTO>> ListNCRs(int? companyId,
                                                                NcrQuery.NcrType mode = NcrQuery.NcrType.Customer,
                                                                NcrQuery.NcrFilter type = NcrQuery.NcrFilter.All,
                                                                string filter = null, int? page = 1, int pageSize = PageSize,
                                                                string orderBy = nameof(NCReportDTO._updatedAt), bool ascending = false)
        {
            IQueryable<NCReport> ncrs = Enumerable.Empty<NCReport>().AsQueryable();
            if (companyId == null)
            {
                ncrs = NcReportService.FindNCReports();
            }
            else
            {
                ncrs = mode == NcrQuery.NcrType.Customer
                    ? NcReportService.FindNCReportByCustomerId((int)companyId).FilterBy((int)companyId, type, NcrQuery.NcrType.Customer, filter)
                    : NcReportService.FindNCReportByVendorId((int)companyId).FilterBy((int)companyId, type, NcrQuery.NcrType.Vendor, filter);
            }

            return await PageOfResultsSetAsync<NCReport, NCReportDTO>(ncrs, page, pageSize, orderBy, ascending, "ListNCRs");
        }

        /// <summary>
        /// Get a NCR
        /// </summary>
        /// <param name="ncrId"></param>
        /// <returns>A NCR</returns>
        //GetNCR
        [ResponseType(typeof(NcrDescriptionViewModel))]
        [Route("ncrs/{ncrId:int}", Name = "ShowNCR")]
        [HttpGet]
        public IHttpActionResult ShowNCR([Required] int ncrId)
        {
            var ncr = NcReportService.FindNCReportById(ncrId);
            if (ncr == null || ncr.TaskId != null && ncr.Task.isEnterprise == false)
            {
                return NotFound();
            }

            try
            {
                var mod = new NcrDescriptionViewModel();
                var errors = HomeBL.NcrDetails(ncrId, ref mod);
                if (errors != null)
                {
                    return BadRequest(errors);
                }

                var model = Mapper.Map<NcrDescriptionDTO>(mod);
                model.NcrHistory = OrdersBL.NcrHistory(ncr);

                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.RetrieveErrorMessage());
            }
        }

        [ResponseType(typeof(NcrDescriptionViewModel))]
        [Route("NCRs/user/{userId}", Name = "CreateNCR")]
        [HttpPost]
        public async Task<IHttpActionResult> Post(string userId)
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            if (UserContext?.Company == null)
                return BadRequest("Invalid User or Customer");

            var httpRequest = HttpContext.Current.Request;
            var allFiles = GetPostedFiles(HomeBL);

            try
            {
                var formDataDic = httpRequest.Form.ToDictionaryOfObjects();
                var model = Slapper.AutoMapper.Map<NcrCreationApiViewModel>(formDataDic, false);

                if (model.OrderId == null)
                    return BadRequest("Missing Order Id");
                if (model.ProductId == null)
                    return BadRequest("Missing Product Id");

                var dbProd = ProductService.FindProductById((int)model.ProductId);
                if (dbProd == null)
                    return BadRequest("Product not found");
                if (!ProductBL.IsUsersProduct(dbProd))
                    return BadRequest("Invalid Product");

                //var dbNcr = NcReportService.FindNCReportByProductIdOrderId((int)model.ProductId, (int)model.OrderId).LastOrDefault();
                //if (dbNcr != null)
                //    return BadRequest("NCR for this Order already exists");

                var dbOrder = OrderService.FindOrderById((int)model.OrderId);
                if (dbOrder?.TaskData == null)
                    return BadRequest("Invalid Order");
                if (dbOrder.TaskData.StateId != (int)States.ProductionComplete)
                    return BadRequest("Order is in a Invalid state, You cant create a new NCR for this order. Order need to be in ProductionComplete state.");


                var ncrDesc = Mapper.Map<NcrDescriptionViewModel>(model);

                var task = dbOrder.TaskData;

                //Fill the data
                ncrDesc.TaskId = task.TaskId;
                ncrDesc.StateId = States.NCRVendorRootCauseAnalysis; //NCR Started by the customer and start with the State that Vender need to look the root cause.
                ncrDesc.CustomerId = UserContext.Company.Id;
                ncrDesc.VendorId = dbProd.VendorId;
                ncrDesc.CarrierName = dbOrder.CarrierName;
                if (ncrDesc.VendorId == null)
                {
                    ncrDesc.VendorId = int.Parse(formDataDic["vendorId"].ToString());
                }


                var ncr = Mapper.Map<NCReport>(ncrDesc);
                ncr.NCDetectedby = DETECTED_BY.CUSTOMER;

                //////////////////////////////////////////
                //Validation
                ModelState.Clear();
                Validate(ncr);
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                if (!(ncr.StateId != null && ncr.StateId >= States.NCRCustomerStarted && ncr.StateId <= States.NCRClosed))
                    return BadRequest("Invalid State");

                using (var tran = AsyncTransactionScope.StartNew())
                {
                    var ncrId = await NcrBL.CreateNCReport(ncrDesc, allFiles);

                    var result = Mapper.Map(ncr, ncrDesc);
                    result.NCRId = ncrId;

                    tran.Complete();
                    return CreatedAtRoute("CreateNCR", new { ncrId = ncrId }, result);
                }
            }
            finally
            {
                DeleteUploadFiles(allFiles);
            }
        }

        /// <summary>
        /// A endpoint to a Vendor Approve a NCR
        /// </summary>
        /// <param name="ncrId">The NCR ID</param>
        /// <param name="userId">The logged user id, will be considered a customer</param>
        /// <returns></returns>
        [HttpPost]
        [Route("NCRs/{ncrId:int}/Approve/vendor/{userId}")]
        [ResponseType(typeof(NcrDescriptionViewModel))]
        public async Task<IHttpActionResult> Approve([Required] string userId)//[Required] int ncrId, [Required] string userId)
        {
            return await ChangeNcrStateByCustomersNcrId(Triggers.NCRApproval, NC_ROOT_CAUSE.VENDOR);
        }

        /// <summary>
        /// A endpoint to a Customer AnalysisRootCause a NCR
        /// </summary>
        /// <param name="ncrId">The NCR ID</param>
        /// <param name="userId">The logged user id, will be considered a customer</param>
        /// <returns></returns>
        [HttpPost]
        [Route("NCRs/{ncrId:int}/AnalysisRootCause/customer/{userId}")]
        [ResponseType(typeof(NcrDescriptionViewModel))]
        public async Task<IHttpActionResult> AnalysisRootCause([Required] string userId)//[Required] int ncrId, [Required] string userId)
        {
            return await ChangeNcrStateByCustomersNcrId(Triggers.NCRAnalysisRootCause);
        }

        /// <summary>
        /// A endpoint to a Vendor Decline a NCR
        /// </summary>
        /// <param name="ncrId">The NCR ID</param>
        /// <param name="userId">The logged user id, will be considered a customer</param>
        /// <returns></returns>
        [HttpPost]
        [Route("NCRs/{ncrId:int}/decline/vendor/{userId}")]
        [ResponseType(typeof(NcrDescriptionViewModel))]
        public async Task<IHttpActionResult> Decline([Required] string userId)//[Required] int ncrId, [Required] string userId)
        {
            return await ChangeNcrStateByCustomersNcrId(Triggers.NCRRootCauseOnCustomer, NC_ROOT_CAUSE.VENDOR);
        }

        /// <summary>
        /// A endpoint to a Customer CorrectiveReceivedApproved a NCR
        /// </summary>
        /// <param name="ncrId">The NCR ID</param>
        /// <param name="userId">The logged user id, will be considered a customer</param>
        /// <returns></returns>
        [HttpPost]
        [Route("NCRs/{ncrId:int}/CorrectiveReceivedApproved/customer/{userId}")]
        [ResponseType(typeof(NcrDescriptionViewModel))]
        public async Task<IHttpActionResult> CorrectiveReceivedApproved([Required] string userId)//[Required] int ncrId, [Required] string userId)
        {
            return await ChangeNcrStateByCustomersNcrId(Triggers.NCRCorrectiveReceivedApproved);
        }

        /// <summary>
        /// A endpoint to a Customer RejectCorrectiveAction a NCR
        /// </summary>
        /// <param name="ncrId">The NCR ID</param>
        /// <param name="userId">The logged user id, will be considered a customer</param>
        /// <returns></returns>
        [HttpPost]
        [Route("NCRs/{ncrId:int}/RejectCorrectiveAction/customer/{userId}")]
        [ResponseType(typeof(NcrDescriptionViewModel))]
        public async Task<IHttpActionResult> RejectCorrectiveAction([Required] string userId)//[Required] int ncrId, [Required] string userId)
        {
            //entity.ArbitrateCustomerCauseReason = value;
            return await ChangeNcrStateByCustomersNcrId(Triggers.NCRRejectCorrectiveAction);
        }

        /// <summary>
        /// A endpoint to a Customer RejectRootCause a NCR
        /// </summary>
        /// <param name="ncrId">The NCR ID</param>
        /// <param name="userId">The logged user id, will be considered a customer</param>
        /// <returns></returns>
        [HttpPost]
        [Route("NCRs/{ncrId:int}/RejectRootCause/customer/{userId}")]
        [ResponseType(typeof(NcrDescriptionViewModel))]
        public async Task<IHttpActionResult> RejectRootCause([Required] string userId)//[Required] int ncrId, [Required] string userId)
        {
            return await ChangeNcrStateByCustomersNcrId(Triggers.NCRRejectRootCause);
        }

        /// <summary>
        /// A endpoint to a Vendor CorrectivePartsComplete a NCR
        /// </summary>
        /// <param name="ncrId">The NCR ID</param>
        /// <param name="userId">The logged user id, will be considered a customer</param>
        /// <returns></returns>
        [HttpPost]
        [Route("NCRs/{ncrId:int}/CorrectivePartsComplete/vendor/{userId}")]
        [ResponseType(typeof(NcrDescriptionViewModel))]
        public async Task<IHttpActionResult> CorrectivePartsComplete([Required] string userId)//[Required] int ncrId, [Required] string userId)
        {
            return await ChangeNcrStateByCustomersNcrId(Triggers.NCRCorrectivePartsComplete, NC_ROOT_CAUSE.VENDOR);
        }

        /// <summary>
        /// A endpoint to a Customer CorrectivePartsReceival a NCR
        /// </summary>
        /// <param name="ncrId">The NCR ID</param>
        /// <param name="userId">The logged user id, will be considered a customer</param>
        /// <returns></returns>
        [HttpPost]
        [Route("NCRs/{ncrId:int}/CorrectivePartsReceival/customer/{userId}")]
        [ResponseType(typeof(NcrDescriptionViewModel))]
        public async Task<IHttpActionResult> CorrectivePartsReceival([Required] string userId)//[Required] int ncrId, [Required] string userId)
        {
            return await ChangeNcrStateByCustomersNcrId(Triggers.NCRCorrectivePartsReceival);
        }

        /// <summary>
        /// A endpoint to a Customer CorrectiveAccepted a NCR
        /// </summary>
        /// <param name="ncrId">The NCR ID</param>
        /// <param name="userId">The logged user id, will be considered a customer</param>
        /// <returns></returns>
        [HttpPost]
        [Route("NCRs/{ncrId:int}/CorrectiveAccepted/customer/{userId}")]
        [ResponseType(typeof(NcrDescriptionViewModel))]
        public async Task<IHttpActionResult> CorrectiveAccepted([Required] string userId)//[Required] int ncrId, [Required] string userId)
        {
            return await ChangeNcrStateByCustomersNcrId(Triggers.NCRCorrectiveAccepted);
        }

        /// <summary>
        /// A endpoint to a Customer CorrectiveReceivedRejected a NCR
        /// </summary>
        /// <param name="ncrId">The NCR ID</param>
        /// <param name="userId">The logged user id, will be considered a customer</param>
        /// <returns></returns>
        [HttpPost]
        [Route("NCRs/{ncrId:int}/CorrectiveReceivedRejected/customer/{userId}")]
        [ResponseType(typeof(NcrDescriptionViewModel))]
        public async Task<IHttpActionResult> CorrectiveReceivedRejected([Required] string userId)//[Required] int ncrId, [Required] string userId)
        {
            //entity.RejectCorrectivePartsReason = value;
            return await ChangeNcrStateByCustomersNcrId(Triggers.NCRCorrectiveReceivedRejected);
        }

        /// <summary>
        /// A endpoint to a Customer ArbitrateVendorCause a NCR
        /// </summary>
        /// <param name="ncrId">The NCR ID</param>
        /// <param name="userId">The logged user id, will be considered a customer</param>
        /// <returns></returns>
        [HttpPost]
        [Route("NCRs/{ncrId:int}/ArbitrateVendorCause/customer/{userId}")]
        [ResponseType(typeof(NcrDescriptionViewModel))]
        public async Task<IHttpActionResult> ArbitrateVendorCause([Required] string userId)//[Required] int ncrId, [Required] string userId)
        {
            //entity.ArbitrateVendorCauseReason = value;
            return await ChangeNcrStateByCustomersNcrId(Triggers.NCRArbitrateVendorCause);
        }

        /// <summary>
        /// A endpoint to a Customer ArbitrateCustomerCause a NCR
        /// </summary>
        /// <param name="ncrId">The NCR ID</param>
        /// <param name="userId">The logged user id, will be considered a customer</param>
        /// <returns></returns>
        [HttpPost]
        [Route("NCRs/{ncrId:int}/ArbitrateCustomerCause/customer/{userId}")]
        [ResponseType(typeof(NcrDescriptionViewModel))]
        public async Task<IHttpActionResult> ArbitrateCustomerCause([Required] string userId)//[Required] int ncrId, [Required] string userId)
        {
            return await ChangeNcrStateByCustomersNcrId(Triggers.NCRArbitrateCustomerCause);
        }

        /// <summary>
        /// A endpoint to a Customer ArbitrateDispute a NCR
        /// </summary>
        /// <param name="ncrId">The NCR ID</param>
        /// <param name="userId">The logged user id, will be considered a customer</param>
        /// <returns></returns>
        [HttpPost]
        [Route("NCRs/{ncrId:int}/ArbitrateDispute/customer/{userId}")]
        [ResponseType(typeof(NcrDescriptionViewModel))]
        public async Task<IHttpActionResult> ArbitrateDispute([Required] string userId)//[Required] int ncrId, [Required] string userId)
        {
            return await ChangeNcrStateByCustomersNcrId(Triggers.NCRArbitrateDispute);
        }

        /// <summary>
        /// A endpoint to a Customer ArbitrateCustomerCauseDamage a NCR
        /// </summary>
        /// <param name="ncrId">The NCR ID</param>
        /// <param name="userId">The logged user id, will be considered a customer</param>
        /// <returns></returns>
        [HttpPost]
        [Route("NCRs/{ncrId:int}/ArbitrateCustomerCauseDamage/customer/{userId}")]
        [ResponseType(typeof(NcrDescriptionViewModel))]
        public async Task<IHttpActionResult> ArbitrateCustomerCauseDamage([Required] string userId)//[Required] int ncrId, [Required] string userId)
        {
            return await ChangeNcrStateByCustomersNcrId(Triggers.NCRArbitrateCustomerCauseDamage);
        }

        /// <summary>
        /// A endpoint to a Customer Close a NCR
        /// </summary>
        /// <param name="ncrId">The NCR ID</param>
        /// <param name="userId">The logged user id, will be considered a customer</param>
        /// <returns></returns>
        [HttpPost]
        [Route("NCRs/{ncrId:int}/Close/customer/{userId}")]
        [ResponseType(typeof(NcrDescriptionViewModel))]
        public async Task<IHttpActionResult> Close([Required] string userId)//[Required] int ncrId, [Required] string userId)
        {
            return await ChangeNcrStateByCustomersNcrId(Triggers.NCRClose);
        }

        /// <summary>
        /// A endpoint to a Customer CustomerRevision a NCR
        /// </summary>
        /// <param name="ncrId">The NCR ID</param>
        /// <param name="userId">The logged user id, will be considered a customer</param>
        /// <returns></returns>
        [HttpPost]
        [Route("NCRs/{ncrId:int}/CustomerRevision/customer/{userId}")]
        [ResponseType(typeof(NcrDescriptionViewModel))]
        public async Task<IHttpActionResult> CustomerRevision([Required] string userId)//[Required] int ncrId, [Required] string userId)
        {
            return await ChangeNcrStateByCustomersNcrId(Triggers.NCRCustomerRevision);
        }

        /// <summary>
        /// A endpoint to a Customer Reject with RootCase a NCR
        /// </summary>
        /// <param name="ncrId">The NCR ID</param>
        /// <param name="userId">The logged user id, will be considered a customer</param>
        /// <returns></returns>
        [HttpPost]
        [Route("NCRs/{ncrId:int}/RejectWithRootCase/customer/{userId}")]
        [ResponseType(typeof(NcrDescriptionViewModel))]
        public async Task<IHttpActionResult> RejectWithRootCase([Required] string userId)//[Required] int ncrId, [Required] string userId, [Required] string description)
        {
            //entity.RejectRootCauseReason = value;
            return await ChangeNcrStateByCustomersNcrIdAndDescription(Triggers.NCRRejectRootCause);
        }

        /// <summary>
        /// A endpoint to a Customer Reject with Corrective a NCR
        /// </summary>
        /// <param name="ncrId">The NCR ID</param>
        /// <param name="userId">The logged user id, will be considered a customer</param>
        /// <returns></returns>
        [HttpPost]
        [Route("NCRs/{ncrId:int}/RejectWithCorrectiveAction/customer/{userId}")]
        [ResponseType(typeof(NcrDescriptionViewModel))]
        public async Task<IHttpActionResult> RejectWithCorrectiveAction([Required] string userId)//[Required] int ncrId, [Required] string userId, [Required] string description)
        {
            //entity.RejectCorrectiveActionReason = value;
            return await ChangeNcrStateByCustomersNcrIdAndDescription(Triggers.NCRRejectCorrectiveAction);
        }



        /// <summary>
        /// Get a customer's/vendor's NCR Statitics
        /// </summary>
        /// <param name="companyId">vendor ID.</param>
        /// <param name="mode">User type</param>
        [HttpGet]
        [Route("companies/{companyId:int}/GetNCRStatistics", Name = "GetNCRStatistics")]
        public ActiveNCRsStatisticsDTO GetNCRStatistics(int companyId, TaskDataQuery.UserType mode = TaskDataQuery.UserType.Vendor)
        {
            IQueryable<NCReport> ncrs = Enumerable.Empty<NCReport>().AsQueryable();


            int numberOfActionableReviewNCRs = 0;
            int numberOfActionableDisputeActiveNCRs = 0;
            int numberOfActionableResolutionActiveNCRs = 0;

            if (mode == UserType.Customer)
            {
                ncrs = NcReportService.FindNCReportByCompanyId(companyId);
                numberOfActionableReviewNCRs = ncrs.CustomerActionableReviewNCRs().Count();
                numberOfActionableDisputeActiveNCRs = ncrs.CustomerActionableDisputeNCRs().Count();
                numberOfActionableResolutionActiveNCRs = ncrs.CustomerActionableResolutionNCRs().Count();
            }
            else
            {
                ncrs = NcReportService.FindNCReportByVendorId(companyId);
                numberOfActionableReviewNCRs = ncrs.VendorActionableReviewNCRs().Count();
                numberOfActionableDisputeActiveNCRs = ncrs.VendorActionableDisputeNCRs().Count();
                numberOfActionableResolutionActiveNCRs = ncrs.VendorActionableResolutionNCRs().Count();
            }

            ActiveNCRsStatisticsDTO stats = new ActiveNCRsStatisticsDTO
            {
                NumberOfAllActiveNCRs = ncrs.ActiveNCRs().Count(),
                TotalAmoutActiveNCRs = GetTotalNcrsAmount(ncrs),
                NumberOfReviewNCRs = ncrs.ReviewNCRs().Count(),
                NumberOfActionableReviewNCRs = numberOfActionableReviewNCRs,
                NumberOfDisputeActiveNCRs = ncrs.DisputeNCRs().Count(),
                NumberOfActionableDisputeActiveNCRs = numberOfActionableDisputeActiveNCRs,
                NumberOfResolutionActiveNCRs = ncrs.ResolvedNCRs().Count(),
                NumberOfActionableResolutionActiveNCRs = numberOfActionableResolutionActiveNCRs,
            };

            return stats;
        }



        /////////
        

        private decimal GetTotalNcrsAmount(IQueryable<NCReport> ncrs)
        {
            decimal totalAmount = 0;
            foreach (var ncr in ncrs)
            {
                if (ncr.Quantity == 0) continue;
                var unitPrice = priceBreakService.FindPriceBreakByProductIdQty(ncr.ProductId, ncr.Quantity.Value)?.UnitPrice;

                if (unitPrice == null || unitPrice == 0) continue;
                totalAmount += unitPrice.Value * ncr.Quantity.Value;
            }
            return totalAmount;
        }

        private async Task<IHttpActionResult> ChangeNcrStateByCustomersNcrId(Triggers trigger, NC_ROOT_CAUSE rootCase = NC_ROOT_CAUSE.CUSTOMER)
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var allFiles = GetPostedFiles(HomeBL, filesAreRequired: false);

            try
            {
                var formDataDic = HttpContext.Current.Request.Form.ToDictionary();

                if (!formDataDic.ContainsKey("ncrId")
                    || string.IsNullOrWhiteSpace(formDataDic["ncrId"])
                    || !int.TryParse(formDataDic["ncrId"], out var ncrId))
                {
                    return BadRequest("Invalid ncrId value.");
                }

                string trackingNumber = formDataDic.ContainsKey("trackingNumber") ? formDataDic["trackingNumber"] : null;
                string carrierName = formDataDic.ContainsKey("carrierName") ? formDataDic["carrierName"] : null;
                string carrierType = formDataDic.ContainsKey("carrierType") ? formDataDic["carrierType"] : null;
                string shippingAccountNumber = formDataDic.ContainsKey("shippingAccountNumber") ? formDataDic["accountNumber"] : null;

                var ncr = NcReportService.FindNCReportById(ncrId);
                int cType = -1;
                if (trackingNumber != null && !int.TryParse(carrierType, out cType))
                {
                    cType = (int)ncr.Order.CarrierType;
                }

                var model = new StateTransitionViewModel
                {
                    group = trigger.ToString(),
                    RootCause = rootCase,
                    Disposition = null,

                    DetailRootCause = formDataDic.GetOrNull("detailRootCause"),
                    ActionTaken = formDataDic.GetOrNull("actionTaken"),
                    ActionTakenBy = formDataDic.GetOrNull("actionTakenBy"),
                    CorrectiveAction = formDataDic.GetOrNull("correctiveAction"),
                    CarrierType = (SHIPPING_CARRIER_TYPE)cType,
                    ShippingAccountNumber = shippingAccountNumber ?? ncr.Order.ShippingAccountNumber,

                    NcrDescriptionVM = new NcrDescriptionViewModel()
                    {
                        ArbitrateCustomerCauseReason = formDataDic.GetOrNull("arbitrateCustomerCauseReason"),
                        ArbitrateVendorCauseReason = formDataDic.GetOrNull("arbitrateVendorCauseReason"),
                        RejectCorrectivePartsReason = formDataDic.GetOrNull("rejectCorrectivePartsReason"),
                        RejectRootCauseReason = formDataDic.GetOrNull("rejectRootCauseReason"),
                        RejectCorrectiveActionReason = formDataDic.GetOrNull("rejectCorrectiveActionReason"),
                        RootCauseOnCustomerReason = formDataDic.GetOrNull("rootCauseOnCustomerReason"),
                        TrackingNumber = trackingNumber,
                        CarrierName = carrierName,
                    },
                };

                return await ChangeNcrStateByCustomers(ncrId, model, allFiles);
            }
            finally
            {
                DeleteUploadFiles(allFiles);
            }
        }

        private async Task<IHttpActionResult> ChangeNcrStateByCustomersNcrIdAndDescription(Triggers trigger)
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var allFiles = GetPostedFiles(HomeBL, filesAreRequired: false);

            try
            {
                var formDataDic = HttpContext.Current.Request.Form.ToDictionary();

                if (!formDataDic.ContainsKey("ncrId")
                    || string.IsNullOrWhiteSpace(formDataDic["ncrId"])
                    || !int.TryParse(formDataDic["ncrId"], out var ncrId))
                {
                    return BadRequest("Invalid ncrId value.");
                }

                var model = new StateTransitionViewModel
                {
                    group = trigger.ToString(), //TODO Check
                    RootCause = NC_ROOT_CAUSE.CUSTOMER,
                    Disposition = NC_DISPOSITION.REPLACE,

                    NcrDescriptionVM = new NcrDescriptionViewModel()
                    {
                        ArbitrateCustomerCauseReason = formDataDic.GetOrNull("arbitrateCustomerCauseReason"),
                        ArbitrateVendorCauseReason = formDataDic.GetOrNull("arbitrateVendorCauseReason"),
                        RejectCorrectivePartsReason = formDataDic.GetOrNull("rejectCorrectivePartsReason"),
                        RejectRootCauseReason = formDataDic.GetOrNull("rejectRootCauseReason"),
                        RejectCorrectiveActionReason = formDataDic.GetOrNull("rejectCorrectiveActionReason"),
                        RootCauseOnCustomerReason = formDataDic.GetOrNull("rootCauseOnCustomerReason"),
                    }
                };

                return await ChangeNcrStateByCustomers(ncrId, model, allFiles);
            }
            finally
            {
                DeleteUploadFiles(allFiles);
            }
        }

        private async Task<IHttpActionResult> ChangeNcrStateByCustomers(int ncrId, StateTransitionViewModel model, List<HttpPostedFileBase> allFiles)
        {
            if (UserContext?.Company == null)
                return BadRequest("Invalid User or Customer");

            var ncr = NcReportService.FindNCReportById(ncrId);
            if (ncr == null)
                return NotFound();
            if (!(UserContext.Company.Id == ncr.CustomerId || UserContext.Company.Id == ncr.VendorId))
                return Unauthorized();
            if (ncr.StateId == States.NCRClosed)
                return BadRequest("This NCR have a State that cant be changed.");

            var dbProd = ProductService.FindProductById(ncr.ProductId);
            if (dbProd == null)
                return BadRequest("Product not found");
            if (!ProductBL.IsUsersProduct(dbProd))
                return BadRequest("Invalid Product");

            var td = ncr.Task ?? TaskDataService.FindTaskDatasByCustomerId((int)ncr.CustomerId)
                                    .Where(t => t.ProductId == ncr.ProductId)
                                    .WhereIsNCRs()
                                    .Where(t => t.StateId == (int)ncr.StateId)
                                    .OrderByDescending(t => t.TaskId)
                                    .FirstOrDefault();
            if (td == null)
                return BadRequest("Task Data not found.");

            model.ProductId = ncr.ProductId;
            model.TaskData = td;
            model.UserType = UserContext.Company.CompanyType == CompanyType.Customer ? USER_TYPE.Customer : USER_TYPE.Vendor;

            using (var tran = AsyncTransactionScope.StartNew())
            {
                var msg = await HomeBL.TaskStateHandler(model, allFiles);

                if (msg != null && !(msg.Equals(IndicatingMessages.SmsWarningMsg) || msg.Equals(IndicatingMessages.NoCellPhoneWarningMsg)))
                    return BadRequest(msg);

                var entity = NcReportService.FindNCReportById(ncrId);

                entity.ArbitrateCustomerCauseReason = (!string.IsNullOrWhiteSpace(model.NcrDescriptionVM.ArbitrateCustomerCauseReason))
                                                    ? model.NcrDescriptionVM.ArbitrateCustomerCauseReason
                                                    : entity.ArbitrateCustomerCauseReason;
                entity.ArbitrateVendorCauseReason = (!string.IsNullOrWhiteSpace(model.NcrDescriptionVM.ArbitrateVendorCauseReason))
                                                  ? model.NcrDescriptionVM.ArbitrateVendorCauseReason
                                                  : entity.ArbitrateVendorCauseReason;
                entity.RejectCorrectivePartsReason = (!string.IsNullOrWhiteSpace(model.NcrDescriptionVM.RejectCorrectivePartsReason))
                                                   ? model.NcrDescriptionVM.RejectCorrectivePartsReason
                                                   : entity.RejectCorrectivePartsReason;
                entity.RejectRootCauseReason = (!string.IsNullOrWhiteSpace(model.NcrDescriptionVM.RejectRootCauseReason))
                                             ? model.NcrDescriptionVM.RejectRootCauseReason
                                             : entity.RejectRootCauseReason;
                entity.RejectCorrectiveActionReason = (!string.IsNullOrWhiteSpace(model.NcrDescriptionVM.RejectCorrectiveActionReason))
                                                    ? model.NcrDescriptionVM.RejectCorrectiveActionReason
                                                    : entity.RejectCorrectiveActionReason;
                entity.RootCauseOnCustomerReason = (!string.IsNullOrWhiteSpace(model.NcrDescriptionVM.RootCauseOnCustomerReason))
                                                 ? model.NcrDescriptionVM.RootCauseOnCustomerReason
                                                 : entity.RootCauseOnCustomerReason;

                NcReportService.UpdateNCReport(entity);

                tran.Complete();
                return Ok();
            }
        }
    }
}