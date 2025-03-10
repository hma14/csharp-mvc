using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using System.Web.Mvc;
using Common;
using Humanizer;
using Libs.Notification;
using Omnae.BusinessLayer.Interface;
using Omnae.BusinessLayer.Models;
using Omnae.BusinessLayer.Services;
using Omnae.Common;
using Omnae.Data;
using Omnae.Libs.Notification;
using Omnae.Libs.ViewModel;
using Omnae.Model.Context;
using Omnae.Model.Extentions;
using Omnae.Model.Models;
using Omnae.Service.Service;
using Omnae.Service.Service.Interfaces;
using Serilog;

namespace Omnae.BusinessLayer
{
    public class TaskDatasBL
    {
        private ILogedUserContext UserContext { get; }
        private IBidRequestRevisionService bidRequestRevisionService { get; }
        private TimerTriggerService timerTriggerService { get; }
        private ITaskDataService taskDataService { get; }
        // private NotificationService NotificationService { get; }
        private ITimerSetupService timerSetupService { get; }
        private IProductService productService { get; }
        private UserContactService userContactService { get; }
        private IPriceBreakService priceBreakService { get; }
        private NotificationBL notificationBL { get; }
        private readonly IRFQBidService rfqBidService;
        private readonly IPartRevisionService partRevisionService;
        private readonly NotificationService notificationService;
        private readonly ICompanyService companyService;
        private readonly IOrderService orderService;
        private readonly IOmnaeInvoiceService omnaeInvoiceService;
        private readonly INCReportService ncReportService;
        private readonly IOrderStateTrackingService orderStateTrackingService;
        private readonly IBidRFQStatusService bidRFQStatusService;
        private readonly IVendorBidRFQStatusService vendorBidRFQStatusService;
        private readonly ICompaniesCreditRelationshipService companiesCreditRelationshipService;
        private ILogger Log { get; }
        private readonly IProductPriceQuoteService productPriceQuoteService;

        public TaskDatasBL(ILogedUserContext UserContext, ITaskDataService taskDataService, IBidRequestRevisionService bidRequestRevisionService,
            TimerTriggerService timerTriggerService, ITimerSetupService timerSetupService, IProductService productService, UserContactService userContactService,
            IPriceBreakService priceBreakService, NotificationBL notificationBL, IRFQBidService rfqBidService, IPartRevisionService partRevisionService,
            NotificationService notificationService, ICompanyService companyService, IOrderService orderService, IOmnaeInvoiceService omnaeInvoiceService,
            INCReportService ncReportService, IOrderStateTrackingService orderStateTrackingService, ILogger log,
            IBidRFQStatusService bidRFQStatusService, IVendorBidRFQStatusService vendorBidRFQStatusService, ICompaniesCreditRelationshipService companiesCreditRelationshipService,
             IProductPriceQuoteService productPriceQuoteService)
        {
            this.taskDataService = taskDataService;
            this.UserContext = UserContext;
            this.bidRequestRevisionService = bidRequestRevisionService;
            this.timerTriggerService = timerTriggerService;
            this.timerSetupService = timerSetupService;
            this.productService = productService;
            this.userContactService = userContactService;
            this.priceBreakService = priceBreakService;
            this.notificationBL = notificationBL;
            this.rfqBidService = rfqBidService;
            this.partRevisionService = partRevisionService;
            this.notificationService = notificationService;
            this.companyService = companyService;
            this.orderService = orderService;
            this.omnaeInvoiceService = omnaeInvoiceService;
            this.ncReportService = ncReportService;
            this.orderStateTrackingService = orderStateTrackingService;
            Log = log;
            this.bidRFQStatusService = bidRFQStatusService;
            this.vendorBidRFQStatusService = vendorBidRFQStatusService;
            this.companiesCreditRelationshipService = companiesCreditRelationshipService;
            this.productPriceQuoteService = productPriceQuoteService;
        }


