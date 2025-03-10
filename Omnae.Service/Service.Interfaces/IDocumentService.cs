using Omnae.Common;
using Omnae.Model.Models;
using System.Collections.Generic;
using System.Linq;
using Omnae.BlobStorage;

namespace Omnae.Service.Service.Interfaces
{
    public interface IDocumentService
    {
        Document FindDocumentById(int id, ExpireTokenInfo expireTokenInfo = ExpireTokenInfo.Hour);
        List<Document> FindDocumentByProductId(int productId, ExpireTokenInfo expireTokenInfo = ExpireTokenInfo.Hour);
        List<Document> FindDocumentList(ExpireTokenInfo expireTokenInfo = ExpireTokenInfo.Hour);
        int AddDocument(Document entity);
        void UpdateDocument(Document entity);
        void RemoveDocument(Document entity);
        void Dispose();

        List<Document> FindDocumentByTaskIdDocType(int taskId, DOCUMENT_TYPE docType, ExpireTokenInfo expireTokenInfo = ExpireTokenInfo.Hour);
        List<Document> FindDocumentByTaskId(int taskId, ExpireTokenInfo expireTokenInfo = ExpireTokenInfo.Hour);
        List<Document> FindDocumentByProductIdDocType(int productId, DOCUMENT_TYPE docType, ExpireTokenInfo expireTokenInfo = ExpireTokenInfo.Hour);

        IQueryable<Document> FindDocuments();
        IQueryable<Document> FindDocumentsByCompanyId(int companyId);
        IQueryable<Document> FindDocumentsByProductId(int productId);
        IQueryable<Document> FindDocumentsByTaskId(int taskId);
        IQueryable<Document> FindDocumentsByVendorId(int vendorId);

        void UpdateDocUrlWithSecurityToken(Document doc, ExpireTokenInfo expireTokenInfo = ExpireTokenInfo.Hour);
        void UpdateDocUrlWithSecurityToken(IEnumerable<Document> docs, ExpireTokenInfo expireTokenInfo = ExpireTokenInfo.Hour);
    }
}
