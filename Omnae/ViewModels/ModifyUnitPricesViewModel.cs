using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Omnae.ViewModels
{
    public class ModifyUnitPricesViewModel
    {
        public int ProductId { get; set; }
        public int? TaskId { get; set; }
        public int CustomerId { get; set; }

        public SelectList Products { get; set; }
        public SelectList Customers { get; set; }
        public string VendorName { get; set; }

        public List<GetQtyUnitPrices> UnitPriceList { get; set; }

    }

    public class GetQtyUnitPrices
    {
        public int PriceBreakId { get; set; }
        public decimal Quantity { get; set; }

        [DisplayFormat(DataFormatString = "{0:n3}", ApplyFormatInEditMode = true)]
        public decimal CustomerUnitPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:n3}", ApplyFormatInEditMode = true)]
        public decimal VendorUnitPrice { get; set; }

        [Display(Name = "Cust. Tooling Charge")]
        public decimal CustomerToolingCharge { get; set; }

        [Display(Name = "Vend. Tooling Charge")]
        public decimal VendorToolingCharge { get; set; }

    }
}