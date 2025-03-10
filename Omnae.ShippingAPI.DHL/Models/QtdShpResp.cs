using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Omnae.ShippingAPI.DHL.Models
{
    public class QtdShpResp
    {
        [Key]
        public int Id { get; set; }
        public string GlobalProductCode { get; set; }
        public string LocalProductCode { get; set; }

        public string ProductShortName { get; set; }
        public string LocalProductName { get; set; }
        public string NetworkTypeCode { get; set; }

        public string POfferedCustAgreement { get; set; }
        public string TransInd { get; set; }
        public string PickupDate { get; set; }
        public string PickupCutoffTime { get; set; }
        public string BookingTime { get; set; }
        public string CurrencyCode { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? WeightCharge { get; set; }
        public decimal? WeightChargeTax { get; set; }
        public string TotalTransitDays { get; set; }
        public string PickupPostalLocAddDays { get; set; }
        public string DeliveryPostalLocAddDays { get; set; }
        public string DeliveryDate { get; set; }
        public string DeliveryTime { get; set; }
        public string DimensionalWeight { get; set; }
        public string WeightUnit { get; set; }
        public string PickupDayOfWeekNum { get; set; }
        public string DestinationDayOfWeekNum { get; set; }

        public List<QtdShpExChrg> QtdShpExChrgs { get; set; }

        public string PricingDate { get; set; }
        public decimal? ShippingCharge { get; set; }
        public decimal? TotalTaxAmount { get; set; }
        public List<QtdSInAdCur> QtdSInAdCurList { get; set; }

        public WeightChargeTaxDet WeightChargeTaxDet { get; set; }

    }
}