        private async Task CollectRevisionReasonsOnTimeOut(TaskData customerTask, List<int> vendorIds, List<BidRequestRevision> brrList)
        {
            var productId = customerTask.ProductId.Value;
            var timer = timerSetupService.FindTimerSetupByProductIdTimerType(productId, TypeOfTimers.RFQRevisionTimer);
            if (timer != null)
            {
                timerTriggerService.RemoveTimerTrigger(timer.Name);
            }
            States newState = States.BidForRFQ;

            if (brrList.Any())
            {
                newState = States.PendingRFQRevision;
            }

            var allTasks = taskDataService.FindTaskDataListByProductId(productId).Where(x => x.StateId != (int)States.VendorRejectedRFQ);
            foreach (var td in allTasks)
            {
                td.StateId = (int)newState;
                td.UpdatedBy = UserContext.User?.UserName;
                td.ModifiedByUserId = UserContext.UserId;
                td.ModifiedUtc = DateTime.UtcNow;
                taskDataService.Update(td);
            }

            //customerTask.StateId = (int)newState;
            //customerTask.UpdatedBy = UserContext.User?.UserName;
            //customerTask.ModifiedUtc = DateTime.UtcNow;
            //customerTask.ModifiedByUserId = UserContext.UserId;
            //taskDataService.Update(customerTask);

            // Update stateid in [BidRFQStatus] table
            var bidRFQStatus = bidRFQStatusService.FindBidRFQStatusListByProductId(customerTask.ProductId.Value).LastOrDefault();
            bidRFQStatus.StateId = (int)newState;
            bidRFQStatus.SubmittedVendors = vendorIds.Count;
            bidRFQStatus.ModifiedByUserId = UserContext.UserId;
            bidRFQStatusService.UpdateBidRFQStatus(bidRFQStatus);

            //if (newState == States.BidForRFQ)
            //{
            //    // Start Bid Timer here
            //    var interval = timerSetupService.FindAllTimerSetupsByProductIdTimerType(productId, TypeOfTimers.BidTimer)
            //        .ToList()
            //        .LastOrDefault()?.Interval;

            //    if (string.IsNullOrEmpty(interval) == false)
            //    {
            //        SetupTimer(productId, interval, TypeOfTimers.BidTimer);
            //    }
            //}

            // Notify
            var customer = customerTask.Product.CustomerCompany; //productService.FindProductById(productId).CustomerCompany;
            if (customer != null)
            {
                var users = userContactService.GetAllActiveUserConnectFromCompany(customer);
                foreach (var contactInformation in users)
                {
                    await notificationBL.SendNotificationsAsync(customerTask, contactInformation.Email, contactInformation.PhoneNumber);
                }
            }
        }


        public async Task<string> RevisionRequestTimeoutHandler(int productId, List<int> vendorIds)
        {
            Log.Information($"RevisionRequestTimeoutHandler -> ProductId:{productId}");
            var allTasks = taskDataService.FindTaskDataListByProductId(productId).Where(x => x.StateId != (int)States.VendorRejectedRFQ);
            var anyPositive = allTasks.Count(x => x.StateId == (int)States.PendingRFQRevision || x.StateId == (int)States.ReviewRFQAccepted);
            Log.Information($"RevisionRequestTimeoutHandler -> anyPositive:{anyPositive}");
            if (anyPositive == 0)
            {
                foreach (var td in allTasks)
                {
                    td.StateId = (int)States.BidForRFQ;
                    td.ModifiedByUserId = UserContext.UserId;
                    td.ModifiedUtc = DateTime.UtcNow;
                    taskDataService.Update(td);
                }

                // Start Bid Timer here
                var interval = timerSetupService.FindAllTimerSetupsByProductIdTimerType(productId, TypeOfTimers.BidTimer)
                    .ToList()
                    .LastOrDefault()?.Interval;
                if (string.IsNullOrEmpty(interval))
                    return IndicatingMessages.TimerIntervalCouldnotBeFound;

                var error = SetupTimer(productId, interval, TypeOfTimers.BidTimer);
                if (error != null)
                    return error;
            }
            else
            {
                var customerTask = allTasks.FirstOrDefault(x => x.RFQBidId == null);
                var brrList = bidRequestRevisionService.FindBidRequestRevisionListByProductId(productId);
                await CollectRevisionReasonsOnTimeOut(customerTask, vendorIds, brrList);

            }
            return null;
        }

