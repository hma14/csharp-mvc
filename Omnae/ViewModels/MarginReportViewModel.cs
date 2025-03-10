using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.ViewModels
{
    public class MarginReportViewModel
    {
        public List<SalesViewModel> SaleList { get; set; }
        public List<PurchaseViewModel> PurchaseList { get; set; }
    }
}