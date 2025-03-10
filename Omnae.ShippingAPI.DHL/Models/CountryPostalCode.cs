using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.ShippingAPI.DHL.Models
{
    public class CountryPostalCode
    {
        [Display(Name = "Country Code")]
        public string CountryCode { get; set; }

        [Display(Name = "Postal Code")]
        public string Postalcode { get; set; }
        public string City { get; set; }
        public string Suburb { get; set; }
    }
}
