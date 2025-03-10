using Omnae.Data.Abstracts;
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
    public class CompaniesCreditRelationshipService : ICompaniesCreditRelationshipService
    {
        private readonly ICompaniesCreditRelationshipRepository companiesCreditRelationshipRepository;
        private readonly IUnitOfWork unitOfWork;

        public CompaniesCreditRelationshipService(ICompaniesCreditRelationshipRepository companiesCreditRelationshipRepository, IUnitOfWork unitOfWork)
        {
            this.companiesCreditRelationshipRepository = companiesCreditRelationshipRepository;
            this.unitOfWork = unitOfWork;
        }
        public void AddCompaniesCreditRelationship(CompaniesCreditRelationship entity)
        {
            companiesCreditRelationshipRepository.AddCompaniesCreditRelationship(entity);
        }

        public void DeleteCompaniesCreditRelationship(CompaniesCreditRelationship entity)
        {
            companiesCreditRelationshipRepository.RemoveCompaniesCreditRelationship(entity);
        }

        public void Dispose()
        {
            companiesCreditRelationshipRepository.Dispose();
        }

        public CompaniesCreditRelationship FindCompaniesCreditRelationshipByCustomerIdVendorId(int customerId, int vendorId)
        {
            return companiesCreditRelationshipRepository.GetCompaniesCreditRelationshipByCustomerIdVendorId(customerId, vendorId);
        }

        public IQueryable<CompaniesCreditRelationship> FindCompaniesCreditRelationshipIQueryableByCustomerId(int customerId)
        {
            return companiesCreditRelationshipRepository.GetCompaniesCreditRelationshipIQueryableByCustomerId(customerId);
        }
 
        public IQueryable<CompaniesCreditRelationship> FindCompaniesCreditRelationshipIQueryableByVendorId(int vendorId)
        {
            return companiesCreditRelationshipRepository.GetCompaniesCreditRelationshipIQueryableByVendorId(vendorId);
        }

        public IQueryable<CompaniesCreditRelationship> FindCompaniesCreditRelationshipIQueryableByCompanyId(int companyId)
        {
            return companiesCreditRelationshipRepository.GetCompaniesCreditRelationshipIQueryableByCompanyId(companyId);
        }



        public List<CompaniesCreditRelationship> FindCompaniesCreditRelationshipsByCustomerId(int customerId)
        {
            return companiesCreditRelationshipRepository.GetCompaniesCreditRelationshipsByCustomerId(customerId);
        }

        public List<CompaniesCreditRelationship> FindCompaniesCreditRelationshipsByVendorId(int vendorId)
        {
            return companiesCreditRelationshipRepository.GetCompaniesCreditRelationshipsByVendorId(vendorId);
        }

        public void UpdateCompaniesCreditRelationship(CompaniesCreditRelationship entity)
        {
            companiesCreditRelationshipRepository.UpdateCompaniesCreditRelationship(entity);
        }
    }
}
