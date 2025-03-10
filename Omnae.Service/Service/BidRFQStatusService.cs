using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Model.Models;
using Omnae.Service.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service
{
    public class BidRFQStatusService : IBidRFQStatusService
    {
        private readonly IBidRFQStatusRepository repository;
        private readonly IUnitOfWork unitOfWork;

        public BidRFQStatusService(IBidRFQStatusRepository repository, IUnitOfWork unitOfWork)
        {
            this.repository = repository;
            this.unitOfWork = unitOfWork;
        }

        public int AddBidRFQStatus(BidRFQStatus entity)
        {
            return repository.AddBidRFQStatus(entity);
        }

        public void Dispose()
        {
            repository.Dispose();
        }

        public BidRFQStatus FindBidRFQStatusById(int Id)
        {
            return repository.GetBidRFQStatusById(Id);
        }

        public IQueryable<BidRFQStatus> FindBidRFQStatusByProductId(int productId)
        {
            return repository.GetBidRFQStatusByProductId(productId);
        }

        public List<BidRFQStatus> FindBidRFQStatusListByProductId(int productId)
        {
            return repository.GetBidRFQStatusListByProductId(productId);
        }

        public void UpdateBidRFQStatus(BidRFQStatus entity)
        {
            repository.UpdateBidRFQStatus(entity);
        }
    }
}
