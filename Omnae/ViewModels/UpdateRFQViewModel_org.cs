using Omnae.Common;
using Omnae.Model.Models;
using System.ComponentModel.DataAnnotations;

namespace Omnae.ViewModels
{
    public class UpdateRFQViewModel
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Sample Lead Time (in days)")]
        public int? SampleLeadTime { get; set; }

        [Display(Name = "Production Lead Time (in days)")]
        public int? ProductionLeadTime { get; set; }

        [Display(Name = "Tooling Setup Charges")]
        public decimal? ToolingSetupCharges { get; set; }

        public Document QuoteDoc { get; set; }

        [Display(Name = "Harmonized Code")]
        public string HarmonizedCode { get; set; }
    }
}