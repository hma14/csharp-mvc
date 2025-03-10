using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service.Interfaces
{
    public interface IBidRequestRevisionService
    {
        void Dispose();
        int AddBidRequestRevision(BidRequestRevision entity);
        List<BidRequestRevision> FindBidRequestRevisionListByProductId(int productId);
        BidRequestRevision FindBidRequestRevisionById(int Id);
        IQueryable<BidRequestRevision> FindRBidRequestRevisiondByVendorIdProductId(int vendorId, int productId);
        List<BidRequestRevision> FindBidRequestRevisionList();
        void UpdateBidRequestRevision(BidRequestRevision entity);
        void DeleteBidRequestRevision(BidRequestRevision entity);
        List<BidRequestRevision> FindBidRequestRevisionListByProductCustomerIdTaskId(int productId, int taskId);
        List<BidRequestRevision> FindRBidRequestRevisiondByVendorIdProductIdCustomerTaskId(int vendorId, int productId, int taskId);
        List<BidRequestRevision> FindBidRequestRevisionListByProductIdTaskIdRevisionNumber(int productId, int taskId, int revisionNumber);
        List<BidRequestRevision> FindRBidRequestRevisiondByVendorIdProductIdTaskId(int vendorId, int productId, int taskId);
        List<BidRequestRevision> FindBidRequestRevisionListByProductTaskId(int productId, int taskId);
    }
}
