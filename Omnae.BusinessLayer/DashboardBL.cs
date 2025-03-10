using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Omnae.BusinessLayer.Models;
using Omnae.Common;
using Omnae.Libs;
using Omnae.Model.Context;
using Omnae.Model.Extentions;
using Omnae.Model.Models;
using Omnae.Service.Service;
using Omnae.Service.Service.Interfaces;
using Omnae.ShippingAPI.DHL.Models;
using AutoMapper;
using Omnae.Data;

namespace Omnae.BusinessLayer
{
    public class DashboardBL
    {
        private TaskSetup TaskSetup { get; }
        private ILogedUserContext UserContext { get; }
        private ITaskDataService TaskDataService { get; }
        private OrderService OrderService { get; }
        private OmnaeInvoiceService OmnaeInvoiceService { get; }
        private DocumentService DocumentService { get; }

        private ProductService ProductService { get; }
        private PriceBreakService PriceBreakService { get; }
        private NCReportService NCReportService { get; }
        private CountryService CountryService { get; }
        private RFQBidService RFQBidService { get; }

        private ExtraQuantityService ExtraQuantityService { get; }
        private PartRevisionService PartRevisionService { get; }
        private AddressService AddressService { get; }
        private ShippingService ShippingService { get; }
        private CompanyService CompanyService { get; }

        private RFQQuantityService RFQQuantityService { get; }
        private NCRImagesService NCRImagesService { get; }

        private IMapper Mapper { get; }

        public DashboardBL(TaskSetup taskSetup, ILogedUserContext userContext, ITaskDataService taskDataService, OrderService orderService, OmnaeInvoiceService omnaeInvoiceService, DocumentService documentService, ProductService productService, PriceBreakService priceBreakService, NCReportService ncReportService, CountryService countryService, RFQBidService rfqBidService, ExtraQuantityService extraQuantityService, PartRevisionService partRevisionService, AddressService addressService, ShippingService shippingService, CompanyService companyService, RFQQuantityService rfqQuantityService, NCRImagesService ncrImagesService, IMapper mapper)
        {
            TaskSetup = taskSetup;
            UserContext = userContext;
            TaskDataService = taskDataService;
            OrderService = orderService;
            OmnaeInvoiceService = omnaeInvoiceService;
            DocumentService = documentService;
            ProductService = productService;
            PriceBreakService = priceBreakService;
            NCReportService = ncReportService;
            CountryService = countryService;
            RFQBidService = rfqBidService;
            ExtraQuantityService = extraQuantityService;
            PartRevisionService = partRevisionService;
            AddressService = addressService;
            ShippingService = shippingService;
            CompanyService = companyService;
            RFQQuantityService = rfqQuantityService;
            NCRImagesService = ncrImagesService;
            this.Mapper = mapper;
        }

        public List<TaskData> GetDashboardData()
        {
            Debug.Assert(UserContext.Company != null, "UserContext.Company != null");

            var model = new List<TaskData>();
            switch (UserContext.UserType)
            {
                case USER_TYPE.Customer:
                    {
                        model = TaskDataService.FindTaskDataByCustomerId(UserContext.Company.Id);
                        break;
                    }
                case USER_TYPE.Vendor:
                    {
                        model = TaskDataService.FindTaskDataByVendorId(UserContext.Company.Id);
                        break;
                    }
                case USER_TYPE.Admin:
                    {
                        break;
                    }
                default: throw new InvalidOperationException("Unknown User Type");
            }

            return model;
            //var taskVMList = new List<TaskViewModel>();
            //foreach (var td in model)
            //{
            //    Order order = td.Orders?.LastOrDefault();
            //    if (order == null)
            //    {
            //        order = OrderService.FindOrdersByProductId(td.ProductId.Value).LastOrDefault();
            //    }
            //    TaskViewModel tvm = new TaskViewModel
            //    {
            //        TaskData = td,
            //        Order = order,
            //        EnumName = Omnae.Common.Extensions.StringExtensions.SplitCamelCase(Enum.GetName(typeof(States), td.StateId)),
            //        VendorUnitPrice = td.Invoices?.LastOrDefault(x => x.UserType == (int)USER_TYPE.Vendor && x.Quantity > 1)?.UnitPrice ?? 0m,
            //        VendorPONumber = td.Invoices?.FirstOrDefault(x => x.UserType == (int)USER_TYPE.Vendor)?.PONumber ?? string.Empty,
            //    };
            //    if (tvm.VendorPONumber == null)
            //    {
            //        tvm.VendorPONumber = OmnaeInvoiceService.FindOmnaeInvoiceByProductId(td.ProductId.Value).FirstOrDefault(x => x.UserType == (int)USER_TYPE.Vendor)?.PONumber ?? string.Empty;
            //    }
            //    taskVMList.Add(tvm);
            //}
            //return taskVMList;
        }

        public CustomerDashboardViewModel GetDashboardDataCustomer()
        {
            Debug.Assert(UserContext.Company != null, "UserContext.Company != null");

            var taskDatas = TaskDataService.FindTaskDataByCustomerId(UserContext.Company.Id);
            return ProcessDashboardCustomer(taskDatas);
        }

