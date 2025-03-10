using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.BusinessLayer.Models
{
    public class PurchaseViewModel : TransactionViewModel
    {
        public string VendorName { get; set; }
        public string EstimateNumber { get; set; }
        public decimal Total1 { get; set; }

        
    }
}