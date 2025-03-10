using Omnae.Common;
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
    public class OmnaeInvoiceRepository : RepositoryBase<OmnaeInvoice>, IOmnaeInvoiceRepository
    {
        public OmnaeInvoiceRepository(OmnaeContext dbContext) : base(dbContext)
        {

        }
        public int AddOmnaeInvoice(OmnaeInvoice entity)
        {
            base.Add(entity);
            DbContext.Commit();
            return entity.Id;
        }

        public void Dispose()
        {
            DbContext.Dispose();
        }

        public List<OmnaeInvoice> GetOmnaeInvoiceByCompanyIdByTaskId(int companyId, int taskId)
        {
            return DbContext.OmnaeInvoices.Where(x => x.CompanyId == companyId && x.TaskId == taskId).ToList();
        }
        public OmnaeInvoice GetOmnaeInvoiceByCompanyIdByOrderId(int companyId, int orderId)
        {
            return DbContext.OmnaeInvoices.Where(x => x.CompanyId == companyId && x.OrderId == orderId).FirstOrDefault();
        }
        public OmnaeInvoice GetOmnaeInvoiceByUserTypeByOrderId(USER_TYPE userType, int orderId)
        {
            return DbContext.OmnaeInvoices.Where(x => x.UserType == (int) userType && x.OrderId == orderId).FirstOrDefault();
        }



        public List<OmnaeInvoice> GetOmnaeInvoiceList()
        {
            return DbContext.OmnaeInvoices.ToList();
        }

        public List<OmnaeInvoice> GetOmnaeInvoiceListByCompanyId(int companyId)
        {
            return DbContext.OmnaeInvoices.Where(x => x.CompanyId == companyId).ToList();
        }

        public List<OmnaeInvoice> GetOmnaeInvoiceListByCompanyIdProductId(int companyId, int productId)
        {
            return DbContext.OmnaeInvoices.Where(x => x.CompanyId == companyId && x.ProductId == productId).ToList();
        }

        public List<OmnaeInvoice> GetOmnaeInvoiceListByTaskIdProductId(int taskId, int productId)
        {
            return DbContext.OmnaeInvoices.Where(x => x.TaskId == taskId && x.ProductId == productId).ToList();
        }

        public List<OmnaeInvoice> GetOmnaeInvoiceListByProductId(int productId)
        {
            return DbContext.OmnaeInvoices.Where(x => x.ProductId == productId).ToList();
        }

        public IQueryable<OmnaeInvoice> GetOmnaeInvoicesByOrderId(int orderId)
        {
            return DbContext.OmnaeInvoices.Where(x => x.OrderId == orderId);
        }

        public OmnaeInvoice GetOmnaeInvoiceById(int Id)
        {
            return DbContext.OmnaeInvoices.Where(x => x.Id == Id).FirstOrDefault();
        }

        public List<OmnaeInvoice> GetOmnaeInvoiceByTaskId(int taskId)
        {
            return DbContext.OmnaeInvoices.Where(x => x.TaskId == taskId).ToList();
        }

        public void RemoveOmnaeInvoice(OmnaeInvoice entity)
        {
            var entry = DbContext.Entry(entity);
            if (entry.State == System.Data.Entity.EntityState.Detached)
            {
                DbContext.OmnaeInvoices.Attach(entity);
            }
            base.Delete(entity);
            DbContext.Commit();
        }

        public void UpdateOmnaeInvoice(OmnaeInvoice entity)
        {
            base.Update(entity);
            DbContext.Commit();
        }

       
        
    }
}
