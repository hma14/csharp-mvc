using AutoMapper;
using Common;
using Libs;
using Libs.Notification;
using Microsoft.Azure;
using Omnae.BlobStorage;
using Omnae.BusinessLayer.Identity.Model;
using Omnae.BusinessLayer.Interface;
using Omnae.BusinessLayer.Models;
using Omnae.BusinessLayer.Services;
using Omnae.Common;
using Omnae.Data;
using Omnae.Model.Context;
using Omnae.Model.Models;
using Omnae.Model.Models.Aspnet;
using Omnae.QuickBooks.QBO;
using Omnae.QuickBooks.ViewModels;
using Omnae.Service.Service;
using Omnae.Service.Service.Interfaces;
using Omnae.ShippingAPI.DHL.Libs;
using Omnae.ShippingAPI.DHL.Models;
using Rotativa;
using Stateless;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Security;
using Humanizer;
using System.Transactions;
using Intuit.Ipp.Data;
using Omnae.Libs.Notification;
using Serilog;
using Omnae.Model.Extentions;
using static Omnae.Data.Query.OrderQuery;

namespace Omnae.BusinessLayer
{
    public class HomeBL : IHomeBL
    {
        public IRFQBidService RfqBidService { get; }
        public ICompanyService CompanyService { get; }
        public ITaskDataService TaskDataService { get; }
        public IPriceBreakService PriceBreakService { get; }
        public IOrderService OrderService { get; }
        public ILogedUserContext UserContext { get; }

        protected IProductService ProductService { get; }

        protected IDocumentService DocumentService { get; }
        protected readonly IDocumentStorageService DocumentStorageService;
        protected readonly IImageStorageService ImageStorageService;

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

        protected readonly IMapper mapper;
        private TaskSetup TaskSetup { get; }
        private TaskDatasBL TaskDatasBL { get; }
        private readonly ICompaniesCreditRelationshipService companiesCreditRelationshipService;
        private readonly DocumentBL documentBL;
        private readonly IProductSharingService productSharingService;
        private readonly IBidRFQStatusService bidRFQStatusService;
        private readonly IVendorBidRFQStatusService vendorBidRFQStatusService;
        private readonly IRFQActionReasonService rfqActionReasonService;
        private readonly IProductPriceQuoteService productPriceQuoteService;
        

        private readonly ILogger Log;

        public HomeBL(IRFQBidService rfqBidService, ICompanyService companyService, ITaskDataService taskDataService, IPriceBreakService priceBreakService, IOrderService orderService, ILogedUserContext userContext, IProductService productService, IDocumentService documentService, IShippingService shippingService, ICountryService countryService, IAddressService addressService, IStateProvinceService stateProvinceService, IOrderStateTrackingService orderStateTrackingService, IProductStateTrackingService productStateTrackingService, IPartRevisionService partRevisionService, IStoredProcedureService spService, INCReportService ncReportService, IRFQQuantityService rfqQuantityService, IExtraQuantityService extraQuantityService, IBidRequestRevisionService bidRequestRevisionService, ITimerSetupService timerSetupService, IQboTokensService qboTokensService, IOmnaeInvoiceService omnaeInvoiceService, INCRImagesService ncrImagesService, IApprovedCapabilityService approvedCapabilityService, IShippingAccountService shippingAccountService, ApplicationDbContext dbUser, ProductBL productBl, NotificationService notificationService, UserContactService userContactService, TimerTriggerService timerTriggerService, NotificationBL notificationBl, PaymentBL paymentBl, ShipmentBL shipmentBl, ChartBL chartBl, IMapper mapper, TaskSetup taskSetup, TaskDatasBL taskDatasBL, ICompaniesCreditRelationshipService companiesCreditRelationshipService, DocumentBL documentBL, IProductSharingService productSharingService, IImageStorageService imageStorageService, IDocumentStorageService documentStorageService, IBidRFQStatusService bidRFQStatusService, IVendorBidRFQStatusService vendorBidRFQStatusService, IRFQActionReasonService irfqActionReasonService, ILogger log, IProductPriceQuoteService productPriceQuoteService)
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
            TaskSetup = taskSetup;
            TaskDatasBL = taskDatasBL;
            this.companiesCreditRelationshipService = companiesCreditRelationshipService;
            this.documentBL = documentBL;
            this.productSharingService = productSharingService;
            ImageStorageService = imageStorageService;
            DocumentStorageService = documentStorageService;
            this.bidRFQStatusService = bidRFQStatusService;
            this.vendorBidRFQStatusService = vendorBidRFQStatusService;
            this.rfqActionReasonService = irfqActionReasonService;
            Log = log;
            this.productPriceQuoteService = productPriceQuoteService;
        }


        public States GetNextState(TaskData taskData, Triggers trigger)
        {
            StateMachine<States, Triggers> stTransition = new StateMachine<States, Triggers>((States)taskData.StateId);
            Func<bool> myFunc = () => TaskSetup.CheckPreconditions(taskData.ProductId.Value, taskData.TaskId);

            if (taskData.isEnterprise == true)
            {
                Utils.RegisterStates(stTransition, myFunc);
            }
            else
            {
                Utils.RegisterStates_Reseller(stTransition, myFunc);
            }


            if (stTransition.CanFire(trigger))
            {
                stTransition.Fire(trigger);
            }
            else
            {
                throw new Exception("Couldn't find next state.");
            }
            return stTransition.State;
        }

        public async Task<string> TaskStateHandler(StateTransitionViewModel model, List<HttpPostedFileBase> files = null)
        {
            using (var ts = AsyncTransactionScope.StartNew())
            {
                if (model.group == null)
                {
                    return "Submit is disabled!";
                }

                TaskData taskData = TaskDataService.FindById(model.TaskData.TaskId);

                if (taskData == null)
                {
                    return IndicatingMessages.TaskNotFound;
                }
                USER_TYPE userType = model.UserType > 0 ? model.UserType : this.UserContext.UserType;
                var orders = OrderService.FindOrderByTaskId(taskData.TaskId);
                Order order = null;
                NCReport ncr = null;
                List<Byte[]> AttachedFileBytes = new List<byte[]>();
                List<string> InvoicesNumbers = new List<string>();
                if (orders != null && orders.Count > 0)
                {
                    order = orders.Last();
                }

                List<int> vendorIds = taskData.ProductId != null ? RfqBidService.FindRFQBidListByProductId(taskData.ProductId.Value).Select(x => x.VendorId).ToList() : new List<int>();

                var trigger = (Triggers)Enum.Parse(typeof(Triggers), model.group);
                States newState = States.CreateRFQ;
                try
                {
                    newState = GetNextState(taskData, trigger);
                }
                catch (Exception ex)
                {
                    return ex.RetrieveErrorMessage();
                }

                if (newState == States.BackFromRFQ || newState == States.PendingRFQRevision)
                {
                    taskData.StateId = (int)newState;
                    TaskDataService.Update(taskData);
                    string res = string.Empty;
                    if (taskData.isEnterprise == true)
                    {
                        res = DoRFQRevision(taskData, taskData.RevisingReason, userType, files);
                    }
                    else
                    {
                        res = DoRFQRevision_Reseller(taskData, taskData.RevisingReason, userType, files);
                    }

                    if (res != null)
                    {
                        return res;
                    }
                }
                else if (newState == States.RFQRevision)
                {
                    List<Document> docs = new List<Document>();
                    try
                    {
                        UploadRevisionDocuments(taskData, userType, files, ref docs);
                    }
                    catch (Exception ex)
                    {
                        return $"State at : {Enum.GetName(typeof(States), taskData.StateId)}, Error: {ex.RetrieveErrorMessage()}";
                    }
                    var tasks = TaskDataService.FindTaskDataListByProductId(taskData.ProductId.Value)
                       .Where(x => x.RFQBidId != null && x.StateId != (int)States.RFQBidComplete);
                    foreach (var td in tasks)
                    {
                        td.StateId = (int)newState;
                        td.RevisingReason = taskData.RevisingReason;
                        td.UpdatedBy = UserContext.User.UserName;
                        td.ModifiedUtc = DateTime.UtcNow;
                        td.ModifiedByUserId = UserContext.UserId;
                        TaskDataService.Update(td);
                    }

                    // Retrieve most recent Bid RFQ Revision from [BidRequestRevisions]
                    var revision = BidRequestRevisionService.FindBidRequestRevisionListByProductId(taskData.ProductId.Value).LastOrDefault();
                    if (revision != null)
                    {
                        revision.Documents = docs;
                        BidRequestRevisionService.UpdateBidRequestRevision(revision);
                    }
                }
                else if (newState == States.ProofRejected)
                {
                    if (model.TaskData.RejectReason != null)
                    {
                        taskData.RejectReason = model.TaskData.RejectReason;
                    }

                    string fname = $"Proof_Reject_Reason_oid-{order.Id}_pid-{taskData.ProductId}";
                    CreateDocument(files, taskData, fname, DOCUMENT_TYPE.CORRESPOND_PROOF_REJECT_PDF, (int)USER_TYPE.Customer);
                }
                else if (newState == States.ProofApproved)
                {
                    // set Proofing Doc to be locked
                    var docs = TaskSetup.GetDocumentsByProductIdDocType(taskData.ProductId.Value, DOCUMENT_TYPE.PROOF_PDF);
                    foreach (var doc in docs)
                    {
                        doc.IsLocked = true;
                    }
                }
                else if (newState == States.SampleRejected)
                {
                    if (model.TaskData.RejectReason != null)
                    {
                        taskData.RejectReason = model.TaskData.RejectReason;
                    }

                    string fname = $"Sample_Reject_Reason_oid-{order.Id}_pid-{taskData.ProductId}";
                    CreateDocument(files, taskData, fname, DOCUMENT_TYPE.CORRESPOND_SAMPLE_REJECT_PDF, (int)USER_TYPE.Customer);
                }
                else if (newState == States.BidForRFQ)
                {
                    // in case to prevent this state transits from NCRCustomerRevisionNeeded -> BackFromRFQ -> BidForRFQ
                    // in the case from NCR, vendor already selectec, therefore, VendorId != null
                    //
                    if (taskData.Product.VendorId == null)
                    {
                        var tasks = TaskDataService.FindTaskDataListByProductId(taskData.ProductId.Value);
                        foreach (var task in tasks)
                        {
                            task.StateId = (int)newState;
                            if (taskData.RevisingReason != null)
                            {
                                task.RevisingReason = taskData.RevisingReason;
                                taskData.UpdatedBy = UserContext.User.UserName;
                                taskData.ModifiedUtc = DateTime.UtcNow;
                                taskData.ModifiedByUserId = UserContext.UserId;
                            }
                            try
                            {
                                TaskDataService.Update(task);
                            }
                            catch (Exception ex)
                            {
                                return $"State at : {Enum.GetName(typeof(States), newState)}, Tried to upload same document twice : {ex.RetrieveErrorMessage()}";
                            }
                        }
                    }
                }
                else if (newState == States.BidReview)
                {
                    int rfqBidId = 0;
                    if (model.RFQVM != null)
                    {
                        try
                        {
                            model.RFQVM.TaskId = taskData.TaskId;
                            model.RFQVM.isEnterprise = taskData.isEnterprise;

                            rfqBidId = await RFQBidReview(model.RFQVM, files);
                            if (rfqBidId < 0)
                            {
                                ts.Complete();
                                return null;
                            }
                        }
                        catch (Exception ex)
                        {
                            return $"State at : {Enum.GetName(typeof(States), newState)}, Error: {ex.RetrieveErrorMessage()}";
                        }
                    }
                }
                else if (newState == States.SetupMarkupExtraQty)
                {
                    if ((Triggers)trigger == Triggers.SetupUnitPricesForExtraQuantities && model.RFQVM != null)
                    {
                        model.RFQVM.TaskId = taskData.TaskId;
                        model.RFQVM.isEnterprise = taskData.isEnterprise;
                        try
                        {
                            SetupExtraQuantity(model.RFQVM, files);
                            if (taskData.isEnterprise)
                            {


                                // Sending notification to customer
                                var uType = UserContext.UserType;
                                var uIds = new List<SimplifiedUser>();

                                uIds.AddRange(taskData.Product.CustomerCompany.Users.Where(u => u.Active == true));
                                foreach (var user in uIds)
                                {
                                    var destination = user.Email;
                                    var destinationSms = user.PhoneNumber;
                                    try
                                    {
                                        taskData.StateId = (int)newState;
                                        await SendNotifications(taskData, destination, destinationSms);
                                    }
                                    catch (Exception ex)
                                    {
                                        string errorMsg = ex.RetrieveErrorMessage();
                                        if (errorMsg.Equals(IndicatingMessages.SmsWarningMsg) || errorMsg.Equals(IndicatingMessages.NoCellPhoneWarningMsg))
                                        {
                                            continue;
                                        }
                                        throw;
                                    }
                                }
                                newState = States.QuoteAccepted;
                            }
                        }
                        catch (Exception ex)
                        {
                            return ex.RetrieveErrorMessage();
                        }
                    }
                }
                else if (newState == States.PendingRFQRevision && model.TaskData != null)
                {
                    taskData.RevisingReason = model.TaskData.RevisingReason;
                    taskData.RejectReason = model.TaskData.RejectReason;
                }
                else if ((Triggers)trigger == Triggers.CompleteInvoiceForVendor && newState == States.ProductionComplete && model.VendorInvoiceVM != null)
                {
                    VendorInvoiceViewModel vm = model.VendorInvoiceVM;
                    if (vm == null)
                    {
                        return $"State at : {Enum.GetName(typeof(States), newState)}, Error: VendorInvoiceViewModel is null!";
                    }
                    if (orders == null || orders.Count == 0)
                    {
                        return "No Order can be found for this Task";
                    }

                    if (taskData.isEnterprise)
                    {
                        if (vm.AttachInvoiceForTooling != null)
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                vm.AttachInvoiceForTooling.InputStream.CopyTo(ms);
                                AttachedFileBytes.Add(ms.ToArray());
                                vm.AttachInvoiceForTooling.InputStream.Position = 0;
                                InvoicesNumbers.Add(vm.VendorAttachedInviceNumberForTooling);
                            }
                        }
                        if (vm.AttachInvoice != null)
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                vm.AttachInvoice.InputStream.CopyTo(ms);
                                AttachedFileBytes.Add(ms.ToArray());
                                vm.AttachInvoice.InputStream.Position = 0;
                                InvoicesNumbers.Add(vm.VendorAttachedInvoiceNumber);
                            }
                        }

