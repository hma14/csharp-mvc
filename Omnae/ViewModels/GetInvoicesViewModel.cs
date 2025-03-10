using Omnae.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.ViewModels
{
    public class GetInvoicesViewModel
    {     
        public decimal Balance { get; set; }
        public decimal Wip { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal AvailableCredit { get; set; }
        public List<OmnaeInvoiceViewModel> OpenInvoices { get; set; }
        public List<OmnaeInvoiceViewModel> ClosedInvoices { get; set; }
        public USER_TYPE UserType { get; set; }
        public int? Term { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentRefNumber { get; set; }

    }
}