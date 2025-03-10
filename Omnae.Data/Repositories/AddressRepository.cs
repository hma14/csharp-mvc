using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omnae.Model.Models;

namespace Omnae.Data.Repositories
{
    public class AddressRepository : RepositoryBase<Address>, IAddressRepository
    {
        public AddressRepository(OmnaeContext dbContext) : base(dbContext)
        {
        }

        public int AddAddress(Address entity)
        {
            base.Add(entity);
            this.DbContext.Commit();  // must call commit here otherwise, entity won't be added
            return entity.Id;
        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        public Address GetAddressById(int id)
        {
            return this.DbContext.Addresses
                .Include("StateProvince")
                .Include("Country")
                .Where(x => x.Id == id).FirstOrDefault();
        }

        public IQueryable<Address> FindAllFromCompanies(int id)
        {
            return this.DbContext.Addresses.Where(x => x.CompanyId == id);
        }
        public void UpdateAddress(Address entity)
        {
            base.Update(entity);
            this.DbContext.Commit();
        }
    }
}
