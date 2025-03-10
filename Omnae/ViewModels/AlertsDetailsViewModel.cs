using Omnae.Common;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Omnae.BusinessLayer.Models;
using Omnae.Models;

namespace Omnae.ViewModels
{
    public class AlertsDetailsViewModel
    {
    
        public int TaskId { get; set; }
        public States StateId { get; set; }

        public Product Product { get; set; }
        public bool? IsTagged { get; set; }
        public StateLastUpdatedViewModel LastUpdated { get; set; }

        public List<OrderStateTracking> OrderStateTrackings { get; set; }

       
    }
}