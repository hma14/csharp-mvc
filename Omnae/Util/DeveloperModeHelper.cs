using System.Web;

namespace Omnae.Util
{
    public static class DeveloperModeHelper
    {
        private const string DevCookieName = "omnaeDEV";
        private const string DevCookieValue = "ksjdkj827B@bainbayt15cHGbaFA";

        public static bool IsARequestFromADev(this HttpRequest request)
        {
            return request.IsLocal || request.Cookies[DevCookieName]?.Value == DevCookieValue;
        }
    }
}