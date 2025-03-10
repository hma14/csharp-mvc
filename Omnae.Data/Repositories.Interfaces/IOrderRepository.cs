using Omnae.Model.Models;
using Omnae.Data.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omnae.Data.Model;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        void Dispose();
        int AddOrder(Order entity);
        List<Order> GetOrderListByCustomerId(int customerId);
        List<Order> GetOrderListByVendorId(int vendorId);
        IQueryable<Order> GetOrdersByCustomerId(int customerId);
        IQueryable<Order> GetOrdersByVendorId(int companyId);
        IQueryable<Order> GetOrdersByCompanyId(int userId);
        Order GetOrderById(int Id);
        List<Order> GetOrdersByProductId(int Id);
        IQueryable<Order> GetOrderList();
        List<Order> GetOrderByTaskId(int taskId);
        void UpdateOrder(Order entity);

        void DeleteOrder(Order entity);

        IQueryable<OrderHistoryModel> GetOrderHistory(int? customerId = null, int? vendorId = null, DateTime? start = null, DateTime? end = null);
        IQueryable<Order> GetOrdersAndSharedByCustomerId(int companyId);

    }
}
