using System;
using System.Collections.Generic;
using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Model.Models;
using System.Linq;

namespace Omnae.Data.Repositories
{
    public class RFQBidRepository : RepositoryBase<RFQBid>, IRFQBidRepository
    {
        public RFQBidRepository(OmnaeContext dbContext) : base(dbContext)
        {

        }

        public int AddRFQBid(RFQBid entity)
        {
            base.Add(entity);
            this.DbContext.Commit();
            return entity.Id;
        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        public RFQBid GetRFQBidById(int Id)
        {
            return this.DbContext.RFQBids.Where(x => x.Id == Id).FirstOrDefault();
        }

        public List<RFQBid> GetRFQBidByVendorId(int vendorId)
        {
            var rfqbids = this.DbContext.RFQBids.Where(x => x.VendorId == vendorId);
            rfqbids = rfqbids.OrderByDescending(o => o.BidDatetime);
            return rfqbids.ToList();
        }

        public RFQBid GetRFQBidByVendorIdProductId(int vendorId, int productId)
        {
            var rfqbids = this.DbContext.RFQBids.Where(x => x.VendorId == vendorId && x.ProductId == productId);
            return rfqbids.FirstOrDefault();
        }

        public List<RFQBid> GetRFQBidList()
        {
            return this.DbContext.RFQBids.ToList();
        }

        public List<RFQBid> GetRFQBidListByProductId(int productId)
        {
            return this.DbContext.RFQBids.Where(x => x.ProductId == productId).ToList();
        }

        public void UpdateRFQBid(RFQBid entity)
        {
            base.Update(entity);
            this.DbContext.Commit();
        }

        public void RemoveRFQBid(RFQBid entity)
        {
            var entry = this.DbContext.RFQBids.Where(x => x.Id == entity.Id).FirstOrDefault();
            if (entry != null)
            {
                base.Delete(entry);
                this.DbContext.Commit();
            }
        }
    }
}
