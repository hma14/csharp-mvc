using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Omnae.SyncHobSpotUser.Service;
//using Serilog;
using Microsoft.Extensions.Logging;

namespace Omnae.SyncHobSpotUser
{
    public static class AuthZeroSync
    {
        const string Every01Minutes = "0 */1 * * * *";
        const string Every05Minutes = "0 */5 * * * *";
        const string Every30Minutes = "0 */30 * * * *";

        static AuthZeroSync()
        {
            //Startup.RedirectAssembies();
        }

        public static bool Runed { get; set; }

        [FunctionName("AuthZeroSync")]
        public static async Task Run([TimerTrigger(Every30Minutes)] TimerInfo myTimer, ILogger log)
        {
            if (Environment.GetEnvironmentVariable("Enable_Auth0_Sync") != "true")
                return;

            try
            {
                if(Debugger.IsAttached && Runed)
                    return;

                //logger = SerilogConfig.InitializeLog(log);
                var service = new UserSyncService(log);

                log.LogInformation("UserSyncService: Started");

                await service.SyncAsync();

                log.LogInformation("UserSyncService: Ended");

                Runed = true;
            }
            finally
            {
                (log as IDisposable)?.Dispose();
                //Log.CloseAndFlush();
            }

            //log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
