using Common;
using Libs.Notification;
using Omnae.Common;
using Omnae.Libs.ViewModel;
using Omnae.Model.Context;
using Omnae.Model.Models;
using Omnae.Service.Service;
using Omnae.Service.Service.Interfaces;
using Omnae.Service.Service.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Common.Extensions;
using Humanizer;
using Omnae.BlobStorage;
using Omnae.Libs.Notification;
using Serilog;

namespace Omnae.BusinessLayer
{
    public class PaymentBL
    {
        private IOrderService orderService { get; }
        private IProductService productService { get; }
        private IDocumentService documentService { get; }
        private ILogedUserContext UserContext { get; }
        private UserContactService UserContactService { get; }
        private NotificationService NotificationService { get; }
        private IOmnaeInvoiceService omnaeInvoiceService { get; }
        private ILogger Log { get; }
        private readonly ICompaniesCreditRelationshipService companiesCreditRelationshipService;


        public PaymentBL(IOrderService orderService, IProductService productService, IDocumentService documentService, ILogedUserContext userContext, UserContactService userContactService, NotificationService notificationService, ILogger log, ICompaniesCreditRelationshipService companiesCreditRelationshipService, IOmnaeInvoiceService omnaeInvoiceService)
        {
            this.orderService = orderService;
            this.productService = productService;
            this.documentService = documentService;
            UserContext = userContext;
            UserContactService = userContactService;
            NotificationService = notificationService;
            Log = log;
            this.companiesCreditRelationshipService = companiesCreditRelationshipService;
            this.omnaeInvoiceService = omnaeInvoiceService;
        }

