using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service.Interfaces
{
    public interface ICompaniesCreditRelationshipService
    {
        void Dispose();
        void AddCompaniesCreditRelationship(CompaniesCreditRelationship entity);
        void UpdateCompaniesCreditRelationship(CompaniesCreditRelationship entity);
        CompaniesCreditRelationship FindCompaniesCreditRelationshipByCustomerIdVendorId(int customerId, int vendorId);
        List<CompaniesCreditRelationship> FindCompaniesCreditRelationshipsByCustomerId(int customerId);
        List<CompaniesCreditRelationship> FindCompaniesCreditRelationshipsByVendorId(int vendorId);
        IQueryable<CompaniesCreditRelationship> FindCompaniesCreditRelationshipIQueryableByCustomerId(int customerId);
        IQueryable<CompaniesCreditRelationship> FindCompaniesCreditRelationshipIQueryableByVendorId(int vendorId);

        void DeleteCompaniesCreditRelationship(CompaniesCreditRelationship entity);
        IQueryable<CompaniesCreditRelationship> FindCompaniesCreditRelationshipIQueryableByCompanyId(int companyId);
    }
}
