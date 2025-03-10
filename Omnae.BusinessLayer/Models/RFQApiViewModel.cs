using System;
using System.ComponentModel.DataAnnotations;
using Humanizer;
using Omnae.Common;
using Omnae.Model.Models;

namespace Omnae.BusinessLayer.Models
{
    public class RFQApiViewModel
    {
        public int? TaskId { get; set; }
        public int? SampleLeadTime { get; set; }
        public int? ProductionLeadTime { get; set; }
        public int? NumberSampleIncluded { get; set; }
        public decimal ToolingSetupCharges { get; set; }
        public string HarmonizedCode { get; set; }

        //[Display(Name = "Quote Doc")]
        //public Document QuoteDoc { get; set; }

        //public PriceBreakViewModel PriceBreakVM { get; set; }

        //public ShippingQuoteRequestViewModel ShippingQuoteVM { get; set; }

        public bool isEnterprise { get; set; }
        public decimal Qty1 { get; set; }
        public decimal? Qty2 { get; set; }
        public decimal? Qty3 { get; set; }
        public decimal? Qty4 { get; set; }
        public decimal? Qty5 { get; set; }
        public decimal? Qty6 { get; set; }
        public decimal? Qty7 { get; set; }
        public MEASUREMENT_UNITS UnitOfMeasurement { get; set; }

        public decimal UnitPrice1 { get; set; }
        public decimal? UnitPrice2 { get; set; }
        public decimal? UnitPrice3 { get; set; }
        public decimal? UnitPrice4 { get; set; }
        public decimal? UnitPrice5 { get; set; }
        public decimal? UnitPrice6 { get; set; }
        public decimal? UnitPrice7 { get; set; }

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


        // nested properties
        public string DimensionUnit { get; set; }

        public string WeightUnit { get; set; }

        public int NumberPieces { get; set; }

        public int NumberPartsPerPiece { get; set; }

        public float Height { get; set; }
        public float Depth { get; set; }
        public float Width { get; set; }
        public float Weight { get; set; }
    }
}