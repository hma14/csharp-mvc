using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Omnae.BusinessLayer;
using Omnae.Common;
using RestSharp;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Auth0.ManagementApi.Paging;
using Omnae.BusinessLayer.Identity.Model;
using Omnae.BusinessLayer.Interface;
using Omnae.BusinessLayer.Interface.Model;
using Omnae.Common.Extensions;

#pragma warning disable 1591

namespace Omnae.WebApi.Services
{
    public class AuthZeroManager : IAuthZeroService
    {
        private static readonly TimeSpan TokeExpirationTimeSpan = TimeSpan.FromSeconds(80000);
        private static DateTime? TokenRequestDateTime { get; set; }
        private static string Token { get; set; }
        private static readonly object _lockObj = new object();

        public ManagementApiClient Client { get; }
        
        public AuthZeroManager()
        {
            var auth0_domain = ConfigurationManager.AppSettings["autho0_domain"];

            RefreshToken();
            Client = new ManagementApiClient(Token, new Uri(auth0_domain));
        }

        private static void RefreshToken()
        {
            if (Token == null || TokenIsExpiredOrWillSoon())
            {
                lock (_lockObj)
                {
                    if (Token == null || TokenIsExpiredOrWillSoon())
                    {
                        Token = RequestRefreshToken();
                        TokenRequestDateTime = DateTime.UtcNow;
                    }
                }
            }
        }

        private static bool TokenIsExpiredOrWillSoon()
        {
            if (TokenRequestDateTime == null)
                return true;
            return (((DateTime) TokenRequestDateTime).Add(TokeExpirationTimeSpan) <= DateTime.UtcNow.AddMinutes(+5));
        }

        private static string RequestRefreshToken()
        {
            var client_id = ConfigurationManager.AppSettings["auth0_client_id"];
            var client_secret = ConfigurationManager.AppSettings["auth0_client_secret"];
            var auth0_base = ConfigurationManager.AppSettings["auth0_base"];
            var auth0_domain = ConfigurationManager.AppSettings["autho0_domain"];

            //TODO: Refactor, use the External ClientId and Secret.
            var client = new RestClient(auth0_base);
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{\"client_id\":\"" + client_id + "\",\"client_secret\":\"" + client_secret + "\",\"audience\":\"" + auth0_domain + "\",\"grant_type\":\"client_credentials\"}", ParameterType.RequestBody);
            var response = client.Execute<OauthTokenResponse>(request);

            return response.Data.access_token;
        }

        public OAuthUserData GetUserByLagacyIdAsync(string userId)
        {
            var legacyId = userId;
            var result = Client.Users.GetAllAsync(new GetUsersRequest { SearchEngine = "v3", Query = $"app_metadata.legacyId:\"{legacyId}\"" }, new PaginationInfo()).GetAwaiter().GetResult();

            if (result?.Count == null)
            {
                throw new BLException("User not Found");
            }

            if (result.Count == 0)
            {
                throw new BLException("Error trying to found the User");
            }

            var oAuthUser = result.Single();

            if (oAuthUser.Blocked == true)
            {
                throw new BLException("User don't have permission to use this api.");
            }

            if (oAuthUser.AppMetadata.companyId == null || oAuthUser.AppMetadata.companyId == 0)
            {
                throw new BLException("User don't have a associated company.");
            }

            var data = new OAuthUserData()
            {
                oAuthUserId = oAuthUser.UserId.Split('|')[1],
                LegacyId = oAuthUser.AppMetadata.legacyId?.ToString(),

                UserType = USER_TYPE.Customer, //oAuthUser.AppMetadata
                UserName = oAuthUser.Email, //oAuthUser.UserName,
                FirstName = oAuthUser.FirstName,
                LastName = oAuthUser.LastName,

                Email = oAuthUser.Email,
                EmailConfirmed = oAuthUser.EmailVerified ?? false,
                PhoneNumber = oAuthUser.PhoneNumber,
                PhoneNumberConfirmed = oAuthUser.PhoneVerified ?? false,

                CompanyId = !string.IsNullOrEmpty((string) oAuthUser.AppMetadata.companyId) ? (int)oAuthUser.AppMetadata.companyId : (int?)null,
                Blocked = oAuthUser.Blocked ?? false,
            };

            return data;
        }

