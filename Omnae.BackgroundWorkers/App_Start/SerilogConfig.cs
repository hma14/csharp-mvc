using System;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.AzureWebJobsTraceWriter;
using Microsoft.Azure.WebJobs.Host;

namespace Omnae.BackgroundWorkers
{
    public static class SerilogConfig
    {
        public static LoggingLevelSwitch LoggingLevelSwitch { get; } = new LoggingLevelSwitch(LogEventLevel.Debug);

        public static ILogger InitializeLog(TraceWriter tracer)
        {
            var applicationID = "Omnae.BackgroundWorkers";
            //var environment = ConfigurationManager.AppSettings["Log.Environment"];

            //Serilog.Debugging.SelfLog.Enable(Console.Error); //Debug

            var log = Log.Logger = new LoggerConfiguration()
                         .Enrich.FromLogContext()
                         //.Enrich.WithProperty("Environment", environment)
                         .Enrich.WithProperty("Application", applicationID)
                         .Enrich.WithProperty("ExecutionId", Guid.NewGuid())
                         .MinimumLevel.ControlledBy(LoggingLevelSwitch)

                         .WriteTo.TraceWriter(tracer) //Debug
                         .WriteTo.Console()
                         .CreateLogger();

            return log;
        }
    }
}
