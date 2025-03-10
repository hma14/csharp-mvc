using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service.Interfaces
{
    public interface INCReportService
    {
        void Dispose();
        int AddNCReport(NCReport entity);
        IQueryable<NCReport> FindNCReportByCustomerId(int customerId);
        IQueryable<NCReport> FindNCReportByVendorId(int vendorId);
        IQueryable<NCReport> FindNCReports();
        NCReport FindNCReportById(int Id);
        NCReport FindNCReportByProductId(int productId);
        List<NCReport> FindNCReportsByProductId(int productId);
        List<NCReport> FindNCReportByProductIdOrderId(int productId, int orderId);
        void UpdateNCReport(NCReport entity);
        string FindNCReportsYearlySequence(int companyId);
        string FindNCReportsYearlySequenceForVendor(int vendorId);
        IQueryable<NCReport> FindNCReportsByOrderId(int orderId);
        NCReport FindNCReportByTaskId(int taskId);
        void RemoveNCReport(NCReport entity);
        IQueryable<NCReport> FindNCReportByCompanyId(int companyId);
    }
}
