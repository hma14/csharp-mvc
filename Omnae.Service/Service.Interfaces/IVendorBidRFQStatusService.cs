using Omnae.Data.Abstracts;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service.Interfaces
{
    public interface IVendorBidRFQStatusService 
    {
        void Dispose();
        int AddVendorBidRFQStatus(VendorBidRFQStatus entity);
        VendorBidRFQStatus FindVendorBidRFQStatusById(int Id);
        IQueryable<VendorBidRFQStatus> FindVendorBidRFQStatusByProductId(int productId);
        List<VendorBidRFQStatus> FindVendorBidRFQStatusListByProductId(int productId);
        IQueryable<VendorBidRFQStatus> FindVendorBidRFQStatusByProductIdVendorId(int productId, int vendorId);
        void UpdateVendorBidRFQStatus(VendorBidRFQStatus entity);
    }
}
