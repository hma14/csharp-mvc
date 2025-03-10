using Omnae.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Omnae.ViewModels
{
    public class BillingAddressViewModel
    {
        [Required]
        // Address
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }

        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }

        public string City { get; set; }

        [Display(Name = "Country")]
        public int CountryId { get; set; }

        [Display(Name = "State/Province")]
        public int? StateProvinceId { get; set; }

        [Display(Name = "Zip Code")]
        [RegularExpression(@"^\d{5}(?:[-\s]\d{4})?$", ErrorMessage = "Incorrect ZipCod")]
        public string ZipCode { get; set; }

        [Display(Name = "Postal Code")]
        [RegularExpression(@"^[A-Za-z]\d[A-Za-z][\s]?\d[A-Za-z]\d$", ErrorMessage = "Incorrect Postal Code")]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; }
    }
}