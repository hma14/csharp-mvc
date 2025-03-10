using System.IO;
using System.Text;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;
using Microsoft.Azure;
using Newtonsoft.Json;
using Omnae.Notification.Model;
using Serilog;

namespace Omnae.Notification
{
    public class BackgroundEmailSender : IEmailSender
    {
        private QueueClient QueueClient { get; }
        private BlobContainerClient Container { get; }

        private const string QueueName = "emailsenderqueue";
        private const string ContainerName = "emailsinthequeue";

        public ILogger Log { get; }

        public BackgroundEmailSender(ILogger log)
        {
            Log = log;
            var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");

            QueueClient = new QueueClient(connectionString, QueueName);
            QueueClient.CreateIfNotExists();

            var blobClient = new BlobServiceClient(connectionString);

            Container = blobClient.GetBlobContainerClient(ContainerName);
            Container.CreateIfNotExists();
        }

        public void SendEmail(EmailMessage message)
        {
            var id = message.Id;
            var data = JsonConvert.SerializeObject(message); 
            Container.UploadBlob($"emailData-{id}.json", new MemoryStream(Encoding.UTF8.GetBytes(data)));
            
            var msgData = JsonConvert.SerializeObject(new EmailIdMessage(message));
            QueueClient.SendMessage(Base64Encode(msgData));
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public void SendNormalEmail(string sender, string subject, string from, string destination, string content)
        {
            SendEmail(new EmailMessage()
            {
                FromEmail = @from,
                FromName = sender,
                DestinationEmail = destination, 
                DestinationName = "Dear Omnae Team",
                Subject = subject + " - sent by " + sender,
                Body = content,
            });
        }
    }
}
