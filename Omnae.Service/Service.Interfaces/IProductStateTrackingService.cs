using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service.Interfaces
{
    public interface IProductStateTrackingService
    {
        void Dispose();
        ProductStateTracking FindProductStateTrackingById(int id);
        List<ProductStateTracking> FindProductStateTrackingListByProductId(int productId);
        int AddProductStateTracking(ProductStateTracking entity);
        IQueryable<ProductStateTracking> FindProductStateTrackingByState(int stateId);
    }
}
