using System.Collections.Generic;

namespace Omnae.BusinessLayer.Models
{
    public enum ChartType
    {
        LineChart = 1,
        BarChart,
        DonutChart,
        PieChart
    }
    public class ChartInfoViewModel
    {
        public ChartType ChartType { get; set; }
        public List<string> Products { get; set; }
        public List<string> DateRange { get; set; }
        public List<int> TotalQuantity { get; set; }
        public List<int> TotalQuantityWithoutNcrs { get; set; }
        public List<int> TotalCustomerNcrs { get; set; }
        public List<int> TotalVendorNcrs { get; set; }
    }
}