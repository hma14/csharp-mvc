using Omnae.Common;
using Omnae.Model.Extentions;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Omnae.Data.Query.NcrQuery;

namespace Omnae.Data.Query
{
    public static class OrderQuery
    {
        public enum OrderFilter
        {
            Current,
            Complete,
            Alert,
            Shared,
            Cancelled,
        }
        public enum UserMode
        {
            Vendor,
            Customer,
            Sharer,
        }

        public enum OrderCancelledBy
        {
            Customer = 1,
            Vendor = 2,
        }

        public static IQueryable<Order> FilterBy(this IQueryable<Order> query, int companyId, OrderFilter filter,
                                                  UserMode mode = UserMode.Customer, string search = null)
        {
            switch (filter)
            {
                case OrderFilter.Current:
                    query = query.Where(x => mode == UserMode.Customer ? (x.Product.CustomerId == companyId && x.ProductSharingId == null ||
                                                                          x.ProductSharingId != null && x.ProductSharing.SharingCompanyId == companyId)
                                                                       : true)
                                 .Where(x => x.TaskData.StateId > (int)States.QuoteAccepted && x.TaskData.StateId < (int)States.ProductionComplete || 
                                             x.TaskData.StateId >= (int) States.CustomerCancelOrder && x.TaskData.StateId <= (int)States.OrderCancelDenied ||
                                              x.TaskData.StateId == (int)States.VendorPendingInvoice);

                    break;

                case OrderFilter.Complete:
                    query = query.Where(x => mode == UserMode.Customer ? (x.Product.CustomerId == companyId && x.ProductSharingId == null ||
                                                                          x.ProductSharingId != null && x.ProductSharing.SharingCompanyId == companyId)
                                                                       : true)
                                 .Where(x => x.TaskData.StateId == (int)States.ProductionComplete);
                    break;

                case OrderFilter.Alert:
                    if (mode == UserMode.Customer)
                    {
                        query = query
                            .Where(x => mode == UserMode.Customer ? (x.Product.CustomerId == companyId && x.ProductSharingId == null ||
                                                                     x.ProductSharingId != null && x.ProductSharing.SharingCompanyId == companyId)
                                                                  : true)
                            .Where(x => x.TaskData.StateId == (int)States.ProofingComplete ||
                                                x.TaskData.StateId == (int)States.OrderInitiated ||
                                                x.TaskData.StateId == (int)States.ReOrderInitiated ||
                                                x.TaskData.StateId == (int)States.SampleComplete);

                    }
                    else
                    {
                        query = query.Where(x => x.TaskData.StateId == (int)States.ProofingStarted ||
                                                 x.TaskData.StateId == (int)States.ProofRejected ||
                                                 x.TaskData.StateId == (int)States.ReOrderPaid ||
                                                 x.TaskData.StateId == (int)States.OrderPaid ||
                                                 x.TaskData.StateId == (int)States.ProofApproved ||
                                                 x.TaskData.StateId == (int)States.ToolingStarted ||
                                                 x.TaskData.StateId == (int)States.SampleStarted ||
                                                 x.TaskData.StateId == (int)States.SampleRejected ||
                                                 x.TaskData.StateId == (int)States.SampleApproved ||
                                                 x.TaskData.StateId == (int)States.ProductionStarted ||
                                                 x.TaskData.StateId == (int)States.AddExtraQuantities ||
                                                 x.TaskData.StateId == (int)States.VendorPendingInvoice ||
                                                 x.TaskData.isEnterprise == true && x.TaskData.StateId == (int)States.PaymentMade ||
                                                 x.TaskData.isEnterprise == true && x.TaskData.StateId == (int)States.ReOrderPaymentMade);
                    }
                    break;

                case OrderFilter.Shared:
                    query = query.Where(x => x.ProductSharingId != null &&
                                            (mode == UserMode.Customer ? x.ProductSharing.OwnerCompanyId == companyId : true));
                    break;

                case OrderFilter.Cancelled:
                    query = query.Where(x => x.TaskData.StateId == (int)States.OrderCancelled == true);

                    break;
                default:
                    throw new Exception("filter is out of range");
            }
            return search == null ? query : query.Found(search);
        }

#if false  // may be used later

        public static IQueryable<Order> QAFilterBy(this IQueryable<Order> orders, NCR_CHART_FILTERS? filter = null, int? val = null)
        {
            switch (filter)
            {
                case NCR_CHART_FILTERS.Product:
                    if (val != null)
                    {
                        orders = orders.Where(x => x.ProductId == val.Value);
                    }
                    break;
                case NCR_CHART_FILTERS.Vendor:
                    if (val != null)
                    {
                        orders = orders.Where(x => x.Product.VendorId == val.Value);
                    }
                    break;

                case NCR_CHART_FILTERS.Customer:
                    if (val != null)
                    {
                        orders = orders
                            .Where(x => x.CustomerId != null ? x.CustomerId == val.Value : true)
                            .Where(x => x.Product.CustomerId == val.Value || x.ProductSharingId != null && x.ProductSharing.SharingCompanyId == val.Value);
                    }
                    break;
                case NCR_CHART_FILTERS.Year:
                    if (val != null)
                    {
                        orders = orders.Where(x => x.OrderDate.Year == val.Value);
                    }
                    break;
                case NCR_CHART_FILTERS.Sharee:
                    if (val != null)
                    {
                        orders = orders.Where(x => x.ProductSharing.SharingCompanyId == val.Value);
                    }
                    break;
                case NCR_CHART_FILTERS.Sharer:
                    if (val != null)
                    {
                        orders = orders.Where(x => x.ProductSharing.OwnerCompanyId == val.Value);
                    }
                    break;
                default:
                    break;
            }
            return orders;
        }
#endif

        private static IQueryable<Order> Found(this IQueryable<Order> orders, string search)
        {
            search = search.Trim().ToUpper();
            return orders.Where(x => x.Product.PartNumber != null && x.Product.PartNumber.ToUpper().Contains(search)
                            || x.Product.Name != null && x.Product.Name.ToUpper().Contains(search)
                            || x.Product.VendorCompany.Name != null && x.Product.VendorCompany.Name.ToUpper().Contains(search)
                            || x.CustomerPONumber != null && x.CustomerPONumber.ToUpper().Contains(search));
        }
    }
}