        public OAuthUserData GetUserByAuthZeroIdAsync(string oAuthUserId)
        {
            var oAuthUser = Client.Users.GetAsync(oAuthUserId).GetAwaiter().GetResult();

            if (oAuthUser == null)
            {
                throw new BLException("User not Found");
            }

            if (oAuthUser.Blocked == true)
            {
                throw new BLException("User don't have permission to use this api.");
            }

            if (oAuthUser.AppMetadata.companyId == null || oAuthUser.AppMetadata.companyId == 0)
            {
                throw new BLException("User don't have a associated company.");
            }

            var data = Map(oAuthUser);

            return data;
        }

        private static OAuthUserData Map(User oAuthUser)
        {
            var data = new OAuthUserData()
            {
                oAuthUserId = oAuthUser.UserId,
                LegacyId = oAuthUser.AppMetadata.legacyId?.ToString(),
                UserType = oAuthUser.AppMetadata.userType,
                UserName = oAuthUser.Email, //oAuthUser.UserName,
                FirstName = oAuthUser.FirstName ?? oAuthUser.UserMetadata.name,
                LastName = oAuthUser.LastName,

                Email = oAuthUser.Email,
                EmailConfirmed = oAuthUser.EmailVerified ?? false,
                PhoneNumber = oAuthUser.PhoneNumber ?? oAuthUser.UserMetadata.phone,
                PhoneNumberConfirmed = oAuthUser.PhoneVerified ?? false,

                CompanyId = !string.IsNullOrEmpty((string) oAuthUser.AppMetadata.companyId)
                    ? (int) oAuthUser.AppMetadata.companyId
                    : (int?) null,
                Blocked = oAuthUser.Blocked ?? false,
            };
            return data;
        }

        //public async Task<OAuthUserData> CreateOrGetUser(ApplicationUser user, int companyId, string newPassword)
        //{
        //    if(user == null)
        //        throw new ArgumentNullException();

        //    User authZeroUser = null;

        //    if (!string.IsNullOrWhiteSpace(user.Id))
        //    {
        //        authZeroUser = (await Client.Users.GetAllAsync(new GetUsersRequest{SearchEngine = "v3", Query = $"app_metadata.legacyId:\"{user.Id}\""}, new PaginationInfo())).FirstOrDefault();
        //    }
        //    if (!string.IsNullOrWhiteSpace(user.Email))
        //    {
        //        authZeroUser = (await Client.Users.GetUsersByEmailAsync(user.Email)).FirstOrDefault();
        //    }

        //    if (authZeroUser != null)
        //        return Map(authZeroUser);

        //    var newUser = await Client.Users.CreateAsync(new UserCreateRequest()
        //    {
        //        //UserName = user.UserName,

        //        Email = user.Email,
        //        EmailVerified = user.EmailConfirmed,
        //        VerifyEmail = false,//!user.EmailConfirmed,

        //        FullName = $"{user.FirstName ?? ""} {user.MiddleName ?? ""} {user.LastName ?? ""}".TrimAll(),
        //        FirstName = $"{user.FirstName ?? ""} {user.MiddleName ?? ""}".TrimAll(),
        //        LastName = user.LastName,

        //        //PhoneNumber = user.PhoneNumber,

        //        Password = newPassword,
        //        AppMetadata = new
        //        {
        //            companyId = user.CompanyId,
        //            active = true,
        //            userType = "CUSTOMER",
        //            role = "OWNER",
        //            customerRole = "ENGINEER",
        //            vendorRole = "ENGINEER"
        //        },
        //        UserMetadata = new
        //        {
        //            invitationId = (string) null,
        //        },
        //        Connection = "Username-Password-Authentication",
        //    });

        //    return Map(newUser);
        //}

        private class OauthTokenResponse
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
        }

    }
}