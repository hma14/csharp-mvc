using ApiMultiPartFormData;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;

namespace Omnae.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            // Set media type to json
            var json = config.Formatters.JsonFormatter;
            //json.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("multipart/form-data"));
            json.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
            json.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize;
            //json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Arrays;
            json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;

            config.Formatters.Add(new MultipartFormDataFormatter());

            // Remove the XM Formatter from the web api
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            


            // setup camel-case for property names
            var settings = GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings;
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


            //Filters
            //TODO: Put the Auth Token in a secure place
            var aceptToken = "H4RtPzdm14JhxBZhtZWgcCeXG4vwkBdWRdYNpbanRSQk4nYKhG4qHj5lY4tqLR7wsgdTkqx0eh1eJxysy69Kev8ktfJfahfCMV1ftLGbehG6OQOllXXTHlJQo9XAOqxIOSvCFTBE6pTACaP4vK3Tbt4Lxz4JXaIidy2Dsr9qpOBsxPmK4GOy2xr4PypBJ7ewmHO79UgB2A0n63FKSwD5B9neWZ0rSQFpNk0zm56U5Qm0SkMoaw7N2HIDHMpjHgXkQf6I69wSjW9zSNAo7RPGT2XtRUxOHRH8Ln9SNVb4NKyayATLI2am69M7H4LDWrOaWFq2XYBVDBDc9s0nd2LqTc1L5bGc9XW2etbs1UY0g9mS3MgmfS9UcPWHvvHqIzFJnLokoToipo641cBm8MmbMMrqpFR69B88C7HRSBaIPqH6TOJkVxo0rXmbgw6lkNQKieXAcGqMjF1UcpyNUenEsUdUhVnjVXuaX7AtCSBwZQmgJWsICkHnZtspQCs7yYK2";
            config.Filters.Add(new ApiKeyFilterAttribute(aceptToken)); //Auth
        }
    }
}
