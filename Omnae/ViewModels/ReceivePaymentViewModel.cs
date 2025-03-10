using Omnae.Common;
using System.Collections.Generic;

namespace Omnae.ViewModels
{
    public class ReceivePaymentViewModel
    {
        public List<OmnaeInvoiceViewModel> OpenInvoices { get; set; }
        public List<OmnaeInvoiceViewModel> ClosedInvoices { get; set; }

    }
}