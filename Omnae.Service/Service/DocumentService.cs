using Omnae.Data.Repositories.Interfaces;
using Omnae.Service.Service.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Omnae.BlobStorage;
using Omnae.Model.Models;
using Omnae.Common;

namespace Omnae.Service.Service
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly DocumentStorageService _documentStorage;

        public DocumentService(IDocumentRepository documentRepository, DocumentStorageService documentStorage)
        {
            _documentRepository = documentRepository;
            _documentStorage = documentStorage;
        }

        public int AddDocument(Document entity)
        {
            return _documentRepository.AddDocument(entity);
        }

        public void Dispose()
        {
            _documentRepository.Dispose();
        }

        public Document FindDocumentById(int id, ExpireTokenInfo expireTokenInfo = ExpireTokenInfo.Hour)
        {
            var doc =  _documentRepository.GetDocumentById(id);
            UpdateDocUrlWithSecurityToken(doc, expireTokenInfo);
            return doc;
        }

        public List<Document> FindDocumentByProductId(int productId, ExpireTokenInfo expireTokenInfo = ExpireTokenInfo.Hour)
        {
            var docs = _documentRepository.GetDocumentByProductId(productId);
            UpdateDocUrlWithSecurityToken(docs, expireTokenInfo);
            return docs;
        }

        public List<Document> FindDocumentByProductIdDocType(int productId, DOCUMENT_TYPE docType, ExpireTokenInfo expireTokenInfo = ExpireTokenInfo.Hour)
        {
            var docs = _documentRepository.GetDocumentByProductIdDocType(productId, docType);
            UpdateDocUrlWithSecurityToken(docs, expireTokenInfo);
            return docs;
        }

        public List<Document> FindDocumentByTaskIdDocType(int taskId, DOCUMENT_TYPE docType, ExpireTokenInfo expireTokenInfo = ExpireTokenInfo.Hour)
        {
            var docs = _documentRepository.GetDocumentByTaskIdDocType(taskId, docType);
            UpdateDocUrlWithSecurityToken(docs, expireTokenInfo);
            return docs;
        }

        public List<Document> FindDocumentByTaskId(int taskId, ExpireTokenInfo expireTokenInfo = ExpireTokenInfo.Hour)
        {
            var docs = _documentRepository.GetDocumentsByTaskId(taskId).ToList();
            UpdateDocUrlWithSecurityToken(docs, expireTokenInfo);
            return docs;
        }

        public List<Document> FindDocumentList(ExpireTokenInfo expireTokenInfo = ExpireTokenInfo.Hour)
        {
            var docs = _documentRepository.GetDocumentList();
            UpdateDocUrlWithSecurityToken(docs, expireTokenInfo);
            return docs;
        }

        public void RemoveDocument(Document entity)
        {
            _documentRepository.RemoveDocument(entity);
        }

        public void UpdateDocument(Document entity)
        {
            _documentRepository.UpdateDocument(entity);
        }

        public IQueryable<Document> FindDocuments()
        {
            return _documentRepository.GetDocuments();
        }

        public IQueryable<Document> FindDocumentsByCompanyId(int companyId)
        {
            return _documentRepository.GetDocumentsByCompanyId(companyId);
        }

        public IQueryable<Document> FindDocumentsByVendorId(int vendorId)
        {
            return _documentRepository.GetDocumentsByVendorId(vendorId);
        }

        public IQueryable<Document> FindDocumentsByProductId(int productId)
        {
            return _documentRepository.GetDocumentsByProductId(productId);
        }

        public IQueryable<Document> FindDocumentsByTaskId(int taskId)
        {
            return _documentRepository.GetDocumentsByTaskId(taskId);
        }
        
        public void UpdateDocUrlWithSecurityToken(Document doc, ExpireTokenInfo expireTokenInfo = ExpireTokenInfo.Hour)
        {
            if (doc == null)
                return;

            var newUrl = _documentStorage.AddSecurityTokenToUrl(doc.DocUri, expireTokenInfo);
            doc.DocUri = newUrl;
        }

        public void UpdateDocUrlWithSecurityToken(IEnumerable<Document> docs, ExpireTokenInfo expireTokenInfo = ExpireTokenInfo.Hour)
        {
            if (docs == null)
                return;

            foreach (var doc in docs)
            {
                if (doc == null)
                    continue;

                UpdateDocUrlWithSecurityToken(doc, expireTokenInfo);
            }
        }
    }

    public static class DocumentServiceEx
    {
        public static T UpdateDocUrlWithSecurityToken<T>(this T docs, IDocumentService documentService, ExpireTokenInfo expireTokenInfo = ExpireTokenInfo.Hour)
            where T : IEnumerable<Document>
        {
            if (documentService == null)
                return docs;
            if (docs == null)
                return default(T);

            documentService.UpdateDocUrlWithSecurityToken(docs, expireTokenInfo);
            return docs;
        }
    }
}
