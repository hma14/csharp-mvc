using System;
using Microsoft.Azure;
using RestSharp;

namespace Omnae.SyncHobSpotUser.Auth0
{
    static class Auth0Token
    {
        private static readonly TimeSpan TokeExpirationTimeSpan = TimeSpan.FromSeconds(80000);
        private static DateTime? TokenRequestDateTime { get; set; }
        public static string Token { get; set; }

        private static readonly object _lockObj = new object();

        public static void RefreshToken()
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

            return ((DateTime)TokenRequestDateTime).Add(TokeExpirationTimeSpan) <= DateTime.UtcNow.AddMinutes(+5);
        }

        private static string RequestRefreshToken()
        {
            var client_id = Environment.GetEnvironmentVariable("auth0_client_id");
            var client_secret = Environment.GetEnvironmentVariable("auth0_client_secret");
            var auth0_base = Environment.GetEnvironmentVariable("auth0_base");
            var auth0_domain = Environment.GetEnvironmentVariable("auth0_domain");

            var client = new RestClient(auth0_base);
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{\"client_id\":\"" + client_id + "\",\"client_secret\":\"" + client_secret + "\",\"audience\":\"" + auth0_domain + "\",\"grant_type\":\"client_credentials\"}", ParameterType.RequestBody);
            
            var response = client.Execute<OauthTokenResponse>(request);
            return response.Data.access_token;
        }

        private class OauthTokenResponse
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
        }
    }
}