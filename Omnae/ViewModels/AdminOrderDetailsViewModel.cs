using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Omnae.ViewModels
{
    public class AdminOrderDetailsViewModel
    {
        public int OrdorId { get; set; }

        [Display(Name = "Part Number")]
        public string PartNumber { get; set; }

        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal? SalesPrice { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal? SalesTax { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        [Display(Name = "Unit Price")]
        public decimal? UnitPrice { get; set; }
        public int Quantity { get; set; }

        [Display(Name = "Shipped Date")]
        public DateTime ShippedDate { get; set; }

        [Display(Name = "Carrier Name")]
        public string CarrierName { get; set; }

        [Display(Name = "Tracking Number")]
        public string TrackingNumber { get; set; }

        // [ForeignKey("Shipping")]
        [Display(Name = "Shipping Id")]
        public int? ShippingId { get; set; }

        [Display(Name = "Ship Leading Time")]
        public int? ShipLeadingTime { get; set; }

        [Display(Name = "Customer PO Number")]
        public string CustomerPONumber { get; set; }
        public DateTime? EstimateCompletionDate { get; set; }
        public bool? IsForToolingOnly { get; set; }

        public SelectList DdlOrders { get; set; }
    }
}