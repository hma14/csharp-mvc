using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.BusinessLayer.Models
{
    public class UserPerformanceViewModel
    {
        public float? PartConformance { get; set; }
        public float? OrderConformance { get; set; }
        public float? OnTimeConformance { get; set; }
        public int? CompletedParts { get; set; }
        public int? CompletedOrders { get; set; }
        public int? AvrLeadTime { get; set; }
        public int? NumberOfMyRootCause { get; set; }
    }
}
