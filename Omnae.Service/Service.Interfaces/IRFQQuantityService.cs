using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service.Interfaces
{
    public interface IRFQQuantityService
    {
        void Dispose();
        int AddRFQQuantity(RFQQuantity entity);
        RFQQuantity FindRFQQuantityById(int Id);
        List<RFQQuantity> FindRFQQuantityList();
        void UpdateRFQQuantity(RFQQuantity entity);
    }
}
