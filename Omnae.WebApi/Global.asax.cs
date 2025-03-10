using Omnae.Libs;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Omnae.WebApi.App_Start;

namespace Omnae.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            UnityConfig.RegisterComponents();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register);            
            RouteConfig.RegisterRoutes(RouteTable.Routes);           
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            SlapperAutoMapperConfig.RegisterConverter();

            // create ATrigger Singleton
            ATriggerApi.Initialize();

            var config = new TemplateServiceConfiguration();
            config.Debug = true;
            config.TemplateManager = new MyTemplateManager("Template");
            Engine.Razor = RazorEngineService.Create(config);
        }
    }
}
