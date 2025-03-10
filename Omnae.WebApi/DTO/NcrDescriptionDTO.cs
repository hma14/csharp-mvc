using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Omnae.BusinessLayer.Models;
using Omnae.Common;

namespace Omnae.WebApi.DTO
{
    public class NcrDescriptionDTO
    {
        public int NCRId { get; set; }
        
        [Required]
        public int? OrderId { get; set; }
        [Required]
        public int? TaskId { get; set; }
        [Required]
        public int? ProductId { get; set; }
        
        public string ProductName { get; set; }
        public string ProductAvatarUri { get; set; }

        [Required]
        public States? StateId { get; set; }

        public int? CustomerId { get; set; }
        public int? VendorId { get; set; }

        [Display(Name = "Vendor / Supplier")]
        public string Vendor { get; set; }
        
        public string Customer { get; set; }

        public string ProductPartNo { get; set; }
        public string PartRevisionNo { get; set; }

        [Display(Name = "Product Description")]
        public string ProductDescription { get; set; }

        [Display(Name = "Affected Quantity")]
        public int? Quantity { get; set; }

        [Display(Name = "Total Product Quantity")]
        public int? TotalProductQuantity { get; set; }

        [Display(Name = "PO No.")]
        public string PONumber { get; set; }


        [Display(Name = "NC Originator")]
        public string NCOriginator { get; set; }

        [Display(Name = "NCR Detected By")]
        public string NCDetectedby { get; set; }

        public DateTime? NCDetectedDate { get; set; }
        public DateTime? DateNcrClosed { get; set; }

        [Display(Name = "NC Description")]
        public string NCDescription { get; set; }

        public string Expectation { get; set; }

        [Display(Name ="Evidence Image Url")]
        public List<string> EvidenceImageUrl { get; set; }

        public NC_ROOT_CAUSE? RootCause { get; set; }

        [Display(Name = "Detailed Root Cause")]
        public string RootCauseFurtherDetails { get; set; }
        public decimal? Cost { get; set; }
        public NC_DISPOSITION? Disposition { get; set; }

        [Display(Name = "Prevent Reoccurrence")]
        public string ActionTakenDetails { get; set; }

        [Display(Name = "Corrective Action")]
        public string CorrectiveAction { get; set; }

        [Display(Name = "Action Taken Verified By")]
        public string ActionTakenVerifiedBy { get; set; }

        [Display(Name = "Action Taken Verified Date")]
        public DateTime? ActionTakenVerifiedDate { get; set; }

        [Display(Name = "Reject Corrective Action Reason")]
        public string RejectCorrectiveActionReason { get; set; }

        [Display(Name = "Reject Corrective Parts Reason")]
        public string RejectCorrectivePartsReason { get; set; }

        [Display(Name = "Reject Root Cause Reason")]
        public string RejectRootCauseReason { get; set; }

        [Display(Name = "Carrier Name")]
        public string CarrierName { get; set; }
        public SHIPPING_CARRIER_TYPE? CarrierType { get; set; }

        public string ShippingAccountNumber { get; set; }

        [Display(Name = "Tracking Number")]
        public string TrackingNumber { get; set; }

        [Display(Name = "Arbitrate Customer Cause Reason")]
        public string ArbitrateCustomerCauseReason { get; set; }
        public List<string> ArbitrateCustomerCauseImageRefUrl { get; set; }
        public string ArbitrateCustomerDamageReason { get; set; }
        public List<string> ArbitrateCustomerDamageImageRefUrl { get; set; }


        [Display(Name = "Arbitrate Vendor Cause Reason")]
        public string ArbitrateVendorCauseReason { get; set; }

        public List<string> ArbitrateVendorCauseImageRefUrl { get; set; }


        [Display(Name = "Root Cause On Customer Reason")]
        public string RootCauseOnCustomerReason { get; set; }
        public List<string> RootCauseOnCustomerImageRefUrl { get; set; }

        public string RootCauseOnVendorReason { get; set; }
        public List<string> RootCauseOnVendorImageRefUrl { get; set; }

        public string NCRNumber { get; set; }
        public string NCRNumberForVendor { get; set; }
        public NcrInfoViewModel NcrHistory { get; set; }

        public List<string> PackingSlipInspectionReport { get; set; }

        public List<string> NCRImageRefUrl { get; set; }


    }
}