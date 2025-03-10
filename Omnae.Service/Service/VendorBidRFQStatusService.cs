using Omnae.Data.Abstracts;
using Omnae.Data.Repositories;
using Omnae.Model.Models;
using Omnae.Service.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service
{
    public class VendorBidRFQStatusService : IVendorBidRFQStatusService
    {
        private readonly VendorBidRFQStatusRepository repository;
        private readonly IUnitOfWork unitOfWork;

        public VendorBidRFQStatusService(VendorBidRFQStatusRepository repository, IUnitOfWork unitOfWork)
        {
            this.repository = repository;
            this.unitOfWork = unitOfWork;
        }

        public int AddVendorBidRFQStatus(VendorBidRFQStatus entity)
        {
            return repository.AddVendorBidRFQStatus(entity);
        }

        public void Dispose()
        {
            repository.Dispose();
        }

        public VendorBidRFQStatus FindVendorBidRFQStatusById(int Id)
        {
            return repository.GetVendorBidRFQStatusById(Id);
        }

        public IQueryable<VendorBidRFQStatus> FindVendorBidRFQStatusByProductId(int productId)
        {
            return repository.GetVendorBidRFQStatusByProductId(productId);
        }

        public IQueryable<VendorBidRFQStatus> FindVendorBidRFQStatusByProductIdVendorId(int productId, int vendorId)
        {
            return repository.GetVendorBidRFQStatusByProductIdVendorId(productId, vendorId);
        }

        public List<VendorBidRFQStatus> FindVendorBidRFQStatusListByProductId(int productId)
        {
            return repository.GetVendorBidRFQStatusListByProductId(productId);
        }

        public void UpdateVendorBidRFQStatus(VendorBidRFQStatus entity)
        {
            repository.UpdateVendorBidRFQStatus(entity);
        }
    }
}
