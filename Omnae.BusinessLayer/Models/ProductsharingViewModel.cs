using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.BusinessLayer.Models
{
    public class ProductsharingViewModel
    {
        public int ProductId { get; set; }
        public string ShareeCompanyName { get; set; }
        public string SharerCompanyName { get; set; }
        public string PartNumber { get; set; }
        public string PartDescription { get; set; }
    }
}
