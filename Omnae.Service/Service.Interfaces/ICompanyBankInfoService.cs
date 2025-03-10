using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service.Interfaces
{
    public interface ICompanyBankInfoService
    {
        void Dispose();
        int AddCompanyBankInfo(CompanyBankInfo entity);
        CompanyBankInfo FindCompanyBankInfoById(int id);
        CompanyBankInfo UpdateCompanyBankInfo(CompanyBankInfo entity);
        void RemoveCompanyBankInfo(CompanyBankInfo entity);

    }
}
