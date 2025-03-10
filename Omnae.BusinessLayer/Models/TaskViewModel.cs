using System;
using System.Collections.Generic;
using Omnae.Common;
using Omnae.Model.Models;

namespace Omnae.BusinessLayer.Models
{
    public class TaskViewModel
    {
        public USER_TYPE UserType { get; set; }
        public TaskData TaskData { get; set; }
        public Order Order { get; set; }
        public string VendorName { get; set; }
        public ProductDetailsViewModel ProductDetailsVM { get; set; }
        public ProductFileViewModel ProductFileVM { get; set; }
        public NcrDescriptionViewModel NcrDescriptionVM { get; set; }
        public string EnumName { get; set; }
        public Func<bool> MyFunc { get; set; }
        public decimal Quantity { get; set; }
        public decimal? VendorUnitPrice { get; set; }
        public decimal? ProductUnitPrice { get; set; }
        public string VendorPONumber { get; set; }
        public string VendorPODocUri { get; set; }  //TODO Check if we will need to add a Token Here. ***************************
        public bool? ChkPreconditions { get; set; }
        public string ChangeRevisionReason { get; set; }
        public RFQViewModel RFQVM { get; set; }
        public RFQViewModel RFQViewModel { get; set; }
        public string BidFailedReason { get; set; }
        public List<Document> ProofDocs { get; set; }
        public List<Document> SampleRejectDocs { get; set; }
        public List<Document> RevisingDocs { get; set; }
        public VendorInvoiceViewModel VendorInvoiceVM { get; set; }
        public NCReport NCReport { get; set; }
        public List<Document> Docs { get; set; }
        public string  CarrierFromOrder { get; set; }
        public bool IsForSearch { get; set; }
    }
}