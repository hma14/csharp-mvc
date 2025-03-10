using Omnae.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.WebApi.DTO
{
    public class GetTimerIntervalsDTO
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public string Interval { get; set; }
        public string CallbackMethod { get; set; }
        public DateTime? TimerStartAt { get; set; }
        public TypeOfTimers TimerType { get; set; }
    }
}