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
    public class CompanyBankInfoRepository : RepositoryBase<CompanyBankInfo>, ICompanyBankInfoRepository
    {
        public CompanyBankInfoRepository(OmnaeContext dbContext) : base(dbContext)
        {

        }

        public int AddCompanyBankInfo(CompanyBankInfo entity)
        {
            base.Add(entity);
            this.DbContext.Commit();
            return entity.Id;
        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        public CompanyBankInfo GetCompanyBankInfoById(int id)
        {
            return this.DbContext.CompanyBankInfos
                .AsNoTracking()
                .Include("BankAddress")
                .Where(x => x.Id == id).FirstOrDefault();
        }

        public CompanyBankInfo UpdateCompanyBankInfo(CompanyBankInfo entity)
        {
            base.Update(entity);
            return entity;
        }

        public void RemoveCompanyBankInfo(CompanyBankInfo entity)
        {
            var ent = this.DbContext.CompanyBankInfos.Where(x => x.Id == entity.Id).FirstOrDefault();
            base.Delete(ent);
            this.DbContext.Commit();
        }
    }
}
