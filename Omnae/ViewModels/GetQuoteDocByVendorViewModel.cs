using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Omnae.ViewModels
{
    public class GetQuoteDocByVendorViewModel
    {
        public int CustomerId { get; set; }
        public int VendorId { get; set; }
        public int ProductId { get; set; }
        public List<Document> QuoteDocs { get; set; }
        public SelectList Customers { get; set; }
        public SelectList ProductList { get; set; }
    }
}