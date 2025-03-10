using Omnae.Model.Models;
using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Repositories
{
    public class StateProvinceRepository : RepositoryBase<StateProvince>, IStateProvinceRepository
    {
        public StateProvinceRepository(OmnaeContext dbContext) : base(dbContext)
        {

        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        public int AddStateProvince(StateProvince entity)
        {
            base.Add(entity);
            this.DbContext.Commit(); 
            return entity.Id;
        }

        public StateProvince FindStateProvinceById(int Id)
        {
            return this.DbContext.StateProvinces.Where(x => x.Id == Id).FirstOrDefault();
        }

        public StateProvince FindStateProvinceByName(string name)
        {
            return this.DbContext.StateProvinces.Where(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
        }

        public List<StateProvince> GetStateProviceByCode(int code)
        {
            return this.DbContext.StateProvinces.Where(x => x.Code == code).ToList();
        }

        public List<StateProvince> GetStateProvince()
        {
            return this.DbContext.StateProvinces.ToList();
        }

        public IQueryable<StateProvince> GetStateProvinceList()
        {
            return this.DbContext.StateProvinces;
        }
    }
}
