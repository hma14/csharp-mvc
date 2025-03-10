using Omnae.Common;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Omnae.ViewModels
{
    public class ShippingAccountViewModel
    {
        [Required]
        [MaxLength(100)]
        [Display(Name = "Account Name")]
        public string Name { get; set; }

        public int CompanyId { get; set; }
        public SHIPPING_CARRIER Carrier { get; set; }
        public SHIPPING_CARRIER_TYPE CarrierType { get; set; }

        [Display(Name ="Account Number")]
        public string AccountNumber { get; set; }
        public SelectList Companies { get; set; }
        public List<ShippingAccount> ShippingAccountList { get; set; }
    }
}