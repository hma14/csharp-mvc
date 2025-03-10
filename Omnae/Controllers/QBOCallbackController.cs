using Intuit.Ipp.OAuth2PlatformClient;
using Newtonsoft.Json;
using Omnae.BlobStorage;
using Omnae.Model.Models;
using Omnae.Service.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Libs.Notification;
using Omnae.BusinessLayer;
using Omnae.BusinessLayer.Identity.Model;
using Omnae.BusinessLayer.Services;
using Omnae.Context;
using Omnae.Service.Service;
using AutoMapper;
using Omnae.Libs.Notification;
using Omnae.Model.Context;
using System.Configuration;

namespace Omnae.Controllers
{
    public class QBOCallbackController : BaseController
    {
        private IdTokenJWTClaimTypes PayloadData { get; set; }

        public QBOCallbackController(IRFQBidService rfqBidService, ICompanyService companyService, ITaskDataService taskDataService, IPriceBreakService priceBreakService, IOrderService orderService, ILogedUserContext userContext, IProductService productService, IDocumentService documentService, IShippingService shippingService, ICountryService countryService, IAddressService addressService, IStateProvinceService stateProvinceService, IOrderStateTrackingService orderStateTrackingService, IProductStateTrackingService productStateTrackingService, IPartRevisionService partRevisionService, IStoredProcedureService spService, INCReportService ncReportService, IRFQQuantityService rfqQuantityService, IExtraQuantityService extraQuantityService, IBidRequestRevisionService bidRequestRevisionService, ITimerSetupService timerSetupService, IQboTokensService qboTokensService, IOmnaeInvoiceService omnaeInvoiceService, INCRImagesService ncrImagesService, IApprovedCapabilityService approvedCapabilityService, IShippingAccountService shippingAccountService, ApplicationDbContext dbUser, ProductBL productBl, NotificationService notificationService, UserContactService userContactService, TimerTriggerService timerTriggerService, NotificationBL notificationBl, PaymentBL paymentBl, ShipmentBL shipmentBl, ChartBL chartBl, IMapper mapper, NcrBL ncrBL, IDocumentStorageService documentStorageService, IImageStorageService imageStorageService) : base(rfqBidService, companyService, taskDataService, priceBreakService, orderService, userContext, productService, documentService, shippingService, countryService, addressService, stateProvinceService, orderStateTrackingService, productStateTrackingService, partRevisionService, spService, ncReportService, rfqQuantityService, extraQuantityService, bidRequestRevisionService, timerSetupService, qboTokensService, omnaeInvoiceService, ncrImagesService, approvedCapabilityService, shippingAccountService, dbUser, productBl, notificationService, userContactService, timerTriggerService, notificationBl, paymentBl, shipmentBl, chartBl, mapper, ncrBL, documentStorageService, imageStorageService)
        {
        }

        public async Task<ActionResult> Index()
        {
            ViewBag.Code = Request.QueryString["code"] ?? "none";
            ViewBag.RealmId = Request.QueryString["realmId"] ?? "none";

            var state = Request.QueryString["state"];
            var tempState = await GetTempStateAsync();

            if (tempState != null && state.Equals(tempState.Item1, StringComparison.Ordinal))
            {
                ViewBag.State = state + " (valid)";
            }
            else
            {
                ViewBag.State = state + " (invalid)";
            }

            ViewBag.Error = Request.QueryString["error"] ?? "none";

            return View();
        }

        [HttpPost]
        [ActionName("Index")]
        public async Task<ActionResult> Token()
        {
            //Request Oauth2 tokens
            var tokenEndpoint = ConfigurationManager.AppSettings["TokenEndpoint"];
            var tokenClient = new TokenClient(tokenEndpoint, AppConfig.ClientId, AppConfig.ClientSecret);
            
            var code = Request.QueryString["code"];
            var realmId = Request.QueryString["realmId"];
            if (realmId != null)
            {
                Session["realmId"] = realmId;
            }
            var tempState = await GetTempStateAsync();
            Request.GetOwinContext().Authentication.SignOut("TempState"); //TODO: Check Why have this code.
            Session.Abandon();
            
            var response = await tokenClient.RequestTokenFromCodeAsync(code, AppConfig.RedirectUrl);

            await ValidateResponseAndSignInAsync(response);

            if (!string.IsNullOrEmpty(response.IdentityToken))
            {
                ViewBag.IdentityToken = response.IdentityToken;
            }
            if (!string.IsNullOrEmpty(response.AccessToken))
            {
                ViewBag.AccessToken = response.AccessToken;
            }
            
            // Store tokens in QboTokens table
            QboTokens qboTokens = QboTokensService.FindQboTokensList().FirstOrDefault();
            if (qboTokens != null)
            {
                qboTokens.RealmId = realmId;
                qboTokens.AccessToken = response.AccessToken;
                qboTokens.RefreshToken = response.RefreshToken;
                if (response.RefreshTokenExpiresIn > 0)
                {
                    qboTokens.RefeshTokenExpireAt = DateTime.Now.AddSeconds(response.RefreshTokenExpiresIn).ToLocalTime();
                }
                qboTokens.AccessTokenCreatedAt = DateTime.Now;
                qboTokens.TokenEndpoint = AppConfig.TokenEndpoint;
                QboTokensService.UpdateQboTokens(qboTokens);
            }
            else
            {
                QboTokens entity = new QboTokens
                {
                    RealmId = realmId,
                    AccessToken = response.AccessToken,
                    RefreshToken = response.RefreshToken,
                    RefeshTokenExpireAt = DateTime.Now.AddSeconds(response.RefreshTokenExpiresIn).ToLocalTime(),
                    AccessTokenCreatedAt = DateTime.Now,
                    TokenEndpoint = AppConfig.TokenEndpoint,
                };
                QboTokensService.AddQboTokens(entity);
            }

            return View("Token", response);
        }

