using System.ComponentModel.DataAnnotations;

namespace Omnae.BusinessLayer.Models
{
    public class CalculateResultViewModel
    {
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        [Display(Name = "Tooling Charge")]
        public decimal? ToolingCharges { get; set; }
        [Display(Name = "Production Charge")]
        public decimal ProductionCharge  { get; set; }
        [Display(Name = "Tax of Both Production and Tooling (if Tooling Charge > 0)")]
        public decimal? SalesTax { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public double? TaxRate { get; set; }
        public string TaxRatePercentage { get; set; }
    }
}