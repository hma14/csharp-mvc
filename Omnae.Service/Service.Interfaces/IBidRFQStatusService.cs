using Omnae.Data.Abstracts;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service.Interfaces
{
    public interface IBidRFQStatusService
    {
        void Dispose();
        int AddBidRFQStatus(BidRFQStatus entity);
        BidRFQStatus FindBidRFQStatusById(int Id);
        IQueryable<BidRFQStatus> FindBidRFQStatusByProductId(int productId);
        List<BidRFQStatus> FindBidRFQStatusListByProductId(int productId);

        void UpdateBidRFQStatus(BidRFQStatus entity);

    }
}
