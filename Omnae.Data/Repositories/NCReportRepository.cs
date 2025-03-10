using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Model.Models;
using System.Collections.Generic;
using System.Linq;

namespace Omnae.Data.Repositories
{
    public class NCReportRepository : RepositoryBase<NCReport>, INCReportRepository
    {
        public NCReportRepository(OmnaeContext dbContext) : base(dbContext)
        {

        }
        public int AddNCReport(NCReport entity)
        {
            base.Add(entity);
            this.DbContext.Commit();
            return entity.Id;
        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        public IQueryable<NCReport> GetNCReportByCustomerId(int customerId)
        {
            var ncreports = this.DbContext.NCReports
                .Include("Order")
                .Include("Product")
                .Include("Task")
                .Where(x => x.CustomerId == customerId ||
            x.Order.ProductSharingId != null && x.Order.ProductSharing.OwnerCompanyId == customerId);
            //(x.Order.ProductSharing.OwnerCompanyId == customerId || x.Order.ProductSharing.SharingCompanyId == customerId));

            return ncreports.OrderBy(x => x.Id);
        }
        public IQueryable<NCReport> GetNCReportByVendorId(int vendorId)
        {
            var ncreports = this.DbContext.NCReports
                .Include("Order")
                .Include("Product")
                .Where(x => x.VendorId == vendorId);

            return ncreports.OrderBy(x => x.Id);
        }

        public IQueryable<NCReport> GetNCReportByCompanyId(int companyId)
        {
            var ncreports = this.DbContext.NCReports
                .Include("Order")
                .Include("Product")
                .Where(x => x.CustomerId == companyId || x.VendorId == companyId);

            return ncreports.OrderBy(x => x.Id);
        }

        public NCReport GetNCReportById(int Id)
        {
            return this.DbContext.NCReports.Include("Task").Where(x => x.Id == Id).FirstOrDefault();
        }

        public NCReport GetNCReportByProductId(int productId)
        {
            return this.DbContext.NCReports.Where(x => x.ProductId == productId).FirstOrDefault();
        }
        public NCReport GetNCReportByTaskId(int taskId)
        {
            return this.DbContext.NCReports.Where(x => x.TaskId == taskId).FirstOrDefault();
        }

        public List<NCReport> GetNCReportByProductIdOrderId(int productId, int orderId)
        {
            return this.DbContext.NCReports
                        .Include("Product")
                        .Include("Order")
                        .Where(x => x.ProductId == productId && x.OrderId == orderId)
                        .OrderBy(x => x.NCDetectedDate).ToList();
        }

        public IQueryable<NCReport> GetNCReportsByOrderId(int orderId)
        {
            return this.DbContext.NCReports
                        .Include("Product")
                        .Include("Order")
                        .Where(x => x.OrderId == orderId)
                        .OrderBy(x => x.NCDetectedDate);
        }
        public IQueryable<NCReport> GetNCReports()
        {
            return this.DbContext.NCReports
                .Include("Product")
                .Include("Order");
        }

        public List<NCReport> GetNCReportsByProductId(int productId)
        {
            return this.DbContext.NCReports
                .Include("Product")
                .Include("Order")
                .Where(x => x.ProductId == productId)
                .OrderBy(x => x.NCDetectedDate).ToList();
        }

        public void UpdateNCReport(NCReport entity)
        {
            base.Update(entity);
            this.DbContext.Commit();
        }
        public void DeleteNCReport(NCReport entity)
        {
            base.Delete(entity);
            this.DbContext.Commit();
        }
    }
}
