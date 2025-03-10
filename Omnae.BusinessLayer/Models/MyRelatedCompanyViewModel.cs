using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.BusinessLayer.Models
{
    public class MyRelatedCompanyViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        
        public RelatedCompanyAddress Address { get; set; }
        public CreditRelationship CreditRelationship { get; set; }
    }

    public class RelatedCompanyAddress
    {
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }

        public string PostalCode { get; set; }
    }

    public class CreditRelationship
    {
        public int CustomerId { get; set; }
        public int VendorId { get; set; }
        public bool isTerm { get; set; }
        public int? TermDays { get; set; }
        public decimal? CreditLimit { get; set; }
        public int? DiscountDays { get; set; }
        public float? Discount { get; set; }
        public int? Deposit { get; set; }
        public int? ToolingDepositPercentage { get; set; }
    }
}
