using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Libs.ViewModels
{
    public class NotifyCustomerOrderEmailViewModel
    {
        public string UserName { get; set; }
        public string PartNumber { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int? LeadTime { get; set; }
        public List<Document> AttachedFiles{ get; set; }
    }
}
