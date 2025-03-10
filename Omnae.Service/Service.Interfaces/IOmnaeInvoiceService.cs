using Omnae.Common;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omnae.BlobStorage;

namespace Omnae.Service.Service.Interfaces
{
    public interface IOmnaeInvoiceService
    {
        void Dispose();
        int AddOmnaeInvoice(OmnaeInvoice entity);
        void UpdateOmnaeInvoice(OmnaeInvoice entity);
        void DeleteOmnaeInvoice(OmnaeInvoice entity);

        OmnaeInvoice FindOmnaeInvoiceById(int Id);
        List<OmnaeInvoice> FindOmnaeInvoiceByCompanyIdByTaskId(int companyId, int taskId);
        List<OmnaeInvoice> FindOmnaeInvoiceListByCompanyId(int companyId);
        List<OmnaeInvoice> FindOmnaeInvoiceList();
        List<OmnaeInvoice> FindOmnaeInvoiceByTaskId(int taskId);
        List<OmnaeInvoice> FindOmnaeInvoiceByProductId(int productId);

        OmnaeInvoice FindOmnaeInvoiceByCompanyIdByOrderId(int companyId, int orderId);
        OmnaeInvoice FindOmnaeInvoiceByUserTypeByOrderId(USER_TYPE userType, int orderId);
        List<OmnaeInvoice> FindOmnaeInvoiceListByCompanyIdProductId(int companyId, int productId);
        List<OmnaeInvoice> FindOmnaeInvoiceListByTaskIdProductId(int taskId, int productId);
        void UpdateDocUrlWithSecurityToken(OmnaeInvoice invoice, ExpireTokenInfo expireTokenInfo = ExpireTokenInfo.Hour);
        void UpdateDocUrlWithSecurityToken(IEnumerable<OmnaeInvoice> invoice, ExpireTokenInfo expireTokenInfo = ExpireTokenInfo.Hour);
        IQueryable<OmnaeInvoice> FindOmnaeInvoicesByOrderId(int orderId);
    }
}
