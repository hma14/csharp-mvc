using Omnae.Model.Models;
using System;
using System.Linq;

namespace Omnae.Data.Query
{
    public static class CompanyQuery
    {
        /// <summary>
        /// Filter for query
        /// </summary>
        /// <param name="query">IQueryable of TaskData</param>
        /// <param name="filter">Filter</param>
        /// <returns></returns>
        public static IQueryable<Company> FilterBy(this IQueryable<Company> query, string filter)
        {
            if (String.IsNullOrEmpty(filter))
            {
                return query;
            }
            else
            {
                return query
                    .Where(x => x.Shipping.EmailAddress != null && x.Name != null)
                    .Where(x => x.Shipping.EmailAddress.ToUpper().Contains(filter.ToUpper()) || x.Name.ToUpper().Contains(filter.ToUpper()));
            }

        }

        public enum VENDOR_TYPE
        {
            Myself = 1,
            MyVendors = 2,
            NetworkVendors = 3,
        }

        public enum PARTNER_TYPE
        {
            All = 1,
            InvitedByMe = 2,
            Network = 3,
            Vendor = 4,
            Customer = 5,
        }
    }
}
