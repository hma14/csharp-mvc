using Hangfire.Annotations;
using Omnae.Model.Models.Aspnet;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Omnae.Model.Models
{
    public class VendorBidRFQStatus
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int VendorId { get; set; }

        public int StateId { get; set; }
        public int TaskId { get; set; }
        
        //public string RejectRFQReason { get; set; }

        [ForeignKey("BidRequestRevision")]
        public int? BidRequestRevisionId { get; set; }

        [ForeignKey("BidRFQStatus")]
        public int? BidRFQStatusId { get; set; }

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

        [CanBeNull]
        public virtual BidRequestRevision BidRequestRevision { get; set; }

        public virtual BidRFQStatus BidRFQStatus { get; set; }

        [CanBeNull]
        public virtual SimplifiedUser CreatedByUser { get; set; }

        [CanBeNull]
        public virtual SimplifiedUser ModifiedByUser { get; set; }

        [CanBeNull]
        public virtual RFQActionReason RFQActionReason { get; set; }


    }
}
