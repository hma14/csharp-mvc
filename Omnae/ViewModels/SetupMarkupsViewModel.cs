using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Omnae.ViewModels
{
    public class SetupMarkupsViewModel
    {
        public int TaskId { get; set; }
        public int ProductId { get; set; }
        public int? RfqBidId { get; set; }

        [Display(Name = "Markup for Tooling Charges for Customer")]
        public float? ToolingMarkup { get; set; }
        public List<QtyMarkup> QtyMarks { get; set; }
        public decimal? CustomerToolingCharges { get; set; }
        public decimal? VendorToolingCharges { get; set; }
        public bool isAddQty { get; set; }
        public int? NumberSampleIncluded { get; set; }

    }

    public class QtyMarkup
    {
        public decimal Quantity { get; set; }

        [Display(Name = "Vendor Unit Price")]
        [DisplayFormat(DataFormatString = "{0:n3}", ApplyFormatInEditMode = true)]
        public decimal VendorUnitPrice { get; set; }

        [Display(Name = "Ship Unit Price")]
        public decimal ShipUnitPrice { get; set; }

        [Display(Name = "Customer Unit Price")]
        public decimal? UnitPrice { get; set; }
        public float? Markup { get; set; }
        
    }
}