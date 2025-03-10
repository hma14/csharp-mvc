using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Model.Models
{
    public class ProductPriceQuote
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int VendorId { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public int ProductionLeadTime { get; set; }
        public DateTime ExpireDate { get; set; }

        public string QuoteDocUri { get; set; }

        public bool? IsActive { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }


        // Navigation Properties

        public virtual Product Product { get; set; }
        public virtual ICollection<PriceBreak> PriceBreaks { get; set; }
    }
}
