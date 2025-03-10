using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.ShippingAPI.DHL.Models
{
    public class QtdShp
    {
        public string GlobalProductCode { get; set; }
        public string LocalProductCode { get; set; }

        public string QtdShpExChrg_SpecialServiceType { get; set; }
        public string QtdShpExChrg_LocalSpecialServiceType { get; set; }

    }
}
