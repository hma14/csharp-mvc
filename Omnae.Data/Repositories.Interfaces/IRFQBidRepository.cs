using Omnae.Data.Abstracts;
using Omnae.Model.Models;
using System.Collections.Generic;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface IRFQBidRepository : IRepository<RFQBid>
    {
        void Dispose();
        int AddRFQBid(RFQBid entity);
        List<RFQBid> GetRFQBidByVendorId(int vendorId);
        RFQBid GetRFQBidById(int Id);
        RFQBid GetRFQBidByVendorIdProductId(int vendorId, int productId);
        List<RFQBid> GetRFQBidList();
        List<RFQBid> GetRFQBidListByProductId(int productId);
        void UpdateRFQBid(RFQBid entity);
        void RemoveRFQBid(RFQBid entity);
    }
}
