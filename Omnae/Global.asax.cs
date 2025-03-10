using Omnae.Libs;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AutoMapper;
using Omnae.BusinessLayer.Models;
using Omnae.Model.Models;
using Omnae.Model.ViewModels;
using Omnae.Util;
using Omnae.ViewModels;
using StackExchange.Profiling;
using StackExchange.Profiling.EntityFramework6;
using StackExchange.Profiling.Mvc;
using Twilio;
using Mapper = AutoMapper.Mapper;

namespace Omnae
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            AntiForgeryConfig.SuppressIdentityHeuristicChecks = true;
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var config = new TemplateServiceConfiguration();
            config.Debug = true;
            config.TemplateManager = new MyTemplateManager("Template");
            Engine.Razor = RazorEngineService.Create(config);

            // create ATrigger Singleton
            ATriggerApi.Initialize();

            ConfigureMiniProfiler();
            ConfigureTwilio();
        }

        private static void ConfigureTwilio()
        {
            TwilioClient.Init(ConfigurationManager.AppSettings["SMSAccountIdentification"], ConfigurationManager.AppSettings["SMSAccountPassword"]);
        }

        private void ConfigureMiniProfiler()
        {
            MiniProfiler.Configure(new MiniProfilerOptions
            {
                // Sets up the route to use for MiniProfiler resources:
                // Here, ~/profiler is used for things like /profiler/mini-profiler-includes.js
                //RouteBasePath = "~/profiler",

                // Example of using SQLite storage instead
                //Storage = new SqliteMiniProfilerStorage(ConnectionString),

                // Different RDBMS have different ways of declaring sql parameters - SQLite can understand inline sql parameters just fine.
                // By default, sql parameters will be displayed.
                //SqlFormatter = new StackExchange.Profiling.SqlFormatters.InlineFormatter(),

                // These settings are optional and all have defaults, any matching setting specified in .RenderIncludes() will
                // override the application-wide defaults specified here, for example if you had both:
                //    PopupRenderPosition = RenderPosition.Right;
                //    and in the page:
                //    @MiniProfiler.Current.RenderIncludes(position: RenderPosition.Left)
                // ...then the position would be on the left on that page, and on the right (the application default) for anywhere that doesn't
                // specified position in the .RenderIncludes() call.
                PopupRenderPosition = RenderPosition.Right,  // defaults to left
                PopupMaxTracesToShow = 10,                   // defaults to 15

                // ResultsAuthorize (optional - open to all by default):
                // because profiler results can contain sensitive data (e.g. sql queries with parameter values displayed), we
                // can define a function that will authorize clients to see the JSON or full page results.
                // we use it on http://stackoverflow.com to check that the request cookies belong to a valid developer.
                ResultsAuthorize = request => request.IsARequestFromADev(),

                // ResultsListAuthorize (optional - open to all by default)
                // the list of all sessions in the store is restricted by default, you must return true to allow it
                ResultsListAuthorize = request =>
                {
                    // you may implement this if you need to restrict visibility of profiling lists on a per request basis
                    return true; // all requests are legit in this example
                },

                // Stack trace settings
                StackMaxLength = 256, // default is 120 characters

                // (Optional) You can disable "Connection Open()", "Connection Close()" (and async variant) tracking.
                // (defaults to true, and connection opening/closing is tracked)
                TrackConnectionOpenClose = true
            }
            // Optional settings to control the stack trace output in the details pane, examples:
            .ExcludeType("SessionFactory")  // Ignore any class with the name of SessionFactory)
            .ExcludeAssembly("NHibernate")  // Ignore any assembly named NHibernate
            .ExcludeMethod("Flush")         // Ignore any method with the name of Flush
            .AddViewProfiling()              // Add MVC view profiling (you want this)
                                             // If using EntityFrameworkCore, here's where it'd go.
                                             // .AddEntityFramework()        // Extension method in the MiniProfiler.EntityFrameworkCore package
            );

            // If we're using EntityFramework 6, here's where it'd go.
            // This is in the MiniProfiler.EF6 NuGet package.
            MiniProfilerEF6.Initialize();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();

            HttpContext ctx = HttpContext.Current;
            KeyValuePair<string, object> error = new KeyValuePair<string, object>("ErrorMessage", ctx.Server.GetLastError().ToString());
            ctx.Response.Clear();
            RequestContext rc = ((MvcHandler)ctx.CurrentHandler).RequestContext;
            string controllerName = rc.RouteData.GetRequiredString("controller");
            IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();
            IController controller = factory.CreateController(rc, controllerName);
            ControllerContext cc = new ControllerContext(rc, (ControllerBase)controller);

            ViewResult viewResult = new ViewResult { ViewName = "Error" };
            viewResult.ViewData.Add(error);
            viewResult.ExecuteResult(cc);
            ctx.Server.ClearError();
        }

        protected void Application_BeginRequest()
        {
            // You can decide whether to profile here, or it can be done in ActionFilters, etc.
            // We're doing it here so profiling happens ASAP to account for as much time as possible.
            if (Request.IsARequestFromADev()) // Example of conditional profiling, you could just call MiniProfiler.StartNew();
            {
                MiniProfiler.StartNew();
            }
        }

        protected void Application_EndRequest()
        {
            MiniProfiler.Current?.Stop(); // Be sure to stop the profiler!
        }
    }
}
