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
    public class VendorBidRFQStatusRepository : RepositoryBase<VendorBidRFQStatus>, IVendorBidRFQStatusRepository
    {
        public VendorBidRFQStatusRepository(OmnaeContext dbContext) : base(dbContext)
        {

        }

        public int AddVendorBidRFQStatus(VendorBidRFQStatus entity)
        {
            base.Add(entity);
            this.DbContext.Commit();
            return entity.Id;
        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        public VendorBidRFQStatus GetVendorBidRFQStatusById(int Id)
        {
            return this.DbContext.VendorBidRFQStatus
                .Include("BidRFQStatus")
                .Include("BidRequestRevision")
                .Include("RFQActionReason")
                .Where(x => x.Id == Id).FirstOrDefault();
        }

        public IQueryable<VendorBidRFQStatus> GetVendorBidRFQStatusByProductId(int productId)
        {
            return this.DbContext.VendorBidRFQStatus
                .Include("BidRFQStatus")
                .Include("BidRequestRevision")
                .Include("RFQActionReason")
                .Where(x => x.ProductId == productId);
        }

        public IQueryable<VendorBidRFQStatus> GetVendorBidRFQStatusByProductIdVendorId(int productId, int vendorId)
        {
            return this.DbContext.VendorBidRFQStatus
                .Include("BidRFQStatus")
                .Include("BidRequestRevision")
                .Include("RFQActionReason")
                .Where(x => x.ProductId == productId && x.VendorId == vendorId);
        }

        public List<VendorBidRFQStatus> GetVendorBidRFQStatusListByProductId(int productId)
        {
            return this.DbContext.VendorBidRFQStatus
                .Include("BidRFQStatus")
                .Include("BidRequestRevision")
                .Include("RFQActionReason")
                .Where(x => x.ProductId == productId).ToList();
        }

        public void UpdateVendorBidRFQStatus(VendorBidRFQStatus entity)
        {
            base.Update(entity);
            this.DbContext.Commit();
        }
    }
}
