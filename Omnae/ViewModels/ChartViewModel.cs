using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.ViewModels
{
    public class ChartViewModel
    {
        public List<string> Products { get; set; }
        public List<string> DateRange { get; set; }
        public List<double> QuantitiesPerPart { get; set; }
        public List<double> NcrsPerPart { get; set; }
    }
}