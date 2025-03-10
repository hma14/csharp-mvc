using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.Exception;
using Intuit.Ipp.OAuth2PlatformClient;
using Newtonsoft.Json.Linq;
using Omnae.Model.Models;
using Omnae.QuickBooks.ViewModels;
using Omnae.Service.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Common;
using Omnae.Data;
using Serilog;

namespace Omnae.QuickBooks.QBO
{
    public class QboApi
    {
        public readonly ILogger Log;
        public readonly IQboTokensService qboTokensService;

        private string accessToken;
        private string refreshToken;
        private string realmId;
        private string tokenEndpoint;
        private ServiceContext servicecontext;

        // OAuth2 client configuration
        private string redirectUrl;
        private string clientID;
        private string clientSecret;
        private DateTime accessTokenCreatedAt;

        public QboApi(IQboTokensService qboTokensService, ILogger log = null)
        {
            this.qboTokensService = qboTokensService;
            Log = log ?? Serilog.Log.Logger;

            var qboTokens = qboTokensService.FindQboTokensList().Where(x => x.AccessToken != null && x.RefreshToken != null).FirstOrDefault();
            if (qboTokens == null || qboTokens.AccessToken == null || qboTokens.RefreshToken == null)
            {
                DateTime current = DateTime.Now;
                QboTokens model = new QboTokens
                {
                    RealmId = ConfigurationManager.AppSettings["initialRealmId"],
                    AccessToken = ConfigurationManager.AppSettings["initialAccessToken"],
                    AccessTokenCreatedAt = current,
                    RefreshToken = ConfigurationManager.AppSettings["initialRefreshToken"],
                    RefeshTokenExpireAt = current.AddMonths(3),
                    TokenEndpoint = ConfigurationManager.AppSettings["initialTokenEndpoint"],
                };
                qboTokensService.AddQboTokens(model);
            }
            else
            {
                this.accessToken = qboTokens.AccessToken;
                this.refreshToken = qboTokens.RefreshToken;
                this.realmId = qboTokens.RealmId;
                this.accessTokenCreatedAt = qboTokens.AccessTokenCreatedAt;
                this.tokenEndpoint = ConfigurationManager.AppSettings["initialTokenEndpoint"];
            }

            clientID = ConfigurationManager.AppSettings["clientID"];
            clientSecret = ConfigurationManager.AppSettings["clientSecret"];
            redirectUrl = ConfigurationManager.AppSettings["redirectUrl"];
        }

        private async System.Threading.Tasks.Task Initialize()
        {
            var span = DateTime.UtcNow - accessTokenCreatedAt;
            if (span.TotalHours > 1 || accessToken == null) // expired after 1 hour since it was created
            {
                await PerformRefreshToken();
            }

            QBOServiceInitializer initializer = new QBOServiceInitializer(accessToken, refreshToken, realmId);

            servicecontext = initializer.InitializeQBOServiceContextUsingoAuth();
            servicecontext.IppConfiguration.BaseUrl.Qbo = ConfigurationManager.AppSettings["BaseUrl"];

#if false
            // Logging
            servicecontext.IppConfiguration.Logger.RequestLog.EnableRequestResponseLogging = true;
            servicecontext.IppConfiguration.Logger.RequestLog.ServiceRequestLoggingLocation = @"F:\Temp\IdsLogs";
            // Serialization
            servicecontext.IppConfiguration.Message.Request.SerializationFormat = SerializationFormat.Json;
            servicecontext.IppConfiguration.Message.Response.SerializationFormat = SerializationFormat.Json;

            // Data Compress
            servicecontext.IppConfiguration.Message.Request.CompressionFormat = CompressionFormat.GZip;
            servicecontext.IppConfiguration.Message.Response.CompressionFormat = CompressionFormat.GZip;
#endif 
        }

        public async Task<string> CreateCustomer(CustomerInfo customerInfo)
        {
            await Initialize();

            Customer customer = QboAccessor.CreateCustomer(servicecontext, customerInfo);

            //Adding the Customer
            Customer added = Helper.Add<Customer>(servicecontext, customer);

            return added.Id;
        }

        public async System.Threading.Tasks.Task<string> CreateVendor(CustomerInfo customerInfo)
        {
            await Initialize();

            Vendor vendor = QboAccessor.CreateVendor(servicecontext, customerInfo);

            //Adding the Vendor
            Vendor added = Helper.Add<Vendor>(servicecontext, vendor);

            return added.Id;
        }

