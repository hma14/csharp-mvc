using System.Linq;
using Omnae.Data.Abstracts;
using Omnae.Model.Models;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface IShippingProfileRepository : IRepository<ShippingProfile>
    {
        IQueryable<ShippingProfile> ListAllByCompanyId(int companyId);
        void SetShippingProfileAsDefault(ShippingProfile entity);
        void AdjustTheDefaultShippingProfile(int companyId, ShippingProfile shippingProfile);
    }
}