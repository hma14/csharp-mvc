using Common;
using Microsoft.Azure;
using Omnae.BlobStorage;
using Omnae.BusinessLayer.Models;
using Omnae.Common;
using Omnae.Model.Context;
using Omnae.Model.Models;
using Omnae.QuickBooks.QBO;
using Omnae.QuickBooks.ViewModels;
using Omnae.Service.Service.Interfaces;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;

namespace Omnae.BusinessLayer
{
    public class OrdersBLExt
    {
        public OrdersBLExt(
            ITaskDataService taskDataService,
            IOrderService orderService,
            IOrderStateTrackingService orderStateTrackingService,
            IProductStateTrackingService productStateTrackingService,
            ShipmentBL shipmentBL,
            IProductService productService,
            TaskDataCustomerBL taskDataCustomerBl,
            IShippingService shippingService,
            IBlobStorageService blobStorageService,
            IDocumentService documentService,
            ICompanyService companyService,
            IPriceBreakService priceBreakService,
            IOmnaeInvoiceService omnaeInvoiceService,
            IQboTokensService qboTokensService,
            OrdersBL ordersBL)
        {
            TaskDataService = taskDataService;
            OrderService = orderService;
            OrderStateTrackingService = orderStateTrackingService;
            ProductStateTrackingService = productStateTrackingService;
            ProductService = productService;
            TaskDataCustomerBl = taskDataCustomerBl;
            ShippingService = shippingService;
            BlobStorageService = blobStorageService;
            DocumentService = documentService;
            CompanyService = companyService;
            PriceBreakService = priceBreakService;
            OmnaeInvoiceService = omnaeInvoiceService;
            ShipmentBL = shipmentBL;
            QboTokensService = qboTokensService;
            OrdersBL = ordersBL;
        }

        private ITaskDataService TaskDataService { get; }
        private IOrderService OrderService { get; }
        private IOrderStateTrackingService OrderStateTrackingService { get; }
        private IProductStateTrackingService ProductStateTrackingService { get; }
        private IProductService ProductService { get; }
        private TaskDataCustomerBL TaskDataCustomerBl { get; }
        private ShipmentBL ShipmentBL { get; }
        private readonly IShippingService ShippingService;
        private readonly IBlobStorageService BlobStorageService;
        private readonly IDocumentService DocumentService;
        private readonly ICompanyService CompanyService;
        private readonly IPriceBreakService PriceBreakService;
        private readonly IOmnaeInvoiceService OmnaeInvoiceService;
        private readonly IQboTokensService QboTokensService;
        private readonly OrdersBL OrdersBL;


        ////////////////////////////////////////////////////////////////////////////


