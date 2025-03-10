using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.ShippingAPI.DHL.Models
{
    public class Address
    {
        public string CountryCode { get; set; }
        public string Postalcode { get; set; }
        public string City { get; set; }
    }
}
