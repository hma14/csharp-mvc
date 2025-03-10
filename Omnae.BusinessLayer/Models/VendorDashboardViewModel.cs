using System.Collections.Generic;
using System.Linq;
using Omnae.Model.Models;

namespace Omnae.BusinessLayer.Models
{
    public class VendorDashboardViewModel
    {
        public List<TaskViewModel> RFQs { get; set; } = new List<TaskViewModel>();
        public List<TaskViewModel> ORDORs { get; set; } = new List<TaskViewModel>();
        public List<TaskViewModel> Billings { get; set; } = new List<TaskViewModel>();
        public List<TaskViewModel> NCRs { get; set; } = new List<TaskViewModel>();
        public List<TaskViewModel> Products { get; set; } = new List<TaskViewModel>();

        public VendorDashboardViewModel()
        {
            List<TaskViewModel> RFQs = new List<TaskViewModel>();
            List<TaskViewModel> ORDORs = new List<TaskViewModel>();
            List<TaskViewModel> Billings = new List<TaskViewModel>();
            List<TaskViewModel> NCRs = new List<TaskViewModel>();
            List<TaskViewModel> Products = new List<TaskViewModel>();
        }

        public bool HasData => RFQs.Any() || ORDORs.Any() || Billings.Any() || NCRs.Any() || Products.Any();
    }
}