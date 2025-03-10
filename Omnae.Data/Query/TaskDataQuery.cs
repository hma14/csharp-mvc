using Omnae.Common;
using Omnae.Model.Extentions;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Omnae.Data.Query
{
    /// <summary>
    /// Query based on TaskData states
    /// </summary>
    public static class TaskDataQuery
    {
        /// <summary>
        /// Product Filter based on the TaskData's states associated with the product
        /// </summary>
        public enum ProductFilter
        {
            All,
            FirstOrder,
            ReOrder,
            Complete,
            Shared
        }

        /// <summary>
        /// Filter Product based on the TaskData's states associated with the product
        /// </summary>
        public enum RFQFilter
        {
            All = 1,
            ActionRequired = 2,
            NoActionRequired = 3,
        }

        public enum OrderFilter
        {
            Current,
            Complete,
            Alert,
        }
        /// <summary>
        /// User Type
        /// </summary>
        public enum UserType
        {
            Vendor,
            Customer,
        }
        /// <summary>
        /// Filter for query
        /// </summary>
        /// <param name="query">IQueryable of TaskData</param>
        /// <param name="filter">Filter</param>
        /// <param name="type">User type</param>
        /// <returns></returns>
        public static IQueryable<TaskData> FilterBy(this IQueryable<TaskData> query, ProductFilter filter, UserType type)
        {
            switch (filter)
            {
                case ProductFilter.All:
                    return query.Where(td => td.StateId >= (int)States.QuoteAccepted &&
                                                         !(td.StateId == (int)States.BidForRFQ ||
                                                             td.StateId == (int)States.BidReview ||
                                                             td.StateId == (int)States.PendingRFQRevision ||
                                                             td.StateId == (int)States.BidTimeout ||
                                                             td.StateId == (int)States.RFQBidComplete ||
                                                             td.StateId == (int)States.BidReviewed));
                case ProductFilter.FirstOrder:
                    {
                        return query.Where(td => td.StateId >= (int)States.QuoteAccepted && td.StateId <= (int)States.SampleRejected);
                    }
                case ProductFilter.ReOrder:
                    if (type == UserType.Customer)
                    {
                        return query.Where(td => td.StateId >= (int)States.SampleApproved &&
                                                td.StateId <= (int)States.ProductionComplete ||
                                                td.StateId == (int)States.NCRClosed ||
                                                td.StateId == (int)States.ReOrderInitiated ||
                                                td.StateId == (int)States.ReOrderPaid ||
                                                td.StateId == (int)States.ReOrderPaymentMade);

                    }
                    throw new Exception("This filter only apply to user type of CUSTOMER");

                case ProductFilter.Complete:
                    if (type == UserType.Vendor)
                    {
                        return query.Where(td => td.StateId == (int)States.ProductionComplete || td.StateId == (int)States.NCRClosed);
                    }
                    throw new Exception("This filter only apply to user type of VENDOR");

                default:
                    throw new Exception("filter is out of range");
            }
        }

        /// <summary>
        /// RFQ Filter for query
        /// </summary>
        /// <param name="query">IQueryable of TaskData</param>
        /// <param name="filter">Filter</param>
        /// <param name="type">User type</param>
        /// <returns></returns>
        public static IQueryable<TaskData> FilterBy(this IQueryable<TaskData> query, RFQFilter filter, UserType type)
        {
            switch (filter)
            {
                case RFQFilter.All:
                    return query.WhereIsRFQs();

                case RFQFilter.ActionRequired:
                    if (type == UserType.Customer)
                    {
                        return query.WhereIsRFQs()
                            .Where(td => td.StateId == (int)States.BidReview ||
                                        td.StateId == (int)States.PendingRFQRevision ||
                                        td.StateId == (int)States.RFQFailed ||
                                        td.StateId == (int)States.BidTimeout ||
                                        td.StateId == (int)States.BackFromRFQ);
                    }
                    else
                    {
                        return query.WhereIsRFQs()
                            .Where(td => td.StateId == (int)States.BidForRFQ ||
                                        td.StateId == (int)States.RFQRevision ||
                                        td.StateId == (int)States.ReviewRFQ ||
                                        td.StateId == (int)States.OutForRFQ ||
                                        td.StateId == (int)States.RFQReviewUpdateQuantity ||
                                        td.StateId == (int)States.RFQBidUpdateQuantity);
                    }
                case RFQFilter.NoActionRequired:
                    if (type == UserType.Customer)
                    {
                        return query.WhereIsRFQs()
                            .Where(td => td.StateId == (int)States.BidForRFQ ||
                                        td.StateId == (int)States.RFQRevision ||
                                        td.StateId == (int)States.ReviewRFQ ||
                                        td.StateId == (int)States.OutForRFQ ||
                                        td.StateId == (int)States.RFQBidComplete ||
                                        td.StateId == (int)States.VendorCancelledRFQ ||
                                        td.StateId == (int)States.RFQReviewUpdateQuantity ||
                                        td.StateId == (int)States.CustomerCancelledRFQ ||
                                        td.StateId == (int)States.VendorRejectedRFQ ||
                                        td.StateId == (int)States.ReviewRFQAccepted ||
                                        td.StateId == (int)States.RFQBidUpdateQuantity);
                    }
                    else
                    {
                        return query.WhereIsRFQs()
                             .Where(td => td.StateId == (int)States.BidReview ||
                                        td.StateId == (int)States.PendingRFQRevision ||
                                        td.StateId == (int)States.RFQFailed ||
                                        td.StateId == (int)States.BidTimeout ||
                                        td.StateId == (int)States.RFQBidComplete ||
                                        td.StateId == (int)States.VendorCancelledRFQ ||
                                        td.StateId == (int)States.CustomerCancelledRFQ ||
                                        td.StateId == (int)States.VendorRejectedRFQ ||
                                        td.StateId == (int)States.ReviewRFQAccepted ||
                                        td.StateId == (int)States.BackFromRFQ);
                    }
                default:
                    throw new Exception("filter is out of range");
            }
        }

        public class PartInfoDto
        {
            public string PartNumber { get; set; }
            public string PartNumberRevision { get; set; }
        }

        /// <summary>
        /// Filter for query by given parts info (partNumber, partNumberRevision)
        /// </summary>
        /// <param name="query">IQueryable of TaskData</param>
        /// <param name="filter">Filter</param>
        /// <param name="type">User type</param>
        /// <returns></returns>
        public static IEnumerable<TaskData> FilterByParts(this IQueryable<TaskData> query, PartInfoDto[] parts)
        {
            var list = query.Where(td => td.StateId >= (int)States.QuoteAccepted &&
                                    !(td.StateId == (int)States.BidForRFQ ||
                                        td.StateId == (int)States.BidReview ||
                                        td.StateId == (int)States.PendingRFQRevision ||
                                        td.StateId == (int)States.BidTimeout ||
                                        td.StateId == (int)States.RFQBidComplete ||
                                        td.StateId == (int)States.BidReviewed))
                            .ToList();
            var list2 = list.Where(x => parts.Any(y => y.PartNumber == x.Product.PartNumber && y.PartNumberRevision == x.Product.PartNumberRevision));
            return list2.GroupBy(x => x.ProductId).Select(x => x.LastOrDefault());
        }

    }
}
