using Omnae.Data.Abstracts;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface IBidRFQStatusRepository : IRepository<BidRFQStatus>
    {
        void Dispose();
        int AddBidRFQStatus(BidRFQStatus entity);
        BidRFQStatus GetBidRFQStatusById(int Id);
        IQueryable<BidRFQStatus> GetBidRFQStatusByProductId(int productId);
        List<BidRFQStatus> GetBidRFQStatusListByProductId(int productId);

        void UpdateBidRFQStatus(BidRFQStatus entity);
    }
}
