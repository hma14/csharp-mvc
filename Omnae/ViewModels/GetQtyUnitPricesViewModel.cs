using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.ViewModels
{

    public class GetQtyUnitPricesViewModel
    {
        public List<GetQtyUnitPrices> UnitPriceList { get; set; }
    }
    public class GetQtyUnitPrices
    {
        public int PriceBreakId { get; set; }
        public int Quantity { get; set; }
        public decimal CustomerUnitPrice { get; set; }
        public decimal VendorUnitPrice { get; set; }

    }
}