        public async Task<CreateOrderResult> CreateOrder(PlaceOrderViewModel model, ControllerContext context = null)
        {
            var UserContext = OrdersBL.UserContext;
            string message = string.Empty;
            Product product = ProductService.FindProductById(model.ProductId);
            if (product == null)
            {
                return new CreateOrderResult
                {
                    Message = "Couldn't find product",
                };
            }

            var shipping = ShippingService.FindShippingByUserId(product.CustomerId.Value);
            var currentDate = DateTime.UtcNow;

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
                        lastOrder.DesireShippingDate.Value.Date == model.DesireShippingDate.Value.Date &&
                        lastOrder.OrderDate.AddDays(days) > currentDate)
                    {
                        return new CreateOrderResult
                        {
                            Message = "You are placing a duplicate order! Please try to place a different order!",
                        };
                    }
                }
            }

            // Create a new TaskData for this new order
            TaskData taskData = TaskDataService.FindById(model.TaskId);
            int taskId = model.TaskId;
            States state = model.StateId;

            if (model.IsReorder)
            {
                state = States.ReOrderPaid;
                if (product.CustomerCompany.Term == null)
                {
                    state = States.ReOrderInitiated;
                }
                taskData = new TaskData()
                {
                    StateId = (int)state,
                    ProductId = product.Id,
                    CreatedUtc = currentDate,
                    ModifiedUtc = currentDate,
                    UpdatedBy = UserContext.User.UserName,
                    CreatedByUserId = UserContext.UserId,
                    ModifiedByUserId = UserContext.UserId,
                    isEnterprise = model.isEnterprise,
                };

                taskId = TaskDataService.AddTaskData(taskData);
            }

            // if ToolingCharges is not null, separate orders need to be created
            List<Order> orderList = new List<Order>();
            int orderId = 0;

            if (model.ToolingCharges > 0)
            {
                Order orderToolingCharge = OrderService.FindOrderByTaskId(taskId).LastOrDefault(x => x.Quantity == 1);
                if (orderToolingCharge != null)
                {
                    orderToolingCharge.OrderDate = currentDate;
                    orderToolingCharge.PartNumber = product.PartNumber;
                    orderToolingCharge.ShippedDate = DateTime.UtcNow.AddDays(product.ProductionLeadTime ?? 0);
                    orderToolingCharge.ProductId = model.ProductId;
                    orderToolingCharge.SalesPrice = model.ToolingCharges;
                    orderToolingCharge.SalesTax = model.TaxRate != null ? (decimal)model.TaxRate * model.ToolingCharges : 0m;
                    orderToolingCharge.ShippingId = shipping?.Id;
                    orderToolingCharge.CustomerPONumber = model.PONumber;
                    orderToolingCharge.IsForToolingOnly = model.IsForOrderTooling;
                    orderToolingCharge.IsReorder = model.IsReorder;
                    orderToolingCharge.Buyer = model.Buyer;
                    orderToolingCharge.DesireShippingDate = model.DesireShippingDate;
                    orderToolingCharge.EarliestShippingDate = model.EarliestShippingDate;
                    orderToolingCharge.CarrierName = model.ShipVia;
                    orderToolingCharge.ModifiedByUserId = OrdersBL.UserContext.UserId;

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
                        ShippedDate = DateTime.UtcNow.AddDays(product.ProductionLeadTime ?? 0),
                        ProductId = model.ProductId,
                        Quantity = 1,
                        SalesPrice = model.ToolingCharges,
                        SalesTax = model.TaxRate != null ? (decimal)model.TaxRate * model.ToolingCharges : 0m,
                        ShippingId = shipping?.Id,
                        CustomerPONumber = model.PONumber,
                        IsForToolingOnly = model.IsForOrderTooling,
                        IsReorder = model.IsReorder,
                        Buyer = model.Buyer,
                        DesireShippingDate = model.DesireShippingDate,
                        EarliestShippingDate = model.EarliestShippingDate,
                        TaskId = taskId,
                        CarrierName = model.ShipVia,
                        CreatedByUserId = UserContext.UserId,
                        ModifiedByUserId = UserContext.UserId,
                    };

                    try
                    {
                        OrderService.AddOrder(orderToolingCharge);
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

            Order orderQuantity = null;
            if (taskData.Orders != null &&
                taskData.Orders.Count > 0 &&
                (orderQuantity = taskData.Orders.FirstOrDefault(x => x.Quantity > 1)) != null)
            {
                orderQuantity.OrderDate = currentDate;
                orderQuantity.PartNumber = product.PartNumber;
                orderQuantity.ShippedDate = DateTime.UtcNow.AddDays(product.ProductionLeadTime ?? 0);
                orderQuantity.ProductId = model.ProductId;
                orderQuantity.Quantity = model.Quantity ?? 0;
                orderQuantity.UnitPrice = model.UnitPrice;
                orderQuantity.SalesPrice = model.Total;
                orderQuantity.SalesTax = model.SalesTax;
                orderQuantity.ShipLeadingTime = null; // ?
                orderQuantity.ShippingId = shipping?.Id;
                orderQuantity.CustomerPONumber = model.PONumber;
                orderQuantity.IsForToolingOnly = model.IsForOrderTooling;
                orderQuantity.IsReorder = model.IsReorder;
                orderQuantity.Buyer = model.Buyer;
                orderQuantity.DesireShippingDate = model.DesireShippingDate;
                orderQuantity.EarliestShippingDate = model.EarliestShippingDate;
                orderQuantity.CarrierName = model.ShipVia;
                orderQuantity.ModifiedByUserId = UserContext.UserId;

                OrderService.UpdateOrder(orderQuantity);
            }
            else
            {
                orderQuantity = new Order
                {
                    OrderDate = currentDate,
                    PartNumber = product.PartNumber,
                    ShippedDate = DateTime.UtcNow.AddDays(product.ProductionLeadTime ?? 0),
                    ProductId = model.ProductId,
                    Quantity = model.Quantity ?? 0,
                    UnitPrice = model.UnitPrice,
                    SalesPrice = model.Total,
                    SalesTax = model.SalesTax,
                    ShipLeadingTime = null,
                    ShippingId = shipping?.Id,
                    CustomerPONumber = model.PONumber,
                    IsForToolingOnly = model.IsForOrderTooling,
                    IsReorder = model.IsReorder,
                    Buyer = model.Buyer,
                    DesireShippingDate = model.DesireShippingDate,
                    EarliestShippingDate = model.EarliestShippingDate,
                    TaskId = taskId,
                    CarrierName = model.ShipVia,
                    CreatedByUserId = UserContext.UserId,
                    ModifiedByUserId = UserContext.UserId,
                };
                orderId = OrderService.AddOrder(orderQuantity);
            }
            orderList.Add(orderQuantity);

            if (orderId == 0)
            {
                orderId = orderQuantity.Id;
            }

            string containerName = CloudConfigurationManager.GetSetting("StorageContainer");
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
                            string fileName = $"po_orderId_{orderId}_productId_{model.ProductId}_{(i + 1)}{Path.GetExtension(postedFile.FileName)}";
                            var docUri = BlobStorageService.BlobStorageUpload(postedFile, containerName, fileName);
                            purchaseOrderDoc = DocumentService.FindDocumentByProductId(model.ProductId, DOCUMENT_TYPE.PO_PDF).LastOrDefault(d => d.Name == fileName);

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
                                    UpdatedBy = UserContext.User.UserName,
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
                        //else
                        //{
                        //    returnString = "Invalid file type - must be PDF file!";
                        //}
                    }
                }
            }

            var currentUserId = UserContext.User.UserId;
            Company customer = CompanyService.FindCompanyByUserId(currentUserId);

            // setup for Estimate/PurchaseOrder info               

            PurchaseOrderViewModel povm = new PurchaseOrderViewModel();
            decimal vendorUnitPrice = 0m;
            povm.TaskId = taskId;
            povm.ProductId = product.Id;
            povm.QboId = customer.QboId;
            povm.PartNumber = product.PartNumber;
            povm.ProductCategory = product.Material;
            povm.PartRevision = product.PartRevision?.Name;
            povm.Attention = shipping?.Attention_FreeText;
            povm.CompanyName = customer.Name;

            if (model.IsForOrderTooling)
            {
                povm.CustomerToolingCharge = model.ToolingCharges ?? 0m;
                povm.VendorToolingCharge = product.ToolingSetupCharges ?? 0m;
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
                var pb = PriceBreakService.FindPriceBreakByTaskIdProductIdQty(taskId, product.Id, model.Quantity.Value);
                if (pb == null)
                {
                    pb = PriceBreakService.FindPriceBreakByProductIdQty(product.Id, model.Quantity.Value);
                }
                if (pb == null)
                {
                    return new CreateOrderResult
                    {
                        Message = "Price break counldn't be found",
                    };
                }


                povm.Quantity = model.Quantity.Value;
                povm.SalesTax = model.SalesTax.Value;
                povm.UnitPrice = model.UnitPrice.Value;
                vendorUnitPrice = pb.VendorUnitPrice;

                povm.CustomerToolingCharge = model.IsReorder ? 0m : pb?.CustomerToolingSetupCharges ?? 0m;
                povm.VendorToolingCharge = model.IsReorder ? 0m : pb?.ToolingSetupCharges ?? 0m;
            }
            povm.Total = model.Total.Value;

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
                povm.PhoneNumber = shipping?.PhoneNumber;
                povm.EmailAddress = shipping?.EmailAddress;


                povm.CustomerId = product.CustomerId;
                if (product.CustomerId != null)
                {
                    povm.CustomerName = customer.Name;
                }

                povm.isBilling = address.isBilling;
                povm.isShipping = address.isShipping;
            }

            Company vendor = product.VendorCompany;
            povm.VendorId = product.VendorId;
            if (vendor != null)
            {
                povm.VendorName = vendor.Name;
            }

            povm.CarrierName = orderQuantity.CarrierName;
            povm.TrackingNumber = orderQuantity.TrackingNumber;
            povm.HarmonizedCode = product.HarmonizedCode;


            povm.ProductName = product.Name.Trim();
            povm.ProductDescription = product.Description?.Trim();
            povm.OrderDate = orderQuantity.OrderDate;
            povm.ShipDate = product.ProductionLeadTime != null ? orderQuantity.OrderDate.AddDays(product.ProductionLeadTime.Value) : model.DesireShippingDate.Value;
            povm.EarliestShippingDate = model.EarliestShippingDate;
            povm.DesireShippingDate = model.DesireShippingDate;

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

            try
            {
                long poNumber = 0;
                foreach (var ord in orderList)
                {
                    povm.Quantity = ord.Quantity;
                    povm.UnitPrice = ord.UnitPrice ?? 0m;
                    povm.VendorUnitPrice = ord.Quantity == 1 ? 0m : vendorUnitPrice;
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

                    int dueDays = 0;
                    if (product.CustomerCompany != null && product.CustomerCompany.Term != null)
                    {
                        dueDays = product.CustomerCompany.Term.Value;
                    }

                    OmnaeInvoice invoice = OmnaeInvoiceService.FindOmnaeInvoiceByCompanyIdByOrderId(povm.CustomerId.Value, ord.Id);
                    if (invoice != null)
                    {
                        invoice.OrderId = ord.Id;
                        invoice.Quantity = povm.Quantity;
                        invoice.UnitPrice = povm.UnitPrice;
                        invoice.ToolingSetupCharges = povm.Quantity == 1 ? povm.CustomerToolingCharge : 0m;
                        invoice.SalesTax = povm.SalesTax;
                        invoice.InvoiceDate = DateTime.UtcNow;
                        invoice.DueDate = DateTime.UtcNow.AddDays(dueDays);
                        invoice.EstimateId = estimateId;
                        invoice.EstimateNumber = estimateNumber;
                        invoice.IsOpen = true;
                        invoice.Term = customer.Term;
                        invoice.PODocUri = purchaseOrderDoc != null ? purchaseOrderDoc.DocUri : null;
                        invoice.PONumber = model.PONumber;

                        OmnaeInvoiceService.UpdateOmnaeInvoice(invoice);
                    }
                    else
                    {
                        invoice = new OmnaeInvoice
                        {
                            OrderId = ord.Id,
                            TaskId = taskId,
                            CompanyId = povm.CustomerId.Value,
                            UserType = (int)USER_TYPE.Customer,
                            InvoiceDate = DateTime.UtcNow,
                            DueDate = DateTime.UtcNow.AddDays(dueDays), // ??
                            Quantity = povm.Quantity,
                            UnitPrice = povm.UnitPrice,
                            ToolingSetupCharges = povm.Quantity == 1 ? povm.CustomerToolingCharge : 0m,
                            SalesTax = povm.SalesTax,
                            ProductId = product.Id,
                            EstimateId = estimateId,
                            EstimateNumber = estimateNumber,
                            IsOpen = true,
                            Term = customer.Term,
                            PONumber = model.PONumber,
                            PODocUri = purchaseOrderDoc != null ? purchaseOrderDoc.DocUri : null,
                        };

                        OmnaeInvoiceService.AddOmnaeInvoice(invoice);
                    }


                    // creating PurchaseOrder for Vendors

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
                        poNumber++;
                    }

                    if (invoice2 != null)
                    {
                        invoice2.OrderId = ord.Id;
                        invoice2.Quantity = povm.Quantity;
                        invoice2.UnitPrice = povm.VendorUnitPrice;
                        invoice2.ToolingSetupCharges = povm.Quantity == 1 ? povm.VendorToolingCharge : 0m;
                        invoice2.InvoiceDate = DateTime.UtcNow;
                        invoice2.DueDate = DateTime.UtcNow.AddDays(dueDays);
                        invoice2.IsOpen = true;
                        invoice2.Term = povm.Term;
                        invoice2.PONumber = poNumber.ToString();
                        OmnaeInvoiceService.UpdateOmnaeInvoice(invoice2);

                    }
                    else
                    {
                        // Store invoice to Omnae DB
                        invoice2 = new OmnaeInvoice
                        {
                            TaskId = taskId,
                            OrderId = ord.Id,
                            CompanyId = povm.VendorId.Value,
                            UserType = (int)USER_TYPE.Vendor,
                            InvoiceDate = DateTime.UtcNow,
                            DueDate = DateTime.UtcNow.AddDays(dueDays), // ?? to be confirmed
                            Quantity = povm.Quantity,
                            UnitPrice = povm.VendorUnitPrice,
                            ToolingSetupCharges = povm.Quantity == 1 ? povm.VendorToolingCharge : 0m,
                            ProductId = product.Id,
                            IsOpen = true,
                            Term = povm.Term,
                            PONumber = poNumber.ToString(),
                        };
                        OmnaeInvoiceService.AddOmnaeInvoice(invoice2);
                    }

                    // Export this purchaseorder to PDF

                    povm.CustomerPONumber = invoice.PONumber;
                    povm.PONumber = invoice2.PONumber;
                    povm.DesireShippingDate = model.DesireShippingDate;
                    povm.EarliestShippingDate = model.EarliestShippingDate;

                    double salesTax = ShipmentBL.GetSalesTax(vendor.Address);
                    povm.Total = povm.Quantity * povm.VendorUnitPrice + povm.VendorToolingCharge;
                    povm.SalesTax = povm.Total * (decimal)salesTax;

                    // For web api, we need to find a way to do this
                    if (context != null)
                    {
                        var vendorPO = ExportVendorPurchaseOrderToPdf(povm, context);
                        invoice2.PODocUri = vendorPO.DocUri;
                        OmnaeInvoiceService.UpdateOmnaeInvoice(invoice2);
                    }
                }
            }
            catch (Exception ex)
            {
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
                Term = customer.Term.Value,
            };

            return ret;
        }

        public async Task<Tuple<string, string>> CallQBOCreateEstimate(PurchaseOrderViewModel model)
        {
            var UserContext = OrdersBL.UserContext;
            string containerName = CloudConfigurationManager.GetSetting("StorageContainer");

            QboApi qboApi = new QboApi(QboTokensService);

            byte[] data = await qboApi.CreateEstimate(model);

            if (data != null && model.CustomerId != null)
            {

                string fileName = $"estimate_companyid_{model.CustomerId.Value}_productid_{model.ProductId}.pdf";

                var docUri = BlobStorageService.BlobStorageUpload(data, containerName, fileName);
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

        public Document ExportVendorPurchaseOrderToPdf(PurchaseOrderViewModel model, ControllerContext context)
        {
            var UserContext = OrdersBL.UserContext;
            if (model == null)
            {
                return null;
            }

            var containerName = CloudConfigurationManager.GetSetting(OMNAE_WEB.StorageContainer);

            var fileName = $"vendor_po_vid_{model.VendorId}_tid_{model.TaskId}_pid_{model.ProductId}.pdf";

            var pdf = new ActionAsPdf("CreateVendorPurchaseOrder", model)
            {
                PageOrientation = Rotativa.Options.Orientation.Landscape,
                PageMargins = { Left = 5, Right = 5 },
                FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName,
            };

            var pdfBytes = pdf.BuildFile(context);
            var ms = new MemoryStream(pdfBytes);

            var docUri = BlobStorageService.BlobStorageUpload(ms, containerName, fileName);
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

            var d = DocumentService.FindDocumentByProductId(model.ProductId).FirstOrDefault(x => x.Name == fileName);
            if (d == null)
            {
                DocumentService.AddDocument(doc);
            }

            return doc;
        }

        
    }
}