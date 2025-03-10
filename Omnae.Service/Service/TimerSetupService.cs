using Omnae.Data.Repositories.Interfaces;
using Omnae.Service.Service.Interfaces;
using Omnae.Model.Models;
using Omnae.Common;
using System.Linq;

namespace Omnae.Service.Service
{
    public class TimerSetupService : ITimerSetupService
    {
        private readonly ITimerSetupRepository _timerSetupRepository;

        public TimerSetupService(ITimerSetupRepository timerSetupRepository)
        {
            this._timerSetupRepository = timerSetupRepository;
        }

        public int AddTimerSetup(TimerSetup entity)
        {
            return _timerSetupRepository.AddTimerSetup(entity);
        }

        public TimerSetup FindTimerSetupByProductId(int productId)
        {
            return _timerSetupRepository.FindTimerSetupByProductId(productId);
        }
        public TimerSetup FindTimerSetupByProductIdTimerType(int productId, TypeOfTimers type)
        {
            return _timerSetupRepository.FindTimerSetupByProductIdTimerType(productId,type);
        }

        public void UpdateTimerSetup(TimerSetup entity)
        {
            _timerSetupRepository.UpdateTimerSetup(entity);
        }

        public IQueryable<TimerSetup> FindAllTimerSetupsByProductIdTimerType(int productId, TypeOfTimers? type = null)
        {
            return _timerSetupRepository.FindAllTimerSetupsByProductIdTimerType(productId, type);
        }
        public IQueryable<TimerSetup> FindAllTimerSetupsByProductId(int productId)
        {
            return _timerSetupRepository.FindAllTimerSetupsByProductId(productId);
        }
    }
}
