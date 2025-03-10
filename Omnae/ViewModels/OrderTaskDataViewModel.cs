using Omnae.Common;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.ViewModels
{
    public class OrderTaskDataViewModel
    {
        public int OrderId { get; set; }
        public int? TaskId { get; set; }
        public int ProductId { get; set; }
        public DateTime OrderDate { get; set; }
        public string PartNumber { get; set; }
        public string PartNumberRevision { get; set; }
        public DateTime? ShippedDate { get; set; }
        public string CarrierName { get; set; }
        public string TrackingNumber { get; set; }
        public string CustomerPONumber { get; set; }
        public DateTime? EstimateCompletionDate { get; set; }
        public DateTime? RequestedShipDate { get; set; }
        public int StateId { get; set; }
        public decimal Quantity { get; set; }
        public string Notes { get; set; }
        public Product Product { get; set; }

    }
}