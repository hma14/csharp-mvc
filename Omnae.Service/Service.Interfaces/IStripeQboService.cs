using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service.Interfaces
{
    public interface IStripeQboService
    {
        void Dispose();
        int AddStripeQbo(StripeQbo entity);
        StripeQbo FindStripeQboById(int Id);

        StripeQbo FindStripeQboByStripeInvoiceId(string stripeInvoiceId);
        IQueryable<StripeQbo> FindStripeQbosByQboId(string qboId);
    }
}
