using System;
using System.ComponentModel.DataAnnotations;
using Humanizer;
using Omnae.Common;
using Omnae.Model.Models;

namespace Omnae.BusinessLayer.Models
{
    public class RFQViewModel
    {
        [Key]
        public int Id { get; set; }
        public int? TaskId { get; set; }

        //[Required(ErrorMessage = "Sample Lead Time is required")]
        [Display(Name = "Sample Lead Time (in days)")]
        public int? SampleLeadTime { get; set; }

        //[Required(ErrorMessage = "Production Lead Time is required")]
        [Display(Name = "Production Lead Time (in days)")]
        public int? ProductionLeadTime { get; set; }

        [Display(Name = "Number Sample Included")]
        public int? NumberSampleIncluded { get; set; }

        [Display(Name = "Tooling Setup Charges")]
        //[RegularExpression(@"^[1-9][0-9]*[\.]?[0-9][0-9]*$", ErrorMessage = "Tooling Setup Charges value is invalid")]
        [RegularExpression(@"^[.][0-9]+$|[0-9]*[.]*[0-9]+$", ErrorMessage = "Tooling Setup Charges value is invalid")]
        [DataType(DataType.Currency)]
        public decimal ToolingSetupCharges { get; set; }

        [Required(ErrorMessage = "Harmonized Code is required")]
        [Display(Name = "Harmonized Code")]        
        public string HarmonizedCode { get; set; }

        [Display(Name = "Quote Doc")]
        public Document QuoteDoc { get; set; }

        public PriceBreakViewModel PriceBreakVM { get; set; }

        public ShippingQuoteRequestViewModel ShippingQuoteVM { get; set; }
        public bool isEnterprise { get; set; }
        /// <summary>
        /// The CurrencyCodes ISO Number
        /// </summary>
        public CurrencyCodes? PreferredCurrency { get; set; }

        /// <summary>
        /// The CurrencyCodes ISO 3 letter Code
        /// </summary>
        public string PreferredCurrencyText
        {
            get => PreferredCurrency?.Humanize();
            set => PreferredCurrency = (CurrencyCodes?)(string.IsNullOrEmpty(value) ? null : Enum.Parse(typeof(CurrencyCodes), value, true));
        }

        public DateTime ExpireDate { get; set; } = DateTime.UtcNow.AddMonths(12);
    }
}