using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service.Interfaces
{
    public interface IOrderStateTrackingService
    {
        void Dispose();
        OrderStateTracking FindOrderStateTrackingById(int id);
        List<OrderStateTracking> FindOrderStateTrackingListByOrderId(int orderId);
        int AddOrderStateTracking(OrderStateTracking entity);
        List<OrderStateTracking> FindOrderStateTrackingList();
        IQueryable<OrderStateTracking> FindOrderStateTrackingsByOrderId(int orderId);
        List<OrderStateTracking> FindOrderStateTrackingByNcrId(int NcrId);
        void RemoveOrderStateTracking(OrderStateTracking entity);
    }
}
