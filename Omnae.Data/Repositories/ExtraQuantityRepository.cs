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
    public class ExtraQuantityRepository : RepositoryBase<ExtraQuantity>, IExtraQuantityRepository
    {
        public ExtraQuantityRepository(OmnaeContext dbContext) : base(dbContext)
        {

        }

        public int AddExtraQuantity(ExtraQuantity entity)
        {
            base.Add(entity);
            this.DbContext.Commit();
            return entity.Id;
        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        public ExtraQuantity GetExtraQuantityById(int Id)
        {
            return this.DbContext.ExtraQuantities.Where(x => x.Id == Id).FirstOrDefault();
        }

        public List<ExtraQuantity> GetExtraQuantityList()
        {
            return this.DbContext.ExtraQuantities.ToList();
        }

        public void UpdateExtraQuantity(ExtraQuantity entity)
        {
            base.Update(entity);
            this.DbContext.Commit();
        }
    }
}
