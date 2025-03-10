using Omnae.Model.Models;
using System.Collections.Generic;

namespace Omnae.BusinessLayer.Models
{
    public class AssignRFQToVendorsViewModel
    {
        public AssignRFQToVendorsViewModel()
        {
            Charts = new List<ChartTypeViewModel>();
        }
        public int ProductId { get; set; }
        public bool isEnterprise { get; set; }
        public bool[] isChosen { get; set; }
        public int[] VendorIds { get; set; }
        public string[] Name { get; set; }
        public Address[] Address { get; set; }
        public List<ChartTypeViewModel> Charts { get; set; }
    }  
}