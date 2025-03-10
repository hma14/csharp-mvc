using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Omnae.BackgroundWorkers.Model;
using Omnae.BackgroundWorkers.Service;
using RestSharp.Deserializers;
using Serilog;

namespace Omnae.BackgroundWorkers
{
    public static class EmailSender
    {
        private const string QueueName = EmailSenderService.QueueName;

        static EmailSender()
        {
            Startup.RedirectAssembies();
        }

        [FunctionName("EmailSender")]
        public static async Task Run([QueueTrigger(QueueName)] string emailDataSerialized, TraceWriter log)
        {
            ILogger logger = null;
            try
            {
                //if(Debugger.IsAttached)
                //    return;

                logger = SerilogConfig.InitializeLog(log);
                var service = new EmailSenderService(logger);

                Log.Information("EmailSenderService: Started");

                var emailData = JsonConvert.DeserializeObject<EmailIdMessage>(emailDataSerialized);
                await service.SyncAsync(emailData);

                Log.Information("EmailSenderService: Ended");
            }
            finally
            {
                (logger as IDisposable)?.Dispose();
                Log.CloseAndFlush();
            }

            //log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
