using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.QuickBooks.ViewModels
{
    public class InvoiceViewModel : QboBaseViewModel
    {
           
        public decimal VendorUnitPrice { get; set; }
   
        
        public string Attention { get; set; }
        public DateTime ShipDate { get; set; }

        public string CompanyName { get; set; }
      
        public int? CustomerAddressId { get; set; }

        public int? VendorAddressId { get; set; }

        public string EstimateId { get; set; }
        public string EstimateNumber { get; set; }
        
        public string InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public string PONumber { get; set; }
        public string Buyer { get; set; }
        public int NumberSampleIncluded { get; set; }
    }
}
