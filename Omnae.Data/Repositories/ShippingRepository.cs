using Omnae.Model.Models;
using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Omnae.Data.Repositories
{
    public class ShippingRepository : RepositoryBase<Shipping>, IShippingRepository
    {
        public ShippingRepository(OmnaeContext dbContext) : base(dbContext)
        {

        }

        public int AddShipping(Shipping entity)
        {
            base.Add(entity);
            this.DbContext.Commit();
            return entity.Id;

        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        public Shipping GetShippingById(int id)
        {
            return this.DbContext.Shippings.Where(x => x.Id == id).Include("Address").FirstOrDefault();
        }

        public Shipping GetShippingByUserId(int companyId)
        {
            return this.DbContext.Companies.Where(c => c.Id == companyId).Include("Shipping.Address").FirstOrDefault()?.Shipping; //Get the default Shipping info from the Company
            //return this.DbContext.Shippings.Where(s => s.CompanyId == companyId).Include("Address").FirstOrDefault();
        }

        public void UpdateShipping(Shipping entity)
        {
            base.Update(entity);
            this.DbContext.Commit();
        }
    }
}
