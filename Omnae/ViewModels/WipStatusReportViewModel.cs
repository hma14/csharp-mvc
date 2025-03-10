using Omnae.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace Omnae.ViewModels
{
    public class WipStatusReportViewModel
    {
        [Display(Name = "Part #")]
        public string PartNumber { get; set; }

        [Display(Name = "Part Rev.")]
        public string PartNumberRevision { get; set; }

        public decimal Qty { get; set; }

        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }
        
        public int CustomerId { get; set; }

        [Display(Name = "Est. Compl. Date")]
        public DateTime? EstimatedCompletionDate { get; set; }

        [Display(Name = "Req. Ship Date")]
        public DateTime? RequestedShipDate { get; set; }

        [Display(Name = "S/O")]
        public string EstimateNumber { get; set; }

        [Display(Name = "Customer")]
        public string CustomerName { get; set; }

        public int VendorId { get; set; }

        [Display(Name = "Vendor")]
        public string VendorName { get; set; }

        [Display(Name = "Cust. PO#")]
        public string CustomerPONumber { get; set; }
        public string CustomerPOUri { get; set; }

        [Display(Name = "Vend. PO#")]
        public string VendorPONumber { get; set; }
        public string VendorPOUri { get; set; }


        [Display(Name = "State")]
        public States State { get; set; } 

        public int OrderId { get; set; }
        public string Notes { get; set; }

    }
}