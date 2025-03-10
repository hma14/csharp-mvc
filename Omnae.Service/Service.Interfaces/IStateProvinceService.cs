using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service.Interfaces
{
    public interface IStateProvinceService
    {
        void Dispose();
        List<StateProvince> FindStateProvince();
        List<StateProvince> FindStateProvinceByCode(int code);
        StateProvince FindStateProvinceById(int Id);
        StateProvince FindStateProvinceByName(string name);
        IQueryable<StateProvince> FindStateProvinceList();
        int AddStateProvince(StateProvince entity);
    }
}
