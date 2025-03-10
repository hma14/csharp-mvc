using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface ICompanyBankInfoRepository
    {
        void Dispose();

        int AddCompanyBankInfo(CompanyBankInfo entity);
        CompanyBankInfo UpdateCompanyBankInfo(CompanyBankInfo entity);
        CompanyBankInfo GetCompanyBankInfoById(int id);
        void RemoveCompanyBankInfo(CompanyBankInfo entity);
    }
}
