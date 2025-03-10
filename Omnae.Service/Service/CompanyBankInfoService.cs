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
    public class CompanyBankInfoService : ICompanyBankInfoService
    {
        private readonly ICompanyBankInfoRepository companyBankInfoRepository;
        private readonly IUnitOfWork unitOfwork;

        public CompanyBankInfoService(ICompanyBankInfoRepository companyBankInfoRepository, IUnitOfWork unitOfwork)
        {
            this.companyBankInfoRepository = companyBankInfoRepository;
            this.unitOfwork = unitOfwork;
        }

        public int AddCompanyBankInfo(CompanyBankInfo entity)
        {
            return companyBankInfoRepository.AddCompanyBankInfo(entity);
        }

        public void Dispose()
        {
            companyBankInfoRepository.Dispose();
        }

        public CompanyBankInfo FindCompanyBankInfoById(int id)
        {
            return companyBankInfoRepository.GetCompanyBankInfoById(id);
        }

        public void RemoveCompanyBankInfo(CompanyBankInfo entity)
        {
            companyBankInfoRepository.RemoveCompanyBankInfo(entity);
        }

        public CompanyBankInfo UpdateCompanyBankInfo(CompanyBankInfo entity)
        {
            return companyBankInfoRepository.UpdateCompanyBankInfo(entity);
        }
    }
}
