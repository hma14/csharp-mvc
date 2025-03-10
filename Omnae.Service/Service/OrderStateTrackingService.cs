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
    public class OrderStateTrackingService : IOrderStateTrackingService
    {
        private readonly IOrderStateTrackingRepository orderStateTrackingRepository;
        private readonly IUnitOfWork unitOfWork;

        public OrderStateTrackingService(IOrderStateTrackingRepository orderStateTrackingRepository, IUnitOfWork unitOfWork)
        {
            this.orderStateTrackingRepository = orderStateTrackingRepository;
            this.unitOfWork = unitOfWork;
        }

        public int AddOrderStateTracking(OrderStateTracking entity)
        {
            return orderStateTrackingRepository.AddOrderStateTracking(entity);
        }

        public void Dispose()
        {
            orderStateTrackingRepository.Dispose();
        }

        public OrderStateTracking FindOrderStateTrackingById(int id)
        {
            return orderStateTrackingRepository.GetOrderStateTrackingById(id);
        }

        public List<OrderStateTracking> FindOrderStateTrackingListByOrderId(int orderId)
        {
            return orderStateTrackingRepository.GetOrderStateTrackingByOrderId(orderId);
        }

        public IQueryable<OrderStateTracking> FindOrderStateTrackingsByOrderId(int orderId)
        {
            return orderStateTrackingRepository.GetOrderStateTrackingsByOrderId(orderId);
        }
        public List<OrderStateTracking> FindOrderStateTrackingList()
        {
            return orderStateTrackingRepository.GetOrderStateTrackingList();
        }
        public List<OrderStateTracking> FindOrderStateTrackingByNcrId(int NcrId)
        {
            return orderStateTrackingRepository.GetOrderStateTrackingByNcrId(NcrId);
        }

        public void RemoveOrderStateTracking(OrderStateTracking entity)
        {
            orderStateTrackingRepository.DeleteOrderStateTracking(entity);
        }
    }
}
