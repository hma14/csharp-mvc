using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.ShippingAPI.DHL.Models
{
    public class RequestQuote : RequestBase
    {      
        public CountryPostalCode Origin { get; set; }
        public CountryPostalCode Destination { get; set; }
        public BkgDetails BkgDetails { get; set; }
        public Dutiable Dutiable { get; set; }
    }
}
