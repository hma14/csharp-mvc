using Omnae.Common;
using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Model.Models;
using System.Linq;

namespace Omnae.Data.Repositories
{
    public class TimerSetupRepository : RepositoryBase<TimerSetup>, ITimerSetupRepository
    {
        public TimerSetupRepository(OmnaeContext dbContext) : base(dbContext)
        {
        }

        public int AddTimerSetup(TimerSetup entity)
        {
            base.Add(entity);
            this.DbContext.Commit();
            return entity.Id;
        }

        public IQueryable<TimerSetup> FindAllTimerSetupsByProductIdTimerType(int productId, TypeOfTimers? type = null)
        {
            if (type != null)
                return this.DbContext.TimerSetups.Where(x => x.ProductId == productId && x.TimerType == type.Value);
            else
                return this.DbContext.TimerSetups.Where(x => x.ProductId == productId);
        }
        public IQueryable<TimerSetup> FindAllTimerSetupsByProductId(int productId)
        {
            return this.DbContext.TimerSetups.Where(x => x.ProductId == productId);
        }

        public TimerSetup FindTimerSetupByProductId(int productId)
        {
            return this.DbContext.TimerSetups.FirstOrDefault(x => x.ProductId == productId);
        }
        public TimerSetup FindTimerSetupByProductIdTimerType(int productId, TypeOfTimers type)
        {
            return this.DbContext.TimerSetups
                .Where(x => x.ProductId == productId && x.TimerType == type)
                .ToList()
                .LastOrDefault();

        }

        public void UpdateTimerSetup(TimerSetup entity)
        {
            base.Update(entity);
            this.DbContext.Commit();
        }
    }
}