        private async Task ValidateResponseAndSignInAsync(TokenResponse response)
        {
            var claims = new List<Claim>();
            if (!string.IsNullOrWhiteSpace(response.IdentityToken))
            {
                bool isIdTokenValid = ValidateToken(response.IdentityToken);//throw error is not valid

                if (isIdTokenValid == true)
                {
                    claims.Add(new Claim("id_token", response.IdentityToken));
                }
            }
            if (Session["realmId"] != null)
            {
                claims.Add(new Claim("realmId", Session["realmId"].ToString()));
            }

            if (!string.IsNullOrWhiteSpace(response.AccessToken))
            {
                var userClaims = await GetUserInfoClaimsAsync(response.AccessToken);
                claims.AddRange(userClaims);

                claims.Add(new Claim("access_token", response.AccessToken));
                claims.Add(new Claim("access_token_expires_at", (DateTime.Now.AddSeconds(response.AccessTokenExpiresIn)).ToString()));
            }

            if (!string.IsNullOrWhiteSpace(response.RefreshToken))
            {
                claims.Add(new Claim("refresh_token", response.RefreshToken));
                claims.Add(new Claim("refresh_token_expires_at", (DateTime.Now.AddSeconds(response.RefreshTokenExpiresIn)).ToString()));
            }

            var id = new ClaimsIdentity(claims, "Cookies");
            Request.GetOwinContext().Authentication.SignIn(id);
        }

        private bool ValidateToken(string identityToken)
        {
            if (AppConfig.Keys == null)
                return false; //Missing mod and expo values

            if (identityToken == null)
                return false; //IdentityToken
            
            //Split the identityToken to get Header and Payload
            string[] splitValues = identityToken.Split('.');
            if (splitValues[0] != null)
            {
                //Decode header 
                var headerJson = Encoding.UTF8.GetString(Base64Url.Decode(splitValues[0].ToString()));

                //Deserilaize headerData
                IdTokenHeader headerData = JsonConvert.DeserializeObject<IdTokenHeader>(headerJson);

                //Verify if the key id of the key used to sign the payload is not null
                if (headerData.Kid == null)
                    return false;

                //Verify if the hashing alg used to sign the payload is not null
                if (headerData.Alg == null)
                    return false;
            }

            if (splitValues[1] != null)
            {
                //Decode payload
                var payloadJson = Encoding.UTF8.GetString(Base64Url.Decode(splitValues[1].ToString()));
                PayloadData = JsonConvert.DeserializeObject<IdTokenJWTClaimTypes>(payloadJson);

                //Verify Aud matches ClientId
                if (PayloadData.Aud == null)
                    return false;
                
                if (PayloadData.Aud[0].ToString() != AppConfig.ClientId)
                    return false;

                //Verify Authtime matches the time the ID token was authorized.                
                if (PayloadData.Auth_time == null)
                    return false;

                //Verify exp matches the time the ID token expires, represented in Unix time (integer seconds).                
                if (PayloadData.Exp != null)
                {
                    long expiration = Convert.ToInt64(PayloadData.Exp);
                    long currentEpochTime = EpochTimeExtensions.ToEpochTime(DateTime.UtcNow);
                    //Verify the ID expiration time with what expiry time you have calculated and saved in your application
                    //If they are equal then it means IdToken has expired 

                    if ((expiration - currentEpochTime) <= 0)
                        return false;
                }

                //Verify Iat matches the time the ID token was issued, represented in Unix time (integer seconds).            
                if (PayloadData.Iat == null)
                    return false;

                //verify Iss matches the  issuer identifier for the issuer of the response.     
                if (PayloadData.Iss == null)
                    return false;

                if (PayloadData.Iss.ToString() != AppConfig.IssuerEndpoint)
                    return false;

                //Verify sub. Sub is an identifier for the user, unique among all Intuit accounts and never reused. 
                //An Intuit account can have multiple emails at different points in time, but the sub value is never changed.
                //Use sub within your application as the unique-identifier key for the user.
                if (PayloadData.Sub == null)
                    return false;
            }

            //Use external lib to decode mod and expo value and generte hashes
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            //Read values of n and e from discovery document.
            rsa.ImportParameters(
                new RSAParameters()
                {
                    //Read values from discovery document
                    Modulus = Base64Url.Decode(AppConfig.Mod),
                    Exponent = Base64Url.Decode(AppConfig.Expo)
                });

            //Verify Siganture hash matches the signed concatenation of the encoded header and the encoded payload with the specified algorithm
            SHA256 sha256 = SHA256.Create();

            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(splitValues[0] + '.' + splitValues[1]));

            RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
            rsaDeformatter.SetHashAlgorithm("SHA256");

            return rsaDeformatter.VerifySignature(hash, Base64Url.Decode(splitValues[2]));
        }

        private async Task<IEnumerable<Claim>> GetUserInfoClaimsAsync(string accessToken)
        {
            //Get UserInfo data when correct scope is set for SIWI and Get App now flows
            var userInfoClient = new UserInfoClient(AppConfig.UserInfoEndpoint);
            UserInfoResponse userInfoResponse = await userInfoClient.GetAsync(accessToken);
            return (userInfoResponse.HttpStatusCode == HttpStatusCode.OK) ? userInfoResponse.Json.ToClaims() : new List<Claim>();;
        }
        
        private async Task<Tuple<string>> GetTempStateAsync()
        {
            var data = await Request.GetOwinContext().Authentication.AuthenticateAsync("TempState");
            if (data == null)
                return null;
            
            var state = data.Identity.FindFirst("state").Value;
            return Tuple.Create(state);
        }
    }
}