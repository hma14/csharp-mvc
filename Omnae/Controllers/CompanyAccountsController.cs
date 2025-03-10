using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Libs.Exceptions;
using Libs.Notification;
using Libs.ViewModels;
using Microsoft.AspNet.Identity;
using Omnae.BlobStorage;
using Omnae.BusinessLayer;
using Omnae.BusinessLayer.Identity;
using Omnae.BusinessLayer.Identity.Model;
using Omnae.BusinessLayer.Services;
using Omnae.Common;
using Omnae.Context;
using Omnae.Data;
using Omnae.Libs.Notification;
using Omnae.Model.Context;
using Omnae.Model.Models.Aspnet;
using Omnae.Model.Security;
using Omnae.Service.Service;
using Omnae.Service.Service.Interfaces;

namespace Omnae.Controllers
{
    [Authorize(Roles = Roles.CompanyAdmin)]
    public class CompanyAccountsController : BaseController
    {
        private ApplicationUserManager UserManager { get; }
        private CompanyAccountsBL CompanyBl { get; }

        public CompanyAccountsController(IRFQBidService rfqBidService, ICompanyService companyService, ITaskDataService taskDataService, IPriceBreakService priceBreakService, IOrderService orderService, ILogedUserContext userContext, IProductService productService, IDocumentService documentService, IShippingService shippingService, ICountryService countryService, IAddressService addressService, IStateProvinceService stateProvinceService, IOrderStateTrackingService orderStateTrackingService, IProductStateTrackingService productStateTrackingService, IPartRevisionService partRevisionService, IStoredProcedureService spService, INCReportService ncReportService, IRFQQuantityService rfqQuantityService, IExtraQuantityService extraQuantityService, IBidRequestRevisionService bidRequestRevisionService, ITimerSetupService timerSetupService, IQboTokensService qboTokensService, IOmnaeInvoiceService omnaeInvoiceService, INCRImagesService ncrImagesService, IApprovedCapabilityService approvedCapabilityService, IShippingAccountService shippingAccountService, ApplicationDbContext dbUser, ProductBL productBl, NotificationService notificationService, UserContactService userContactService, TimerTriggerService timerTriggerService, NotificationBL notificationBl, PaymentBL paymentBl, ShipmentBL shipmentBl, ChartBL chartBl, IMapper mapper, NcrBL ncrBL, IDocumentStorageService documentStorageService, IImageStorageService imageStorageService, ApplicationUserManager userManager, CompanyAccountsBL companyBl) : base(rfqBidService, companyService, taskDataService, priceBreakService, orderService, userContext, productService, documentService, shippingService, countryService, addressService, stateProvinceService, orderStateTrackingService, productStateTrackingService, partRevisionService, spService, ncReportService, rfqQuantityService, extraQuantityService, bidRequestRevisionService, timerSetupService, qboTokensService, omnaeInvoiceService, ncrImagesService, approvedCapabilityService, shippingAccountService, dbUser, productBl, notificationService, userContactService, timerTriggerService, notificationBl, paymentBl, shipmentBl, chartBl, mapper, ncrBL, documentStorageService, imageStorageService)
        {
            UserManager = userManager;
            CompanyBl = companyBl;
        }

        // GET: CompanyAccounts
        public async Task<ActionResult> Index()
        {
            var currentCompanyId = UserContext.Company?.Id;
            if (currentCompanyId == null)
                return RedirectToAction("Index", "Home");

            var users = await CompanyBl.ListAllUsersAsync(currentCompanyId, ignoreThisUserID: UserContext.UserId);
            return View(users);
        }

        // GET: CompanyAccounts/Details/5
        public async Task<ActionResult> Details([Required] string id)
        {
            var userId = id;

            var currentCompanyId = UserContext.Company?.Id;
            if (currentCompanyId == null)
                return RedirectToAction("Index", "Home");
            if (userId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            var simplifiedUser = await CompanyBl.FindByIdAsync(userId, currentCompanyId);
            if (simplifiedUser == null)
                return HttpNotFound();

            return View(simplifiedUser);
        }

        // GET: CompanyAccounts/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Email,FirstName,MiddleName,LastName,PhoneNumber,UserType")] SimplifiedUser simplifiedUser)
        {
            var currentCompanyId = UserContext.Company?.Id;
            if (currentCompanyId == null)
                return RedirectToAction("Index", "Home");

            if (simplifiedUser.UserType == USER_TYPE.Admin || simplifiedUser.UserType == USER_TYPE.Unknown)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            simplifiedUser.Id = Guid.NewGuid().ToString();
            simplifiedUser.CompanyId = currentCompanyId;
            simplifiedUser.Active = true;
            
            if (!ModelState.IsValid)
                return View();

            try
            {
                using (var tran = AsyncTransactionScope.StartNew())
                {
                    await CompanyBl.CreateAsync(simplifiedUser, currentCompanyId);

                    //var t1 = ResetPasswordInternal(simplifiedUser.Id);
                    //var t2 = SendEmailConfirmationTokenAsync(simplifiedUser.Id, simplifiedUser.Email, "Omnae.com Account Confirmation");
                    //await Task.WhenAll(t1, t2);
                    await ResetPasswordAndConfirmEmailInternal(simplifiedUser.Id);

                    tran.Complete();
                }
            }
            catch (BusinessRuleException e)
            {
                ViewBag.Error = e.Message;
                return View();
            }

            return RedirectToAction("Index");
        }

        
        private async Task<string> SendEmailConfirmationTokenAsync(string userID, string userName, string subject)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url?.Action("ConfirmEmail", "Account", new { userId = userID, code = code }, protocol: Request.Url.Scheme);

            var model = new AccountConfirmEmailViewModel()
            {
                UserName = userName,
                CallbackUrl = callbackUrl ?? ""
            };

            NotificationService.NotifyConfirmEmail(subject, userName, UserManager.GetPhoneNumber(userID), model);

            return code;
        }

