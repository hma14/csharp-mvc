using Common;
using Libs.Notification;
using Microsoft.Azure;
using Omnae.BlobStorage;
using Omnae.BusinessLayer.Identity.Model;
using Omnae.BusinessLayer.Models;
using Omnae.BusinessLayer.Services;
using Omnae.Common;
using Omnae.Data;
using Omnae.Data.Query;
using Omnae.Libs.ViewModel;
using Omnae.Model.Context;
using Omnae.Model.Extentions;
using Omnae.Model.Models;
using Omnae.Model.Models.Aspnet;
using Omnae.Model.ViewModels;
using Omnae.QuickBooks.QBO;
using Omnae.Service.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using Omnae.Libs.Notification;
using static Omnae.Data.Query.CompanyQuery;
using Omnae.Service.Service;
using RazorEngine.Compilation.ImpromptuInterface;

namespace Omnae.BusinessLayer
{
    public class ProductBL
    {
        private IRFQBidService RfqBidService { get; }
        private IPriceBreakService PriceBreakService { get; }
        private IExtraQuantityService ExtraQuantityService { get; }
        private ILogedUserContext UserContext { get; }
        private IStoredProcedureService SpService { get; }
        private IDocumentService DocumentService { get; }
        private ITaskDataService TaskDataService { get; }
        private ICountryService CountryService { get; }
        private IOrderService OrderService { get; }
        private ICompanyService CompanyService { get; }
        private IOrderStateTrackingService OrderStateTrackingService { get; }
        private IProductStateTrackingService ProductStateTrackingService { get; }
        private IPartRevisionService PartRevisionService { get; }
        private IShippingAccountService ShippingAccountService { get; }
        private TaskDataCustomerBL TaskDataCustomerBl { get; }
        private IQboTokensService QboTokensService { get; }
        private IProductService ProductService { get; }
        private IImageStorageService ImageStorageService { get; }

        private ChartBL ChartBl { get; }
        private ApplicationDbContext DbUser { get; }
        private NotificationService NotificationService { get; }
        private IRFQQuantityService RfqQuantityService { get; }
        private IApprovedCapabilityService ApprovedCapabilityService { get; }
        private NotificationBL NotificationBL { get; }
        private IProductSharingService ProductSharingService { get; }
        private readonly ICompaniesCreditRelationshipService companiesCreditRelationshipService;
        private readonly ITimerSetupService timerSetupService;
        private readonly TaskDatasBL taskDataBL;
        private readonly TimerTriggerService timerTriggerService;
        private readonly IBidRFQStatusService bidRFQStatusService;
        private readonly IVendorBidRFQStatusService vendorBidRFQStatusService;
        private readonly IRFQActionReasonService rfqActionReasonService;


        public ProductBL(IRFQBidService rfqBidService, IPriceBreakService priceBreakService, IExtraQuantityService extraQuantityService, ILogedUserContext userContext, IStoredProcedureService spService, IDocumentService documentService, ITaskDataService taskDataService, TaskDataCustomerBL taskDataCustomerBl, ICountryService countryService, IOrderService orderService, ICompanyService companyService, IOrderStateTrackingService orderStateTrackingService, IProductStateTrackingService productStateTrackingService, IPartRevisionService partRevisionService, IShippingAccountService shippingAccountService, IQboTokensService qboTokensService, IProductService productService, ChartBL chartBl, ApplicationDbContext dbUser, NotificationService notificationService, IRFQQuantityService rfqQuantityService, IApprovedCapabilityService approvedCapabilityService, NotificationBL notificationBl, IProductSharingService productSharingService, IImageStorageService imageStorageService,
            ICompaniesCreditRelationshipService companiesCreditRelationshipService,
            ITimerSetupService timerSetupService, TaskDatasBL taskDataBL, TimerTriggerService timerTriggerService,
            IBidRFQStatusService bidRFQStatusService, IVendorBidRFQStatusService vendorBidRFQStatusService,
            IRFQActionReasonService rfqActionReasonService)
        {
            RfqBidService = rfqBidService;
            PriceBreakService = priceBreakService;
            ExtraQuantityService = extraQuantityService;
            UserContext = userContext;
            SpService = spService;
            DocumentService = documentService;
            TaskDataService = taskDataService;
            TaskDataCustomerBl = taskDataCustomerBl;
            CountryService = countryService;
            OrderService = orderService;
            CompanyService = companyService;
            OrderStateTrackingService = orderStateTrackingService;
            ProductStateTrackingService = productStateTrackingService;
            PartRevisionService = partRevisionService;
            ShippingAccountService = shippingAccountService;
            QboTokensService = qboTokensService;
            ProductService = productService;
            ChartBl = chartBl;
            DbUser = dbUser;
            NotificationService = notificationService;
            RfqQuantityService = rfqQuantityService;
            ApprovedCapabilityService = approvedCapabilityService;
            NotificationBL = notificationBl;
            this.ProductSharingService = productSharingService;
            ImageStorageService = imageStorageService;
            this.companiesCreditRelationshipService = companiesCreditRelationshipService;
            this.timerSetupService = timerSetupService;
            this.taskDataBL = taskDataBL;
            this.timerTriggerService = timerTriggerService;
            this.bidRFQStatusService = bidRFQStatusService;
            this.vendorBidRFQStatusService = vendorBidRFQStatusService;
            this.rfqActionReasonService = rfqActionReasonService;
        }

        public ProductDetailsViewModel SetupProductDetailsVM(TaskData td, int? orderId = null)
        {
            var rfqBid = td.RFQBid;
            int sampleLeadTime = td.Product?.SampleLeadTime ?? 0;
            int prodLeadTime = td.Product?.ProductionLeadTime ?? 0;

            var numberSampleIncluded = PriceBreakService.FindPriceBreakByTaskIdProductId(td.TaskId, td.ProductId.Value)
                .FirstOrDefault(x => x.NumberSampleIncluded != null)
                ?.NumberSampleIncluded;
            var extraQuantityPartDetailsViewModel = GetExtraQuantityPartDetails(td.Product);

            var order = orderId != null ? OrderService.FindOrderById(orderId.Value) : null;

            var model = new ProductDetailsViewModel
            {
                SampleLeadTime = sampleLeadTime,
                ProdLeadTime = prodLeadTime,
                NumberSampleIncluded = numberSampleIncluded ?? 1,
                ExtraQtyPartDetailsVM = extraQuantityPartDetailsViewModel,
                Product = td.Product,
                Order = order,
            };

            return model;
        }

        [NonAction]
        private ExtraQuantityPartDetailsViewModel GetExtraQuantityPartDetails(Product product)
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

        // GET: Products
        public List<ProductListViewModel> NewOrderList()
        {
            var userId = UserContext.UserId;
            var param1 = new SqlParameter("@UserId", userId);
            var param2 = new SqlParameter("@IsNewOrder", 3);
            List<ProductListViewModel> prodList = SpService.SpGetUserProducts("spGetUserProducts @UserId, @IsNewOrder", new[] { param1, param2 });
            return prodList;
        }

        // GET: Products
        public List<ProductListViewModel> ReOrderList()
        {
            var userId = UserContext.UserId;
            var param1 = new SqlParameter("@UserId", userId);
            var param2 = new SqlParameter("@IsNewOrder", 3);
            List<ProductListViewModel> prodList = SpService.SpGetUserProducts("spGetUserProducts @UserId, @IsNewOrder", new[] { param1, param2 });
            return prodList;
        }

        public async Task<PlaceOrderViewModel> StartOrderAsync(int? Id, bool isReorder, int? revisionId = null)
        {
            var taskData = TaskDataService.FindById(Id.Value);
            if (taskData == null)
            {
                throw new BLException(IndicatingMessages.TaskNotFound);
            }

            var prods = TaskDataCustomerBl.GetCustomerProductsOnTaskId(Id.Value);
            if (prods == null)
            {
                throw new BLException(IndicatingMessages.InvalidAccess);
            }

            Product product = null;
            if (revisionId != null)
            {
                product = prods.FirstOrDefault(x => x.PartRevisionId == revisionId.Value);
                if (product == null)
                {
                    throw new BLException(IndicatingMessages.ProductNotFound);
                }
            }
            else
            {
                if (taskData.Product == null)
                {
                    throw new BLException(IndicatingMessages.ProductNotFound);
                }

                product = taskData.Product;
            }

            var currentUserId = UserContext.UserId;
            var company = CompanyService.FindCompanyByUserId(currentUserId);

            List<OrderStateTracking> orderStates = new List<OrderStateTracking>();

            var orders = OrderService.FindOrderByTaskId(taskData.TaskId);
            if (orders != null && orders.Count > 0)
            {
                orderStates = OrderStateTrackingService.FindOrderStateTrackingListByOrderId(orders.Last().Id);
            }
            List<ProductStateTracking> productStates = ProductStateTrackingService.FindProductStateTrackingListByProductId(product.Id).OrderBy(x => x.ModifiedUtc).ToList();
            StateLastUpdatedViewModel stateLastUpdated = new StateLastUpdatedViewModel
            {
                StateId = (States)taskData.StateId,
                LastUpdated = taskData.ModifiedUtc,
            };

            int originProductId = 0;
            if (product.PartRevisionId != null && product.PartRevisionId > 0)
            {
                var partRevision = PartRevisionService.FindPartRevisionById(product.PartRevisionId.Value);
                originProductId = partRevision?.OriginProductId ?? product.Id;
            }

            PlaceOrderViewModel model = null;
            string taxRatePercentage = company.isEnterprise ? "0%" : await GetSalesTaxPercentage(company.Address);

            SelectList shippingAccountDdl = null;
            if (taskData.isEnterprise)
            {
                var shippingAccounts = ShippingAccountService.FindShippingAccountByCompanyId(company.Id)
                    .Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString(),
                    }).ToList();

