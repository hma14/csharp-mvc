using Omnae.ShippingAPI.DHL.Libs;

namespace Omnae.ShippingAPI.DHL.Models
{
    public class RequestBase
    {
        public ServiceHeader ServiceHeader { get; set; }
        public REQUESTS RequestType { get; set; }
    }
}
