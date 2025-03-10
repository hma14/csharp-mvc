using System;
using System.Configuration;
using System.Web;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Json;
using System.IO;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.SqlServer.Destructurers;

namespace Omnae.WebApi
{
    public static class SerilogConfig
    {
        public static LoggingLevelSwitch LoggingLevelSwitch { get; } = new LoggingLevelSwitch(LogEventLevel.Debug);

        public static ILogger InitializeLog()
        {
            var applicationID = "Omnae.WebApi";
            var environment = ConfigurationManager.AppSettings["Log.Environment"];

            //Serilog.Debugging.SelfLog.Enable(Console.Error); //Debug

            var log = Log.Logger = new LoggerConfiguration()
                         .Enrich.FromLogContext()
                         .Enrich.WithThreadId() //Debug
                         .Enrich.WithProperty("Environment", environment)
                         .Enrich.WithProperty("Application", applicationID)
                         .Enrich.WithProperty("ExecutionId", Guid.NewGuid())
                         .Enrich.WithWebApiControllerName()
                         .Enrich.WithWebApiActionName()
                         .Enrich.WithHttpRequestUrl()
                         .Enrich.WithUserName()
                         .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                             .WithDefaultDestructurers()
                             .WithDestructurers(new[] { new SqlExceptionDestructurer() }))

                         .MinimumLevel.ControlledBy(LoggingLevelSwitch)

                         .WriteTo.ApplicationInsights(TelemetryConfiguration.Active, TelemetryConverter.Traces)
                         .WriteTo.Trace() //Debug
                         .CreateLogger();

            Log.Information("Log Created.");

            return log;
        }
    }
}
