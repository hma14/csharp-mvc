using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Repositories
{
    public class OrderStateTrackingRepository : RepositoryBase<OrderStateTracking>, IOrderStateTrackingRepository
    {
        public OrderStateTrackingRepository(OmnaeContext dbContext) : base(dbContext)
        {

        }
        public int AddOrderStateTracking(OrderStateTracking entity)
        {
            base.Add(entity);
            this.DbContext.Commit();
            return entity.Id;
        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        public OrderStateTracking GetOrderStateTrackingById(int id)
        {
            return this.DbContext.OrderStateTrackings.Where(x => x.Id == id).FirstOrDefault();
        }

        public List<OrderStateTracking> GetOrderStateTrackingByOrderId(int orderId)
        {
            var orderStateTrackings = this.DbContext.OrderStateTrackings.Where(x => x.OrderId == orderId).OrderBy(x => x.StateId);
            return orderStateTrackings.ToList();
        }

        public List<OrderStateTracking> GetOrderStateTrackingList()
        {
            return DbContext.OrderStateTrackings.ToList();
        }

        public IQueryable<OrderStateTracking> GetOrderStateTrackingsByOrderId(int orderId)
        {
            var orderStateTrackings = this.DbContext.OrderStateTrackings.Where(x => x.OrderId == orderId).OrderBy(x => x.StateId);
            return orderStateTrackings;
        }
        public List<OrderStateTracking> GetOrderStateTrackingByNcrId(int Ncrid)
        {
            return this.DbContext.OrderStateTrackings.Where(x => x.NcrId == Ncrid).ToList();
        }
        public void DeleteOrderStateTracking(OrderStateTracking entity)
        {
            base.Delete(entity);
            this.DbContext.Commit();
        }


    }
}
