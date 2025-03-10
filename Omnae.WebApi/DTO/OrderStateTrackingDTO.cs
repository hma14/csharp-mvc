using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.WebApi.DTO
{
    public class OrderStateTrackingDTO
    {
        public int OrderId { get; set; }
        public int StateId { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime ModifiedUtc { get; set; }
    }
}