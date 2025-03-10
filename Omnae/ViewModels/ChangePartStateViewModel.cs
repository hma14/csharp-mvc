using Omnae.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Omnae.ViewModels
{
    public class ChangePartStateViewModel
    {
        [Key]
        [Display(Name ="Product ID")]
        public int ProductId { get; set; }
        [Display(Name = "State ID")]
        public States StateId { get; set; }
    }
}