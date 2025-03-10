using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.BusinessLayer.Models
{
    public class RFQUpdateQuantityViewModel
    {
        public string UserName { get; set; }
        public string PartNumber { get; set; }
        public string ProductName { get; set; }

        public decimal? Qty1 { get; set; }
        public decimal? Qty2 { get; set; }
        public decimal? Qty3 { get; set; }
        public decimal? Qty4 { get; set; }
        public decimal? Qty5 { get; set; }
        public decimal? Qty6 { get; set; }
        public decimal? Qty7 { get; set; }


    }
}
