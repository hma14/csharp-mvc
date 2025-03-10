using Omnae.Common;
using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Model.Extentions;
using Omnae.Model.Models;
using System.Collections.Generic;
using System.Linq;

namespace Omnae.Data.Repositories
{
    public class TaskDataRepository : RepositoryBase<TaskData>, ITaskDataRepository
    {
        public TaskDataRepository(OmnaeContext dbContext) : base(dbContext)
        {
        }

        public List<TaskData> GetTaskDataByCustomerId(int customerId, States? state = null)
        {
            var taskData = this.DbContext.TaskDatas
                                         .AsNoTracking()
                                         .Include("Product")
                                         .Include("Product.CustomerCompany")
                                         .Include("Product.PartRevision")
                                         .Include("Product.CustomerCompany")
                                         .Include("Product.VendorCompany")
                                         .Include("Product.PriceBreak")
                                         .Include("Product.RFQQuantity")
                                         .Include("Orders")
                                         .Include("Invoices")
                                         .Include("RFQBid")
                                         .Where(x => x.Product.CustomerId == customerId)
                                         .Where(x => state == null || x.StateId == (int?)state)
                                         .OrderByDescending(x => x.ModifiedUtc)
                                         .ToList();
            return taskData;
        }

        public IQueryable<TaskData> GetTaskDatasByCompanyId(int companyId)
        {
            var taskData = this.DbContext.TaskDatas
                                         .AsNoTracking()
                                         .Include("Product")
                                         .Where(x => x.Product.CustomerId == companyId || x.Product.VendorId == companyId);
            return taskData;
        }

        public IQueryable<TaskData> GetTaskDatasByCustomerId(int customerId)
        {
            var taskData = this.DbContext.TaskDatas
                                         .AsNoTracking()
                                         .Include("Product")
                                         .Where(x => x.Product.CustomerId == customerId);
            return taskData;
        }

        public IQueryable<TaskData> GetTaskDatasByVendorId(int vendorId)
        {
            var taskData = this.DbContext.TaskDatas
                                         .AsNoTracking()
                                         .Include("Product")
                                         .Where(x => x.Product.VendorId == vendorId);
            return taskData;
        }
        public IQueryable<TaskData> GetTaskDatas()
        {
            var taskData = this.DbContext.TaskDatas
                                         .AsNoTracking()
                                         .Include("RFQBid");
            return taskData;
        }
        public List<TaskData> GetTaskDataByVendorId(int vendorId, States? state = null)
        {
            var taskData = this.DbContext.TaskDatas
                                         .AsNoTracking()
                                         .Include("Product")
                                         .Include("Product.CustomerCompany")
                                         .Include("Product.PartRevision")
                                         .Include("Product.CustomerCompany")
                                         .Include("Product.VendorCompany")
                                         .Include("Product.PriceBreak")
                                         .Include("Orders")
                                         .Include("Invoices")
                                         .Include("RFQBid")
                                         .Where(x => (x.RFQBid != null && x.RFQBid.VendorId == vendorId)
                                                  || (x.RFQBid == null && x.Product.VendorId == vendorId))
                                         .Where(x => state == null || x.StateId == (int?)state)
                                         .OrderByDescending(x => x.ModifiedUtc)
                                         .ToList();
            return taskData;
        }

        public List<TaskData> GetTasksThatNeedAdminAtention()
        {
            var taskData = this.DbContext.TaskDatas
                                         .AsNoTracking()
                                         .Include("Product")
                                         .Include("Product.CustomerCompany")
                                         .Include("Product.PartRevision")
                                         .Include("Product.CustomerCompany")
                                         .Include("Product.VendorCompany")
                                         .Include("Product.PriceBreak")
                                         .Include("Orders")
                                         .Include("Invoices")
                                         .Include("RFQBid")
                                         .WhereIsNeedAdminAtention()
                                         .OrderByDescending(x => x.ModifiedUtc)
                                         .ToList();

            return taskData;
        }

        public TaskData GetTaskDataByAdminId(string adminId)
        {
            var taskData = this.DbContext.TaskDatas.AsNoTracking().Where(x => x.Product.AdminId == adminId);
            List<TaskData> list = taskData.OrderByDescending(x => x.ModifiedUtc).ToList();
            return list.FirstOrDefault();
        }