                        string fname = $"vendor_invoice_vid-{ taskData.Product.VendorId}_tid-{taskData.TaskId}_pid-{taskData.ProductId}_orderid-{order.Id}";
                        try
                        {
                            CreateDocument(files, taskData, fname, DOCUMENT_TYPE.ENTERPRISE_VENDOR_INVOICE_PDF, (int)USER_TYPE.Vendor);
                        }
                        catch (Exception ex)
                        {
                            return ex.RetrieveErrorMessage();
                        }
                    }
                    else
                    {
                        foreach (var ord in orders)
                        {
                            try
                            {
                                await CallQBOCreateVendorInvoice(taskData, ord, vm);
                            }
                            catch (Exception ex)
                            {
                                return $"State at : {Enum.GetName(typeof(States), newState)}, {ex.RetrieveErrorMessage()}";
                            }
                        }
                    }
                }
                else if ((newState == States.SampleComplete || newState == States.ProductionComplete))
                {
                    // Handle Uploaded Outgoing Inspection Report
                    string fname = string.Empty;
                    if (newState == States.SampleComplete)
                    {
                        fname = $"inspect_report_tid-{taskData.TaskId}_pid-{taskData.ProductId}_oid-{order.Id}_sample";
                    }
                    else
                    {
                        fname = $"inspect_report_tid-{taskData.TaskId}_pid-{taskData.ProductId}_oid-{order.Id}_product";
                    }
                    try
                    {
                        CreateDocument(files, taskData, fname, DOCUMENT_TYPE.PACKING_SLIP_INSPECTION_REPORT_PDF, (int)USER_TYPE.Vendor);
                    }
                    catch (Exception ex)
                    {
                        return ex.RetrieveErrorMessage();
                    }

                    if (orders == null || orders.Count == 0)
                    {
                        return IndicatingMessages.OrderNotFound;
                    }
                    Order ord = null;
                    var pb = PriceBreakService.FindPriceBreakByTaskId(taskData.TaskId).FirstOrDefault();
                    if (pb == null)
                    {
                        pb = PriceBreakService.FindPriceBreakByProductId(taskData.ProductId.Value).FirstOrDefault();
                        if (pb == null)
                        {
                            return IndicatingMessages.PriceBreaksNotFound;
                        }
                    }
                    var numberSampleIncluded = (pb.NumberSampleIncluded != null && pb.NumberSampleIncluded > 0) ? pb.NumberSampleIncluded.Value : 1;
                    if (newState == States.SampleComplete)
                    {
                        ord = orders?.Where(x => x.Quantity == numberSampleIncluded || x.Quantity == 1 || x.Quantity == 0)?.LastOrDefault();
                    }
                    else
                    {
                        ord = orders?.Where(x => x.Quantity > numberSampleIncluded || x.IsForToolingOnly == true).LastOrDefault();
                    }
                    if (ord == null)
                    {
                        ord = orders.LastOrDefault();
                        if (ord == null)
                        {
                            return IndicatingMessages.OrderNotFound;
                        }
                    }

                    // For resellers only
                    if (model.RequireShippingInfo == true)
                    {
                        ord.TrackingNumber = model.TrackingNumber.Contains(" ") ? model.TrackingNumber.Replace(" ", string.Empty) : model.TrackingNumber;
                        if (!string.IsNullOrEmpty(model.CarrierName))
                        {
                            ord.CarrierName = model.CarrierName;
                            ord.CarrierType = model.CarrierType;
                            ord.ShippingAccountNumber = model.ShippingAccountNumber;
                        }
                        ord.ShippedDate = DateTime.UtcNow;
                        OrderService.UpdateOrder(ord);
                    }

                    // For resellers only
                    if (taskData.isEnterprise == false &&
                        taskData.Product.CustomerCompany.Term != null &&
                        taskData.Product.CustomerCompany.Term > 0 &&
                        string.IsNullOrEmpty(taskData.RejectReason))
                    {
                        try
                        {
                            var tuple = await CallQBOCreateCustomerInvoice(taskData, ord, numberSampleIncluded);
                            if (tuple != null)
                            {
                                AttachedFileBytes.Add(tuple.Item1);
                                InvoicesNumbers.Add(tuple.Item2);
                            }
                        }
                        catch (Exception ex)
                        {
                            return $"State at : {Enum.GetName(typeof(States), newState)}, Error: {ex.RetrieveErrorMessage()}";
                        }
                    }
                    // TODO: will be changed below back to: 
                    // if (newState == States.ProductionComplete) 
                    if (newState == States.ProductionComplete && taskData.isEnterprise == false)
                    {
                        newState = States.VendorPendingInvoice;
                    }
                }
                else if (newState == States.SampleApproved && orders.First().IsForToolingOnly == true)
                {
                    // TODO: will be changed below back to: 
                    // newState = States.VendorPendingInvoice;
                    if (taskData.isEnterprise == true)
                    {
                        newState = States.ProductionComplete;
                    }
                    else
                    {
                        newState = States.VendorPendingInvoice;
                    }
                }
                else if (newState == States.SampleStarted || newState == States.ProductionStarted)
                {
                    if (orders == null || orders.Count == 0)
                    {
                        return IndicatingMessages.OrderNotFound;
                    }
                    if (model.EstimateCompletionDate != null)
                    {
                        Order ord = null;
                        if (newState == States.SampleStarted)
                        {
                            ord = orders.First();
                        }
                        else
                        {
                            ord = orders.Last();
                        }
                        ord.EstimateCompletionDate = model.EstimateCompletionDate;
                        OrderService.UpdateOrder(ord);
                    }

                }
                else if (newState == States.CustomerCancelOrder)
                {
                    order.TaskData.TaskStateBeforeCustomerCancelOrder = order.TaskData.StateId;
                    order.CancelOrderReason = model.CustomerCancelOrderReason;
                    OrderService.UpdateOrder(order);
                }
                else if (newState == States.OrderCancelDenied)
                {
                    newState = (States)taskData.TaskStateBeforeCustomerCancelOrder;
                    order.DenyCancelOrderReason = model.DenyCancelOrderReason;
                    OrderService.UpdateOrder(order);
                }
                else if (newState == States.OrderCancelled)
                {
                    var ords = OrderService.FindOrdersByProductId(order.ProductId);
                    foreach (var ord in ords)
                    {
                        ord.TaskData.StateId = (int)States.OrderCancelled;
                        TaskDataService.Update(ord.TaskData);
                        ord.IsOrderCancelled = true;
                        ord.DenyCancelOrderReason = null;
                        OrderService.UpdateOrder(ord);
                    }
                }

                // Update PartRevision table with new state
                var product = taskData.Product;
                if (product.PartRevisionId != null)
                {
                    var partRevision = product?.PartRevision;
                    if (partRevision != null)
                    {
                        partRevision.StateId = newState;
                        PartRevisionService.UpdatePartRevision(partRevision);
                    }
                }

                // NCR
                if (newState == States.NCRCustomerApproval
                    || newState == States.NCRCustomerRevisionNeeded
                    || newState == States.NCRDamagedByCustomer
                    || newState == States.NCRVendorRootCauseAnalysis)
                {
                    ncr = NcReportService.FindNCReportByTaskId(taskData.TaskId);
                    if (ncr == null)
                    {
                        // retrieve  NCReport table based on model.ProductId, order id and set to entity
                        var ncrList = NcReportService.FindNCReportsByProductId(product.Id).Where(x => x.StateId != States.NCRClosed);
                        if (ncrList == null || !ncrList.Any())
                        {
                            return $"No Non-Conformance Report being found for product id = {product.Id} and order id = {order.Id}";
                        }
                        ncr = ncrList.Last();
                    }
                    order = ncr.Order;
                    ncr.StateId = newState;
                    if (model.Disposition != null)
                    {
                        ncr.Disposition = model.Disposition;
                    }

                    if (newState == States.NCRCustomerRevisionNeeded || newState == States.NCRDamagedByCustomer)
                    {
                        ncr.RootCause = NC_ROOT_CAUSE.CUSTOMER;
                        ncr.NCRootCauseCompanyId = product.CustomerId;
                        if (order.ProductSharingId != null)
                        {
                            // Change the taskData in ProductSharing table to current taskData to reflect current state
                            order.ProductSharing.TaskId = taskData.TaskId;
                            order.ProductSharing.ModifiedByUserId = UserContext.UserId;
                            order.ProductSharing.ModifiedUtc = DateTime.UtcNow;
                            productSharingService.UpdateProductSharing(order.ProductSharing);
                        }
                    }
                    else
                    {
                        ncr.RootCause = NC_ROOT_CAUSE.VENDOR;
                        ncr.NCRootCauseCompanyId = product.VendorId;
                    }
                    ncr.VendorId = product.VendorId;
                    if (model.DetailRootCause != null)
                    {
                        ncr.RootCauseFurtherDetails = model.DetailRootCause;
                    }
                    if (model.ActionTaken != null)
                    {
                        ncr.ActionTakenDetails = model.ActionTaken;
                    }
                    if (model.CorrectiveAction != null)
                    {
                        ncr.CorrectiveAction = model.CorrectiveAction;
                    }
                    if (model.ActionTakenBy != null)
                    {
                        ncr.ActionTakenVerifiedBy = model.ActionTakenBy;
                    }

                    for (int i = 0; i < files.Count; i++)
                    {
                        string fileName = string.Empty;
                        string imageUrl = string.Empty;
                        string ext = Path.GetExtension(files[i].FileName);

                        NCR_IMAGE_TYPE type = NCR_IMAGE_TYPE.VENDOR_CAUSE_REF;
                        if (newState == States.NCRVendorRootCauseAnalysis)
                        {
                            if ((Triggers)trigger == Triggers.NCRArbitrateVendorCause)
                            {
                                fileName = $"ncr_arbitrate_to_vendor_cause_image_tid-{taskData.TaskId}_pid-{product.Id}-v{i + 1}{ext}";
                                type = NCR_IMAGE_TYPE.ARBITRATE_VENDOR_CAUSE_REF;
                            }
                            else if ((Triggers)trigger == Triggers.NCRArbitrateCustomerCause)
                            {
                                fileName = $"ncr_arbitrate_customer_cause_image_tid-{taskData.TaskId}_pid-{product.Id}-v{i + 1}{ext}";
                                type = NCR_IMAGE_TYPE.ARBITRATE_CUSTOMER_CAUSE_REF;
                            }
                        }
                        else if (newState == States.NCRDamagedByCustomer)
                        {
                            fileName = $"ncr_arbitrate_customer_damage_image_tid-{taskData.TaskId}_pid-{product.Id}-v{i + 1}{ext}";
                            type = NCR_IMAGE_TYPE.ARBITRATE_CUSTOMER_DAMAGE_REF;
                        }
                        else if (newState == States.NCRCustomerRevisionNeeded)
                        {
                            fileName = $"ncr_arbitrate_customer_revision_needed_image_tid-{taskData.TaskId}_pid-{product.Id}-v{i + 1}{ext}";
                            type = NCR_IMAGE_TYPE.ARBITRATE_CUSTOMER_CAUSE_REF;
                        }
                        else
                        {
                            fileName = $"ncr_customer_cause_image_tid-{taskData.TaskId}_pid-{product.Id}-v{i + 1}{ext}";
                            type = NCR_IMAGE_TYPE.CUSTOMER_CAUSE_REF;
                        }

                        imageUrl = DocumentStorageService.Upload(files[i], fileName);
                        NCRImages entity = new NCRImages
                        {
                            NCReportId = ncr.Id,
                            ImageUrl = imageUrl,
                            Type = (int)type,
                        };
                        NCRImagesService.AddNCRImages(entity);
                    }


                    try
                    {
                        NcReportService.UpdateNCReport(ncr);
                    }
                    catch (Exception ex)
                    {
                        return ex.RetrieveErrorMessage();
                    }
                }
                else if (newState > States.NCRVendorRootCauseAnalysis && newState <= States.NCRClosed ||
                         newState == States.NCRCustomerCorrectivePartsAccepted ||
                         newState == States.NCRDamagedByCustomer ||
                         (newState == States.RFQRevision && (Triggers)trigger == Triggers.NCRCustomerRevision))
                {
                    ncr = NcReportService.FindNCReportByTaskId(taskData.TaskId);
                    if (ncr == null)
                    {
                        var ncrList = NcReportService.FindNCReportsByProductId(product.Id).Where(x => x.StateId != States.NCRClosed);
                        if (!ncrList.Any())
                        {
                            return $"No Non-Conformance Report being found for product id = {product.Id}";
                        }
                        ncr = ncrList.Last();
                    }
                    order = ncr.Order;
                    string prefix = "ncr_rootcause_on_customer";


                    for (int i = 0; i < files.Count; i++)
                    {
                        if (files[i].ContentLength == 0)
                        {
                            continue;
                        }

                        NCR_IMAGE_TYPE type = NCR_IMAGE_TYPE.ROOT_CAUSE_ON_CUSTOMER;
                        if ((Triggers)trigger == Triggers.NCRArbitrateCustomerCauseDamage)
                        {
                            type = NCR_IMAGE_TYPE.ARBITRATE_CUSTOMER_DAMAGE_REF;
                        }
                        if (newState == States.NCRVendorCorrectivePartsComplete)
                        {
                            prefix = "ncr_inspection_corrective_part";
                            type = NCR_IMAGE_TYPE.PACKING_SLIP_INSPECTION_REPORT_PDF;
                        }
                        var docs = NCRImagesService.FindNCRImagesByNCReportId(ncr.Id).Where(d => d.Type == (int)type);
                        string ext = Path.GetExtension(files[i].FileName);
                        string fileNewName = $"{prefix}_tid-{taskData.TaskId}_pid-{product.Id}-v{docs.Count() + i + 1}{ext}";

                        try
                        {
                            var url = DocumentStorageService.Upload(files[i], fileNewName);
                            NCRImages entity = new NCRImages
                            {
                                NCReportId = ncr.Id,
                                ImageUrl = url,
                                Type = (int)type,
                            };
                            NCRImagesService.AddNCRImages(entity);
                        }
                        catch (Exception ex)
                        {
                            return ex.RetrieveErrorMessage();
                        }
                    }

                    if (!string.IsNullOrEmpty(model.NcrDescriptionVM?.TrackingNumber))
                    {
                        order.TrackingNumber = model.NcrDescriptionVM.TrackingNumber.Contains(" ") ?
                                                model.NcrDescriptionVM.TrackingNumber.Replace(" ", string.Empty) :
                                                model.NcrDescriptionVM.TrackingNumber;
                        if (!string.IsNullOrEmpty(model.NcrDescriptionVM.CarrierName))
                        {
                            order.CarrierName = model.NcrDescriptionVM.CarrierName;
                            order.CarrierType = model.CarrierType;
                            order.ShippingAccountNumber = model.ShippingAccountNumber;
                        }
                        order.ShippedDate = DateTime.UtcNow;
                        OrderService.UpdateOrder(order);

                        ncr.CarrierName = string.IsNullOrEmpty(model.NcrDescriptionVM.CarrierName) ? order.CarrierName : model.NcrDescriptionVM.CarrierName;
                        ncr.TrackingNumber = model.NcrDescriptionVM.TrackingNumber;
                    }
                    ncr.StateId = newState;
                    if (newState == States.NCRClosed)
                    {
                        if ((Triggers)trigger == Triggers.NCRCustomerRevision)
                        {
                            var originTaskData = TaskDataService.FindTaskDataListByProductId(product.Id)
                                        .FirstOrDefault(x => x.StateId == (int)States.ProductionComplete);

                            var newTask = new TaskData
                            {
                                ProductId = product.Id,
                                StateId = (int)States.PendingRFQRevision,
                                isEnterprise = originTaskData.isEnterprise,
                                IsRiskBuild = originTaskData.IsRiskBuild,
                                isTagged = originTaskData.isTagged,
                                CreatedUtc = DateTime.UtcNow,
                                CreatedByUserId = UserContext.UserId,
                                ModifiedUtc = DateTime.UtcNow,
                                ModifiedByUserId = UserContext.UserId,
                                UpdatedBy = UserContext.User.UserName,

                            };
                            TaskDataService.AddTaskData(newTask);
                        }
                        ncr.DateNcrClosed = DateTime.UtcNow;
                    }

                    try
                    {
                        NcReportService.UpdateNCReport(ncr);
                    }
                    catch (Exception ex)
                    {
                        return ex.RetrieveErrorMessage();
                    }
                }
                else if (newState == States.PaymentMade || newState == States.ReOrderPaymentMade)
                {
                    var fileNameWithoutExtention = $"payment_proof_oid-{order.Id}";
                    documentBL.UploadGeneralFiles(files, fileNameWithoutExtention, DOCUMENT_TYPE.PAYMENT_PROOF, taskData, (int)USER_TYPE.Customer);
                }
                else if (newState == States.ProofApproved || newState == States.SampleApproved)
                {
                    // clear the RejectReason column caused in Proof Approving, 
                    // because coming Sample Approving also share this slot. 
                    taskData.RejectReason = null;
                    if (taskData.IsRiskBuild == true && newState == States.ProofApproved)
                    {
                        newState = States.SampleApproved;
                    }
                }
                else if (newState == States.ReviewRFQAccepted || newState == States.VendorRejectedRFQ)
                {
                    taskData.StateId = (int)newState;
                    taskData.UpdatedBy = UserContext.User.UserName;
                    taskData.ModifiedUtc = DateTime.UtcNow;
                    taskData.ModifiedByUserId = UserContext.UserId;
                    TaskDataService.Update(taskData);

                    var productId = taskData.ProductId.Value;
                    var vendorId = UserContext.Company.Id;
                    int? vendorRejectRFQId = null;
                    if (newState == States.VendorRejectedRFQ)
                    {
                        vendorRejectRFQId = rfqActionReasonService.FindRFQActionReasonListByProductIdVendorId(productId, vendorId, REASON_TYPE.REJECT_RFQ)?.Id;
                    }

                    var originTask = TaskDataService.FindTaskDataListByProductId(productId)
                            .Where(x => x.RFQBidId == null)
                            .FirstOrDefault();

                    if (originTask == null)
                    {
                        return IndicatingMessages.TaskNotFound;
                    }

                    // Now store to VendorBidRFQStatus table
                    var bidRFQStatus = bidRFQStatusService.FindBidRFQStatusListByProductId(productId).LastOrDefault();
                    var vendorBidRFQStatus = new VendorBidRFQStatus
                    {
                        ProductId = productId,
                        TaskId = taskData.TaskId,
                        VendorId = vendorId,
                        StateId = (int)newState,
                        CreatedByUserId = UserContext.UserId,
                        BidRFQStatusId = bidRFQStatus?.Id,
                        RFQActionReasonId = vendorRejectRFQId,
                    };
                    var vendorBidRFQStatusId = vendorBidRFQStatusService.AddVendorBidRFQStatus(vendorBidRFQStatus);
                    if (TaskDatasBL.IsAllVendorsResponsed(taskData.ProductId.Value, vendorIds.Count))
                    {
                        var vendors = TaskDataService.FindTaskDataListByProductId(productId)
                            .Where(x => x.RFQBidId != null && !(x.StateId == (int)States.VendorRejectedRFQ ||
                                        x.StateId == (int)States.RFQBidComplete ||
                                        x.StateId == (int)States.BidTimeout));

                        var anyRevisionRequest = vendors.Count(x => x.StateId == (int)States.PendingRFQRevision);
                        if (anyRevisionRequest > 0)
                        {
                            newState = States.PendingRFQRevision;
                        }
                        else if (newState == States.VendorRejectedRFQ)
                        {
                            var anyRFQReviewAccepted = vendors.Count(x => x.StateId == (int)States.ReviewRFQAccepted);
                            var anyBidReview = vendors.Count(x => x.StateId == (int)States.BidReview);
                            if (anyRFQReviewAccepted > 0)
                            {
                                newState = States.BidForRFQ;
                            }
                            else if (anyBidReview > 0)
                            {
                                newState = States.BidReview;
                            }
                            else
                            {
                                newState = States.RFQFailed;
                            }
                        }
                        else
                        {
                            newState = States.BidForRFQ;
                        }
                        if (newState == States.BidForRFQ)
                        {
                            // Start Bid Timer here
                            var interval = TimerSetupService.FindAllTimerSetupsByProductIdTimerType(productId, TypeOfTimers.BidTimer)
                                .ToList()
                                .LastOrDefault()?.Interval;
                            if (string.IsNullOrEmpty(interval))
                                return IndicatingMessages.TimerIntervalCouldnotBeFound;

                            var error = TaskDatasBL.SetupTimer(productId, interval, TypeOfTimers.BidTimer);
                            if (error != null)
                                return error;
                        }

                        foreach (var td in vendors)
                        {
                            vendorBidRFQStatus = new VendorBidRFQStatus
                            {
                                ProductId = productId,
                                TaskId = td.TaskId,
                                VendorId = td.RFQBid.VendorId,
                                StateId = (int)newState,
                                CreatedByUserId = UserContext.UserId,
                                BidRFQStatusId = bidRFQStatus?.Id,
                                RFQActionReasonId = vendorRejectRFQId,
                            };
                            vendorBidRFQStatusId = vendorBidRFQStatusService.AddVendorBidRFQStatus(vendorBidRFQStatus);

                            if (td.TaskId == taskData.TaskId) continue;
                            td.StateId = (int)newState;
                            td.UpdatedBy = UserContext.User.UserName;
                            td.ModifiedByUserId = UserContext.UserId;
                            td.ModifiedUtc = DateTime.UtcNow;
                            TaskDataService.Update(td);
                        }

                        // Update customer TaskData state to PendingRFQRevision = 40
                        originTask.StateId = (int)newState;
                        originTask.UpdatedBy = UserContext.User.UserName;
                        originTask.ModifiedUtc = DateTime.UtcNow;
                        originTask.ModifiedByUserId = UserContext.UserId;
                        TaskDataService.Update(originTask);

                        // Update stateid in [BidRFQStatus] table
                        bidRFQStatus.StateId = (int)newState;
                        bidRFQStatus.SubmittedVendors = vendors.Count();
                        bidRFQStatus.ModifiedByUserId = UserContext.UserId;
                        bidRFQStatusService.UpdateBidRFQStatus(bidRFQStatus);
                    }
                }

                // Update TaskData table
                if (!(taskData.StateId == (int)States.VendorRejectedRFQ || newState == States.ReviewRFQAccepted))
                {
                    taskData.StateId = (int)newState;
                    taskData.UpdatedBy = UserContext.User.UserName;
                    taskData.ModifiedUtc = DateTime.UtcNow;
                    taskData.ModifiedByUserId = UserContext.UserId;

                    try
                    {
                        TaskDataService.Update(taskData);
                    }
                    catch (Exception ex)
                    {
                        return ex.RetrieveErrorMessage();
                    }
                }


                // Add to OrderStateTracking table with new state and time stamp
                OrderStateTracking orderState = new OrderStateTracking()
                {
                    OrderId = order != null ? order.Id : ncr?.OrderId,
                    StateId = taskData.StateId,
                    UpdatedBy = taskData.UpdatedBy,
                    ModifiedUtc = taskData.ModifiedUtc,
                    NcrId = ncr?.Id,
                };

                OrderStateTrackingService.AddOrderStateTracking(orderState);


                // Fill in ProductStateTracking table with product id, state id ...
                var productState = new ProductStateTracking()
                {
                    ProductId = taskData.ProductId.Value,
                    StateId = taskData.StateId,
                    UpdatedBy = taskData.UpdatedBy,
                    ModifiedUtc = taskData.ModifiedUtc,
                    NcrId = ncr?.Id,
                    OrderId = ncr?.OrderId,
                };

                ProductStateTrackingService.AddProductStateTracking(productState);

                if (trigger == Triggers.VendorAcceptCancelOrderRequest ||
                    trigger == Triggers.VendorDenyCancelOrderRequest ||
                    trigger == Triggers.CustomerCancelOrderRequest)
                {
                    ts.Complete();
                    return null;
                }

                // Notify              

                if (taskData.StateId == (int)States.OrderPaid || taskData.StateId == (int)States.ReOrderPaid)
                {
                    string subject = taskData.StateId == (int)States.ReOrderPaid ? "Omnae.com Re-Order Confirmation {0}, {1}" : "Omnae.com Order Confirmation {0}, {1}";
                    try
                    {
                        PaymentBl.NotifyOnPayment(order, (States)taskData.StateId, subject);
                    }
                    catch (Exception ex)
                    {
                        var msg = ex.RetrieveErrorMessage();
                        if (!msg.Equals(IndicatingMessages.SmsWarningMsg) && !msg.Equals(IndicatingMessages.NoCellPhoneWarningMsg))
                        {
                            return msg;
                        }
                    }
                    ts.Complete();
                    return null;
                }

                // Disable notifications state changes during purchasing order
                if (taskData.isEnterprise == true && !(taskData.IsRFQs() || taskData.IsNcr()))
                {
                    ts.Complete();
                    return null;
                }

                if (taskData.StateId == (int)States.BidReview)
                {
                    //var vendorId = taskData.RFQBidId != null ? taskData.RFQBid?.VendorId : taskData.Product.VendorId.Value;
                    var vendorId = taskData.RFQBid?.VendorId ?? (taskData.Product.VendorId != null ? taskData.Product.VendorId.Value : (int?)null);
                    if (vendorId != null)
                    {
                        var userContacts = UserContactService.GetAllActiveUserConnectFromCompany(vendorId.Value);
                        foreach (var contactInformation in userContacts)
                        {
                            var destination = contactInformation.Email;
                            var destinationSms = contactInformation.PhoneNumber;
                            try
                            {
                                await SendNotifications(taskData, destination, destinationSms);
                            }
                            catch (Exception ex)
                            {
                                var msg = ex.RetrieveErrorMessage();
                                if (msg.Equals(IndicatingMessages.SmsWarningMsg) || msg.Equals(IndicatingMessages.NoCellPhoneWarningMsg))
                                {
                                    continue;
                                }
                                return msg;
                            }
                        }


                        // Notify Admin or notify customer who starts bid process, if in enterprise mode
                        var pid = taskData.ProductId.Value;
                        if (TaskDatasBL.IsAllVendorsResponsed(pid, vendorIds.Count))
                        {

                            var destination = ConfigurationManager.AppSettings["AdminEmail"];
                            var destinationSms = ConfigurationManager.AppSettings["AdminPhone"];
                            if (taskData.isEnterprise && taskData.Product != null)
                            {
                                userContacts = UserContactService.GetAllActiveUserConnectFromCompany(taskData.Product.CustomerId.Value);
                                foreach (var contactInformation in userContacts)
                                {
                                    destination = contactInformation.Email;
                                    destinationSms = contactInformation.PhoneNumber;
                                    try
                                    {
                                        await SendNotifications(taskData, destination, destinationSms);
                                    }
                                    catch (Exception ex)
                                    {
                                        var msg = ex.RetrieveErrorMessage();
                                        if (msg.Equals(IndicatingMessages.SmsWarningMsg) || msg.Equals(IndicatingMessages.NoCellPhoneWarningMsg))
                                        {
                                            continue;
                                        }
                                        throw;
                                    }
                                }
                            }
                            else
                            {
                                try
                                {
                                    await SendNotifications(taskData, destination, destinationSms, true);
                                }
                                catch (Exception ex)
                                {
                                    return ex.RetrieveErrorMessage();
                                }
                            }
                        }
                    }
                    ts.Complete();
                    return null;
                }

                if (taskData.StateId == (int)States.PaymentMade || taskData.StateId == (int)States.ReOrderPaymentMade)
                {
                    if (taskData.isEnterprise == false)
                    {
                        var destination = ConfigurationManager.AppSettings["AdminEmail"];
                        var destinationSms = ConfigurationManager.AppSettings["AdminPhone"];
                        try
                        {
                            await SendNotifications(taskData, destination, destinationSms, true);
                        }
                        catch (Exception ex)
                        {
                            return ex.RetrieveErrorMessage();
                        }
                        ts.Complete();
                        return null;
                    }
                }

                if (taskData.StateId == (int)States.PendingRFQRevision)
                {
                    ts.Complete();
                    // Skip Notification
                    return null;
                }

                var userIds = new List<SimplifiedUser>();
                if (userType == USER_TYPE.Admin || userType == USER_TYPE.Customer)
                {
                    if (taskData.Product.VendorCompany != null)
                    {
                        userIds.AddRange(taskData.Product.VendorCompany.Users);
                    }
                    else if (taskData.RFQBid != null)
                    {
                        var vendorId = taskData.RFQBid.VendorId;
                        var company = CompanyService.FindCompanyById(vendorId);
                        userIds.AddRange(company.Users);
                    }
                    else
                    {
                        var vendorTasks = TaskDataService.FindTaskDataListByProductId(taskData.ProductId.Value).Where(x => x.RFQBid != null);
                        foreach (var task in vendorTasks)
                        {
                            userIds.AddRange(task.RFQBid.VendorCompany.Users);
                        }
                    }
                    if ((Triggers)trigger == Triggers.NCRArbitrateVendorCause ||
                        (Triggers)trigger == Triggers.NCRArbitrateCustomerCauseDamage ||
                        (Triggers)trigger == Triggers.NCRArbitrateCustomerCause)
                    {
                        // Notify to customer too
                        if ((Triggers)trigger == Triggers.NCRArbitrateCustomerCause && order?.ProductSharingId != null)
                        {
                            // Notify sharer too if this NCR was created by sharee
                            userIds.AddRange(order.ProductSharing.ProductOwnerCompany.Users);
                        }
                        userIds.AddRange(ncr.CustomerCompany.Users);
                    }
                }
                else if (userType == USER_TYPE.Vendor && taskData.StateId == (int)States.NCRAdminDisputesIntervention)
                {
                    // Notify to both customer and vendor
                    userIds.AddRange(ncr.CustomerCompany.Users);
                    userIds.AddRange(taskData.Product.VendorCompany.Users);
                }
                else
                {
                    if (order?.ProductSharingId != null)
                    {
                        userIds.AddRange(order.ProductSharing.ProductSharingCompany.Users);
                    }
                    else
                    {
                        userIds.AddRange(taskData.Product.CustomerCompany.Users);
                    }
                }

                foreach (var user in userIds.Where(u => u.Active == true))
                {
                    var destination = user.Email;
                    var destinationSms = user.PhoneNumber;
                    try
                    {
                        if (AttachedFileBytes != null && AttachedFileBytes.Any())
                        {
                            await SendNotifications(taskData, destination, destinationSms, false, AttachedFileBytes, InvoicesNumbers);
                        }
                        else if (taskData.isEnterprise == true || taskData.StateId != (int)States.VendorPendingInvoice && taskData.StateId != (int)States.ProductionComplete)
                        {
                            if ((Triggers)trigger == Triggers.NCRArbitrateVendorCause ||
                                 (Triggers)trigger == Triggers.NCRArbitrateCustomerCauseDamage ||
                                 (Triggers)trigger == Triggers.NCRArbitrateCustomerCause)
                            {
                                await SendNotifications(taskData, destination, destinationSms, true);
                            }
                            else
                            {
                                await SendNotifications(taskData, destination, destinationSms);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var msg = ex.RetrieveErrorMessage();
                        if (msg.Equals(IndicatingMessages.SmsWarningMsg) || msg.Equals(IndicatingMessages.NoCellPhoneWarningMsg))
                        {
                            continue;
                        }
                        return ex.RetrieveErrorMessage();
                    }
                }

                ts.Complete();
                return null;
            }
        }

        private void UploadRevisionDocuments_Reseller(TaskData taskData, USER_TYPE userType, List<HttpPostedFileBase> files)
        {
            if (files == null)
            {
                throw new Exception(IndicatingMessages.ForgotUploadFile);
            }

            int docVersion = 0;
            var docs = TaskSetup.GetDocumentsByProductIdDocType(taskData.ProductId.Value, DOCUMENT_TYPE.REVISING_DOCS);
            if (docs != null && docs.Count > 0)
            {
                docVersion = docs[docs.Count - 1].Version;
            }
            for (int i = 0; i < files.Count; i++)
            {
                var postedFile = files[i];
                if (postedFile != null && postedFile.ContentLength == 0)
                {
                    continue;
                }

                if (postedFile.FileName == "")
                {
                    continue;
                }

                string fileName = $"revising_tid-{taskData.TaskId}_pid-{taskData.ProductId}_v-{++docVersion}{Path.GetExtension(postedFile.FileName)}";
                var docUri = DocumentStorageService.Upload(postedFile, fileName);

                var doc = new Document()
                {
                    TaskId = taskData.TaskId,
                    Version = docVersion,
                    Name = fileName,
                    DocUri = docUri,
                    UserType = (int)userType,
                    DocType = (int)DOCUMENT_TYPE.REVISING_DOCS,
                    ProductId = taskData.ProductId.Value,
                    UpdatedBy = UserContext.User.UserName,
                    CreatedUtc = DateTime.UtcNow,
                    ModifiedUtc = DateTime.UtcNow,
                    CreatedByUserId = UserContext.UserId,
                    ModifiedByUserId = UserContext.UserId,
                };
                DocumentService.AddDocument(doc);
            }
        }

        private void UploadRevisionDocuments(TaskData taskData, USER_TYPE userType, List<HttpPostedFileBase> files, ref List<Document> documents, int? bidRequestRevisionId = null)
        {
            if (files == null)
            {
                throw new Exception(IndicatingMessages.ForgotUploadFile);
            }
            var product = taskData.Product;
            if (product == null)
            {
                throw new Exception(IndicatingMessages.ProductNotFound);
            }
            //int docVersion = 0;
            //var docs = TaskSetup.GetDocumentsByProductIdDocType(taskData.ProductId.Value, DOCUMENT_TYPE.REVISING_DOCS);
            //docVersion = docs.Select(x => int.Parse(x.Name.Split('-').Last().Split('.')[0])).OrderBy(x => x).LastOrDefault();

            // get company Name
            var vendor = CompanyService.FindCompanyByUserId(UserContext.UserId);

            //if (docs != null && docs.Count > 0)
            //{
            //    docVersion = docs[docs.Count - 1].Version;
            //}
            for (int i = 0; i < files.Count; i++)
            {
                var postedFile = files[i];
                if (postedFile != null && postedFile.ContentLength == 0)
                {
                    continue;
                }

                if (postedFile.FileName == "")
                {
                    continue;
                }

                string fileName = $"{vendor.Name}-{product.PartNumber}-{product.PartNumberRevision}{Path.GetExtension(postedFile.FileName)}";
                var docUri = DocumentStorageService.Upload(postedFile, fileName);

                var doc = new Document()
                {
                    TaskId = taskData.TaskId,
                    Version = 0,
                    Name = fileName,
                    DocUri = docUri,
                    UserType = (int)userType,
                    DocType = (int)DOCUMENT_TYPE.REVISING_DOCS,
                    ProductId = taskData.ProductId.Value,
                    UpdatedBy = UserContext.User.UserName,
                    CreatedUtc = DateTime.UtcNow,
                    ModifiedUtc = DateTime.UtcNow,
                    CreatedByUserId = UserContext.UserId,
                    ModifiedByUserId = UserContext.UserId,
                    BidRequestRevisionId = bidRequestRevisionId,
                };
                DocumentService.AddDocument(doc);
                documents.Add(doc);
            }
        }

        private void CollectRevisionReasons(TaskData td, List<BidRequestRevision> brrList, States currentState)
        {
            foreach (var brr in brrList)
            {
                td.RevisingReason = string.Join(", ", brr.RFQActionReason?.Reason);
                //if (string.IsNullOrEmpty(td.RevisingReason))
                //    throw new Exception("Revision Reason is required");
            }
            var timer = TimerSetupService.FindTimerSetupByProductId(td.ProductId.Value);
            if (timer != null)
            {
                TimerTriggerService.RemoveTimerTrigger(timer.Name);
            }
        }

        private void CreateDocument(List<HttpPostedFileBase> files, TaskData taskData, string fname, DOCUMENT_TYPE docType, int userType)
        {
            if (taskData.ProductId == null)
            {
                throw new Exception("No product related to this task");
            }
            var productId = taskData.ProductId.Value;

            var documents = taskData.Product?.Documents?.ToList();
            IEnumerable<Document> docList = null;

            foreach (var postedFile in files)
            {
                string ext = Path.GetExtension(postedFile.FileName);
                if (postedFile != null && !string.IsNullOrEmpty(postedFile.FileName))
                {
                    if (documents != null)
                    {
                        docList = documents.Where(x => x.DocType == (int)docType);
                    }
                    else
                    {
                        docList = DocumentService.FindDocumentByProductId(productId).Where(x => x.DocType == (int)docType);
                    }
                    Document doc = null;
                    int docsCount = docList != null && docList.Count() > 0 ? docList.Count() + 1 : 1;
                    fname = Path.GetFileNameWithoutExtension(fname) + $"_v-{docsCount}{ext}";
                    var docUri = DocumentStorageService.Upload(postedFile, fname);
                    doc = new Document()
                    {
                        TaskId = taskData.TaskId,
                        Version = docsCount,
                        Name = fname,
                        UserType = userType,
                        DocUri = docUri,
                        DocType = (int)docType,
                        ProductId = productId,
                        UpdatedBy = UserContext.User.UserName,
                        CreatedUtc = DateTime.UtcNow,
                        ModifiedUtc = DateTime.UtcNow,
                        CreatedByUserId = UserContext.UserId,
                        ModifiedByUserId = UserContext.UserId,
                    };
                    DocumentService.AddDocument(doc);
                }
            }
        }

        public string DoRFQRevision_Reseller(TaskData taskData, string revisingReason, USER_TYPE userType, List<HttpPostedFileBase> files = null)
        {
            int productId = taskData.ProductId.Value;
            int taskId = taskData.TaskId;
            List<int> vendorIds = taskData.ProductId != null ? RfqBidService.FindRFQBidListByProductId(taskData.ProductId.Value).Select(x => x.VendorId).ToList() : new List<int>();
            try
            {
                UploadRevisionDocuments_Reseller(taskData, userType, files);
            }
            catch (Exception ex)
            {
                return $"State at : {Enum.GetName(typeof(States), taskData.StateId)}, Error: {ex.RetrieveErrorMessage()}";
            }

            // Store data in BidRequestRevision table
            var originTask = TaskDataService.FindTaskDataListByProductId(productId).Where(x => x.RFQBidId == null).FirstOrDefault();
            if (originTask == null)
            {
                return IndicatingMessages.TaskNotFound;
            }
            var company = UserContext.Company; // CompanyService.FindCompanyByUserId(currentUserId);
            var lastEntry = BidRequestRevisionService.FindRBidRequestRevisiondByVendorIdProductIdCustomerTaskId(company.Id, productId, originTask.TaskId).LastOrDefault();

            var lastRevisionNumber = lastEntry != null ? lastEntry.RevisionNumber : 0;
            var rr = new BidRequestRevision()
            {
                TaskId = taskId,
                CustomerTaskId = originTask.TaskId,
                ProductId = productId,
                VendorId = company.Id,
                CreateDateTime = DateTime.UtcNow,
                RevisionNumber = ++lastRevisionNumber,
            };
            BidRequestRevisionService.AddBidRequestRevision(rr);

            var brrList = BidRequestRevisionService.FindBidRequestRevisionListByProductIdTaskIdRevisionNumber(productId, originTask.TaskId, rr.RevisionNumber.Value);
            if (brrList.Count() == vendorIds.Count || TaskDatasBL.IsAllVendorsResponsed(productId, vendorIds.Count))
            {
                CollectRevisionReasons(originTask, brrList, (States)taskData.StateId);
                var tdList = TaskDataService.FindTaskDataListByProductId(productId).Where(x => x.RFQBidId != null && x.RFQBid.IsActive == null).ToList();
                tdList.Add(originTask);
                foreach (var t in tdList)
                {
                    t.StateId = taskData.StateId;
                    t.UpdatedBy = UserContext.User.UserName;
                    t.ModifiedUtc = DateTime.UtcNow;
                    t.ModifiedByUserId = UserContext.UserId;

                    TaskDataService.Update(t);
                }
            }
            else
            {
                if (brrList.Count() == 1)
                {
                    // start timer for revision requests by other vendor. if this timer timeout, system won't wait for any other
                    // revision requests and handle the requests so far collected
                    var timer = TimerSetupService.FindTimerSetupByProductIdTimerType(productId, TypeOfTimers.RFQRevisionTimer);
                    if (timer != null)
                    {
                        TimerTriggerService.StartTimer(timer.Name, timer.UnitEnum, timer.Interval, timer.ProductId, timer.CallbackMethod);
                        timer.TimerStartAt = DateTime.UtcNow;
                        TimerSetupService.UpdateTimerSetup(timer);
                    }
                }
            }
            return null;
        }


        public string DoRFQRevision(TaskData taskData, string revisingReason, USER_TYPE userType, List<HttpPostedFileBase> files = null)
        {
            if (taskData.ProductId == null)
                return IndicatingMessages.ProductNotFound;

            int productId = taskData.ProductId.Value;
            int taskId = taskData.TaskId;
            var allTasks = TaskDataService.FindTaskDataListByProductId(productId);

            List<int> vendorIds = allTasks
                //.Where(x => x.RFQBid != null && !(x.StateId == (int)States.RFQFailed || x.StateId == (int)States.VendorRejectedRFQ))
                .Where(x => x.RFQBid != null)
                .Select(x => x.RFQBid.VendorId).ToList();

            List<Document> docs = new List<Document>();


            // Store data in BidRequestRevision table
            var originTask = allTasks.Where(x => x.RFQBidId == null).FirstOrDefault();
            if (originTask == null)
                return IndicatingMessages.TaskNotFound;

            var company = UserContext.Company; // CompanyService.FindCompanyByUserId(currentUserId);
            var lastEntry = BidRequestRevisionService.FindRBidRequestRevisiondByVendorIdProductIdCustomerTaskId(company.Id, productId, originTask.TaskId);
            //var lastRevisionCycle = BidRequestRevisionService.FindBidRequestRevisionListByProductId(productId)
            //    .Select(x => x.RevisionNumber).OrderBy(x => x).LastOrDefault();

            var lastRevisionNumber = lastEntry?.Select(x => x.RevisionNumber).OrderBy(x => x).LastOrDefault() ?? 0;
            int? actionReasonId = rfqActionReasonService.FindRFQActionReasonListByProductIdVendorId(productId, company.Id, REASON_TYPE.RFQ_REVISION_REQUEST)?.Id;

            var rr = new BidRequestRevision()
            {
                TaskId = taskId,
                CustomerTaskId = originTask.TaskId,
                ProductId = productId,
                VendorId = company.Id,
                CreateDateTime = DateTime.UtcNow,
                RevisionNumber = ++lastRevisionNumber,
                RFQActionReasonId = actionReasonId,
            };
            var revisionId = BidRequestRevisionService.AddBidRequestRevision(rr);

            try
            {
                UploadRevisionDocuments(taskData, userType, files, ref docs, revisionId);
            }
            catch (Exception ex)
            {
                return $"State at : {Enum.GetName(typeof(States), taskData.StateId)}, Error: {ex.RetrieveErrorMessage()}";
            }

            var bidRFQRevision = BidRequestRevisionService.FindBidRequestRevisionById(revisionId);
            bidRFQRevision.Documents = docs;
            BidRequestRevisionService.UpdateBidRequestRevision(bidRFQRevision);
            var bidRFQStatus = bidRFQStatusService.FindBidRFQStatusListByProductId(productId).LastOrDefault();

            bidRFQStatus.SubmittedVendors = TaskDatasBL.NumberVendorResponsed(taskData.ProductId.Value);
            bidRFQStatusService.UpdateBidRFQStatus(bidRFQStatus);

            // Now store to VendorBidRFQStatus table
            var vendorBidRFQStatus = new VendorBidRFQStatus
            {
                ProductId = productId,
                TaskId = taskId,
                VendorId = company.Id,
                StateId = taskData.StateId,
                BidRequestRevisionId = revisionId,
                CreatedByUserId = UserContext.UserId,
                BidRFQStatusId = bidRFQStatus?.Id,
            };
            var vendorBidRFQStatusId = vendorBidRFQStatusService.AddVendorBidRFQStatus(vendorBidRFQStatus);
            var vendorBidRFQStatuses = vendorBidRFQStatusService.FindVendorBidRFQStatusListByProductId(productId);


            var brrList = BidRequestRevisionService.FindBidRequestRevisionListByProductIdTaskIdRevisionNumber(productId, taskId, rr.RevisionNumber.Value);
            if (brrList.Count() == vendorIds.Count || TaskDatasBL.IsAllVendorsResponsed(productId, vendorIds.Count))
            {
                var tasks = allTasks.Where(x => x.StateId != (int)States.VendorRejectedRFQ);
                var rejected = allTasks.Where(x => x.StateId == (int)States.VendorRejectedRFQ);
                if (rejected.Count() == vendorIds.Count)
                {
                    taskData.StateId = (int)States.RFQFailed;
                }
                else
                {
                    CollectRevisionReasons(originTask, brrList, (States)taskData.StateId);
                }

                foreach (var td in tasks)
                {
                    if (td.TaskId == taskData.TaskId) continue;
                    td.StateId = taskData.StateId;
                    td.UpdatedBy = UserContext.User.UserName;
                    td.ModifiedByUserId = UserContext.UserId;
                    td.ModifiedUtc = DateTime.UtcNow;
                    TaskDataService.Update(td);
                }

                // Update stateid in [BidRFQStatus] table
                bidRFQStatus.StateId = taskData.StateId;
                bidRFQStatus.SubmittedVendors = vendorIds.Count;
                bidRFQStatus.ModifiedByUserId = UserContext.UserId;
                bidRFQStatusService.UpdateBidRFQStatus(bidRFQStatus);
            }
            return null;
        }


        public async Task<int> RFQBidReview(RFQViewModel model, List<HttpPostedFileBase> files)
        {
            using (var ts = AsyncTransactionScope.StartNew())
            {
                int rfqbidid = 0;

                var product = ProductService.FindProductById(model.Id);
                var td = TaskDataService.FindById(model.TaskId.Value);
                States newState = States.BidReview;
                RFQBid bid = td.RFQBid;
                int? vendorId = null;
                if (bid != null)
                {
                    vendorId = bid.VendorId;
                }
                else
                {
                    vendorId = product?.VendorId.Value;
                }
                if (vendorId == null)
                {
                    throw new Exception(IndicatingMessages.VendorNotFound);
                }
                int v = 0;
                Document quoteDoc = null;
                var vendor = CompanyService.FindCompanyById(vendorId.Value);
                foreach (var postedFile in files)
                {                 
                    if (!string.IsNullOrEmpty(postedFile.FileName))
                    {
                        v++;
                        if (postedFile.ContentType.Contains("application/pdf"))
                        {
                            //string fileName = $"quote_pid{product.Id}_v{v}_coid-{vendorId.Value}.pdf";
                            string fileName = $"{vendor.Name}-{product.PartNumber}-{product.PartNumberRevision}_v-{v}.pdf";
                            var docUri = DocumentStorageService.Upload(postedFile, fileName);
                            quoteDoc = new Document()
                            {
                                TaskId = model.TaskId,
                                UserType = (int)USER_TYPE.Vendor,
                                Version = 1,
                                Name = fileName,
                                DocUri = docUri,
                                DocType = (int)DOCUMENT_TYPE.QUOTE_PDF,
                                ProductId = product.Id,
                                UpdatedBy = UserContext.User.UserName,
                                CreatedUtc = DateTime.UtcNow,
                                ModifiedUtc = DateTime.UtcNow,
                            };
                            var doc = DocumentService.FindDocumentByProductId(product.Id)
                                .Where(x => x.Name == fileName &&
                                            x.DocType == (int)DOCUMENT_TYPE.QUOTE_PDF).FirstOrDefault();
                            if (doc == null)
                            {
                                var docId = DocumentService.AddDocument(quoteDoc);
                                if (bid != null)
                                {
                                    bid.QuoteDocId = docId;
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("Uploaded quote file must be in PDF format.");
                        }
                    }
                }


                // Query DHL Shipping quote

                var now = DateTime.UtcNow;
                model.ShippingQuoteVM.BkgDetails.PaymentAccountNumber = ConfigurationManager.AppSettings["DHL_Padtech_Payment_Account"];
                model.ShippingQuoteVM.BkgDetails.Date = now.ToString("yyyy-MM-dd");
                model.ShippingQuoteVM.BkgDetails.ReadyTime = "PT10H21M";
                model.ShippingQuoteVM.BkgDetails.ReadyTimeGMTOffset = "+01:00";

                // Create a Pieces list based on the first input of Piece which is based on Qty = 100

                var piece = model.ShippingQuoteVM.BkgDetails.Pieces.First();
                float initialWeight = model.ShippingQuoteVM.BkgDetails.Pieces[0].Weight;
                float initialVolume = piece.Width * piece.Depth;

                //int pieceId = 2; // skip the first one
                if (bid != null)
                {
                    bid.ProductLeadTime = model.ProductionLeadTime;
                    bid.SampleLeadTime = model.SampleLeadTime;
                    bid.ToolingCharge = model.ToolingSetupCharges;
                    bid.HarmonizedCode = model.HarmonizedCode;
                    bid.PreferredCurrency = model.PreferredCurrency;
                    RfqBidService.UpdateRFQBid(bid);
                }
                else
                {
                    bid = new RFQBid()
                    {
                        ProductId = product.Id,
                        VendorId = vendorId.Value,
                        BidDatetime = DateTime.UtcNow,
                        ProductLeadTime = model.ProductionLeadTime,
                        SampleLeadTime = model.SampleLeadTime,
                        HarmonizedCode = model.HarmonizedCode,
                        ToolingCharge = model.ToolingSetupCharges,
                        PreferredCurrency = model.PreferredCurrency,
                    };
                    rfqbidid = RfqBidService.AddRFQBid(bid);
                }
                int pieceId = 1;
                int numberPartsPerPiece = model.ShippingQuoteVM.BkgDetails.NumberPartsPerPiece;
                var pbs = model.PriceBreakVM.PriceBreakList;

                if (pbs == null || numberPartsPerPiece <= 0)
                {
                    ts.Complete();
                    return rfqbidid;
                }
                int? productPriceQuoteId = null;
                if (model.isEnterprise)
                {
                    // Create a new entry in ProductPriceQuote table for this priceBreak
                    ProductPriceQuote q = new ProductPriceQuote
                    {
                        VendorId = bid.VendorId,
                        ProductId = product.Id,
                        ProductionLeadTime = product.ProductionLeadTime != null ? product.ProductionLeadTime.Value : 0,
                        ExpireDate = model.ExpireDate,

                        // set to false for now, later on will be set to true for the chosen one.
                        IsActive = false, 

                        // ToDo: presuming quote doc is single, if it is multiple, here only choose the last one.
                        QuoteDocUri = quoteDoc.DocUri, 
                    };
                    productPriceQuoteId = productPriceQuoteService.AddProductPriceQuote(q);
                }

                pbs = pbs.Where(x => x != null && x.Quantity > 0).Select(q => q).ToList();
                for (int i = 0; i < pbs.Count; i++)
                {
                    double rates = (double) pbs[i].Quantity / numberPartsPerPiece;
                    int numberPieces = (int)Math.Ceiling(rates);
                    for (int j = pieceId; j <= numberPieces; j++)
                    {
                        var p = new Piece()
                        {
                            Height = piece.Height,
                            Width = piece.Width,
                            Depth = piece.Depth,
                            Weight = piece.Weight
                        };
                        p.PieceID = j;
                        if (j == 1)
                        {
                            model.ShippingQuoteVM.BkgDetails.Pieces[0] = p;
                        }
                        else
                        {
                            model.ShippingQuoteVM.BkgDetails.Pieces.Add(p);
                        }
                    }
                    pieceId = model.ShippingQuoteVM.BkgDetails.Pieces.Last().PieceID + 1;
                    model.ShippingQuoteVM.BkgDetails.ShipmentWeight = numberPieces * initialWeight;
                    model.ShippingQuoteVM.BkgDetails.Volume = numberPieces * initialVolume;
                    model.ShippingQuoteVM.BkgDetails.NumberPieces = numberPieces;


                    // Set Dutiable values
                    var duti = new Dutiable()
                    {
                        DeclaredCurrency = Common.Currency.USD,
                        DeclaredValue = (Convert.ToDecimal(pbs[i].Quantity) *
                                         Convert.ToDecimal(pbs[i].VendorUnitPrice)).ToString()
                    };

                    model.ShippingQuoteVM.BkgDetails.IsDutiable = "Y";
                    model.ShippingQuoteVM.Dutiable = duti;

                    DHLShippingQuoteResponse response = null;
                    PriceBreak existingPriceBreaks = null;
                    response = SendQuoteRequest(model.ShippingQuoteVM);

                    if (response == null)
                    {
                        continue;
                    }
                    // Save response together with price breaks to PriceBreaks table
                    var pb = pbs[i];
                    pb.RFQBidId = bid.Id;
                    pb.Quantity = pbs[i].Quantity;
                    pb.VendorUnitPrice = pbs[i].VendorUnitPrice;
                    pb.ShippingDays = response.ShippingDays;
                    pb.ShippingUnitPrice = response.ShippingCharge / pbs[i].Quantity;
                    pb.ToolingSetupCharges = model.ToolingSetupCharges;
                    pb.NumberSampleIncluded = model.NumberSampleIncluded ?? 1;
                    pb.TaskId = model.TaskId;
                    pb.ProductId = product.Id;

                    // test if isEnterprise is true
                    if (model.isEnterprise)
                    {
                        pb.UnitPrice = pbs[i].VendorUnitPrice;
                        pb.CustomerToolingSetupCharges = model.ToolingSetupCharges;
                        pb.ProductPriceQuoteId = productPriceQuoteId;
                    }
                    if (pb.TaskId != null)
                    {
                        existingPriceBreaks = PriceBreakService.FindPriceBreakByTaskIdProductIdExactQty(pb.TaskId.Value, pb.ProductId, pb.Quantity);
                    }
                    else if (pb.RFQBidId > 0)
                    {
                        existingPriceBreaks = PriceBreakService.FindPriceBreakByProductIdRFQBidIdQty(pb.ProductId, pb.RFQBidId.Value, pb.Quantity).LastOrDefault();
                    }
                    else
                    {
                        existingPriceBreaks = PriceBreakService.FindPriceBreakByProductIdQty(pb.ProductId, pb.Quantity);
                    }

                    if (existingPriceBreaks == null)
                    {
                        pb.CreatedByUserId = UserContext.UserId;

                        var pbId = PriceBreakService.AddPriceBreak(pb);
                    }
                    else
                    {
                        existingPriceBreaks.VendorUnitPrice = pb.VendorUnitPrice;
                        existingPriceBreaks.ShippingDays = pb.ShippingDays;
                        existingPriceBreaks.ShippingUnitPrice = pb.ShippingUnitPrice;
                        existingPriceBreaks.ToolingSetupCharges = pb.ToolingSetupCharges;
                        existingPriceBreaks.NumberSampleIncluded = pb.NumberSampleIncluded ?? 1;

                        if (model.isEnterprise)
                        {
                            existingPriceBreaks.UnitPrice = pb.UnitPrice;
                            existingPriceBreaks.CustomerToolingSetupCharges = pb.CustomerToolingSetupCharges;
                        }

                        PriceBreakService.ModifyPriceBreak(existingPriceBreaks);
                    }
                }

                if (rfqbidid > 0)
                {
                    td.RFQBidId = rfqbidid;
                }
                td.StateId = (int)newState;
                TaskDataService.Update(td);

                // update this product's PriceBreakId to the last of newly created price breaks

                product.PriceBreakId = pbs.LastOrDefault()?.Id;
                ProductService.UpdateProduct(product);

                int ret = await TaskDatasBL.CheckIfAllVendorsResponsed(td);
                if (ret == 0)
                {
                    ts.Complete();
                    return rfqbidid;
                }
                else
                {
                    ts.Complete();
                    return -1;
                }
            }
        }

        public IEnumerable<CompanyDTO> GetUserPerformanceWrapper(IEnumerable<CompanyDTO> vendors, CompanyType companyType, int? toCompanyId = null)
        {
            foreach (var vendor in vendors)
            {
                UserPerformanceViewModel vp = GetUserPerformance(vendor.Id.Value, companyType, toCompanyId);
                if (vp == null)
                {
                    continue;
                }
                vendor.UserPerformance = Mapper.Map<UserPerformance>(vp);
                vendor.UserPerformance.UserName = vendor.Name;
                vendor.UserPerformance.UserLocation = Mapper.Map<AddressDTO>(vendor.MainCompanyAddress ?? vendor.Address);
            }
            return vendors;
        }

        public UserPerformanceViewModel GetUserPerformance(int fromCompanyId, CompanyType companyType, int? toCompanyId = null)
        {
            var orders = (companyType == CompanyType.Vendor ? OrderService.FindOrdersByVendorId(fromCompanyId) : OrderService.FindOrdersByCustomerId(fromCompanyId));
            var tasks = companyType == CompanyType.Vendor ? TaskDataService.FindTaskDatasByVendorId(fromCompanyId) : TaskDataService.FindTaskDatasByCustomerId(fromCompanyId);

            var completedOrders = (from td in tasks
                                   join ord in orders on td.TaskId equals ord.TaskId into td_ord
                                   from tdo in td_ord.DefaultIfEmpty()
                                   where td.StateId == (int)States.ProductionComplete &&
                                         tdo.Product != null &&
                                         tdo.PartNumber != null && tdo.Quantity > 0
                                   select new
                                   {
                                       Id = tdo.Id,
                                       State = td.StateId,
                                       Quantity = tdo.Quantity,
                                       ShippedDate = tdo.ShippedDate,
                                       DesireShippingDate = tdo.DesireShippingDate
                                   });

            var orderQuantity = orders.Sum(x => (int?)x.Quantity) ?? 0;
            var ncrs = (companyType == CompanyType.Vendor ? NcReportService.FindNCReportByVendorId(fromCompanyId) : NcReportService.FindNCReportByCustomerId(fromCompanyId));
            var totalNcrs = (from ord in orders
                             join ncr in ncrs on ord.Id equals ncr.OrderId into ord_ncr
                             from nc in ord_ncr.DefaultIfEmpty()
                             where ord != null && ord.Quantity > 0 && nc != null && nc.Quantity > 0
                             select new
                             {
                                 Qty = nc.Quantity,
                             });

            var ncrQuantity = totalNcrs.Sum(x => (int?)x.Qty) ?? 0;
            int orderCount = orders.Count();
            int ncrCount = ncrs.Count();
            int numberOfMyRootcause = ncrs.Count(x => x.NCRootCauseCompanyId != null && x.NCRootCauseCompanyId.Value == fromCompanyId);
            int onTimeOrders = completedOrders.ToList()
                                .Where(x => x.ShippedDate != null && x.DesireShippingDate != null)
                                .Count(x => (x.ShippedDate.Value - x.DesireShippingDate.Value).TotalDays < 0);
            var LeadTimes = orders.Where(x => x.ShipLeadingTime != null);
            var avrLeadTime = LeadTimes != null && LeadTimes.Any() ? (int?)LeadTimes.Average(x => x.ShipLeadingTime.Value) : null;
            if (avrLeadTime == null)
            {
                LeadTimes = orders.Where(x => x.Product.ProductionLeadTime != null);
                avrLeadTime = LeadTimes != null && LeadTimes.Any() ? (int?)LeadTimes.Average(x => x.Product.ProductionLeadTime.Value) : null;
            }
            int? completeOrderCount = completedOrders.Count();

            if (orderQuantity == 0 || orderCount == 0)
                return null;

            var model = new UserPerformanceViewModel
            {
                PartConformance = orderQuantity > 0 ? (float)(orderQuantity - ncrQuantity) / orderQuantity : 0,
                OrderConformance = orderCount > 0 ? (float)(orderCount - ncrCount) / orderCount : 0,
                CompletedOrders = completeOrderCount,
                CompletedParts = completedOrders != null && completedOrders.Count() > 0 ? (int)completedOrders.Sum(x => x.Quantity) : 0,
                OnTimeConformance = completeOrderCount != null && completeOrderCount > 0 ? (float)onTimeOrders / (float)completeOrderCount.Value : 0,
                AvrLeadTime = avrLeadTime,
                NumberOfMyRootCause = numberOfMyRootcause,
            };

            //Fix NaN
            //if (model.PartConformance != null && float.IsNaN(model.PartConformance.Value))
            //{
            //    model.PartConformance = null;
            //}
            //if (model.OrderConformance != null && float.IsNaN(model.OrderConformance.Value))
            //{
            //    model.OrderConformance = null;
            //}
            //if (model.OnTimeConformance != null && float.IsNaN(model.OnTimeConformance.Value))
            //{
            //    model.OnTimeConformance = null;
            //}

            return model;
        }

        // 

        public IEnumerable<CompanyDTO> GetUserPerformanceByProductIdWrapper(IEnumerable<CompanyDTO> companies, IQueryable<Order> orders,
                                                                            IQueryable<NCReport> ncrs)
        {
            foreach (var company in companies)
            {
                UserPerformanceViewModel vp = GetPerformance(company.Id.Value, orders, ncrs);
                if (vp == null)
                {
                    continue;
                }
                company.UserPerformance = Mapper.Map<UserPerformance>(vp);
                company.UserPerformance.UserName = company.Name;
                company.UserPerformance.UserLocation = Mapper.Map<AddressDTO>(company.MainCompanyAddress ?? company.Address);
            }
            return companies;
        }



        public UserPerformanceViewModel GetPerformance(int fromCompanyId, IQueryable<Order> orders, IQueryable<NCReport> ncrs)
        {
            var completedOrders = orders.Where(x => x.TaskData.StateId == (int)States.ProductionComplete)
                .Select(x => new
                {
                    Id = x.Id,
                    Quantity = x.Quantity,
                    Month = x.OrderDate.Month,
                    ShippedDate = x.ShippedDate,
                    DesireShippingDate = x.DesireShippingDate,
                    isSampleQty = (x.TaskData.Product.PriceBreakId != null ? x.Quantity == x.TaskData.Product.PriceBreak.NumberSampleIncluded : false),
                });

            var orderQuantity = orders.Sum(x => (int?)x.Quantity) ?? 0;
            var totalNcrs = (from ord in orders
                             join ncr in ncrs on ord.Id equals ncr.OrderId into ord_ncr
                             from nc in ord_ncr.DefaultIfEmpty()
                             where ord != null && ord.Quantity > 0 && nc != null && nc.Quantity > 0
                             select new
                             {
                                 OrderId = nc.OrderId,
                                 Month = nc.Order.OrderDate.Month,
                                 Qty = nc.Quantity,
                             });

            var ncrQuantity = totalNcrs.Sum(x => (int?)x.Qty) ?? 0;
            int orderCount = orders.Count();
            int ncrCount = ncrs.Count();
            int numberOfMyRootcause = ncrs.Count(x => x.NCRootCauseCompanyId != null && x.NCRootCauseCompanyId.Value == fromCompanyId);
            int onTimeOrders = completedOrders.ToList()
                                .Where(x => x.ShippedDate != null && x.DesireShippingDate != null)
                                .Count(x => (x.ShippedDate.Value - x.DesireShippingDate.Value).TotalDays < 0);
            var LeadTimes = orders.Where(x => x.ShipLeadingTime != null);
            var avrLeadTime = LeadTimes != null && LeadTimes.Any() ? (int?)LeadTimes.Average(x => x.ShipLeadingTime.Value) : null;
            if (avrLeadTime == null)
            {
                LeadTimes = orders.Where(x => x.Product.ProductionLeadTime != null);
                avrLeadTime = LeadTimes != null && LeadTimes.Any() ? (int?)LeadTimes.Average(x => x.Product.ProductionLeadTime.Value) : null;
            }
            int? completeOrderCount = completedOrders.Count();

            if (orderQuantity == 0 || orderCount == 0)
                return null;

            // calculate actual completed order quantities (substracting NCR amount)
            var actualCompletedOrders = (from ord in completedOrders
                                         join ncr in totalNcrs
                                         on ord.Month equals ncr.Month into ord_ncr
                                         from nc in ord_ncr.DefaultIfEmpty()
                                         select new
                                         {
                                             Qty = nc != null && ord.Id == nc.OrderId && !ord.isSampleQty ? ord.Quantity - nc.Qty : ord.Quantity,
                                         }
                );

            UserPerformanceViewModel model = new UserPerformanceViewModel
            {
                PartConformance = (float)(orderQuantity - ncrQuantity) / orderQuantity,
                OrderConformance = (float)(orderCount - ncrCount) / orderCount,
                CompletedOrders = completeOrderCount ?? 0,
                CompletedParts = actualCompletedOrders != null && actualCompletedOrders.Count() > 0 ? (int) actualCompletedOrders.Sum(x => x.Qty) : 0,
                OnTimeConformance = completeOrderCount > 0 ? (float)onTimeOrders / (float)completeOrderCount.Value : 0,
                AvrLeadTime = avrLeadTime,
                NumberOfMyRootCause = numberOfMyRootcause,
            };

            //Fix NaN
            //if (model.PartConformance != null && float.IsNaN(model.PartConformance.Value))
            //{
            //    model.PartConformance = null;
            //}
            //if (model.OrderConformance != null && float.IsNaN(model.OrderConformance.Value))
            //{
            //    model.OrderConformance = null;
            //}
            //if (model.OnTimeConformance != null && float.IsNaN(model.OnTimeConformance.Value))
            //{
            //    model.OnTimeConformance = null;
            //}

            return model;
        }


        public async Task<CompanyQualityAnalyticsStatisticsViewModel> GetCompanyQualityAnalyticsStatistics(IQueryable<Order> orders, IQueryable<NCReport> ncrs, int companyId, UserMode mode)
        {
            float ordersConformaceCount = orders.Count();

            var totalNcrs = (from ord in orders
                             join ncr in ncrs on ord.Id equals ncr.OrderId into ord_ncr
                             from nc in ord_ncr.DefaultIfEmpty()
                             where ord != null && ord.Quantity > 0 && nc != null && nc.Quantity > 0
                             select nc
                             );

            float totalNcrCount = mode == UserMode.Customer ? totalNcrs.Where(x => x.NCRootCauseCompanyId != companyId).Count() : totalNcrs.Where(x => x.NCRootCauseCompanyId == companyId).Count();

            float totalShippedOrderCounts = orders.Where(x => x.ShippedDate != null).Count();
            var ordersConformanceRate = totalShippedOrderCounts > 0 ? (totalShippedOrderCounts - totalNcrCount) / totalShippedOrderCounts : 0;

            float partsConformanceCount = (float) orders.Sum(x => x.Quantity);
            float shippedParts = (int)  orders.Where(x => x.ShippedDate != null).ToList().Sum(x => x.Quantity);
            float ncrParts = totalNcrs.Where(x => x.Quantity != null).ToList().Sum(x => x.Quantity.Value);
            var partsConformanceRate = shippedParts > 0 ? (shippedParts - ncrParts) / shippedParts : 0;

            float shippedOrderCount = orders.Count(x => x.ShippedDate != null);
            var onTimeOrderCount = orders.ToList()
                                         .Where(x => x.ShippedDate != null && x.DesireShippingDate != null)
                                         .Count(x => (x.ShippedDate.Value - x.DesireShippingDate.Value).TotalDays < 0);

            float onTimeConformanceRate = shippedOrderCount > 0 ? (float)(onTimeOrderCount / shippedOrderCount) : 0;

            var stats = new CompanyQualityAnalyticsStatisticsViewModel
            {
                OrdersConformanceRate = ordersConformanceRate,
                OrdersConformaceCount = ordersConformaceCount,
                PartsConformanceRate = partsConformanceRate,
                PartsConformanceCount = partsConformanceCount,
                OnTimeConformanceRate = onTimeConformanceRate,
            };

            return await System.Threading.Tasks.Task.FromResult(stats);

        }

      

        private RequestBase SetupQuoteRequest(ShippingQuoteRequestViewModel model)
        {
            RequestBase requestQuote = mapper.Map<RequestQuote>(model);

            requestQuote.RequestType = REQUESTS.CAPABILITY;

            return requestQuote;
        }


        private DHLShippingQuoteResponse SendQuoteRequest(ShippingQuoteRequestViewModel shippingQuoteVM)
        {
            string xmlFile = ConfigurationManager.AppSettings["QuoteRequest"];
            string xmlWriteToFile = ConfigurationManager.AppSettings["WriteToQuoteRequest"];
            string filePath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(@"~/DHLShippingXml/"), xmlFile);
            string fileWriteToPath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(@"~/DHLShippingXml/"), xmlWriteToFile);
            //string filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), xmlFile);
            //string fileWriteToPath = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), xmlWriteToFile);

            string url = ConfigurationManager.AppSettings["DHLWebApi"];

            // Send Quote Request to DHL web api
            RequestBase request = SetupQuoteRequest(shippingQuoteVM);
            try
            {
                DHLApi.SetupRequest(filePath, fileWriteToPath, request);
            }
            catch
            {
                return null;
            }

            string responseString = DHLApi.XmlRequest(url, fileWriteToPath).Result;
            DHLResponse resp = DHLApi.XmlResponse(responseString, REQUESTS.CAPABILITY);

            string productName = string.Empty;

            QtdShpResp quoteResp = null;
            if (resp.Quote.BkgDetails.QtdShps != null)
            {
                foreach (var quote in resp.Quote.BkgDetails.QtdShps)
                {
                    productName = quote.LocalProductName;
                    if (productName.Equals(DHL_SHIPPING_PRODUCT_NAME.EXPRESS_WORLDWIDE_NONDOC, StringComparison.CurrentCultureIgnoreCase))
                    {
                        quoteResp = quote;
                        break;
                    }
                }
            }

            var response = new DHLShippingQuoteResponse()
            {
                ShippingDays = quoteResp != null ? Convert.ToInt16(quoteResp.TotalTransitDays) : 0,
                ShippingCharge = quoteResp != null ? Convert.ToDecimal(quoteResp.ShippingCharge) : 0,
            };

            return response;
        }
        private async System.Threading.Tasks.Task CallQBOCreateVendorInvoice(TaskData taskData, Order order, VendorInvoiceViewModel vm)
        {
            // Create Bill in QBO for vendor
            Product p = ProductService.FindProductById(taskData.ProductId.Value);
            if (taskData.Product != null && taskData.Product.VendorId != null)
            {
                var vendor = taskData.Product.VendorCompany;

                var bill = OmnaeInvoiceService.FindOmnaeInvoiceByCompanyIdByTaskId(vendor.Id, taskData.TaskId).Where(x => x.OrderId == order.Id).FirstOrDefault();

                if (bill != null)
                {
                    BillViewModel billVM = new BillViewModel();
                    var prod = taskData.Product;

                    billVM.TaskId = bill.TaskId;
                    billVM.BillId = bill.BillId;
                    billVM.BillNumber = vm.VendorAttachedInvoiceNumber;
                    billVM.ProductId = bill.ProductId;
                    billVM.PartNumber = prod.PartNumber;
                    billVM.PartRevision = prod.PartRevision != null ? prod.PartRevision.Name : null;
                    billVM.ProductCategory = prod.Material;
                    if (bill.UnitPrice > 0)
                    {
                        billVM.Total = bill.Quantity * bill.UnitPrice;
                        if (vm.AttachInvoice != null)
                        {
                            billVM.AttachFileName = Path.GetFileName(vm.AttachInvoice.FileName);
                            billVM.AttachFileSize = vm.AttachInvoice.ContentLength;
                            billVM.AttachFileContentType = vm.AttachInvoice.ContentType;
                            billVM.FStream = vm.AttachInvoice.InputStream;
                            billVM.BillNumber = vm.VendorAttachedInvoiceNumber;
                            billVM.AttachInvoice = vm.AttachInvoice;
                        }
                    }
                    else
                    {
                        billVM.Total = bill.ToolingSetupCharges;
                        if (vm.AttachInvoiceForTooling != null)
                        {
                            billVM.AttachFileName = Path.GetFileName(vm.AttachInvoiceForTooling.FileName);
                            billVM.AttachFileSize = vm.AttachInvoiceForTooling.ContentLength;
                            billVM.AttachFileContentType = vm.AttachInvoiceForTooling.ContentType;
                            billVM.FStream = vm.AttachInvoiceForTooling.InputStream;
                            billVM.BillNumber = vm.VendorAttachedInviceNumberForTooling;
                            billVM.AttachInvoice = vm.AttachInvoiceForTooling;
                        }
                    }

                    //billVM.ToolingCharges = bill.ToolingSetupCharges;
                    billVM.UnitPrice = bill.UnitPrice;
                    billVM.Quantity = bill.Quantity;
                    billVM.CustomerId = prod.CustomerId;
                    billVM.SalesTax = bill.SalesTax;
                    if (prod.CustomerId != null)
                    {
                        billVM.CustomerName = CompanyService.FindCompanyById(prod.CustomerId.Value).Name;
                    }

                    billVM.VendorId = prod.VendorId;
                    if (prod.VendorId != null)
                    {
                        billVM.VendorName = CompanyService.FindCompanyById(prod.VendorId.Value).Name;
                    }

                    billVM.ProductName = prod.Name.Trim();
                    billVM.ProductDescription = prod.Description != null ? prod.Description.Trim() : null;
                    billVM.OrderDate = vm.OrderDate;
                    billVM.DueDate = vendor.Term != null ? DateTime.UtcNow.AddDays(vendor.Term.Value) : vm.ShippedDate;
                    billVM.Term = vendor.Term;
                    billVM.QboId = vendor.QboId;
                    billVM.CustomerQboId = prod.CustomerCompany.QboId;
                    billVM.OrderId = order.Id;
                    billVM.isEnterprise = taskData.isEnterprise;
                    billVM.NumberSampleIncluded = vm.NumberSampleIncluded != null ? vm.NumberSampleIncluded.Value : 1;
                    billVM.UnitOfMeasurement = p.RFQQuantityId != null ? p.RFQQuantity.UnitOfMeasurement : 0;

                    // Call QBO API
                    await CallQBOCreateVendorBill(billVM);
                    bill.BillId = billVM.BillId;
                    bill.BillNumber = billVM.BillNumber;
                    OmnaeInvoiceService.UpdateOmnaeInvoice(bill);
                }
            }
        }

        private async System.Threading.Tasks.Task CallQBOCreateVendorBill(BillViewModel model)
        {
            QboApi qboApi = new QboApi(QboTokensService);
            await qboApi.CreateBill(model);
#if false
            byte[] data = await qboApi.CreateBill(model);
            if (data != null && model.VendorId != null)
            {
                string fileName = $"vendor_bill_vid-{model.VendorId.Value}_tid-{model.TaskId}_pid-{model.ProductId}_orderid-{model.OrderId}.pdf";
                var docUri = DocumentStorageService.Upload(data, fileName);
                var doc = new Document()
                {
                    TaskId = model.TaskId,
                    Version = 1,
                    Name = fileName,
                    DocUri = docUri,
                    DocType = (int)DOCUMENT_TYPE.QBO_BILL_PDF,
                    UserType = (int)USER_TYPE.Vendor,
                    ProductId = model.ProductId,
                    UpdatedBy = UserContext.User.UserName,
                    CreatedUtc = DateTime.UtcNow,
                    ModifiedUtc = DateTime.UtcNow
                };
                var d = DocumentService.FindDocumentByProductId(model.ProductId).Where(x => x.Name == fileName).FirstOrDefault();
                if (d == null)
                {
                    DocumentService.AddDocument(doc);
                }
            }
#endif
        }



        public System.Threading.Tasks.Task SendNotifications(TaskData taskData, string destination, string destinationSms,
                                       bool isAdmin = false, List<byte[]> attachmentData = null,
                                       List<string> VendorInvoicesNumbers = null)
        {
            return NotificationBL.SendNotificationsAsync(taskData, destination, destinationSms, isAdmin, attachmentData, VendorInvoicesNumbers);
        }

        private void SetupExtraQuantity(RFQViewModel model, List<HttpPostedFileBase> files)
        {
            var product = ProductService.FindProductById(model.Id);
            if (product.ExtraQuantityId == null)
                return;

            // update ExtraQuantity table with existing record retrieved from this Product
            ExtraQuantity exQty = ExtraQuantityService.FindExtraQuantityById(product.ExtraQuantityId.Value);

            exQty.SampleLeadTime = model.SampleLeadTime;
            exQty.ProductionLeadTime = model.ProductionLeadTime;
            exQty.ToolingSetupCharges = model.ToolingSetupCharges;
            exQty.HarmonizedCode = model.HarmonizedCode;
            exQty.NumberSampleIncluded = model.NumberSampleIncluded;

            ExtraQuantityService.UpdateExtraQuantity(exQty);

            int companyId = this.UserContext.Company.Id;

            foreach (HttpPostedFileBase postedFile in files)
            {
                Document quoteDoc = null;
                if (postedFile.FileName != "")
                {
                    if (postedFile.ContentType.Contains("application/pdf"))
                    {
                        string fileName = $"quote_pid-{product.Id}_coid-{companyId}{Path.GetExtension(postedFile.FileName)}";
                        var docUri = DocumentStorageService.Upload(postedFile, fileName);
                        quoteDoc = new Document()
                        {
                            TaskId = model.TaskId,
                            Version = 1,
                            Name = fileName,
                            DocUri = docUri,
                            DocType = (int)DOCUMENT_TYPE.QUOTE_PDF,
                            UserType = (int)USER_TYPE.Vendor,
                            ProductId = product.Id,
                            UpdatedBy = UserContext.User.UserName,
                            CreatedUtc = DateTime.UtcNow,
                            ModifiedUtc = DateTime.UtcNow
                        };
                        DocumentService.AddDocument(quoteDoc);
                    }
                    else
                    {
                        throw new Exception("Invalid file type - must be PDF file!");
                    }
                }
            }


            // Query DHL Shipping quote

            var now = DateTime.UtcNow;
            model.ShippingQuoteVM.BkgDetails.PaymentAccountNumber = ConfigurationManager.AppSettings["DHL_Padtech_Payment_Account"];
            model.ShippingQuoteVM.BkgDetails.Date = now.ToString("yyyy-MM-dd");
            model.ShippingQuoteVM.BkgDetails.ReadyTime = "PT10H21M";
            model.ShippingQuoteVM.BkgDetails.ReadyTimeGMTOffset = "+01:00";

            // Create a Pieces list based on the first input of Piece which is based on Qty = 100

            var piece = model.ShippingQuoteVM.BkgDetails.Pieces.First();
            float initialWeight = model.ShippingQuoteVM.BkgDetails.Pieces[0].Weight;
            float initialVolume = piece.Width * piece.Depth;

            int pieceId = 1;
            int numberPartsPerPiece = model.ShippingQuoteVM.BkgDetails.NumberPartsPerPiece;

            // Used for get markup value from previous qty's markup of same product id in PriceBreaks table.
            // previousQuantityPriceBreak should not be null, otherwise we'll stop here.
            PriceBreak previousQuantityPriceBreak = PriceBreakService.FindPriceBreakByProductId(product.Id).LastOrDefault();
            if (previousQuantityPriceBreak == null)
            {
                return;
            }

            for (int i = 0; i < model.PriceBreakVM.PriceBreakList.Count; i++)
            {
                var qty = model.PriceBreakVM.PriceBreakList[i].Quantity;
                int numberPieces = (int)Math.Ceiling(qty / numberPartsPerPiece);


                for (int j = pieceId; j <= numberPieces; j++)
                {
                    var p = new Piece()
                    {
                        Height = piece.Height,
                        Width = piece.Width,
                        Depth = piece.Depth,
                        Weight = piece.Weight
                    };
                    p.PieceID = j;
                    if (j == 1)
                    {
                        model.ShippingQuoteVM.BkgDetails.Pieces[0] = p;
                    }
                    else
                    {
                        model.ShippingQuoteVM.BkgDetails.Pieces.Add(p);
                    }

                }
                pieceId = model.ShippingQuoteVM.BkgDetails.Pieces.Last().PieceID + 1;


                model.ShippingQuoteVM.BkgDetails.ShipmentWeight = numberPieces * initialWeight;
                model.ShippingQuoteVM.BkgDetails.Volume = numberPieces * initialVolume;
                model.ShippingQuoteVM.BkgDetails.NumberPieces = numberPieces;


                // Set Dutiable values
                Dutiable duti = new Dutiable()
                {
                    DeclaredCurrency = Common.Currency.USD,
                    DeclaredValue = (Convert.ToDecimal(model.PriceBreakVM.PriceBreakList[i].Quantity) *
                                     Convert.ToDecimal(model.PriceBreakVM.PriceBreakList[i].VendorUnitPrice)).ToString()
                };

                model.ShippingQuoteVM.BkgDetails.IsDutiable = "Y";
                model.ShippingQuoteVM.Dutiable = duti;

                DHLShippingQuoteResponse response = null;
                response = SendQuoteRequest(model.ShippingQuoteVM);

                if (response != null)
                {

                    // Save response together with price breaks to PriceBreaks table
                    var rfqBidId = TaskDataService.FindTaskDataListByProductId(model.Id).LastOrDefault(x => x.RFQBidId != null)?.RFQBidId;
                    var pb = model.PriceBreakVM.PriceBreakList[i];
                    pb.Quantity = model.PriceBreakVM.PriceBreakList[i].Quantity;
                    pb.VendorUnitPrice = model.PriceBreakVM.PriceBreakList[i].VendorUnitPrice;
                    pb.ShippingDays = response.ShippingDays;
                    pb.ShippingUnitPrice = response.ShippingCharge / model.PriceBreakVM.PriceBreakList[i].Quantity;
                    pb.UnitPrice = pb.VendorUnitPrice + pb.ShippingUnitPrice;
                    if (!model.isEnterprise)
                    {
                        pb.UnitPrice *= (decimal)(previousQuantityPriceBreak.Markup != null ? previousQuantityPriceBreak.Markup : COMMON_MAX.DEFAULT_MARKUP);
                    }

                    pb.NumberSampleIncluded = model.NumberSampleIncluded;
                    pb.RFQBidId = rfqBidId ?? null;
                    pb.ToolingSetupCharges = model.ToolingSetupCharges;
                    pb.CustomerToolingSetupCharges = model.isEnterprise == true ? model.ToolingSetupCharges : (decimal?)null;
                    pb.TaskId = model.TaskId;
                    pb.CreatedByUserId = UserContext.UserId;
                    pb.NumberSampleIncluded = model.NumberSampleIncluded;

                    PriceBreak existingPriceBreak = null;
                    if (pb.TaskId != null)
                    {
                        existingPriceBreak = PriceBreakService.FindPriceBreakByTaskIdProductIdExactQty(pb.TaskId.Value, pb.ProductId, pb.Quantity);
                    }
                    else
                    {
                        existingPriceBreak = PriceBreakService.FindPriceBreakByProductId(pb.ProductId).FirstOrDefault(x => x.Quantity == pb.Quantity);
                    }
                    if (existingPriceBreak == null)
                    {
                        PriceBreakService.AddPriceBreak(pb);
                    }
                    else
                    {
                        pb.Id = existingPriceBreak.Id;
                        pb.ModifiedByUserId = UserContext.UserId;
                        PriceBreakService.ModifyPriceBreak(pb);
                    }
                }
            }
        }



        public PackingSlipUriViewModel CreatePackingSlip(TaskData td, ControllerContext controllerContext, Order order)
        {
            using (var ts = AsyncTransactionScope.StartNew())
            {
                string url = string.Empty;
                PackingSlipViewModel packingSlip = null;

                int taskId = td.TaskId;

                if (td.StateId == (int)States.SampleStarted || td.StateId == (int)States.SampleComplete || td.StateId == (int)States.SampleRejected)
                {
                    var pb = PriceBreakService.FindPriceBreakByTaskId(taskId).FirstOrDefault();
                    if (pb == null)
                    {
                        pb = PriceBreakService.FindPriceBreakByProductId(td.ProductId.Value).FirstOrDefault(x => x.RFQBid.IsActive == true);
                    }
                    if (order != null)
                    {
                        if (order.IsForToolingOnly == true)
                        {
                            // Packing slip for Tooling Charge only
                            packingSlip = SetupPackingSlipViewModel(td.ProductId.Value, taskId, pb.NumberSampleIncluded ?? 1);
                        }
                        else
                        {
                            // Packing slip for samples
                            packingSlip = SetupPackingSlipViewModel(td.ProductId.Value, taskId, pb.NumberSampleIncluded ?? 1);
                        }
                    }
                }
                else
                {
                    if (order != null)
                    {
                        packingSlip = SetupPackingSlipViewModel(td.ProductId.Value, taskId, order.Quantity);
                    }
                }

                var model = new PackingSlipUriViewModel
                {
                    OrderId = order.Id,
                };
                if (packingSlip != null)
                {
                    url = ExportPackingSlipToPdf(packingSlip, controllerContext);
                    model.Uri = url;
                }
                ts.Complete();
                return model;
            }
        }


        private PackingSlipViewModel SetupPackingSlipViewModel(int productId, int taskId, decimal quantity)
        {
            var product = ProductService.FindProductById(productId);
            if (product.CustomerId == null || product.VendorId == null)
            {
                return null;
            }

            var company = product.CustomerCompany; // CompanyService.FindCompanyById(product.CustomerId.Value);
            if (company.AddressId == null || company.ShippingId == null)
            {
                return null;
            }

            var taskData = TaskDataService.FindById(taskId);

            var orders = taskData.Orders; // OrderService.FindOrderByTaskId(taskData.TaskId);
            if (orders == null || orders.Count == 0)
            {
                return null;
            }

            Order order = orders.Where(x => x.Quantity == quantity || x.Quantity == 1).FirstOrDefault();
            if (order == null)
            {
                order = taskData.StateId == (int)States.SampleStarted ? orders.FirstOrDefault() : orders.LastOrDefault();
            }

            var billAddr = company.BillAddressId != null ? AddressService.FindAddressById(company.BillAddressId.Value) : AddressService.FindAddressById(company.AddressId.Value);
            var shipping = company.Shipping; // ShippingService.FindShippingById(company.ShippingId.Value);

            var shippingAddr = billAddr;
            if (shipping.AddressId != null)
            {
                shippingAddr = shipping.Address; // AddressService.FindAddressById(shipping.AddressId.Value);
            }

            if (taskData.Invoices == null || taskData.Invoices.Count == 0)
            {
                return null;
            }

            var invoice = taskData.Invoices.FirstOrDefault(x => x.UserType == (int)USER_TYPE.Customer);
            if (invoice == null)
            {
                return null;
            }
            //var userPhoneNumber = company.Users.FirstOrDefault()?.PhoneNumber; //TODO: Chant the Logic to accepts many Users.
            var userPhoneNumber = shipping?.PhoneNumber;
            var credit = companiesCreditRelationshipService.FindCompaniesCreditRelationshipByCustomerIdVendorId(product.CustomerId.Value, product.VendorId.Value);
            PackingSlipViewModel model = null;

            if (taskData.isEnterprise == true)
            {
                var vendor = product.VendorCompany;
                if (vendor == null)
                {
                    return null;
                }
                var vendorEmailAddress = vendor.Users?.FirstOrDefault()?.Email;

                model = new PackingSlipViewModel
                {
                    ProductId = productId,
                    TaskId = taskId,
                    State = (States)taskData.StateId,
                    PartName = product.Name,
                    PartNumber = product.PartNumber,
                    PartDescription = product.Description,
                    PartRevision = product.PartRevision?.Name,
                    ShippingDate = DateTime.UtcNow,
                    Quantity = quantity,
                    Term = credit != null && credit.isTerm ? credit.TermDays : company.Term,
                    CustomerName = product.CustomerCompany.Name,
                    CustomerPONumber = order.CustomerPONumber,
                    VendorId = product.VendorId.Value,
                    CustomerPhoneNumber = userPhoneNumber,
                    Attention = shipping.Attention_FreeText,

                    // bill addr
                    CustomerAddress_AddressLine1 = billAddr.AddressLine1,
                    CustomerAddress_AddressLine2 = billAddr.AddressLine2,
                    CustomerAddress_City = billAddr.City,
                    CustomerAddress_State = billAddr.StateProvince?.Name,
                    CustomerAddress_CountryName = billAddr.Country?.CountryName,
                    CustomerAddress_ZipCode = billAddr.ZipCode,
                    CustomerAddress_PostalCode = billAddr.PostalCode,

                    // shipping addr
                    CustomerShippingAddress_AddressLine1 = shippingAddr.AddressLine1,
                    CustomerShippingAddress_AddressLine2 = shippingAddr.AddressLine2,
                    CustomerShippingAddress_City = shippingAddr.City,
                    CustomerShippingAddress_State = shippingAddr.StateProvince?.Name,
                    CustomerShippingAddress_CountryName = shippingAddr.Country?.CountryName,
                    CustomerShippingAddress_ZipCode = shippingAddr.ZipCode,
                    CustomerShippingAddress_PostalCode = shippingAddr.PostalCode,

                    // vendor addr
                    VendorAddress_AddressLine1 = vendor.Address != null ? vendor.Address.AddressLine1 : vendor.Shipping.Address.AddressLine1,
                    VendorAddress_AddressLine2 = vendor.Address != null ? vendor.Address.AddressLine2 : vendor.Shipping.Address.AddressLine2,
                    VendorAddress_City = vendor.Address != null ? vendor.Address.City : vendor.Shipping.Address.City,
                    VendorAddress_CountryName = vendor.Address != null ? vendor.Address.Country?.CountryName : vendor.Shipping.Address.Country?.CountryName,
                    VendorAddress_State = vendor.Address != null ? vendor.Address.StateProvince?.Name : vendor.Shipping.Address.StateProvince?.Name,
                    VendorAddress_PostalCode = vendor.Address != null ? vendor.Address?.PostalCode : vendor.Shipping.Address.PostalCode,
                    VendorName = vendor.Name,
                    VendorEmailAddress = vendorEmailAddress,
                    VendorPhoneNumber = vendor.Users.FirstOrDefault()?.PhoneNumber,
                    Buyer = order.Buyer,
                    EstimateNumber = invoice.EstimateNumber,
                    OrderDate = order.OrderDate,
                    CompanyLogoUri = vendor.CompanyLogoUri,

                    isEnterprise = taskData.isEnterprise,
                    CarrierName = order.CarrierName ?? "TPC",
                    CarrierType = order.CarrierType?.Humanize() ?? "",

                    ShippingAccount = order.ShippingAccountNumber,
                };
            }
            else
            {
                var admin = CompanyService.FindAllCompanies().FirstOrDefault(x => x.Name == OMNAE_WEB.Admin);
                if (admin == null)
                {
                    return null;
                }
                var adminEmailAddress = admin.Users.FirstOrDefault()?.Email;

                model = new PackingSlipViewModel
                {
                    ProductId = productId,
                    TaskId = taskId,
                    State = (States)taskData.StateId,
                    PartName = product.Name,
                    PartNumber = product.PartNumber,
                    PartDescription = product.Description,
                    PartRevision = product.PartRevision?.Name,
                    ShippingDate = DateTime.UtcNow,
                    Quantity = quantity,
                    Term = credit != null && credit.isTerm ? credit.TermDays : company.Term,
                    CustomerName = product.CustomerCompany.Name,
                    CustomerPONumber = order.CustomerPONumber,
                    VendorId = product.VendorId.Value,
                    CustomerPhoneNumber = userPhoneNumber,
                    Attention = shipping.Attention_FreeText,

                    // bill addr
                    CustomerAddress_AddressLine1 = billAddr.AddressLine1,
                    CustomerAddress_AddressLine2 = billAddr.AddressLine2,
                    CustomerAddress_City = billAddr.City,
                    CustomerAddress_State = billAddr.StateProvince?.Name,
                    CustomerAddress_CountryName = billAddr.Country?.CountryName,
                    CustomerAddress_ZipCode = billAddr.ZipCode,
                    CustomerAddress_PostalCode = billAddr.PostalCode,

                    // shipping addr
                    CustomerShippingAddress_AddressLine1 = shippingAddr.AddressLine1,
                    CustomerShippingAddress_AddressLine2 = shippingAddr.AddressLine2,
                    CustomerShippingAddress_City = shippingAddr.City,
                    CustomerShippingAddress_State = shippingAddr.StateProvince?.Name,
                    CustomerShippingAddress_CountryName = shippingAddr.Country?.CountryName,
                    CustomerShippingAddress_ZipCode = shippingAddr.ZipCode,
                    CustomerShippingAddress_PostalCode = shippingAddr.PostalCode,

                    // admin addr
                    AdminAddress_AddressLine1 = admin.Address.AddressLine1,
                    AdminAddress_AddressLine2 = admin.Address.AddressLine2,
                    AdminAddress_City = admin.Address.City,
                    AdminAddress_CountryName = admin.Address.Country?.CountryName,
                    AdminAddress_State = admin.Address.StateProvince?.Name,
                    AdminAddress_PostalCode = admin.Address.PostalCode,
                    AdminName = Administrator_Account.Name,
                    AdminEmailAddress = adminEmailAddress,

                    Buyer = order.Buyer,
                    EstimateNumber = invoice.EstimateNumber,
                    OrderDate = order.OrderDate,
                    CompanyLogoUri = admin.CompanyLogoUri,

                    isEnterprise = taskData.isEnterprise,
                    CarrierName = order.CarrierName ?? "TPC",
                    CarrierType = order.CarrierType?.Humanize() ?? "",

                    ShippingAccount = order.ShippingAccountNumber,
                };
            }
            return model;
        }

        public string ExportPackingSlipToPdf(PackingSlipViewModel model, ControllerContext controllerContext)
        {
            var fileName = model.State == States.SampleComplete ? $"packing_slip_tid-{model.TaskId}_vid-{model.VendorId}_pid-{model.ProductId}_Sample.pdf"
                                                               : $"packing_slip_tid-{model.TaskId}_vid-{model.VendorId}_pid-{model.ProductId}_Production.pdf";

            var pdf = new ActionAsPdf("PackingSlip", model)
            {
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageMargins = { Left = 5, Right = 5 },
                FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName,
            };
            var docs = DocumentService.FindDocumentByProductId(model.ProductId)
                                      .Where(x => x.DocType == (int)DOCUMENT_TYPE.PACKING_SLIP_PDF);

            int docsCount = docs != null ? docs.Count() + 1 : 1;
            if (docs != null && docs.Count() > 0)
            {
                fileName = Path.GetFileNameWithoutExtension(fileName) + $"_v-{docsCount}" + Path.GetExtension(fileName);
            }

            var pdfBytes = pdf.BuildFile(controllerContext);
            MemoryStream ms = new MemoryStream(pdfBytes);
            var docUri = DocumentStorageService.Upload(ms, fileName);

            var doc = new Document()
            {
                TaskId = model.TaskId,
                Version = docsCount,
                Name = fileName,
                DocUri = docUri,
                DocType = (int)DOCUMENT_TYPE.PACKING_SLIP_PDF,
                UserType = (int)USER_TYPE.Vendor,
                ProductId = model.ProductId,
                UpdatedBy = UserContext.User.UserName,
                CreatedUtc = DateTime.UtcNow,
                ModifiedUtc = DateTime.UtcNow
            };

            DocumentService.AddDocument(doc);

            //if (docs == null)
            //{
            //    DocumentService.AddDocument(doc);
            //}
            //else
            //{
            //    doc.Name = fileName;
            //    doc.Version++;
            //    DocumentService.AddDocument(doc);
            //}

            docUri = DocumentStorageService.AddSecurityTokenToUrl(docUri);
            return docUri;
        }



        private async Task<Tuple<Byte[], string>> CallQBOCreateCustomerInvoice(TaskData taskData, Order order, int numberSampleIncluded)
        {
            // Call QBO for creating Invoice for customer

            InvoiceViewModel model = new InvoiceViewModel();
            var prod = ProductService.FindProductById(taskData.ProductId.Value);
            if (prod == null || prod.CustomerId == null)
            {
                return null;
            }

            var invoice = OmnaeInvoiceService.FindOmnaeInvoiceByCompanyIdByTaskId(prod.CustomerId.Value, taskData.TaskId).Where(x => x.OrderId == order.Id).FirstOrDefault();

            if (invoice == null)
            {
                return null;
            }

            Model.Models.Company customer = taskData.Product.CustomerCompany;
            var shipping = ShippingService.FindShippingByUserId(prod.CustomerId.Value);
            if (shipping.AddressId == null)
            {
                return null;
            }

            var address = AddressService.FindAddressById(shipping.AddressId.Value);

            model.TaskId = taskData.TaskId;
            model.PartNumber = prod.PartNumber;
            model.PartRevision = prod.PartRevision != null ? prod.PartRevision.Name : null;
            model.ProductId = prod.Id;
            model.ProductCategory = prod.Material;
            model.Attention = shipping.Attention_FreeText;
            model.CompanyName = prod.CustomerCompany.Name;
            model.Quantity = order.Quantity;
            model.SalesTax = order.SalesTax ?? 0m;
            model.UnitPrice = order.UnitPrice ?? 0m;
            model.VendorUnitPrice = invoice.UnitPrice;
            if (model.UnitPrice > 0)
            {
                model.Total = model.Quantity * model.UnitPrice;
            }
            else
            {
                model.Total = invoice.ToolingSetupCharges;
            }

            model.AddressLine1 = address.AddressLine1;
            model.AddressLine2 = address.AddressLine2;
            model.City = address.City;
            model.CountryId = address.CountryId;
            model.CountryName = address.Country.CountryName;
            if (address.StateProvinceId != null)
            {
                model.State = address.StateProvince.Abbreviation;
            }
            model.PostalCode = address.PostalCode;
            model.ZipCode = address.ZipCode;
            model.PhoneNumber = shipping.PhoneNumber;
            model.EmailAddress = shipping.EmailAddress;

            model.CustomerId = prod.CustomerId;
            if (prod.CustomerId != null)
            {
                model.CustomerName = prod.CustomerCompany.Name;
            }

            model.VendorId = prod.VendorId;
            if (prod.VendorId != null)
            {
                model.VendorName = prod.VendorCompany.Name;
            }

            model.isBilling = address.isBilling;
            model.isShipping = address.isShipping;
            model.CarrierName = order != null ? order.CarrierName : null;
            model.TrackingNumber = order != null ? order.TrackingNumber : null;
            model.HarmonizedCode = prod.HarmonizedCode;

            model.ProductName = prod.Name.Trim();
            model.ProductDescription = prod.Description != null ? prod.Description.Trim() : null;
            model.EstimateId = invoice.EstimateId;
            model.EstimateNumber = invoice.EstimateNumber;
            model.PONumber = order.CustomerPONumber;
            model.Buyer = order.Buyer;
            model.Term = customer.Term;
            model.QboId = customer.QboId;

            int dueDays = prod.ProductionLeadTime.Value;
            if (prod.CustomerCompany != null && prod.CustomerCompany.Term != null)
            {
                dueDays = prod.CustomerCompany.Term.Value;
            }
            model.DueDate = DateTime.UtcNow.AddDays(dueDays);
            model.ShipDate = order.ShippedDate != null ? order.ShippedDate.Value : DateTime.UtcNow;
            model.NumberSampleIncluded = numberSampleIncluded;

            QboApi qboApi = new QboApi(QboTokensService);
            try
            {
                byte[] data = await qboApi.CreateInvoice(model);
                invoice.InvoiceId = model.InvoiceId;
                invoice.InvoiceNumber = model.InvoiceNumber;
                invoice.InvoiceDate = DateTime.UtcNow;
                invoice.DueDate = DateTime.UtcNow.AddDays(dueDays);

                // Update Invoice to Customer
                OmnaeInvoiceService.UpdateOmnaeInvoice(invoice);

                if (data != null && model.CustomerId != null)
                {
                    //string fileName = string.Format("customer_invoice_cid-{0}_tid-{1}_pid-{2}.pdf", model.CustomerId.Value, model.TaskId, model.ProductId);
                    string fileName = string.Format($"customer_invoice_cid-{model.CustomerId.Value}_tid-{model.TaskId}_pid-{model.ProductId}_oid-{order.Id}.pdf");
                    var docUri = DocumentStorageService.Upload(data, fileName);
                    var doc = new Document()
                    {
                        TaskId = taskData.TaskId,
                        Version = 1,
                        Name = fileName,
                        DocUri = docUri,
                        DocType = (int)DOCUMENT_TYPE.QBO_INVOICE_PDF,
                        UserType = (int)USER_TYPE.Customer,
                        ProductId = model.ProductId,
                        UpdatedBy = UserContext.User.UserName,
                        CreatedUtc = DateTime.UtcNow,
                        ModifiedUtc = DateTime.UtcNow
                    };
                    try
                    {
                        var d = DocumentService.FindDocumentByProductId(model.ProductId).Where(x => x.Name == fileName).FirstOrDefault();
                        if (d == null)
                        {
                            DocumentService.AddDocument(doc);
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
                return new Tuple<byte[], string>(data, model.InvoiceNumber);
            }
            catch
            {
                throw;
            }
        }

        public string NcrDetails(int id, ref NcrDescriptionViewModel model)
        {

            var ncr = NcReportService.FindNCReportById(id);
            if (ncr == null)
            {
                return IndicatingMessages.NotFound;
            }

            var product = ProductService.FindProductById(ncr.ProductId);
            if (product == null)
            {
                return IndicatingMessages.ProductNotFound;
            }

            string customerName = product.CustomerCompany != null ? product.CustomerCompany.Name : null;
            string vendorName = product.VendorCompany != null ? product.VendorCompany.Name : null;

            model = mapper.Map<NcrDescriptionViewModel>(ncr);

            var order = OrderService.FindOrderById(ncr.OrderId);
            model.NCRId = ncr.Id;
            model.ProductId = product.Id;
            model.Customer = customerName;
            model.Vendor = vendorName;
            model.ProductPartNo = product.PartNumber;
            model.PartRevisionNo = product.PartNumberRevision;
            model.ProductDescription = product.Description;
            model.PONumber = order.CustomerPONumber;
            model.ShippingAccountNumber = order.ShippingAccountNumber;
            model.CarrierType = order.CarrierType;
            model.TotalProductQuantity = OrderService.FindOrderByCustomerId(product.CustomerId.Value)
                                                  .Where(o => o.ProductId == product.Id)
                                                  .Sum(q => q.Quantity); ;

            if (ncr.StateId == States.NCRDamagedByCustomer)
            {
                model.ArbitrateCustomerDamageReason = ncr.ArbitrateCustomerCauseReason;
                model.ArbitrateCustomerCauseReason = null;
            }
            else if (ncr.StateId == States.NCRCustomerRevisionNeeded)
            {
                model.ArbitrateCustomerCauseReason = ncr.ArbitrateCustomerCauseReason;
                model.ArbitrateCustomerDamageReason = null;
            }

            TaskSetup.SetupNCRImages(ref model, ncr.Id);

            return null;
        }

        public string AssignCustomerTerms(CompaniesCreditRelationshipViewModel model)
        {
            try
            {
                using (var ts = AsyncTransactionScope.StartNew())
                {
                    CompaniesCreditRelationship entity = mapper.Map<CompaniesCreditRelationship>(model);
                    if (companiesCreditRelationshipService.FindCompaniesCreditRelationshipByCustomerIdVendorId(model.CustomerId, model.VendorId) == null)
                    {
                        companiesCreditRelationshipService.AddCompaniesCreditRelationship(entity);
                    }
                    else
                    {
                        companiesCreditRelationshipService.UpdateCompaniesCreditRelationship(entity);
                    }


                    ts.Complete();
                    return null;
                }
            }
            catch (Exception ex)
            {
                return ex.RetrieveErrorMessage();
            }

        }

        public string AssignTermCreditLimit(CompaniesCreditRelationshipViewModel model)
        {
            using (var trans = AsyncTransactionScope.StartNew())
            {
                try
                {
                    CompaniesCreditRelationship entity = mapper.Map<CompaniesCreditRelationship>(model);
                    entity.isTerm = true;
                    var ent = companiesCreditRelationshipService.FindCompaniesCreditRelationshipByCustomerIdVendorId(entity.CustomerId, entity.VendorId);
                    if (ent != null)
                    {
                        ent.isTerm = entity.isTerm;
                        ent.TermDays = entity.TermDays;
                        ent.CreditLimit = entity.CreditLimit;
                        ent.DiscountDays = entity.DiscountDays;
                        ent.Discount = entity.Discount;
                        ent.Deposit = entity.Deposit;
                        ent.Currency = entity.Currency;
                        ent.ToolingDepositPercentage = entity.ToolingDepositPercentage;
                        ent.TaxPercentage = entity.TaxPercentage;

                        companiesCreditRelationshipService.UpdateCompaniesCreditRelationship(ent);
                    }
                    else
                    {
                        companiesCreditRelationshipService.AddCompaniesCreditRelationship(entity);
                    }
                }
                catch (Exception ex)
                {
                    return ex.RetrieveErrorMessage();
                }
                trans.Complete();
                return null;
            }
        }

        public string RemoveTermCreditLimit(RemoveCreditRelationshipViewModel model)
        {
            using (var trans = AsyncTransactionScope.StartNew())
            {
                try
                {
                    var entity = companiesCreditRelationshipService.FindCompaniesCreditRelationshipByCustomerIdVendorId(model.CustomerId, model.VendorId);
                    if (entity != null)
                    {
                        companiesCreditRelationshipService.DeleteCompaniesCreditRelationship(entity);
                    }
                    else
                    {
                        return "No Term and Credit Limit was found to be assigned to this customer by the vendor!";
                    }
                }
                catch (Exception ex)
                {
                    return ex.RetrieveErrorMessage();
                }
                trans.Complete();
                return null;
            }
        }

        public IQueryable<TaskData> Search(string search, int companyId)
        {
            var model = new List<TaskData>();
            USER_TYPE userType = UserContext.UserType == USER_TYPE.Unknown ? USER_TYPE.Customer : UserContext.UserType;

            var taskDatas = userType == USER_TYPE.Customer
                ? TaskDataService.FindTaskDatasByCustomerId(companyId)
                : TaskDataService.FindTaskDatasByVendorId(companyId);

            search = search.Trim().ToUpper();
            return taskDatas.Where(x => x.Product.PartNumber != null && x.Product.PartNumber.ToUpper().Contains(search)
                                       || x.Product.Name != null && x.Product.Name.ToUpper().Contains(search)
                                       || x.Product.VendorCompany.Name != null && x.Product.VendorCompany.Name.ToUpper().Contains(search));
        }


        public IList<Document> GetDocumentsByProductId(int productId)
        {
            var docs = DocumentService.FindDocumentByProductId(productId);
            return docs;
        }


        public CustomerInfo CreateUsersForQBO(int id)
        {
            CustomerInfo customerInfo = new CustomerInfo();
            try
            {
                Model.Models.Company company = CompanyService.FindCompanyById(id);

                var userEntity = company.Users.OrderBy(u => u.Id).FirstOrDefault();
                //if (userEntity == null)
                //    return null;

                Omnae.Model.Models.Address address = AddressService.FindAddressById(company.AddressId.Value);
                Omnae.Model.Models.Shipping shipping = null;
                if (company.ShippingId != null)
                {
                    shipping = ShippingService.FindShippingById(company.ShippingId.Value);
                }


                StateProvince state = null;
                if (address.StateProvinceId != null)
                {
                    state = StateProvinceService.FindStateProvinceById(address.StateProvinceId.Value);
                }

                // setup CustomerInfo with dummy data
                customerInfo.GivenName = userEntity?.FirstName;
                customerInfo.MiddleName = userEntity?.MiddleName;
                customerInfo.FamilyName = userEntity?.LastName ?? company.Name;

                customerInfo.Taxable = true; // model.Taxable;
                customerInfo.Balance = 0;
                //customerInfo.BalanceWithJobs = 0;
                //customerInfo.Notes = "This is test Notes";
                //customerInfo.Title = "Mr.";
                //customerInfo.webAddr = new WebSiteAddress { URI = "http://www.google.com", Default = true, DefaultSpecified = true };

                customerInfo.CompanyName = company.Name;
                customerInfo.DisplayName = company.Name;
                customerInfo.Term = company.Term;

                var phone = new TelephoneNumber
                {
                    FreeFormNumber = userEntity?.PhoneNumber,
                };

                var email = new EmailAddress
                {
                    Address = userEntity?.Email,
                    Default = userEntity != null ? true : false,
                    DefaultSpecified = userEntity != null ? true : false,
                };

                customerInfo.primaryPhone = phone;
                customerInfo.alternatePhone = phone;
                customerInfo.mobile = phone;

                customerInfo.primaryEmailAddr = email;

                var addr = new Intuit.Ipp.Data.PhysicalAddress
                {
                    Line1 = address.AddressLine1,
                    Line2 = address.AddressLine2,
                    City = address.City,
                    Country = address.Country.CountryName,
                    CountryCode = address.CountryId.ToString(),
                    PostalCode = address.PostalCode ?? address.ZipCode,
                    CountrySubDivisionCode = state?.Abbreviation,
                };
                customerInfo.BillAddr = addr;

                if (address.isBilling)
                {
                    customerInfo.BillAddr = addr;
                }
                if (address.isShipping)
                {
                    customerInfo.ShipAddr = addr;
                }
                else if (shipping != null)
                {
                    var shippingAddr = AddressService.FindAddressById(shipping.AddressId.Value);
                    state = StateProvinceService.FindStateProvinceByCode(shippingAddr.StateProvinceId.Value).FirstOrDefault();
                    customerInfo.ShipAddr = new Intuit.Ipp.Data.PhysicalAddress
                    {
                        Line1 = shippingAddr.AddressLine1,
                        Line2 = shippingAddr.AddressLine2,
                        City = shippingAddr.City,
                        Country = shippingAddr.Country.CountryName,
                        CountryCode = shippingAddr.CountryId.ToString(),
                        PostalCode = shippingAddr.PostalCode ?? shippingAddr.ZipCode,
                        CountrySubDivisionCode = state?.Abbreviation,
                    };
                }
                customerInfo.CurrencyText = Enum.GetName(typeof(CurrencyCodes), company.Currency);
            }
            catch
            {
                throw;
            }
            return customerInfo;
        }


        public CustomerInfo CreateUsersForQBO(int id, CurrencyCodes currencyCode, int? term)
        {
            CustomerInfo customerInfo = new CustomerInfo();
            try
            {
                Model.Models.Company company = CompanyService.FindCompanyById(id);

                var userEntity = company.Users.OrderBy(u => u.Id).FirstOrDefault();
                //if (userEntity == null)
                //    return null;
       

                Omnae.Model.Models.Address address = AddressService.FindAddressById(company.AddressId.Value);
                Omnae.Model.Models.Shipping shipping = null;
                if (company.ShippingId != null)
                {
                    shipping = ShippingService.FindShippingById(company.ShippingId.Value);
                }


                StateProvince state = null;
                if (address.StateProvinceId != null)
                {
                    state = StateProvinceService.FindStateProvinceById(address.StateProvinceId.Value);
                }

                // setup CustomerInfo with dummy data
                customerInfo.GivenName = userEntity?.FirstName;
                customerInfo.MiddleName = userEntity?.MiddleName;
                customerInfo.FamilyName = userEntity?.LastName ?? company.Name;

                customerInfo.Taxable = true; // model.Taxable;
                customerInfo.Balance = 0;
                //customerInfo.BalanceWithJobs = 0;
                //customerInfo.Notes = "This is test Notes";
                //customerInfo.Title = "Mr.";
                //customerInfo.webAddr = new WebSiteAddress { URI = "http://www.google.com", Default = true, DefaultSpecified = true };

                customerInfo.CompanyName = company.Name;
                customerInfo.DisplayName = company.Name;
                customerInfo.Term = term;

                var phone = new TelephoneNumber
                {
                    FreeFormNumber = userEntity?.PhoneNumber,
                };


                var email = new EmailAddress
                {
                    Address = userEntity?.Email,
                    Default = userEntity != null ? true : false,
                    DefaultSpecified = userEntity != null ? true : false,
                };

                customerInfo.primaryPhone = phone;
                customerInfo.alternatePhone = phone;
                customerInfo.mobile = phone;

                customerInfo.primaryEmailAddr = email;

                var addr = new Intuit.Ipp.Data.PhysicalAddress
                {
                    Line1 = address.AddressLine1,
                    Line2 = address.AddressLine2,
                    Line3 = address.StateProvince?.Name,
                    City = address.City,
                    Country = address.Country.CountryName,
                    CountryCode = address.CountryId.ToString(),
                    PostalCode = address.PostalCode ?? address.ZipCode,
                    CountrySubDivisionCode = state?.Abbreviation,
                };
                customerInfo.BillAddr = addr;

                if (address.isBilling)
                {
                    customerInfo.BillAddr = addr;
                }
                if (address.isShipping)
                {
                    customerInfo.ShipAddr = addr;
                }
                else if (shipping != null)
                {
                    var shippingAddr = AddressService.FindAddressById(shipping.AddressId.Value);
                    state = StateProvinceService.FindStateProvinceByCode(shippingAddr.StateProvinceId.Value).FirstOrDefault();
                    customerInfo.ShipAddr = new Intuit.Ipp.Data.PhysicalAddress
                    {
                        Line1 = shippingAddr.AddressLine1,
                        Line2 = shippingAddr.AddressLine2,
                        City = shippingAddr.City,
                        Country = shippingAddr.Country.CountryName,
                        CountryCode = shippingAddr.CountryId.ToString(),
                        PostalCode = shippingAddr.PostalCode ?? shippingAddr.ZipCode,
                        CountrySubDivisionCode = state?.Abbreviation,
                    };
                }
  //              customerInfo.CurrencyName = Enum.GetName(typeof(CurrencyCodes), company.Currency); // ToDo: find the name of the CurrencyCodes
                customerInfo.CurrencyText = Enum.GetName(typeof(CurrencyCodes), currencyCode);
            }
            catch
            {
                throw;
            }
            return customerInfo;
        }


        public CustomerInfo AddCustomerQBO(CreateCustomerInfoViewModel model)
        {
            CustomerInfo customerInfo = new CustomerInfo();
            try
            {
                Model.Models.Company company = CompanyService.FindCompanyById(model.CompanyId);
                Omnae.Model.Models.Address address = AddressService.FindAddressById(company.AddressId.Value);
                Omnae.Model.Models.Shipping shipping = null;
                if (company.ShippingId != null)
                {
                    shipping = ShippingService.FindShippingById(company.ShippingId.Value);
                }


                StateProvince state = null;
                if (address.StateProvinceId != null)
                {
                    state = StateProvinceService.FindStateProvinceById(address.StateProvinceId.Value);
                }

                // setup CustomerInfo with dummy data
                customerInfo.GivenName = model.FirstName;
                //customerInfo.MiddleName = userEntity.MiddleName;
                //customerInfo.FamilyName = userEntity.LastName;

                customerInfo.Taxable = true; // model.Taxable;
                customerInfo.Balance = 0;
                //customerInfo.BalanceWithJobs = 0;
                //customerInfo.Notes = "This is test Notes";
                //customerInfo.Title = "Mr.";
                //customerInfo.webAddr = new WebSiteAddress { URI = "http://www.google.com", Default = true, DefaultSpecified = true };

                customerInfo.CompanyName = company.Name;
                customerInfo.DisplayName = company.Name;
                customerInfo.Term = company.Term;

                var phone = new TelephoneNumber
                {
                    FreeFormNumber = model.PhoneNumber,
                };

                var email = new EmailAddress
                {
                    Address = model.Email,
                    Default = true,
                    DefaultSpecified = true,
                };

                customerInfo.primaryPhone = phone;
                customerInfo.alternatePhone = phone;
                customerInfo.mobile = phone;

                customerInfo.primaryEmailAddr = email;

                var addr = new Intuit.Ipp.Data.PhysicalAddress
                {
                    Line1 = address.AddressLine1,
                    Line2 = address.AddressLine2,
                    City = address.City,
                    Country = address.Country.CountryName,
                    CountryCode = address.CountryId.ToString(),
                    PostalCode = address.PostalCode ?? address.ZipCode,
                    CountrySubDivisionCode = state?.Abbreviation,
                };
                customerInfo.BillAddr = addr;

                if (address.isBilling)
                {
                    customerInfo.BillAddr = addr;
                }
                if (address.isShipping)
                {
                    customerInfo.ShipAddr = addr;
                }
                else if (shipping != null)
                {
                    var shippingAddr = AddressService.FindAddressById(shipping.AddressId.Value);
                    state = StateProvinceService.FindStateProvinceByCode(shippingAddr.StateProvinceId.Value).FirstOrDefault();
                    customerInfo.ShipAddr = new Intuit.Ipp.Data.PhysicalAddress
                    {
                        Line1 = shippingAddr.AddressLine1,
                        Line2 = shippingAddr.AddressLine2,
                        City = shippingAddr.City,
                        Country = shippingAddr.Country.CountryName,
                        CountryCode = shippingAddr.CountryId.ToString(),
                        PostalCode = shippingAddr.PostalCode ?? shippingAddr.ZipCode,
                        CountrySubDivisionCode = state?.Abbreviation,
                    };
                }
            }
            catch
            {
                throw;
            }
            return customerInfo;
        }

        public CustomerInfo CreateUsersForQBO(CustomerInfoViewModel vm)
        {
            CustomerInfo customerInfo = new CustomerInfo();
            Model.Models.Company company = CompanyService.FindCompanyById(vm.Id);
            Omnae.Model.Models.Address address = AddressService.FindAddressById(company.AddressId.Value);
            Omnae.Model.Models.Shipping shipping = null;
            if (company.ShippingId != null)
            {
                shipping = ShippingService.FindShippingById(company.ShippingId.Value);
            }


            StateProvince state = null;
            if (address.StateProvinceId != null)
            {
                state = StateProvinceService.FindStateProvinceById(address.StateProvinceId.Value);
            }

            // setup CustomerInfo with dummy data
            customerInfo.GivenName = vm.FirstName;
            customerInfo.MiddleName = vm.MiddleName;
            customerInfo.FamilyName = vm.LastName;

            customerInfo.Taxable = true; // model.Taxable;
            customerInfo.Balance = 0;
            //customerInfo.BalanceWithJobs = 0;
            //customerInfo.Notes = "This is test Notes";
            //customerInfo.Title = "Mr.";
            //customerInfo.webAddr = new WebSiteAddress { URI = "http://www.google.com", Default = true, DefaultSpecified = true };

            customerInfo.CompanyName = company.Name;
            customerInfo.DisplayName = company.Name;
            customerInfo.Term = company.Term;

            var phone = new TelephoneNumber
            {
                FreeFormNumber = vm.PhoneNumber,
            };

            var email = new EmailAddress
            {
                Address = vm.EmailAddress,
                Default = true,
                DefaultSpecified = true,
            };

            customerInfo.primaryPhone = phone;
            customerInfo.alternatePhone = phone;
            customerInfo.mobile = phone;

            customerInfo.primaryEmailAddr = email;

            var addr = new Intuit.Ipp.Data.PhysicalAddress
            {
                Line1 = address.AddressLine1,
                Line2 = address.AddressLine2,
                City = address.City,
                Country = address.Country.CountryName,
                CountryCode = address.CountryId.ToString(),
                PostalCode = address.PostalCode ?? address.ZipCode,
                CountrySubDivisionCode = state?.Abbreviation,
            };
            customerInfo.BillAddr = addr;

            if (address.isBilling)
            {
                customerInfo.BillAddr = addr;
            }
            if (address.isShipping)
            {
                customerInfo.ShipAddr = addr;
            }
            else if (shipping != null)
            {
                var shippingAddr = AddressService.FindAddressById(shipping.AddressId.Value);
                state = StateProvinceService.FindStateProvinceByCode(shippingAddr.StateProvinceId.Value).FirstOrDefault();
                customerInfo.ShipAddr = new Intuit.Ipp.Data.PhysicalAddress
                {
                    Line1 = shippingAddr.AddressLine1,
                    Line2 = shippingAddr.AddressLine2,
                    City = shippingAddr.City,
                    Country = shippingAddr.Country.CountryName,
                    CountryCode = shippingAddr.CountryId.ToString(),
                    PostalCode = shippingAddr.PostalCode ?? shippingAddr.ZipCode,
                    CountrySubDivisionCode = state?.Abbreviation,
                };
            }

            return customerInfo;
        }

        public async System.Threading.Tasks.Task CreateUserInQBO(int id)
        {
            QboApi qboApi = new QboApi(QboTokensService);

            // create Customer on QBO
            CustomerInfo customerInfo = CreateUsersForQBO(id);
            string qboId = await qboApi.CreateCustomer(customerInfo);

            // update Company table by insert this qbo id
            var company = CompanyService.FindCompanyById(id);
            company.QboId = qboId;
            CompanyService.UpdateCompany(company);
        }

        public async System.Threading.Tasks.Task CreateUserInQBO(CustomerInfoViewModel vm)
        {
            QboApi qboApi = new QboApi(QboTokensService, Log.ForContext<QboApi>());

            // create Customer on QBO
            var customerInfo = CreateUsersForQBO(vm);
            string qboId = await qboApi.CreateCustomer(customerInfo);

            // update Company table by insert this qbo id
            var company = CompanyService.FindCompanyById(vm.Id);
            company.QboId = qboId;
            CompanyService.UpdateCompany(company);
        }
    }
}
