using Omnae.Model.Context.Model;
using System;
using System.Collections.Generic;

namespace Omnae.BusinessLayer.Models
{
    public class RFQChartDataViewModel
    {
        public CompanyInfo Company { get; set; }
        public List<RFQChartData> ChartData { get; set; } = new List<RFQChartData>();
    }

    public class CompanyInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }


    public class RFQChartData
    {
        //public DateTime At { get; set; }
        public string At { get; set; }
        public int ShippedParts { get; set; }
        public int NCRParts { get; set; }
        public int ShippedOrders { get; set; }
        public int OrdersOnTime { get; set; }
        public int NcrsByVendor { get; set; }
        public float PartsConformance { get; set; }
        public float OrderConformance { get; set; }
        public float OnTimeConformance { get; set; }
        public int AvrLeadTime { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}

