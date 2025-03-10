using Omnae.Common;
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
    public class BidRequestRevisionRepository : RepositoryBase<BidRequestRevision>, IBidRequestRevisionRepository
    {
        public BidRequestRevisionRepository(OmnaeContext dbContext) : base(dbContext)
        {

        }
        public int AddBidRequestRevision(BidRequestRevision entity)
        {
            base.Add(entity);
            this.DbContext.Commit();
            return entity.Id;
        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        public BidRequestRevision GetBidRequestRevisionById(int Id)
        {
            return this.DbContext.BidRequestRevisions
                .Include("VendorTaskData")
                .Include("RFQActionReason")
                .Where(x => x.Id == Id).FirstOrDefault();
        }

        public List<BidRequestRevision> GetBidRequestRevisionListByProductId(int productId)
        {
            var rr = this.DbContext.BidRequestRevisions
                .Include("VendorTaskData")
                .Include("RFQActionReason")
                .Where(x => x.ProductId == productId && x.VendorTaskData.StateId != (int) States.VendorRejectedRFQ);
            return rr.ToList();
        }

        public List<BidRequestRevision> GetBidRequestRevisionListByProductIdTaskIdRevisionNumber(int productId, int taskId, int revisionNumber)
        {
            var rr = this.DbContext.BidRequestRevisions
                .Include("VendorTaskData")
                .Include("RFQActionReason")
                .Where(x => x.ProductId == productId && x.TaskId == taskId && x.RevisionNumber == revisionNumber);
            return rr.ToList();
        }

        public List<BidRequestRevision> GetBidRequestRevisionListByProductIdTaskId(int productId, int taskId)
        {
            var rr = this.DbContext.BidRequestRevisions
                .Include("VendorTaskData")
                .Include("RFQActionReason")
                .Where(x => x.ProductId == productId && x.TaskId == taskId);
            return rr.ToList();
        }
        public List<BidRequestRevision> GetBidRequestRevisionListByProductIdCustomerTaskId(int productId, int taskId)
        {
            var rr = this.DbContext.BidRequestRevisions
                .Include("VendorTaskData")
                .Include("RFQActionReason")
                .Where(x => x.ProductId == productId && x.CustomerTaskId == taskId);
            return rr.ToList();
        }

        public List<BidRequestRevision> GetBidRequestRevisionList()
        {
            return this.DbContext.BidRequestRevisions.ToList();
        }

        public List<BidRequestRevision> GetRBidRequestRevisiondByVendorIdProductIdCustomerTaskId(int vendorId, int productId, int taskId)
        {
            return this.DbContext.BidRequestRevisions
                .Include("VendorTaskData")
                .Include("RFQActionReason")
                .Where(x => x.VendorId == vendorId && x.ProductId == productId && x.CustomerTaskId == taskId).ToList();
        }
        public List<BidRequestRevision> GetRBidRequestRevisiondByVendorIdProductIdTaskId(int vendorId, int productId, int taskId)
        {
            return this.DbContext.BidRequestRevisions
                .Include("VendorTaskData")
                .Include("RFQActionReason")
                .Where(x => x.VendorId == vendorId && x.ProductId == productId && x.TaskId == taskId).ToList();
        }


        public IQueryable<BidRequestRevision> GetRBidRequestRevisiondByVendorIdProductId(int vendorId, int productId)
        {
            return this.DbContext.BidRequestRevisions
                .Include("VendorTaskData")
                .Include("RFQActionReason")
                .Where(x => x.VendorId == vendorId && x.ProductId == productId); 
        }


        public void RemoveBidRequestRevision(BidRequestRevision entity)
        {
            var entry = this.DbContext.Entry(entity);
            if (entry.State == System.Data.Entity.EntityState.Detached)
                DbContext.BidRequestRevisions.Attach(entity);
            base.Delete(entity);
            this.DbContext.Commit();
        }

        public void UpdateBidRequestRevision(BidRequestRevision entity)
        {
            base.Update(entity);
            this.DbContext.Commit();
        }
    }
}