        public List<TaskData> GetTaskDataListByStateId(int stateId)
        {
            var entities = this.DbContext.TaskDatas.AsNoTracking().Where(x => x.StateId == stateId).OrderByDescending(x => x.ModifiedUtc);
            return entities.ToList();
        }

        public List<TaskData> GetTaskDataList()
        {
            return this.DbContext.TaskDatas.ToList();
        }

        public TaskData GetTaskDataById(int Id)
        {
            return this.DbContext.TaskDatas.AsNoTracking()
                .Include("Product")
                .FirstOrDefault(x => x.TaskId == Id);
        }
        public TaskData GetTaskDataByIdWithExtraData(int id)
        {
            return this.DbContext.TaskDatas.AsNoTracking()
                                 .Include("Product")
                                 .Include("Product.CustomerCompany")
                                 .Include("Product.PartRevision")
                                 .Include("Product.CustomerCompany")
                                 .Include("Product.VendorCompany")
                                 .Include("Product.PriceBreak")
                                 .Include("Orders")
                                 .Include("Invoices")
                                 .Include("RFQBid")
                                 .FirstOrDefault(x => x.TaskId == id);
        }

        public TaskData GetTaskDataByProductId(int productId)
        {
            return this.DbContext.TaskDatas.AsNoTracking()
                .Include("Product")
                .Include("Product.CustomerCompany")
                .Include("Product.PartRevision")
                .Include("Product.CustomerCompany")
                .Include("Product.VendorCompany")
                .Include("Product.PriceBreak")
                .Include("Orders")
                .Include("Invoices")
                .Include("RFQBid")
                .FirstOrDefault(x => x.ProductId == productId);
        }

        public TaskData GetOriginalTaskDataByProductId(int productId)
        {
            return this.DbContext.TaskDatas.AsNoTracking()
                .FirstOrDefault(x => x.ProductId == productId && x.RFQBidId == null);
        }

        public List<TaskData> GetTaskDataListByProductId(int productId)
        {
            return this.DbContext.TaskDatas.AsNoTracking()
                .Include("Product")
                .Include("Product.CustomerCompany")
                .Include("Product.PartRevision")
                .Include("Product.CustomerCompany")
                .Include("Product.VendorCompany")
                .Include("Product.PriceBreak")
                .Include("Orders")
                .Include("Invoices")
                .Include("RFQBid")
                .Where(x => x.ProductId == productId).ToList();
        }
        public IQueryable<TaskData> GetTaskDatasByProductId(int productId)
        {
            return this.DbContext.TaskDatas.AsNoTracking()
                .Include("Product")
                .Include("Product.CustomerCompany")
                .Include("Product.PartRevision")
                .Include("Product.CustomerCompany")
                .Include("Product.VendorCompany")
                .Include("Product.PriceBreak")
                .Include("Orders")
                .Include("Invoices")
                .Include("RFQBid")
                .Where(x => x.ProductId == productId);
        }

        public int AddTaskData(TaskData entity)
        {
            base.Add(entity);
            this.DbContext.Commit();  // must call commit here otherwise, entity won't be added
            return entity.TaskId;
        }

        public void UpdateTaskData(TaskData entity)
        {
            base.Update(entity);
            this.DbContext.Commit();
        }

        public void RemoveTaskData(TaskData entity)
        {
#if false
            var entry = this.DbContext.Entry(entity);
            if (entry.State == System.Data.Entity.EntityState.Detached)
            {
                DbContext.TaskDatas.Attach(entity);
            }
#else
            var entry = this.DbContext.TaskDatas.Where(x => x.TaskId == entity.TaskId).FirstOrDefault();
#endif
            if (entry != null)
            {
                base.Delete(entry);
                this.DbContext.Commit();
            }
        }

        //public TaskData GetTaskDataByOrderId(int orderId)
        //{
        //    return this.DbContext.TaskDatas.FirstOrDefault(x => x.OrderId == orderId);
        //}

        public TaskData GetTaskDataByRFQBidId(int bidId)
        {
            return this.DbContext.TaskDatas.FirstOrDefault(x => x.RFQBidId == bidId);
        }
    }
}
