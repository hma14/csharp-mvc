using Stateless;
using System;
using System.Collections.Generic;

namespace Omnae.Common
{
    public class Utils
    {
        public static void RegisterStates(StateMachine<States, Triggers> stateTransition, Func<bool> CheckPreconditions)
        {
            stateTransition.Configure(States.PendingRFQ)
                //.PermitReentry(Triggers.RFQCancelled)
                .Permit(Triggers.VendorReviewRFQ, States.ReviewRFQ);

            stateTransition.Configure(States.ReviewRFQ)
                .Permit(Triggers.RFQCancelled, States.PendingRFQ)
                .Permit(Triggers.RevisionRequired, States.PendingRFQRevision)
                .Permit(Triggers.VendorAcceptRFQ, States.ReviewRFQAccepted)
                .Permit(Triggers.VendorRejectRFQ, States.VendorRejectedRFQ);


            stateTransition.Configure(States.RFQReviewUpdateQuantity)
                .Permit(Triggers.RFQCancelled, States.PendingRFQ)
                .Permit(Triggers.RevisionRequired, States.PendingRFQRevision)
                .Permit(Triggers.VendorAcceptRFQ, States.ReviewRFQAccepted)
                .Permit(Triggers.VendorRejectRFQ, States.VendorRejectedRFQ);


            stateTransition.Configure(States.BidForRFQ)
                .Permit(Triggers.ReadyToOrder, States.BidReview)
                .Permit(Triggers.VendorRejectRFQ, States.VendorRejectedRFQ);

            stateTransition.Configure(States.RFQBidUpdateQuantity)
                .Permit(Triggers.ReadyToOrder, States.BidReview)
                .Permit(Triggers.VendorRejectRFQ, States.VendorRejectedRFQ);

            stateTransition.Configure(States.OutForRFQ)
                .Permit(Triggers.RevisionRequired, States.PendingRFQRevision)
                .Permit(Triggers.ReadyToOrder, States.BidReview);

            stateTransition.Configure(States.PendingRFQRevision)
                .Permit(Triggers.RevisingRFQ, States.ReviewRFQ);


            stateTransition.Configure(States.BackFromRFQ)
                .Permit(Triggers.RevisingRFQ, States.ReviewRFQ);


            //stateTransition.Configure(States.RFQRevision)
            //    .Permit(Triggers.RevisionRequired, States.PendingRFQRevision)
            //    .Permit(Triggers.RFQCancelled, States.PendingRFQ)
            //    .Permit(Triggers.ReadyToOrder, States.BidReview);

            stateTransition.Configure(States.QuoteAccepted)
                .Permit(Triggers.ReadyToOrder, States.OrderInitiated);

            stateTransition.Configure(States.AddExtraQuantities)
                .Permit(Triggers.SetupUnitPricesForExtraQuantities, States.SetupMarkupExtraQty);

            stateTransition.Configure(States.SetupMarkupExtraQty)
                .Permit(Triggers.SetupMarkupForExtraQty, States.QuoteAccepted);

            stateTransition.Configure(States.OrderInitiated)
                .Permit(Triggers.CustomerCancelOrderRequest, States.CustomerCancelOrder)
                .Permit(Triggers.PendingPaymentMade, States.PaymentMade);

            stateTransition.Configure(States.PaymentMade)
                .Permit(Triggers.CustomerCancelOrderRequest, States.CustomerCancelOrder)
                .Permit(Triggers.OrderPaid, States.OrderPaid);

            stateTransition.Configure(States.OrderPaid)
                .Permit(Triggers.StartedProof, States.ProofingStarted);

            stateTransition.Configure(States.ProofingStarted)
                .PermitIf(Triggers.ProofApproval, States.ProofingComplete, CheckPreconditions);

            stateTransition.Configure(States.ProofingComplete)
                .Permit(Triggers.CustomerCancelOrderRequest, States.CustomerCancelOrder)
                .Permit(Triggers.ApprovedProof, States.ProofApproved)
                .Permit(Triggers.ProofRejected, States.ProofRejected);

            stateTransition.Configure(States.ProofApproved)
                .Permit(Triggers.ReadyForTooling, States.ToolingStarted);

            stateTransition.Configure(States.ProofRejected)
                .Permit(Triggers.CorrectingProof, States.ProofingStarted);


            stateTransition.Configure(States.ToolingStarted)
                .Permit(Triggers.ToolingComplete, States.SampleStarted);

            stateTransition.Configure(States.SampleStarted)
                .Permit(Triggers.CompleteSample, States.SampleComplete);

            stateTransition.Configure(States.SampleComplete)
                .Permit(Triggers.CustomerCancelOrderRequest, States.CustomerCancelOrder)
                .Permit(Triggers.ApproveSample, States.SampleApproved)
                .Permit(Triggers.RejectedSample, States.SampleRejected);

            stateTransition.Configure(States.SampleApproved)
                .Permit(Triggers.InProduction, States.ProductionStarted);

            stateTransition.Configure(States.SampleRejected)
                .Permit(Triggers.CorrectingSample, States.ToolingStarted);


            stateTransition.Configure(States.ProductionStarted)
                .Permit(Triggers.CompleteProduction, States.ProductionComplete);

            //stateTransition.Configure(States.ProductionComplete)
            //    //.Permit(Triggers.PendingReorderPaymentMade, States.ReOrderInitiated)  // ??
            //    //.Permit(Triggers.NCRStarted, States.NCRCustomerStarted)
            //    .PermitIf(Triggers.CreateInvoiceForVendor, States.VendorPendingInvoice, CheckPreconditions);

            // Customer Cancel Order
            stateTransition.Configure(States.CustomerCancelOrder)
                .Permit(Triggers.VendorAcceptCancelOrderRequest, States.OrderCancelled)
                .Permit(Triggers.VendorDenyCancelOrderRequest, States.OrderCancelDenied);



            stateTransition.Configure(States.ReOrderInitiated)
                .Permit(Triggers.PendingReorderPaymentMade, States.ReOrderPaymentMade);

            stateTransition.Configure(States.ReOrderPaymentMade)
                 .Permit(Triggers.PaidReOrder, States.ReOrderPaid);


            stateTransition.Configure(States.ReOrderPaid)
                .Permit(Triggers.InProduction, States.ProductionStarted);



            // NCR
            stateTransition.Configure(States.NCRCustomerStarted)
                .Permit(Triggers.NCRAnalysisRootCause, States.NCRVendorRootCauseAnalysis);

            stateTransition.Configure(States.NCRVendorRootCauseAnalysis)  //Vendor Action
                .Permit(Triggers.NCRApproval, States.NCRCustomerApproval) //Vendor Approval
                .Permit(Triggers.NCRRootCauseOnCustomer, States.NCRAdminDisputesIntervention); // Jump state to NCRAdminDisputesIntervention
                                                                                               //.Permit(Triggers.NCRRootCauseOnCustomer, States.NCRRootCauseDisputes); //Vendor Decline


            stateTransition.Configure(States.NCRCustomerApproval)
                .Permit(Triggers.NCRCorrectiveReceivedApproved, States.NCRVendorCorrectivePartsInProduction)
                .Permit(Triggers.NCRRejectCorrectiveAction, States.NCRCustomerRejectCorrective)
                .Permit(Triggers.NCRRejectRootCause, States.NCRCustomerRejectRootCause);

            stateTransition.Configure(States.NCRVendorCorrectivePartsInProduction)
                .Permit(Triggers.NCRCorrectivePartsComplete, States.NCRVendorCorrectivePartsComplete);

            stateTransition.Configure(States.NCRVendorCorrectivePartsComplete)
                .Permit(Triggers.NCRCorrectiveAccepted, States.NCRCustomerCorrectivePartsAccepted)
                .Permit(Triggers.NCRCorrectiveReceivedRejected, States.NCRVendorCorrectivePartsInProduction);

            stateTransition.Configure(States.NCRCustomerRejectCorrective)
                .Permit(Triggers.NCRArbitrateVendorCause, States.NCRVendorRootCauseAnalysis)
                .Permit(Triggers.NCRArbitrateCustomerCause, States.NCRCustomerRevisionNeeded);

            stateTransition.Configure(States.NCRCustomerRejectRootCause)
                .Permit(Triggers.NCRArbitrateVendorCause, States.NCRVendorRootCauseAnalysis)
                .Permit(Triggers.NCRArbitrateCustomerCause, States.NCRCustomerRevisionNeeded);

            stateTransition.Configure(States.NCRRootCauseDisputes)
                .Permit(Triggers.NCRArbitrateDispute, States.NCRAdminDisputesIntervention);

            stateTransition.Configure(States.NCRAdminDisputesIntervention)
                .Permit(Triggers.NCRArbitrateVendorCause, States.NCRVendorRootCauseAnalysis)
                .Permit(Triggers.NCRArbitrateCustomerCauseDamage, States.NCRDamagedByCustomer)
                .Permit(Triggers.NCRArbitrateCustomerCause, States.NCRCustomerRevisionNeeded);

            stateTransition.Configure(States.NCRDamagedByCustomer)
                .Permit(Triggers.NCRClose, States.NCRClosed);

            stateTransition.Configure(States.NCRCustomerRevisionNeeded)
                .Permit(Triggers.NCRCustomerRevision, States.NCRClosed);

            stateTransition.Configure(States.NCRCustomerCorrectivePartsReceived)
               .Permit(Triggers.NCRCorrectiveReceivedRejected, States.NCRCustomerRejectCorrective)
               .Permit(Triggers.NCRCorrectiveReceivedApproved, States.NCRCustomerCorrectivePartsAccepted);

            stateTransition.Configure(States.NCRCustomerCorrectivePartsAccepted)
                .Permit(Triggers.NCRClose, States.NCRClosed);

            //stateTransition.Configure(States.NCRClosed)
            //    .Permit(Triggers.RevisionRequired, States.BackFromRFQ)
            //    .Permit(Triggers.CompleteProduction, States.ProductionComplete);

            ///END of NCR

            // Create Invoice
            stateTransition.Configure(States.VendorPendingInvoice)
                .Permit(Triggers.CompleteInvoiceForVendor, States.ProductionComplete);
        }

