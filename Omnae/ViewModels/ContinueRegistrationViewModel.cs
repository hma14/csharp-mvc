using Omnae.Common;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Omnae.ViewModels
{
    public class ContinueRegistrationViewModel
    {
       
        // Company
        [Required]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        // Address
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }

        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }

        public string City { get; set; }

        [Display(Name = "Country Id")]
        public int CountryId { get; set; }

        [Display(Name = "State/Province")]
        public int? StateId { get; set; }
        public int? ProvinceId { get; set; }

        [Display(Name = "Zip Code")]
        [RegularExpression(@"^\d{5}(?:[-\s]\d{4})?$", ErrorMessage = "Incorrect ZipCod")]
        public string ZipCode { get; set; }

        [Display(Name = "Postal Code")]
        [RegularExpression(@"^[A-Za-z]\d[A-Za-z][ -]?\d[A-Za-z]\d$", ErrorMessage = "Incorrect Postal Code")]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; }

        [Display(Name = "Area Code (Non-US, Non-Canada)")]
        public string AreaCode { get; set; }


        [Display(Name = "Is Address for Billing")]
        public bool IsBilling { get; set; }

        [Display(Name = "Is Address for Shipping")]
        public bool IsShipping { get; set; }

        public USER_TYPE UserType { get; set; } = USER_TYPE.Customer;

        public string Attention { get; set; }


        // Billing Address
        [Display(Name = "Address Line 1")]
        public string Bill_AddressLine1 { get; set; }

        [Display(Name = "Address Line 2")]
        public string Bill_AddressLine2 { get; set; }

        public string Bill_City { get; set; }

        [Display(Name = "Country Id")]
        public int Bill_CountryId { get; set; }

        [Display(Name = "State/Province")]
        public int? Bill_StateId { get; set; }
        public int? Bill_ProvinceId { get; set; }

        [Display(Name = "Zip Code")]
        [RegularExpression(@"^\d{5}(?:[-\s]\d{4})?$", ErrorMessage = "Incorrect ZipCod")]
        public string Bill_ZipCode { get; set; }

        [Display(Name = "Postal Code")]
        [RegularExpression(@"^[A-Za-z]\d[A-Za-z][ -]?\d[A-Za-z]\d$", ErrorMessage = "Incorrect Postal Code")]
        [DataType(DataType.PostalCode)]
        public string Bill_PostalCode { get; set; }

        [Display(Name = "Area Code (Non-US, Non-Canada)")]
        public string Bill_AreaCode { get; set; }

        // Billing
        public string Bill_Attention { get; set; }


        // Shipping Address
        [Display(Name = "Address Line 1")]
        public string Shipping_AddressLine1 { get; set; }

        [Display(Name = "Address Line 2")]
        public string Shipping_AddressLine2 { get; set; }

        public string Shipping_City { get; set; }

        [Display(Name = "Country Id")]
        public int Shipping_CountryId { get; set; }

        [Display(Name = "State/Province")]
        public int? Shipping_StateId { get; set; }
        public int? Shipping_ProvinceId { get; set; }

        [Display(Name = "Zip Code")]
        [RegularExpression(@"^\d{5}(?:[-\s]\d{4})?$", ErrorMessage = "Incorrect ZipCod")]
        public string Shipping_ZipCode { get; set; }

        [Display(Name = "Postal Code")]
        [RegularExpression(@"^[A-Za-z]\d[A-Za-z][ -]?\d[A-Za-z]\d$", ErrorMessage = "Incorrect Postal Code")]
        [DataType(DataType.PostalCode)]
        public string Shipping_PostalCode { get; set; }

        [Display(Name = "Area Code (Non-US, Non-Canada)")]
        public string Shipping_AreaCode { get; set; }

        // Shipping
        public string Shipping_Attention { get; set; }

        public HttpPostedFileBase CompanyLogo { get; set; }

        public bool isEnterprise { get; set; } = false;
    }
}