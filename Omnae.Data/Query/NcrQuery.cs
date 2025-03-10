using System;
using System.Linq;
using Omnae.Common;
using Omnae.Model.Models;

namespace Omnae.Data.Query
{
    public static class NcrQuery
    {
        /// <summary>
        /// Filters for NCR
        /// </summary>
        public enum NcrFilter
        {
            /// <summary>
            /// All NCRs
            /// </summary>
            All,
            /// <summary>
            /// NCRs that are open
            /// </summary>
            Open,
            /// <summary>
            /// NCR That need some Attention
            /// </summary>
            Alert,
            /// <summary>
            /// NCR created from shared product
            /// </summary>
            Shared,
            /// <summary>
            /// Closed NCR
            /// </summary>
            Closed,
        }

        public enum NcrType
        {
            All,
            Vendor,
            Customer,
        }

        public enum NCR_CHART_FILTERS
        {
            Product = 1,
            Year = 2,
            Vendor = 3,
            Customer = 4,
            Sharee = 5,
            Sharer = 6,
        }

        public static IQueryable<NCReport> FilterBy(this IQueryable<NCReport> query, int companyId, NcrFilter filter, NcrType mode = NcrType.All, string search = null)
        {
            switch (filter)
            {
                case NcrFilter.All:
                    query = query.Where(x => mode != NcrType.Customer || (x.CustomerId == companyId));
                    break;

                case NcrFilter.Open:
                    query = query
                        .Where(x => mode != NcrType.Customer || (x.CustomerId == companyId))
                        .Where(n => (n.StateId >= States.NCRCustomerStarted && n.StateId < States.NCRClosed)
                                          || n.StateId == States.NCRCustomerCorrectivePartsAccepted
                                          || n.StateId == States.NCRDamagedByCustomer);
                    break;

                case NcrFilter.Alert:
                    {
                        switch (mode)
                        {
                            case NcrType.All:
                                query = query.Where(n => (n.StateId > States.NCRCustomerStarted && n.StateId < States.NCRClosed)
                                                          || n.StateId == States.NCRCustomerCorrectivePartsAccepted
                                                          || n.StateId == States.NCRDamagedByCustomer);
                                break;
                            case NcrType.Vendor:
                                query = query.Where(n => n.StateId == States.NCRVendorRootCauseAnalysis
                                                         || n.StateId == States.NCRCustomerRejectCorrective
                                                         || n.StateId == States.NCRCustomerRejectRootCause
                                                         || n.StateId == States.NCRVendorCorrectivePartsInProduction);
                                break;
                            case NcrType.Customer:
                                query = query
                                    .Where(x => x.CustomerId == companyId)
                                    .Where(n => n.StateId == States.NCRCustomerApproval
                                                         || n.StateId == States.NCRCustomerRejectCorrective
                                                         || n.StateId == States.NCRCustomerRejectRootCause
                                                         || n.StateId == States.NCRVendorCorrectivePartsComplete
                                                         || n.StateId == States.NCRCustomerCorrectivePartsReceived
                                                         || n.StateId == States.NCRRootCauseDisputes
                                                         || n.StateId == States.NCRCustomerRevisionNeeded
                                                         || n.StateId == States.NCRCustomerCorrectivePartsAccepted);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
                        }
                        break;
                    }
                case NcrFilter.Shared:
                    query = query.Where(n => n.Order.ProductSharingId != null && n.Order.ProductSharing.OwnerCompanyId == companyId);
                    break;

                case NcrFilter.Closed:
                    query = query.Where(x => mode != NcrType.Customer || (x.CustomerId == companyId))
                                 .Where(n => n.Task.StateId == (int)States.NCRClosed);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(filter), filter, null);
            }
            return search == null ? query : query.Found(search);
        }


        public static IQueryable<NCReport> ActiveNCRs(this IQueryable<NCReport> ncrs)
        {
            var q = from ncr in ncrs
                    where ((ncr.StateId >= States.NCRCustomerStarted && ncr.StateId < States.NCRClosed)
                                          || ncr.StateId == States.NCRCustomerCorrectivePartsAccepted
                                          || ncr.StateId == States.NCRDamagedByCustomer)
                    select ncr;
            return q;
        }
        public static IQueryable<NCReport> ReviewNCRs(this IQueryable<NCReport> ncrs)
        {
            var q = from ncr in ncrs.ActiveNCRs()
                    where ncr.StateId == States.NCRCustomerApproval
                    || ncr.StateId == States.NCRCustomerRejectCorrective
                    || ncr.StateId == States.NCRCustomerRejectRootCause
                    || ncr.StateId == States.NCRVendorRootCauseAnalysis
                    select ncr;
            return q;
        }
       

