using Omnae.Data.Repositories.Interfaces;
using Omnae.Model.Models;
using Omnae.Service.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service
{
    public class StripeQboService : IStripeQboService
    {
        private IStripeQboRepository Repository { get; }

        public StripeQboService(IStripeQboRepository Repository)
        {
            this.Repository = Repository;
        }

        public void Dispose()
        {
            Repository.Dispose();
        }

        public int AddStripeQbo(StripeQbo entity)
        {
            return Repository.AddStripeQbo(entity);
        }

        public StripeQbo FindStripeQboById(int Id)
        {
            return Repository.GetStripeQboById(Id);
        }

        public StripeQbo FindStripeQboByStripeInvoiceId(string stripeInvoiceId)
        {
            return Repository.GetStripeQboByStripeInvoiceId(stripeInvoiceId);
        }

        public IQueryable<StripeQbo> FindStripeQbosByQboId(string qboId)
        {
            return Repository.GetStripeQbosByQboId(qboId);
        }
    }
}
