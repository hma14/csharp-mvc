using Humanizer;
using Omnae.Common;
using Omnae.Libs;
using Serilog;

namespace Omnae.BusinessLayer.Services
{
    public class TimerTriggerService
    {
        public TimerTriggerService(ILogger log)
        {
            Log = log;
        }

        public ILogger Log { get; }


        public void StartTimer(string triggerName, TimerUnit unit, int interval, int productId, string timeoutHandler)
        {
#if !DEBUG
            CreateTimerTrigger(triggerName, unit, interval.ToString(), productId, timeoutHandler);
#endif
        }

        public void StartTimer(string triggerName, TimerUnit unit, string interval, int productId, string timeoutHandler)
        {
#if !DEBUG
            CreateTimerTrigger(triggerName, unit, interval, productId, timeoutHandler);
#endif
        }
        
        private void CreateTimerTrigger(string triggerName, TimerUnit unit, string interval, int productId, string timeoutHandler)
        {
            Log.Information("ATrigger - {Method} - Name:{triggerName}, Unit:{unit}, Interval:{interval}, ProductId:{productId}, TimeoutHandler:{timeoutHandler}", "CreateTimerTrigger", triggerName, unit, interval, productId, timeoutHandler);

            ATriggerApi.Remove(triggerName);
            ATriggerApi.Create(triggerName, unit.Humanize(), interval, productId, timeoutHandler);
        }

        public void RemoveTimerTrigger(string triggerName)
        {
            Log.Information("ATrigger - {Method} - Name:{triggerName}", "RemoveTimerTrigger", triggerName);

            ATriggerApi.Remove(triggerName);
        }

        private  void StopTimerTrigger(string triggerName)
        {
            ATriggerApi.Pause(triggerName);
        }

        private void ResumeTimerTrigger(string triggerName)
        {
            ATriggerApi.Resume(triggerName);
        }
    }
}