        public static IQueryable<NCReport> DisputeNCRs(this IQueryable<NCReport> ncrs)
        {
            var q = from ncr in ncrs.ActiveNCRs()
                    where (ncr.StateId == States.NCRRootCauseDisputes ||
                           ncr.StateId == States.NCRAdminDisputesIntervention ||
                           ncr.StateId == States.NCRCustomerCorrectivePartsAccepted)
                    select ncr;
            return q;
        }
        public static IQueryable<NCReport> ResolvedNCRs(this IQueryable<NCReport> ncrs)
        {
            var q = from ncr in ncrs.ActiveNCRs()
                    where (ncr.StateId == States.NCRCustomerCorrectivePartsReceived)

                    select ncr;
            return q;
        }
        public static IQueryable<NCReport> CustomerActionableReviewNCRs(this IQueryable<NCReport> ncrs)
        {
            var q = from ncr in ncrs.ActiveNCRs()
                    where (ncr.StateId == States.NCRCustomerRejectCorrective
                             || ncr.StateId == States.NCRCustomerRejectRootCause
                             || ncr.StateId == States.NCRCustomerApproval)                        
                    select ncr;
            return q;
        }

        public static IQueryable<NCReport> CustomerActionableDisputeNCRs(this IQueryable<NCReport> ncrs)
        {
            var q = from ncr in ncrs.ActiveNCRs()
                    where ncr.StateId == States.NCRCustomerRevisionNeeded
                    select ncr;
            return q;
        }

        public static IQueryable<NCReport> CustomerActionableResolutionNCRs(this IQueryable<NCReport> ncrs)
        {
            var q = from ncr in ncrs.ActiveNCRs()
                    where (ncr.StateId == States.NCRCustomerCorrectivePartsReceived ||
                           ncr.StateId == States.NCRCustomerCorrectivePartsAccepted)
                    select ncr;
            return q;
        }


        public static IQueryable<NCReport> VendorActionableReviewNCRs(this IQueryable<NCReport> ncrs)
        {
            var q = from ncr in ncrs.ActiveNCRs()
                    where ncr.StateId == States.NCRVendorRootCauseAnalysis
                        
                    select ncr;
            return q;
        }

        public static IQueryable<NCReport> VendorActionableDisputeNCRs(this IQueryable<NCReport> ncrs)
        {
            var q = from ncr in ncrs.ActiveNCRs()
                    where ncr.StateId == States.NCRRootCauseDisputes
                    select ncr;
            return q;
        }



        public static IQueryable<NCReport> VendorActionableResolutionNCRs(this IQueryable<NCReport> ncrs)
        {
            var q = from ncr in ncrs.ActiveNCRs()
                    where (ncr.StateId == States.NCRVendorCorrectivePartsInProduction ||
                           ncr.StateId == States.NCRVendorCorrectivePartsComplete)
                        
                    select ncr;
            return q;
        }


        private static IQueryable<NCReport> Found(this IQueryable<NCReport> ncrs, string search)
        {
            search = search.Trim().ToUpper();
            return ncrs.Where(x => x.Product.PartNumber != null && x.Product.PartNumber.ToUpper().Contains(search)
                                || x.Product.Name != null && x.Product.Name.ToUpper().Contains(search)
                                || x.NCRNumber != null && x.NCRNumber.ToUpper().Contains(search)
                                || x.NCRNumberForVendor != null && x.NCRNumberForVendor.ToUpper().Contains(search)
                                || x.TrackingNumber != null && x.TrackingNumber.ToUpper().Contains(search)
                                || x.Product.VendorCompany.Name != null && x.Product.VendorCompany.Name.ToUpper().Contains(search)
                                || x.Order.CustomerPONumber != null && x.Order.CustomerPONumber.ToUpper().Contains(search));
        }
    }
}
