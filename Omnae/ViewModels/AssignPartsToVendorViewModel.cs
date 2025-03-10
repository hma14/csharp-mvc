using Omnae.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Omnae.ViewModels
{
    public class AssignPartsToVendorViewModel
    {
        [Display(Name ="Product ID")]
        public int ProductId { get; set; }
        [Display(Name = "Vendor ID")]
        public int VendorId { get; set; }
    }
}