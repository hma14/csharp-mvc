using Omnae.Model.Models;
using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Service.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service
{
    public class ShippingService : IShippingService
    {
        private readonly IShippingRepository shippingRepository;
        private readonly IUnitOfWork unitOfWork;

        public ShippingService(IShippingRepository shippingRepository, IUnitOfWork unitOfWork)
        {
            this.shippingRepository = shippingRepository;
            this.unitOfWork = unitOfWork;
        }

        public int AddShipping(Shipping entity)
        {
            return shippingRepository.AddShipping(entity);
        }

        public void Dispose()
        {
            shippingRepository.Dispose();
        }

        public Shipping FindShippingById(int id)
        {
            return shippingRepository.GetShippingById(id);
        }

        public Shipping FindShippingByUserId(int companyId)
        {
            return shippingRepository.GetShippingByUserId(companyId);
        }

        public void UpdateShipping(Shipping entity)
        {
            shippingRepository.UpdateShipping(entity);
        }
    }
}
