using Omnae.Common;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Model.Models;
using Omnae.Service.Service.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Omnae.Service.Service
{
    public class RFQActionReasonService : IRFQActionReasonService
    {
        private IRFQActionReasonRepository Repository { get; }

        public RFQActionReasonService(IRFQActionReasonRepository repository)
        {
            this.Repository = repository;
        }

        public int AddRFQActionReason(RFQActionReason entity)
        {
            return Repository.AddRFQActionReason(entity);
        }

        public void Dispose()
        {
            Repository.Dispose();
        }

        public RFQActionReason FindRFQActionReasonById(int Id)
        {
            return Repository.GetRFQActionReasonById(Id);
        }

        public List<RFQActionReason> FindRFQActionReasonsListByProductId(int productId)
        {
            return Repository.GetRFQActionReasonsListByProductId(productId);
        }
        public RFQActionReason FindRFQActionReasonListByProductIdVendorId(int productId, int vendorId, REASON_TYPE reasonType)
        {
            return Repository.GetRFQActionReasonListByProductIdVendorId(productId, vendorId, reasonType);
        }

        public IQueryable<RFQActionReason> FindRFQActionReasonsByProductId(int productId)
        {
            return Repository.GetRFQActionReasonsByProductId(productId);
        }

        public void UpdateRFQActionReason(RFQActionReason entity)
        {
            Repository.UpdateRFQActionReason(entity);
        }
    }
}