        public void NotifyOnPayment(Order order, States state, string subject)
        {
            var product = productService.FindProductById(order.ProductId);

            subject = string.Format(subject, product.PartNumber, product.Description);
            var task = order.TaskData;
            if (task == null)
                return;

            var docs = product.Documents?.UpdateDocUrlWithSecurityToken(documentService, ExpireTokenInfo.FourDays)?.ToNullIfEmpty()
                       ?? documentService.FindDocumentByProductId(order.ProductId) 
                       ?? documentService.FindDocumentByTaskId(task.TaskId);
            

            CompaniesCreditRelationship credit = null;
            decimal? quantity2 = null;
            decimal? price2 = null;
            var orders = orderService.FindOrderByTaskId(task.TaskId).OrderBy(x => x.Quantity);
            if (task.isEnterprise)
            {
                credit = companiesCreditRelationshipService.FindCompaniesCreditRelationshipByCustomerIdVendorId(product.CustomerId.Value, product.VendorId.Value);
                if (credit != null && credit.isTerm == true && order.IsReorder == false && order.IsForToolingOnly == false)
                {                 
                    quantity2 = orders.LastOrDefault().Quantity;
                    price2 = orders.LastOrDefault().SalesPrice;
                }
            }
            else if (orders.FirstOrDefault().IsReorder == false)
            {
                quantity2 = orders.FirstOrDefault().Quantity;
                price2 = orders.FirstOrDefault().SalesPrice;
            }
            

            IEnumerable<UserContactInformationModel> destinationContact = new List<UserContactInformationModel>();
            IEnumerable<UserContactInformationModel> destinationVendorContact = new List<UserContactInformationModel>();

            if (state == States.OrderPaid || state == States.ReOrderPaid)
            {
                var customersContacts = UserContactService.GetAllActiveUserConnectFromCompany(product.CustomerCompany);
                var vendorContacts = UserContactService.GetAllActiveUserConnectFromCompany(product.VendorCompany);
                destinationContact = customersContacts;
                destinationVendorContact = vendorContacts;
            }
            else
            {
                var userType = UserContext.UserType;
                var companyContacts = UserContactService.GetAllActiveUserConnectFromCompany((userType == USER_TYPE.Admin || userType == USER_TYPE.Customer) ? product.VendorCompany : product.CustomerCompany);
                destinationContact = companyContacts;
            }

            var currInfo = NMoneys.Currency.TryGet((credit?.Currency.Humanize() ?? CurrencyCodes.USD.Humanize()), out var curr) ? curr : NMoneys.Currency.Usd;

            var entity = new OrderCompleteEmailViewModel
            {
                CurrencySymbol = currInfo.Symbol,
                CurrencyTextSymbol = currInfo.IsoSymbol,

                PartNumber = product.PartNumber,
                Description = product.Description,
                Url = ConfigurationManager.AppSettings["URL"],
                Quantity = order.Quantity,
                Price = order.SalesPrice,
                ProductionLeadTime = product.ProductionLeadTime,
                Doc_2D = docs.Where(x => x.DocType == (int)DOCUMENT_TYPE.PRODUCT_2D_PDF).OrderBy(x => x.CreatedUtc).LastOrDefault(),
                Doc_3D = docs.Where(x => x.DocType == (int)DOCUMENT_TYPE.PRODUCT_3D_STEP).OrderBy(x => x.CreatedUtc).LastOrDefault(),
                Doc_Quote = docs.Where(x => x.DocType == (int)DOCUMENT_TYPE.QUOTE_PDF && x.TaskId == task.TaskId).OrderBy(x => x.CreatedUtc).LastOrDefault(),
                Doc_VENDOR_PO = docs.Where(x => x.DocType == (int)DOCUMENT_TYPE.QBO_PURCHASEORDER_PDF).OrderBy(x => x.CreatedUtc).Where(x => x.Name.Contains($"oid-{order.Id}")).FirstOrDefault(),
                PONumber = order.CustomerPONumber,
                IsSampleOrder = order.UnitPrice == null,
                Quantity2 = quantity2,
                Price2 = price2,
                IsRiskBuild = task.IsRiskBuild,
            };
            Exception exp = null;
            foreach (var contactInformation in destinationContact)
            {
                var destination = contactInformation.Email;
                var destinationSms = contactInformation.PhoneNumber;

                entity.UserName = destination;
                try
                {
                    NotificationService.NotifyStateChange(state, subject, destination, destinationSms, entity);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Error when Notification the Customer for a new Order/ReOrder. State:{state}, Subject:{subject}, DestinationEmail:{email}", state, subject, destination);

                    string msg = ex.RetrieveErrorMessage();
                    if (msg.Equals(IndicatingMessages.SmsWarningMsg) || msg.Equals(IndicatingMessages.NoCellPhoneWarningMsg))
                    {
                        exp = ex;
                        continue;
                    }
                    throw ex;
                }
            }
            if (state == States.OrderPaid || state == States.ReOrderPaid)
            {
                foreach (var userContactInformation in destinationVendorContact)
                {
                    var destinationVendor = userContactInformation.Email;
                    var destinationSmsVendor = userContactInformation.PhoneNumber;

                    // Notify Vendor
                    entity.UserName = destinationVendor;

                    try
                    {
                        if (task.isEnterprise == false)
                        {
                            var omnaeInvoice = omnaeInvoiceService.FindOmnaeInvoiceByUserTypeByOrderId(USER_TYPE.Vendor, order.Id);
                            entity.VendorPrice = omnaeInvoice.SalesTax + omnaeInvoice.UnitPrice * omnaeInvoice.Quantity;
                        }
                        
                        NotificationService.NotifyStateChange(state, subject, destinationVendor, destinationSmsVendor, entity, true);
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(ex, "Error when Notification the Vendor for a new Order/ReOrder. State:{state}, Subject:{subject}, DestinationEmail:{email}", state, subject, destinationVendor);
                        
                        string msg = ex.RetrieveErrorMessage();
                        if (msg.Equals(IndicatingMessages.SmsWarningMsg) || msg.Equals(IndicatingMessages.NoCellPhoneWarningMsg))
                        {
                            exp = ex;
                            continue;
                        }
                        throw ex;
                    }

                }
            }
            if (exp != null)
            {
                throw exp;
            }
        }
    }
}