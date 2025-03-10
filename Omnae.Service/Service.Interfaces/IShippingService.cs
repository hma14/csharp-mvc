using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service.Interfaces
{
    public interface IShippingService
    {
        void Dispose();
        int AddShipping(Shipping entity);
        Shipping FindShippingById(int id);
        Shipping FindShippingByUserId(int companyId);
        void UpdateShipping(Shipping entity);
    }
}
