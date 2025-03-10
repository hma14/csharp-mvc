using Omnae.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.WebApi.DTO
{
    public class TimerSetupDTO
    {

        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public string Interval { get; set; }
        public string CallbackMethod { get; set; }
        public DateTime? TimerStartAt { get; set; }

        //public TimerUnit UnitEnum => (TimerUnit)Enum.Parse(typeof(TimerUnit), TIMER_UNIT.DAY, true);
        public TypeOfTimers TimerType { get; set; }
    }
}