        public async Task BidTimeoutHandler(int productId)
        {
            string destination = string.Empty;
            string destinationSms = string.Empty;

            // trigger Admin(reseller mode) or customer (SAAS mode) to review bid

            var tasks = taskDataService.FindTaskDataListByProductId(productId).Where(x => x.StateId != (int)States.VendorRejectedRFQ);
            var pb = priceBreakService.FindPriceBreakByProductId(productId).GroupBy(g => g.RFQBidId).Select(x => x.First()).ToList();

            foreach (var td in tasks)
            {
                if (pb.Count == 0)
                {
                    td.StateId = (int)States.BidTimeout;
                }
                else if (pb.Count(x => x.TaskId == td.TaskId) == 0 && td.RFQBidId != null)
                {
                    td.StateId = (int)States.BidTimeout;
                }
                else if (td.RFQBidId == null)
                {
                    // set customer state to BidReview
                    td.StateId = (int)States.BidReview;
                }
                else
                {
                    continue;
                }
                td.UpdatedBy = UserContext.User?.UserName;
                td.ModifiedUtc = DateTime.UtcNow;
                td.ModifiedByUserId = UserContext.UserId;
                taskDataService.Update(td);

                if (td.RFQBidId != null)
                {
                    // Now store to VendorBidRFQStatus table
                    var status = bidRFQStatusService.FindBidRFQStatusListByProductId(productId).LastOrDefault();
                    var vendorBidRFQStatus = new VendorBidRFQStatus
                    {
                        ProductId = productId,
                        TaskId = td.TaskId,
                        VendorId = td.RFQBid.VendorId,
                        StateId = td.StateId,
                        CreatedByUserId = UserContext.UserId,
                        BidRFQStatusId = status?.Id,
                    };
                    var vendorBidRFQStatusId = vendorBidRFQStatusService.AddVendorBidRFQStatus(vendorBidRFQStatus);
                }

                // Notify
                {
                    var users = td.RFQBidId != null ?
                        userContactService.GetAllActiveUserConnectFromCompany(td.RFQBid.VendorId) :
                        userContactService.GetAllActiveUserConnectFromCompany(td.Product.CustomerId.Value);

                    foreach (var contactInformation in users)
                    {
                        await notificationBL.SendNotificationsAsync(td, contactInformation.Email, contactInformation.PhoneNumber);
                    }
                }
            }

            // Update stateid in [BidRFQStatus] table
            var bidRFQStatus = bidRFQStatusService.FindBidRFQStatusListByProductId(productId).LastOrDefault();
            bidRFQStatus.StateId = tasks.Where(x => x.RFQBidId == null).FirstOrDefault().StateId;
            bidRFQStatus.SubmittedVendors = tasks.Count(x => x.StateId == (int)States.BidReview && x.RFQBidId != null);
            bidRFQStatus.TotalVendors = tasks.Count(x => x.RFQBidId != null);
            bidRFQStatus.ModifiedByUserId = UserContext.UserId;
            bidRFQStatusService.UpdateBidRFQStatus(bidRFQStatus);

            var triggerName = $"BidTimerWillExpire-p_{productId}";
            var bidTimer = $"{ConfigurationManager.AppSettings["BidTimer"]}-p_{productId}";
            timerTriggerService.RemoveTimerTrigger(triggerName);
            timerTriggerService.RemoveTimerTrigger(bidTimer);
        }

        public async Task BidWillExpireSoonHandler(int productId, bool removeTheTrigger = true)
        {
            var timer = timerSetupService.FindTimerSetupByProductIdTimerType(productId, TypeOfTimers.BidTimer);
            if (timer == null || timer.TimerStartAt == null)
            {
                Log.Warning("Invalid Timer, No valid timer was found for this ProductId:{productId}", productId);
                return;
            }

            var bitTimerInt = int.Parse(timer.Interval);
            var bidDate = timer.TimerStartAt.Value.AddDays(bitTimerInt); //Todo, Check if is days or other Unit in the Timmer Data
            var timeToExpire = (DateTime.UtcNow - bidDate).Humanize();

            var notificatedVendors = new HashSet<int>();
            var notificatedEmails = new HashSet<string>();

            var tasks = taskDataService.FindTaskDataListByProductId(productId).Where(td => td.StateId == (int)States.BidForRFQ && td.RFQBidId != null).ToList();
            Log.Information("Found {Qnt} TaskData to notify that the BidWillExpireSoonHandler", tasks.Count);
            if (tasks.Count == 0)
            {
                Log.Warning("No TaskData was found ready to bid (BidForRFQ) for this ProductId:{productId}", productId);
                return;
            }

            foreach (var td in tasks)
            {
                var vendorId = td.RFQBid?.VendorId;
                if (vendorId == null)
                    continue;
                if (notificatedVendors.Contains(vendorId.Value))
                    continue;

                Log.Information("Sending BidWillExpireSoonHandler notification to the VendorId:{Vendor}", vendorId);

                // Notify
                var users = userContactService.GetAllActiveUserConnectFromCompany(vendorId.Value);
                foreach (var contactInformation in users)
                {
                    if (notificatedEmails.Contains(contactInformation.Email))
                        continue;

                    await notificationBL.SendBidWillExpireNotificationsAsync(td, contactInformation.FullName, contactInformation.Email, timeToExpire);
                    notificatedEmails.Add(contactInformation.Email);
                }

                notificatedVendors.Add(vendorId.Value);
            }

            if (removeTheTrigger)
            {
                var triggerName = $"BidTimerWillExpire-p_{productId}";
                timerTriggerService.RemoveTimerTrigger(triggerName);
                Log.Information("Removing the trigger from ATrigger. {Name}", triggerName);
            }
        }

