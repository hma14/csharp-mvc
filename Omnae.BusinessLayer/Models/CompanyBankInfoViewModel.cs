using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Omnae.BusinessLayer.Models
{
   
    public class CompanyBankInfoViewModel
    {
        [Required]
        public string BankName { get; set; }

        // Bank address

        [Required]
        public int CountryId { get; set; }

        //public int? StateOrProvinceId { get; set; }
        public string StateOrProvinceName { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string ZipCode { get; set; }

        public string PostalCode { get; set; }

        // Bank account info
        [Required]
        public string TransitNumber { get; set; }

        [Required]
        public string InstitutionNumber { get; set; }

        [Required]
        public string AccountNumber { get; set; }

        public string BeneficiaryBankSwiftNumber { get; set; }

        public string IntermediaryBank { get; set; }

        public string IntermediaryBankSwiftNumber { get; set; }
    }
}