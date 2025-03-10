using Omnae.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Model.Models
{
    public class CompaniesCreditRelationship
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        [Key]
        [Column(Order = 1)]
        [ForeignKey("Vendor")]
        public int VendorId { get; set; }

        public bool isTerm { get; set; }
        public int? TermDays { get; set; }

        [Display(Name = "Credit Limit ($)")]
        [DataType(DataType.Currency)]
        public decimal? CreditLimit { get; set; }

        [Display(Name = "Tax (%)")]
        [Required]
        [Range(0, int.MaxValue)]
        public int TaxPercentage { get; set; } = 0;

        public CurrencyCodes Currency { get; set; }

        public int? DiscountDays { get; set; }
        public float? Discount { get; set; }
        public int? Deposit { get; set; }   
        
        [Display(Name = "Tooling Deposit (%)")]
        [Range(0,int.MaxValue)]
        public int? ToolingDepositPercentage { get; set; }

        // Navigation Properties
        public virtual Company Vendor { get; set; }
        public virtual Company Customer { get; set; }
    }
}
