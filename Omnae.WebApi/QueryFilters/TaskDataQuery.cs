using Omnae.Common;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Omnae.WebApi.QueryFilters
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
            /// <summary>
            /// All TaskData
            /// </summary>
            All,
            /// <summary>
            /// TaskData related products that are for the first order
            /// </summary>
            FirstOrder,
            /// <summary>
            /// TaskData related products that are for re-order
            /// </summary>
            ReOrder,
        }

        /// <summary>
        /// Filter Product based on the TaskData's states associated with the product
        /// </summary>
        public enum RFQFilter
        {
            /// <summary>
            /// All RFQs associated with current user
            /// </summary>
            Current,
            /// <summary>
            /// TaskData states that needs action from current user
            /// </summary>
            Alert,

        }
        /// <summary>
        /// User Type
        /// </summary>
        public enum UserType
        {          
            /// <summary>
            /// Type is Vendor
            /// </summary>
            Vendor,
            /// <summary>
            /// Type is Customer
            /// </summary>
            Customer,
        }
        /// <summary>
        /// Filter for query
        /// </summary>
        /// <param name="query">IQueryable of TaskData</param>
        /// <param name="filter">Filter</param>
        /// <param name="type">User type</param>
        /// <returns></returns>
        public static IQueryable<TaskData> FilterBy(this IQueryable<TaskData> query, ProductFilter filter = ProductFilter.All)
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
                                                             td.StateId == (int)States.BidReviewed ));
                case ProductFilter.FirstOrder:
                    {
                        return query.Where(td => td.StateId >= (int)States.QuoteAccepted &&
                                                         !(td.StateId == (int)States.BidForRFQ ||
                                                             td.StateId == (int)States.BidReview ||
                                                             td.StateId == (int)States.PendingRFQRevision ||
                                                             td.StateId == (int)States.BidTimeout ||
                                                             td.StateId == (int)States.RFQBidComplete ||
                                                             td.StateId == (int)States.BidReviewed ||
                                                             td.StateId == (int)States.ReOrderInitiated ||
                                                             td.StateId == (int)States.ReOrderPaid ||
                                                             td.StateId == (int)States.ReOrderPaymentMade ||
                                                             td.StateId == (int)States.ProductionComplete));                      
                    }
                case ProductFilter.ReOrder:
                    {
                        return query.Where(td => td.StateId == (int)States.ProductionStarted ||
                                                td.StateId == (int)States.ProductionComplete ||
                                                td.StateId == (int)States.NCRClosed ||
                                                td.StateId == (int)States.ReOrderInitiated ||
                                                td.StateId == (int)States.ReOrderPaid ||
                                                td.StateId == (int)States.ReOrderPaymentMade);                       
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(filter), filter, null);
            }
        }

        /// <summary>
        /// RFQ Filter for query
        /// </summary>
        /// <param name="query">IQueryable of TaskData</param>
        /// <param name="filter">Filter</param>
        /// <param name="type">User type</param>
        /// <returns></returns>
        public static IEnumerable<TaskData> FilterBy(this IEnumerable<TaskData> query, RFQFilter filter = RFQFilter.Current, UserType type = UserType.Customer)
        {
            switch (filter)
            {
                case RFQFilter.Current:
                    return query.Where(td => td.StateId < (int)States.QuoteAccepted ||
                                             td.StateId == (int)States.BidForRFQ ||
                                             td.StateId == (int)States.BidReview ||
                                             td.StateId == (int)States.PendingRFQRevision ||
                                             td.StateId == (int)States.BidTimeout ||
                                             td.StateId == (int)States.RFQBidComplete ||
                                             td.StateId == (int)States.BidReviewed);
                case RFQFilter.Alert:
                    if (type == UserType.Customer)
                    {
                        return query.Where(td => td.StateId == (int)States.BidReview ||
                                                 td.StateId == (int)States.PendingRFQRevision ||
                                                 td.StateId == (int)States.BackFromRFQ);
                    }
                    else
                    {
                        return query.Where(td => td.StateId == (int)States.BidForRFQ ||
                                                 td.StateId == (int)States.RFQRevision);
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
