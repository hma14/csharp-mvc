using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Model.Models
{
    public class ExtraQuantity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

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

        //[Required(ErrorMessage = "Quantity 1 is required")]
        [Display(Name = "Qty 1")]
        //[RegularExpression(@"^([1-9][0]{1,4})", ErrorMessage = "MaxQuantity must be between 10 and 90000, such that 10, 20 ..., or 100, 200 ..., 90000, ")]
        public decimal? Qty1 { get; set; }

        [Display(Name = "Qty 2")]
        //[RegularExpression(@"^([1-9][0]{1,4})", ErrorMessage = "MaxQuantity must be between 10 and 90000, such that 10, 20 ..., or 100, 200 ..., 90000, ")]
        public decimal? Qty2 { get; set; }

        [Display(Name = "Qty 3")]
        //[RegularExpression(@"^([1-9][0]{1,4})", ErrorMessage = "MaxQuantity must be between 10 and 90000, such that 10, 20 ..., or 100, 200 ..., 90000, ")]
        public decimal? Qty3 { get; set; }

        [Display(Name = "Qty 4")]
        //[RegularExpression(@"^([1-9][0]{1,4})", ErrorMessage = "MaxQuantity must be between 10 and 90000, such that 10, 20 ..., or 100, 200 ..., 90000, ")]
        public decimal? Qty4 { get; set; }

        [Display(Name = "Qty 5")]
        //[RegularExpression(@"^([1-9][0]{1,4})", ErrorMessage = "MaxQuantity must be between 10 and 90000, such that 10, 20 ..., or 100, 200 ..., 90000, ")]
        public decimal? Qty5 { get; set; }

        [Display(Name = "Qty 6")]
        //[RegularExpression(@"^([1-9][0]{1,4})", ErrorMessage = "MaxQuantity must be between 10 and 90000, such that 10, 20 ..., or 100, 200 ..., 90000, ")]
        public decimal? Qty6 { get; set; }

        [Display(Name = "Qty 7")]
        //[RegularExpression(@"^([1-9][0]{1,4})", ErrorMessage = "MaxQuantity must be between 10 and 90000, such that 10, 20 ..., or 100, 200 ..., 90000, ")]
        public decimal? Qty7 { get; set; }

        public int? NumberSampleIncluded { get; set; }
    }
}
