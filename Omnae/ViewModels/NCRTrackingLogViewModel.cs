using Omnae.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Omnae.ViewModels
{
    public class NCRTrackingLogViewModel
    {
        [Display(Name = "Date Initiated")]
        public DateTime? NCDetectedDate { get; set; }

        [Display(Name = "Root Cause")]
        public NC_ROOT_CAUSE? RootCause { get; set; }

        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Display(Name = "Vendor")]
        public string VendorName { get; set; }
        public decimal? Cost { get; set; }

        [Display(Name = "Date Closed")]
        public DateTime? DateClosed { get; set; }
    }
}