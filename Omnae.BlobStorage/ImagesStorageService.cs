using Microsoft.Azure;
using Serilog;

namespace Omnae.BlobStorage
{
    public class ImagesStorageService : AzureBlobStorageServiceBase, IImageStorageService
    {
        public ImagesStorageService(ILogger log) : base(log, "ImageStorageContainer")
        {
        }
    }
}