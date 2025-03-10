using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentEmail.Mailgun;
using Microsoft.Azure;
using Newtonsoft.Json;
using Omnae.BackgroundWorkers.Auth0;
using Omnae.BackgroundWorkers.Model;
using Serilog;

namespace Omnae.BackgroundWorkers.Service
{
    public class EmailSenderService
    {
        internal const string QueueName = "emailsenderqueue";
        private const string ContainerName = "emailsinthequeue";
        private string AccountingMail { get; } = CloudConfigurationManager.GetSetting("ACCOUNTING_EMAIL");
        private string FromMail { get; } = CloudConfigurationManager.GetSetting("FROM_EMAIL");

        private ILogger Log { get; }
        private BlobContainerClient Container { get; }

        public EmailSenderService(ILogger log)
        {
            Log = log;

            Email.DefaultSender = new MailgunSender(
                CloudConfigurationManager.GetSetting("Mailgun.Domain"), // Mailgun Domain
                CloudConfigurationManager.GetSetting("Mailgun.APIKEY") // Mailgun API Key
            );

            var connectionString = CloudConfigurationManager.GetSetting("AzureWebJobsStorage");
            var blobClient = new BlobServiceClient(connectionString);

            Container = blobClient.GetBlobContainerClient(ContainerName);
            Container.CreateIfNotExists();
        }

        public async Task SyncAsync(EmailIdMessage messageId)
        {
            var id = messageId.EmailDataIdInTheBlob;
            var blob = Container.GetBlobClient($"emailData-{id}.json");
            var download = await blob.DownloadAsync();
            var dataReader = new StreamReader(download.Value.Content);
            var data = await dataReader.ReadToEndAsync();
            var emailData = JsonConvert.DeserializeObject<EmailMessage>(data);

            if (id != emailData.Id)
                throw new ApplicationException("Internal Error - Invalid ID in the email queue.");

            await SyncAsync(emailData);

            await blob.DeleteAsync();
        }

        private Task SyncAsync(EmailMessage message)
        {
            var email = Email
                .From(message.FromEmail ?? FromMail, message.FromName ?? "Omnae Team")
                .To(message.DestinationEmail, message.DestinationName ?? "Omnae User")
                .Subject(message.Subject)
                .Body(message.Body, true);

            if (message.ToEmails != null)
            {
                foreach (var toMail in message.ToEmails.Select(e => e.Trim()).Where(e => message.DestinationEmail.Equals(e, StringComparison.CurrentCultureIgnoreCase) == false))
                {
                    email = email.To(toMail);
                }
            }
            if (message.ToAccounting)
            {
                email = email.CC(AccountingMail);
            }

            if (message.AttachData != null && message.AttachData.Any())
            {
                for (var i = 0; i < message.AttachData.Count; i++)
                {
                    var fileData = message.AttachData[i];
                    var fileName = message.AttachName[i];
                    email = email.Attach(new Attachment { Data = new MemoryStream(fileData), Filename = fileName, IsInline = false });
                }
            }

            return SendEmailAsync(message.Subject, message.DestinationEmail, email);
        }

        private async Task SendEmailAsync(string subject, string destination, IFluentEmail email)
        {
            try
            {
                Log.Debug("Sending email...");
                var response = await email.SendAsync();

                if (response.Successful)
                {
                    Log.Information("Email send. Subject:{Subject}, To:{To}, Successful:{Successful}", subject, destination, response.Successful);
                }
                else
                {
                    foreach (var errorMessage in response.ErrorMessages)
                    {
                        Log.Error("Error sending a e-mail. Subject:{Subject}, To:{To} - Msg: {Msg}", subject, destination, errorMessage);
                    }

                    throw new ApplicationException($"Error sending a e-mail. {string.Join(";", response.ErrorMessages)}");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error sending a e-mail. Subject:{Subject}, To:{To}", subject, destination);
                throw;
            }
        }
    }
}