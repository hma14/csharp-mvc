using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.QuickBooks.ViewModels
{
    public class StripeInvoiceViewModel
    {
        public string InvoiceId { get; set; }
        public string StripeCustomerId { get; set; }
        public string QboId { get; set; }
        public string CompanyName { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? DueDate { get; set; }

        public string PhoneNumber { get; set; }

        public string EmailAddress { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string CountryName { get; set; }

        public string ZipCode { get; set; }

        public string PostalCode { get; set; }

        public string InvoiceNumber { get; set; }

        public int Quantity { get; set; }
        public long Total { get; set; }

        public DateTime OrderDate { get; set; }


        public string PONumber { get; set; }

        public string Buyer { get; set; }
        public string HostedInvoiceUrl { get; set; }
        public string InvoicePdf { get; set; }
        public string InvoiceStatus { get; set; }

    }
}
