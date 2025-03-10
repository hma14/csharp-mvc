using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Mailgun;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.Sql.Fluent.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Omnae.DemoBackgroundWorkers
{
    public static class RestoreDemoDatabase
    {
        private static readonly string dbFromName = "OmnaeDbDemo_ReadyToDemo_v3";
        private static readonly string dbToName = "OmnaeDbDemo";

        [FunctionName("RestoreDemoDatabase")]
        public static async Task Run([QueueTrigger("demo-restore-db", Connection = "AzureWebJobsStorage")]string myQueueItem, ILogger log, ExecutionContext context)
        {
            log.LogInformation("Database Restore Started.");

            try
            {            
                var config = new ConfigurationBuilder()
                            .SetBasePath(context.FunctionAppDirectory)
                            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                            .AddEnvironmentVariables()
                            .Build();


                log.LogInformation($"Queue trigger function processed: {myQueueItem}");

                log.LogInformation("Connection to Azure Management....");

                var credentials = SdkContext.AzureCredentialsFactory.FromFile(Path.Combine(context.FunctionAppDirectory,"my.azureauth.txt"));
                var azure = await Azure.Configure()
                                       .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                                       .Authenticate(credentials)
                                       .WithDefaultSubscriptionAsync();

                log.LogInformation("Connected.");

                var sqlServer = await azure.SqlServers.GetByIdAsync("/subscriptions/8c276e77-b1a5-4335-991a-54a32710d4ee/resourceGroups/OmnaeDemo/providers/Microsoft.Sql/servers/omnaedemo-srv");

                log.LogInformation("Delete the Main Database. Start...");

                var dbToBeDeleted = await sqlServer.Databases.GetAsync(dbToName);
                if (dbToBeDeleted != null)
                {
                    await dbToBeDeleted.DeleteAsync();
                }

                log.LogInformation("Delete the Main Database. Ended.");


                log.LogInformation("Restore the database. Started....");

                var dbToRestore = sqlServer.Databases.Get(dbFromName);
                var database = await sqlServer.Databases.Define(dbToName)
                                                        .WithSourceDatabase(dbToRestore)
                                                        .WithMode(CreateMode.Copy)
                                                        .CreateAsync();

                await SendEmail(config, log);

                log.LogInformation("Restore the database. Ended.");

            }
            catch (Exception ex)
            {
                log.LogError("Error", ex);
                throw;
            }
            log.LogInformation("Database Restore ended.");
        }

        public static async Task SendEmail(IConfigurationRoot config, ILogger log)
        {
            //Email
            Email.DefaultSender = new MailgunSender(
                config["Mailgun.Domain"], // Mailgun Domain
                config["Mailgun.APIKEY"] // Mailgun API Key
            );

            var subject = $"Demo Environment - Request to restore the Database Ended - Date {DateTime.Now:s}";
            var email = Email
                .From("info@omnae.com", "Omnae Backend Services")
                .ReplyTo("hma@omnae.com", "Omnae Backend Services - Support")
                .Subject(subject)
                .Body(@"This is a automatic email just to notify that the DEMO Database restore ended.");

            var toEmails = config["SendToEmails"].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(e => e.Trim()).ToList();
            foreach (var toMail in toEmails)
            {
                email = email.To(toMail);
            }

            var response = await email.SendAsync();
            log.LogInformation("Email sended. Subject:{Subject}, Status:{Status}", subject, response.Successful);
        }
    }
}
