using Omnae.Model.Models;
using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omnae.Common;

namespace Omnae.Data.Repositories
{
    public class DocumentRepository : RepositoryBase<Document>, IDocumentRepository
    {
        public DocumentRepository(OmnaeContext dbContext) : base(dbContext)
        {

        }
        public int AddDocument(Document entity)
        {
            base.Add(entity);
            this.DbContext.Commit();
            return entity.Id;
        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        public Document GetDocumentById(int Id)
        {
            return this.DbContext.Documents.Where(x => x.Id == Id).FirstOrDefault();
        }

        public List<Document> GetDocumentByProductId(int productId)
        {
            return this.DbContext.Documents.Where(x => x.ProductId == productId).OrderBy(x => x.ModifiedUtc).ToList();
        }

        public List<Document> GetDocumentByProductIdDocType(int productId, DOCUMENT_TYPE docType)
        {
            return this.DbContext.Documents.Where(x => x.ProductId == productId && x.DocType == (int)docType).ToList();
        }
        public List<Document> GetDocumentByTaskIdDocType(int taskId, DOCUMENT_TYPE docType)
        {
            return this.DbContext.Documents.Where(p => p.TaskId == taskId && p.DocType == (int)docType).ToList();
        }

        public List<Document> GetDocumentList()
        {
            return this.DbContext.Documents.ToList();
        }

        public void RemoveDocument(Document entity)
        {
            var entry = this.DbContext.Documents.Where(x => x.Id == entity.Id).FirstOrDefault();
            if (entry != null)
            {
                base.Delete(entry);
                this.DbContext.Commit();
            }
        }

        public void UpdateDocument(Document entity)
        {
            base.Update(entity);
            this.DbContext.Commit();
        }

        public IQueryable<Document> GetDocuments()
        {
            return this.DbContext.Documents;
        }

        public IQueryable<Document> GetDocumentsByCompanyId(int companyId)
        {
            return this.DbContext.Documents
                .Include("Product")
                .Include("TaskData")
                .Where(d => d.Product.CustomerId == companyId ||
                            d.Product.VendorId == companyId ||
                            d.TaskData.RFQBid.VendorId == companyId ||
                            d.TaskData.RFQBidId == null);
        }

        public IQueryable<Document> GetDocumentsByVendorId(int companyId)
        {
            return this.DbContext.Documents
                .Include("Product")
                .Include("TaskData")
                .Where(d => d.Product.VendorId == companyId || d.TaskData.RFQBid.VendorId == companyId)
                .Where(d => d.DocType != (int)DOCUMENT_TYPE.QUOTE_PDF || (d.DocType == (int)DOCUMENT_TYPE.QUOTE_PDF && d.TaskData.RFQBid.VendorId == companyId))
                .Where(d => d.DocType != (int)DOCUMENT_TYPE.REVISING_DOCS || (d.DocType == (int)DOCUMENT_TYPE.REVISING_DOCS && d.TaskData.RFQBid.VendorId == companyId));
        }

        public IQueryable<Document> GetDocumentsByProductId(int productId)
        {
            return this.DbContext.Documents.Where(x => x.ProductId == productId);
        }

        public IQueryable<Document> GetDocumentsByTaskId(int taskId)
        {
            return this.DbContext.Documents.Where(x => x.TaskId == taskId);
        }
    }
}
