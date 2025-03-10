using System;
using Omnae.Common;

namespace Omnae.BusinessLayer.Models
{
    public class StateLastUpdatedViewModel
    {
        public States StateId { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}