        public string SetupTimer_Reseller(SetupTimerViewModel model)
        {
            using (var trans = AsyncTransactionScope.StartNew())
            {
                var bidTimerTriggerName = $"{ConfigurationManager.AppSettings["BidTimer"]}-p_{model.ProductId}";
                var bidTimerWillExpireTriggerName = $"BidTimerWillExpire-p_{model.ProductId}";

                try
                {
                    var bitTimer = int.Parse(model.BidTimerInterval);

                    timerTriggerService.StartTimer(bidTimerWillExpireTriggerName, TimerUnit.Day, Math.Max(bitTimer - 1, 0), model.ProductId, "CheckTimeoutAlertBidWillExpire");
                    timerTriggerService.StartTimer(bidTimerTriggerName, TimerUnit.Day, bitTimer, model.ProductId, "CheckTimeoutForBid");
                }
                catch (Exception ex)
                {
                    return ex.RetrieveErrorMessage();
                }

                // only add timer info for RevisionRequest timer to database TimerSetup table
                string revisionRequestTimer = $"{ConfigurationManager.AppSettings["RevisionRequestTimer"]}-p_{model.ProductId}";
                var timer = new TimerSetup()
                {
                    ProductId = model.ProductId,
                    Name = revisionRequestTimer,
                    Unit = TIMER_UNIT.DAY,
                    Interval = model.RevisionRequestTimerInterval,
                    CallbackMethod = "CheckTimeoutForBidRevisionRequest",
                    TimerType = TypeOfTimers.RFQRevisionTimer, // this record is reserved for Revision Timer if there is Revision request.
                };
                timerSetupService.AddTimerSetup(timer);

                var td = taskDataService.FindById(model.TaskId);
                td.StateId = (int)States.BidForRFQ;
                taskDataService.Update(td);

                trans.Complete();
                return null;
            }
        }

        public string SetupTimer(SetupTimerViewModel model)
        {
            using (var trans = AsyncTransactionScope.StartNew())
            {              
                string triggerName = $"{ConfigurationManager.AppSettings["RevisionRequestTimer"]}-p_{model.ProductId}";
                string callback = "CheckTimeoutForBidRevisionRequest";
                var interval = model.Interval != null ? int.Parse(model.Interval) : int.Parse(model.RevisionRequestTimerInterval);
                var strTimerUnit = ConfigurationManager.AppSettings["MyTimerUnit"];
                var timerUnit = (TimerUnit)int.Parse(strTimerUnit); // TimerUnit.Day
                if (model.TimerType == TypeOfTimers.BidTimer)
                {
                    interval = model.Interval != null ? int.Parse(model.Interval) : int.Parse(model.BidTimerInterval);
                    triggerName = $"{ConfigurationManager.AppSettings["BidTimer"]}-p_{model.ProductId}";
                    var timerToBeExpiredTriggerName = $"BidTimerWillExpire-p_{model.ProductId}";
                    var timerToBeExpiredCallback = "CheckTimeoutAlertBidWillExpire";
                    callback = "CheckTimeoutForBid";
                    timerTriggerService.StartTimer(timerToBeExpiredTriggerName, timerUnit, Math.Max(interval - 1, 0), model.ProductId, timerToBeExpiredCallback);
                }
                try
                {
                    timerTriggerService.StartTimer(triggerName, timerUnit, interval, model.ProductId, callback);

                    var timer = new TimerSetup()
                    {
                        ProductId = model.ProductId,
                        Name = triggerName,
                        Unit = timerUnit == TimerUnit.Day ? TIMER_UNIT.DAY : TIMER_UNIT.MINUTE,
                        Interval = model.Interval ?? model.RevisionRequestTimerInterval,
                        CallbackMethod = callback,
                        TimerStartAt = DateTime.UtcNow,
                        TimerType = model.TimerType,
                    };
                    // add to TimerSetup
                    timerSetupService.AddTimerSetup(timer);

                    if (model.BidTimerInterval != null)
                    {
                        triggerName = $"{ConfigurationManager.AppSettings["BidTimer"]}-p_{model.ProductId}";
                        callback = "CheckTimeoutForBid";
                        timer = new TimerSetup()
                        {
                            ProductId = model.ProductId,
                            Name = triggerName,
                            Unit = timerUnit == TimerUnit.Day ? TIMER_UNIT.DAY : TIMER_UNIT.MINUTE,
                            Interval = model.BidTimerInterval,
                            CallbackMethod = callback,
                            TimerType = TypeOfTimers.BidTimer,
                        };

                        // Store Bid Timer interval to TimerSetup
                        timerSetupService.AddTimerSetup(timer);
                    }
                }
                catch (Exception ex)
                {
                    return ex.RetrieveErrorMessage();
                }

                if (model.isEnterprise == false)
                {
                    var td = taskDataService.FindById(model.TaskId);
                    td.StateId = (int)States.BidForRFQ;
                    taskDataService.Update(td);
                }
                trans.Complete();
                return null;
            }
        }

