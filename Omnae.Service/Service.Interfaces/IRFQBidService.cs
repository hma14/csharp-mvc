using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service.Interfaces
{
    public interface IRFQBidService
    {
        void Dispose();
        int AddRFQBid(RFQBid entity);
        List<RFQBid> FindRFQBidByVendorId(int vendorId);
        RFQBid FindRFQBidById(int Id);
        RFQBid FindRFQBidByVendorIdProductId(int vendorId, int productId);
        List<RFQBid> FindRFQBidListByProductId(int productId);
        List<RFQBid> FindRFQBidList();
        void UpdateRFQBid(RFQBid entity);

        void DeleteRFQBid(RFQBid entity);
    }
}
