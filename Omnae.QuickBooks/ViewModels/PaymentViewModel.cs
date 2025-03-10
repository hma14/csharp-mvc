using Intuit.Ipp.Data;
using Omnae.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.QuickBooks.ViewModels
{
    public class PaymentViewModel 
    {
        public string QboInvoiceId { get; set; }    
        public decimal UnappliedAmt { get; set; }
        public decimal TotalAmt { get; set; }
        public string QboId { get; set; }
        public string CustomerName { get; set; }
        public string PaymentRefNum { get; set; }
        public PaymentTypeEnum PaymentType { get; set; }

    }
}