        public async System.Threading.Tasks.Task<byte[]> CreateEstimate(PurchaseOrderViewModel model)
        {
            await Initialize();
            Estimate estimate = await QboAccessor.CreateEstimate(servicecontext, model, Log);

            //Adding the Estimate
            Estimate added = Helper.Add<Estimate>(servicecontext, estimate);
            if (added == null)
                throw new ApplicationException("Adding new Estimate on QBO is failed");

            model.EstimateId = added.Id;
            model.EstimateNumber = added.DocNumber;

            // Get PO.pdf
            DataService dataService = new DataService(servicecontext);

            // GetPdf: Only entitites of type SalesReceipt, Invoice and Estimate are supported for this operation.
            byte[] response = dataService.GetPdf<Estimate>(added);
            return response;
        }

        public async System.Threading.Tasks.Task<byte[]> CreateInvoice(InvoiceViewModel model)
        {
            await Initialize();

            Invoice invoice = await QboAccessor.CreateInvoice(servicecontext, model, Log);

            Invoice added = Helper.Add<Invoice>(servicecontext, invoice);
            if (added == null)
                throw new ApplicationException("Adding new Invoice on QBO is failed");

            model.InvoiceId = added.Id;
            model.InvoiceNumber = added.DocNumber;

            // Get Invoice.pdf
            DataService dataService = new DataService(servicecontext);
            byte[] response = dataService.GetPdf<Invoice>(added);

            return response;
        }

        public async System.Threading.Tasks.Task<string> CreateQboInvoice(StripeInvoiceViewModel model)
        {
            await Initialize();

            Invoice invoice = QboAccessor.CreateStripeInvoice(servicecontext, model, Log);
            Invoice added = Helper.AddAsync<Invoice>(servicecontext, invoice);
            if (added == null)
                throw new ApplicationException("Adding new Stripe Invoice on QBO is failed");

            return added.Id;
        }

        public async System.Threading.Tasks.Task CreateQboPayment(PaymentViewModel model)
        {
            await Initialize();

            Payment payment = QboAccessor.CreatePayment(servicecontext, model);
            var added = Helper.Add<Payment>(servicecontext, payment);
            if (added == null)
                throw new Exception("Adding new Payment on QBO is failed");
        }

        public async System.Threading.Tasks.Task CreateBill(BillViewModel model)
        {
            await Initialize();

            Bill bill = await QboAccessor.CreateBill(servicecontext, model, Log);

            if (bill == null)
                throw new Exception("Creating Bill on QBO failed");

            Bill added = Helper.Add<Bill>(servicecontext, bill);
            if (added == null)
                throw new Exception("Adding new Bill on QBO failed");

            model.BillId = added.Id;
            model.BillNumber = added.DocNumber;

            // create attachment for this bill and link the attachable to this bill
            model.AttachedReferenceId = added.Id;
            Attachable attachable = QboAccessor.CreateAttachable(servicecontext, model);

            if (model.FStream != null)
            {
                //Upload Attachment
                attachable = Helper.Upload(servicecontext, attachable, model.FStream);
                Attachable addedAttachable = Helper.Add<Attachable>(servicecontext, attachable);
            }
        }

        public async System.Threading.Tasks.Task<Report> GetReport(string reportName)
        {
            await Initialize();

            Report report = QboAccessor.GetReport(servicecontext, reportName);
            return report;
        }

