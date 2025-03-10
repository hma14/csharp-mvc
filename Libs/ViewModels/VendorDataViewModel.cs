using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Omnae.Common.Extensions;

namespace Omnae.Libs.ViewModels
{
    public class VendorDataViewModel : IValidatableObject
    {
        [Required]
        [MaxLength(150)]
        public string VendorName { get; set; }

        [Required]
        [MaxLength(500)]
        public string ContactFirstName { get; set; }

        [Required]
        [MaxLength(500)]
        public string ContactLastName { get; set; }

        [Required]
        [MaxLength(1024)]
        public string AddressLine1 { get; set; }
        [CanBeNull]
        [MaxLength(1024)]
        public string AddressLine2 { get; set; }
        [Required]
        [MaxLength(1024)]
        public string City { get; set; }
        [Required]
        [MaxLength(150)]
        public string Country { get; set; }
        [CanBeNull]
        [MaxLength(150)]
        public string StateOrProvince { get; set; }

        [CanBeNull]
        [MaxLength(32)]
        public string ZipCode { get; set; }
        [CanBeNull]
        [MaxLength(32)]
        public string PostalCode { get; set; }

        [Required, EmailAddress, MaxLength(1024)]
        public string Email { get; set; }

        public string OriginalPhoneEntry { get; set; }

        public string PhoneCountryCode { get; set; }

        [Required, DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\s*\+?([0-9\-\.\s\(\)]{3,22})\s*$", ErrorMessage = "Invalid Phone number. Use a international phone number format without format.")]
        public string Phone { get; set; }


        [Required, DataType(DataType.Currency), StringLength(9)]
        public string Currency { get; set; }

        public bool? IsTerm { get; set; }
        public string Address { get; set; }
        
        [RegularExpression(@"[+]?[0-9]+", ErrorMessage = "Not a valid Term Days number")]
        public string TermDays { get; set; }
        
        public string CreditLimit { get; set; }
        [RegularExpression(@"[+]?[0-9]+", ErrorMessage = "Not a valid Early Payment Discount Days number")]
        public string EarlyPaymentDiscountDays { get; set; }
        [RegularExpression(@"[+]?[0-9]+", ErrorMessage = "Not a valid Early Payment Discount Percentage number")]
        public string EarlyPaymentDiscountPercentage { get; set; }
        [RegularExpression(@"[+]?[0-9]+", ErrorMessage = "Not a valid Early Deposite Percentage number")]
        public string DepositePercentage { get; set; }
        [RegularExpression(@"[+]?[0-9]+", ErrorMessage = "Not a valid Tooling Deposite Percentage number")]
        public string ToolingDepositePercentage { get; set; }


        /////////////////////////////////

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(ZipCode?.ToNullIfEmpty() == null && PostalCode?.ToNullIfEmpty() == null && City != "Hong Kong")
                yield return new ValidationResult($"Invalid ZipCode or PostalCode: {Currency}");

            if (!NMoneys.Currency.TryGet(this.Currency, out var curr))
                yield return new ValidationResult($"Invalid Currency: {Currency}");

            if (!string.IsNullOrWhiteSpace(TermDays) && !(int.TryParse(TermDays, out var td) && td >= 0))
                yield return new ValidationResult($"Invalid TermDays: {TermDays}");
            if (!string.IsNullOrWhiteSpace(CreditLimit) && !(decimal.TryParse(CreditLimit, out var cl) && cl >= 0))
                yield return new ValidationResult($"Invalid CreditLimit: {CreditLimit}");
            if (!string.IsNullOrWhiteSpace(EarlyPaymentDiscountDays) && !(int.TryParse(EarlyPaymentDiscountDays, out var t1) && t1 >= 0))
                yield return new ValidationResult($"Invalid EarlyPaymentDiscountDays: {EarlyPaymentDiscountDays}");
            if (!string.IsNullOrWhiteSpace(EarlyPaymentDiscountPercentage) && !(int.TryParse(EarlyPaymentDiscountPercentage, out var t2) && t2 >= 0))
                yield return new ValidationResult($"Invalid EarlyPaymentDiscountPercentage: {EarlyPaymentDiscountPercentage}");
            if (!string.IsNullOrWhiteSpace(DepositePercentage) && !(int.TryParse(DepositePercentage, out var t3) && t3 >= 0))
                yield return new ValidationResult($"Invalid DepositePercentage: {DepositePercentage}");
            if (!string.IsNullOrWhiteSpace(ToolingDepositePercentage) && !(int.TryParse(ToolingDepositePercentage, out var t4) && t4 >= 0))
                yield return new ValidationResult($"Invalid ToolingDepositePercentage: {ToolingDepositePercentage}");

            yield break;
        }
    }
}