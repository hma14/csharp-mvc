using Omnae.Model.Models;
using Omnae.Data.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omnae.Common;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface IDocumentRepository : IRepository<Document>
    {
        Document GetDocumentById(int Id);
        List<Document> GetDocumentByProductId(int productId);
        List<Document> GetDocumentList();
        int AddDocument(Document entity);
        void UpdateDocument(Document entity);
        void RemoveDocument(Document entity);

        void Dispose();

        List<Document> GetDocumentByTaskIdDocType(int taskId, DOCUMENT_TYPE docType);
        List<Document> GetDocumentByProductIdDocType(int productId, DOCUMENT_TYPE docType);
        IQueryable<Document> GetDocuments();
        IQueryable<Document> GetDocumentsByCompanyId(int companyId);
        IQueryable<Document> GetDocumentsByProductId(int productId);
        IQueryable<Document> GetDocumentsByTaskId(int taskId);
        IQueryable<Document> GetDocumentsByVendorId(int companyId);
    }
}
