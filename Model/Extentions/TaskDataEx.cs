using Omnae.Common;
using Omnae.Model.Models;
using System.Collections.Generic;
using System.Linq;

namespace Omnae.Model.Extentions
{
    public static class TaskDataEx
    {
        public static IEnumerable<TaskData> WhereIsRFQs(this IEnumerable<TaskData> list) => list.Where(IsRFQs);

        public static IQueryable<TaskData> WhereIsRFQs(this IQueryable<TaskData> list)
        {
            var q = from td in list
                    where (td.StateId < (int)States.QuoteAccepted ||
                           td.StateId == (int)States.BidReview ||
                           td.StateId == (int)States.RFQBidComplete ||
                           td.StateId == (int)States.BidForRFQ ||
                           td.StateId == (int)States.PendingRFQRevision ||
                           td.StateId == (int)States.BidTimeout ||
                           td.StateId == (int)States.CustomerCancelledRFQ ||
                           td.StateId == (int)States.VendorRejectedRFQ ||
                           td.StateId == (int)States.ReviewRFQ ||
                           td.StateId == (int)States.ReviewRFQAccepted ||
                           td.StateId == (int)States.RFQFailed ||
                           td.StateId == (int)States.RFQReviewUpdateQuantity ||
                           td.StateId == (int)States.RFQBidUpdateQuantity
                           )
                    select td;
            return q;
        }

        public static bool IsRFQs(this TaskData td)
        {
            return (td.StateId < (int)States.QuoteAccepted ||
                    td.StateId == (int)States.BidReview ||
                    td.StateId == (int)States.RFQBidComplete ||
                    td.StateId == (int)States.BidForRFQ ||
                    td.StateId == (int)States.PendingRFQRevision ||
                    td.StateId == (int)States.BidTimeout ||
                    td.StateId == (int)States.CustomerCancelledRFQ ||
                    td.StateId == (int)States.VendorRejectedRFQ ||
                    td.StateId == (int)States.ReviewRFQ ||
                    td.StateId == (int)States.ReviewRFQAccepted ||
                    td.StateId == (int)States.RFQReviewUpdateQuantity ||
                    td.StateId == (int)States.RFQBidUpdateQuantity
                    );
        }

        public static IQueryable<TaskData> ActiveRFQs(this IQueryable<TaskData> list)
        {
            var q = from td in list
                    where (td.StateId >= (int)States.CustomerCancelledRFQ &&
                           td.StateId <= (int)States.KeepCurrentRFQRevision && 
                           !(td.StateId == (int)States.RFQBidComplete || td.StateId == (int)States.QuoteAccepted))
                    select td;
            return q;
        }
        public static IQueryable<TaskData> BiddingRFQs(this IQueryable<TaskData> list)
        {
            var q = from td in list
                    where (td.StateId >= (int)States.BidForRFQ &&
                           (td.StateId <= (int)States.BidTimeout &&
                           td.StateId != (int)States.RFQBidComplete ||
                           td.StateId == (int)States.BidReviewed) )
                    select td;
            return q;
        }
        public static IQueryable<TaskData> ActiveRFQStatistics(this IQueryable<TaskData> list)
        {
            var list1 = list.ActiveRFQs();
            var list2 = list1.Concat(list.BiddingRFQs());
            return list2;
        }

        public static IEnumerable<TaskData> WhereIsOrder(this IEnumerable<TaskData> list) => list.Where(IsOrder);

        public static bool IsOrder(this TaskData td)
        {
            return (td.StateId != (int)States.OutForRFQ &&
                    td.StateId != (int)States.RFQRevision &&
                    td.StateId != (int)States.BidReview &&
                    td.StateId != (int)States.RFQBidComplete &&
                    td.StateId != (int)States.QuoteAccepted &&
                    td.StateId != (int)States.BackFromRFQ &&
                    td.StateId < (int)States.ProductionComplete);
        }

        public static IEnumerable<TaskData> WhereIsNCRs(this IEnumerable<TaskData> list) => list.Where(IsNcr);

