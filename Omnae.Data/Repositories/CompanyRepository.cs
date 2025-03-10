using Omnae.Model.Models;
using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using System.Linq;

namespace Omnae.Data.Repositories
{
    public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
    {
        public CompanyRepository(OmnaeContext dbContext) : base(dbContext)
        {

        }

        public int AddCompany(Company entity)
        {
            base.Add(entity);
            this.DbContext.Commit(); // must call commit here otherwise, entity won't be added
            return entity.Id;
        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        public IQueryable<Company> GetAllCompanies(CompanyType? typeOfCompany = null, bool onlyActive = true)
        {
            var q = from company in DbContext.Companies.AsNoTracking()
                    where company.IsActive || onlyActive == false
                    where (typeOfCompany == null || 
                           (company.isEnterprise == false && company.CompanyType == typeOfCompany) || 
                           (company.isEnterprise == true))
                    select company;
            return q;
        }

        public Company GetCompanyById(int Id)
        {
            var company = this.DbContext.Companies
                .AsNoTracking()
                .Include("Address")
                .Include("BillAddress")
                .Include("Shipping")
                .Include("CompanyBankInfo")
                .FirstOrDefault(x => x.Id == Id); 
            return company;
        }

        public Company GetCompanyByUserId(string userId)
        {
            return DbContext.Users.Find(userId)?.Company;
            //var companies = this.DbContext.Companies.Where(x => x.UserId == userId).Include("Address");
            //if (companies != null)
            //    return companies.FirstOrDefault();
            //else
            //    return null;
        }

        public Company GetCompanyByStripeCustomerId(string stripeCustomerId)
        {
            var company = this.DbContext.Companies
                .AsNoTracking()
                .Include("Address")
                .Include("BillAddress")
                .Include("Shipping")
                .Include("CompanyBankInfo")
                .FirstOrDefault(x => x.StripeCustomerId == stripeCustomerId);
            return company;
        }

        public void UpdateCompany(Company entity)
        {
            base.Update(entity);
        }

        public void UpdateCompanyShippingId(int id, int shippingId)
        {
            var entity = this.DbContext.Companies.Where(x => x.Id == id).FirstOrDefault();
            entity.ShippingId = shippingId;
            UpdateCompany(entity);          
        }

        public void RemoveCompany(Company entity)
        {
            var ent = this.DbContext.Companies.Where(x => x.Id == entity.Id).FirstOrDefault();
            base.Delete(ent);
            this.DbContext.Commit();
        }
    }
}
