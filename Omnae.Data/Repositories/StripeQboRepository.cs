using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Repositories
{
    public class StripeQboRepository : RepositoryBase<StripeQbo>, IStripeQboRepository
    {
        public StripeQboRepository(OmnaeContext dbContext) : base(dbContext)
        {
        }

        public int AddStripeQbo(StripeQbo entity)
        {
            base.Add(entity);
            this.DbContext.Commit();
            return entity.Id;
        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        public StripeQbo GetStripeQboById(int Id)
        {
            return this.DbContext.StripeQbos.FirstOrDefault(x => x.Id == Id);
        }

        public StripeQbo GetStripeQboByStripeInvoiceId(string stripeInvoiceId)
        {
            return this.DbContext.StripeQbos.FirstOrDefault(x => x.StripeInvoiceId == stripeInvoiceId);
        }

        public IQueryable<StripeQbo> GetStripeQbosByQboId(string qboId)
        {
            return this.DbContext.StripeQbos.Where(x => x.QboId == qboId);
        }
    }
}
