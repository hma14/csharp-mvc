using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface ICompaniesCreditRelationshipRepository
    {
        void Dispose();
        void AddCompaniesCreditRelationship(CompaniesCreditRelationship entity);
        void UpdateCompaniesCreditRelationship(CompaniesCreditRelationship entity);
        CompaniesCreditRelationship GetCompaniesCreditRelationshipByCustomerIdVendorId(int customerId, int vendorId);
        List<CompaniesCreditRelationship> GetCompaniesCreditRelationshipsByCustomerId(int customerId);
        List<CompaniesCreditRelationship> GetCompaniesCreditRelationshipsByVendorId(int vendorId);
        IQueryable<CompaniesCreditRelationship> GetCompaniesCreditRelationshipIQueryableByCustomerId(int customerId);
        IQueryable<CompaniesCreditRelationship> GetCompaniesCreditRelationshipIQueryableByVendorId(int vendorId);
        void RemoveCompaniesCreditRelationship(CompaniesCreditRelationship entity);
        IQueryable<CompaniesCreditRelationship> GetCompaniesCreditRelationshipIQueryableByCompanyId(int companyId);
    }
}
