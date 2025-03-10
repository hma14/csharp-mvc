using Omnae.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.WebApi.DTO
{
    public class ProductStateTrackingDTO
    {
        public int ProductId { get; set; }
        public List<StateDatetimePair> StateDateTime { get; set; }

    }

    public class StateDatetimePair
    {
        public States State { get; set; }
        public DateTime ModifiedUtc { get; set; }
    }

}