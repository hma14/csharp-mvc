using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omnae.Data.Model;

namespace Omnae.Service.Service.Interfaces
{
    public interface IOrderService
    {
        void Dispose();
        int AddOrder(Order entity);
        List<Order> FindOrderByCustomerId(int customerId);
        List<Order> FindOrderByVendorId(int vendorId);
        IQueryable<Order> FindOrderByCompanyId(int userId);
        IQueryable<Order> FindOrdersByCustomerId(int userId);
        IQueryable<Order> FindOrdersByVendorId(int userId);
        Order FindOrderById(int Id);
        IQueryable<Order> FindOrderList();
        void UpdateOrder(Order entity);
        List<Order> FindOrdersByProductId(int Id);
        List<Order> FindOrderByTaskId(int taskId);

        void RemoveOrder(Order entity);


        IQueryable<OrderHistoryModel> GetOrderHistory(int? customerId = null, int? vendorId = null, DateTime? start = null, DateTime? end = null);
        IQueryable<Order> FindOrdersAndSharedByCustomerId(int userId);
    }
}
