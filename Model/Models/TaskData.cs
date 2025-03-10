using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;
using Omnae.Model.Models.Aspnet;
using Omnae.Common;

namespace Omnae.Model.Models
{

    public class TaskData
    {   
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int TaskId { get; set; }

        public int StateId { get; set; }

        [ForeignKey("Product")]
        public int? ProductId { get; set; }

        [Display(Name = "Revising Reason")]
        public string RevisingReason { get; set; }

        [Display(Name = "Reject Reason")]
        public string RejectReason { get; set; }

        public DateTime CreatedUtc { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime ModifiedUtc { get; set; }

        public bool? IsRiskBuild { get; set; }
        public bool? isTagged { get; set; }

        public int? RFQBidId { get; set; }
        
        public bool isEnterprise { get; set; }

        [ForeignKey("CreatedByUser")]
        public string CreatedByUserId { get; set; }
        
        [ForeignKey("ModifiedByUser")]
        public string ModifiedByUserId { get; set; }

        public int? TaskStateBeforeCustomerCancelOrder { get; set; }

        // Navigation Property

        [CanBeNull]
        public virtual RFQBid RFQBid { get; set; }
        
        [CanBeNull]
        public virtual Product Product { get; set; }

        public virtual ICollection<OmnaeInvoice> Invoices { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

        public virtual ICollection<NCReport> NCReports { get; set; }

        //////////

        [CanBeNull]
        public virtual SimplifiedUser CreatedByUser { get; set; }

        [CanBeNull]
        public virtual SimplifiedUser ModifiedByUser { get; set; }
    }
}