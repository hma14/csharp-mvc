using Omnae.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Omnae.Model.Models
{
    public class NCReport
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public States? StateId { get; set; }
        public int? NCRootCauseCompanyId { get; set; }

        [ForeignKey("CustomerCompany")]
        public int? CustomerId { get; set; }
        public int? VendorId { get; set; }

        public DateTime? NCDetectedDate { get; set; }
        public int? Quantity { get; set; }

        [StringLength(50)]
        public string NCOriginator { get; set; }

        [StringLength(50)]
        public string NCDetectedby { get; set; }

        public string NCDescription { get; set; }
        public string Expectation { get; set; }
        public NC_ROOT_CAUSE? RootCause { get; set; }
        public string RootCauseFurtherDetails { get; set; }
        public decimal? Cost { get; set; }
        public NC_DISPOSITION? Disposition { get; set; }
        public string ActionTakenDetails { get; set; } //Prevention Plan
        public string CorrectiveAction { get; set; }
        public string ActionTakenVerifiedBy { get; set; }
        public DateTime? ActionTakenVerifiedDate { get; set; }

        public string RejectCorrectiveActionReason { get; set; }
        public string RejectRootCauseReason { get; set; }

        public string RejectCorrectivePartsReason { get; set; }

        [Display(Name = "Carrier Name")]
        [StringLength(25)]
        public string CarrierName { get; set; }

        [Display(Name = "Tracking Number")]
        [StringLength(25)]
        public string TrackingNumber { get; set; }

        public DateTime? DateNcrClosed { get; set; }

        public string ArbitrateCustomerCauseReason { get; set; }

        public string ArbitrateVendorCauseReason { get; set; }

        public string RootCauseOnCustomerReason { get; set; }

        public string NCRNumber { get; set; }
        public string NCRNumberForVendor { get; set; }

        public int? TaskId { get; set; }

        public DateTime? NCRApprovalDate { get; set; }
        public DateTime? RootCauseAnalysisDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime? _createdAt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime? _updatedAt { get; set; }


        // Navigation Property

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }

        [CanBeNull]
        public virtual TaskData Task { get; set; }
        [CanBeNull]
        public virtual Company CustomerCompany { get; set; }

    }
}
