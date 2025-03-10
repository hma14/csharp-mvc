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
    public class CompaniesCreditRelationshipRepository : RepositoryBase<CompaniesCreditRelationship>, ICompaniesCreditRelationshipRepository
    {
        public CompaniesCreditRelationshipRepository(OmnaeContext dbContext) : base(dbContext)
        {

        }
        public void AddCompaniesCreditRelationship(CompaniesCreditRelationship entity)
        {
            base.Add(entity);
            this.DbContext.Commit();
        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        public CompaniesCreditRelationship GetCompaniesCreditRelationshipByCustomerIdVendorId(int customerId, int vendorId)
        {
            return this.DbContext.CompaniesCreditRelationships.Where(x => x.CustomerId == customerId && x.VendorId == vendorId).FirstOrDefault();
        }

        public IQueryable<CompaniesCreditRelationship> GetCompaniesCreditRelationshipIQueryableByCustomerId(int customerId)
        {
            return this.DbContext.CompaniesCreditRelationships.Where(x => x.CustomerId == customerId);
        }

        public IQueryable<CompaniesCreditRelationship> GetCompaniesCreditRelationshipIQueryableByVendorId(int vendorId)
        {
            return this.DbContext.CompaniesCreditRelationships.Where(x => x.VendorId == vendorId);
        }
        public IQueryable<CompaniesCreditRelationship> GetCompaniesCreditRelationshipIQueryableByCompanyId(int companyId)
        {
            return this.DbContext.CompaniesCreditRelationships.Where(x => x.CustomerId == companyId || x.VendorId == companyId);
        }

        public List<CompaniesCreditRelationship> GetCompaniesCreditRelationshipsByCustomerId(int customerId)
        {
            return this.DbContext.CompaniesCreditRelationships
                .Include("Vendor")
                .Where(x => x.CustomerId == customerId).ToList();
        }

        public List<CompaniesCreditRelationship> GetCompaniesCreditRelationshipsByVendorId(int vendorId)
        {
            return this.DbContext.CompaniesCreditRelationships.Where(x => x.VendorId == vendorId).ToList();
        }

        public void RemoveCompaniesCreditRelationship(CompaniesCreditRelationship entity)
        {
            var entry = this.DbContext.CompaniesCreditRelationships.Where(x => x.CustomerId == entity.CustomerId && x.VendorId == entity.VendorId).FirstOrDefault();
            if (entry != null)
            {
                base.Delete(entry);
                this.DbContext.Commit();
            }
        }

        public void UpdateCompaniesCreditRelationship(CompaniesCreditRelationship entity)
        {
            base.Update(entity);
            this.DbContext.Commit();
        }
    }
}
