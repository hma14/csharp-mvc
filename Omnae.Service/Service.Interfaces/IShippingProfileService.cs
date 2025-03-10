using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service.Interfaces
{
    public interface IShippingProfileService
    {
        int Add(ShippingProfile entity);
        ShippingProfile FindById(int id);
        void Update(ShippingProfile entity);
        IQueryable<ShippingProfile> FindAllByCompanyId(int companyId);
        void Delete(ShippingProfile profileInDatabase);
        void SetShippingProfileAsDefault(ShippingProfile entity);
    }
}
