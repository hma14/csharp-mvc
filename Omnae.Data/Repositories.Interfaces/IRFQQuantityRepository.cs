using Omnae.Data.Abstracts;
using Omnae.Model.Models;
using System.Collections.Generic;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface IRFQQuantityRepository : IRepository<RFQQuantity>
    {
        void Dispose();
        int AddRFQQuantity(RFQQuantity entity);
        RFQQuantity GetRFQQuantityById(int Id);
        List<RFQQuantity> GetRFQQuantityList();
        void UpdateRFQQuantity(RFQQuantity entity);
    }
}
