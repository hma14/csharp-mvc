using Omnae.Model.Models;
using Omnae.Data.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface IStateProvinceRepository : IRepository<StateProvince>
    {
        void Dispose();
        List<StateProvince> GetStateProvince();
        List<StateProvince> GetStateProviceByCode(int code);

        StateProvince FindStateProvinceById(int Id);
        StateProvince FindStateProvinceByName(string name);
        IQueryable<StateProvince> GetStateProvinceList();
        int AddStateProvince(StateProvince entity);
    }
}
