using AutoMapper;
using Microsoft.AspNet.Identity;
using Omnae.BlobStorage;
using Omnae.Common;
using Omnae.Model.Models;
using Omnae.Service.Service.Interfaces;
using Omnae.ViewModels;
using Stripe;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Mvc;
using Libs.Notification;
using Omnae.BusinessLayer;
using Omnae.BusinessLayer.Identity.Model;
using Omnae.BusinessLayer.Models;
using Omnae.BusinessLayer.Services;
using Omnae.Context;
using Omnae.Service.Service;
using Common;
using Omnae.Libs.Notification;
using Omnae.Model.Context;

namespace Omnae.Controllers
{
    public class StripeChargeController : BaseController
    {
        private OrdersBL OrdersBL { get; }

        public StripeChargeController(IRFQBidService rfqBidService, ICompanyService companyService, ITaskDataService taskDataService, IPriceBreakService priceBreakService, IOrderService orderService, ILogedUserContext userContext, IProductService productService, IDocumentService documentService, IShippingService shippingService, ICountryService countryService, IAddressService addressService, IStateProvinceService stateProvinceService, IOrderStateTrackingService orderStateTrackingService, IProductStateTrackingService productStateTrackingService, IPartRevisionService partRevisionService, IStoredProcedureService spService, INCReportService ncReportService, IRFQQuantityService rfqQuantityService, IExtraQuantityService extraQuantityService, IBidRequestRevisionService bidRequestRevisionService, ITimerSetupService timerSetupService, IQboTokensService qboTokensService, IOmnaeInvoiceService omnaeInvoiceService, INCRImagesService ncrImagesService, IApprovedCapabilityService approvedCapabilityService, IShippingAccountService shippingAccountService, ApplicationDbContext dbUser, ProductBL productBl, NotificationService notificationService, UserContactService userContactService, TimerTriggerService timerTriggerService, NotificationBL notificationBl, PaymentBL paymentBl, ShipmentBL shipmentBl, ChartBL chartBl, IMapper mapper, NcrBL ncrBL, IDocumentStorageService documentStorageService, IImageStorageService imageStorageService, OrdersBL ordersBl) : base(rfqBidService, companyService, taskDataService, priceBreakService, orderService, userContext, productService, documentService, shippingService, countryService, addressService, stateProvinceService, orderStateTrackingService, productStateTrackingService, partRevisionService, spService, ncReportService, rfqQuantityService, extraQuantityService, bidRequestRevisionService, timerSetupService, qboTokensService, omnaeInvoiceService, ncrImagesService, approvedCapabilityService, shippingAccountService, dbUser, productBl, notificationService, userContactService, timerTriggerService, notificationBl, paymentBl, shipmentBl, chartBl, mapper, ncrBL, documentStorageService, imageStorageService)
        {
            OrdersBL = ordersBl;
        }

        // GET: StripeCharge
        public ActionResult Index(int? Id, bool? isReorder)
        {
            if (Id == null)
            {
                HandleErrorInfo info = new HandleErrorInfo(new Exception(IndicatingMessages.ParameterIsNull), "StripeCharge", "Index");
                return View("Error", info);
            }

            var order = OrderService.FindOrderById(Id.Value);
            var model = new StripeChargeViewModel
            {
                OrderId = Id.Value,
                IsReorder = isReorder,
            };

            // Create dropdown lists
            int usaCode = (int)STATE_PROVINCE_CODE.US_STATE_CODE;
            int canadaCode = (int)STATE_PROVINCE_CODE.CA_PROVINCE_CODE;

            var currentUserId = User.Identity.GetUserId();
            var company = CompanyService.FindCompanyByUserId(currentUserId);

            if (company != null && company.Address.isBilling)
            {
                model.address = mapper.Map<BillingAddressViewModel>(company.Address);
                ViewBag.Country = new SelectList(CountryService.FindAllCountries(), "Id", "CountryName", model.address.CountryId);
                ViewBag.USA = new SelectList(StateProvinceService.FindStateProvinceByCode(usaCode), "Id", "Name", model.address.StateProvinceId);
                ViewBag.Canada = new SelectList(StateProvinceService.FindStateProvinceByCode(canadaCode), "Id", "Name", model.address.StateProvinceId);
            }
            else if (company != null && company.BillAddress != null)
            {
                model.address = mapper.Map<BillingAddressViewModel>(company.BillAddress);
                ViewBag.Country = new SelectList(CountryService.FindAllCountries(), "Id", "CountryName", model.address.CountryId);
                ViewBag.USA = new SelectList(StateProvinceService.FindStateProvinceByCode(usaCode), "Id", "Name", model.address.StateProvinceId);
                ViewBag.Canada = new SelectList(StateProvinceService.FindStateProvinceByCode(canadaCode), "Id", "Name", model.address.StateProvinceId);
            }
            else
            {
                ViewBag.Country = new SelectList(CountryService.FindAllCountries(), "Id", "CountryName", (int)COUNTRY_ID.CA);
                ViewBag.USA = new SelectList(StateProvinceService.FindStateProvinceByCode(usaCode), "Id", "Name");
                ViewBag.Canada = new SelectList(StateProvinceService.FindStateProvinceByCode(canadaCode), "Id", "Name", "59");
            }


            model.TaskId = order.TaskId;
            model.Amount = order.SalesPrice.Value;

            // ToDo: DropdownList in UI ?
            model.Currency = Currency.CAD;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Charge(StripeChargeViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index");

            try
            {
                var chargeId = await ProcessPayment(model);

                // You should do something with the chargeId --> Persist it maybe?
                if (chargeId != null)
                {
                    var order = OrderService.FindOrderById(model.OrderId);
                    string error = OrdersBL.PostPlaceOrderAction(order, order.CustomerPONumber, model.IsReorder ?? false);
                    if (error != null)
                    {
                        TempData["StripeException"] = error;
                        return RedirectToAction("Charge", "Orders", new { @id = model.OrderId, model.IsReorder });
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (StripeException ex)
            {
                TempData["StripeException"] = ex.Message;
            }

            return RedirectToAction("Charge", "Orders", new { @id = model.OrderId, @isReorder = model.IsReorder });
        }

        private static async Task<string> ProcessPayment(StripeChargeViewModel model)
        {
            return await Task.Run(() =>
            {
                var myCharge = new ChargeCreateOptions
                {
                    // convert the amount of £12.50 to pennies i.e. 1250
                    Amount = (int)(model.Amount * 100),
                    Currency = model.Currency,
                    Description = "Description for test charge",
                    Source = model.Token,
                    StatementDescriptor = "Custom descriptor",
                };

                StripeConfiguration.ApiKey = ConfigurationManager.AppSettings["stripe_secrit_key"];
                var chargeService = new ChargeService();
                var stripeCharge = chargeService.Create(myCharge);

                return stripeCharge.Id;
            });
        }
    }
}
