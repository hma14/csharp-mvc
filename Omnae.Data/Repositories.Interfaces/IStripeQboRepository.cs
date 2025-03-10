using Omnae.Data.Abstracts;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface IStripeQboRepository : IRepository<StripeQbo>
    {
        void Dispose();
        int AddStripeQbo(StripeQbo entity);
        StripeQbo GetStripeQboById(int Id);

        StripeQbo GetStripeQboByStripeInvoiceId(string stripeInvoiceId);
        IQueryable<StripeQbo> GetStripeQbosByQboId(string qboId);
    }
}
