using Omnae.ShippingAPI.DHL.Models;

namespace Omnae.BusinessLayer.Models
{
    public class ShippingQuoteRequestViewModel
    {
        public CountryPostalCode Origin { get; set; }
        public CountryPostalCode Destination { get; set; }
        public BkgDetails BkgDetails { get; set; }
        public Dutiable Dutiable { get; set; }
    }
}