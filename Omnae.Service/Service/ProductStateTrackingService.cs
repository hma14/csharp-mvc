using Omnae.Service.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omnae.Model.Models;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Data.Abstracts;

namespace Omnae.Service.Service
{
    public class ProductStateTrackingService : IProductStateTrackingService
    {
        private readonly IProductStateTrackingRepository productStateTrackingRepository;
        private readonly IUnitOfWork unitOfWork;

        public ProductStateTrackingService(IProductStateTrackingRepository productStateTrackingRepository, IUnitOfWork unitOfWork)
        {
            this.productStateTrackingRepository = productStateTrackingRepository;
            this.unitOfWork = unitOfWork;
        }

        public int AddProductStateTracking(ProductStateTracking entity)
        {
            return productStateTrackingRepository.AddProductStateTracking(entity);
        }

        public void Dispose()
        {
            productStateTrackingRepository.Dispose();
        }

        public ProductStateTracking FindProductStateTrackingById(int id)
        {
            return productStateTrackingRepository.GetProductStateTrackingById(id);
        }

        public List<ProductStateTracking> FindProductStateTrackingListByProductId(int productId)
        {
            return productStateTrackingRepository.GetProductStateTrackingByProductId(productId);
        }

        public IQueryable<ProductStateTracking> FindProductStateTrackingByState(int stateId)
        {
            return productStateTrackingRepository.GetProductStateTrackingByState(stateId);
        }
    }
}
