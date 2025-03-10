using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Omnae.BusinessLayer.Models
{
    public class TransactionViewModel
    {
        [Display(Name = "Customer")]
        public string CustomerName { get; set; }
        [Display(Name = "P/N")]
        public string PNRev { get; set; }
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }
        [Display(Name = "ER @")]
        public decimal ExchangeRate { get; set; }

        // Sales
        [Display(Name = "S/O")]
        public string EstimateNumber { get; set; }
        [Display(Name = "Sub Total USD")]
        public decimal CustomerTotal1 { get; set; }
        [Display(Name = "Tooling Setup")]
        public decimal CustomerToolingSetup { get; set; }
        [Display(Name = "Sales Tax")]
        public decimal SalesTax { get; set; }
        [Display(Name = "Sub Total")]
        public decimal CustomerTotal2 { get; set; }
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }
        [Display(Name = "Ship Date")]
        public DateTime? ShipDate { get; set; }
        [Display(Name = "Net Total CAD")]
        public decimal CustomerNetTotalCAD { get; set; }

        // Purchase
        [Display(Name = "Vendor")]
        public string VendorName { get; set; }
        [Display(Name = "PO #")]
        public string PONumber { get; set; }
        public string POUri { get; set; }
        [Display(Name = "Total USD")]
        public decimal VendorTotal1 { get; set; }
        [Display(Name = "Tooling Setup")]
        public decimal VendorToolingSetup { get; set; }
        [Display(Name = "Sub Total USD")]
        public decimal VendorTotal2 { get; set; }
        [Display(Name = "Total CAD")]
        public decimal VendorTotalCAD { get; set; }
        [Display(Name = "Shipping CAD")]
        public decimal ShippingCAD { get; set; }
        [Display(Name = "Net Total CAD")]
        public decimal VendorNetTotalCAD { get; set; }

        [Display(Name = "Profit & Loss CAD")]
        public decimal ProfitLossCAD { get; set; }
        [Display(Name = "Gross Profit Margin")]
        public double GrossProfitMargin { get; set; }
        [Display(Name = "Invoice #")]
        public string InvoiceNumber { get; set; }

        
    }
}