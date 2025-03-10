using Omnae.Common;
using Omnae.Data.Abstracts;
using Omnae.Model.Models;
using System.Linq;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface ITimerSetupRepository : IRepository<TimerSetup>
    {
        int AddTimerSetup(TimerSetup entity);
        void UpdateTimerSetup(TimerSetup entity);
        TimerSetup FindTimerSetupByProductId(int productId);
        TimerSetup FindTimerSetupByProductIdTimerType(int productId, TypeOfTimers type);
        IQueryable<TimerSetup> FindAllTimerSetupsByProductId(int productId);
        IQueryable<TimerSetup> FindAllTimerSetupsByProductIdTimerType(int productId, TypeOfTimers? type);
    }
}
