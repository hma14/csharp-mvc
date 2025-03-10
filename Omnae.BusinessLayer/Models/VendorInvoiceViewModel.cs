using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Omnae.BusinessLayer.Models
{
    public class VendorInvoiceViewModel
    {
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Due Date")]
        public DateTime? DueDate { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; }
        public string PartNumber { get; set; }
        public string PartRevision { get; set; }
        public string ProductDescription { get; set; }

        [Display(Name = "Display Name")]
        public string CustomerName { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal SalesPrice { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal SalesTax { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        public decimal ToolingCharges { get; set; }
        public decimal Quantity { get; set; }

        public decimal Total { get; set; }

        [Display(Name = "Shipped Date")]
        public DateTime ShippedDate { get; set; }

        [Display(Name = "Carrier Name")]
        public string CarrierName { get; set; }

        [Display(Name = "Tracking Number")]
        public string TrackingNumber { get; set; }

        [Display(Name = "Attach Invoice For Tooling Setup")]
        public HttpPostedFileBase AttachInvoiceForTooling { get; set; }

        [Display(Name = "Attached Invoice Number For Tooling Setup")]
        public string VendorAttachedInviceNumberForTooling { get; set; }

        [Display(Name = "Attach Invoice")]
        public HttpPostedFileBase AttachInvoice { get; set; }

        [Display(Name = "Attached Invoice Number")]
        public string VendorAttachedInvoiceNumber { get; set; }
        public bool IsToolingSeparate { get; set; }
        public int? NumberSampleIncluded { get; set; }
    }
}