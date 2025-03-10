using Omnae.Common;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service.Interfaces
{
    public interface IRFQActionReasonService
    {
        void Dispose();
        int AddRFQActionReason(RFQActionReason entity);
        RFQActionReason FindRFQActionReasonById(int Id);
        IQueryable<RFQActionReason> FindRFQActionReasonsByProductId(int productId);
        List<RFQActionReason> FindRFQActionReasonsListByProductId(int productId);
        RFQActionReason FindRFQActionReasonListByProductIdVendorId(int productId, int vendorId, REASON_TYPE reasonType);
        void UpdateRFQActionReason(RFQActionReason entity);
    }
}
