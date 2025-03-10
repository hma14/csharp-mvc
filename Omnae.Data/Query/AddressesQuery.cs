using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omnae.Model.Models;

namespace Omnae.Data.Query
{
    public static class AddressesQuery
    {
        public enum AddressFilter
        {
            All,
            Shipping,
            Billing,
            Mailing
        }

        public static IQueryable<Address> FilterBy(this IQueryable<Address> query, AddressFilter type = AddressFilter.All)
        {
            switch(type)
            {
                case AddressFilter.All:
                    return query;

                case AddressFilter.Shipping:
                    return query.Where(n => n.isShipping);

                case AddressFilter.Billing:
                    return query.Where(n => n.isBilling);

                case AddressFilter.Mailing:
                    return query.Where(n => n.isMainAddress);
            }

            return query;
        }
    }
}