        public static void RegisterStates_Reseller(StateMachine<States, Triggers> stateTransition, Func<bool> CheckPreconditions)
        {
            stateTransition.Configure(States.PendingRFQ)
                //.PermitReentry(Triggers.RFQCancelled)
                .Permit(Triggers.BiddingRFQ, States.BidForRFQ);

            stateTransition.Configure(States.BidForRFQ)
                .Permit(Triggers.RevisionRequired, States.PendingRFQRevision)
                .Permit(Triggers.ReadyToOrder, States.BidReview);
            //.Permit(Triggers.ReviewRFQBid, States.BidReview);

            //stateTransition.Configure(States.BidReview)
            //    .Permit(Triggers.AssignRFQ, States.OutForRFQ);
            //.Permit(Triggers.CompleteRFQBid, States.RFQBidComplete);

            stateTransition.Configure(States.OutForRFQ)
                .Permit(Triggers.RevisionRequired, States.PendingRFQRevision)
                .Permit(Triggers.ReadyToOrder, States.BidReview);
            //.Permit(Triggers.RevisingRFQ, States.BidForRFQ);
            //.Permit(Triggers.RFQCancelled, States.PendingRFQ)
            //.Permit(Triggers.ReadyToOrder, States.QuoteAccepted);

            stateTransition.Configure(States.PendingRFQRevision)
                .Permit(Triggers.RevisingRFQ, States.RFQRevision);
            //.Permit(Triggers.ReadyToOrder, States.QuoteAccepted);


            stateTransition.Configure(States.BackFromRFQ)
                .Permit(Triggers.RevisingRFQ, States.BidForRFQ);


            stateTransition.Configure(States.RFQRevision)
                .Permit(Triggers.RevisionRequired, States.PendingRFQRevision)
                .Permit(Triggers.RFQCancelled, States.PendingRFQ)
                .Permit(Triggers.ReadyToOrder, States.BidReview);

            stateTransition.Configure(States.QuoteAccepted)
                .Permit(Triggers.ReadyToOrder, States.OrderInitiated);

            stateTransition.Configure(States.AddExtraQuantities)
                .Permit(Triggers.SetupUnitPricesForExtraQuantities, States.SetupMarkupExtraQty);

            stateTransition.Configure(States.SetupMarkupExtraQty)
                .Permit(Triggers.SetupMarkupForExtraQty, States.QuoteAccepted);

            stateTransition.Configure(States.OrderInitiated)
                .Permit(Triggers.CustomerCancelOrderRequest, States.CustomerCancelOrder)
                .Permit(Triggers.PendingPaymentMade, States.PaymentMade);

            stateTransition.Configure(States.PaymentMade)
                .Permit(Triggers.CustomerCancelOrderRequest, States.CustomerCancelOrder)
                .Permit(Triggers.OrderPaid, States.OrderPaid);

            stateTransition.Configure(States.OrderPaid)
                .Permit(Triggers.StartedProof, States.ProofingStarted);

            stateTransition.Configure(States.ProofingStarted)
                .PermitIf(Triggers.ProofApproval, States.ProofingComplete, CheckPreconditions);

            stateTransition.Configure(States.ProofingComplete)
                .Permit(Triggers.CustomerCancelOrderRequest, States.CustomerCancelOrder)
                .Permit(Triggers.ApprovedProof, States.ProofApproved)
                .Permit(Triggers.ProofRejected, States.ProofRejected);

            stateTransition.Configure(States.ProofApproved)
                .Permit(Triggers.ReadyForTooling, States.ToolingStarted);

            stateTransition.Configure(States.ProofRejected)
                .Permit(Triggers.CorrectingProof, States.ProofingStarted);


            stateTransition.Configure(States.ToolingStarted)
                .Permit(Triggers.ToolingComplete, States.SampleStarted);

            stateTransition.Configure(States.SampleStarted)
                .Permit(Triggers.CompleteSample, States.SampleComplete);

            stateTransition.Configure(States.SampleComplete)
                .Permit(Triggers.CustomerCancelOrderRequest, States.CustomerCancelOrder)
                .Permit(Triggers.ApproveSample, States.SampleApproved)
                .Permit(Triggers.RejectedSample, States.SampleRejected);

            stateTransition.Configure(States.SampleApproved)
                .Permit(Triggers.InProduction, States.ProductionStarted);

            stateTransition.Configure(States.SampleRejected)
                .Permit(Triggers.CorrectingSample, States.ToolingStarted);


            stateTransition.Configure(States.ProductionStarted)
                .Permit(Triggers.CompleteProduction, States.ProductionComplete);

            //stateTransition.Configure(States.ProductionComplete)
            //    //.Permit(Triggers.PendingReorderPaymentMade, States.ReOrderInitiated)  // ??
            //    //.Permit(Triggers.NCRStarted, States.NCRCustomerStarted)
            //    .PermitIf(Triggers.CreateInvoiceForVendor, States.VendorPendingInvoice, CheckPreconditions);

            // Customer Cancel Order
            stateTransition.Configure(States.CustomerCancelOrder)
                .Permit(Triggers.VendorAcceptCancelOrderRequest, States.OrderCancelled)
                .Permit(Triggers.VendorDenyCancelOrderRequest, States.OrderCancelDenied);



            stateTransition.Configure(States.ReOrderInitiated)
                .Permit(Triggers.PendingReorderPaymentMade, States.ReOrderPaymentMade);

            stateTransition.Configure(States.ReOrderPaymentMade)
                 .Permit(Triggers.PaidReOrder, States.ReOrderPaid);


            stateTransition.Configure(States.ReOrderPaid)
                .Permit(Triggers.InProduction, States.ProductionStarted);



            // NCR
            stateTransition.Configure(States.NCRCustomerStarted)
                .Permit(Triggers.NCRAnalysisRootCause, States.NCRVendorRootCauseAnalysis);

            stateTransition.Configure(States.NCRVendorRootCauseAnalysis)  //Vendor Action
                .Permit(Triggers.NCRApproval, States.NCRCustomerApproval) //Vendor Approval
                .Permit(Triggers.NCRRootCauseOnCustomer, States.NCRAdminDisputesIntervention); // Jump state to NCRAdminDisputesIntervention
                                                                                               //.Permit(Triggers.NCRRootCauseOnCustomer, States.NCRRootCauseDisputes); //Vendor Decline


            stateTransition.Configure(States.NCRCustomerApproval)
                .Permit(Triggers.NCRCorrectiveReceivedApproved, States.NCRVendorCorrectivePartsInProduction)
                .Permit(Triggers.NCRRejectCorrectiveAction, States.NCRCustomerRejectCorrective)
                .Permit(Triggers.NCRRejectRootCause, States.NCRCustomerRejectRootCause);

            stateTransition.Configure(States.NCRVendorCorrectivePartsInProduction)
                .Permit(Triggers.NCRCorrectivePartsComplete, States.NCRVendorCorrectivePartsComplete);

            stateTransition.Configure(States.NCRVendorCorrectivePartsComplete)
                .Permit(Triggers.NCRCorrectiveAccepted, States.NCRCustomerCorrectivePartsAccepted)
                .Permit(Triggers.NCRCorrectiveReceivedRejected, States.NCRVendorCorrectivePartsInProduction);

            stateTransition.Configure(States.NCRCustomerRejectCorrective)
                .Permit(Triggers.NCRArbitrateVendorCause, States.NCRVendorRootCauseAnalysis)
                .Permit(Triggers.NCRArbitrateCustomerCause, States.NCRCustomerRevisionNeeded);

            stateTransition.Configure(States.NCRCustomerRejectRootCause)
                .Permit(Triggers.NCRArbitrateVendorCause, States.NCRVendorRootCauseAnalysis)
                .Permit(Triggers.NCRArbitrateCustomerCause, States.NCRCustomerRevisionNeeded);

            stateTransition.Configure(States.NCRRootCauseDisputes)
                .Permit(Triggers.NCRArbitrateDispute, States.NCRAdminDisputesIntervention);

            stateTransition.Configure(States.NCRAdminDisputesIntervention)
                .Permit(Triggers.NCRArbitrateVendorCause, States.NCRVendorRootCauseAnalysis)
                .Permit(Triggers.NCRArbitrateCustomerCauseDamage, States.NCRDamagedByCustomer)
                .Permit(Triggers.NCRArbitrateCustomerCause, States.NCRCustomerRevisionNeeded);

            stateTransition.Configure(States.NCRDamagedByCustomer)
                .Permit(Triggers.NCRClose, States.NCRClosed);

            stateTransition.Configure(States.NCRCustomerRevisionNeeded)
                .Permit(Triggers.NCRCustomerRevision, States.NCRClosed);

            stateTransition.Configure(States.NCRCustomerCorrectivePartsReceived)
               .Permit(Triggers.NCRCorrectiveReceivedRejected, States.NCRCustomerRejectCorrective)
               .Permit(Triggers.NCRCorrectiveReceivedApproved, States.NCRCustomerCorrectivePartsAccepted);

            stateTransition.Configure(States.NCRCustomerCorrectivePartsAccepted)
                .Permit(Triggers.NCRClose, States.NCRClosed);

            //stateTransition.Configure(States.NCRClosed)
            //    .Permit(Triggers.RevisionRequired, States.BackFromRFQ)
            //    .Permit(Triggers.CompleteProduction, States.ProductionComplete);

            ///END of NCR

            // Create Invoice
            stateTransition.Configure(States.VendorPendingInvoice)
                .Permit(Triggers.CompleteInvoiceForVendor, States.ProductionComplete);
        }

