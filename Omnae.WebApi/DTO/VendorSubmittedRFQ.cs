using Humanizer;
using Omnae.BusinessLayer.Models;
using Omnae.Common;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.WebApi.DTO
{
    public class VendorSubmittedRFQ
    {
        public int TaskId { get; set; }
        public int VendorId { get; set; }
        public States State { get; set; }

        public int SampleLeadTime { get; set; }

        public int ProductionLeadTime { get; set; }

        public int NumberSampleIncluded { get; set; }

        public string HarmonizedCode { get; set; }

        public List<string> QuoteDocUri { get; set; }

        public IEnumerable<PriceBreakDTO> PriceBreaks { get; set; }

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


    }
}