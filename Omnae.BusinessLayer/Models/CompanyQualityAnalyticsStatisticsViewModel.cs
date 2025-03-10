using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.BusinessLayer.Models
{
    public class CompanyQualityAnalyticsStatisticsViewModel
    {
        /// <summary>
        /// (totals.shippedOrders - totals.ncrsByVendor) / totals.shippedOrders
        /// </summary>
        public float? OrdersConformanceRate { get; set; }

        /// <summary>
        /// total count of all orders used for above metric
        /// </summary>
        public float? OrdersConformaceCount { get; set; }

        /// <summary>
        /// (totals.shippedParts - totals.ncrParts) / totals.shippedParts
        /// </summary>
        public float? PartsConformanceRate { get; set; }


        /// <summary>
        /// total count of all parts 
        /// </summary>
        public float? PartsConformanceCount { get; set; }

        /// <summary>
        /// totals.ordersOnTime / totals.shippedOrders
        /// </summary>
        public float? OnTimeConformanceRate { get; set; }


    }
}
