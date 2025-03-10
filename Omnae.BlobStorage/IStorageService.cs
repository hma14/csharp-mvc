using System.IO;
using System.Web;

namespace Omnae.BlobStorage
{
    public interface IStorageService
    {
        string Upload(HttpPostedFileBase file, string targetFileName);
        string Upload(byte[] data, string targetFileName);
        string Upload(MemoryStream memoryStream, string targetFileName);
        string Download(string docToUpload);
        void Delete(string docName);
    }

    public interface IDocumentStorageService : IStorageService
    {
        string AddSecurityTokenToUrl(string url, ExpireTokenInfo expireTokenInfo = ExpireTokenInfo.Hour);
    }

    public interface IImageStorageService : IStorageService
    {
    }

}
