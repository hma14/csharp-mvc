using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.ShippingAPI.DHL.Models
{
    public class QtdSExtrChrgInAdCur
    {
        [Key]
        public int Id { get; set; }
        public decimal? ChargeValue { get; set; }
        public decimal? ChargeTaxAmount { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyRoleTypeCode { get; set; }

        public ChargeTaxAmountDet ChargeTaxAmountDet { get; set; }
    }
}