        public string SetupTimer(int pid, string interval, TypeOfTimers type, ActionFlag? flag = null)
        {
            using (var ts = AsyncTransactionScope.StartNew())
            {
                var tds = taskDataService.FindTaskDataListByProductId(pid)
                    .Where(x => !(x.StateId == (int)States.VendorRejectedRFQ || x.StateId == (int)States.RFQFailed));

                foreach (var td in tds)
                {
                    if (td.StateId == (int)States.RFQReviewUpdateQuantity || td.StateId == (int)States.RFQBidUpdateQuantity)
                        continue;

                    if (td.IsRFQBid() ||
                        td.StateId == (int)States.BidTimeout && flag == ActionFlag.ExtendTimeLimit)
                    {
                        td.StateId = (int)States.BidForRFQ;
                    }
                    else if (td.IsRFQReview() ||
                             td.StateId == (int)States.BidTimeout && flag == ActionFlag.AssignNewVendors && td.RFQBidId == null)
                    {
                        td.StateId = (int)States.ReviewRFQ;
                    }

                    td.UpdatedBy = UserContext.User?.UserName;
                    td.ModifiedByUserId = UserContext.UserId;
                    td.ModifiedUtc = DateTime.UtcNow;
                    taskDataService.Update(td);
                }
                var customerTask = tds.FirstOrDefault(x => x.RFQBidId == null);
                if (customerTask == null)
                {
                    var product = productService.FindProductById(pid); 
                    if (product.ParentPartRevisionId != null) 
                    {
                        customerTask = tds.FirstOrDefault();;
                    }
                    else
                        return IndicatingMessages.TaskNotFound;
                }

                var setupTimerVM = new SetupTimerViewModel
                {
                    ProductId = pid,
                    TaskId = customerTask.TaskId,
                    isEnterprise = customerTask.isEnterprise,
                    Interval = interval,
                    TimerType = type,
                };

                ts.Complete();
                return SetupTimer(setupTimerVM);
            }
        }

#if false
        public string SetupRFQRevisionTimer(SetupTimerViewModel model)
        {
            using (var trans = AsyncTransactionScope.StartNew())
            {
                // Add timer info for RevisionRequest timer to database TimerSetup table
                string revisionRequestTimer = $"{ConfigurationManager.AppSettings["RevisionRequestTimer"]}-p_{model.ProductId}";
                var timer = timerSetupService.FindTimerSetupByProductId(model.ProductId);
                var callback = "CheckTimeoutForBidRevisionRequest";
                DateTime utcNow = DateTime.UtcNow;
                if (timer == null)
                {
                    timer = new TimerSetup()
                    {
                        ProductId = model.ProductId,
                        Name = revisionRequestTimer,
                        Unit = TIMER_UNIT.DAY,
                        Interval = model.Interval,
                        CallbackMethod = callback,
                        TimerStartAt = utcNow,
                        IsExpired = false,
                    };
                    // add to TimerSetup
                    timerSetupService.AddTimerSetup(timer);
                }
                else
                {
                    timer.Interval = model.Interval ?? timer.Interval;
                    timer.TimerStartAt = utcNow;
                    timer.IsExpired = false;

                    // update TimerSetup
                    timerSetupService.UpdateTimerSetup(timer);
                }

                // Start RFQRevision Timer now
                try
                {
                    var interval = int.Parse(model.Interval);
                    timerTriggerService.StartTimer(revisionRequestTimer, TimerUnit.Day, interval, model.ProductId, callback);
                }
                catch (Exception ex)
                {
                    return ex.RetrieveErrorMessage();
                }
                trans.Complete();
                return null;
            }
        }
#endif