        public static Dictionary<States, string> ConstructStateIconMapping()
        {
            Dictionary<States, string> dic = new Dictionary<States, string>
            {
                { States.PendingRFQ, "<i class=\"far fa-clock fa-2x\" ></i>" },
                { States.PendingRFQRevision, "<i class=\"far fa-edit fa-2x\" ></i>" },
                { States.BidTimeout, "<i class=\"fas fa-hourglass-start fa-2x\" ></i>" },
                { States.BidForRFQ, "<i class=\"fas fa-wrench fa-2x\" ></i>" },
                { States.BidReview, "<i class=\"fas fa-check-square fa-2x\" ></i>" },
                { States.RFQRevision, "<i class=\"fas fa-pencil-alt fa-2x\" ></i>" },
                { States.RFQBidComplete, "<i class=\"fas fa-times-circle fa-2x\" ></i>" },
                { States.OutForRFQ, "<i class=\"fas fa-sign-out-alt fa-2x\" ></i>" },

                { States.BackFromRFQ, "<i class=\"fas fa-sign-in-alt fa-2x\" ></i>" },

                { States.VendorCancelledRFQ, "<i class=\"fas fa-trash-alt fa-2x\" ></i>" },
                { States.AddExtraQuantities, "<i class=\"fas fa-cart-plus fa-2x\" ></i>" },
                { States.QuoteAccepted, "<i class=\"far fa-check-circle fa-2x\" ></i>" },
                { States.OrderInitiated, "<i class=\"far fa-paper-plane fa-2x\" ></i>" },
                { States.PaymentMade, "<i class=\"fab fa-monero fa-2x\" ></i>" },
                { States.OrderPaid, "<i class=\"fab fa-monero fa-2x\" ></i>" },
                { States.ReOrderPaid, "<i class=\"fab fa-monero fa-2x\" ></i>" },
                { States.ProofingStarted, "<i class=\"fas fa-hourglass-start fa-2x\" ></i>" },
                { States.ProofingComplete, "<i class=\"far fa-calendar-check fa-2x\" ></i>" },
                { States.ProofApproved, "<i class=\"fas fa-thumbs-up fa-2x\" ></i>" },
                { States.ProofRejected, "<i class=\"fas fa-thumbs-down fa-2x\" ></i>" },
                { States.ToolingStarted, "<i class=\"fas fa-stopwatch fa-2x\" ></i>" },
                { States.SampleStarted, "<i class=\"fas fa-clock fa-2x\" ></i>" },
                { States.SampleComplete, "<i class=\"fas fa-calendar-check fa-2x\" ></i>" },
                { States.SampleApproved, "<i class=\"far fa-thumbs-up fa-2x\" ></i></i>" },
                { States.SampleRejected, "<i class=\"far fa-thumbs-down fa-2x\" ></i>" },
                { States.ReOrderInitiated, "<i class=\"fas fa-redo-alt fa-2x\" ></i>" },
                { States.ReOrderPaymentMade, "<i class=\"fas fa-dollar-sign fa-2x\" ></i>" },
                { States.ProductionStarted, "<i class=\"far fa-play-circle fa-2x\" ></i>" },
                { States.ProductionComplete, "<i class=\"far fa-check-square fa-2x\" ></i>" },

                // NCR
                { States.NCRCustomerStarted, "<i class=\"fas fa-bug fa-2x\" ></i>" },
                { States.NCRVendorRootCauseAnalysis, "<i class=\"fas fa-bug fa-2x\" ></i>" },
                { States.NCRCustomerApproval, "<i class=\"fas fa-bug fa-2x\" ></i>" },
                { States.NCRVendorCorrectivePartsInProduction, "<i class=\"fas fa-bug fa-2x\" ></i>" },
                { States.NCRVendorCorrectivePartsComplete, "<i class=\"fas fa-bug fa-2x\" ></i>" },
                { States.NCRCustomerCorrectivePartsReceived, "<i class=\"fas fa-bug fa-2x\" ></i>" },
                { States.NCRCustomerCorrectivePartsAccepted, "<i class=\"fas fa-bug fa-2x\" ></i>" },
                { States.NCRCustomerRejectCorrective, "<i class=\"fas fa-bug fa-2x\" ></i>" },
                { States.NCRCustomerRejectRootCause, "<i class=\"fas fa-bug fa-2x\" ></i>" },
                { States.NCRCustomerRevisionNeeded, "<i class=\"fas fa-bug fa-2x\" ></i>" },
                { States.NCRRootCauseDisputes, "<i class=\"fas fa-bug fa-2x\" ></i>" },
                { States.NCRAdminDisputesIntervention, "<i class=\"fas fa-bug fa-2x\" ></i>" },
                { States.NCRDamagedByCustomer, "<i class=\"fas fa-bug fa-2x\" ></i>" },
                { States.NCRClosed, "<i class=\"fas fa-bug fa-2x\" ></i>" },

                { States.VendorPendingInvoice, "<i class=\"fas fa-money-bill-alt fa-2x\"></i>" },
            };

            return dic;
        }

