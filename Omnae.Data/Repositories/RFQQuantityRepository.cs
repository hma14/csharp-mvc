using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Repositories
{
    public class RFQQuantityRepository : RepositoryBase<RFQQuantity>, IRFQQuantityRepository
    {
        public RFQQuantityRepository(OmnaeContext dbContext) : base(dbContext)
        {

        }

        public int AddRFQQuantity(RFQQuantity entity)
        {
            base.Add(entity);
            this.DbContext.Commit();
            return entity.Id;
        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        public RFQQuantity GetRFQQuantityById(int Id)
        {
            return this.DbContext.RFQQuantities.Where(x => x.Id == Id).FirstOrDefault();
        }

        public List<RFQQuantity> GetRFQQuantityList()
        {
            return this.DbContext.RFQQuantities.ToList();
        }

        public void UpdateRFQQuantity(RFQQuantity entity)
        {
            base.Update(entity);
            this.DbContext.Commit();
        }
    }
}
