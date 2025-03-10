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
    public class BidRFQStatusRepository : RepositoryBase<BidRFQStatus>, IBidRFQStatusRepository
    {
        public BidRFQStatusRepository(OmnaeContext dbContext) : base(dbContext)
        {

        }

        public int AddBidRFQStatus(BidRFQStatus entity)
        {
            base.Add(entity);
            this.DbContext.Commit();
            return entity.Id;
        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        public BidRFQStatus GetBidRFQStatusById(int Id)
        {
            return this.DbContext.BidRFQStatus
                .Include("PartRevision")
                .Include("RFQActionReason")
                .Where(x => x.Id == Id).FirstOrDefault();
        }

        public IQueryable<BidRFQStatus> GetBidRFQStatusByProductId(int productId)
        {
            return this.DbContext.BidRFQStatus
                .Include("PartRevision")
                .Include("RFQActionReason")
                .Where(x => x.ProductId == productId);
        }

        public List<BidRFQStatus> GetBidRFQStatusListByProductId(int productId)
        {
            return this.DbContext.BidRFQStatus
                .Include("PartRevision")
                .Include("RFQActionReason")
                .Where(x => x.ProductId == productId).ToList();
        }

        public void UpdateBidRFQStatus(BidRFQStatus entity)
        {
            base.Update(entity);
            this.DbContext.Commit();
        }
    }
}
