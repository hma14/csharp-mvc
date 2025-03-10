using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.ShippingAPI.DHL.Models
{
    public class Dutiable
    {
        [Required(ErrorMessage = "Declared Currency is requried")]
        [Display(Name = "Declared Currency")]
        public string DeclaredCurrency { get; set; }

        [Required(ErrorMessage = "Declared Value is requried")]
        [Display(Name = "Declared Value")]
        public string DeclaredValue { get; set; }       
    }
}
