using Omnae.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Humanizer;

namespace Omnae.BusinessLayer.Models
{
    public class CompaniesCreditRelationshipViewModel : IValidatableObject
    {
        [Display(Name = "Customer ID")]
        public int CustomerId { get; set; }

        [Display(Name = "Vendor ID")]
        public int VendorId { get; set; }

        [Display(Name = "Is a Term?")]
        public bool isTerm { get; set; }

        [Display(Name = "Term days")]
        public int TermDays { get; set; }

        [Display(Name = "Credit limit")]
        [DataType(DataType.Currency)]
        public decimal? CreditLimit { get; set; }

        [Display(Name = "Tax (%)")]
        [Required]
        [Range(0, int.MaxValue)]
        public int TaxPercentage { get; set; } = 0;

        /// <summary>
        /// The CurrencyCodes ISO Number
        /// </summary>
        public CurrencyCodes? Currency { get; set; }

        /// <summary>
        /// The CurrencyCodes ISO 3 letter Code
        /// </summary>
        public string CurrencyText
        {
            get => Currency.Humanize();
            set => Currency = (CurrencyCodes?) (string.IsNullOrEmpty(value) ? null : Enum.Parse(typeof(CurrencyCodes), value, true));
        }
        
        [Display(Name = "Vendor's early payment discount in days")]
        public int? DiscountDays { get; set; }

        [Display(Name = "Vendor's early payment discount in %")]
        public float? Discount { get; set; }

        [Display(Name = "Deposits in % if required")]
        public int? Deposit { get; set; }

        [Display(Name = "Tooling deposit %")]
        public int? ToolingDepositPercentage { get; set; }

        public SelectList Customers { get; set; }
        public SelectList Vendors { get; set; }
        public SelectList Currencies { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Currency == null)
                yield return new ValidationResult($"{nameof(Currency)} or {nameof(CurrencyText)} must be defined.", new[] { nameof(Currency), nameof(CurrencyText) });
        }
    }
}
