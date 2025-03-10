using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Omnae.Model.Models.Aspnet;

namespace Omnae.Model.Models
{
    public class PriceBreak
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Index("IX_PriceBreak", 1, IsUnique = true)]
        public int? RFQBidId { get; set; }

        [Index("IX_PriceBreak", 2, IsUnique = true)]
        public int ProductId { get; set; }
        
        [Index("IX_PriceBreak", 3, IsUnique = true)]
        [DisplayFormat(DataFormatString = "{0:0.000}", ApplyFormatInEditMode = true)]
        public decimal Quantity { get; set; }

        //[DisplayFormat(DataFormatString = "{0:n3}", ApplyFormatInEditMode = true)]
        //public decimal QuantityTemp { get; set; }

        [Index("IX_PriceBreak", 4, IsUnique = true)]
        public int? TaskId { get; set; }

        //[DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:0.000}", ApplyFormatInEditMode = true)]
        public decimal? UnitPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.000}", ApplyFormatInEditMode = true)]
        public decimal VendorUnitPrice { get; set; }

        //[DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal? ShippingUnitPrice { get; set; }

        public int? ShippingDays { get; set; }

        public float? Markup { get; set; }

        [Display(Name = "Tooling Charges")]
        public decimal? ToolingSetupCharges { get; set; }

        
        public decimal? CustomerToolingSetupCharges { get; set; }

        public int? NumberSampleIncluded { get; set; }

        [ForeignKey("CreatedByUser")]
        public string CreatedByUserId { get; set; }
        
        [ForeignKey("ModifiedByUser")]
        public string ModifiedByUserId { get; set; }

        [ForeignKey("ProductPriceQuote")]
        public int? ProductPriceQuoteId { get; set; }

        public int UnitOfMeasurement { get; set; }


        // Navigation Property

        [CanBeNull]
        public virtual RFQBid RFQBid { get; set; }

        [CanBeNull]
        public virtual SimplifiedUser CreatedByUser { get; set; }

        [CanBeNull]
        public virtual SimplifiedUser ModifiedByUser { get; set; }

        [CanBeNull]
        public virtual ProductPriceQuote ProductPriceQuote { get; set; }
    }
}