        public static IQueryable<TaskData> WhereIsNCRs(this IQueryable<TaskData> list)
        {
            var q = from td in list
                    where (td.StateId == (int)States.NCRCustomerStarted ||
                           td.StateId == (int)States.NCRVendorRootCauseAnalysis ||
                           td.StateId == (int)States.NCRCustomerApproval ||
                           td.StateId == (int)States.NCRCustomerRejectCorrective ||
                           td.StateId == (int)States.NCRCustomerRejectRootCause ||
                           td.StateId == (int)States.NCRVendorCorrectivePartsInProduction ||
                           td.StateId == (int)States.NCRVendorCorrectivePartsComplete ||
                           td.StateId == (int)States.NCRCustomerCorrectivePartsReceived ||
                           td.StateId == (int)States.NCRRootCauseDisputes ||
                           td.StateId == (int)States.NCRAdminDisputesIntervention ||
                           td.StateId == (int)States.NCRCustomerRevisionNeeded ||
                           td.StateId == (int)States.NCRClosed ||
                           td.StateId == (int)States.NCRCustomerCorrectivePartsAccepted ||
                           td.StateId == (int)States.NCRDamagedByCustomer)
                    select td;
            return q;
        }

        public static bool IsNcr(this TaskData td)
        {
            return (td.StateId == (int)States.NCRCustomerStarted ||
                    td.StateId == (int)States.NCRVendorRootCauseAnalysis ||
                    td.StateId == (int)States.NCRCustomerApproval ||
                    td.StateId == (int)States.NCRCustomerRejectCorrective ||
                    td.StateId == (int)States.NCRCustomerRejectRootCause ||
                    td.StateId == (int)States.NCRVendorCorrectivePartsInProduction ||
                    td.StateId == (int)States.NCRVendorCorrectivePartsComplete ||
                    td.StateId == (int)States.NCRCustomerCorrectivePartsReceived ||
                    td.StateId == (int)States.NCRRootCauseDisputes ||
                    td.StateId == (int)States.NCRAdminDisputesIntervention ||
                    td.StateId == (int)States.NCRCustomerRevisionNeeded ||
                    td.StateId == (int)States.NCRClosed ||
                    td.StateId == (int)States.NCRCustomerCorrectivePartsAccepted ||
                    td.StateId == (int)States.NCRDamagedByCustomer);
        }

        public static bool IsNeedAtention(this TaskData td)
        {
            switch (td.StateId)
            {
                case (int)States.OutForRFQ:
                case (int)States.BidForRFQ:
                case (int)States.RFQRevision:
                case (int)States.ProofingStarted:
                case (int)States.ProofRejected:
                case (int)States.ReOrderPaid:
                case (int)States.OrderPaid:
                case (int)States.ProofApproved:
                case (int)States.ToolingStarted:
                case (int)States.SampleStarted:
                case (int)States.SampleRejected:
                case (int)States.SampleApproved:
                case (int)States.NCRClosed:
                case (int)States.ProductionStarted:
                case (int)States.NCRCustomerStarted:
                case (int)States.NCRVendorRootCauseAnalysis:
                case (int)States.NCRDamagedByCustomer:
                case (int)States.AddExtraQuantities:
                case (int)States.VendorPendingInvoice:
                case (int)States.NCRVendorCorrectivePartsInProduction:
                //case (int)States.BidReview:
                //case (int)States.PendingRFQRevision:
                case (int)States.NCRCustomerRevisionNeeded:
                case (int)States.BackFromRFQ:

                    return true;

                case (int)States.PaymentMade:
                case (int)States.ReOrderPaymentMade:

                    if (td.isEnterprise == true)
                        return true;
                    break;
            }

            return false;
        }

        public static IQueryable<TaskData> WhereIsNeedAdminAtention(this IQueryable<TaskData> tds)
        {
            return tds.Where(x => (x.isEnterprise == false && (x.StateId == (int)States.PaymentMade || x.StateId == (int)States.ReOrderPaymentMade)) ||
                                  (x.isEnterprise == false && x.StateId == (int)States.PendingRFQ) ||
                                  x.StateId == (int)States.SetupMarkupExtraQty ||
                                  x.StateId == (int)States.NCRAdminDisputesIntervention ||
                                  x.StateId == (int)States.NCRCustomerRejectCorrective ||
                                  (x.StateId == (int)States.NCRCustomerCorrectivePartsAccepted || x.StateId == (int)States.BidReview) && x.isEnterprise == false ||
                                  x.StateId == (int)States.NCRDamagedByCustomer ||
                                  x.StateId == (int)States.NCRCustomerRejectRootCause);
        }

