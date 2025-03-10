using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface IShippingRepository
    {
        void Dispose();

        int AddShipping(Shipping entity);
        Shipping GetShippingById(int userId);
        Shipping GetShippingByUserId(int companyId);

        void UpdateShipping(Shipping entity);
    }
}
