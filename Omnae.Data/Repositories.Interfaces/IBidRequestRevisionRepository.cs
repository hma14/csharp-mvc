using Omnae.Data.Abstracts;
using Omnae.Model.Models;
using System.Collections.Generic;
using System.Linq;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface IBidRequestRevisionRepository : IRepository<BidRequestRevision>
    {
        void Dispose();
        int AddBidRequestRevision(BidRequestRevision entity);
        List<BidRequestRevision> GetBidRequestRevisionListByProductId(int productId);
        BidRequestRevision GetBidRequestRevisionById(int Id);
        IQueryable<BidRequestRevision> GetRBidRequestRevisiondByVendorIdProductId(int vendorId, int productId);
        List<BidRequestRevision> GetBidRequestRevisionList();
        void UpdateBidRequestRevision(BidRequestRevision entity);
        void RemoveBidRequestRevision(BidRequestRevision entity);
        List<BidRequestRevision> GetBidRequestRevisionListByProductIdCustomerTaskId(int productId, int taskId);
        List<BidRequestRevision> GetRBidRequestRevisiondByVendorIdProductIdCustomerTaskId(int vendorId, int productId, int taskId);
        List<BidRequestRevision> GetBidRequestRevisionListByProductIdTaskIdRevisionNumber(int productId, int taskId, int revisionNumber);
        List<BidRequestRevision> GetRBidRequestRevisiondByVendorIdProductIdTaskId(int vendorId, int productId, int taskId);
        List<BidRequestRevision> GetBidRequestRevisionListByProductIdTaskId(int productId, int taskId);
    }
}
