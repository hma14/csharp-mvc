using Omnae.Common;
using Omnae.Data.Abstracts;
using Omnae.Data.Repositories;
using Omnae.Model.Models;
using Omnae.Service.Service.Interfaces;
using System.Collections.Generic;
using Omnae.BlobStorage;
using System.Linq;

namespace Omnae.Service.Service
{
    public class OmnaeInvoiceService : IOmnaeInvoiceService
    {
        private OmnaeInvoiceRepository InvoiceRepository { get; }
        private IUnitOfWork OfWork { get; }
        private IDocumentStorageService DocumentStorageService { get; }

        public OmnaeInvoiceService(OmnaeInvoiceRepository omnaeInvoiceRepository, IUnitOfWork unitOfWork, IDocumentStorageService documentStorageService)
        {
            InvoiceRepository = omnaeInvoiceRepository;
            OfWork = unitOfWork;
            DocumentStorageService = documentStorageService;
        }

        public int AddOmnaeInvoice(OmnaeInvoice entity)
        {
            return InvoiceRepository.AddOmnaeInvoice(entity);
        }

        public void DeleteOmnaeInvoice(OmnaeInvoice entity)
        {
            InvoiceRepository.RemoveOmnaeInvoice(entity);
        }

        public void Dispose()
        {
            InvoiceRepository.Dispose();
        }

        public List<OmnaeInvoice> FindOmnaeInvoiceByCompanyIdByTaskId(int companyId, int taskId)
        {
            var invoices = InvoiceRepository.GetOmnaeInvoiceByCompanyIdByTaskId(companyId, taskId);
            UpdateDocUrlWithSecurityToken(invoices);
            return invoices;
        }

        public OmnaeInvoice FindOmnaeInvoiceByCompanyIdByOrderId(int companyId, int orderId)
        {
            var invoice = InvoiceRepository.GetOmnaeInvoiceByCompanyIdByOrderId(companyId, orderId);
            UpdateDocUrlWithSecurityToken(invoice);
            return invoice;
        }

        public OmnaeInvoice FindOmnaeInvoiceByUserTypeByOrderId(USER_TYPE userType, int orderId)
        {
            var invoice = InvoiceRepository.GetOmnaeInvoiceByUserTypeByOrderId(userType, orderId);
            UpdateDocUrlWithSecurityToken(invoice);
            return invoice;
        }

        public List<OmnaeInvoice> FindOmnaeInvoiceList()
        {
            var invoices = InvoiceRepository.GetOmnaeInvoiceList();
            UpdateDocUrlWithSecurityToken(invoices);
            return invoices;
        }

        public List<OmnaeInvoice> FindOmnaeInvoiceListByCompanyId(int companyId)
        {
            var invoices = InvoiceRepository.GetOmnaeInvoiceListByCompanyId(companyId);
            UpdateDocUrlWithSecurityToken(invoices);
            return invoices;
        }

        public List<OmnaeInvoice> FindOmnaeInvoiceListByCompanyIdProductId(int companyId, int productId)
        {
            var invoices = InvoiceRepository.GetOmnaeInvoiceListByCompanyIdProductId(companyId, productId);
            UpdateDocUrlWithSecurityToken(invoices);
            return invoices;
        }
        public List<OmnaeInvoice> FindOmnaeInvoiceListByTaskIdProductId(int taskId, int productId)
        {
            var invoices = InvoiceRepository.GetOmnaeInvoiceListByTaskIdProductId(taskId, productId);
            UpdateDocUrlWithSecurityToken(invoices);
            return invoices;
        }
        public OmnaeInvoice FindOmnaeInvoiceById(int Id)
        {
            var invoice = InvoiceRepository.GetOmnaeInvoiceById(Id);
            UpdateDocUrlWithSecurityToken(invoice);
            return invoice;
        }

        public void UpdateOmnaeInvoice(OmnaeInvoice entity)
        {
            InvoiceRepository.UpdateOmnaeInvoice(entity);
        }

        public List<OmnaeInvoice> FindOmnaeInvoiceByTaskId(int taskId)
        {
            var invoices = InvoiceRepository.GetOmnaeInvoiceByTaskId(taskId);
            UpdateDocUrlWithSecurityToken(invoices);
            return invoices;
        }

        public List<OmnaeInvoice> FindOmnaeInvoiceByProductId(int productId)
        {
            var invoices = InvoiceRepository.GetOmnaeInvoiceListByProductId(productId);
            UpdateDocUrlWithSecurityToken(invoices);
            return invoices;
        }

        public IQueryable<OmnaeInvoice> FindOmnaeInvoicesByOrderId(int orderId)
        {
            var invoices = InvoiceRepository.GetOmnaeInvoicesByOrderId(orderId);
            UpdateDocUrlWithSecurityToken(invoices);
            return invoices;
        }

        public void UpdateDocUrlWithSecurityToken(OmnaeInvoice invoice, ExpireTokenInfo expireTokenInfo = ExpireTokenInfo.Hour)
        {
            if (invoice == null)
                return;

            var newUrl = DocumentStorageService.AddSecurityTokenToUrl(invoice.PODocUri, expireTokenInfo);
            invoice.PODocUri = newUrl;
        }

        public void UpdateDocUrlWithSecurityToken(IEnumerable<OmnaeInvoice> invoices, ExpireTokenInfo expireTokenInfo = ExpireTokenInfo.Hour)
        {
            if (invoices == null)
                return;

            foreach (var invoice in invoices)
            {
                if (invoice == null)
                    continue;

                UpdateDocUrlWithSecurityToken(invoice, expireTokenInfo);
            }
        }
    }
}
