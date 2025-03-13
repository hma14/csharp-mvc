using AutoMapper;
using Libs.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Omnae.BlobStorage;
using Omnae.Common;
using Omnae.Model.Security;
using Omnae.Models;
using Omnae.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Libs.Notification;
using Omnae.BusinessLayer;
using Omnae.BusinessLayer.Identity;
using Omnae.BusinessLayer.Identity.Model;
using Omnae.BusinessLayer.Services;
using Omnae.Context;
using Omnae.Filters;
using Omnae.Service.Service;
using Omnae.Service.Service.Interfaces;
using Omnae.Data;
using Omnae.Libs.Notification;
using Omnae.Model.Context;

namespace Omnae.Controllers
{
    [Authorize]
    [CustomHandleError]
    public class AccountController : BaseController
    {
        private ApplicationSignInManager SignInManager { get; }
        private ApplicationUserManager UserManager { get; }
        private IAuthenticationManager AuthenticationManager { get; }

        public AccountController(IRFQBidService rfqBidService, ICompanyService companyService, ITaskDataService taskDataService, IPriceBreakService priceBreakService, IOrderService orderService, ILogedUserContext userContext, IProductService productService, IDocumentService documentService, IShippingService shippingService, ICountryService countryService, IAddressService addressService, IStateProvinceService stateProvinceService, IOrderStateTrackingService orderStateTrackingService, IProductStateTrackingService productStateTrackingService, IPartRevisionService partRevisionService, IStoredProcedureService spService, INCReportService ncReportService, IRFQQuantityService rfqQuantityService, IExtraQuantityService extraQuantityService, IBidRequestRevisionService bidRequestRevisionService, ITimerSetupService timerSetupService, IQboTokensService qboTokensService, IOmnaeInvoiceService omnaeInvoiceService, INCRImagesService ncrImagesService, IApprovedCapabilityService approvedCapabilityService, IShippingAccountService shippingAccountService, ApplicationDbContext dbUser, ProductBL productBl, NotificationService notificationService, UserContactService userContactService, TimerTriggerService timerTriggerService, NotificationBL notificationBl, PaymentBL paymentBl, ShipmentBL shipmentBl, ChartBL chartBl, IMapper mapper, NcrBL ncrBL, IDocumentStorageService documentStorageService, IImageStorageService imageStorageService, ApplicationSignInManager signInManager, ApplicationUserManager userManager, IAuthenticationManager authenticationManager) : base(rfqBidService, companyService, taskDataService, priceBreakService, orderService, userContext, productService, documentService, shippingService, countryService, addressService, stateProvinceService, orderStateTrackingService, productStateTrackingService, partRevisionService, spService, ncReportService, rfqQuantityService, extraQuantityService, bidRequestRevisionService, timerSetupService, qboTokensService, omnaeInvoiceService, ncrImagesService, approvedCapabilityService, shippingAccountService, dbUser, productBl, notificationService, userContactService, timerTriggerService, notificationBl, paymentBl, shipmentBl, chartBl, mapper, ncrBL, documentStorageService, imageStorageService)
        {
            SignInManager = signInManager;
            UserManager = userManager;
            AuthenticationManager = authenticationManager;
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Require the user to have a confirmed email before they can log on.
            // var user = await UserManager.FindByNameAsync(model.Email);

            var user = UserManager.Find(model.Email, model.Password);
            if (user?.Active != true)
            {
                ModelState.AddModelError("", "Invalid login attempt. Maybe the Password is invalid or the User is not found or disabled.");
                return View(model);
            }

            if (!await UserManager.IsEmailConfirmedAsync(user.Id))
            {
                string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, user.UserName, "Confirm your account-Resend");
                //ViewBag.Message = "Check your email and confirm your account, you must be confirmed before you can log in.";

                //ViewBag.UserId = user.Id;
                //return View("Info");
                ViewBag.errorMessage = "You must have a confirmed email to log on.";
                return View("Error");
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);
            Session.Abandon();

            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt. Maybe the Password is invalid or the User is not found or disabled.");
                    return View(model);
            }
        }

        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
                return View("Error");
            
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            Session.Abandon();

            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(CreateAccountForCustomerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (var trans = AsyncTransactionScope.StartNew())
            {
                var user = new ApplicationUser
                {
                    //UserName = model.Resgister.Email,
                    Email = model.Resgister.Email,
                    UserType = USER_TYPE.Customer,
                    PhoneNumber = model.Resgister.PhoneNumber,
                    FirstName = model.Resgister.FirstName,
                    MiddleName = model.Resgister.MiddleName,
                    LastName = model.Resgister.LastName,
                    Active = true,
                };

                var result = await UserManager.CreateAsync(user, model.Resgister.Password);
                if (!result.Succeeded)
                {
                    ViewBag.ErrorMessage = "Add user to database failed. ";
                    if (result.Errors != null && result.Errors.Any())
                    {
                        ViewBag.ErrorMessage += result.Errors.ToArray()[0];
                    }
                    return View("Register");
                }

                if (result.Succeeded)
                {
                    //Add default Role
                    await UserManager.AddToRoleAsync(user.Id, Roles.CompanyAdmin);

                    //  Comment the following line to prevent log in until the user is confirmed.
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    Session.Abandon();

                    string code = await SendEmailConfirmationTokenAsync(user.Id, model.Resgister.Email, "Omnae.com Account Confirmation");

                    ViewBag.Message = "Check your email and confirm your account, you must be confirmed before you can log in.";
                    ViewBag.UserId = user.Id;
                    ViewBag.TokenCode = code;

                    trans.Complete();
                    return View("Info");
                }

                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }

            var user = await UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }

            var currentLoggedUserId = this.User.Identity.GetUserId();
            if (!string.IsNullOrEmpty(currentLoggedUserId) && currentLoggedUserId != userId)
            {
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                Session.Abandon();
            }

            var result = await UserManager.ConfirmEmailAsync(userId, code);
            if (!result.Succeeded)
            {
                if (result.Errors?.ElementAtOrDefault(0) != null)
                {
                    TempData["ErrorMessage"] = result.Errors.ElementAtOrDefault(0).ToString();
                }
                return View("Error");
            }

            if (user.CompanyId != null)
            {
                return RedirectToAction("Index", "Home");
            }

            TempData["UserID"] = userId;
            TempData.Keep();
            return RedirectToAction("ContinueRegistration", "Home");
        }

        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await UserManager.FindByNameAsync(model.Email) 
                     ?? await UserManager.FindByEmailAsync(model.Email);

            if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return View("ForgotPasswordConfirmation");
            }

            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
            // Send an email with this link
            string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");

            return RedirectToAction("ForgotPasswordConfirmation", "Account");
            // If we got this far, something failed, redisplay form
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await UserManager.FindByNameAsync(model.Email)
                       ?? await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
                return RedirectToAction("ResetPasswordConfirmation", "Account"); // Don't reveal that the user does not exist

            var hasPwd = await UserManager.HasPasswordAsync(user.Id);

            if (hasPwd)
            {
                var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
                if (result.Succeeded)
                    return RedirectToAction("ResetPasswordConfirmation", "Account");

                AddErrors(result);
            }
            else
            {
                var result = await UserManager.AddPasswordAsync(user.Id, model.Password);
                if (result.Succeeded)
                    return RedirectToAction("ResetPasswordConfirmation", "Account");

                AddErrors(result);
            }


            return View();
        }
        
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        // GET: /Account/CreatePassword
        [AllowAnonymous]
        public async Task<ActionResult> CreatePassword(string userId, string code) //code is ConfirmEmail Code (token)
        {
            if (userId == null || code == null)
                return View("Error");

            var user = await UserManager.FindByIdAsync(userId);
            if (user == null)
                return View("Error");

            var currentLoggedUserId = this.User.Identity.GetUserId();
            if (!string.IsNullOrEmpty(currentLoggedUserId) && currentLoggedUserId != userId)
            {
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                Session.Abandon();
            }

            var result = await UserManager.ConfirmEmailAsync(userId, code);
            if (!result.Succeeded)
            {
                if (result.Errors != null && result.Errors.ElementAtOrDefault(0) != null)
                {
                    TempData["ErrorMessage"] = result.Errors.ElementAtOrDefault(0).ToString();
                }
                return View("Error");
            }

            var model = new ResetPasswordViewModel { Email = user.Email, Code = code };
            return View(model);
        }

        //
        // POST: /Account/CreatePassword
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken] //This is generating a error because of the SignOut.
        public async Task<ActionResult> CreatePassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                Session.Abandon();
                return RedirectToAction("Index", "Home");
            }

            //var result = await UserManager.ConfirmEmailAsync(user.Id, model.Code);
            //if (!result.Succeeded)
            //{
            //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            //    Session.Abandon();
            //    return RedirectToAction("Index", "Home");
            //}

            var resultPwd = await UserManager.AddPasswordAsync(user.Id, model.Password);
            if (!resultPwd.Succeeded)
            {
                AddErrors(resultPwd);
                return View();
            }

            return RedirectToAction("Index", "Home");
        }

        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
                return View("Error");
            
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
                return View();

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
                return View("Error");
            
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
                return RedirectToAction("Login");

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                Session.Abandon();
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                    return View("ExternalLoginFailure");
                
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    UserType = model.UserType,
                    PhoneNumber = model.PhoneNumber,
                };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        Session.Abandon();

                        if (returnUrl == null)
                            return RedirectToAction("ContinueRegistration", "Home");
                        
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session.Abandon();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult LogOff(int? dummy)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
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

        #endregion
    }

}