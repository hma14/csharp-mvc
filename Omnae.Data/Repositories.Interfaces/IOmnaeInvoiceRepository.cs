using Omnae.Common;
using Omnae.Data.Abstracts;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface IOmnaeInvoiceRepository : IRepository<OmnaeInvoice>
    {
        void Dispose();
        int AddOmnaeInvoice(OmnaeInvoice entity);
        void UpdateOmnaeInvoice(OmnaeInvoice entity);
        void RemoveOmnaeInvoice(OmnaeInvoice entity);
        OmnaeInvoice GetOmnaeInvoiceById(int Id);
        List<OmnaeInvoice> GetOmnaeInvoiceByCompanyIdByTaskId(int companyId, int taskId);
        List<OmnaeInvoice> GetOmnaeInvoiceListByCompanyId(int companyId);
        List<OmnaeInvoice> GetOmnaeInvoiceList();

        List<OmnaeInvoice> GetOmnaeInvoiceByTaskId(int taskId);

        List<OmnaeInvoice> GetOmnaeInvoiceListByProductId(int productId);

        OmnaeInvoice GetOmnaeInvoiceByCompanyIdByOrderId(int companyId, int orderId);
        OmnaeInvoice GetOmnaeInvoiceByUserTypeByOrderId(USER_TYPE userType, int orderId);
        List<OmnaeInvoice> GetOmnaeInvoiceListByCompanyIdProductId(int companyId, int productId);
        List<OmnaeInvoice> GetOmnaeInvoiceListByTaskIdProductId(int taskId, int productId);
        IQueryable<OmnaeInvoice> GetOmnaeInvoicesByOrderId(int orderId);
    }
}
