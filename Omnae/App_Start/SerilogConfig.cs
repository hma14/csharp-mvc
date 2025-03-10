using System;
using System.Configuration;
using System.Diagnostics;
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

namespace Omnae.App_Start
{
    public static class SerilogConfig
    {
        public static LoggingLevelSwitch LoggingLevelSwitch { get; } = new LoggingLevelSwitch(LogEventLevel.Debug);

        public static ILogger InitializeLog()
        {
            var applicationID = "Omnae.Web";
            var environment = ConfigurationManager.AppSettings["Log.Debug"];

            Serilog.Debugging.SelfLog.Enable(Console.Error); //Debug

            var log = Log.Logger = new LoggerConfiguration()
                         .Enrich.FromLogContext()
                         .Enrich.WithThreadId() //Debug
                         .Enrich.WithProperty("Environment", environment)
                         .Enrich.WithProperty("Application", applicationID)
                         .Enrich.WithProperty("ExecutionId", Guid.NewGuid())
                         .Enrich.WithMvcControllerName()
                         .Enrich.WithMvcActionName()
                         .Enrich.WithHttpRequestUrl()
                         .Enrich.WithUserName()
                         .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                             .WithDefaultDestructurers()
                             .WithDestructurers(new[] { new SqlExceptionDestructurer() }))

                         .MinimumLevel.ControlledBy(LoggingLevelSwitch)
                         
                         .WriteTo.ApplicationInsights(TelemetryConfiguration.Active, TelemetryConverter.Traces, LogEventLevel.Debug)
                         .WriteTo.Trace() //Debug
                         .CreateLogger();

            Log.Information("Log Created.");

            return log;
        }
    }
}
