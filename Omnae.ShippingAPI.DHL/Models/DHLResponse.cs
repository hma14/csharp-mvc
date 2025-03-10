using Omnae.ShippingAPI.DHL.Libs;
using System.Collections.Generic;

namespace Omnae.ShippingAPI.DHL.Models
{
    public class DHLResponse
    {
        public REQUESTS RequestType { get; set; }
        public List<ResponseAWBInfo> Trackings { get; set; }
        public ResponseQuote Quote { get; set; }

    }
}
