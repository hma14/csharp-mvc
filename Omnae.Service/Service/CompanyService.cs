using Omnae.Data.Repositories.Interfaces;
using Omnae.Service.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omnae.Model.Models;

namespace Omnae.Service.Service
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository companyRepository;

        public CompanyService(ICompanyRepository companyRepository)
        {
            this.companyRepository = companyRepository;
        }

        public int AddCompany(Company entity)
        {
            return companyRepository.AddCompany(entity);
        }

        public void Dispose()
        {
            companyRepository.Dispose();
        }

        public IQueryable<Company> FindAllCompanies(CompanyType? typeOfCompany = null, bool onlyActive = true)
        {
            return companyRepository.GetAllCompanies(typeOfCompany, onlyActive);
        }

        public Company FindCompanyById(int Id)
        {
            return companyRepository.GetCompanyById(Id);
        }

        public Company FindCompanyByUserId(string userId)
        {
            return companyRepository.GetCompanyByUserId(userId);
        }

        public Company FindCompanyByStripeCustomerId(string stripeCustomerId)
        {
            return companyRepository.GetCompanyByStripeCustomerId(stripeCustomerId);
        }

        public void UpdateCompany(Company entity)
        {
            companyRepository.UpdateCompany(entity);
        }

        public void UpdateCompanyShippingId(int id, int shippingId)
        {
            companyRepository.UpdateCompanyShippingId(id, shippingId);
        }

        public void RemoveCompany(Company entity)
        {
            companyRepository.RemoveCompany(entity);
        }
    }
}
