using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Service.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omnae.Model.Models;

namespace Omnae.Service.Service
{
    public class RFQBidService : IRFQBidService
    {
        private readonly IRFQBidRepository rFQBidRepository;
        private readonly IUnitOfWork unitOfWork;

        public RFQBidService(IRFQBidRepository rFQBidRepository, IUnitOfWork unitOfWork)
        {
            this.rFQBidRepository = rFQBidRepository;
            this.unitOfWork = unitOfWork;
        }

        public int AddRFQBid(RFQBid entity)
        {
            return rFQBidRepository.AddRFQBid(entity);
        }

        public void DeleteRFQBid(RFQBid entity)
        {
            rFQBidRepository.RemoveRFQBid(entity);
        }

        public void Dispose()
        {
            rFQBidRepository.Dispose();
        }

        public RFQBid FindRFQBidById(int Id)
        {
            return rFQBidRepository.GetRFQBidById(Id);
        }

        public List<RFQBid> FindRFQBidByVendorId(int vendorId)
        {
            return rFQBidRepository.GetRFQBidByVendorId(vendorId);
        }

        public RFQBid FindRFQBidByVendorIdProductId(int vendorId, int productId)
        {
            return rFQBidRepository.GetRFQBidByVendorIdProductId(vendorId, productId);
        }

        public List<RFQBid> FindRFQBidList()
        {
            return rFQBidRepository.GetRFQBidList();
        }

        public List<RFQBid> FindRFQBidListByProductId(int productId)
        {
            return rFQBidRepository.GetRFQBidListByProductId(productId);
        }

        public void UpdateRFQBid(RFQBid entity)
        {
            rFQBidRepository.UpdateRFQBid(entity);
        }
    }
}
