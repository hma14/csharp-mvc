using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.WebApi.DTO
{
    public class VendorPerformanceDTO
    {
        public string VendorName { get; set; }
        public string VendorLocation { get; set; }
        public float PartConformance { get; set; }
        public float OrderConformance { get; set; }
        public float OnTime { get; set; }
        public int CompletedParts { get; set; }
        public int CompletedOrders { get; set; }
        public int AvrLeadTime { get; set; }
        
    }
}