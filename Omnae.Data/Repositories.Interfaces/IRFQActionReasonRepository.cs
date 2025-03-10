using Omnae.Common;
using Omnae.Data.Abstracts;
using Omnae.Model.Models;
using System.Collections.Generic;
using System.Linq;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface IRFQActionReasonRepository : IRepository<RFQActionReason>
    {
        void Dispose();
        int AddRFQActionReason(RFQActionReason entity);
        RFQActionReason GetRFQActionReasonById(int Id);
        IQueryable<RFQActionReason> GetRFQActionReasonsByProductId(int productId);
        List<RFQActionReason> GetRFQActionReasonsListByProductId(int productId);
        RFQActionReason GetRFQActionReasonListByProductIdVendorId(int productId, int vendorId, REASON_TYPE reasonType);

        void UpdateRFQActionReason(RFQActionReason entity);
    }
}
