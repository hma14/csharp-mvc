using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Omnae.QuickBooks.ViewModels
{
    public class BillViewModel : QboBaseViewModel
    {
        public Stream FStreamForTooling { get; set; }
        public Stream FStream { get; set; }
        //public string PurchaseOrderId { get; set; }
        public string BillId { get; set; }
        public string BillNumber { get; set; }
        public int OrderId { get; set; }
        public bool isEnterprise { get; set; }
        public HttpPostedFileBase AttachInvoice { get; set; }
        public int NumberSampleIncluded { get; set; }
    }

}
