using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.ShippingAPI.DHL.Models
{
    public class WeightChargeTaxDet
    {
        [Key]
        public int Id { get; set; }
        public decimal? TaxTypeRate { get; set; }
        public string TaxTypeCode { get; set; }
        public decimal? WeightChargeTax { get; set; }
        public decimal? BaseAmt { get; set; }
    }
}
