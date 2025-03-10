using Common;
using Omnae.BusinessLayer;
using Omnae.BusinessLayer.Interface;
using Omnae.BusinessLayer.Models;
using Omnae.Common;
using Omnae.Model.Models;
using Omnae.QuickBooks.QBO;
using Omnae.QuickBooks.ViewModels;
using Omnae.Service.Service.Interfaces;
using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Omnae.Controllers
{
    public class StripeWebhookController : Controller
    {
        private readonly IQboTokensService qboTokensService;
        private readonly ICompanyService companyService;
        private readonly IHomeBL homeBL;
        private readonly IStripeQboService stripeQboService;


        public StripeWebhookController(IQboTokensService qboTokensService, ICompanyService companyService, IHomeBL homeBL,
                                       IStripeQboService stripeQboService)
        {
            this.qboTokensService = qboTokensService;
            this.companyService = companyService;
            this.homeBL = homeBL;
            this.stripeQboService = stripeQboService;
        }

        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {

            string secret = ConfigurationManager.AppSettings["webhook_secret"];
            StripeConfiguration.ApiKey = ConfigurationManager.AppSettings["stripe_secrit_key"];

            try
            {
                var json = new StreamReader(HttpContext.Request.InputStream).ReadToEnd();
                if (!string.IsNullOrEmpty(json))
                {
                    var stripeEvent = EventUtility.ConstructEvent(json,
                        Request.Headers["Stripe-Signature"], secret);

                    Invoice invoice = new Invoice();
                    Subscription subscription = new Subscription();
                    PaymentIntent paymentIntent = new PaymentIntent();
                    InvoiceService invoiceService = new InvoiceService();

                    switch (stripeEvent.Type)
                    {

                        case "customer.subscription.created":
                        case "customer.subscription.updated":
                        case "customer.subscription.deleted":
                        case "customer.subscription.trial_will_end":
                            subscription = stripeEvent.Data.Object as Subscription;
                            invoice = invoiceService.Get(subscription.LatestInvoiceId);
                            if (invoice.AmountPaid == 0)
                            {
                                await CreateQboInvoice(invoice);
                            }

                            break;

                        case "invoice.created":
                            //invoice = stripeEvent.Data.Object as Invoice;
                            //await CreateQboInvoice(invoice);
                            break;


                        //case "invoice.finalized":
                        //    invoice = stripeEvent.Data.Object as Invoice;

                        //    break;

                        //case "invoice.payment_succeeded":
                        //    paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                        //    await CreateQboPayment(paymentIntent);

                        //    break;


                        case "payment_intent.succeeded":
                            paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                            await CreateQboInvoicePayment(paymentIntent);

                            break;



                        case "coupon.created":
                        case "coupon.updated":
                        case "coupon.deleted":
                            var coupon = stripeEvent.Data.Object as Coupon;
                            // do work

                            break;


                    }
                }
                //return new EmptyResult();
                return View();

            }
            catch (StripeException e)
            {
                // Invalid Signature

                ViewBag.Message = e.RetrieveErrorMessage();
                return View();
            }

        }


        private async Task<StripeQbo> CreateQboInvoice(Invoice invoice)
        {
            if (invoice.CustomerId == null)
            {
                ViewBag.Message = IndicatingMessages.CompanyNotFound;
                return null;
            }


            var qboApi = new QboApi(qboTokensService);
            string qboId = string.Empty;
            var company = companyService.FindCompanyByStripeCustomerId(invoice.CustomerId);
            if (company == null)
            {
                ViewBag.Message = IndicatingMessages.CompanyNotFound;
                return null;
            }

            if (company.QboId == null)
            {
                // Add this company to QBO
                CreateCustomerInfoViewModel info = new CreateCustomerInfoViewModel
                {
                    CompanyId = company.Id,
                    FirstName = invoice.CustomerName ?? company.Name,
                    //LastName = ?? // ToDo
                    PhoneNumber = invoice.CustomerPhone,
                    Email = invoice.CustomerEmail,

                };
                CustomerInfo customerInfo = homeBL.AddCustomerQBO(info);
                if (customerInfo == null)
                {
                    ViewBag.Message = "This company is not existing in Omnae's [AspNetUsers] table";
                    return null;
                }

                qboId = await qboApi.CreateCustomer(customerInfo);

                // update Company table by insert this qbo id
                company.QboId = qboId;
                companyService.UpdateCompany(company);
            }

            StripeInvoiceViewModel model = new StripeInvoiceViewModel
            {
                InvoiceId = invoice.Id,
                StripeCustomerId = invoice.CustomerId,
                QboId = company.QboId,
                CompanyName = company.Name,
                IssueDate = invoice.WebhooksDeliveredAt, 
                DueDate = invoice.DueDate,
                InvoiceNumber = invoice.Number,  
                PhoneNumber = invoice.CustomerPhone,
                EmailAddress = invoice.CustomerEmail,
                AddressLine1 = invoice.Customer?.Address?.Line1,
                AddressLine2 = invoice.Customer?.Address?.Line2,
                City = invoice.Customer?.Address?.City,
                State = invoice.Customer?.Address?.State,
                CountryName = invoice.AccountCountry,
                ZipCode = company.Address.ZipCode,
                PostalCode = company.Address.PostalCode,
                Quantity = 1,
                Total = invoice.Total / 100,
                Buyer = invoice.CustomerName,
                HostedInvoiceUrl = invoice.HostedInvoiceUrl,
                InvoicePdf = invoice.InvoicePdf,
                InvoiceStatus = invoice.Status,
            };


            string qboInvoiceId = await qboApi.CreateQboInvoice(model);
            StripeQbo entity = new StripeQbo
            {
                StripeInvoiceId = invoice.Id,
                QboInvoiceId = qboInvoiceId,
                QboId = company.QboId,
                QboInvoiceNumber = invoice.Number,
            };

            //stripeQboService.AddStripeQbo(entity);
            return entity;
        }

        private async Task CreateQboInvoicePayment(PaymentIntent paymentIntent)
        {
            InvoiceService invoiceService = new InvoiceService();
            var invoice = invoiceService.Get(paymentIntent.InvoiceId);
            

            var qboApi = new QboApi(qboTokensService);
            var company = companyService.FindCompanyByStripeCustomerId(paymentIntent.CustomerId);
            if (company == null)
            {
                ViewBag.Message = IndicatingMessages.CompanyNotFound;
                return;
            }
            if (paymentIntent.Status.ToLower() != "succeeded")
            {
                ViewBag.Message = "Payment failed on Stripe";
                return;
            }

            //StripeQbo sq = stripeQboService.FindStripeQboByStripeInvoiceId(paymentIntent.InvoiceId);
            StripeQbo sq = await CreateQboInvoice(invoice);

            if (sq != null)
            {
                PaymentViewModel model = new PaymentViewModel
                {
                    PaymentRefNum = sq.QboInvoiceNumber,
                    QboInvoiceId = sq.QboInvoiceId,
                    TotalAmt = paymentIntent.AmountReceived / 100,
                    QboId = company.QboId,
                    CustomerName = company.Name,
                };
                await qboApi.CreateQboPayment(model);
            }
        }


    }
}