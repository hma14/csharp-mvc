using System.Collections.Generic;
using System.Linq;
using Omnae.Model.Models;

namespace Omnae.BusinessLayer.Models
{
    public class CustomerDashboardViewModel
    {
        public List<TaskViewModel> NewlyUpdates { get; set; }
        public List<TaskViewModel> OrderTrackings { get; set; }
        public List<TaskViewModel> FirstOrders { get; set; }
        public List<TaskViewModel> ReOrders { get; set; }

        public CustomerDashboardViewModel()
        {
            NewlyUpdates = new List<TaskViewModel>();
            OrderTrackings = new List<TaskViewModel>();
            FirstOrders = new List<TaskViewModel>();
            ReOrders = new List<TaskViewModel>();
        }

        public bool HasData => FirstOrders.Any() || OrderTrackings.Any() || NewlyUpdates.Any() || ReOrders.Any();
    }
}