        public static bool IsNeedAdminAtention(this TaskData td)
        {
            switch (td.StateId)
            {
                case (int)States.PaymentMade:
                case (int)States.SetupMarkupExtraQty:
                case (int)States.ReOrderPaymentMade:
                case (int)States.PendingRFQ:
                case (int)States.NCRAdminDisputesIntervention:
                case (int)States.NCRCustomerRejectCorrective:
                case (int)States.NCRDamagedByCustomer:
                case (int)States.NCRCustomerRejectRootCause:
                    return true;

                case (int)States.BidReview:
                case (int)States.NCRCustomerCorrectivePartsAccepted:
                    if (td.isEnterprise == false)
                        return true;
                    break;
            }

            return false;
        }


        public static TaskData CanPlaceOrder(this List<TaskData> tds)
        {
            return tds.LastOrDefault(x => x.StateId == (int)States.QuoteAccepted ||
                                           x.StateId >= (int)States.SampleApproved &&
                                           x.StateId <= (int)States.NCRClosed &&
                                           x.StateId != (int)States.RFQBidComplete &&
                                           x.StateId != (int)States.SampleRejected ||
                                           x.StateId == (int)States.NCRCustomerCorrectivePartsAccepted ||
                                           x.StateId == (int)States.NCRDamagedByCustomer);
        }



        public static TaskData PartCanBeShared(this List<TaskData> tds)
        {
            return tds.FirstOrDefault(x => x.StateId >= (int)States.SampleApproved &&
                                           x.StateId <= (int)States.NCRClosed &&
                                           x.StateId != (int)States.RFQBidComplete &&
                                           x.StateId != (int)States.SampleRejected ||
                                           x.StateId == (int)States.NCRCustomerCorrectivePartsAccepted ||
                                           x.StateId == (int)States.NCRDamagedByCustomer);
        }

        public static bool CanReorder(this TaskData td)
        {
            return (td.StateId >= (int)States.SampleApproved &&
                    td.StateId <= (int)States.NCRClosed &&
                    td.StateId != (int)States.RFQBidComplete &&
                    td.StateId != (int)States.SampleRejected ||
                    td.StateId == (int)States.NCRCustomerCorrectivePartsAccepted ||
                    td.StateId == (int)States.NCRDamagedByCustomer);
        }


        public static bool IsRFQReview(this TaskData td)
        {
            return (
                    td.StateId == (int)States.PendingRFQRevision ||
                    td.StateId == (int)States.OutForRFQ ||
                    td.StateId == (int)States.ReviewRFQ ||
                    td.StateId == (int)States.ReviewRFQAccepted  ||
                    td.StateId == (int)States.RFQFailed
                    );
        }

        public static bool IsRFQBid(this TaskData td)
        {
            return (td.StateId == (int)States.BidReview ||
                    td.StateId == (int)States.BidForRFQ ||
                    td.StateId == (int)States.KeepCurrentRFQRevision ||
                    td.StateId == (int)States.RFQBidComplete
                    );
        }

        public static IQueryable<TaskData> CustomerActionableReviewRFQs(this IQueryable<TaskData> tds)
        {
            return tds.Where(td =>  td.StateId == (int)States.PendingRFQRevision ||
                                    td.StateId == (int)States.RFQFailed ||
                                    td.StateId == (int)States.BackFromRFQ);
        }
        public static IQueryable<TaskData> CustomerActionableBiddingRFQs(this IQueryable<TaskData> tds)
        {
            return tds.Where(td => td.StateId == (int)States.BidReview ||
                                   td.StateId == (int)States.BidTimeout);
        }

        public static IQueryable<TaskData> VendorActionableReviewRFQs(this IQueryable<TaskData> tds)
        {
            return tds.Where(td =>  td.StateId == (int)States.RFQRevision ||
                                    td.StateId == (int)States.ReviewRFQ ||
                                    td.StateId == (int)States.OutForRFQ ||
                                    td.StateId == (int)States.RFQReviewUpdateQuantity);
        }
        public static IQueryable<TaskData> VendorActionableBiddingRFQs(this IQueryable<TaskData> tds)
        {
            return tds.Where(td => td.StateId == (int)States.BidForRFQ ||                                   
                                    td.StateId == (int)States.RFQBidUpdateQuantity);
        }

    }
}
