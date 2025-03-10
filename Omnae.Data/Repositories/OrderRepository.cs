using System;
using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Model.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Omnae.Common;
using Omnae.Data.Model;

namespace Omnae.Data.Repositories
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(OmnaeContext dbContext) : base(dbContext)
        {

        }
        public int AddOrder(Order entity)
        {
            base.Add(entity);
            this.DbContext.Commit();
            return entity.Id;
        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        public Order GetOrderById(int Id)
        {
            return this.DbContext.Orders
                .Include("Product")
                .Where(x => x.Id == Id).FirstOrDefault();
        }

        public List<Order> GetOrderListByCustomerId(int customerId)
        {
            var orders = this.DbContext.Orders
                .Where(x => x.CustomerId != null ? x.CustomerId == customerId : true)
                .Where(x => x.Product.CustomerId == customerId && x.ProductSharingId == null ||
                            x.ProductSharingId != null && x.ProductSharing.SharingCompanyId == customerId)
                .Include("TaskData")
                .Include("Product")
                .Include("ProductSharing");
            orders = orders.OrderBy(o => o.OrderDate);
            return orders.ToList();
        }      

        public List<Order> GetOrderListByVendorId(int vendorId)
        {
            var orders = this.DbContext.Orders.Where(x => x.Product.VendorId == vendorId);
            orders = orders.OrderBy(o => o.OrderDate);
            return orders.ToList();
        }
        public List<Order> GetOrderByTaskId(int taskId)
        {
            var orders = this.DbContext.Orders.Where(x => x.TaskId == taskId);
            orders = orders.OrderBy(o => o.OrderDate);
            return orders.ToList();
        }
        public IQueryable<Order> GetOrdersByCompanyId(int companyId)
        {
            var orders = this.DbContext.Orders
                                       .Where(x => x.TaskId != null &&
                                                   x.PartNumber != null &&
                                                   (x.Product.CustomerId == companyId ||
                                                    x.ProductSharingId != null && x.ProductSharing.SharingCompanyId == companyId ||
                                                    x.Product.VendorId == companyId))
                                       .Include("TaskData")
                                       .Include("Product")
                                       .Include("ProductSharing")
                                       .OrderBy(o => o.OrderDate);
            return orders;
        }

        public IQueryable<Order> GetOrdersByCustomerId(int customerId)
        {
            var orders = this.DbContext.Orders
                                       .Where(x => x.CustomerId != null ? x.CustomerId == customerId : true)
                                       .Where(x => x.Product.CustomerId == customerId || x.ProductSharingId != null && x.ProductSharing.SharingCompanyId == customerId)
                                       .Include("TaskData")
                                       .Include("Product")
                                       .Include("ProductSharing")
                                       .OrderBy(o => o.OrderDate);
            return orders;
        }

        public IQueryable<Order> GetOrdersAndSharedByCustomerId(int companyId)
        {
            var orders = this.DbContext.Orders
                                       .Where(x => x.TaskId != null &&
                                                   x.PartNumber != null &&
                                                   (x.Product.CustomerId == companyId || x.ProductSharingId != null))
                                       .Include("TaskData")
                                       .Include("Product")
                                       .Include("ProductSharing")
                                       .OrderBy(o => o.OrderDate);
            return orders;
        }
        public IQueryable<Order> GetOrdersByVendorId(int companyId)
        {
            var orders = this.DbContext.Orders
                                       .Where(x => x.TaskId != null && x.PartNumber != null && x.Product.VendorId == companyId)
                                       .Include("TaskData")
                                       .Include("Product")
                                       .OrderBy(o => o.OrderDate);
            return orders;
        }

        public IQueryable<Order> GetOrderList()
        {
            return this.DbContext.Orders.Include("Product");
        }


        public void UpdateOrder(Order entity)
        {
            base.Update(entity);
            this.DbContext.Commit();
        }

        public List<Order> GetOrdersByProductId(int Id)
        {
            return this.DbContext.Orders.Where(p => p.ProductId == Id)
                .Include("TaskData")
                .Include("Shipping").ToList();
        }

        public void DeleteOrder(Order entity)
        {
            base.Delete(entity);
            this.DbContext.Commit();
        }

        public IQueryable<OrderHistoryModel> GetOrderHistory(int? customerId = null, int? vendorId = null, DateTime? start = null, DateTime? end = null)
        {
            end = end?.AddHours(23).AddMinutes(59).AddSeconds(59);

            var orders = this.DbContext.Orders
                .Where(x => x.Product.CustomerCompany != null && x.Product.CustomerCompany.isEnterprise == false)
                .Where(x => x.TaskData != null && (x.TaskData.StateId >= (int)States.SampleComplete || x.TaskData.StateId <= (int)States.ProductionComplete))
                .Where(x => x.Product != null)
                .Where(x => customerId == null || x.Product.CustomerId == customerId)
                .Where(x => vendorId == null || x.Product.VendorId == vendorId)
                .Where(x => start == null || x.ShippedDate >= start)
                .Where(x => end == null || x.ShippedDate <= end);

            var query = from o in orders
                        where o.Invoices.Any()
                        let vendorInvoice = o.Invoices.FirstOrDefault(i => i.UserType == (int)USER_TYPE.Vendor && i.OrderId == o.Id)
                        let customerInvoice = o.Invoices.FirstOrDefault(i => i.UserType == (int)USER_TYPE.Customer && i.OrderId == o.Id)
                        where vendorInvoice != null && customerInvoice != null
                        select new OrderHistoryModel
                        {
                            Order = o,
                            VendorInvoice = vendorInvoice,
                            VendorName = vendorInvoice.Company.Name,
                            CustomerInvoice = customerInvoice,
                            CustomerName = customerInvoice.Company.Name
                        };

            return query;
        }      
    }
}
