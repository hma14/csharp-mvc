using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service.Interfaces
{
    public interface ICompanyService
    {
        void Dispose();

        int AddCompany(Company entity);

        void UpdateCompany(Company entity);

        void UpdateCompanyShippingId(int id, int shippingId);

        Company FindCompanyByUserId(string userId);

        Company FindCompanyById(int Id);

        Company FindCompanyByStripeCustomerId(string stripeCustomerId);

        IQueryable<Company> FindAllCompanies(CompanyType? type = null, bool onlyActive = true);
        void RemoveCompany(Company entity);
    }
}
