using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.Util
{
    public class ElmahSecurity : Elmah.ErrorLogPageFactory
    {
        public override IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
        {
            var isDev = context.Request.IsARequestFromADev();
            if (isDev)
            {
                return base.GetHandler(context, requestType, url, pathTranslated);
            }

            HttpResponse response = context.Response;
            response.Status = "403 Forbidden";
            response.Write("You don't have permitting to be here");
            response.End();
            return null;
        }

        public override void ReleaseHandler(IHttpHandler handler)
        {
            base.ReleaseHandler(handler);
        }
    }
}