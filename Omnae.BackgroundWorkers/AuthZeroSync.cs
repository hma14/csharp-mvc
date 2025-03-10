using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Omnae.BackgroundWorkers.Service;
using Serilog;

namespace Omnae.BackgroundWorkers
{
    public static class AuthZeroSync
    {
        const string Every01Minutes = "0 */1 * * * *";
        const string Every05Minutes = "0 */5 * * * *";
        const string Every30Minutes = "0 */30 * * * *";

        static AuthZeroSync()
        {
            Startup.RedirectAssembies();
        }

        public static bool Runed { get; set; }

        [FunctionName("AuthZeroSync")]
        public static async Task Run([TimerTrigger(Every30Minutes)] TimerInfo myTimer, TraceWriter log)
        {
            if (CloudConfigurationManager.GetSetting("Enable_Auth0_Sync") != "true")
                return;

            ILogger logger = null;
            try
            {
                if(Debugger.IsAttached && Runed)
                    return;

                logger = SerilogConfig.InitializeLog(log);
                var service = new UserSyncService(logger);

                Log.Information("UserSyncService: Started");

                await service.SyncAsync();

                Log.Information("UserSyncService: Ended");

                Runed = true;
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
