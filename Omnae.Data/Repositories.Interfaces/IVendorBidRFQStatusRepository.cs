using Omnae.Data.Abstracts;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface IVendorBidRFQStatusRepository : IRepository<VendorBidRFQStatus>
    {
        void Dispose();
        int AddVendorBidRFQStatus(VendorBidRFQStatus entity);
        VendorBidRFQStatus GetVendorBidRFQStatusById(int Id);
        IQueryable<VendorBidRFQStatus> GetVendorBidRFQStatusByProductId(int productId);
        List<VendorBidRFQStatus> GetVendorBidRFQStatusListByProductId(int productId);
        IQueryable<VendorBidRFQStatus> GetVendorBidRFQStatusByProductIdVendorId(int productId, int vendorId);
        void UpdateVendorBidRFQStatus(VendorBidRFQStatus entity);
    }
}
