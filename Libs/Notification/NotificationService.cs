using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using Libs.Notification;
using Libs.ViewModels;
using Omnae.Common;
using Omnae.Libs.ViewModel;
using Omnae.Libs.ViewModels;
using Omnae.Notification;
using Omnae.Notification.Model;
using RazorEngine;
using RazorEngine.Templating;

namespace Omnae.Libs.Notification
{
    public class NotificationService
    {
        private IEmailSender EmailSender { get; }
        private ISmsSender SmsSender { get; }

        public NotificationService(IEmailSender emailSender, ISmsSender smsSender)
        {
            EmailSender = emailSender;
            SmsSender = smsSender;
        }

        public void NotifyStateChange(States state, string subject, string destEmail, string destSms, BaseViewModel model, bool isVendor = false, bool isAdmin = false)
        {
            bool toAccounting = false;

            List<byte[]> attachmentData = null;
            List<string> attachmentName = null;
            List<string> toEmails = null;

            var filename = state switch
            {
                States.PendingRFQ => "~/Template/PendingRFQEmail.html",
                States.PendingRFQRevision => "~/Template/PendingRFQRevisionEmail.html",
                States.RFQRevision => "~/Template/RevisionRequestedEmail.html",
                States.BidForRFQ => "~/Template/BidForRFQEmail.html",
                States.BidReview when isAdmin => "~/Template/BidReviewEmailAdmin.html",
                States.BidReview when !isAdmin => "~/Template/BidReviewEmail.html",
                States.BidTimeout => "~/Template/BidTimeoutEmail.html",
                States.BackFromRFQ => "~/Template/BackFromRFQEmail.html",
                States.QuoteAccepted => "~/Template/QuoteCompleteEmail.html",
                States.OrderPaid when isVendor => "~/Template/OrderConfirmationEmailVendor.html",
                States.OrderPaid when !isVendor => "~/Template/OrderConfirmationEmail.html",
                States.ReOrderPaid when isVendor => "~/Template/OrderConfirmationEmailVendor.html",
                States.ReOrderPaid when !isVendor => "~/Template/ReOrderConfirmationEmail.html",
                States.ProofingComplete => "~/Template/ProofCompleteEmail.html",
                States.ProofApproved => "~/Template/ProofApprovedEmail.html",
                States.SampleStarted => "~/Template/ToolingCompleteEmail.html",
                States.SampleComplete => "~/Template/SampleCompleteEmail.html",
                States.SampleApproved => "~/Template/SampleApprovedEmail.html",
                States.ProductionStarted => "~/Template/ProductionStartedEmail.html",
                States.VendorPendingInvoice => "~/Template/ProductionCompleteEmail.html",
                States.ProductionComplete => "~/Template/ProductionCompleteEmail.html",
                States.OrderInitiated => "~/Template/OrderInitiatedEmail.html",
                States.ReOrderInitiated => "~/Template/OrderInitiatedEmail.html",
                States.PaymentMade => "~/Template/PaymentMadeEmail.html",
                States.ReOrderPaymentMade => "~/Template/PaymentMadeEmail.html",
                States.BidReviewed => "~/Template/BidReviewedEmail.html",
                States.AddExtraQuantities => "~/Template/AddExtraQtyEmail.html",
                States.SetupMarkupExtraQty => "~/Template/SetupMarkupExtraQtyEmail.html",
                States.NCRVendorRootCauseAnalysis when isAdmin => "~/Template/NCRArbitrateEmail.html",
                States.NCRVendorRootCauseAnalysis when !isAdmin => "~/Template/NCRCustomerStartedEmail.html",
                States.NCRDamagedByCustomer when isAdmin => "~/Template/NCRArbitrateEmail.html",
                States.NCRDamagedByCustomer when !isAdmin => "~/Template/NCRCustomerStartedEmail.html",
                States.NCRCustomerRevisionNeeded when isAdmin => "~/Template/NCRArbitrateEmail.html",
                States.NCRCustomerRevisionNeeded when !isAdmin => "~/Template/NCRCustomerStartedEmail.html",
                States.NCRCustomerApproval => "~/Template/NCRCustomerApprovalEmail.html",
                States.NCRVendorCorrectivePartsInProduction => "~/Template/NCRVendorCorrectivePartsInProductionEmail.html",
                States.NCRRootCauseDisputes => "~/Template/NCRRootCauseDisputesEmail.html",
                States.NCRAdminDisputesIntervention => "~/Template/NCRAdminDisputesInterventionEmail.html",
                States.NCRCustomerRejectCorrective => "~/Template/NCRCustomerRejectCorrectiveEmail.html",
                States.NCRCustomerRejectRootCause => "~/Template/NCRCustomerRejectRootCauseEmail.html",
                States.NCRVendorCorrectivePartsComplete => "~/Template/NCRVendorCorrectivePartsCompleteEmail.html",
                States.NCRCustomerCorrectivePartsReceived => "~/Template/NCRCustomerCorrectivePartsReceivedEmail.html",
                States.NCRCustomerCorrectivePartsAccepted => "~/Template/NCRCustomerCorrectivePartsAcceptedEmail.html",
                States.NCRClosed => "~/Template/NCRClosedEmail.html",
                States.VendorCancelledRFQ => "~/Template/VendorCancelledRFQEmail.html",
                States.CustomerCancelledRFQ => "~/Template/CustomerCancelledRFQEmail.html",
                States.ProofRejected => "~/Template/ProofRejectedEmail.html",
                States.SampleRejected => "~/Template/SampleRejectedEmail.html",
                _ => throw new Exception($"Unknown state: {Enum.GetName(typeof(States), state)}")
            };

            switch (state)
            {
                case States.SampleComplete:
                case States.VendorPendingInvoice:
                case States.ProductionComplete:
                    toAccounting = true;
                    var sampleCompleteViewModel = (SampleCompleteViewModel)model;

                    toEmails = sampleCompleteViewModel.QboEmails;
                    attachmentData = sampleCompleteViewModel.InvoiceData;
                    attachmentName = sampleCompleteViewModel.InvoiceName?.Count > 1 && sampleCompleteViewModel.InvoiceData?.Count == 1
                                    ? new List<string> {sampleCompleteViewModel.InvoiceName.Last()}
                                    : sampleCompleteViewModel.InvoiceName;
                    break;
            }

            var template = File.ReadAllText(HttpContext.Current.Server.MapPath(filename));
            var body = Engine.Razor.RunCompile(template, null, model);
            var message = new EmailMessage() { Subject = subject, DestinationEmail = destEmail, Body = body , ToAccounting = toAccounting, ToEmails = toEmails, AttachData = attachmentData, AttachName = attachmentName};

            EmailSender.SendEmail(message);
        }

