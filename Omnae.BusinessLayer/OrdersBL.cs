using Common;
using Microsoft.Azure;
using Omnae.BlobStorage;
using Omnae.BusinessLayer.Models;
using Omnae.Common;
using Omnae.Data;
using Omnae.Model.Context;
using Omnae.Model.Models;
using Omnae.QuickBooks.QBO;
using Omnae.QuickBooks.ViewModels;
using Omnae.Service.Service;
using Omnae.Service.Service.Interfaces;
using Omnae.ViewModels;
using Rotativa;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using AutoMapper;
using Omnae.Common.Extensions;
using System.Data;
using System.Transactions;

namespace Omnae.BusinessLayer
{
    public class OrdersBL
    {
        public OrdersBL(ILogedUserContext userContext,
            ITaskDataService taskDataService,
            IOrderService orderService,
            IOrderStateTrackingService orderStateTrackingService,
            IProductStateTrackingService productStateTrackingService,
            ShipmentBL shipmentBL,
            IProductService productService,
            TaskDataCustomerBL taskDataCustomerBl,
            IShippingService shippingService,
            IDocumentService documentService,
            ICompanyService companyService,
            IPriceBreakService priceBreakService,
            IOmnaeInvoiceService omnaeInvoiceService,
            IQboTokensService qboTokensService,
            PaymentBL paymentBL,
            IAddressService addressService,
            NCReportService ncReportService,
            ShippingAccountService shippingAccountService,
            ICompaniesCreditRelationshipService companiesCreditRelationshipService,
            ShippingProfileService shippingProfileService,
            IProductSharingService productSharingService,
            IDocumentStorageService documentDocumentStorageService)
        {
            UserContext = userContext;
            TaskDataService = taskDataService;
            OrderService = orderService;
            OrderStateTrackingService = orderStateTrackingService;
            ProductStateTrackingService = productStateTrackingService;
            ProductService = productService;
            TaskDataCustomerBl = taskDataCustomerBl;
            ShippingService = shippingService;
            DocumentService = documentService;
            CompanyService = companyService;
            PriceBreakService = priceBreakService;
            OmnaeInvoiceService = omnaeInvoiceService;
            ShipmentBL = shipmentBL;
            QboTokensService = qboTokensService;
            this.paymentBL = paymentBL;
            this.addressService = addressService;
            this.ncReportService = ncReportService;
            this.shippingAccountService = shippingAccountService;
            this.companiesCreditRelationshipService = companiesCreditRelationshipService;
            ShippingProfileService = shippingProfileService;
            this.productSharingService = productSharingService;
            DocumentStorageService = documentDocumentStorageService;
        }

        public ILogedUserContext UserContext { get; }
        private ITaskDataService TaskDataService { get; }
        private IOrderService OrderService { get; }
        private IOrderStateTrackingService OrderStateTrackingService { get; }
        private IProductStateTrackingService ProductStateTrackingService { get; }
        private IProductService ProductService { get; }
        private TaskDataCustomerBL TaskDataCustomerBl { get; }
        private ShipmentBL ShipmentBL { get; }
        private readonly IShippingService ShippingService;
        private readonly IDocumentStorageService DocumentStorageService;
        private readonly IDocumentService DocumentService;
        private readonly ICompanyService CompanyService;
        private readonly IPriceBreakService PriceBreakService;
        private readonly IOmnaeInvoiceService OmnaeInvoiceService;
        private readonly IQboTokensService QboTokensService;
        private readonly PaymentBL paymentBL;
        private readonly IAddressService addressService;
        private readonly NCReportService ncReportService;
        private readonly ShippingAccountService shippingAccountService;
        private readonly ICompaniesCreditRelationshipService companiesCreditRelationshipService;
        private ShippingProfileService ShippingProfileService { get; }
        private readonly IProductSharingService productSharingService;

        ////////////////////////////////////////////////////////////////////////////

