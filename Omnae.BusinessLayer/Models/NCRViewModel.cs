using Omnae.Common;
using Omnae.Model.Models;
using System.Collections.Generic;

namespace Omnae.BusinessLayer.Models
{   
    public class NCRViewModel
    {
        public ChartTypeViewModel ChartType { get; set; }
        public List<NcrInfoViewModel> NcrInfoList { get; set; }
        public List<Order> Orders { get; set; }
        public List<Product> Products { get; set; }
    }
}