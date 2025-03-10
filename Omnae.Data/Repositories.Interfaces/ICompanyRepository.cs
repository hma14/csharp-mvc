using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface ICompanyRepository
    {
        void Dispose();

        int AddCompany(Company entity);

        void UpdateCompany(Company entity);

        void UpdateCompanyShippingId(int id, int shippingId);

        Company GetCompanyByUserId(string userId);
        Company GetCompanyById(int Id);
        Company GetCompanyByStripeCustomerId(string stripeCustomerId);

        IQueryable<Company> GetAllCompanies(CompanyType? typeOfCompany = null, bool onlyActive = true);
        void RemoveCompany(Company entity);
    }
}
