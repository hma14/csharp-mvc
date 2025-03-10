using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.ShippingAPI.DHL.Models
{
    public class QtdSInAdCur
    {
        [Key]
        public int Id { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyRoleTypeCode { get; set; }
        public decimal? WeightCharge { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? TotalTaxAmount { get; set; }
        public decimal? WeightChargeTax { get; set; }
        public WeightChargeTaxDet WeightChargeTaxDet { get; set; }
    }
}