        // GET: CompanyAccounts/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            var userId = id;

            var currentCompanyId = UserContext.Company?.Id;
            if (currentCompanyId == null)
                return RedirectToAction("Index", "Home");
            if (userId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var simplifiedUser = await CompanyBl.FindByIdAsync(userId, currentCompanyId);
            if (simplifiedUser == null)
                return HttpNotFound();

            return View(simplifiedUser);
        }

        // POST: CompanyAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Email,FirstName,MiddleName,LastName,PhoneNumber,UserType")] SimplifiedUser simplifiedUser)
        {
            var currentCompanyId = UserContext.Company?.Id;
            if (currentCompanyId == null)
                return RedirectToAction("Index", "Home");

            if (simplifiedUser.UserType == USER_TYPE.Admin || simplifiedUser.UserType == USER_TYPE.Unknown)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (!ModelState.IsValid)
                return View(simplifiedUser);

            if (await CompanyBl.UserIsFromCompanyAsync(simplifiedUser.Id, currentCompanyId) == false)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            try
            {
                using (var tran = AsyncTransactionScope.StartNew())
                {
                    await CompanyBl.EditAsync(simplifiedUser);

                    tran.Complete();
                }
            }
            catch (BusinessRuleException e)
            {
                ViewBag.Error = e.Message;
                return View();
            }

            return RedirectToAction("Index");
        }

        // GET: CompanyAccounts/Deactivate/5
        public async Task<ActionResult> Deactivate([Required] string id)
        {
            var userId = id;

            var currentCompanyId = UserContext.Company?.Id;
            if (currentCompanyId == null)
                return RedirectToAction("Index", "Home");
            if (userId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var simplifiedUser = await CompanyBl.FindByIdAsync(id, currentCompanyId);
            if (simplifiedUser == null)
                return HttpNotFound();

            if(simplifiedUser.Active == false)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            return View(simplifiedUser);
        }

        // POST: CompanyAccounts/Deactivate/5
        [HttpPost, ActionName("Deactivate")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeactivateConfirmed([Required] string id)
        {
            var userId = id;

            var currentCompanyId = UserContext.Company?.Id;
            if (currentCompanyId == null)
                return RedirectToAction("Index", "Home");
            if (userId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (await CompanyBl.UserIsFromCompanyAsync(userId, currentCompanyId) == false)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            await CompanyBl.DeactivateAsync(id);

            return RedirectToAction("Index");
        }

        
        // GET: CompanyAccounts/Deactivate/5
        public async Task<ActionResult> Activate([Required] string id)
        {
            var userId = id;

            var currentCompanyId = UserContext.Company?.Id;
            if (currentCompanyId == null)
                return RedirectToAction("Index", "Home");
            if (userId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var simplifiedUser = await CompanyBl.FindByIdAsync(id, currentCompanyId);
            if (simplifiedUser == null)
                return HttpNotFound();

            if(simplifiedUser.Active == true)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            return View(simplifiedUser);
        }

        // POST: CompanyAccounts/Deactivate/5
        [HttpPost, ActionName("Activate")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ActivateConfirmed([Required] string id)
        {
            var userId = id;

            var currentCompanyId = UserContext.Company?.Id;
            if (currentCompanyId == null)
                return RedirectToAction("Index", "Home");
            if (userId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (await CompanyBl.UserIsFromCompanyAsync(userId, currentCompanyId) == false)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            await CompanyBl.ActivateAsync(id);

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> ResetPassword(string id)
        {
            var userId = id;

            var currentCompanyId = UserContext.Company?.Id;
            if (currentCompanyId == null)
                return RedirectToAction("Index", "Home");
            if (userId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (await CompanyBl.UserIsFromCompanyAsync(userId, currentCompanyId) == false)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            await ResetPasswordInternal(userId);

            ViewBag.Message = "The user wil receive a email with a password reset link.";
            return RedirectToAction("Index");
        }

        private async Task ResetPasswordInternal(string userId)
        {
            string code = await UserManager.GeneratePasswordResetTokenAsync(userId);

            var callbackUrl = Url.Action("ResetPassword", "Account", new {userId = userId, code = code}, protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(userId, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
        }

        private async Task ResetPasswordAndConfirmEmailInternal(string userId)
        {
            string emailConfirmationCode = await UserManager.GenerateEmailConfirmationTokenAsync(userId);

            var callbackUrl = Url.Action("CreatePassword", "Account", new { userId = userId, code = emailConfirmationCode }, protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(userId, "Create a Password", "Thank you for creating an account on Omnae.com Please click <a href=\"" + callbackUrl + "\">here</a> to confirm your email address and create a new password.");
        }

        public async Task<ActionResult> ResendConfirmation(string id)
        {
            var userId = id;

            var currentCompanyId = UserContext.Company?.Id;
            if (currentCompanyId == null)
                return RedirectToAction("Index", "Home");
            if (userId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (await CompanyBl.UserIsFromCompanyAsync(userId, currentCompanyId) == false)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var dbUser = await CompanyBl.FindByIdAsync(userId, currentCompanyId);
            if (dbUser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            await SendEmailConfirmationTokenAsync(dbUser.Id, dbUser.Email, "Omnae.com Account Confirmation");

            return RedirectToAction("Index");
        }
    }
}