                shippingAccountDdl = new SelectList(shippingAccounts, "Value", "Text");
            }
            List<PriceBreak> pbs = new List<PriceBreak>();
            if (taskData.RFQBidId != null)
            {
                pbs = PriceBreakService.FindPriceBreakByTaskIdProductId(taskData.TaskId, product.Id).Where(x => x.VendorUnitPrice > 0).ToList();
                List<PriceBreak> pBreaksForExtraQty = new List<PriceBreak>();

                if (pbs == null || pbs.Count == 0)
                {
                    pbs = PriceBreakService.FindPriceBreakByProductId(product.Id).Where(x => x.VendorUnitPrice > 0).ToList();
                }

                if (product.ExtraQuantityId != null)
                {
                    var qtys = ExtraQuantityService.FindExtraQuantityById(product.ExtraQuantityId.Value);
                    List<decimal> quantities = new List<decimal>();
                    if (qtys.Qty1 != null)
                    {
                        quantities.Add(qtys.Qty1.Value);
                    }
                    if (qtys.Qty2 != null)
                    {
                        quantities.Add(qtys.Qty2.Value);
                    }
                    if (qtys.Qty3 != null)
                    {
                        quantities.Add(qtys.Qty3.Value);
                    }
                    if (qtys.Qty4 != null)
                    {
                        quantities.Add(qtys.Qty4.Value);
                    }
                    if (qtys.Qty5 != null)
                    {
                        quantities.Add(qtys.Qty5.Value);
                    }
                    if (qtys.Qty6 != null)
                    {
                        quantities.Add(qtys.Qty6.Value);
                    }
                    if (qtys.Qty7 != null)
                    {
                        quantities.Add(qtys.Qty7.Value);
                    }

                    foreach (var qty in quantities)
                    {
                        PriceBreak pb = pbs.Where(q => q.Quantity == qty).Select(x => x).FirstOrDefault();
                        if (pb != null && pb.UnitPrice > 0)
                        {
                            pb.ToolingSetupCharges = qtys.ToolingSetupCharges;
                            pBreaksForExtraQty.Add(pb);
                        }
                    }
                }
                var numberSampleIncluded = pbs.First().NumberSampleIncluded != null ? pbs.First().NumberSampleIncluded.Value : 1;
                model = new PlaceOrderViewModel
                {
                    Product = product,
                    ProductId = product.Id,
                    TaskId = Id.Value,
                    PriceBreaks = (from pb in pbs
                                   where pb.UnitPrice > 0
                                   select new PBreaks
                                   {
                                       Quantity = pb.Quantity,
                                       UnitPrice = pb.UnitPrice,
                                       ToolingSetupCharges = pb.CustomerToolingSetupCharges != null ? pb.CustomerToolingSetupCharges : pb.ToolingSetupCharges,
                                   }).ToList(),

                    CountryId = company.Address.CountryId,
                    ProvinceId = company.Address.StateProvinceId != null ? company.Address.StateProvinceId.Value : (int?)null,
                    StateId = taskData.StateId,
                    OrderStateTrackings = orderStates,
                    ProductStateTrackings = productStates,
                    LastUpdated = stateLastUpdated,
                    IsReorder = isReorder,
                    PartRevisions = PartRevisionService.FindPartRevisionByProductId(originProductId),
                    TaxRatePercentage = taxRatePercentage,
                    isEnterprise = taskData.isEnterprise,
                    ShippingAccountDDL = shippingAccountDdl,
                    NumberSampleIncluded = numberSampleIncluded,
                    ToolingCharges = pbs.First().CustomerToolingSetupCharges,
                    OrderCompanyId = company.Id,
                };
            }
            else
            {
                var pbs2 = PriceBreakService.FindPriceBreakByProductId(product.Id);
                pbs = pbs2;
                if (isReorder == false)
                {
                    pbs = pbs2.Where(x => x.TaskId == taskData.TaskId && x.RFQBid?.IsActive == true).ToList();
                    if (pbs.Count == 0)
                    {
                        pbs = pbs2.Where(x => x.TaskId == taskData.TaskId).ToList();
                    }                 
                }
                var numberSampleIncluded = pbs.Where(x => x.NumberSampleIncluded != null)?.FirstOrDefault()?.NumberSampleIncluded ?? (pbs2?.Where(x => x.NumberSampleIncluded != null)?.FirstOrDefault()?.NumberSampleIncluded ?? 1);
                model = new PlaceOrderViewModel
                {
                    Product = product,
                    ProductId = product.Id,
                    TaskId = Id.Value,
                    PriceBreaks = (from pb in pbs.Where(x => x.UnitPrice != null)
                                   select new PBreaks
                                   {
                                       Quantity = pb.Quantity,
                                       UnitPrice = pb.UnitPrice,
                                       ToolingSetupCharges = pb.CustomerToolingSetupCharges,
                                   }).ToList(),
                    CountryId = company.Address.CountryId,
                    ProvinceId = company.Address.StateProvinceId,
                    StateId = taskData.StateId,
                    OrderStateTrackings = orderStates,
                    ProductStateTrackings = productStates,
                    LastUpdated = stateLastUpdated,
                    IsReorder = isReorder,
                    PartRevisions = PartRevisionService.FindPartRevisionByProductId(originProductId),
                    TaxRatePercentage = taxRatePercentage,
                    isEnterprise = taskData.isEnterprise,
                    ShippingAccountDDL = shippingAccountDdl,
                    NumberSampleIncluded = numberSampleIncluded,
                    ToolingCharges = pbs.First().CustomerToolingSetupCharges,
                    OrderCompanyId = company.Id,
                };
            }

            // create a dropdown list to show payment methods, when Term is available for a customer
            // don't show other methods except Term. If Term is not available to this customer, 
            // dropdown list show everything except Term.
            List<SelectListItem> PaymentMethods = new List<SelectListItem>();
            PaymentMethods.Add(new SelectListItem() { Text = "Payment Methods", Value = "0" });
            if (company.Term > 0)
            {
                PaymentMethods.Add(new SelectListItem() { Text = "Term", Value = "2" });
            }
            else
            {
                PaymentMethods.Add(new SelectListItem() { Text = "Credit Card", Value = "1" });
                PaymentMethods.Add(new SelectListItem() { Text = "Cheque", Value = "3" });
                PaymentMethods.Add(new SelectListItem() { Text = "Wire", Value = "4" });
            }
            model.PaymentMethod = int.Parse(PaymentMethods.First().Value);
            model.PaymentMethods = new SelectList(PaymentMethods, "Value", "Text");

