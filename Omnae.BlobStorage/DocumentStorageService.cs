using Azure.Storage.Sas;
using Serilog;
using System;
using System.IO;

namespace Omnae.BlobStorage
{
    public class DocumentStorageService : AzureBlobStorageServiceBase, IDocumentStorageService
    {
        public DocumentStorageService(ILogger log) : base(log, "StorageContainer")
        {
        }

        public virtual string AddSecurityTokenToUrl(string url, ExpireTokenInfo expireTokenInfo = ExpireTokenInfo.Hour)
        {
            if (expireTokenInfo == ExpireTokenInfo.None)
                return url;
            if (string.IsNullOrWhiteSpace(url))
                return url;

            var uri = new Uri(url);
            var fileName = Path.GetFileName(uri.AbsolutePath);
            fileName = fileName.Replace("%20", " ");
            var blockBlob = Container.GetBlobClient(fileName);
            var finalFileName = Uri.UnescapeDataString(blockBlob.Name);

            var expire = expireTokenInfo == ExpireTokenInfo.Hour ? DateTimeOffset.UtcNow.AddHours(1)  : DateTimeOffset.UtcNow.AddDays(4);

            var sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = blockBlob.BlobContainerName,
                BlobName = finalFileName,
                Resource = "b",
                Protocol = SasProtocol.Https,
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
                ExpiresOn = expire,
            };
            sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);

            var token = sasBuilder.ToSasQueryParameters(KeyCredential).ToString();

            var newUri = new UriBuilder(blockBlob.Uri) {Query = token};
            
            Log.Debug("Adding SAS Token to Doc URL. {docName}", blockBlob.Name);

            return newUri.ToString();
        }
    }

    public enum ExpireTokenInfo
    {
        None,
        Hour,
        FourDays,
    }
}