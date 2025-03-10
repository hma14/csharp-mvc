using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.ShippingAPI.DHL.Models
{
    public class ServiceHeader
    {
        public DateTime MessageTime { get; set; }
        public string MessageReference { get; set; }
        public string SiteID { get; set; }
        public string Password { get; set; }

    }
}
