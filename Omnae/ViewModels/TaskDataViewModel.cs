using Omnae.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Omnae.ViewModels
{
    public class TaskDataViewModel
    {
        [Key]
        public int TaskId { get; set; }

        [Required]
        [Display(Name = "Vendor User Name")]
        public int VendorId { get; set; }

        [Required]
        [Display(Name = "Current State")]
        public States StateId { get; set; }

        public IEnumerable<SelectListItem> Vendors { get; set; }


    }
}