using Omnae.Service.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omnae.Model.Models;
using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;

namespace Omnae.Service.Service
{
    public class NCReportService : INCReportService
    {
        private readonly INCReportRepository nCReportRepository;
        private readonly IUnitOfWork unitOfWork;

        public NCReportService(INCReportRepository nCReportRepository, IUnitOfWork unitOfWork)
        {
            this.nCReportRepository = nCReportRepository;
            this.unitOfWork = unitOfWork;
        }
        public int AddNCReport(NCReport entity)
        {
            return nCReportRepository.AddNCReport(entity);
        }

        public void Dispose()
        {
            nCReportRepository.Dispose();
        }

        public IQueryable<NCReport> FindNCReportByCustomerId(int customerId)
        {
            return nCReportRepository.GetNCReportByCustomerId(customerId);
        }
        public IQueryable<NCReport> FindNCReportByVendorId(int vendorId)
        {
            return nCReportRepository.GetNCReportByVendorId(vendorId);
        }

        public IQueryable<NCReport> FindNCReportByCompanyId(int companyId)
        {
            return nCReportRepository.GetNCReportByCompanyId(companyId);
        }

        public NCReport FindNCReportById(int Id)
        {
            return nCReportRepository.GetNCReportById(Id);
        }

        public NCReport FindNCReportByProductId(int productId)
        {
            return nCReportRepository.GetNCReportByProductId(productId);
        }
        public NCReport FindNCReportByTaskId(int taskId)
        {
            return nCReportRepository.GetNCReportByTaskId(taskId);
        }

        public List<NCReport> FindNCReportsByProductId(int productId)
        {
            return nCReportRepository.GetNCReportsByProductId(productId);
        }

        public List<NCReport> FindNCReportByProductIdOrderId(int productId, int orderId)
        {
            return nCReportRepository.GetNCReportByProductIdOrderId(productId, orderId);
        }

        public IQueryable<NCReport> FindNCReportsByOrderId(int orderId)
        {
            return nCReportRepository.GetNCReportsByOrderId(orderId);
        }
        public IQueryable<NCReport> FindNCReports()
        {
            return nCReportRepository.GetNCReports();
        }

        public void UpdateNCReport(NCReport entity)
        {
            nCReportRepository.UpdateNCReport(entity);
        }

        public string FindNCReportsYearlySequence(int companyId)
        {
            int currentYear = DateTime.UtcNow.Year;
            string seq = currentYear.ToString() + "-" +  (nCReportRepository.GetNCReports()
                .Where(x => x.NCDetectedDate.Value.Year == currentYear && x.CustomerId == companyId).Count() + 1);
            return seq;
        }

        public string FindNCReportsYearlySequenceForVendor(int companyId)
        {
            int currentYear = DateTime.UtcNow.Year;
            string seq = currentYear.ToString() + "-" + (nCReportRepository.GetNCReports()
                .Where(x => x.NCDetectedDate.Value.Year == currentYear && x.VendorId == companyId).Count() + 1);
            return seq;
        }

        public void RemoveNCReport(NCReport entity)
        {
            nCReportRepository.DeleteNCReport(entity);
        }
    }
}
