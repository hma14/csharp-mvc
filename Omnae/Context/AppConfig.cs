using System.Collections.Generic;
using System.Configuration;
using Intuit.Ipp.OAuth2PlatformClient;

namespace Omnae.Context
{
    public class AppConfig 
    {
        public static string Mod { get; set; }
        public static string Expo { get; set; }

        public static string ClientId { get; } = ConfigurationManager.AppSettings["clientid"];
        public static string ClientSecret { get; } = ConfigurationManager.AppSettings["clientsecret"];
        public static string RedirectUrl { get; } = ConfigurationManager.AppSettings["redirectUrl"];

        public static string AuthorizeUrl { get; set; } = "";
        public static string TokenEndpoint { get; set; } = "";
        public static string RevocationEndpoint { get; set; } = "";
        public static string UserInfoEndpoint { get; set; } = "";
        public static string IssuerEndpoint { get; set; } = "";

        public static IList<JsonWebKey> Keys { get; set; }
    }
}