        public CustomerDashboardViewModel ProcessDashboardCustomer(List<TaskData> list)
        {
            var mod = new CustomerDashboardViewModel();
            list = list.Where(x => x.RFQBidId == null || 
                                   x.RFQBid.IsActive == true ||
                                   x.RFQBidId != null && x.RFQBid.QuoteAcceptDate != null &&
                                   (x.StateId == (int)States.PendingRFQRevision || x.StateId == (int)States.RFQRevision)).ToList();

            foreach (var td in list.Reverse<TaskData>())
            {
                if (td.StateId != (int)States.RFQBidComplete &&
                    td.StateId != (int)States.ProductionComplete &&
                    td.StateId <= (int)States.NCRClosed ||
                    td.StateId == (int)States.NCRVendorCorrectivePartsInProduction ||
                    //td.StateId == (int)States.NCRCustomerCorrectivePartsAccepted && td.isEnterprise == true ||
                    td.StateId == (int)States.NCRCustomerCorrectivePartsAccepted ||
                    //td.StateId == (int)States.PendingRFQ ||
                    td.StateId == (int)States.BidForRFQ ||
                    td.StateId == (int)States.BidReview ||
                    td.StateId == (int)States.NCRDamagedByCustomer ||
                    td.StateId == (int)States.AddExtraQuantities ||
                    td.StateId == (int)States.SetupMarkupExtraQty ||
                    td.StateId == (int)States.PendingRFQRevision ||
                    td.StateId == (int)States.VendorPendingInvoice ||
                    (td.StateId == (int)States.PaymentMade && td.isEnterprise))
                {
                    //if (td.isEnterprise && td.StateId == (int)States.QuoteAccepted && td.RFQBidId == null)
                    //{
                    //    TaskDataService.DeleteTaskData(td);
                    //    continue;
                    //}

                    Order order = td.Orders.LastOrDefault();
                    if (order == null)
                    {
                        order = OrderService.FindOrdersByProductId(td.ProductId.Value).LastOrDefault();
                    }
                    TaskViewModel tvm = new TaskViewModel
                    {
                        UserType = UserContext.UserType,
                        TaskData = td,
                        Order = order,
                        EnumName = Omnae.Common.Extensions.StringExtensions.SplitCamelCase(Enum.GetName(typeof(States), td.StateId)),
                    };


                    if (td.StateId >= (int)States.NCRVendorRootCauseAnalysis && td.StateId <= (int)States.NCRClosed ||
                     td.StateId == (int)States.NCRCustomerCorrectivePartsAccepted ||
                     td.StateId == (int)States.NCRDamagedByCustomer ||
                     td.StateId == (int)States.RFQRevision)
                    {
                        tvm.NcrDescriptionVM = TaskSetup.SetupNcrDescriptionViewModel(td);
                    }

                    if (tvm.UserType == USER_TYPE.Vendor)
                    {
                        if (td.StateId == (int)States.OutForRFQ ||
                            td.StateId == (int)States.RFQRevision ||
                            td.StateId == (int)States.BidForRFQ)
                        {
                            tvm.RFQVM = SetupRFQBidVM(td);
                        }

                        if (td.StateId == (int)States.AddExtraQuantities)
                        {
                            tvm.NcrDescriptionVM = TaskSetup.SetupNcrDescriptionViewModel(td);
                            tvm.RFQVM = SetupExtraQuantity(td);
                        }
                        if (td.StateId == (int)States.RFQBidComplete && td.RFQBidId != null)
                        {
                            tvm.BidFailedReason = GetBidFailedReason(td.RFQBidId.Value);
                        }

                        if (td.RejectReason != null && td.StateId == (int)States.SampleRejected)
                        {
                            tvm.SampleRejectDocs = TaskSetup.GetDocumentsByProductIdDocType(td.ProductId.Value, DOCUMENT_TYPE.CORRESPOND_SAMPLE_REJECT_PDF);
                        }
                    }

                    if (td.StateId == (int)States.VendorPendingInvoice && order != null)
                    {
                        tvm.VendorInvoiceVM = SetupVendorCreateInvoiceViewModel(td.ProductId.Value, td.TaskId, new List<Order> { order });
                    }

                    if (td.StateId == (int)States.NCRVendorCorrectivePartsComplete)
                    {
                        tvm.NCReport = TaskSetup.GetNCReport(td.ProductId.Value, order.Id);
                    }

                    tvm.Docs = DocumentService.FindDocumentByProductId(td.ProductId.Value);


                    mod.NewlyUpdates.Add(tvm);
                }


                if (td.StateId >= (int)States.OrderPaid && td.StateId < (int)States.BidForRFQ ||
                    td.StateId == (int)States.VendorPendingInvoice)
                {
                    Order order = td.Orders.LastOrDefault();
                    if (order == null)
                    {
                        order = OrderService.FindOrdersByProductId(td.ProductId.Value).LastOrDefault();
                    }
                    TaskViewModel tvm = new TaskViewModel
                    {
                        TaskData = td,
                        Order = order,
                        EnumName = Omnae.Common.Extensions.StringExtensions.SplitCamelCase(Enum.GetName(typeof(States), td.StateId)),
                    };


                    mod.OrderTrackings.Add(tvm);
                }


                if (td.StateId > (int)States.QuoteAccepted && td.StateId < (int)States.ProductionStarted)
                {
                    Order order = td.Orders.LastOrDefault();
                    if (order == null)
                    {
                        order = OrderService.FindOrdersByProductId(td.ProductId.Value).LastOrDefault();
                    }
                    TaskViewModel tvm = new TaskViewModel
                    {
                        TaskData = td,
                        Order = order,
                        EnumName = Omnae.Common.Extensions.StringExtensions.SplitCamelCase(Enum.GetName(typeof(States), td.StateId)),
                    };

                    mod.FirstOrders.Add(tvm);
                }


                if (td.StateId >= (int)States.ProductionStarted &&
                    td.StateId < (int)States.NCRCustomerStarted ||
                    td.StateId == (int)States.ReOrderPaid ||
                    td.StateId == (int)States.NCRClosed)
                {
                    Order order = td.Orders.LastOrDefault();
                    if (order == null)
                    {
                        order = OrderService.FindOrdersByProductId(td.ProductId.Value).LastOrDefault();
                    }
                    TaskViewModel tvm = new TaskViewModel
                    {
                        TaskData = td,
                        Order = order,
                        EnumName = Omnae.Common.Extensions.StringExtensions.SplitCamelCase(Enum.GetName(typeof(States), td.StateId)),
                    };
                    mod.ReOrders.Add(tvm);
                }

            }

            // Remove ReOrders duplicated and sorted on datetime desc
            if (mod.ReOrders != null && mod.ReOrders.Any())
            {
                mod.ReOrders = mod.ReOrders.OrderByDescending(x => x.TaskData.ModifiedUtc).GroupBy(g => g.TaskData.ProductId).Select(y => y.First()).ToList();
            }

            mod.NewlyUpdates = mod.NewlyUpdates.OrderByDescending(x => x.TaskData.ModifiedUtc).ToList();
            mod.ReOrders = mod.ReOrders.OrderByDescending(x => x.TaskData.ModifiedUtc).ToList();
            mod.OrderTrackings = mod.OrderTrackings.OrderByDescending(x => x.TaskData.ModifiedUtc).ToList();
            mod.FirstOrders = mod.FirstOrders.OrderByDescending(x => x.TaskData.ModifiedUtc).ToList();

            return mod;
        }