        public async System.Threading.Tasks.Task<Invoice> GetInvoice(string invoiceId, int retries = 3)
        {
            if (retries <= 0)
                throw new ApplicationException($"Error calling {nameof(GetInvoice)}. Retired too many times.");

            try
            {
                await Initialize();

                Invoice invoice = QboAccessor.GetInvoice(servicecontext, invoiceId);
                return invoice;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Equals("429"))
                {
                    Log.Warning(ex, $"Error calling {nameof(GetInvoice)}");
                    System.Threading.Thread.Sleep(10000); //Todo Find a better way to deal with this.
                    return await GetInvoice(invoiceId, --retries);
                }

                throw;
            }
        }
        public async System.Threading.Tasks.Task<Bill> GetBill(string billId, int retries = 3)
        {
            if (retries <= 0)
                throw new ApplicationException($"Error calling {nameof(GetBill)}. Retired too many times.");

            try
            {
                await Initialize();

                Bill bill = QboAccessor.GetBill(servicecontext, billId);
                return bill;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Equals("429"))
                {
                    Log.Warning(ex, $"Error calling {nameof(GetInvoice)}");
                    System.Threading.Thread.Sleep(10000); //Todo Find a better way to deal with this.
                    return await GetBill(billId, --retries);
                }

                throw;
            }
        }
        public async System.Threading.Tasks.Task<Payment> GetPayment(string paymentId)
        {
            await Initialize();

            Payment payment = QboAccessor.GetPayment(servicecontext, paymentId);
            return payment;
        }

        public async System.Threading.Tasks.Task<PaymentMethod> GetPaymentMethod(string paymentMethodId)
        {
            await Initialize();

            PaymentMethod paymentMethod = QboAccessor.GetPaymentMethod(servicecontext, paymentMethodId);
            return paymentMethod;
        }

        public async Task<BillPayment> GetBillPayment(string billPaymentId)
        {
            await Initialize();
            
            BillPayment billPayment = QboAccessor.GetBillPayment(servicecontext, billPaymentId);
            return billPayment;
        }

        public async Task<Decimal?> GetExchangeRate(DateTime date)
        {
            await Initialize();

            ExchangeRate exchangeRate = await QboAccessor.GetExchangeRate(servicecontext, date, "CAD", Log);
           
            return exchangeRate?.Rate;
        }

        public async Task<List<string>> GetQboCustomerEmails(string custId)
        {
            await Initialize();

            List<string> list = QboAccessor.FindQboCustomerEmails(servicecontext, custId);
            return list;
        }
        public async Task<Term> GetQboTerm(int? term)
        {
            await Initialize();

            return QboAccessor.FindQboTerm(servicecontext, term);
        }

        public async Task<decimal> GetTaxRateValue(string country, string countrySubDivisionCode)
        {
            await Initialize();

            TaxRate taxRate = QboAccessor.GetTaxRate(servicecontext, country, countrySubDivisionCode);
            return taxRate?.RateValue ?? 0m;
        }

        private async System.Threading.Tasks.Task PerformRefreshToken()
        {
            try
            {
                //Request Oauth2 tokens
                var tokenClient = new TokenClient(tokenEndpoint, clientID, clientSecret);

                //Handle exception 401 and then make this call
                // Call RefreshToken endpoint to get new access token when you recieve a 401 Status code
                if (refreshToken == null)
                {
                    refreshToken = ConfigurationManager.AppSettings["RefreshToken"];
                }

                var newTokenDate = DateTime.Now;
                var refereshtokenCallResponse = await tokenClient.RequestRefreshTokenAsync(refreshToken);

                if (refereshtokenCallResponse == null)
                {
                    Log.Error("Error PerformRefreshToken in QBO. Null Response");
                    throw new ApplicationException("Error PerformRefreshToken in QBO. Null Response");
                }
                if (refereshtokenCallResponse.IsError)
                {
                    Log.Error(refereshtokenCallResponse.Exception, "Error PerformRefreshToken in QBO. {ErrorType} - {ErrorMessage} - {ErrorDescription}", refereshtokenCallResponse.ErrorType, refereshtokenCallResponse.Error, refereshtokenCallResponse.ErrorDescription);
                    var jobject = refereshtokenCallResponse.Json;
                    throw new ApplicationException($"Perform Refresh QBO Access Token failed - error:  {jobject["error"]}, error description: {jobject["error_description"]}");
                }

                if (refereshtokenCallResponse.AccessToken == null)
                {
                    var jsonObject = JObject.Parse(refereshtokenCallResponse.Raw);
                    string errorDescription = jsonObject["error_description"]?.ToString() ?? "null error_description - don't have the error detail";

                    var jobject = refereshtokenCallResponse.Json;
                    throw new ApplicationException($"Perform Refresh QBO Access Token failed - error:  {jobject["error"]}, error description: {jobject["error_description"]}");
                }
                if (refereshtokenCallResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    //Update datase Tokens
                    var qboTokens = qboTokensService.FindQboTokens();
                    if (qboTokens == null)
                    {
                        qboTokens = new QboTokens
                        {
                            AccessToken = refereshtokenCallResponse.AccessToken,
                            RefreshToken = refereshtokenCallResponse.RefreshToken,
                            AccessTokenCreatedAt = newTokenDate,
                            RefeshTokenExpireAt = newTokenDate.AddSeconds(refereshtokenCallResponse.RefreshTokenExpiresIn),
                        };
                        qboTokensService.AddQboTokens(qboTokens);
                    }
                    else
                    {
                        qboTokens.AccessToken = refereshtokenCallResponse.AccessToken;
                        qboTokens.RefreshToken = refereshtokenCallResponse.RefreshToken;
                        qboTokens.AccessTokenCreatedAt = newTokenDate;
                        qboTokens.RefeshTokenExpireAt = newTokenDate.AddSeconds(refereshtokenCallResponse.RefreshTokenExpiresIn);

                        qboTokensService.UpdateQboTokens(qboTokens);
                    }
                    //Update Local Memory TOkens
                    this.accessToken = qboTokens.AccessToken;
                    this.refreshToken = qboTokens.RefreshToken;
                }
                else
                {
                    throw new ApplicationException($"Undocumented Error QBO Access Token PerformRefreshToken: Invalid HttpStatusCode Response.");
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Refresh QBO Access Token failed");
                throw;
            }
        }
    }
}