        public void NotifyAdminOrder(string subject, string destination, string destinationSms, NotifyAdminOrderEmailViewModel model)
        {
            var template = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Template/OrderNotifyAdminEmail.html"));

            //Confirmation email to the customer including: Part number, quantity, price paid, lead time, production files attached.
            var body = Engine.Razor.RunCompile(template, null, model);
            var message = new EmailMessage() { Subject = subject, DestinationEmail = destination, Body = body };

            EmailSender.SendEmail(message);
        }

        public void NotifyConfirmEmail(string subject, string destination, string destinationSms, AccountConfirmEmailViewModel model)
        {
            var template = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Template/AccountConfirmEmail.html"));

            var body = Engine.Razor.RunCompile(template, null, model);
            var message = new EmailMessage() { Subject = subject, DestinationEmail = destination, Body = body };

            EmailSender.SendEmail(message);
        }

        public void NotifyCreateNewPart(string subject, string destination, string destinationSms, CreateNewPartViewModel model)
        {
            var template = model.IsCustomer
                            ? File.ReadAllText(HttpContext.Current.Server.MapPath("~/Template/CreateNewPartEmail.html"))
                            : File.ReadAllText(HttpContext.Current.Server.MapPath("~/Template/CreateNewPartEmailVendor.html"));

            var body = Engine.Razor.RunCompile(template, null, model);
            var message = new EmailMessage() { Subject = subject, DestinationEmail = destination, Body = body };

            EmailSender.SendEmail(message);
        }

        public void NotifyContact(string sender, string from, string subject, string content)
        {
            var destination = ConfigurationManager.AppSettings["EMAIL_TO_OMNAE"];
            EmailSender.SendNormalEmail(sender, subject, @from, destination, content);
        }
    }
}
