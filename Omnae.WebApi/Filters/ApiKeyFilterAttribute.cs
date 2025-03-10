using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;

namespace System.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ApiKeyFilterAttribute : System.Web.Http.AuthorizeAttribute
    {
        private string ApiKey { get; }
        private const string HeaderName = "Authorization";

        public ApiKeyFilterAttribute(string allowedApiKey)
        {
            ApiKey = allowedApiKey ?? throw new ArgumentNullException(nameof(allowedApiKey));
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (actionContext.ControllerContext.ControllerDescriptor.ControllerName == "Ping")
                return true;

            // return base.IsAuthorized(actionContext);
            if (!actionContext.Request.Headers.Contains(HeaderName))
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.ExpectationFailed);
                actionContext.Response.ReasonPhrase = "Please provide the Authorization Bearer with Token header.";
                return false;
            }

            string authenticationToken = Convert.ToString(actionContext.Request.Headers.GetValues(HeaderName).FirstOrDefault());
            if($"Bearer {ApiKey}" != authenticationToken)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
                return false;
            }

            return true;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            base.OnAuthorization(actionContext);
        }
    }
}
