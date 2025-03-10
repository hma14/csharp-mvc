using Omnae.Data.Abstracts;
using Omnae.Model.Models;
using System.Collections.Generic;
using System.Linq;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface IOrderStateTrackingRepository : IRepository<OrderStateTracking>
    {
        void Dispose();

        OrderStateTracking GetOrderStateTrackingById(int id);
        List<OrderStateTracking> GetOrderStateTrackingByOrderId(int orderId);
        int AddOrderStateTracking(OrderStateTracking entity);
        List<OrderStateTracking> GetOrderStateTrackingList();
        IQueryable<OrderStateTracking> GetOrderStateTrackingsByOrderId(int orderId);
        List<OrderStateTracking> GetOrderStateTrackingByNcrId(int Ncrid);
        void DeleteOrderStateTracking(OrderStateTracking entity);
    }
}
