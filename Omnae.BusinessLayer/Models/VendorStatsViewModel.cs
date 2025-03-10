using Omnae.Model.Context.Model;
using System;
using System.Collections.Generic;
using static Omnae.Data.Query.CompanyQuery;

namespace Omnae.BusinessLayer.Models
{
    public class VendorStatsViewModel
    {
        public int Id { get; set; }

        // Orderby (sort) following
        public string Name { get; set; }
        public string Country { get; set; }
        public float PartsConformance { get; set; }
        public float OrderConformance { get; set; }
        public float OnTimeConformance { get; set; }
        public int CompletedParts { get; set; } // Shipped Parts in the new Front-end
        public int CompletedOrders { get; set; } // Shipped Orders in the new Front-end
        public int LeadTime { get; set; } //Avarage  in the new Front-end

        public CompanyInfo Company { get; set; }
        public RFQChartData Stats { get; set; }
        public VENDOR_TYPE VendorType { get; set; }
        public PARTNER_TYPE PartnerType { get; set; }
    }
}

