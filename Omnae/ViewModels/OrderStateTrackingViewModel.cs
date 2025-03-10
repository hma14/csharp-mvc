using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.ViewModels
{
    public class OrderStateTrackingViewModel
    {
        public OrderStateTracking OrderStateTracking { get; set; }
        public bool IsRiskbuild { get; set; }
    }
}