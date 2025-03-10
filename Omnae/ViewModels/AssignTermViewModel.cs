using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Omnae.ViewModels
{
    public class AssignTermViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Cust. Payment Term (days)")]
        public int? Term { get; set; }

        [Display(Name = "Pay to Vendor Term (days)")]
        public int? VendorTerm { get; set; }

        [Display(Name = "Credit Limit ($)")]
        [DataType(DataType.Currency)]
        public decimal? CreditLimit { get; set; }

    }
}