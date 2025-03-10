using Omnae.Data.Abstracts;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface INCReportRepository : IRepository<NCReport>
    {
        void Dispose();
        int AddNCReport(NCReport entity);
        IQueryable<NCReport> GetNCReportByCustomerId(int customerId);
        IQueryable<NCReport> GetNCReportByVendorId(int customerId);
        IQueryable<NCReport> GetNCReports();
        NCReport GetNCReportById(int Id);
        NCReport GetNCReportByProductId(int productId);
        List<NCReport> GetNCReportsByProductId(int productId);
        List<NCReport> GetNCReportByProductIdOrderId(int productId, int orderId);
        void UpdateNCReport(NCReport entity);
        IQueryable<NCReport> GetNCReportsByOrderId(int orderId);
        NCReport GetNCReportByTaskId(int taskId);
        void DeleteNCReport(NCReport entity);
        IQueryable<NCReport> GetNCReportByCompanyId(int companyId);
    }
}
