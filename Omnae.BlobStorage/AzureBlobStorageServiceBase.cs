using System;
using System.IO;
using System.Web;
using Azure.Storage;
using Microsoft.Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Serilog;

namespace Omnae.BlobStorage
{
    public abstract class AzureBlobStorageServiceBase
    {
        public readonly string ContainerName;

        protected BlobContainerClient Container { get; }
        protected StorageSharedKeyCredential KeyCredential { get; }
        protected string AccountName { get; }

        protected ILogger Log { get; }

        protected AzureBlobStorageServiceBase(ILogger log, string configNameContainerName)
        {
            Log = log;
            ContainerName = CloudConfigurationManager.GetSetting(configNameContainerName);
            var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
            var storageAccountKey = CloudConfigurationManager.GetSetting("StorageAccountKey");

            var blobClient = new BlobServiceClient(connectionString);

            AccountName = blobClient.AccountName;
            KeyCredential = new StorageSharedKeyCredential(AccountName, storageAccountKey);

            Container = blobClient.GetBlobContainerClient(ContainerName);
            Container.CreateIfNotExists();
        }
        
        public virtual void Delete(string docName)
        {
            Log.Information("Blob, Deleting... {AccountName} {DocName}", AccountName, docName);

            try
            {
                docName = HttpUtility.UrlDecode(docName);

                var blockBlob = Container.GetBlobClient(docName);
                blockBlob.DeleteIfExistsAsync().Wait();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error Deleting file in BlobStorage. {AccountName}/{DocName}", AccountName, docName);
                throw;
            }
        }

        public virtual string Download(string docName)
        {
            Log.Information("Blob, Download.... {AccountName} {DocName}", AccountName, docName);

            try
            {
                docName = Path.GetFileName(docName);
                docName = HttpUtility.UrlDecode(docName);

                var blockBlob = Container.GetBlobClient(docName);

                var sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = blockBlob.BlobContainerName,
                    BlobName = blockBlob.Name,
                    Resource = "b",
                    Protocol = SasProtocol.Https,
                    StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
                    ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(10),
                };
                sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);

                var token = sasBuilder.ToSasQueryParameters(KeyCredential).ToString();
                var newUri = new UriBuilder(blockBlob.Uri) { Query = token };
                return newUri.ToString();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error Downloading file in BlobStorage. {AccountName}/{DocName}", AccountName, docName);
                throw;
            }
        }

        public virtual string Upload(HttpPostedFileBase file, string fileNewName)
        {
            Log.Information("Blob, Upload (HttpPostedFileBase)... {AccountName} {FileNewName}", AccountName, fileNewName);

            try
            {
                var blockBlob = Container.GetBlobClient(fileNewName);
                if (blockBlob.Exists())
                {
                    Log.Warning("A blob file exits with the same name. Deleting... {AccountName} {FileNewName}", AccountName, fileNewName);
                    blockBlob.DeleteIfExists(DeleteSnapshotsOption.IncludeSnapshots);
                }

                blockBlob.Upload(file.InputStream);
                blockBlob.SetHttpHeaders(new BlobHttpHeaders() { ContentType = file.ContentType });

                return blockBlob.Uri.AbsoluteUri;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error Uploading (HttpPostedFileBase) file in BlobStorage. {AccountName}/{fileNewName}", AccountName, fileNewName);
                throw;
            }
        }

        public virtual string Upload(byte[] data, string targetFileName)
        {
            Log.Information("Blob, Upload (byte[])... {AccountName} {FileNewName}", AccountName, targetFileName);

            var blockBlob = Container.GetBlobClient(targetFileName);
            if (blockBlob.Exists())
            {
                Log.Warning("A blob file exits with the same name. Deleting... {AccountName} {FileNewName}", AccountName, targetFileName);
                blockBlob.DeleteIfExists(DeleteSnapshotsOption.IncludeSnapshots);
            }

            try
            {
                using (var memoryStream = new MemoryStream(data))
                {
                    blockBlob.Upload(memoryStream);
                    blockBlob.SetHttpHeaders(new BlobHttpHeaders() { ContentType = "application/pdf" });
                }
                return blockBlob.Uri.AbsoluteUri;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error Uploading (byte[]) file in BlobStorage. {AccountName}/{fileNewName}", AccountName, targetFileName);
                return ex.Message.ToString(); //BUG
            }
        }

        public virtual string Upload(MemoryStream memoryStream, string targetFileName)
        {
            Log.Information("Blob, Upload (MemoryStream)... {AccountName} {FileNewName}", AccountName, targetFileName);

            var blockBlob = Container.GetBlobClient(targetFileName);
            if (blockBlob.Exists())
            {
                Log.Warning("A blob file exits with the same name. Deleting... {AccountName} {FileNewName}", AccountName, targetFileName);
                blockBlob.DeleteIfExists(DeleteSnapshotsOption.IncludeSnapshots);
            }

            try
            {
                blockBlob.Upload(memoryStream);
                blockBlob.SetHttpHeaders(new BlobHttpHeaders() { ContentType = "application/pdf" });

                return blockBlob.Uri.AbsoluteUri;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error Uploading (MemoryStream) file in BlobStorage. {AccountName}/{fileNewName}", AccountName, targetFileName);
                return ex.Message.ToString(); //BUG
            }
        }
    }
}