using Omnae.Common;
using System.Collections.Generic;

namespace Omnae.BusinessLayer.Models
{
    public class ChartTypeViewModel
    {

        public ChartType ChartType { get; set; }
        public float ConformanceRate { get; set; }
        public float PctgNCRsOrders { get; set; }
        public float PctgCustomerNCRsQuantities { get; set; }
        public float PctgVendorNCRsQuantities { get; set; }
        public int? Index { get; set; }
        public int TotalOrderQty { get; set; }
        public int TotalOrders { get; set; }
        public ChartData chartData { get; set; }
        public Dictionary<string, string> DicFilter { get; set; }
    }

    public class ChartData
    {
        public string[] DateRange { get; set; }
        public int[] TotalQuantity { get; set; }
        public int[] TotalCustomerNcrs { get; set; }
        public int[] TotalVendorNcrs { get; set; }
    }
}