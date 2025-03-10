using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Omnae.Common;

namespace Omnae.Model.Models
{
    public class TimerSetup
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public string Interval { get; set; }
        public string CallbackMethod { get; set; }
        public DateTime? TimerStartAt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime? _updatedAt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime? _createdAt { get; set; }


        public TimerUnit UnitEnum => (TimerUnit) Enum.Parse(typeof(TimerUnit), TIMER_UNIT.DAY, true);
        public TypeOfTimers TimerType { get; set; }
    }
}