        public static Dictionary<States, string> ConstructStateExplanationMapping()
        {
            Dictionary<States, string> dic = new Dictionary<States, string>
            {
                { States.PendingRFQ, "Your part is being reviewed by our team and select qualified manufacturers. Please allow us 1 business day to get back to you with feedback. If everything looks good, you will get pricing right away." },
                { States.BidForRFQ, "Your part is being reviewed by our team and select qualified manufacturers. Please allow us 1 business day to get back to you with feedback. If everything looks good, you will get pricing right away." },
                { States.BidReview, "Your part is being reviewed by our team and select qualified manufacturers. Please allow us 1 business day to get back to you with feedback. If everything looks good, you will get pricing right away." },

                { States.OutForRFQ, "Your part is being reviewed by our team and select qualified manufacturers. Please allow us 1 business day to get back to you with feedback. If everything looks good, you will get pricing right away." },
                { States.RFQRevision, "We have reviewed your part and need to request a revision to address feedback from the factory. Please allow us 1 business day to check over the revision. If everything looks good, you will get pricing right away." },
                { States.BackFromRFQ, "Our factories have finished reviewing your part. You will see a finished quote of feedback shortly." },
                { States.QuoteAccepted, "Your quote has been accepted. Now you are able to place order." },
                { States.VendorCancelledRFQ, "Upon review, our factories have found that your part can not be manufactured as designed. Please check your email for a detailed explanation of the issues inherent in the design. If you would like assistance designing for manufacturability, please contact Padtech’s Engineering team." },
                { States.AddExtraQuantities, "Add extra quantities for this product, depending on how big the quantities the new tooling setup charge may be requried." },
                { States.OrderInitiated, "Your order has been initiated but is pending payment to proceed. Please make your payment and then notify us by proceeding to “payment made” from the Dashboard or Alerts section of Omnae." },
                { States.PaymentMade, "Thank you for sending payment for this order. Once received and cleared, the order will progress." },
                { States.OrderPaid, "Your first order of this part is underway. Since this is your first order of this part, you will be sent a production proof for approval and then a sample of the product. Once you approve the sample, production will begin." },
                { States.ProofingStarted, "Your factory is making a production proof drawing for your review. It represents their exact interpretation of your specifications and is what the final product will be built off of. Please review it once available." },
                { States.ProofingComplete, "Your production proof is ready! Please review it carefully to make sure it matches your specification exactly. If you are satisfied, please approve the proof. If not, please reject the proof and tell us what needs to be changed." },
                { States.ProofApproved, "Your assigned factory will start building tooling now. You will be notified when it is complete and when a sample is on the way for your approval." },
                { States.ProofRejected, "Your factory is working on a revised proof to adjust their drawing based on your feedback." },
                { States.ToolingStarted, "Your proof has been approved. Your factory is now building the tooling needed to produce your part reliably. Please allow for the quoted tooling build time at this stage. Once finished, your factory will build samples with the new tooling for your approval." },
                { States.SampleStarted, "The tooling needed to produce your part is complete. Your assigned factory is building samples with that tooling and will notify you when they are on the way." },
                { States.SampleComplete, "Your assigned factory has shipped samples for you to review. Please review the samples carefully and insure they match your specifications." },
                { States.SampleApproved, "Your assigned factory will start production immediately." },
                { States.SampleRejected, "Your factory is refining it’s tooling based on your feedback from sample inspection. They will ship revised samples for your approval as soon as possible." },
                { States.ReOrderInitiated, "Your order has been initiated but is pending payment to proceed. Please make your payment and then notify us by proceeding to “payment made” from the Dashboard or Alerts section of Omnae." },
                { States.ReOrderPaymentMade, "Thank you for sending payment for the re-order. Once received and cleared, the re-order will progress." },
                { States.ReOrderPaid, "Thanks for your new order. Production will start shortly." },
                { States.ProductionStarted, "Production has started on your order. You will be notified when the parts ship." },
                { States.ProductionComplete, "Your production run is complete and your parts have shipped. Please see the order tracking page for carrier and tracking information." },
                { States.VendorPendingInvoice, "Creating a bill for the vendor on QuickBooks Online." },

            };

            return dic;
        }

        public static string GetStateIconUrl(Dictionary<States, string> dic, States state)
        {
            if (dic.ContainsKey(state))
            {
                return dic[state];
            }
            else
            {
                return dic[States.VendorCancelledRFQ];
            }
        }
    }
}
