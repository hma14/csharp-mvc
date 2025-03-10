using Omnae.Service.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omnae.Model.Models;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Data.Abstracts;
using Omnae.Data.Model;

namespace Omnae.Service.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository orderRepository;
        private readonly IUnitOfWork unitOfWork;

        public OrderService(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
        {
            this.orderRepository = orderRepository;
            this.unitOfWork = unitOfWork;
        }
        public int AddOrder(Order entity)
        {
            return orderRepository.AddOrder(entity);
        }

        public void Dispose()
        {
            orderRepository.Dispose();
        }

        public Order FindOrderById(int Id)
        {
            return orderRepository.GetOrderById(Id);
        }

        public IQueryable<Order> FindOrderByCompanyId(int userId)
        {
            return orderRepository.GetOrdersByCompanyId(userId);
        }

        public IQueryable<Order> FindOrderList()
        {
            return orderRepository.GetOrderList();
        }

        public void UpdateOrder(Order entity)
        {
            orderRepository.UpdateOrder(entity);
        }

        public List<Order> FindOrdersByProductId(int Id)
        {
            return orderRepository.GetOrdersByProductId(Id);
        }

        public List<Order> FindOrderByCustomerId(int customerId)
        {
            return orderRepository.GetOrderListByCustomerId(customerId);
        }

        public List<Order> FindOrderByVendorId(int vendorId)
        {
            return orderRepository.GetOrderListByVendorId(vendorId);
        }

        public List<Order> FindOrderByTaskId(int taskId)
        {
            return orderRepository.GetOrderByTaskId(taskId);
        }

        public void RemoveOrder(Order entity)
        {
            orderRepository.DeleteOrder(entity);
        }

        public IQueryable<Order> FindOrdersByCustomerId(int userId)
        {
            return orderRepository.GetOrdersByCustomerId(userId);
        }
        public IQueryable<Order> FindOrdersAndSharedByCustomerId(int userId)
        {
            return orderRepository.GetOrdersAndSharedByCustomerId(userId);
        }

        public IQueryable<Order> FindOrdersByVendorId(int userId)
        {
            return orderRepository.GetOrdersByVendorId(userId);
        }

        public IQueryable<OrderHistoryModel> GetOrderHistory(int? customerId = null, int? vendorId = null, DateTime? start = null, DateTime? end = null)
        {
            return orderRepository.GetOrderHistory(customerId, vendorId, start, end);
        }
    }
}
