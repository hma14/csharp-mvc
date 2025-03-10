using System.Collections.Generic;
using Omnae.Common;
using Omnae.Model.Models;

namespace Omnae.BusinessLayer.Models
{
    public class ProductFileViewModel
    {
        public int TaskId { get; set; }
        public int ProductId { get; set; }
        public States StateId { get; set; }
        public int? RFQBidId { get; set; }
        public List<Document> Doc2Ds { get; set; }
        public List<Document> Doc3Ds { get; set; }
        public List<Document> QuoteDocs { get; set; }
        public Document QuoteDocumentForRFQBid { get; set; }
        public List<Document> RevisingDocs { get; set; }
        public List<Document> ProofDocs { get; set; }
        public List<Document> VendorPODocs { get; set; }
        public List<Document> PackingDocs { get; set; }
        public List<Document> InspectionReportDocs { get; set; }
        public List<Document> CustInvoiceDocs { get; set; }
        public List<Document> PaymentProofDocs { get; set; }
        public USER_TYPE UserType { get; set; }
        
    }
}