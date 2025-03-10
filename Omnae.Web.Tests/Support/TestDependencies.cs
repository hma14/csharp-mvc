using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Testing;
using SpecFlow.Unity;
using TechTalk.SpecFlow;
using Unity;
using Unity.RegistrationByConvention;

namespace Omnae.Web.Tests.Support
{
    public static class TestDependencies
    {
        [ScenarioDependencies]
        public static IUnityContainer CreateContainer()
        {
            var container = UnityTestConfig.GetConfiguredContainer();
            container.RegisterTypes(typeof(TestDependencies).Assembly.GetTypes().Where(t => Attribute.IsDefined(t, typeof(BindingAttribute))), WithMappings.FromMatchingInterface, WithName.Default, WithLifetime.ContainerControlled);

            StartUp();

            return container;
        }

        private static void StartUp()
        {
            TestServer.Create(app =>
            {
                var st = new Startup();
                st.ConfigureAuth(app, new DpapiDataProtectionProvider("TestServer"));
            });

            //var routes = new RouteCollection();
            //MvcApplication.RegisterRoutes(routes);

            //var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            //request.SetupGet(x => x.ApplicationPath).Returns("/");
            //request.SetupGet(x => x.Url).Returns(new Uri("http://localhost/a", UriKind.Absolute));
            //request.SetupGet(x => x.ServerVariables).Returns(new System.Collections.Specialized.NameValueCollection());

            //var response = new Mock<HttpResponseBase>(MockBehavior.Strict);
            //response.Setup(x => x.ApplyAppPathModifier("/post1")).Returns("http://localhost/post1");

            //var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            //context.SetupGet(x => x.Request).Returns(request.Object);
            //context.SetupGet(x => x.Response).Returns(response.Object);

            //var controller = new LinkbackController(dbF.Object);
            //controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            //controller.Url = new UrlHelper(new RequestContext(context.Object, new RouteData()), routes);


            HttpContext.Current = FakeHttpContext();
        }

        public static HttpContext FakeHttpContext()
        {
            var httpRequest = new HttpRequest("", "http://omnaeDevTest/", "");
            var stringWriter = new StringWriter();
            var httpResponse = new HttpResponse(stringWriter);
            var httpContext = new HttpContext(httpRequest, httpResponse);

            var sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(),
                new HttpStaticObjectsCollection(), 10, true, HttpCookieMode.AutoDetect, SessionStateMode.InProc, false);

            SessionStateUtility.AddHttpSessionStateToContext(httpContext, sessionContainer);
           
            return httpContext;
        }
    }
}