            if (model.IsReorder)
            {
                model.ToolingCharges = 0;
            }
            return model;
        }

        public async Task<CalculateResultViewModel> CalculateAsync(int? pid, int? TaskId, decimal? Quantity, bool? isReorder, double? taxRate = null, bool? isNewWorkFlow = null)
        {
            if (pid == null || TaskId == null)
            {
                throw new BLException("No product pid or task pid can be found");
            }

            TaskData td = TaskDataService.FindById(TaskId.Value);
            var moq = PriceBreakService.FindMinimumOrderQuantity(pid.Value);
            if (moq == null)
                throw new BLException(IndicatingMessages.PriceBreaksNotFound);

            if (isNewWorkFlow == null)
            {
                if (Quantity != null && Quantity > 0 && moq != null && Quantity < moq.Value)
                {
                    throw new BLException($"Quantity you entered is smaller than MOQ (Minimum Order Quantity = {moq}). Please enter a quantity that must be equal or bigger than MOQ ({moq})!");
                }
            }
            decimal? unitPrice = null;

            PriceBreak pb = null;
            if (Quantity == 0)
            {
                pb = PriceBreakService.FindPriceBreakByTaskId(TaskId.Value).LastOrDefault();
            }
            else if (Quantity <= moq.Value)
            {
                pb = PriceBreakService.FindPriceBreakByProductIdQty(pid.Value, moq.Value);
            }
            else
            {
                pb = PriceBreakService.FindPriceBreakByProductIdQty(pid.Value, Quantity.Value);
            }

            if (pb == null)
            {
                pb = PriceBreakService.FindPriceBreakByProductIdQty(pid.Value, Quantity.Value);
                if (pb == null)
                {
                    if (td.RFQBidId == null)
                    {
                        var taskData = TaskDataService.FindTaskDataListByProductId(pid.Value).FirstOrDefault(x => x.RFQBidId != null);
                        if (taskData != null)
                        {
                            pb = PriceBreakService.FindClosestPriceBreakByProductIdRFQBidIdQty(pid.Value, Quantity.Value, taskData.RFQBidId);
                        }
                        else if (td.Product.ExtraQuantityId == null)
                        {
                            pb = PriceBreakService.FindPriceBreakByProductId(pid.Value).Where(x => x.RFQBidId > 0 && x.Quantity <= Quantity.Value).OrderBy(x => x.Quantity).LastOrDefault();
                        }
                    }
                    else
                    {
                        pb = PriceBreakService.FindClosestPriceBreakByProductIdRFQBidIdQty(pid.Value, Quantity.Value, td.RFQBidId);
                    }
                }
            }
            if (pb != null)
            {
                unitPrice = pb.UnitPrice;
            }
            else
            {
                pb = PriceBreakService.FindPriceBreakByProductId(pid.Value).Where(x => x.UnitPrice > 0 && x.Quantity <= Quantity).OrderBy(x => x.Quantity).LastOrDefault();
                if (pb != null)
                {
                    unitPrice = pb.UnitPrice;
                }
                else
                {
                    unitPrice = 0m;
                }
            }
            decimal? toolingSetupCharges = null;
            if (isReorder == null || isReorder.Value == false)
            {
                toolingSetupCharges = pb?.CustomerToolingSetupCharges;
            }

            //var subTotal = unitPrice.Value * Quantity.Value + (toolingSetupCharges != null ? toolingSetupCharges.Value : 0);
            var productionCharge = unitPrice.Value * Quantity.Value;
            decimal salesTax = 0m;
            decimal tooling = 0m;
            var currentUserId = UserContext.UserId;
            var company = CompanyService.FindCompanyByUserId(currentUserId);

            if (!td.isEnterprise && taxRate == null)
            {
                taxRate = 0d;
                Address addr = company.Shipping.Address;
                if (addr.StateProvince != null)
                {
                    taxRate = (double)await GetTaxRateValue(addr.Country.CountryName, addr.StateProvince.Abbreviation);
                    taxRate /= 100;
                }
            }

            if (toolingSetupCharges != null)
            {
                tooling = toolingSetupCharges.Value;
            }

            decimal subTotal = Quantity > 0 ? productionCharge + tooling : tooling;
            salesTax = (decimal)(taxRate ?? 0) * subTotal;
            decimal total = subTotal + salesTax;

            var model = new CalculateResultViewModel
            {
                UnitPrice = Quantity > 0 ? unitPrice.Value : 0m,
                ToolingCharges = tooling,
                ProductionCharge = productionCharge,
                Subtotal = subTotal,
                SalesTax = salesTax,
                Total = total,
                TaxRate = taxRate,
                TaxRatePercentage = $"{taxRate * 100}%", // await GetSalesTaxPercentage(company.Address),
            };

            return model;
        }

        public decimal? GetUnitPriceOnQty(int pid, int Quantity)
        {
            var moq = PriceBreakService.FindMinimumOrderQuantity(pid);
            if (moq != null && Quantity < moq.Value)
            {
                throw new BLException($"Quantity you entered is smaller than MOQ (Minimum Order Quantity = {moq}). Please enter a quantity that must be equal or bigger than MOQ ({moq})!");
            }
            var pb = PriceBreakService.FindPriceBreakByProductIdQty(pid, Quantity);
            return pb?.UnitPrice;
        }

        public NewPartResultModel CreatePartRevision(CreatePartRevisionViewModel model)
        {
            using (var trans = AsyncTransactionScope.StartNew())
            {
                int partRevisionId = 0;
                int newProductId = 0;
                int originProductId = model.ProductId;
                var product = ProductService.FindProductById(model.ProductId);              
                var parentTask = TaskDataService.FindTaskDataByProductId(originProductId);
                try
                {
                    var parentPartNumberRevision = product.PartNumberRevision;
                    var parentPartRevisionId = product.PartRevisionId;
                    product.Description = model.PartRevisionDesc;
                    product.PartNumberRevision = model.PartRevision;
                    product.ParentPartRevisionId = parentPartRevisionId;
                    product.ParentPartNumberRevision = parentPartNumberRevision;
                    product.CreatedDate = DateTime.UtcNow;
                    if (product.VendorId == null)
                    {
                        var rfqBid = RfqBidService.FindRFQBidListByProductId(model.ProductId).LastOrDefault();
                        if (model.VendorId != null)
                        {
                            product.VendorId = model.VendorId;
                        }
                        else if (rfqBid != null)
                        {
                            
                            product.VendorId = rfqBid.VendorId;
                        }
                        else
                        {
                            product.VendorId = parentTask.Product.VendorId;
                        }
                    }

                    // in case this product RFQQuantityId is null - some old data
                    if (product.RFQQuantityId == null)
                    {
                        var qtys = PriceBreakService.FindPriceBreakByProductId(product.Id).Select(x => x.Quantity).ToArray();
                        var rfqQuantity = new RFQQuantity()
                        {
                            Qty1 = qtys[0],
                            Qty2 = qtys.Length >= 2 ? qtys[1] : (decimal?)null,
                            Qty3 = qtys.Length >= 3 ? qtys[2] : (decimal?)null,
                            Qty4 = qtys.Length >= 4 ? qtys[3] : (decimal?)null,
                            Qty5 = qtys.Length >= 5 ? qtys[4] : (decimal?)null,
                            Qty6 = qtys.Length >= 6 ? qtys[5] : (decimal?)null,
                            Qty7 = qtys.Length == 7 ? qtys[6] : (decimal?)null,
                            IsAddedExtraQty = false,
                        };

                        product.RFQQuantityId = RfqQuantityService.AddRFQQuantity(rfqQuantity);
                    }

                    Product pd = product;
                    if (model.NewAvatar != null)
                    {
                        var postedFile = model.NewAvatar;
                        var fileName = FileUtil.MakeValidFileName($"Avatar-Vendor-{product.VendorId}-PartRevision-Part-{product.Id}-{Guid.NewGuid()}{Path.GetExtension(postedFile.FileName)}");
                        pd.AvatarUri = ImageStorageService.Upload(postedFile, fileName);
                    }

                    // Create a new product with this new PartRevision
                    pd.Id = 0;
                    pd.PriceBreakId = null;

                    newProductId = ProductService.AddProduct(pd);
                }
                catch (Exception ex)
                {
                    throw new BLException(ex.RetrieveErrorMessage());
                }

                // Create a new TaskData to link this new product revision

                TaskData taskData = new TaskData()
                {
                    StateId = (int)States.BidForRFQ,
                    ProductId = newProductId,
                    CreatedUtc = DateTime.UtcNow,
                    ModifiedUtc = DateTime.UtcNow,
                    UpdatedBy = UserContext.User.UserName,
                    isEnterprise = parentTask.isEnterprise,
                    CreatedByUserId = UserContext.UserId,
                    ModifiedByUserId = UserContext.UserId,
                    IsRiskBuild = false,
                };
                var taskId = TaskDataService.AddTaskData(taskData);
                var taskState = (States)taskData.StateId;

                // Add this Part Revision to PartRevision table
                var partRevision = new PartRevision()
                {
                    TaskId = taskId,
                    OriginProductId = originProductId,
                    Name = model.PartRevision,
                    Description = model.PartRevisionDesc,
                    StateId = States.BidForRFQ, //States.OutForRFQ,
                    CreatedBy = UserContext.User.UserName,
                    CreatedUtc = DateTime.UtcNow,
                };

                partRevisionId = PartRevisionService.AddPartRevision(partRevision);

                var newPd = ProductService.FindProductById(newProductId);
                newPd.PartRevisionId = partRevisionId;
                ProductService.UpdateProduct(newPd);

                // Create new entry in BidRFQStatus table
                var bidRFQStatus = new BidRFQStatus
                {
                    ProductId = newProductId,
                    CustomerId = UserContext.Company.Id,
                    StateId = taskData.StateId,
                    TaskId = taskId,
                    SubmittedVendors = 0,
                    TotalVendors = 1,
                    CreatedByUserId = UserContext.UserId,
                };

                var bidRFQStatusId = bidRFQStatusService.AddBidRFQStatus(bidRFQStatus);


                // Notify with email and sms
                string subject = $"New Revision Created as {partRevision.Name} on Omnae by {UserContext.User.UserName}";

                var userIds = new List<SimplifiedUser>();
                var customer = CompanyService.FindCompanyByUserId(UserContext.UserId);
                userIds.AddRange(customer.Users);

                if (parentTask.Product.VendorCompany != null)
                {
                    userIds.AddRange(parentTask.Product.VendorCompany.Users);
                }
                else if (parentTask.RFQBid != null)
                {
                    var vendorId = parentTask.RFQBid.VendorId;
                    var company = CompanyService.FindCompanyById(vendorId);
                    userIds.AddRange(company.Users);
                }
                else
                {
                    var vendorTasks = TaskDataService.FindTaskDataListByProductId(model.ProductId).Where(x => x.RFQBid != null);
                    foreach (var task in vendorTasks)
                    {
                        userIds.AddRange(task.RFQBid.VendorCompany.Users);
                    }
                }
                var mod = new CreatePartRevisionViewModel()
                {
                    BidRequestRevisionId = model.BidRequestRevisionId,
                    UserName = UserContext.User.Email,
                    ProductId = model.ProductId,
                    PartRevision = partRevision.Name,
                    PartRevisionDesc = partRevision.Description,
                };

                foreach (var usr in userIds)
                {
                    mod.UserName = usr.Email;
                    NotificationBL.NotifyCreateNewRevision(subject, usr.Email, usr.PhoneNumber, mod);
                }
                trans.Complete();
                return new NewPartResultModel(newProductId, taskId, taskState, partRevisionId);
            }
        }

        public NewPartResultModel CreateRFQRevision(CreatePartRevisionViewModel model)
        {
            using (var trans = AsyncTransactionScope.StartNew())
            {
                int? partRevisionId = null;
                int? bidRFQStatusId = null;
                int originProductId = model.ProductId;
                var tasks = TaskDataService.FindTaskDatasByProductId(model.ProductId);
                if (model.TaskId == null)
                {
                    throw new Exception("No task was found this RFQ");
                }
                var td = TaskDataService.FindById(model.TaskId.Value);

                var product = ProductService.FindProductById(model.ProductId);
                PartRevision revision = null;
                if (product?.PartRevisionId != null)
                {
                    revision = PartRevisionService.FindPartRevisionById(product.PartRevisionId.Value);
                    if (revision != null)
                    {
                        originProductId = revision.OriginProductId;
                    }
                }

                

                try
                {
                    if (revision != null)
                    {
                        // Update existing Part Revision in PartRevision table
                        revision.TaskId = td.TaskId;
                        revision.OriginProductId = originProductId;
                        revision.Name = model.PartRevision;
                        revision.Description = model.PartRevisionDesc;
                        revision.StateId = States.ReviewRFQ;
                        revision.Description = model.PartRevisionDesc;
                        PartRevisionService.UpdatePartRevision(revision);
                        partRevisionId = revision.Id;
                    }
                    else
                    {
                        // Add this Part Revision to PartRevision table
                        var entity = new PartRevision()
                        {
                            TaskId = td.TaskId,
                            OriginProductId = originProductId,
                            Name = model.PartRevision,
                            Description = model.PartRevisionDesc,
                            StateId = States.ReviewRFQ,
                            CreatedBy = UserContext.User.UserName,
                            CreatedUtc = DateTime.UtcNow,
                        };
                        partRevisionId = PartRevisionService.AddPartRevision(entity);
                    }                   

                    // update product PartNumberRevision and PartRevisionId
                    product.PartNumberRevision = model.PartRevision;
                    product.PartRevisionId = partRevisionId;
                    ProductService.UpdateProduct(product);

                    td.StateId = (int)States.ReviewRFQ;
                    td.ModifiedUtc = DateTime.UtcNow;
                    td.ModifiedByUserId = UserContext.UserId;
                    TaskDataService.Update(td);

                    // Add new RFQ Cycle
                    AddRFQCycle(td, tasks, partRevisionId);

                    // Remove pricebreaks so far made by vendors, if there are any
                    var pricebreaks = PriceBreakService.FindPriceBreakByProductId(model.ProductId);
                    foreach (var pb in pricebreaks)
                    {
                        PriceBreakService.DeletePriceBreak(pb);
                    }
                }
                catch (Exception ex)
                {
                    throw new BLException(ex.RetrieveErrorMessage());
                }

                var parentTask = tasks.Where(x => x.RFQBidId != null && x.RFQBid.IsActive == true).FirstOrDefault();

                // Notify with email and sms
                var partRevision = PartRevisionService.FindPartRevisionById(partRevisionId.Value);
                string subject = $"New Revision Created as {model.PartRevision} on Omnae by {UserContext.User.UserName}";

                var userIds = new List<SimplifiedUser>();
                var customer = product.CustomerCompany;
                userIds.AddRange(customer.Users);

                if (product.VendorId != null)
                {
                    userIds.AddRange(product.VendorCompany.Users);
                }
                else if (parentTask != null && parentTask.RFQBid != null)
                {
                    var vendorId = parentTask.RFQBid.VendorId;
                    var vendor = CompanyService.FindCompanyById(vendorId);
                    userIds.AddRange(vendor.Users);
                }
                else
                {
                    var vendorTasks = TaskDataService.FindTaskDataListByProductId(model.ProductId).Where(x => x.RFQBid != null);
                    foreach (var task in vendorTasks)
                    {
                        userIds.AddRange(task.RFQBid.VendorCompany.Users);
                    }
                }
                var mod = new CreatePartRevisionViewModel()
                {
                    UserName = UserContext.User.Email,
                    ProductId = model.ProductId,
                    PartRevision = partRevision.Name,
                    PartRevisionDesc = partRevision.Description,
                };

                foreach (var usr in userIds)
                {
                    mod.UserName = usr.Email;
                    NotificationBL.NotifyCreateNewRevision(subject, usr.Email, usr.PhoneNumber, mod);
                }

                // Stop all timers related to this product if there are any running
                var timers = timerSetupService.FindAllTimerSetupsByProductId(product.Id);
                foreach (var timer in timers)
                {
                    timerTriggerService.RemoveTimerTrigger(timer.Name);
                }
                var timerToBeExpiredTriggerName = $"BidTimerWillExpire-p_{product.Id}";
                timerTriggerService.RemoveTimerTrigger(timerToBeExpiredTriggerName);

                // Now set TimerStartAt = null for Bid Timer, so that front end won't display this timer anymore
                var timerSetup = timerSetupService.FindTimerSetupByProductIdTimerType(product.Id, TypeOfTimers.BidTimer);
                timerSetup.TimerStartAt = null;
                timerSetupService.UpdateTimerSetup(timerSetup);


                trans.Complete();
                return new NewPartResultModel
                    (td.ProductId.Value, td.TaskId, (States)td.StateId, null,
                    partRevisionId, bidRFQStatusId);
            }
        }

        public void AddRFQCycle(TaskData td, IQueryable<TaskData> tasks, int? partRevisionId = null)
        {
            try
            {
                // Create a  new row in BidRFQStatus table
                var productId = td.ProductId.Value;
                var totalVendors = tasks
                    .Where(x => x.RFQBidId != null && !(x.StateId == (int)States.RFQFailed || x.StateId == (int)States.VendorRejectedRFQ));

                var lastRFQStatus = bidRFQStatusService.FindBidRFQStatusListByProductId(productId).LastOrDefault();
                var lastRevisionCycle = lastRFQStatus?.RevisionCycle != null ? (lastRFQStatus.RevisionCycle) : 0;
                int newState = td.StateId;
                var bidRFQStatus = new BidRFQStatus
                {
                    ProductId = productId,
                    CustomerId = td.Product.CustomerId.Value,
                    StateId = newState,
                    TaskId = td.TaskId,
                    SubmittedVendors = 0,
                    TotalVendors = totalVendors.Count(),
                    PartRevisionId = partRevisionId,
                    RevisionCycle = lastRevisionCycle + 1,
                    CreatedByUserId = UserContext.UserId,
                };

                var bidRFQStatusId = bidRFQStatusService.AddBidRFQStatus(bidRFQStatus);

                foreach (var task in totalVendors)
                {
                    task.StateId = newState == (int)States.KeepCurrentRFQRevision ? (int)States.BidForRFQ : newState;
                    task.ModifiedUtc = DateTime.UtcNow;
                    task.ModifiedByUserId = UserContext.UserId;
                    TaskDataService.Update(task);
                }
            }
            catch
            {
                throw;
            }
        }

        public List<VendorStatsViewModel> GetQualifiedVendors(int productId, VENDOR_TYPE vendorType, string search = null)
        {
            var product = ProductService.FindProductById(productId);
            List<VendorStatsViewModel> vendorsStats = new List<VendorStatsViewModel>();
            if (product == null)
                return vendorsStats;

            List<Company> vendors = new List<Company>() { product.CustomerCompany };
            List<Company> vends = new List<Company>();
            if (vendorType == VENDOR_TYPE.NetworkVendors)
            {
                if (product.BuildType == BUILD_TYPE.Process && product.ProcessType != null)
                {
                    vends = ApprovedCapabilityService.FindApprovedCapabilitiesByParams(product.BuildType, product.Material, (int)product.ProcessType.Value)
                               .GroupBy(x => x.VendorId)
                               .Select(x => x.First().Company)
                               .ToList();
                }
                else
                {
                    switch (product.Material)
                    {
                        case MATERIALS_TYPE.PrecisionMetals:
                            if (product.MetalsProcesses != null)
                            {
                                vends = ApprovedCapabilityService.FindApprovedCapabilitiesByParams(product.BuildType, product.Material, (int)product.MetalsProcesses.Value)
                                    .GroupBy(x => x.VendorId)
                                    .Select(x => x.First().Company)
                                    .ToList();
                            }
                            break;
                        case MATERIALS_TYPE.PrecisionPlastics:
                            if (product.PlasticsProcesses != null)
                            {
                                vends = ApprovedCapabilityService.FindApprovedCapabilitiesByParams(product.BuildType, product.Material, (int)product.PlasticsProcesses.Value)
                                    .GroupBy(x => x.VendorId)
                                    .Select(x => x.First().Company)
                                    .ToList();
                            }
                            break;
                        default:
                            vends = ApprovedCapabilityService.FindApprovedCapabilitiesByParams(product.BuildType, product.Material)
                                    .GroupBy(x => x.VendorId)
                                    .Select(x => x.First().Company)
                                    .ToList();
                            break;
                    }
                }
            }
            else if (vendorType == VENDOR_TYPE.MyVendors)
            {
                // Find out the vendors who has credit relationship with the customer of current product
                vends = companiesCreditRelationshipService.FindCompaniesCreditRelationshipsByCustomerId(product.CustomerId.Value)
                                                          .GroupBy(x => (x.CustomerId, x.VendorId))
                                                          .Select(x => x.FirstOrDefault().Vendor).ToList();
            }

            vendors.AddRange(vends);
            vendors = vendors.GroupBy(x => x.Id).Select(x => x.FirstOrDefault()).ToList();
            if (vendors.Count > 0 && search != null)
            {
                vendors = SearchCompany(vendors, search);
            }
            foreach (var vendor in vendors)
            {
                VendorStatsViewModel stats = ChartBl.NCRStatsApi(vendor.Id);
                stats.VendorType = vendorType;
                vendorsStats.Add(stats);
            }

            return vendorsStats;
        }

        public List<VendorStatsViewModel> GetQualifiedVendors(int customerId,
                                                                BUILD_TYPE buildType,
                                                                VENDOR_TYPE vendorType,
                                                                MATERIALS_TYPE? materialType = null,
                                                                int? processType = null,
                                                                string search = null)
        {
            List<VendorStatsViewModel> vendorsStats = new List<VendorStatsViewModel>();
            List<Company> vendors = new List<Company>();

            if (vendorType != VENDOR_TYPE.Myself)
            {
                if (buildType == BUILD_TYPE.Process && processType != null)
                {
                    if (materialType != null)
                    {
                        vendors = ApprovedCapabilityService
                                        .FindApprovedCapabilitiesByParams(buildType, materialType.Value, processType.Value)
                                        .GroupBy(x => x.VendorId)
                                        .Select(x => x.First().Company)
                                        .ToList();
                    }
                    else
                    {
                        vendors = ApprovedCapabilityService
                                       .FindApprovedCapabilitiesByParams(buildType, (Process_Type)processType.Value)
                                       .GroupBy(x => x.VendorId)
                                       .Select(x => x.First().Company)
                                       .ToList();
                    }
                }
                else if (materialType != null)
                {
                    switch (materialType)
                    {
                        case MATERIALS_TYPE.PrecisionMetals:
                        case MATERIALS_TYPE.PrecisionPlastics:
                            if (processType != null)
                            {
                                vendors = ApprovedCapabilityService
                                    .FindApprovedCapabilitiesByParams(buildType, materialType.Value, processType.Value)
                                    .GroupBy(x => x.VendorId)
                                    .Select(x => x.First().Company)
                                    .ToList();
                            }
                            else
                            {
                                vendors = ApprovedCapabilityService
                                    .FindApprovedCapabilitiesByParams(buildType, materialType.Value)
                                    .GroupBy(x => x.VendorId)
                                    .Select(x => x.First().Company)
                                    .ToList();
                            }
                            break;

                        default:
                            vendors = ApprovedCapabilityService
                                    .FindApprovedCapabilitiesByParams(buildType, materialType.Value)
                                    .GroupBy(x => x.VendorId)
                                    .Select(x => x.First().Company)
                                    .ToList();
                            break;
                    }
                }

                if (vendorType == VENDOR_TYPE.MyVendors)
                {
                    // Find out the vendors who has credit relationship with the customer
                    var myvendors = companiesCreditRelationshipService
                            .FindCompaniesCreditRelationshipsByCustomerId(customerId)
                            .Select(x => x.Vendor).ToList();

                    vendors = (from vd in vendors
                               join mv in myvendors
                               on vd.Id equals mv.Id into vd_mv
                               from vm in vd_mv.DefaultIfEmpty()
                               where vm != null
                               select vm
                              ).ToList();
                }
            }

            vendors = vendors.Where(x => x.Id != customerId).ToList();
            if (vendors.Count > 0 && search != null)
            {
                vendors = SearchCompany(vendors, search);
            }
            foreach (var vendor in vendors)
            {
                VendorStatsViewModel stats = ChartBl.NCRStatsApi(vendor.Id);
                stats.VendorType = vendorType;

                // The following are for sorting purpose
                stats.Country = stats.Company.Name;
                stats.PartsConformance = stats.Stats.PartsConformance;
                stats.OrderConformance = stats.Stats.OrderConformance;
                stats.OnTimeConformance = stats.Stats.OnTimeConformance;
                stats.CompletedParts = stats.Stats.ShippedParts;
                stats.CompletedOrders = stats.Stats.ShippedOrders;
                stats.LeadTime = stats.Stats.AvrLeadTime;

                vendorsStats.Add(stats);
            }

            return vendorsStats;
        }

        public VendorStatsViewModel GetVendorStatsByCompanyId(int companyId)
        {
            var vendor = CompanyService.FindCompanyById(companyId);
            if (vendor == null)
                return new VendorStatsViewModel();
            return ChartBl.NCRStatsApi(companyId);
        }

        public IQueryable<Company> GetCompanyParnters(int companyId, PARTNER_TYPE type, string search = null)
        {
            IQueryable<Company> partners = Enumerable.Empty<Company>().AsQueryable();
            switch (type)
            {
                case PARTNER_TYPE.InvitedByMe:
                    partners = CompanyService.FindAllCompanies()
                        .Where(x => x.InvitedByCompanyId == companyId);
                    break;

                case PARTNER_TYPE.Network:
                    partners = OrderService.FindOrderByCompanyId(companyId)
                                    .GroupBy(x => x.Product.VendorId)
                                    .Select(x => x.FirstOrDefault().Product.VendorCompany);
                    break;

                case PARTNER_TYPE.Vendor:
                    partners = ProductService.FindProductsByCustomerId(companyId)
                        .Where(x => x.VendorId != null)
                        .GroupBy(x => x.VendorId)
                        .Select(x => x.FirstOrDefault().VendorCompany);

                    break;

                case PARTNER_TYPE.Customer:
                    partners = ProductService.FindProductsByVendorId(companyId)
                        .Where(x => x.CustomerId != null)
                        .GroupBy(x => x.CustomerId)
                        .Select(x => x.FirstOrDefault().CustomerCompany);

                    break;

                default:
                    partners = companiesCreditRelationshipService
                                .FindCompaniesCreditRelationshipIQueryableByCompanyId(companyId)
                                .Select(x => x.CustomerId == companyId ?  x.Vendor : x.Customer);

                    // get Network vendors
                    var vendors = OrderService.FindOrderByCompanyId(companyId)
                                    .GroupBy(x => x.Product.VendorId)
                                    .Select(x => x.FirstOrDefault().Product.VendorCompany);
                    if (vendors != null)
                    {
                        partners = partners.Union(vendors);
                    }

                    break;
            }

            if (type != PARTNER_TYPE.Customer)
            {
                // vendors from sharing products
                var sharing = ProductSharingService.FindProductSharingsByComanyId(companyId)
                    .Where(x => x.Product.VendorId != null)
                    .GroupBy(x => x.Product.VendorId)
                    .Select(x => x.FirstOrDefault().Product.VendorCompany);
                partners = partners.Union(sharing);
            }
            else 
            {
                // customers from sharing products
                var sharing = ProductSharingService.FindProductSharingsByComanyId(companyId)
                    .Where(x => x.Product.CustomerId != null)
                    .GroupBy(x => x.Product.CustomerId)
                    .Select(x => x.FirstOrDefault().Product.CustomerCompany);
                partners = partners.Union(sharing);
            }

            if (type != PARTNER_TYPE.Vendor && type != PARTNER_TYPE.Customer)
            {
                partners = partners.Where(x => x.Id != companyId);
            }

            if (partners.Count() > 0 && search != null)
            {
                partners = SearchCompanyIncludingEmail(partners, search);

            }
            return partners;
        }

        public List<VendorStatsViewModel> GetCompanyParntersWithQAStats(int companyId, PARTNER_TYPE type, string search = null)
        {
            List<VendorStatsViewModel> vendorsStats = new List<VendorStatsViewModel>();
            IQueryable<Company> partners = Enumerable.Empty<Company>().AsQueryable();
            switch (type)
            {
                case PARTNER_TYPE.InvitedByMe:
                    partners = CompanyService.FindAllCompanies()
                       .Where(x => x.InvitedByCompanyId == companyId);
                    break;

                case PARTNER_TYPE.Network:
                    partners = OrderService.FindOrderByCompanyId(companyId)
                                    .GroupBy(x => x.Product.VendorId)
                                    .Select(x => x.FirstOrDefault().Product.VendorCompany);
                    break;

                default:
                    partners = companiesCreditRelationshipService
                                .FindCompaniesCreditRelationshipIQueryableByCompanyId(companyId)
                                .GroupBy(x => x.CustomerId == companyId ? x.VendorId : x.CustomerId)
                                .Select(x => x.FirstOrDefault().CustomerId == companyId ? x.FirstOrDefault().Vendor : x.FirstOrDefault().Customer);


                    // get Network vendors
                    var vendors = OrderService.FindOrderByCompanyId(companyId)
                                    .GroupBy(x => x.Product.VendorId)
                                    .Select(x => x.FirstOrDefault().Product.VendorCompany);
                    if (vendors != null)
                    {
                        partners = partners.Union(vendors);
                    }

                    break;
            }
            if (type != PARTNER_TYPE.Vendor)
            {
                partners = partners.Where(x => x.Id != companyId);
            }
            if (partners.Count() > 0 && search != null)
            {
                partners = SearchCompany(partners, search);

            }
            foreach (var partner in partners)
            {
                VendorStatsViewModel stats = ChartBl.NCRStatsApi(partner.Id);

                // The following are for sorting purpose
                stats.PartnerType = type;
                stats.Country = stats.Country;
                stats.PartsConformance = stats.Stats.PartsConformance;
                stats.OrderConformance = stats.Stats.OrderConformance;
                stats.OnTimeConformance = stats.Stats.OnTimeConformance;
                stats.CompletedParts = stats.Stats.ShippedParts;
                stats.CompletedOrders = stats.Stats.ShippedOrders;
                stats.LeadTime = stats.Stats.AvrLeadTime;

                vendorsStats.Add(stats);
            }

            return vendorsStats;
        }


        public async Task<(AssignRFQToVendorsViewModel VendorModel, NewPartResultModel PartResult, string WarningMsg)> CreateAsync(ProductViewModel model, Product product)
        {
            var option = new TransactionOptions() { Timeout = TimeSpan.FromMinutes(15), IsolationLevel = IsolationLevel.ReadCommitted };
            using (var ts = AsyncTransactionScope.StartNew(option))
            {
                string warningMsg = null;
                product.QuoteId = 1;
                var userId = UserContext.UserId;
                if (userId == null)
                {
                    throw new BLException("User was not found");
                }
                var company = CompanyService.FindCompanyByUserId(userId);
                if (company == null)
                {
                    throw new BLException("Company was not found");
                }

                List<Company> vendors = new List<Company>() { company };
                if (company.isEnterprise)
                {
                    if (model.BuildType == BUILD_TYPE.Process && product.ProcessType != null)
                    {
                        vendors = ApprovedCapabilityService.FindApprovedCapabilitiesByParams(product.BuildType, product.Material, (int)product.ProcessType.Value)
                               .GroupBy(x => x.VendorId)
                               .Select(x => x.First().Company)
                               .ToList();
                    }
                    else if (model.Material != null && model.BuildType != null)
                    {
                        switch (model.Material)
                        {
                            case MATERIALS_TYPE.PrecisionMetals:
                                vendors = ApprovedCapabilityService.FindApprovedCapabilitiesByParams(model.BuildType.Value, model.Material.Value, (int)model.MetalsProcesses.Value)
                                    .Select(x => x.Company).GroupBy(x => x.Id).Select(x => x.First()).ToList();
                                break;
                            case MATERIALS_TYPE.PrecisionPlastics:
                                vendors = ApprovedCapabilityService.FindApprovedCapabilitiesByParams(model.BuildType.Value, model.Material.Value, (int)model.PlasticsProcesses.Value)
                                    .Select(x => x.Company).GroupBy(x => x.Id).Select(x => x.First()).ToList();
                                break;
                            default:
                                vendors = ApprovedCapabilityService.FindApprovedCapabilitiesByParams(model.BuildType.Value, model.Material.Value)
                                        .Select(x => x.Company).GroupBy(x => x.Id).Select(x => x.First()).ToList();
                                break;
                        }
                    }


                    // find out this customer's all onboarding products
                    var vnds = companiesCreditRelationshipService.FindCompaniesCreditRelationshipsByCustomerId(company.Id).Select(x => x.Vendor);
                    vendors.AddRange(vnds);
                }

                product.CustomerId = company.Id;
                product.CustomerPriority = CustomerBidPreference.Preferences[model.CustomerPriority];

                // Add to PartRevision table
                var partRevsion = new PartRevision()
                {
                    Name = model.PartNumberRevision,
                    Description = "Initial Part Revision",
                    CreatedBy = UserContext.User.UserName,
                    CreatedUtc = DateTime.UtcNow
                };
                int partRevisionId = PartRevisionService.AddPartRevision(partRevsion);
                product.PartRevisionId = partRevisionId;

                if (model.AvatarFile != null)
                {
                    var postedFile = model.AvatarFile;
                    var fileName = FileUtil.MakeValidFileName($"Avatar-Customer-{product.CustomerId}-{Guid.NewGuid()}{Path.GetExtension(postedFile.FileName)}");
                    product.AvatarUri = ImageStorageService.Upload(postedFile, fileName);
                }

                // Create RFQQuantity table based on user inputs
                var qtys = model.QuantityList.Where(x => x != null).Select(q => q.Value).ToList();
                var lastQty = qtys.Last();

                // Add two more extra qty calculated as below if last two qty columns available
                if (qtys.Count < 6)
                {
                    qtys.Add((decimal)(lastQty * (decimal)1.5));
                    qtys.Add(lastQty * 2);
                }

                RFQQuantity rfqqty = new RFQQuantity();
                for (int i = 0; i < qtys.Count; i++)
                {
                    if (i == 0 && qtys[i] > 0)
                    {
                        rfqqty.Qty1 = qtys[i];
                    }
                    if (i == 1 && qtys[i] > 0)
                    {
                        rfqqty.Qty2 = qtys[i];
                    }
                    if (i == 2 && qtys[i] > 0)
                    {
                        rfqqty.Qty3 = qtys[i];
                    }
                    if (i == 3 && qtys[i] > 0)
                    {
                        rfqqty.Qty4 = qtys[i];
                    }
                    if (i == 4 && qtys[i] > 0)
                    {
                        rfqqty.Qty5 = qtys[i];
                    }
                    if (i == 5 && qtys[i] > 0)
                    {
                        rfqqty.Qty6 = qtys[i];
                    }
                    if (i == 6 && qtys[i] > 0)
                    {
                        rfqqty.Qty7 = qtys[i];
                    }

                }
                rfqqty.UnitOfMeasurement = model.UnitOfMeasurement;
                int rfqqtyid = RfqQuantityService.AddRFQQuantity(rfqqty);

                product.RFQQuantityId = rfqqtyid;
                product.CreatedDate = DateTime.UtcNow;

                var productId = ProductService.AddProduct(product);

                // Create a new TaskData from here
                var taskData = new TaskData()
                {
                    StateId = (int)States.PendingRFQ,
                    ProductId = productId,
                    CreatedUtc = DateTime.UtcNow,
                    ModifiedUtc = DateTime.UtcNow,
                    UpdatedBy = UserContext.User.UserName,
                    IsRiskBuild = model.RiskBuild,
                    isEnterprise = company.isEnterprise,
                };
                var taskId = TaskDataService.AddTaskData(taskData);

                // set this task id to PartRevision
                var partRevision = PartRevisionService.FindPartRevisionById(partRevisionId);
                partRevsion.OriginProductId = productId;
                partRevision.TaskId = taskId;
                partRevision.StateId = States.PendingRFQ;
                PartRevisionService.UpdatePartRevision(partRevision);

                // Fill in ProductStateTracking table with product id, state id ...
                var productState = new ProductStateTracking()
                {
                    ProductId = productId,
                    StateId = taskData.StateId,
                    UpdatedBy = taskData.UpdatedBy,
                    ModifiedUtc = taskData.ModifiedUtc
                };
                ProductStateTrackingService.AddProductStateTracking(productState);

                // Notify with email and sms
                string subject = $"Omnae.com New Part Created {product.PartNumber}, {product.Description}";
                var user = DbUser.Users.Find(userId);
                string destEmail = user.Email;
                string destSms = user.PhoneNumber;
                var entity = new CreateNewPartViewModel()
                {
                    UserName = destEmail,
                    PartNumber = product.PartNumber,
                    Description = product.Description,
                    IsCustomer = true,
                };

                try
                {
                    NotificationService.NotifyCreateNewPart(subject, destEmail, destSms, entity);
                }
                catch (Exception ex)
                {
                    warningMsg = ex.RetrieveErrorMessage();
                }

                taskData.Product = product;

                if (company.isEnterprise == false)
                {
                    // Notify Admin to take action
                    destEmail = ConfigurationManager.AppSettings["AdminEmail"];
                    destSms = ConfigurationManager.AppSettings["AdminPhone"];

                    taskData.Product = product;
                    try
                    {
                        await NotificationBL.SendNotificationsAsync(taskData, destEmail, destSms);
                    }
                    catch (Exception ex)
                    {
                        warningMsg = ex.RetrieveErrorMessage();
                    }
                }

                if (company.isEnterprise)
                {
                    var mod = new AssignRFQToVendorsViewModel
                    {
                        ProductId = productId,
                        isEnterprise = taskData.isEnterprise,
                        isChosen = new bool[vendors.Count + 1],
                        VendorIds = new int[vendors.Count + 1],
                        Name = new string[vendors.Count + 1],
                        Address = new Address[vendors.Count + 1],
                    };
                    for (int i = 0; i < vendors.Count; i++)
                    {
                        mod.VendorIds[i] = vendors[i].Id;
                        mod.Name[i] = vendors[i].Name;
                        mod.Address[i] = vendors[i].Address;
                        mod.Charts.Add(ChartBl.GetVendorQAChart(vendors[i].Id, i));
                    }
                    ts.Complete();
                    return (mod, new NewPartResultModel(productId, taskId, (States)taskData.StateId, partRevisionId), warningMsg);
                }
                ts.Complete();
                return (null, new NewPartResultModel(productId, taskId, (States)taskData.StateId, partRevisionId), warningMsg);
            }
        }


        public AssignRFQResult AssignRFQToVendors(int productId, int[] vendorIds)
        {
            using (var ts = AsyncTransactionScope.StartNew())
            {
                Product product = ProductService.FindProductById(productId);
                if (product == null)
                    throw new Exception(IndicatingMessages.ProductNotFound);

                int taskId = 0;
                TaskData taskData = null;

                bool selected = false;
                if (vendorIds == null)
                {
                    throw new Exception("No vendor selected.");
                }
                int length = vendorIds.Length > COMMON_MAX.MAX_VENDORS_FOR_RFQ ? COMMON_MAX.MAX_VENDORS_FOR_RFQ : vendorIds.Length;
                vendorIds = vendorIds.Take(length).ToArray();

                string subject = $"Omnae.com New Part Created {product.PartNumber}, {product.Description}";
                var entity = new CreateNewPartViewModel()
                {
                    PartNumber = product.PartNumber,
                    Description = product.Description,
                };

                // Create a new row in BidRFQStatus table
                var CustomerTaskData = TaskDataService.FindTaskDataListByProductId(productId)
                    .Where(x => x.RFQBidId == null).FirstOrDefault();

                // change customer taskData state to ReviewRFQ
                CustomerTaskData.StateId = (int)States.ReviewRFQ;
                CustomerTaskData.UpdatedBy = UserContext.User.UserName;
                CustomerTaskData.ModifiedUtc = DateTime.UtcNow;
                TaskDataService.Update(CustomerTaskData);

                var status = bidRFQStatusService.FindBidRFQStatusListByProductId(productId).LastOrDefault();
                var bidRFQStatus = new BidRFQStatus
                {
                    ProductId = productId,
                    CustomerId = UserContext.Company.Id,
                    StateId = (int)States.ReviewRFQ,
                    TaskId = CustomerTaskData.TaskId,
                    SubmittedVendors = 0,
                    TotalVendors = vendorIds.Length,
                    CreatedByUserId = UserContext.UserId,
                };

                var bidRFQStatusId = bidRFQStatusService.AddBidRFQStatus(bidRFQStatus);

                foreach (var vid in vendorIds)
                {
                    var rfqbid = RfqBidService.FindRFQBidByVendorIdProductId(vid, productId);
                    if (rfqbid != null)
                    {
                        selected = true;
                        continue;
                    }

                    selected = false;
                    DateTime utcNow = DateTime.UtcNow;
                    RFQBid bid = new RFQBid()
                    {
                        ProductId = productId,
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
                    // 2. then set this productId to the product's TaskData's ProductId

                    taskData = new TaskData()
                    {
                        StateId = (int)States.ReviewRFQ,
                        ProductId = productId,
                        RFQBidId = bidId,
                        CreatedUtc = utcNow,
                        ModifiedUtc = utcNow,
                        UpdatedBy = UserContext.User.UserName,
                        CreatedByUserId = UserContext.UserId,
                        isEnterprise = true,
                    };
                    taskId = TaskDataService.AddTaskData(taskData);

                    // Notify selected vendors
                    var vendor = CompanyService.FindCompanyById(vid);
                    foreach (var usr in vendor.Users)
                    {
                        entity.UserName = usr.Email;
                        entity.IsCustomer = false;
                        NotificationService.NotifyCreateNewPart(subject, usr.Email, null, entity);
                    }

                    var vendorBidRFQStatus = new VendorBidRFQStatus
                    {
                        ProductId = productId,
                        TaskId = taskData.TaskId,
                        VendorId = vid,
                        StateId = taskData.StateId,
                        BidRFQStatusId = bidRFQStatusId,
                    };
                    var vendorBidRFQStatusId = vendorBidRFQStatusService.AddVendorBidRFQStatus(vendorBidRFQStatus);
                }



                var ret = new AssignRFQResult
                {
                    alreadySelected = selected,
                    productId = productId,
                    vendors = vendorIds,
                };
                ts.Complete();
                return ret;
            }
        }

        public int SetRFQActionReasonTable(int productId, int vendorId, string reason, string description, REASON_TYPE reasonType)
        {
            using (var ts = AsyncTransactionScope.StartNew())
            {
                RFQActionReason entity = new RFQActionReason
                {
                    ProductId = productId,
                    VendorId = vendorId,
                    ReasonType = reasonType,
                    Reason = reason,
                    Description = description,
                    CreatedByUserId = UserContext.UserId,
                };
                var exist = rfqActionReasonService.FindRFQActionReasonListByProductIdVendorId(productId, vendorId, reasonType);
                int id = 0;
                if (exist != null)
                {
                    exist.Reason = reason;
                    exist.Description = description;
                    exist.ModifiedByUserId = UserContext.UserId;
                    exist.ReasonType = reasonType;
                    rfqActionReasonService.UpdateRFQActionReason(exist);
                    id = exist.Id;
                }
                else
                {
                    id = rfqActionReasonService.AddRFQActionReason(entity);
                }

                ts.Complete();
                return id;
            }
        }

        private async Task<string> GetSalesTaxPercentage(Model.Models.Address addr)
        {
            decimal taxRate = 0m;
            if (addr.StateProvince != null)
            {
                taxRate = await GetTaxRateValue(addr.Country.CountryName, addr.StateProvince.Abbreviation);
            }
            return taxRate + "%";
        }

        public async Task<decimal> GetTaxRateValue(string countryName, string countrySubDivisionCode)
        {
            QboApi qboApi = new QboApi(QboTokensService);
            return await qboApi.GetTaxRateValue(countryName, countrySubDivisionCode);
        }

        public bool IsUsersProduct(Product product)
        {
            var userType = UserContext.UserType;
            var company = UserContext.Company; //CompanyService.FindCompanyByUserId(currentUserId);

            var productSharing = ProductSharingService.FindProductSharingByCompanyIdProductId(company.Id, product.Id);
            return (product.CustomerId == company.Id || product.VendorId == company.Id || productSharing != null || userType == USER_TYPE.Vendor);
        }

        public bool HasProductOwnership(Company company, Product product)
        {
            return (product.CustomerId == company.Id || product.VendorId == company.Id);
        }

        public AssignRFQToVendorsViewModel AssignRFQToValidVendors(int id)
        {
            using (var ts = AsyncTransactionScope.StartNew())
            {

                var product = ProductService.FindProductById(id);
                List<int> vendorIds = null;

                // list qualified vendors : 
                // 1) isQualified = true, 
                // 2) retrieve from ApprovedCapabilities table based on BuildType, MaterialType, 
                // MetalProcess (if MaterialType is metal) or PlasticsProcess if MaterialType is Plastics) chosen by customer when create a new part.

                switch (product.Material)
                {
                    case MATERIALS_TYPE.PrecisionMetals:
                        vendorIds = ApprovedCapabilityService.FindApprovedCapabilitiesByParams(product.BuildType, product.Material, (int)product.MetalsProcesses.Value).Select(x => x.VendorId).ToList();
                        break;
                    case MATERIALS_TYPE.PrecisionPlastics:
                        vendorIds = ApprovedCapabilityService.FindApprovedCapabilitiesByParams(product.BuildType, product.Material, (int)product.PlasticsProcesses.Value).Select(x => x.VendorId).ToList();
                        break;
                    default:
                        vendorIds = ApprovedCapabilityService.FindApprovedCapabilitiesByParams(product.BuildType, product.Material).Select(x => x.VendorId).ToList();
                        break;
                }
                if (vendorIds == null || vendorIds.Count == 0)
                {
                    throw new BLException("No vendor was found in OMNAE System whose material or process is matching you just selected for the new RFQ.");
                }
                var td = TaskDataService.FindTaskDataByProductId(id);
                var mod = new AssignRFQToVendorsViewModel
                {
                    ProductId = id,
                    isEnterprise = true,
                    isChosen = new bool[vendorIds.Count + 1],
                    VendorIds = new int[vendorIds.Count + 1],
                    Name = new string[vendorIds.Count + 1],
                    Address = new Address[vendorIds.Count + 1],
                };

                for (int i = 0; i < vendorIds.Count; i++)
                {
                    var vendor = CompanyService.FindCompanyById(vendorIds[i]);
                    if (vendor != null)
                    {
                        mod.VendorIds[i] = vendor.Id;
                        mod.Name[i] = vendor.Name;
                        mod.Address[i] = vendor.Address;
                        mod.Charts.Add(ChartBl.GetVendorQAChart(vendorIds[i], i));
                    }
                }
                ts.Complete();
                return mod;
            }
        }

        public string UpdateRFQQuantities(int productId, List<int> qtys)
        {
            using (var trans = AsyncTransactionScope.StartNew())
            {
                var product = ProductService.FindProductById(productId);
                if (qtys.Any() && qtys.Count < 6)
                {
                    int lastQty = qtys.Last();
                    qtys.Add((int)(lastQty * 1.5));
                    qtys.Add(lastQty * 2);
                }

                RFQQuantity rfqqty = new RFQQuantity();
                for (int i = 0; i < qtys.Count; i++)
                {
                    if (i == 0 && qtys[i] > 0)
                    {
                        rfqqty.Qty1 = qtys[i];
                    }
                    if (i == 1 && qtys[i] > 0)
                    {
                        rfqqty.Qty2 = qtys[i];
                    }
                    if (i == 2 && qtys[i] > 0)
                    {
                        rfqqty.Qty3 = qtys[i];
                    }
                    if (i == 3 && qtys[i] > 0)
                    {
                        rfqqty.Qty4 = qtys[i];
                    }
                    if (i == 4 && qtys[i] > 0)
                    {
                        rfqqty.Qty5 = qtys[i];
                    }
                    if (i == 5 && qtys[i] > 0)
                    {
                        rfqqty.Qty6 = qtys[i];
                    }
                    if (i == 6 && qtys[i] > 0)
                    {
                        rfqqty.Qty7 = qtys[i];
                    }
                }

                try
                {
                    product.RFQQuantityId = RfqQuantityService.AddRFQQuantity(rfqqty);
                    ProductService.UpdateProduct(product);

                    string subject = $"Quantities for RFQ {product.Name} on Omnae have been updated by {UserContext.User.UserName}";
                    var model = new RFQUpdateQuantityViewModel()
                    {
                        PartNumber = product.PartNumber,
                        ProductName = product.Name,
                        Qty1 = rfqqty.Qty1,
                        Qty2 = rfqqty.Qty2,
                        Qty3 = rfqqty.Qty3,
                        Qty4 = rfqqty.Qty4,
                        Qty5 = rfqqty.Qty5,
                        Qty6 = rfqqty.Qty6,
                        Qty7 = rfqqty.Qty7,
                    };

                    var tasks = TaskDataService.FindTaskDataListByProductId(productId).Where(x => x.StateId != (int)States.VendorRejectedRFQ);
                    var customerTask = tasks.Where(x => x.RFQBidId == null).FirstOrDefault();
                    var newState = customerTask.IsRFQReview() ? (int)States.RFQReviewUpdateQuantity : (int)States.RFQBidUpdateQuantity;
                    var totalVendors = tasks.Count(x => x.RFQBidId != null && x.StateId != (int)States.RFQFailed);

                    var lastRFQStatus = bidRFQStatusService.FindBidRFQStatusListByProductId(productId).LastOrDefault();
                    var lastRevisionCycle = lastRFQStatus?.RevisionCycle != null ? (lastRFQStatus.RevisionCycle) : 0;
                    var bidRFQStatus = new BidRFQStatus
                    {
                        ProductId = productId,
                        CustomerId = customerTask.Product.CustomerId.Value,
                        StateId = (int)newState,
                        TaskId = customerTask.TaskId,
                        SubmittedVendors = 0,
                        TotalVendors = totalVendors,
                        RevisionCycle = lastRevisionCycle + 1,
                        CreatedByUserId = UserContext.UserId,
                    };

                    var bidRFQStatusId = bidRFQStatusService.AddBidRFQStatus(bidRFQStatus);
                    var timerType = customerTask.IsRFQReview() ? TypeOfTimers.RFQRevisionTimer : TypeOfTimers.BidTimer;
                    foreach (var task in tasks)
                    {
                        task.StateId = newState;
                        task.ModifiedUtc = DateTime.UtcNow;
                        task.ModifiedByUserId = UserContext.UserId;
                        TaskDataService.Update(task);

                        if (task.RFQBidId == null) continue;
                        var vendorBidRFQStatus = new VendorBidRFQStatus
                        {
                            ProductId = productId,
                            TaskId = task.TaskId,
                            VendorId = task.RFQBid.VendorId,
                            StateId = task.StateId,
                            CreatedByUserId = UserContext.UserId,
                            BidRFQStatusId = bidRFQStatusId,
                        };
                        vendorBidRFQStatusService.AddVendorBidRFQStatus(vendorBidRFQStatus);

                        // Notify to vendors with email about quantity changes
                        if (task.RFQBid == null) continue;
                        var users = task.RFQBid.VendorCompany.Users;
                        foreach (var user in users)
                        {
                            model.UserName = user.Email;
                            NotificationBL.NotifyUpdateQuantities(subject, user.Email, user.PhoneNumber, model);
                        }
                    }

                    // Re-Start Bid Timer here
                    var interval = timerSetupService.FindAllTimerSetupsByProductIdTimerType(productId, timerType)
                        .ToList()
                        .LastOrDefault()?.Interval;
                    if (string.IsNullOrEmpty(interval))
                        return IndicatingMessages.TimerIntervalCouldnotBeFound;

                    var error = taskDataBL.SetupTimer(productId, interval, timerType);
                    if (error != null)
                        return error;

                    trans.Complete();
                    return null;
                }
                catch (Exception ex)
                {
                    return ex.RetrieveErrorMessage();
                }
            }
        }


#if true
        private List<Company> SearchCompany(List<Company> companies, string search)
        {
            search = search.Trim().ToUpper();
            return companies.Where(x => x.Name.ToUpper().Contains(search)).ToList();

        }
#else

        private List<Company> SearchCompany(List<Company> companies, string search)
        {
            search = search.Trim().ToUpper();
            return companies.Where(x => x.Name.ToUpper().Contains(search)
                                        || x.Shipping != null && x.Shipping.EmailAddress.ToUpper().Contains(search))
                            .ToList();

        }
#endif

        private IQueryable<Company> SearchCompany(IQueryable<Company> companies, string search)
        {

            search = search.Trim().ToUpper();
            return companies.Where(x => x.Name.ToUpper().Contains(search));

        }
        private IQueryable<Company> SearchCompanyIncludingEmail(IQueryable<Company> companies, string search)
        {

            search = search.Trim().ToUpper();
            return companies.Where(x => x.Name.ToUpper().Contains(search)
                                        || x.Users.FirstOrDefault().FirstName.ToUpper().Contains(search)
                                        || x.Users.FirstOrDefault().LastName.ToUpper().Contains(search)
                                        || x.Shipping != null && x.Shipping.EmailAddress.ToUpper().Contains(search));


        }
    }
}