using Hangfire.Annotations;
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
    public class BidRFQStatus
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public int StateId { get; set; }
        public int TaskId { get; set; }
        public int SubmittedVendors { get; set; }
        public int TotalVendors { get; set; }

        public string KeepCurrentRevisionReason { get; set; }

        [ForeignKey("PartRevision")]
        public int? PartRevisionId { get; set; }
        public int? RevisionCycle { get; set; }
      
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime? _updatedAt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime? _createdAt { get; set; }

        [ForeignKey("CreatedByUser")]
        public string CreatedByUserId { get; set; }

        [ForeignKey("ModifiedByUser")]
        public string ModifiedByUserId { get; set; }

        [ForeignKey("RFQActionReason")]
        public int? RFQActionReasonId { get; set; }

        // Navigation Property

        //public virtual ICollection<Document> RevisionDocs { get; set; }
        public virtual ICollection<VendorBidRFQStatus> VendorBidRFQStatus { get; set; }

        [CanBeNull]
        public virtual PartRevision PartRevision { get; set; }

        [CanBeNull]
        public virtual SimplifiedUser CreatedByUser { get; set; }

        [CanBeNull]
        public virtual SimplifiedUser ModifiedByUser { get; set; }

        [CanBeNull]
        public virtual RFQActionReason RFQActionReason { get; set; }

    }
}
