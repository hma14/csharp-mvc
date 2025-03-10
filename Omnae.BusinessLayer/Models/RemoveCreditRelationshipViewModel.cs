using Omnae.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Omnae.BusinessLayer.Models
{
    public class RemoveCreditRelationshipViewModel
    {
        [Display(Name = "Customer ID")]
        public int CustomerId { get; set; }

        [Display(Name = "Vendor ID")]
        public int VendorId { get; set; } 

        public SelectList Customers { get; set; }
        public SelectList Vendors { get; set; }
    }
}
