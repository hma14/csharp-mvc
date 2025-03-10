using Omnae.Common;
using Omnae.Model.Models;
using Stateless;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Omnae.BusinessLayer.Models;

namespace Omnae.ViewModels
{
    public class StateTransitionViewModel
    {
        public string group { get; set; }
        public TaskData TaskData { get; set; }
        public StateMachine<States, Triggers> StTransition { get; set; }

        [Display(Name = "Carrier Name")]
        public string CarrierName { get; set; }

        [Display(Name = "Tracking Number")]
        public string TrackingNumber { get; set; }

        // NCR required
        public int ProductId { get; set; }
        public NC_DISPOSITION? Disposition { get; set; }

        [Display(Name = "Root Cause")]
        public NC_ROOT_CAUSE RootCause { get; set; }

        [Display(Name = "Detailed Root Cause")]
        public string DetailRootCause { get; set; }

        [Display(Name = "Prevent Reoccurrence")]
        public string ActionTaken { get; set; }

        [Display(Name = "Corrective Action")]
        public string CorrectiveAction { get; set; }

        [Display(Name = "Action and Verification Deadline")]
        public string ActionTakenBy { get; set; }

        [Display(Name = "Estimate Completion Date")]
        public DateTime? EstimateCompletionDate { get; set; }
        public DateTime? Date { get; set; }

        public string CustomerNCDescription { get; set; }

        public NcrDescriptionViewModel NcrDescriptionVM { get; set; }

        public RFQViewModel RFQVM { get; set; }

        public bool? HideSubmitButton  { get; set; }

        public VendorInvoiceViewModel VendorInvoiceVM { get; set; }

        [Display(Name = "Reject Reason Doc")]
        public HttpPostedFileBase RejectReasonDoc { get; set; }

        public HttpPostedFileBase[] PackingSlipInspectionReport { get; set; }

        public PackingSlipViewModel PackingSlip { get; set; }
        public string PackingSlipUri { get; set; }

        [Display(Name = "Enter Attached Invoice #")]
        public string AttachedInvoiceNumber { get; set; }

        public USER_TYPE UserType { get; set; }
        public Func<bool> MyFunc { get; set; }
        public string EnumName { get; set; }
        public List<Document> RevisingDocs { get; set; }
        public List<Document> PackingSlipDocs { get; set; }

    }
}