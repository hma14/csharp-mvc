using Omnae.Model.Models;
using System.Collections.Generic;

namespace Omnae.BusinessLayer.Models
{
    public class QualifiedVendorsViewModel
    {
        public QualifiedVendorsViewModel()
        {
            Charts = new List<ChartTypeViewModel>();
        }
        public int ProductId { get; set; }
        public int[] VendorIds { get; set; }
        public string[] Name { get; set; }
        public Address[] Address { get; set; }
        public List<ChartTypeViewModel> Charts { get; set; }
    }  
}