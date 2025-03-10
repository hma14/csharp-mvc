using Libs.ViewModels;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Omnae.Common;

namespace Omnae.Libs.ViewModel
{
    public class OrderCompleteEmailViewModel : BaseViewModel
    {
        public decimal Quantity { get; set; }

        public string CurrencySymbol { get; set; } = NMoneys.Currency.Usd.Symbol;
        public string CurrencyTextSymbol { get; set; } = NMoneys.Currency.Usd.IsoSymbol;

        public decimal? Price { get; set; }
        public decimal? Quantity2 { get; set; }
        public decimal? Price2 { get; set; }
        public decimal? VendorPrice { get; set; }

        public int? ProductionLeadTime { get; set; }
        public Document Doc_2D { get; set; }
        public Document Doc_3D { get; set; }
        public Document Doc_Quote { get; set; }
        public Document Doc_PO { get; set; }
        public Document Doc_VENDOR_PO { get; set; }
        public string PONumber { get; set; }
        public Document Doc_CustomerInvoice { get; set; }
        public bool IsSampleOrder { get; set; }
        public bool? IsRiskBuild { get; set; }
    }
}