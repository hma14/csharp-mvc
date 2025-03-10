using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.ShippingAPI.DHL.Models
{
    public class QtdShpExChrg
    {
        [Key]
        public int Id { get; set; }
        public string SpecialServiceType { get; set; }
        public string LocalServiceType { get; set; }
        public string GlobalServiceName { get; set; }
        public string LocalServiceTypeName { get; set; }
        public string ChargeCodeType { get; set; }
        public string CurrencyCode { get; set; }
        public decimal? ChargeValue { get; set; }
        public decimal? ChargeTaxAmount { get; set; }

        public ChargeTaxAmountDet ChargeTaxAmountDet { get; set; }
        public List<QtdSExtrChrgInAdCur> QtdSExtrChrgInAdCurList { get; set; }
    }
}
