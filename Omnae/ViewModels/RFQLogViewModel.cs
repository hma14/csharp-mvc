using Omnae.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Omnae.ViewModels
{
    public class RFQLogViewModel
    {
        [Display(Name = "RFQ Created Date")]
        public DateTime? RFQCreatedDate { get; set; }

        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Display(Name = "Product Type")]
        public MATERIALS_TYPE ProductType { get; set; }

        [Display(Name = "Part Number")]
        public string PartNumber { get; set; }
        [Display(Name = "Revision Number")]
        public string RevisionNumber { get; set; }
        [Display(Name = "Quote Accepted Date")]
        public DateTime? QuoteAcceptDate { get; set; }
        [Display(Name = "Selected Vendor Name")]
        public string SelectedVendorName { get; set; }
        [Display(Name = "Current State")]
        public States CurrentState { get; set; }
    }
}