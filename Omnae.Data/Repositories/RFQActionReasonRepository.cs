using Omnae.Common;
using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Model.Models;
using System.Collections.Generic;
using System.Linq;

namespace Omnae.Data.Repositories
{
    public class RFQActionReasonRepository : RepositoryBase<RFQActionReason>, IRFQActionReasonRepository
    {
        public RFQActionReasonRepository(OmnaeContext dbContext) : base(dbContext)
        {
        }

        public int AddRFQActionReason(RFQActionReason entity)
        {
            base.Add(entity);
            this.DbContext.Commit();
            return entity.Id;
        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        public RFQActionReason GetRFQActionReasonById(int Id)
        {
            return this.DbContext.RFQActionReasons.FirstOrDefault(x => x.Id == Id);
        }

        public List<RFQActionReason> GetRFQActionReasonsListByProductId(int productId)
        {
            return this.DbContext.RFQActionReasons.Where(x => x.ProductId == productId).ToList();
        }
        public RFQActionReason GetRFQActionReasonListByProductIdVendorId(int productId, int vendorId, REASON_TYPE reasonType)
        {
            return this.DbContext.RFQActionReasons.FirstOrDefault(x => x.ProductId == productId && x.VendorId == vendorId && x.ReasonType == reasonType);
        }

        public IQueryable<RFQActionReason> GetRFQActionReasonsByProductId(int productId)
        {
            return this.DbContext.RFQActionReasons.Where(x => x.ProductId == productId);
        }

        public void UpdateRFQActionReason(RFQActionReason entity)
        {
            base.Update(entity);
            this.DbContext.Commit();
        }
    }
}
