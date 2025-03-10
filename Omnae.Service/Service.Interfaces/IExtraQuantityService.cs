using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service.Interfaces
{
    public interface IExtraQuantityService
    {
        void Dispose();
        int AddExtraQuantity(ExtraQuantity entity);
        ExtraQuantity FindExtraQuantityById(int Id);
        List<ExtraQuantity> FindExtraQuantityList();
        void UpdateExtraQuantity(ExtraQuantity entity);
    }
}