        public string ReviewBid(bool selected, int vendorId, int productId, int? ReasonIndex)
        {
            using (var ts = AsyncTransactionScope.StartNew())
            {
                RFQBid bid = rfqBidService.FindRFQBidByVendorIdProductId(vendorId, productId);
                if (bid == null)
                {
                    return $"ReviewBid: Bid {IndicatingMessages.NotFound}";
                }

                TaskData td = taskDataService.FindTaskDataByRFQBidId(bid.Id);

                // if bidVendorId == vendorId -> this vendor win the bid
                if (selected)
                {
                    td.ProductId = productId;

                    Product product = productService.FindProductById(productId);
                    product.AdminId = UserContext.UserId;
                    product.PriceBreakId = priceBreakService.FindLowestUnitPricePricePriceBreakId(productId, bid.Id);
                    product.ModifiedByUserId = UserContext.UserId;

                    // set IsActive in RFQBids table
                    bid.IsActive = true;
                    bid.QuoteAcceptDate = DateTime.UtcNow;
                    bid.ModifiedByUserId = UserContext.UserId;

                    rfqBidService.UpdateRFQBid(bid);

                    td.StateId = (int)States.QuoteAccepted;
                    td.UpdatedBy = UserContext.User.UserName;
                    td.ModifiedUtc = DateTime.UtcNow;
                    td.ModifiedByUserId = UserContext.UserId;

                    // Check old task to be removed to find its task ID and remove this task later
                    var otd = taskDataService.FindTaskDataListByProductId(productId).FirstOrDefault(x => x.RFQBidId == null);
                    if (otd != null && (otd.Orders == null || otd.Orders.Count == 0))
                    {
                        td.IsRiskBuild = otd.IsRiskBuild;
                        taskDataService.Update(td);
                        var partRevision = partRevisionService.FindPartRevisionByTaskId(otd.TaskId);
                        if (partRevision != null)
                        {
                            // update taskId in PartRevision table to this taskId 
                            partRevision.TaskId = td.TaskId;
                            partRevision.ModifiedByUserId = UserContext.UserId;
                            partRevisionService.UpdatePartRevision(partRevision);
                        }
                        if (td.isEnterprise == true && (otd.NCReports == null || otd.NCReports.Count == 0))
                        {
                            taskDataService.DeleteTaskData(otd);
                        }
                    }

                    // if this is Enterprise mode, check tasks for other vendors and set those 
                    // tasks state to RFQBidComplete and set BidFailedReason
                    if (td.isEnterprise)
                    {
                        var tds = taskDataService.FindTaskDataListByProductId(productId)
                            .Where(x => x.RFQBidId != null &&
                                        x.RFQBid.VendorId != vendorId &&
                                        !(x.StateId == (int)States.BidTimeout || x.StateId == (int)States.VendorRejectedRFQ));
                        foreach (var task in tds)
                        {
                            task.RFQBid.IsActive = false;
                            // task.RFQBid.BidFailedReason = BidFailedReason.Reasons[8]; // ToDo: don't show the failed reason for new
                            rfqBidService.UpdateRFQBid(task.RFQBid);
                            task.StateId = (int)States.RFQBidComplete;
                            taskDataService.Update(task);
                        }
                    }

                    var pricebreaks = priceBreakService.FindPriceBreakByTaskIdProductId(td.TaskId, productId);
                    if (pricebreaks == null || pricebreaks.Count == 0)
                    {
                        return $"ReviewBid: {IndicatingMessages.PriceBreaksNotFound}";
                    }

                    // Update Lead times, tooling charges for Product table
                    int shippingDays = pricebreaks.Last().ShippingDays.Value;
                    product.SampleLeadTime = bid.SampleLeadTime + shippingDays + 2;
                    product.ProductionLeadTime = bid.ProductLeadTime + shippingDays + 2;
                    product.HarmonizedCode = bid.HarmonizedCode;
                    product.ToolingSetupCharges = bid.ToolingCharge;
                    product.VendorId = vendorId;

                    productService.UpdateProduct(product);

                    if (td.isEnterprise)
                    {
                        // if customer has relationship with selected vendor then store vendor's PreferredCurrency to the [CompaniesCreditRelationships] table
                        var ent = companiesCreditRelationshipService.FindCompaniesCreditRelationshipByCustomerIdVendorId(product.CustomerId.Value, vendorId);
                        if (ent != null)
                        {
                            ent.Currency = bid.PreferredCurrency ?? ent.Currency;
                            companiesCreditRelationshipService.UpdateCompaniesCreditRelationship(ent);
                        }
                        else
                        {
                            // create a new relationship in CompaniesCreditRelationships table
                            var companiesCreditRelationship = new CompaniesCreditRelationship
                            {
                                CustomerId = (int)product.CustomerId,
                                VendorId = vendorId,
                                isTerm = false,
                                //TermDays = termDays,
                                //CreditLimit = creditLimit,
                                //DiscountDays = discountDays,
                                //Discount = discount,
                                //Deposit = deposit,
                                Currency = bid.PreferredCurrency != null ? bid.PreferredCurrency.Value : CurrencyCodes.CAD,
                                //ToolingDepositPercentage = toolingDeposit,
                            };
                            companiesCreditRelationshipService.AddCompaniesCreditRelationship(companiesCreditRelationship);
                        }

                        // Update this ProductPriceQuote to active
                        var productPriceQuote = productPriceQuoteService.FindProductPriceQuotes(product.Id, vendorId).ToList().LastOrDefault();
                        if (productPriceQuote != null)
                        {
                            productPriceQuote.IsActive = true;
                            productPriceQuoteService.UpdateProductPriceQuote(productPriceQuote);
                        }
                    }

                    //Notify
                    var users = userContactService.GetAllActiveUserConnectFromCompany(vendorId);
                    foreach (var user in users)
                    {
                        var destination = user.Email;
                        var destinationSms = user.PhoneNumber;

                        string subject = $"Omnae.com RFQ {product.PartNumber}, {product.Description} has been assigned to vendor: {destination}";
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
                            notificationService.NotifyStateChange(States.BidReviewed, subject, destination, destinationSms, entity);
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
                else
                {
                    bid.IsActive = false;
                    bid.BidFailedReason = BidFailedReason.Reasons[ReasonIndex.Value];
                    rfqBidService.UpdateRFQBid(bid);

                    td.StateId = (int)States.RFQBidComplete;
                    td.UpdatedBy = UserContext.User.UserName;
                    td.ModifiedUtc = DateTime.UtcNow;
                    td.ModifiedByUserId = UserContext.UserId;

                    taskDataService.Update(td);
                }
                ts.Complete();
                return null;
            }
        }

        public int NumberVendorResponsed(int productId, bool includedRFQFailed = false)
        {
            return taskDataService.FindTaskDataListByProductId(productId)
                    .Where(x => x.RFQBidId != null)
                    //.Count(x => x.StateId != (int)States.ReviewRFQ);
                    .Count(c => c.StateId == (int)States.PendingRFQRevision ||
                           c.StateId == (int)States.BidReview ||
                           c.StateId == (int)States.BackFromRFQ ||
                           c.StateId == (int)States.RFQBidComplete ||
                           c.StateId == (int)States.RFQNoResponse ||
                           c.StateId == (int)States.VendorRejectedRFQ ||
                           c.StateId == (int)States.ReviewRFQAccepted ||
                           c.StateId == (int)States.BidTimeout ||
                           c.StateId == (int)States.RFQFailed && includedRFQFailed == true);
        }

        public bool IsAllVendorsResponsed(int productId, int numberVendors)
        {
            return numberVendors == NumberVendorResponsed(productId, true);
        }

        public async Task<int> CheckIfAllVendorsResponsed(TaskData td)
        {
            var productId = td.ProductId.Value;
            List<int> vendorIds = rfqBidService.FindRFQBidListByProductId(productId)
                .Select(x => x.VendorId).ToList();

            if (IsAllVendorsResponsed(td.ProductId.Value, vendorIds.Count) || td.Product.VendorId != null)
            {
                var numberPendingRFQRevision = taskDataService.FindTaskDataListByProductId(td.ProductId.Value)
                                                              .Where(x => x.RFQBidId != null)
                                                              .Count(c => c.StateId == (int)States.PendingRFQRevision || c.StateId == (int)States.BackFromRFQ);
                if (numberPendingRFQRevision > 0)
                {
                    await RevisionRequestTimeoutHandler(td.ProductId.Value, vendorIds);
                    return -1;
                }
                else
                {
                    var allTasks = taskDataService.FindTaskDataListByProductId(productId);
                    var activeTasks = allTasks.Where(x => !(x.StateId == (int)States.VendorRejectedRFQ || x.StateId == (int)States.BidTimeout));
                    var rejected = allTasks.Where(x => x.StateId == (int)States.VendorRejectedRFQ);
                    if (rejected.Count() == vendorIds.Count)
                    {
                        foreach (var task in allTasks)
                        {
                            task.StateId = (int)States.RFQFailed;
                            task.UpdatedBy = UserContext.User.UserName;
                            task.ModifiedByUserId = UserContext.UserId;
                            task.ModifiedUtc = DateTime.UtcNow;
                            taskDataService.Update(task);
                        }
                    }
                    else
                    {
                        if (td.isEnterprise)
                        {
                            foreach (var task in activeTasks)
                            {
                                task.StateId = (int)States.BidReview;
                                task.UpdatedBy = UserContext.User.UserName;
                                task.ModifiedByUserId = UserContext.UserId;
                                task.ModifiedUtc = DateTime.UtcNow;
                                taskDataService.Update(task);
                            }
                        }
                    }

                    var revisionRequestTimer = $"{ConfigurationManager.AppSettings["RevisionRequestTimer"]}-p_{td.ProductId.Value}";
                    var triggerName = $"BidTimerWillExpire-p_{td.ProductId.Value}";
                    var bidTimer = $"{ConfigurationManager.AppSettings["BidTimer"]}-p_{td.ProductId.Value}";
                    timerTriggerService.RemoveTimerTrigger(revisionRequestTimer);
                    timerTriggerService.RemoveTimerTrigger(triggerName);
                    timerTriggerService.RemoveTimerTrigger(bidTimer);
                }
            }
            return 0;
        }


        public async Task<string> CancelRFQ(int Id)
        {
            using (var trans = AsyncTransactionScope.StartNew())
            {
                var td = taskDataService.FindById(Id);
                if (td == null || td.ProductId == null)
                {
                    return IndicatingMessages.TaskNotFound;
                }
                // Notify to customer of cancel RFQ action.
                var customer = td.Product.CustomerCompany;
                if (customer != null)
                {
                    var users = userContactService.GetAllActiveUserConnectFromCompany(customer);
                    td.StateId = (int)States.VendorCancelledRFQ;
                    foreach (var contactInformation in users)
                    {
                        await notificationBL.SendNotificationsAsync(td, contactInformation.Email, contactInformation.PhoneNumber);
                    }
                }
                int productId = td.ProductId.Value;
                if (td.Product.VendorId != null)
                {
                    var product = td.Product;
                    product.VendorId = null;
                    productService.UpdateProduct(product);
                }

                if (td.RFQBidId != null)
                {
                    var rfqBid = td.RFQBid;
                    taskDataService.DeleteTaskData(td);
                    rfqBidService.DeleteRFQBid(rfqBid);
                }
                else
                {
                    td.StateId = (int)States.PendingRFQ;
                    taskDataService.Update(td);
                }
                var originalTask = taskDataService.FindTaskDataListByProductId(productId).Where(x => x.RFQBidId == null).FirstOrDefault();
                if (originalTask != null)
                {
                    await CheckIfAllVendorsResponsed(originalTask);
                }
                trans.Complete();
                return null;
            }
        }

        public string ResetRFQ(int productId, TaskData td, string userName, string userId)
        {
            using (var trans = AsyncTransactionScope.StartNew())
            {
                try
                {
                    var timer = timerSetupService.FindTimerSetupByProductIdTimerType(productId, TypeOfTimers.RFQRevisionTimer);
                    if (timer != null)
                    {
                        var setupTimerVM = new SetupTimerViewModel
                        {
                            ProductId = productId,
                            TaskId = td.TaskId,
                            isEnterprise = td.isEnterprise,
                            Interval = timer.Interval,
                            TimerType = TypeOfTimers.RFQRevisionTimer,
                        };

                        // Reset Bid timer
                        var error = SetupTimer(setupTimerVM);
                        if (error != null)
                        {
                            throw new Exception("SetupTimers: " + error);
                        }


                        // reset task state to PendingRFQ for selecting new vendors
                        td.StateId = (int)States.PendingRFQ;
                        td.UpdatedBy = userName;
                        td.ModifiedByUserId = userId;
                        taskDataService.Update(td);

                        // set vendor's task to BidTimeout
                        var tds = taskDataService.FindTaskDataListByProductId(productId).Where(x => x.RFQBidId != null);
                        foreach (var t in tds)
                        {
                            t.StateId = (int)States.BidTimeout;
                            t.UpdatedBy = userName;
                            t.ModifiedByUserId = userId;
                            t.RFQBid.IsActive = false;
                            t.RFQBid.ModifiedByUserId = userId;
                            rfqBidService.UpdateRFQBid(t.RFQBid);
                            taskDataService.Update(t);
                        }
                    }
                    else
                    {
                        return IndicatingMessages.NotFound;
                    }
                }
                catch
                {
                    throw;
                }

                trans.Complete();
                return null;
            }
        }
    }
}