        public VendorDashboardViewModel GetDashboardDataVendor()
        {
            Debug.Assert(UserContext.Company != null, "UserContext.Company != null");

            var taskDatas = TaskDataService.FindTaskDataByVendorId(UserContext.Company.Id);
            return ProcessDashboardVendorData(taskDatas);
        }

        public VendorDashboardViewModel ProcessDashboardVendorData(List<TaskData> list)
        {
            var mod = new VendorDashboardViewModel();

            foreach (var td in list)
            {

                if (td.StateId == (int)States.OutForRFQ ||
                    td.StateId == (int)States.PendingRFQ ||
                    td.StateId == (int)States.PendingRFQRevision ||
                    td.StateId == (int)States.RFQRevision ||
                    td.StateId == (int)States.BackFromRFQ ||
                    td.StateId == (int)States.BidReview ||
                    td.StateId == (int)States.QuoteAccepted ||
                    td.StateId == (int)States.RFQBidComplete ||
                    td.StateId == (int)States.BidTimeout ||
                    td.StateId == (int)States.BidForRFQ ||
                    td.StateId == (int)States.SetupMarkupExtraQty ||
                    td.StateId == (int)States.AddExtraQuantities)
                {
                    Order order = td.Orders.LastOrDefault();
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
                        Quantity = td.Invoices?.LastOrDefault(x => x.UserType == (int)USER_TYPE.Vendor)?.Quantity ?? 0,
                        VendorUnitPrice = td.Invoices?.LastOrDefault(x => x.UserType == (int)USER_TYPE.Vendor && x.Quantity > 1)?.UnitPrice ?? 0m,
                        VendorPONumber = td.Invoices.LastOrDefault(x => x.UserType == (int)USER_TYPE.Vendor)?.PONumber ?? null,
                        VendorPODocUri = td.Invoices.LastOrDefault(x => x.UserType == (int)USER_TYPE.Vendor)?.PODocUri ?? null,
                        ChkPreconditions = td.StateId == (int)States.ProductionComplete ? TaskSetup.CheckPreconditions(td) : (bool?)null,
                    };
                    if (tvm.VendorPONumber == null)
                    {
                        tvm.VendorPONumber = OmnaeInvoiceService.FindOmnaeInvoiceByProductId(td.ProductId.Value).FirstOrDefault(x => x.UserType == (int)USER_TYPE.Vendor)?.PONumber ?? string.Empty;
                    }
                    mod.RFQs.Add(tvm);
                }


                if (td.StateId > (int)States.QuoteAccepted && td.StateId < (int)States.ProductionComplete)
                {
                    var orders = td.Orders;
                    if (orders == null || orders.Count == 0)
                    {
                        orders = OrderService.FindOrdersByProductId(td.ProductId.Value);
                    }
                    
                    OmnaeInvoiceService.UpdateDocUrlWithSecurityToken(td.Invoices);

                    foreach (var order in orders)
                    {
                        TaskViewModel tvm = new TaskViewModel
                        {
                            TaskData = td,
                            Order = order,
                            EnumName = Omnae.Common.Extensions.StringExtensions.SplitCamelCase(Enum.GetName(typeof(States), td.StateId)),
                            Quantity = order?.Quantity ?? 0,
                            VendorUnitPrice = order.UnitPrice ?? 0m,
                            VendorPONumber = td.Invoices.LastOrDefault(x => x.UserType == (int)USER_TYPE.Vendor && x.OrderId == order.Id)?.PONumber,
                            VendorPODocUri = td.Invoices.LastOrDefault(x => x.UserType == (int)USER_TYPE.Vendor && x.OrderId == order.Id)?.PODocUri,
                            ChkPreconditions = td.StateId == (int)States.ProductionComplete ? TaskSetup.CheckPreconditions(td) : (bool?)null,
                        };
                        if (tvm.VendorPONumber == null)
                        {
                            tvm.VendorPONumber = OmnaeInvoiceService.FindOmnaeInvoiceByProductId(td.ProductId.Value).FirstOrDefault(x => x.UserType == (int)USER_TYPE.Vendor)?.PONumber ?? string.Empty;
                        }
                        mod.ORDORs.Add(tvm);
                    }
                }


                if (td.StateId == (int)States.VendorPendingInvoice)
                {
                    var invoice = td.Invoices.LastOrDefault(x => x.UserType == (int)USER_TYPE.Vendor);
                    if (invoice?.IsOpen == true)
                    {
                        Order order = td.Orders.LastOrDefault();
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
                            Quantity = td.Invoices?.LastOrDefault(x => x.UserType == (int)USER_TYPE.Vendor)?.Quantity ?? 0,
                            VendorUnitPrice = td.Invoices?.LastOrDefault(x => x.UserType == (int)USER_TYPE.Vendor && x.Quantity > 1)?.UnitPrice ?? 0m,
                            VendorPONumber = td.Invoices?.LastOrDefault(x => x.UserType == (int)USER_TYPE.Vendor)?.PONumber ?? null,
                            VendorPODocUri = td.Invoices.LastOrDefault(x => x.UserType == (int)USER_TYPE.Vendor)?.PODocUri ?? null,
                            ChkPreconditions = td.StateId == (int)States.ProductionComplete ? TaskSetup.CheckPreconditions(td) : (bool?)null,
                        };
                        mod.Billings.Add(tvm);
                    }
                }
                if (td.StateId > (int)States.ProductionComplete && td.StateId < (int)States.NCRClosed ||
                    td.StateId == (int)States.NCRVendorCorrectivePartsInProduction ||
                    td.StateId == (int)States.NCRCustomerCorrectivePartsAccepted)
                {
                    Order order = td.Orders.LastOrDefault();
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
                        Quantity = td.Invoices?.LastOrDefault(x => x.UserType == (int)USER_TYPE.Vendor)?.Quantity ?? 0,
                        VendorUnitPrice = td.Invoices?.LastOrDefault(x => x.UserType == (int)USER_TYPE.Vendor && x.Quantity > 1)?.UnitPrice ?? 0m,
                        VendorPONumber = td.Invoices?.LastOrDefault(x => x.UserType == (int)USER_TYPE.Vendor)?.PONumber ?? null,
                        VendorPODocUri = td.Invoices.LastOrDefault(x => x.UserType == (int)USER_TYPE.Vendor)?.PODocUri ?? null,
                        ChkPreconditions = td.StateId == (int)States.ProductionComplete ? TaskSetup.CheckPreconditions(td) : (bool?)null,
                    };
                    if (tvm.VendorPONumber == null)
                    {
                        tvm.VendorPONumber = OmnaeInvoiceService.FindOmnaeInvoiceByProductId(td.ProductId.Value).FirstOrDefault(x => x.UserType == (int)USER_TYPE.Vendor)?.PONumber ?? string.Empty;
                    }
                    mod.NCRs.Add(tvm);
                }
                if (td.StateId == (int)States.ProductionComplete || td.StateId == (int)States.NCRClosed)
                {
                    Order order = td.Orders.LastOrDefault();
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
                        Quantity = td.Invoices?.LastOrDefault(x => x.UserType == (int)USER_TYPE.Vendor)?.Quantity ?? 0,
                        VendorUnitPrice = td.Invoices?.LastOrDefault(x => x.UserType == (int)USER_TYPE.Vendor && x.Quantity > 1)?.UnitPrice ?? 0m,
                        VendorPONumber = td.Invoices?.LastOrDefault(x => x.UserType == (int)USER_TYPE.Vendor)?.PONumber ?? null,
                        VendorPODocUri = td.Invoices.LastOrDefault(x => x.UserType == (int)USER_TYPE.Vendor)?.PODocUri ?? null,
                        ChkPreconditions = td.StateId == (int)States.ProductionComplete ? TaskSetup.CheckPreconditions(td) : (bool?)null,
                    };
                    if (tvm.VendorPONumber == null)
                    {
                        tvm.VendorPONumber = OmnaeInvoiceService.FindOmnaeInvoiceByProductId(td.ProductId.Value).FirstOrDefault(x => x.UserType == (int)USER_TYPE.Vendor)?.PONumber ?? string.Empty;
                    }
                    mod.Products.Add(tvm);
                }
            }

            //var model = new VendorDashboardViewModel
            //{
            //    RFQs = RFQs, 
            //    ORDORs = ORDORs, 
            //    Billings = Billings, 
            //    NCRs = NCRs
            //};

            return mod;
        }

        public List<TaskViewModel> GetDisplayRFQs()
        {
            var model = TaskDataService.FindTaskDataByVendorId(UserContext.Company.Id) //TODO Filter this on the database
                                       .WhereIsRFQs() //Change this to be IQueryable to improve performance
                                       .ToList();

            var taskVMList = new List<TaskViewModel>();
            foreach (var td in model)
            {
                Order order = td.Orders?.LastOrDefault();
                if (order == null)
                {
                    order = OrderService.FindOrdersByProductId(td.ProductId.Value).LastOrDefault();
                }
                TaskViewModel tvm = new TaskViewModel
                {
                    TaskData = td,
                    Order = order,
                    EnumName = Omnae.Common.Extensions.StringExtensions.SplitCamelCase(Enum.GetName(typeof(States), td.StateId)),
                    VendorUnitPrice = td.Invoices?.LastOrDefault(x => x.UserType == (int)USER_TYPE.Vendor && x.Quantity > 1)?.UnitPrice ?? 0m,
                    VendorPONumber = td.Invoices?.FirstOrDefault(x => x.UserType == (int)USER_TYPE.Vendor)?.PONumber ?? string.Empty,
                };
                if (tvm.VendorPONumber == null)
                {
                    tvm.VendorPONumber = OmnaeInvoiceService.FindOmnaeInvoiceByProductId(td.ProductId.Value).FirstOrDefault(x => x.UserType == (int)USER_TYPE.Vendor)?.PONumber ?? string.Empty;
                }
                taskVMList.Add(tvm);
            }

            return taskVMList;
        }

        public List<TaskViewModel> GetDisplayOrders()
        {
            var model = TaskDataService.FindTaskDataByVendorId(UserContext.Company.Id) //TODO Filter this on the database
                                       .WhereIsOrder() //Change this to be IQueryable to improve performance
                                       .ToList();

            var taskVMList = new List<TaskViewModel>();
            foreach (var td in model)
            {
                var orders = td.Orders;
                if (orders == null || orders.Count == 0)
                {
                    orders = OrderService.FindOrdersByProductId(td.ProductId.Value);
                }
                
                OmnaeInvoiceService.UpdateDocUrlWithSecurityToken(td.Invoices);

                foreach (var order in orders)
                {
                    var tvm = new TaskViewModel
                    {
                        //TaskData = td,
                        //Order = order,
                        //EnumName = Omnae.Common.Extensions.StringExtensions.SplitCamelCase(Enum.GetName(typeof(States), td.StateId)),
                        //VendorUnitPrice = td.Invoices?.LastOrDefault(x => x.UserType == (int)USER_TYPE.Vendor && x.Quantity > 1)?.UnitPrice ?? 0m,
                        //VendorPONumber = td.Invoices?.FirstOrDefault(x => x.UserType == (int)USER_TYPE.Vendor)?.PONumber ?? string.Empty,
                        TaskData = td,
                        Order = order,
                        EnumName = Omnae.Common.Extensions.StringExtensions.SplitCamelCase(Enum.GetName(typeof(States), td.StateId)),
                        Quantity = order?.Quantity ?? 0,
                        VendorUnitPrice = order.UnitPrice ?? 0m,
                        VendorPONumber = td.Invoices.FirstOrDefault(x => x.UserType == (int)USER_TYPE.Vendor && x.OrderId == order.Id)?.PONumber,
                        VendorPODocUri = td.Invoices.FirstOrDefault(x => x.UserType == (int)USER_TYPE.Vendor && x.OrderId == order.Id)?.PODocUri,
                        //ChkPreconditions = td.StateId == (int)States.ProductionComplete ? TaskSetup.CheckPreconditions(td) : (bool?)null,
                    };
                    if (tvm.VendorPONumber == null)
                    {
                        tvm.VendorPONumber = OmnaeInvoiceService.FindOmnaeInvoiceByProductId(td.ProductId.Value).FirstOrDefault(x => x.UserType == (int)USER_TYPE.Vendor)?.PONumber ?? string.Empty;
                    }
                    taskVMList.Add(tvm);
                }
            }
            return taskVMList;
        }

        public VendorInvoiceViewModel SetupVendorCreateInvoiceViewModel(int productId, int taskId, List<Order> orders)
        {
            if (orders == null || orders.Count == 0)
            {
                return null;
            }

            var product = ProductService.FindProductById(productId);
            decimal toolingCharges = 0m;
            if (product.VendorId == null)
            {
                return null;
            }
            var pb = PriceBreakService.FindPriceBreakByTaskId(taskId).FirstOrDefault();
            if (pb == null)
            {
                pb = PriceBreakService.FindPriceBreakByProductId(productId).FirstOrDefault();
            }
            if (pb == null)
            {
                return null;
            }

            var invoices = OmnaeInvoiceService.FindOmnaeInvoiceByCompanyIdByTaskId(product.VendorId.Value, taskId);
            if (invoices != null && invoices.Count > 0)
            {
                toolingCharges = invoices.First().ToolingSetupCharges;
#if true
                bool isToolingSeparate = (orders.First().IsForToolingOnly == false) && (toolingCharges > 0);

#else
                bool isToolingSeparate = (orders.First().IsForToolingOnly == false) && (invoices.First().Quantity == pb.NumberSampleIncluded || invoices.First().Quantity == 1) && (toolingCharges > 0);
#endif
                var quantity = orders.Last().Quantity;
                decimal salesTax = invoices.Last().SalesTax;
                decimal unitPrice = invoices.Last().UnitPrice;
                decimal salesPrice = unitPrice * quantity;

                decimal total = unitPrice * quantity + toolingCharges + salesTax;
                VendorInvoiceViewModel model = new VendorInvoiceViewModel
                {
                    OrderDate = orders.Last().OrderDate,
                    ProductName = product.Name,
                    PartNumber = product.PartNumber,
                    PartRevision = product.PartRevision != null ? product.PartRevision.Name : null,
                    ProductDescription = product.Description,
                    SalesPrice = salesPrice,
                    SalesTax = salesTax,
                    UnitPrice = unitPrice,
                    ToolingCharges = toolingCharges,
                    Quantity = quantity,
                    Total = total,
                    ShippedDate = invoices.Last().InvoiceDate,
                    TrackingNumber = orders.Last().TrackingNumber,
                    CustomerName = product.CustomerCompany.Name,
                    IsToolingSeparate = isToolingSeparate,
                    NumberSampleIncluded = pb.NumberSampleIncluded != null ? pb.NumberSampleIncluded.Value : 1,
                };
                return model;
            }
            return null;
        }


        [NonAction] //Used in Views
        public RFQViewModel SetupRFQBidVM(TaskData td)
        {
            using (var ts = AsyncTransactionScope.StartNew())
            {
                RFQBid bid = td.RFQBid;
                Product product = td.Product;
                RFQViewModel model = Mapper.Map<RFQViewModel>(product);
                model.ToolingSetupCharges = 0;
                model.PriceBreakVM = SetupPriceBreaks(td);
                if (model.PriceBreakVM == null)
                    return null;

                // Shipping info

                ShippingQuoteRequestViewModel smodel = new ShippingQuoteRequestViewModel();
                smodel.BkgDetails = new BkgDetails();

                Piece piece = new Piece();
                piece.PieceID = 1;

                smodel.BkgDetails.Pieces = new List<Piece>();
                smodel.BkgDetails.Pieces.Add(piece);

                SetupShippingInfo(product.Id, ref smodel);
                if (bid != null)
                {
                    SetupVendorShippingInfo(bid, ref smodel);
                }
                else
                {
                    SetupVendorShippingInfo(product.Id, ref smodel);
                }


                var company = product.CustomerCompany;
                if (company.ShippingId != null)
                {
                    var shipping = ShippingService.FindShippingById(company.ShippingId.Value);
                    if (shipping.AddressId == null)
                    {
                        return null;
                    }

                    var addr = AddressService.FindAddressById(shipping.AddressId.Value);
                    var country = CountryService.FindCountryById(addr.CountryId);
                    smodel.BkgDetails.PaymentCountryCode = country.CountryCode;
                }

                // Dropdownlists
                var ddlDimensionUnit = new List<SelectListItem>()
                {
                    new SelectListItem() { Text="IN", Value="IN" },
                    new SelectListItem() { Text="CM", Value="CM" }
                };
                smodel.BkgDetails.DdlDimensionUnit = new SelectList(ddlDimensionUnit, "Value", "Text", "CM");

                var ddlWeightUnit = new List<SelectListItem>()
                {
                    new SelectListItem() { Text="LB", Value="LB" },
                    new SelectListItem() { Text="KG", Value="KG" }
                };
                smodel.BkgDetails.DdlWeightUnit = new SelectList(ddlWeightUnit, "Value", "Text", "KG");

                var ddlIsDutiable = new List<SelectListItem>()
                {
                    new SelectListItem() { Text="Yes", Value="Y" },
                    new SelectListItem() { Text="No", Value="N" }
                };
                smodel.BkgDetails.DdlIsDutiable = new SelectList(ddlIsDutiable, "Value", "Text");

                model.ShippingQuoteVM = smodel;
                int docId = bid?.QuoteDocId ?? -1;
                model.QuoteDoc = DocumentService.FindDocumentById(docId);

                ts.Complete();
                return model;
            }
        }

        [NonAction] //Used in a View
        public RFQViewModel SetupExtraQuantity(TaskData td)
        {
            RFQBid bid = null;
            Product product = null;

            if (td?.ProductId == null)
            {
                return null;
            }

            if (td.RFQBidId != null)
            {
                bid = td.RFQBid;
                if (bid == null)
                {
                    return null;
                }

                product = bid.Product;
            }
            else
            {
                if (td.ProductId != null)
                {
                    product = td.Product;
                }
            }
            if (product == null)
            {
                return null;
            }

            RFQViewModel model = Mapper.Map<RFQViewModel>(product);
            model.PriceBreakVM = SetupPriceBreaksExtrafQty(td);

            // Shipping info
            var smodel = new ShippingQuoteRequestViewModel
            {
                BkgDetails = new BkgDetails
                {
                    Pieces = new List<Piece> { new Piece { PieceID = 1 } }
                }
            };

            SetupShippingInfo(product.Id, ref smodel);
            SetupVendorShippingInfo(product.Id, ref smodel);

            if (smodel == null)
            {
                return null;
            }

            smodel.BkgDetails.PaymentCountryCode = CountryService.FindCountryById((int)COUNTRY_ID.CA).CountryCode;

            // Dropdownlists
            var ddlDimensionUnit = new List<SelectListItem>()
            {
                new SelectListItem() { Text="IN", Value="IN" },
                new SelectListItem() { Text="CM", Value="CM" }
            };
            smodel.BkgDetails.DdlDimensionUnit = new SelectList(ddlDimensionUnit, "Value", "Text", "CM");

            var ddlWeightUnit = new List<SelectListItem>()
            {
                new SelectListItem() { Text="LB", Value="LB" },
                new SelectListItem() { Text="KG", Value="KG" }
            };
            smodel.BkgDetails.DdlWeightUnit = new SelectList(ddlWeightUnit, "Value", "Text", "KG");

            var ddlIsDutiable = new List<SelectListItem>()
            {
                new SelectListItem() { Text="Yes", Value="Y" },
                new SelectListItem() { Text="No", Value="N" }
            };
            smodel.BkgDetails.DdlIsDutiable = new SelectList(ddlIsDutiable, "Value", "Text");

            model.ShippingQuoteVM = smodel;
            var docs = TaskSetup.GetDocumentsByProductIdDocType(product.Id, DOCUMENT_TYPE.QUOTE_PDF);
            if (docs?.Any() == true)
            {
                model.QuoteDoc = DocumentService.FindDocumentById(docs.First().Id);
            }

            //TempData["RFQViewModel"] = model;
            //TempData.Keep();

            return model;
        }



        private void SetupShippingInfo(int productId, ref ShippingQuoteRequestViewModel model)
        {
            var product = ProductService.FindProductById(productId);
            if (product?.CustomerId == null)
            {
                return;
            }

            var company = CompanyService.FindCompanyById(product.CustomerId.Value);
            if (company.ShippingId == null)
            {
                return;
            }

            int shippingId = company.ShippingId.Value;
            var shipping = ShippingService.FindShippingById(shippingId);
            if (shipping == null)
            {
                return;
            }

            int addressId = shipping.AddressId.Value;
            var address = AddressService.FindAddressById(addressId);
            var country = CountryService.FindCountryById(address.CountryId);

            model.Destination = new CountryPostalCode
            {
                City = address.City,
                CountryCode = country.CountryCode,
                Postalcode = address.PostalCode ?? address.ZipCode
            };
        }

        private void SetupVendorShippingInfo(RFQBid bid, ref ShippingQuoteRequestViewModel model)
        {
            if (bid == null)
            {
                model = null;
                return;
            }
            int companyId = bid.VendorId;
            var company = CompanyService.FindCompanyById(companyId);
            if (company.ShippingId == null)
            {
                model = null;
                return;
            }
            int shippingId = company.ShippingId.Value;
            var shipping = ShippingService.FindShippingById(shippingId);
            int addressId = shipping.AddressId.Value;
            var address = AddressService.FindAddressById(addressId);
            var country = CountryService.FindCountryById(address.CountryId);

            model.Origin = new CountryPostalCode
            {
                City = address.City,
                CountryCode = country.CountryCode,
                Postalcode = address.PostalCode ?? address.ZipCode
            };
        }

        protected void SetupVendorShippingInfo(int productId, ref ShippingQuoteRequestViewModel model)
        {
            var prod = ProductService.FindProductById(productId);
            int vendorId;
            if (prod.VendorId != null)
            {
                vendorId = prod.VendorId.Value;
            }
            else
            {
                var td = TaskDataService.FindTaskDataListByProductId(productId).FirstOrDefault(x => x.RFQBid != null && x.RFQBid.ProductId == productId);
                vendorId = td?.RFQBid?.VendorId ?? -1;
            }

            if (vendorId <= 0)
            {
                return;
            }

            var company = CompanyService.FindCompanyById(vendorId);
            if (company == null)
            {
                return;
            }

            if (company.ShippingId == null)
            {
                model = null;
                return;
            }
            int shippingId = company.ShippingId.Value;
            var shipping = ShippingService.FindShippingById(shippingId);
            int addressId;
            if (shipping.AddressId != null)
            {
                addressId = shipping.AddressId.Value;
            }
            else
            {
                return;
            }

            var address = AddressService.FindAddressById(addressId);
            var country = CountryService.FindCountryById(address.CountryId);

            model.Origin = new CountryPostalCode
            {
                City = address.City,
                CountryCode = country.CountryCode,
                Postalcode = address.PostalCode ?? address.ZipCode
            };
        }

        private PriceBreakViewModel SetupPriceBreaks(TaskData td)
        {

            Product product = td.Product;
            var priceBreaks = PriceBreakService.FindAllPriceBreaksByTaskId(td.TaskId).ToList(); // including unitprice = 0

            if (product.RFQQuantityId == null)
                return null;

            var qty = RFQQuantityService.FindRFQQuantityById(product.RFQQuantityId.Value);
            int counter = 0;
            if (qty.Qty1 != null)
                counter++;
            if (qty.Qty2 != null)
                counter++;
            if (qty.Qty3 != null)
                counter++;
            if (qty.Qty4 != null)
                counter++;
            if (qty.Qty5 != null)
                counter++;
            if (qty.Qty6 != null)
                counter++;
            if (qty.Qty7 != null)
                counter++;

            if (priceBreaks == null || priceBreaks.Count == 0 || counter > priceBreaks.Count)
            {
                if (counter > priceBreaks.Count)
                {
                    foreach (var pb in priceBreaks)
                    {
                        PriceBreakService.DeletePriceBreak(pb);
                    }
                    priceBreaks.Clear();
                }
                if (qty == null && product.ParentPartRevisionId != null)
                {
                    var partRevision = PartRevisionService.FindPartRevisionById(product.ParentPartRevisionId.Value);
                    priceBreaks = PriceBreakService.FindPriceBreakByProductId(partRevision.OriginProductId);
                }
                else if (qty != null)
                {
                    PriceBreak pb = null;

                    if (qty.Qty1 != null)
                    {
                        pb = new PriceBreak()
                        {
                            RFQBidId = td.RFQBidId != null ? td.RFQBidId.Value : 0,
                            ProductId = product.Id,
                            Quantity = qty.Qty1.Value,
                            ToolingSetupCharges = product.ToolingSetupCharges,
                            UnitPrice = null,
                            TaskId = td.TaskId,
                        };
                        priceBreaks.Add(pb);
                    }
                    if (qty.Qty2 != null)
                    {
                        pb = new PriceBreak()
                        {
                            RFQBidId = td.RFQBidId != null ? td.RFQBidId.Value : 0,
                            ProductId = product.Id,
                            Quantity = qty.Qty2.Value,
                            ToolingSetupCharges = product.ToolingSetupCharges,
                            UnitPrice = null,
                            TaskId = td.TaskId,
                        };
                        priceBreaks.Add(pb);
                    }
                    if (qty.Qty3 != null)
                    {
                        pb = new PriceBreak()
                        {
                            RFQBidId = td.RFQBidId != null ? td.RFQBidId.Value : 0,
                            ProductId = product.Id,
                            Quantity = qty.Qty3.Value,
                            ToolingSetupCharges = product.ToolingSetupCharges,
                            UnitPrice = null,
                            TaskId = td.TaskId,
                        };
                        priceBreaks.Add(pb);
                    }
                    if (qty.Qty4 != null)
                    {
                        pb = new PriceBreak()
                        {
                            RFQBidId = td.RFQBidId != null ? td.RFQBidId.Value : 0,
                            ProductId = product.Id,
                            Quantity = qty.Qty4.Value,
                            ToolingSetupCharges = product.ToolingSetupCharges,
                            UnitPrice = null,
                            TaskId = td.TaskId,
                        };
                        priceBreaks.Add(pb);
                    }

                    if (qty.Qty5 != null)
                    {
                        pb = new PriceBreak()
                        {
                            RFQBidId = td.RFQBidId != null ? td.RFQBidId.Value : 0,
                            ProductId = product.Id,
                            Quantity = qty.Qty5.Value,
                            ToolingSetupCharges = product.ToolingSetupCharges,
                            UnitPrice = null,
                            TaskId = td.TaskId,
                        };
                        priceBreaks.Add(pb);
                    }

                    if (qty.Qty6 != null)
                    {
                        pb = new PriceBreak()
                        {
                            RFQBidId = td.RFQBidId != null ? td.RFQBidId.Value : 0,
                            ProductId = product.Id,
                            Quantity = qty.Qty6.Value,
                            ToolingSetupCharges = product.ToolingSetupCharges,
                            UnitPrice = null,
                            TaskId = td.TaskId,
                        };
                        priceBreaks.Add(pb);
                    }

                    if (qty.Qty7 != null)
                    {
                        pb = new PriceBreak()
                        {
                            RFQBidId = td.RFQBidId != null ? td.RFQBidId.Value : 0,
                            ProductId = product.Id,
                            Quantity = qty.Qty7.Value,
                            ToolingSetupCharges = product.ToolingSetupCharges,
                            UnitPrice = null,
                            TaskId = td.TaskId,
                        };
                        priceBreaks.Add(pb);
                    }

                    //if (qty.IsAddedExtraQty == false)
                    //{
                    //    // one and half of last quantity
                    //    int last = priceBreaks.Last().Quantity;
                    //    PriceBreak oneandhalf = new PriceBreak()
                    //    {
                    //        RFQBidId = td.RFQBidId != null ? td.RFQBidId.Value : 0,
                    //        ProductId = product.Id,
                    //        Quantity = (int)(last * 1.5),
                    //        ToolingSetupCharges = product.ToolingSetupCharges,
                    //        UnitPrice = null
                    //    };

                    //    // add last two qtys
                    //    priceBreaks.Add(oneandhalf);
                    //    AddNewQty(qty, oneandhalf.Quantity);

                    //    // double of last quantity
                    //    PriceBreak dble = new PriceBreak()
                    //    {
                    //        RFQBidId = td.RFQBidId != null ? td.RFQBidId.Value : 0,
                    //        ProductId = product.Id,
                    //        Quantity = last * 2,
                    //        ToolingSetupCharges = product.ToolingSetupCharges,
                    //        UnitPrice = null
                    //    };
                    //    priceBreaks.Add(dble);
                    //    AddNewQty(qty, dble.Quantity);
                    //    qty.IsAddedExtraQty = true;
                    //    RFQQuantityService.UpdateRFQQuantity(qty);
                    //}
                }
            }

            var model = new PriceBreakViewModel
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductAvatarUri = product.AvatarUri,
                PriceBreakList = priceBreaks.GroupBy(x => x.Quantity).Select(x => x.Last()).ToList(),
            };

            return model;
        }

        private void AddNewQty(RFQQuantity qty, int qtyValue)
        {
            if (qty.Qty2 == null)
                qty.Qty2 = qtyValue;
            else if (qty.Qty2 != null && qty.Qty3 == null)
                qty.Qty3 = qtyValue;
            else if (qty.Qty3 != null && qty.Qty4 == null)
                qty.Qty4 = qtyValue;
            else if (qty.Qty4 != null && qty.Qty5 == null)
                qty.Qty5 = qtyValue;
            else if (qty.Qty5 != null && qty.Qty6 == null)
                qty.Qty6 = qtyValue;
            else if (qty.Qty6 != null && qty.Qty7 == null)
                qty.Qty7 = qtyValue;

        }

        private PriceBreakViewModel SetupPriceBreaks(TaskData td, int productId)
        {
            Product product = td.Product;
            var priceBreaks = PriceBreakService.FindPriceBreaksByTaskId(td.TaskId).ToList();

            if (priceBreaks == null || priceBreaks.Count == 0)
            {
                if (product.ParentPartRevisionId != null)
                {
                    var partRevision = PartRevisionService.FindPartRevisionById(product.ParentPartRevisionId.Value);
                    var parentProductId = partRevision.OriginProductId;
                    priceBreaks = PriceBreakService.FindPriceBreakByProductId(parentProductId);
                }
            }

            var model = new PriceBreakViewModel
            {
                ProductId = productId,
                ProductName = product.Name,
                ProductAvatarUri = product.AvatarUri,
                PriceBreakList = priceBreaks.GroupBy(x => x.Quantity).Select(x => x.Last()).ToList(),
            };

            return model;
        }

        private PriceBreakViewModel SetupPriceBreaksExtrafQty(TaskData td)
        {
            if (td?.ProductId == null)
            {
                return null;
            }

            var product = td.Product;
            if (product?.ExtraQuantityId == null)
            {
                return null;
            }

            var qty = ExtraQuantityService.FindExtraQuantityById(product.ExtraQuantityId.Value);

            var priceBreaks = new List<PriceBreak>();
            var pb = new PriceBreak();

            if (qty.Qty1 != null)
            {
                pb = new PriceBreak()
                {
                    ProductId = product.Id,
                    Quantity = qty.Qty1.Value,
                    ToolingSetupCharges = product.ToolingSetupCharges,
                    CustomerToolingSetupCharges = td.isEnterprise == true ? product.ToolingSetupCharges : null,
                    UnitPrice = null,
                };
                priceBreaks.Add(pb);
            }
            if (qty.Qty2 != null)
            {
                pb = new PriceBreak()
                {
                    ProductId = product.Id,
                    Quantity = qty.Qty2.Value,
                    ToolingSetupCharges = product.ToolingSetupCharges,
                    CustomerToolingSetupCharges = td.isEnterprise == true ? product.ToolingSetupCharges : null,
                    UnitPrice = null
                };
                priceBreaks.Add(pb);
            }
            if (qty.Qty3 != null)
            {
                pb = new PriceBreak()
                {
                    ProductId = product.Id,
                    Quantity = qty.Qty3.Value,
                    ToolingSetupCharges = product.ToolingSetupCharges,
                    CustomerToolingSetupCharges = td.isEnterprise == true ? product.ToolingSetupCharges : null,
                    UnitPrice = null
                };
                priceBreaks.Add(pb);
            }
            if (qty.Qty4 != null)
            {
                pb = new PriceBreak()
                {
                    ProductId = product.Id,
                    Quantity = qty.Qty4.Value,
                    ToolingSetupCharges = product.ToolingSetupCharges,
                    CustomerToolingSetupCharges = td.isEnterprise == true ? product.ToolingSetupCharges : null,
                    UnitPrice = null
                };
                priceBreaks.Add(pb);
            }

            if (qty.Qty5 != null)
            {
                pb = new PriceBreak()
                {
                    ProductId = product.Id,
                    Quantity = qty.Qty5.Value,
                    ToolingSetupCharges = product.ToolingSetupCharges,
                    CustomerToolingSetupCharges = td.isEnterprise == true ? product.ToolingSetupCharges : null,
                    UnitPrice = null
                };
                priceBreaks.Add(pb);
            }

            if (qty.Qty6 != null)
            {
                pb = new PriceBreak()
                {
                    ProductId = product.Id,
                    Quantity = qty.Qty6.Value,
                    ToolingSetupCharges = product.ToolingSetupCharges,
                    CustomerToolingSetupCharges = td.isEnterprise == true ? product.ToolingSetupCharges : null,
                    UnitPrice = null
                };
                priceBreaks.Add(pb);
            }

            if (qty.Qty7 != null)
            {
                pb = new PriceBreak()
                {
                    ProductId = product.Id,
                    Quantity = qty.Qty7.Value,
                    ToolingSetupCharges = product.ToolingSetupCharges,
                    CustomerToolingSetupCharges = td.isEnterprise == true ? product.ToolingSetupCharges : null,
                    UnitPrice = null
                };
                priceBreaks.Add(pb);
            }

            PriceBreakViewModel model = new PriceBreakViewModel
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductAvatarUri = product.AvatarUri,
                PriceBreakList = priceBreaks,
            };

            return model;
        }

        public string GetBidFailedReason(int bidId)
        {
            var bid = RFQBidService.FindRFQBidById(bidId);
            if (bid != null)
            {
                return bid.BidFailedReason;
            }
            return null;
        }
    }
}