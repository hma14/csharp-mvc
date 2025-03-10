using Omnae.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Omnae.BusinessLayer.Models;

namespace Omnae.WebApi.DTO
{
    public class RFQBidSummary
    {
        public int TaskId { get; set; }
        public States State { get; set; }

        public PartDetails PartDetails { get; set; }
        public List<BidDetails> BidDetails { get; set; }

        
        

    }

    public class PartDetails
    {

        public string PartName { get; set; }
        public BUILD_TYPE BuildType { get; set; }
        public MATERIALS_TYPE MaterialType { get; set; }

        public string PartNumber { get; set; }
        public string RevisionNumber { get; set; }

        public int? SampleLeadTime { get; set; }

        public int? ProductionLeadTime { get; set; }

        public int? NumberSampleIncluded { get; set; }

        public decimal? UnitPrice { get; set; }
        public int? Quantity { get; set; }

       
    }

    public class BidDetails
    {
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public string HarmonizedCode { get; set; }

       // public IEnumerable<PriceBreakDTO> PriceBreaks { get; set; }

        public Quantities Quantities { get; set; }
        public List<decimal> UnitPrices { get; set; } = new List<decimal>();
        public List<string> QuoteDocUri { get; set; }
        public int? SampleTime { get; set; }
        public int? ProductionLeadTime { get; set; }
        public int? EstShippingTime { get; set; }
        public decimal? AvrShipping { get; set; }
        public decimal? UnitShipping { get; set; }
        public decimal? ToolingCharge { get; set; }
        public UserPerformance VendorPerformance { get; set; }
    }

    public class UserPerformance
    {
        public string UserName { get; set; }
        public AddressDTO UserLocation { get; set; }
        public float? PartConformance { get; set; }
        public float OrderConformance { get; set; }
        public float? OnTimeConformance { get; set; }
        public int? CompletedParts { get; set; }
        public int? CompletedOrders { get; set; }
        public int? AvrLeadTime { get; set; }

    }
}