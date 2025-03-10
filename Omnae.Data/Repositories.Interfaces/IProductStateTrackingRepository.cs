using Omnae.Data.Abstracts;
using Omnae.Model.Models;
using System.Collections.Generic;
using System.Linq;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface IProductStateTrackingRepository : IRepository<ProductStateTracking>
    {
        void Dispose();

        ProductStateTracking GetProductStateTrackingById(int id);
        List<ProductStateTracking> GetProductStateTrackingByProductId(int productId);
        int AddProductStateTracking(ProductStateTracking entity);
        IQueryable<ProductStateTracking> GetProductStateTrackingByState(int stateId);
    }
}
