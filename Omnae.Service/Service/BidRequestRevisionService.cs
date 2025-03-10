using Omnae.Data.Abstracts;
using Omnae.Data.Repositories;
using Omnae.Model.Models;
using Omnae.Service.Service.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Omnae.Service.Service
{
    public class BidRequestRevisionService : IBidRequestRevisionService
    {
        private readonly BidRequestRevisionRepository bidRequestRevisionRepository;
        private readonly IUnitOfWork unitOfWork;

        public BidRequestRevisionService(BidRequestRevisionRepository bidRequestRevisionRepository, IUnitOfWork unitOfWork)
        {
            this.bidRequestRevisionRepository = bidRequestRevisionRepository;
            this.unitOfWork = unitOfWork;
        }
        public int AddBidRequestRevision(BidRequestRevision entity)
        {
            return bidRequestRevisionRepository.AddBidRequestRevision(entity);
        }

        public void DeleteBidRequestRevision(BidRequestRevision entity)
        {
            bidRequestRevisionRepository.RemoveBidRequestRevision(entity);
        }

        public void Dispose()
        {
            bidRequestRevisionRepository.Dispose();
        }

        public BidRequestRevision FindBidRequestRevisionById(int Id)
        {
            return bidRequestRevisionRepository.GetBidRequestRevisionById(Id);
        }

        public List<BidRequestRevision> FindBidRequestRevisionList()
        {
            return bidRequestRevisionRepository.GetBidRequestRevisionList();
        }

        public List<BidRequestRevision> FindBidRequestRevisionListByProductId(int productId)
        {
            return bidRequestRevisionRepository.GetBidRequestRevisionListByProductId(productId);
        }
        public List<BidRequestRevision> FindBidRequestRevisionListByProductIdTaskIdRevisionNumber(int productId, int taskId, int revisionNumber)
        {
            return bidRequestRevisionRepository.GetBidRequestRevisionListByProductIdTaskIdRevisionNumber(productId, taskId, revisionNumber);
        }

        public List<BidRequestRevision> FindBidRequestRevisionListByProductTaskId(int productId, int taskId)
        {
            return bidRequestRevisionRepository.GetBidRequestRevisionListByProductIdTaskId(productId, taskId);
        }

        public List<BidRequestRevision> FindBidRequestRevisionListByProductCustomerIdTaskId(int productId, int taskId)
        {
            return bidRequestRevisionRepository.GetBidRequestRevisionListByProductIdCustomerTaskId(productId, taskId);
        }

        public IQueryable<BidRequestRevision> FindRBidRequestRevisiondByVendorIdProductId(int vendorId, int productId)
        {
            return bidRequestRevisionRepository.GetRBidRequestRevisiondByVendorIdProductId(vendorId, productId);
        }
        public List<BidRequestRevision> FindRBidRequestRevisiondByVendorIdProductIdCustomerTaskId(int vendorId, int productId, int taskId)
        {
            return bidRequestRevisionRepository.GetRBidRequestRevisiondByVendorIdProductIdCustomerTaskId(vendorId, productId, taskId);
        }
        public List<BidRequestRevision> FindRBidRequestRevisiondByVendorIdProductIdTaskId(int vendorId, int productId, int taskId)
        {
            return bidRequestRevisionRepository.GetRBidRequestRevisiondByVendorIdProductIdTaskId(vendorId, productId, taskId);
        }

        public void UpdateBidRequestRevision(BidRequestRevision entity)
        {
            bidRequestRevisionRepository.UpdateBidRequestRevision(entity);
        }
    }
}
