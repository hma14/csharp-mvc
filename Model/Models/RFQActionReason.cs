using Hangfire.Annotations;
using Omnae.Common;
using Omnae.Model.Models.Aspnet;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Model.Models
{
    public class RFQActionReason
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Index("IX_VendorRejectRFQ", 1, IsUnique = true)]
        public int ProductId { get; set; }

        [Index("IX_VendorRejectRFQ", 2, IsUnique = true)]
        public int VendorId { get; set; }

        [Index("IX_VendorRejectRFQ", 3, IsUnique = true)]
        public REASON_TYPE? ReasonType { get; set; }
        public string Reason { get; set; }
        public string Description { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime? _updatedAt { get; set; }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime? _createdAt { get; set; }

        [ForeignKey("CreatedByUser")]
        public string CreatedByUserId { get; set; }

        [ForeignKey("ModifiedByUser")]
        public string ModifiedByUserId { get; set; }

        [CanBeNull]
        public virtual SimplifiedUser CreatedByUser { get; set; }

        [CanBeNull]
        public virtual SimplifiedUser ModifiedByUser { get; set; }
    }
}