        public List<TaskViewModel> GetOrders()
        {
            Debug.Assert(UserContext.Company != null, "UserContext.Company != null");

            var taskVMList = new List<TaskViewModel>();
            var taskDatas = new List<TaskData>();
            switch (UserContext.UserType)
            {
                case USER_TYPE.Customer:
                    {
                        taskDatas = TaskDataService.FindTaskDataByCustomerId(UserContext.Company.Id).Where(x => x.StateId > (int)States.QuoteAccepted).ToList();
                        break;
                    }
                case USER_TYPE.Vendor:
                    {
                        taskDatas = TaskDataService.FindTaskDataByVendorId(UserContext.Company.Id).Where(x => x.StateId >= (int)States.QuoteAccepted).ToList();
                        break;
                    }
                case USER_TYPE.Admin:
                    {
                        break;
                    }
                default: throw new InvalidOperationException("Unknown User Type");
            }

            foreach (var td in taskDatas)
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


        public StateTrackingViewModel Details(int id)
        {
            var taskData = TaskDataService.FindById(id);
            var prods = TaskDataCustomerBl.GetCustomerProductsOnTaskId(id);
            if (prods == null)
            {
                return null;
            }

            Product product = taskData.Product; //prods.Where(x => x.Id == taskData.ProductId).FirstOrDefault();

            var orders = OrderService.FindOrderByTaskId(taskData.TaskId);

            Order order = null;
            List<OrderStateTracking> orderStates = null;
            List<ProductStateTracking> productStates = null;

            if (orders != null && orders.Count > 0)
            {
                order = orders.Last();
                orderStates = OrderStateTrackingService.FindOrderStateTrackingListByOrderId(order.Id).OrderBy(x => x.ModifiedUtc).ToList(); ;
            }
            productStates = ProductStateTrackingService.FindProductStateTrackingListByProductId(product.Id).OrderBy(x => x.ModifiedUtc).ToList();

            DocumentService.UpdateDocUrlWithSecurityToken(product?.Documents);

            var model = new StateTrackingViewModel()
            {
                TaskId = id,
                ProductId = taskData.ProductId,
                StateId = (States)taskData.StateId,
                OrderStateTrackings = orderStates,
                ProductStateTrackings = productStates,
                Order = order,
                Product = product,
                IsTagged = taskData.isTagged,
                LastUpdated = new StateLastUpdatedViewModel
                {
                    StateId = (States)taskData.StateId,
                    LastUpdated = taskData.ModifiedUtc,
                },
                isEnterprise = taskData.isEnterprise,
                UserType = UserContext.UserType,
            };

            return model;
        }

        public async Task<CreateOrderResult> CreateOrder(PlaceOrderViewModel model, ControllerContext context = null)
        {
            var option = new TransactionOptions() { Timeout = TimeSpan.FromMinutes(15), IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted };
            using (var ts = AsyncTransactionScope.StartNew(option))
            {
                string message = string.Empty;
                Product product = ProductService.FindProductById(model.ProductId);
                if (product == null)
                {
                    return new CreateOrderResult
                    {
                        Message = IndicatingMessages.ProductNotFound,
                    };
                }


                var isToSendToASpecificAddrProfile = (model.ShippingProfileId != null);
                var shippingProfileToSend = isToSendToASpecificAddrProfile
                                                ? ShippingProfileService.FindById(model.ShippingProfileId.Value)
                                                : null;
                var shippingAddr = isToSendToASpecificAddrProfile
                                    ? shippingProfileToSend?.Shipping
                                    : ShippingService.FindShippingByUserId(model.OrderCompanyId);

                var thisOrderHaveAddrJustForThisOrder = (model.Shipping != null);
                if (thisOrderHaveAddrJustForThisOrder)
                {
                    var newShippingAddr = Mapper.Map<Shipping>(model.Shipping);
                    newShippingAddr.CompanyId = newShippingAddr.Address.CompanyId = model.OrderCompanyId;
                    newShippingAddr.Address.isShipping = true;

                    addressService.FixIds(newShippingAddr.Address);

                    var sAddrId = ShippingService.AddShipping(newShippingAddr);
                    shippingAddr = ShippingService.FindShippingById(sAddrId); ;
                }
                if (shippingAddr == null)
                    throw new ValidationException("You must set the Shipping Address for a Order, or set a default Shipping Address for the company.");

                var currentDate = DateTime.UtcNow;

                if (model.IsNewWorkflow == false)
                {
                    // Prevent submitting duplicate orders
                    var sameOrders = OrderService.FindOrdersByProductId(model.ProductId);
                    if (sameOrders != null && sameOrders.Any())
                    {
                        var lastOrder = sameOrders.OrderBy(x => x.OrderDate).Last();
                        if (lastOrder.DesireShippingDate != null)
                        {
                            int days = int.Parse(ConfigurationManager.AppSettings["daysForDuplicateOrder"]);

                            // conditions for orders to be treated as duplicate
                            if (lastOrder.CustomerPONumber.Equals(model.PONumber, StringComparison.CurrentCultureIgnoreCase) &&
                                (model.DesireShippingDate != null ? lastOrder.DesireShippingDate.Value.Date == model.DesireShippingDate.Value.Date : false) &&
                                lastOrder.OrderDate.AddDays(days) > currentDate)
                            {
                                return new CreateOrderResult
                                {
                                    Message = IndicatingMessages.DuplicateOrder,
                                };
                            }
                        }
                    }
                }


                // Create a new TaskData for this new order
                TaskData taskData = TaskDataService.FindById(model.TaskId);
                int taskId = model.TaskId;

                CompaniesCreditRelationship credit = null;
                if (taskData.isEnterprise)
                {
                    credit = companiesCreditRelationshipService.FindCompaniesCreditRelationshipByCustomerIdVendorId(model.OrderCompanyId, product.VendorId.Value);
                }
                States state = (States)model.StateId;


                if (model.IsReorder)
                {
                    if (model.IsNewWorkflow == true)
                    {
                        state = States.ReOrderPaid;
                    }
                    else
                    {
                        state = States.ReOrderInitiated;
                        if (credit != null && credit.isTerm == true || taskData.isEnterprise == false && product.CustomerCompany.Term > 0)
                        {
                            state = States.ReOrderPaid;
                        }
                    }

                    var task = new TaskData()
                    {
                        StateId = (int)state,
                        ProductId = product.Id,
                        CreatedUtc = currentDate,
                        ModifiedUtc = currentDate,
                        UpdatedBy = UserContext.User.UserName,
                        CreatedByUserId = UserContext.UserId,
                        ModifiedByUserId = UserContext.UserId,
                        isEnterprise = model.isEnterprise,
                        IsRiskBuild = model.IsForRiskBuild,
                    };

                    taskId = TaskDataService.AddTaskData(task);
                }

                // if ToolingCharges is not null, separate orders need to be created
                List<Order> orderList = new List<Order>();
                int orderId = 0;
                Order orderQuantity = null;
                TaskData newTask = null;
                int? newTaskId = null;

                if (taskData.isEnterprise == true &&
                    model.IsReorder == false &&
                    model.IsForOrderTooling == false &&
                    (credit == null || credit.isTerm == false))
                {
                    newTask = new TaskData()
                    {
                        StateId = (int)model.StateId,
                        ProductId = product.Id,
                        CreatedUtc = currentDate,
                        ModifiedUtc = currentDate,
                        UpdatedBy = UserContext.User.UserName,
                        CreatedByUserId = UserContext.UserId,
                        ModifiedByUserId = UserContext.UserId,
                        isEnterprise = model.isEnterprise,
                        IsRiskBuild = model.IsForRiskBuild,
                    };
                    newTaskId = TaskDataService.AddTaskData(newTask);
                }

                int? shippingAccountIdToUse = model.ShippingAccountId ?? shippingProfileToSend?.ShippingAccountId;
                ShippingAccount sa = (shippingAccountIdToUse != null)
                                        ? shippingAccountService.FindById((int)shippingAccountIdToUse)
                                        : null;

                var tax = (decimal)(model.TaxRate ?? 0) * model.ToolingCharges;
                var orderCompanyName = CompanyService.FindCompanyById(model.OrderCompanyId)?.Name;
                if (model.IsForOrderTooling == true)
                {
                    orderQuantity = new Order
                    {
                        OrderDate = currentDate,
                        PartNumber = product.PartNumber,
                        //ShippedDate = DateTime.UtcNow.AddDays(product.ProductionLeadTime ?? 0),
                        ProductId = model.ProductId,
                        Quantity = model.Quantity != null && model.Quantity.Value > 0 ? model.Quantity.Value : model.NumberSampleIncluded,
                        SalesPrice = tax + model.ToolingCharges,
                        SalesTax = tax,
                        ShippingId = shippingAddr?.Id,
                        CustomerPONumber = model.PONumber,
                        IsForToolingOnly = model.IsForOrderTooling,
                        IsReorder = model.IsReorder,
                        Buyer = model.Buyer,
                        DesireShippingDate = model.DesireShippingDate,
                        EarliestShippingDate = model.EarliestShippingDate,
                        TaskId = taskId,

                        CarrierName = sa?.Carrier,
                        CarrierType = sa?.CarrierType,
                        ShippingAccountNumber = sa?.AccountNumber,

                        CreatedByUserId = UserContext.UserId,
                        ModifiedByUserId = UserContext.UserId,

                        OrderCompanyName = orderCompanyName,
                        CustomerId = model.OrderCompanyId,
                    };

                    try
                    {
                        orderId = OrderService.AddOrder(orderQuantity);
                    }
                    catch (Exception ex)
                    {
                        return new CreateOrderResult
                        {
                            Message = ex.RetrieveErrorMessage(),
                        };
                    }

                    orderList.Add(orderQuantity);
                }
                else
                {
                    if (model.IsReorder == false && model.ToolingCharges > 0)
                    {
                        Order orderToolingCharge = OrderService.FindOrderByTaskId(taskId).LastOrDefault(x => x.Quantity == model.NumberSampleIncluded);

                        if (orderToolingCharge != null)
                        {
                            orderToolingCharge.OrderDate = currentDate;
                            orderToolingCharge.PartNumber = product.PartNumber;
                            //orderToolingCharge.ShippedDate = DateTime.UtcNow.AddDays(product.ProductionLeadTime ?? 0);
                            orderToolingCharge.ProductId = model.ProductId;
                            orderToolingCharge.Quantity = model.NumberSampleIncluded;
                            orderToolingCharge.SalesPrice = tax + model.ToolingCharges;
                            orderToolingCharge.SalesTax = tax;
                            orderToolingCharge.ShippingId = shippingAddr?.Id;
                            orderToolingCharge.CustomerPONumber = model.PONumber;
                            orderToolingCharge.IsForToolingOnly = model.IsForOrderTooling;
                            orderToolingCharge.IsReorder = model.IsReorder;
                            orderToolingCharge.Buyer = model.Buyer;
                            orderToolingCharge.DesireShippingDate = model.DesireShippingDate;
                            orderToolingCharge.EarliestShippingDate = model.EarliestShippingDate;

                            orderToolingCharge.CarrierName = sa?.Carrier;
                            orderToolingCharge.CarrierType = sa?.CarrierType;
                            orderToolingCharge.ShippingAccountNumber = sa?.AccountNumber;

                            orderToolingCharge.ModifiedByUserId = UserContext.UserId;

                            orderToolingCharge.OrderCompanyName = orderCompanyName;
                            orderToolingCharge.CustomerId = model.OrderCompanyId;

                            try
                            {
                                OrderService.UpdateOrder(orderToolingCharge);
                            }
                            catch (Exception ex)
                            {
                                return new CreateOrderResult
                                {
                                    Message = ex.RetrieveErrorMessage(),
                                };
                            }
                        }
                        else
                        {
                            orderToolingCharge = new Order
                            {
                                OrderDate = currentDate,
                                PartNumber = product.PartNumber,
                                //ShippedDate = DateTime.UtcNow.AddDays(product.ProductionLeadTime ?? 0),
                                ProductId = model.ProductId,
                                Quantity = model.NumberSampleIncluded,
                                SalesPrice = tax + model.ToolingCharges,
                                SalesTax = tax,
                                ShippingId = shippingAddr?.Id,
                                CustomerPONumber = model.PONumber,
                                IsForToolingOnly = model.IsForOrderTooling,
                                IsReorder = model.IsReorder,
                                Buyer = model.Buyer,
                                DesireShippingDate = model.DesireShippingDate,
                                EarliestShippingDate = model.EarliestShippingDate,
                                TaskId = taskId,
                                CarrierName = sa?.Carrier,
                                CarrierType = sa?.CarrierType,
                                ShippingAccountNumber = sa?.AccountNumber,

                                CreatedByUserId = UserContext.UserId,
                                ModifiedByUserId = UserContext.UserId,

                                OrderCompanyName = orderCompanyName,
                                CustomerId = model.OrderCompanyId,
                            };

                            int orderToolingChargeId = 0;
                            try
                            {
                                orderToolingChargeId = OrderService.AddOrder(orderToolingCharge);
                            }
                            catch (Exception ex)
                            {
                                return new CreateOrderResult
                                {
                                    Message = ex.RetrieveErrorMessage(),
                                };
                            }
                        }
                        orderList.Add(orderToolingCharge);
                    }
                    var salesTax = model.UnitPrice * model.Quantity * (decimal)(model.TaxRate ?? 0);
                    var qtyOrderTotal = model.UnitPrice * model.Quantity + salesTax;
                    ProductSharing ps = null;
                    int? productSharingId = null;
                    if (product.CustomerId != model.OrderCompanyId)
                    {
                        ps = productSharingService.FindProductSharingByCompanyIdProductId(model.OrderCompanyId, product.Id);
                        productSharingId = ps?.Id;
                    }

                    orderQuantity = new Order
                    {
                        OrderDate = currentDate,
                        PartNumber = product.PartNumber,
                        //ShippedDate = DateTime.UtcNow.AddDays(product.ProductionLeadTime ?? 0),
                        ProductId = model.ProductId,
                        Quantity = model.Quantity ?? model.NumberSampleIncluded,
                        UnitPrice = model.UnitPrice,
                        SalesPrice = qtyOrderTotal,
                        SalesTax = salesTax,
                        ShipLeadingTime = null,
                        ShippingId = shippingAddr?.Id,
                        CustomerPONumber = model.PONumber,
                        IsForToolingOnly = model.IsForOrderTooling,
                        IsReorder = model.IsReorder,
                        Buyer = model.Buyer,
                        DesireShippingDate = model.DesireShippingDate,
                        EarliestShippingDate = model.EarliestShippingDate,
                        TaskId = newTaskId != null ? newTaskId.Value : taskId,
                        CarrierName = sa?.Carrier,
                        CarrierType = sa?.CarrierType,
                        ShippingAccountNumber = sa?.AccountNumber,
                        CreatedByUserId = UserContext.UserId,
                        ModifiedByUserId = UserContext.UserId,
                        ProductSharingId = productSharingId,
                        OrderCompanyName = orderCompanyName,
                        CustomerId = model.OrderCompanyId,
                    };
                    orderId = OrderService.AddOrder(orderQuantity);

                    orderList.Add(orderQuantity);

                    if (orderId == 0)
                    {
                        orderId = orderQuantity.Id;
                    }

                }

                Document purchaseOrderDoc = null;
                if (model.PurchaseOrder?.Any() == true)
                {
                    for (int i = 0; i < model.PurchaseOrder.Count(); i++)
                    {
                        var postedFile = model.PurchaseOrder[i];
                        if (!string.IsNullOrEmpty(postedFile.FileName))
                        {
                            if (postedFile.ContentType.Contains("application/pdf"))
                            {
                                string fileName = string.Empty;
                                if (i > 0)
                                {
                                    fileName = $"customer_po_oid-{orderId}_pid-{model.ProductId}_{(i + 1)}{Path.GetExtension(postedFile.FileName)}";
                                }
                                else
                                {
                                    fileName = $"customer_po_oid-{orderId}_pid-{model.ProductId}{Path.GetExtension(postedFile.FileName)}";
                                }
                                var docUri = DocumentStorageService.Upload(postedFile, fileName);
                                purchaseOrderDoc = DocumentService.FindDocumentByProductIdDocType(model.ProductId, DOCUMENT_TYPE.PO_PDF, ExpireTokenInfo.None).LastOrDefault(d => d.Name == fileName);

                                if (purchaseOrderDoc == null)
                                {
                                    purchaseOrderDoc = new Document()
                                    {
                                        TaskId = taskId,
                                        Version = 1,
                                        Name = fileName,
                                        DocUri = docUri,
                                        DocType = (int)DOCUMENT_TYPE.PO_PDF,
                                        ProductId = product.Id,
                                        //UpdatedBy = UserContext.User.UserName,
                                        CreatedUtc = DateTime.UtcNow,
                                        ModifiedUtc = DateTime.UtcNow
                                    };
                                    try
                                    {
                                        DocumentService.AddDocument(purchaseOrderDoc);
                                    }
                                    catch (Exception ex)
                                    {
                                        return new CreateOrderResult
                                        {
                                            Message = ex.RetrieveErrorMessage(),
                                        };
                                    }
                                }
                            }
                        }
                    }
                }

                var currentUserId = UserContext.User.UserId;
                Company customer = CompanyService.FindCompanyByUserId(currentUserId);

                // setup for Estimate/PurchaseOrder info               

                PurchaseOrderViewModel povm = new PurchaseOrderViewModel();
                decimal vendorUnitPrice = 0m;
                //povm.TaskId = taskId;
                povm.ProductId = product.Id;
                povm.QboId = customer.QboId;
                povm.PartNumber = product.PartNumber;
                povm.ProductCategory = product.Material;
                povm.PartRevision = product.PartNumberRevision;
                if (shippingAddr.Attention_FreeText == null)
                {
                    shippingAddr.Attention_FreeText = model.Buyer;
                    ShippingService.UpdateShipping(shippingAddr);
                }
                povm.Attention = model.Buyer;
                povm.CompanyName = customer.Name;
                povm.NumberSampleIncluded = model.NumberSampleIncluded;

                if (model.IsForOrderTooling)
                {
                    povm.CustomerToolingCharge = model.ToolingCharges ?? 0m;
                    povm.VendorToolingCharge = product.PriceBreak?.ToolingSetupCharges ?? 0m;
                }
                else
                {
                    if (model.UnitPrice == null || model.Quantity == null)
                    {
                        return new CreateOrderResult
                        {
                            Message = "Error: Unit Price or Quantity is null",
                        };
                    }
                    PriceBreak pb = null;
                    if (model.IsReorder == true)
                    {
                        pb = PriceBreakService.FindPriceBreakByProductId(product.Id)
                            .Where(x => x.Quantity <= model.Quantity.Value)
                            .LastOrDefault();
                    }
                    else
                    {
                        pb = PriceBreakService.FindPriceBreakByTaskIdProductIdQty(model.TaskId, product.Id, model.Quantity.Value);
                        if (pb == null)
                        {
                            pb = PriceBreakService.FindPriceBreakByProductIdQty(product.Id, model.Quantity.Value);
                        }
                    }


                    if (pb == null)
                    {
                        if (model.IsNewWorkflow == true)
                        {
                            var moq = PriceBreakService.FindMinimumOrderQuantity(product.Id);
                            if (moq == null)
                                return new CreateOrderResult
                                {
                                    Message = "Price break couldn't be found",
                                };

                            if (model.Quantity.Value <= moq)
                            {
                                pb = PriceBreakService.FindPriceBreakByProductIdQty(product.Id, moq.Value);
                            }

                        }
                        else
                            return new CreateOrderResult
                            {
                                Message = "Price break couldn't be found",
                            };
                    }


                    povm.Quantity = model.Quantity.Value;
                    povm.UnitOfMeasurement = pb?.UnitOfMeasurement;
                    povm.SalesTax = model.SalesTax.Value;
                    povm.UnitPrice = model.UnitPrice.Value;
                    vendorUnitPrice = pb?.VendorUnitPrice ?? 0;

                    povm.CustomerToolingCharge = model.IsReorder ? 0m : pb?.CustomerToolingSetupCharges ?? 0m;
                    povm.VendorToolingCharge = model.IsReorder ? 0m : pb?.ToolingSetupCharges ?? 0m;
                }
                povm.Total = model.Total ?? 0m;
                Company vendor = product.VendorCompany;
                povm.VendorId = product.VendorId;
                if (vendor != null)
                {
                    povm.VendorName = vendor.Name;
                }

                povm.CarrierName = orderQuantity.CarrierName;
                povm.TrackingNumber = orderQuantity.TrackingNumber;
                povm.HarmonizedCode = product.HarmonizedCode;


                povm.ProductName = product.Name?.Trim();
                povm.ProductDescription = product.Description?.Trim();
                povm.OrderDate = orderQuantity.OrderDate;
                povm.ShipDate = product.ProductionLeadTime != null ? orderQuantity.OrderDate.AddDays(product.ProductionLeadTime.Value) : model.DesireShippingDate.Value;
                povm.EarliestShippingDate = model.EarliestShippingDate;
                povm.DesireShippingDate = model.DesireShippingDate;

                if (taskData.isEnterprise == false)
                {
                    var address = customer.Address;
                    if (address != null)
                    {
                        povm.AddressLine1 = address.AddressLine1;
                        povm.AddressLine2 = address.AddressLine2;
                        povm.City = address.City;
                        povm.CountryId = address.CountryId;
                        if (address.Country != null)
                        {
                            povm.CountryName = address.Country.CountryName;
                        }
                        if (address.StateProvince != null)
                        {
                            povm.State = address.StateProvince.Abbreviation;
                        }
                        povm.PostalCode = address.PostalCode;
                        povm.ZipCode = address.ZipCode;
                        povm.PhoneNumber = shippingAddr?.PhoneNumber;
                        povm.EmailAddress = shippingAddr?.EmailAddress;


                        povm.CustomerId = product.CustomerId;
                        if (product.CustomerId != null)
                        {
                            povm.CustomerName = customer.Name;
                        }

                        povm.isBilling = address.isBilling;
                        povm.isShipping = address.isShipping; //TODO: Check if this needed to be chenged for shippingProfile.
                    }

                    // set up for PurchaseOrder
                    if (product.VendorId != null && vendor?.Address != null)
                    {
                        povm.VendorAddress_AddressLine1 = vendor.Address.AddressLine1;
                        povm.VendorAddress_AddressLine2 = vendor.Address.AddressLine2;
                        povm.VendorAddress_City = vendor.Address.City;
                        povm.VendorAddress_CountryName = vendor.Address.Country?.CountryName;
                        povm.VendorAddress_State = vendor.Address.StateProvince?.Name;
                        povm.VendorAddress_PostalCode = vendor.Address.PostalCode ?? vendor.Address.ZipCode;
                    }

                    var admin = CompanyService.FindAllCompanies().FirstOrDefault(x => x.Name == OMNAE_WEB.Admin);
                    if (admin != null)
                    {
                        povm.AdminAddress_AddressLine1 = admin.Address.AddressLine1;
                        povm.AdminAddress_AddressLine2 = admin.Address.AddressLine2;
                        povm.AdminAddress_City = admin.Address.City;
                        povm.AdminAddress_CountryName = admin.Address.Country?.CountryName;
                        povm.AdminAddress_State = admin.Address.StateProvince?.Name;
                        povm.AdminAddress_PostalCode = admin.Address.PostalCode;
                        povm.CompanyName = Administrator_Account.Name;
                        povm.CustomerPONumber = model.PONumber;

                        povm.EmailAddress = admin.Users.FirstOrDefault()?.Email ?? ""; //dbUser.Users.Where(u => u.Id == admin.UserId).First().Email;
                    }

                    // Get Shipping Address
                    if ((isToSendToASpecificAddrProfile && shippingAddr != null)
                         || ((address == null || address.isShipping == false) && shippingAddr != null))
                    {
                        povm.ShipAddress_AddressLine1 = shippingAddr.Address?.AddressLine1;
                        povm.ShipAddress_AddressLine2 = shippingAddr.Address?.AddressLine2;
                        povm.ShipAddress_City = shippingAddr.Address?.City;
                        povm.ShipAddress_CountryName = shippingAddr.Address?.Country?.CountryName;
                        povm.ShipAddress_State = shippingAddr.Address?.StateProvince?.Name;
                        povm.ShipAddress_PostalCode = shippingAddr.Address?.PostalCode ?? shippingAddr.Address?.ZipCode;
                    }

                    // Get Bill Address
                    if ((address == null || address.isBilling == false) && customer.BillAddress != null)
                    {
                        var billAddr = customer.BillAddress;
                        povm.BillAddress_AddressLine1 = billAddr.AddressLine1;
                        povm.BillAddress_AddressLine2 = billAddr.AddressLine2;
                        povm.BillAddress_City = billAddr.City;
                        povm.BillAddress_CountryName = billAddr.Country?.CountryName;
                        povm.BillAddress_State = billAddr.StateProvince?.Name;
                        povm.BillAddress_PostalCode = billAddr.PostalCode ?? billAddr.ZipCode;
                    }
                }

                try
                {
                    long poNumber = 0;
                    foreach (var ord in orderList)
                    {
                        if (model.IsReorder && ord.Quantity == 1)
                        {
                            continue;
                        }

                        povm.Quantity = ord.Quantity;
                        povm.UnitPrice = ord.UnitPrice ?? 0m;
                        povm.TaskId = ord.TaskId.Value; //ord.Quantity == model.NumberSampleIncluded || model.IsReorder == true ? taskId : newTaskId;
                        povm.VendorUnitPrice = ord.Quantity == model.NumberSampleIncluded ? 0m : vendorUnitPrice;
                        povm.SalesTax = ord.SalesTax ?? 0m;
                        povm.Total = ord.SalesPrice ?? 0m;

                        string estimateId = string.Empty;
                        string estimateNumber = string.Empty;

                        // If isEnterprise is true don't call QBO to create Estimate
                        if (!model.isEnterprise)
                        {
                            // Call QBO for creating Estimate

                            Tuple<string, string> tuple = await CallQBOCreateEstimate(povm);
                            estimateId = tuple.Item1;
                            estimateNumber = tuple.Item2;
                        }

                        // Store into OmnaeInvoice table for Customers

                        int dueDays = credit != null && credit.isTerm ? credit.TermDays.Value : 0;

                        // in case of reseller model
                        if (dueDays == 0 && product.CustomerCompany?.Term != null)
                        {
                            dueDays = product.CustomerCompany.Term.Value;
                        }



                        OmnaeInvoice invoice = OmnaeInvoiceService.FindOmnaeInvoiceByCompanyIdByOrderId(model.OrderCompanyId, ord.Id);
                        if (invoice != null)
                        {
                            invoice.OrderId = ord.Id;
                            invoice.Quantity = povm.Quantity;
                            invoice.UnitPrice = povm.Quantity == model.NumberSampleIncluded ? 0m : povm.UnitPrice;
                            invoice.ToolingSetupCharges = povm.Quantity == model.NumberSampleIncluded || povm.Quantity == 0 ? povm.CustomerToolingCharge : 0m;
                            invoice.SalesTax = povm.SalesTax;
                            invoice.InvoiceDate = DateTime.UtcNow;
                            invoice.DueDate = DateTime.UtcNow.AddDays(dueDays);
                            invoice.EstimateId = estimateId;
                            invoice.EstimateNumber = estimateNumber;
                            invoice.IsOpen = true;
                            invoice.Term = credit != null && credit.isTerm ? credit.TermDays : customer.Term;
                            invoice.PODocUri = purchaseOrderDoc?.DocUri?.RemoveQueryStringFromUrl();
                            invoice.PONumber = model.PONumber;

                            OmnaeInvoiceService.UpdateOmnaeInvoice(invoice);
                        }
                        else
                        {
                            invoice = new OmnaeInvoice
                            {
                                OrderId = ord.Id,
                                TaskId = ord.TaskId.Value, //ord.Quantity == model.NumberSampleIncluded || model.IsReorder == true ? taskId : newTaskId,
                                CompanyId = model.OrderCompanyId,
                                UserType = (int)USER_TYPE.Customer,
                                InvoiceDate = DateTime.UtcNow,
                                DueDate = DateTime.UtcNow.AddDays(dueDays), // ??
                                Quantity = povm.Quantity,
                                UnitPrice = povm.Quantity == model.NumberSampleIncluded ? 0m : povm.UnitPrice,
                                ToolingSetupCharges = povm.Quantity == model.NumberSampleIncluded || povm.Quantity == 0 ? povm.CustomerToolingCharge : 0m,
                                SalesTax = povm.SalesTax,
                                ProductId = product.Id,
                                EstimateId = estimateId,
                                EstimateNumber = estimateNumber,
                                IsOpen = true,
                                Term = credit != null && credit.isTerm ? credit.TermDays : customer.Term,
                                PONumber = model.PONumber,
                                PODocUri = purchaseOrderDoc?.DocUri?.RemoveQueryStringFromUrl(),
                            };

                            OmnaeInvoiceService.AddOmnaeInvoice(invoice);
                        }

                        OmnaeInvoice invoice2 = OmnaeInvoiceService.FindOmnaeInvoiceByCompanyIdByOrderId(povm.VendorId.Value, ord.Id);
                        if (poNumber == 0) // only take first time value
                        {
                            var inv = OmnaeInvoiceService.FindOmnaeInvoiceList().Where(x => x.UserType == (int)USER_TYPE.Vendor).OrderBy(x => x.PONumber).LastOrDefault();
                            if (inv != null)
                            {
                                long.TryParse(inv.PONumber, out poNumber);
                            }
                            else
                            {
                                poNumber = 1000;
                            }
                        }
                        poNumber++;
                        if (invoice2 != null)
                        {
                            invoice2.OrderId = ord.Id;
                            invoice2.Quantity = povm.Quantity;
                            invoice2.UnitPrice = povm.Quantity == model.NumberSampleIncluded ? 0m : povm.UnitPrice;
                            invoice2.ToolingSetupCharges = povm.Quantity == model.NumberSampleIncluded || povm.Quantity == 0 ? povm.VendorToolingCharge : 0m;
                            invoice2.InvoiceDate = DateTime.UtcNow;
                            invoice2.DueDate = DateTime.UtcNow.AddDays(dueDays);
                            invoice2.IsOpen = true;
                            invoice2.Term = credit != null && credit.isTerm ? credit.TermDays : povm.Term;
                            invoice2.PONumber = poNumber.ToString();
                            OmnaeInvoiceService.UpdateOmnaeInvoice(invoice2);
                        }
                        else
                        {
                            // Store invoice to Omnae DB
                            invoice2 = new OmnaeInvoice
                            {
                                TaskId = ord.TaskId.Value, //ord.Quantity == model.NumberSampleIncluded || model.IsReorder == true ? taskId : newTaskId,
                                OrderId = ord.Id,
                                CompanyId = povm.VendorId.Value,
                                UserType = (int)USER_TYPE.Vendor,
                                InvoiceDate = DateTime.UtcNow,
                                DueDate = DateTime.UtcNow.AddDays(dueDays), // ?? to be confirmed
                                Quantity = povm.Quantity,
                                UnitPrice = povm.VendorUnitPrice,
                                ToolingSetupCharges = povm.Quantity == model.NumberSampleIncluded || povm.Quantity == 0 ? povm.VendorToolingCharge : 0m,
                                ProductId = product.Id,
                                IsOpen = true,
                                Term = credit != null && credit.isTerm ? credit.TermDays : povm.Term,
                                PONumber = poNumber.ToString(),
                            };
                            OmnaeInvoiceService.AddOmnaeInvoice(invoice2);
                        }

                        // Export this purchaseorder to PDF only when this is Reseller
                        if (taskData.isEnterprise == false)
                        {
                            povm.CustomerPONumber = invoice.PONumber;
                            povm.PONumber = invoice2.PONumber;
                            povm.DesireShippingDate = model.DesireShippingDate;
                            povm.EarliestShippingDate = model.EarliestShippingDate;

                            double salesTax = ShipmentBL.GetSalesTax(vendor.Address);
                            if (povm.Quantity == model.NumberSampleIncluded || povm.Quantity == 0)
                            {
                                povm.Total = povm.VendorToolingCharge;
                            }
                            else
                            {
                                povm.Total = povm.Quantity * povm.VendorUnitPrice;
                            }
                            povm.SalesTax = povm.Total * (decimal)salesTax;
                            povm.OrderId = ord.Id;
                            povm.NumberSampleIncluded = model.NumberSampleIncluded;
                            povm.CompanyLogoUri = customer.CompanyLogoUri;

                            if (context != null)
                            {
                                var vendorPO = ExportVendorPurchaseOrderToPdf(povm, context);
                                invoice2.PODocUri = vendorPO.DocUri; //TODO: Remove the Token here.
                                OmnaeInvoiceService.UpdateOmnaeInvoice(invoice2);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //BUG: Dont cath and ignore the Exception
                    return new CreateOrderResult
                    {
                        Message = ex.RetrieveErrorMessage(),
                    };
                }


                CreateOrderResult ret = new CreateOrderResult
                {
                    Message = message,
                    CustomerName = customer.Name,
                    TaskId = taskId,
                    OrderId = orderId,
                    Term = customer.Term ?? 0,
                };

                ts.Complete();
                return ret;
            }
        }

        public string PostPlaceOrderActionWithTerm(Order order, string customerPONumber, bool isReorder)
        {
            var taskData = order.TaskData; // TaskDataService.FindById(taskId);
            string subject = "Omnae.com Order Confirmation {0}, {1}";
            if (isReorder)
            {
                subject = "Omnae.com Re-Order Initiated {0}, {1}";
            }
            else
            {
                taskData.StateId = (int)States.OrderPaid;
                taskData.UpdatedBy = UserContext.User.UserName;
                taskData.ModifiedUtc = DateTime.UtcNow;
                taskData.ModifiedByUserId = UserContext.UserId;
                TaskDataService.Update(taskData);
            }

            // Add to OrderStateTracking table with new state and time stamp
            OrderStateTracking orderState = new OrderStateTracking()
            {
                OrderId = order.Id,
                StateId = taskData.StateId,
                UpdatedBy = taskData.UpdatedBy,
                ModifiedUtc = taskData.ModifiedUtc,
                CreatedByUserId = taskData.CreatedByUserId,
                ModifiedByUserId = taskData.ModifiedByUserId,
            };
            OrderStateTrackingService.AddOrderStateTracking(orderState);

            // Fill in ProductStateTracking table with product id, state id ...
            ProductStateTracking productState = new ProductStateTracking()
            {
                ProductId = taskData.ProductId.Value,
                StateId = taskData.StateId,
                UpdatedBy = taskData.UpdatedBy,
                ModifiedUtc = taskData.ModifiedUtc,
                CreatedByUserId = taskData.CreatedByUserId,
                ModifiedByUserId = taskData.ModifiedByUserId,
            };
            ProductStateTrackingService.AddProductStateTracking(productState);

            // Disable notification on state changes when in Enterprise mode

            if (taskData.isEnterprise == false)
            {
                try
                {
                    var orders = OrderService.FindOrderByTaskId(order.TaskId.Value);
                    foreach (var ord in orders)
                    {
                        paymentBL.NotifyOnPayment(ord, (States)taskData.StateId, subject);
                    }
                }
                catch (Exception ex)
                {
                    string msg = ex.RetrieveErrorMessage();
                    return msg;
                }
            }

            return null;
        }

        public string PostPlaceOrderAction(Order order, string customerPONumber, bool isReorder)
        {
            using (var ts = AsyncTransactionScope.StartNew())
            {
                var taskData = order.TaskData; // TaskDataService.FindById(taskId);
                string subject = "Omnae.com Order Initiated {0}, {1}";
                if (isReorder)
                {
                    subject = "Omnae.com Re-Order Initiated {0}, {1}";
                }
                else
                {
                    taskData.StateId = (int)States.OrderInitiated;
                    taskData.UpdatedBy = UserContext.User.UserName;
                    taskData.ModifiedUtc = DateTime.UtcNow;
                    taskData.ModifiedByUserId = UserContext.UserId;
                    TaskDataService.Update(taskData);
                }


                // Add to OrderStateTracking table with new state and time stamp
                OrderStateTracking orderState = new OrderStateTracking()
                {
                    OrderId = order.Id,
                    StateId = taskData.StateId,
                    UpdatedBy = taskData.UpdatedBy,
                    ModifiedUtc = taskData.ModifiedUtc,
                    CreatedByUserId = taskData.CreatedByUserId,
                    ModifiedByUserId = taskData.ModifiedByUserId,
                };
                OrderStateTrackingService.AddOrderStateTracking(orderState);

                // Fill in ProductStateTracking table with product id, state id ...
                ProductStateTracking productState = new ProductStateTracking()
                {
                    ProductId = taskData.ProductId.Value,
                    StateId = taskData.StateId,
                    UpdatedBy = taskData.UpdatedBy,
                    ModifiedUtc = taskData.ModifiedUtc,
                    CreatedByUserId = taskData.CreatedByUserId,
                    ModifiedByUserId = taskData.ModifiedByUserId,
                };
                ProductStateTrackingService.AddProductStateTracking(productState);

                // Disable notification on state changes when in Enterprise mode

                if (taskData.isEnterprise == false)
                {
                    try
                    {
                        paymentBL.NotifyOnPayment(order, (States)taskData.StateId, subject);
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.RetrieveErrorMessage();
                        return msg;
                    }
                }
                ts.Complete();
                return null;
            }
        }

        public void PostDoProductionRun(Order order, string customerPONumber, bool isReorder, bool isInStock = false)
        {
            var taskData = order.TaskData;
            string subject = "Omnae.com Production Run Started for Order {0}, {1}";
            if (isReorder)
            {
                subject = "Omnae.com Production Run Started for Re-Order {0}, {1}";
            }
            else 
            {
                if (isInStock)
                {
                    subject = "Omnae.com Product is Already in Stock for this Order {0}, {1}";
                    taskData.StateId = (int)States.ProductionComplete;
                }
                else
                {
                    taskData.StateId = (int)States.OrderPaid;
                }                
                taskData.UpdatedBy = UserContext.User.UserName;
                taskData.ModifiedUtc = DateTime.UtcNow;
                taskData.ModifiedByUserId = UserContext.UserId;
                TaskDataService.Update(taskData);
            }
            

            // Add to OrderStateTracking table with new state and time stamp
            OrderStateTracking orderState = new OrderStateTracking()
            {
                OrderId = order.Id,
                StateId = taskData.StateId,
                UpdatedBy = taskData.UpdatedBy,
                ModifiedUtc = taskData.ModifiedUtc,
                CreatedByUserId = taskData.CreatedByUserId,
                ModifiedByUserId = taskData.ModifiedByUserId,
            };
            OrderStateTrackingService.AddOrderStateTracking(orderState);

            // Fill in ProductStateTracking table with product id, state id ...
            ProductStateTracking productState = new ProductStateTracking()
            {
                ProductId = taskData.ProductId.Value,
                StateId = taskData.StateId,
                UpdatedBy = taskData.UpdatedBy,
                ModifiedUtc = taskData.ModifiedUtc,
                CreatedByUserId = taskData.CreatedByUserId,
                ModifiedByUserId = taskData.ModifiedByUserId,
            };
            ProductStateTrackingService.AddProductStateTracking(productState);

            // Disable new work-flow place order creation notification emails.
#if false
            var orders = OrderService.FindOrderByTaskId(order.TaskId.Value);
            foreach (var ord in orders)
            {
                paymentBL.NotifyOnPayment(ord, (States)taskData.StateId, subject);
            }
#endif
        }


        public async Task<Tuple<string, string>> CallQBOCreateEstimate(PurchaseOrderViewModel model)
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
                    UpdatedBy = UserContext.User.UserName,
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

        private Document ExportVendorPurchaseOrderToPdf(PurchaseOrderViewModel model, ControllerContext context)
        {
            if (model == null)
                return null;

            var fileName = $"vendor_po_vid-{model.VendorId}_tid-{model.TaskId}_pid-{model.ProductId}_oid-{model.OrderId}.pdf";

            var pdf = new ActionAsPdf("CreateVendorPurchaseOrder", model)
            {
                PageOrientation = Rotativa.Options.Orientation.Landscape,
                PageMargins = { Left = 5, Right = 5 },
                FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName,
            };
            var pdfBytes = pdf.BuildFile(context);
            var ms = new MemoryStream(pdfBytes);

            var docUri = DocumentStorageService.Upload(ms, fileName);
            var doc = new Document()
            {
                TaskId = model.TaskId,
                Version = 1,
                Name = fileName,
                DocUri = docUri,
                DocType = (int)DOCUMENT_TYPE.QBO_PURCHASEORDER_PDF,
                ProductId = model.ProductId,
                UpdatedBy = UserContext.User.UserName,
                CreatedUtc = DateTime.UtcNow,
                CreatedByUserId = UserContext.UserId,
                ModifiedUtc = DateTime.UtcNow
            };

            var d = DocumentService.FindDocumentByProductId(model.ProductId, ExpireTokenInfo.None).FirstOrDefault(x => x.Name == fileName);
            if (d == null)
            {
                DocumentService.AddDocument(doc);
            }

            return doc;
        }

        public NcrInfoViewModel NcrHistory(NCReport ncr)
        {
            NcrInfoViewModel model = new NcrInfoViewModel
            {
                Id = ncr.Id,
                StateId = (States)ncr.StateId,
                NCRNumber = ncr.NCRNumber,
                DateInitiated = ncr.NCDetectedDate,
                RootCause = ncr.RootCause != null ? Enum.GetName(typeof(NC_ROOT_CAUSE), ncr.RootCause) : null,
                Vendor = ncr.VendorId != null ? CompanyService.FindCompanyById(ncr.VendorId.Value).Name : null,
                Cost = ncr.Cost,
                DateClosed = ncr.DateNcrClosed,
                UserType = UserContext.UserType,
            };

            var trackNCRCustomerApproval = OrderStateTrackingService.FindOrderStateTrackingByNcrId(ncr.Id)
                .Where(x => x.StateId == (int)States.NCRVendorCorrectivePartsInProduction).FirstOrDefault();

            var orderNCRVendorRootCauseAnalysis = OrderStateTrackingService.FindOrderStateTrackingByNcrId(ncr.Id)
                .Where(x => x.StateId == (int)States.NCRVendorRootCauseAnalysis).FirstOrDefault();

            model.NCRApprovalDate = trackNCRCustomerApproval?.ModifiedUtc;
            model.RootCauseAnalysisDate = orderNCRVendorRootCauseAnalysis?.ModifiedUtc;

            return model;
        }
    }
}