using System.ComponentModel.DataAnnotations;

namespace Omnae.BusinessLayer.Models
{
    public class ExtraQuantityPartDetailsViewModel
    {
        [Display(Name = "Tooling Lead Time")]
        public int? ToolingLeadTime { get; set; }

        [Display(Name = "Sample Lead Time")]
        public int? SampleLeadTime { get; set; }

        [Display(Name = "Prod. Lead Time")]
        public int? ProductionLeadTime { get; set; }

        [Display(Name = "Tooling Charges")]
        public decimal? ToolingSetupCharges { get; set; }

        [Display(Name = "Cust. Tooling Charges")]
        public decimal? CustomerToolingSetupCharges { get; set; }

        [Display(Name = "Harmonized Code")]
        public string HarmonizedCode { get; set; }

        public int? NumberSampleIncluded { get; set; }
    }
}