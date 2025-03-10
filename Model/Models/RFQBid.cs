using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Omnae.Common;
using Omnae.Model.Models.Aspnet;

namespace Omnae.Model.Models
{
    public class RFQBid
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Index("IX_ProductId")]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [Index("IX_VendorId")]
        [ForeignKey("VendorCompany")]
        public int VendorId { get; set; }

        public string BidFailedReason { get; set; }

        public int? RFQQuantityId { get; set; }

        public int? ProductLeadTime { get; set; }
        public int? SampleLeadTime { get; set; }
        public decimal? ToolingCharge { get; set; }

        public bool? IsActive { get; set; }
        public DateTime BidDatetime { get; set; }

        public int? QuoteDocId { get; set; }
        public DateTime? QuoteAcceptDate { get; set; }
        public string HarmonizedCode { get; set; }


        [ForeignKey("CreatedByUser")]
        public string CreatedByUserId { get; set; }
        
        [ForeignKey("ModifiedByUser")]
        public string ModifiedByUserId { get; set; }

        public CurrencyCodes? PreferredCurrency { get; set; }

        // Navigation Property

        [CanBeNull]
        public virtual SimplifiedUser CreatedByUser { get; set; }

        [CanBeNull]
        public virtual SimplifiedUser ModifiedByUser { get; set; }

        public virtual Product Product { get; set; }

        public virtual Company VendorCompany { get; set; }

    }
}
