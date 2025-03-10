using Omnae.Data.Abstracts;
using Omnae.Model.Models;
using System.Collections.Generic;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface IExtraQuantityRepository : IRepository<ExtraQuantity>
    {
        void Dispose();
        int AddExtraQuantity(ExtraQuantity entity);
        ExtraQuantity GetExtraQuantityById(int Id);
        List<ExtraQuantity> GetExtraQuantityList();
        void UpdateExtraQuantity(ExtraQuantity entity);
    }
}
