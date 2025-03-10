using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Libs.ViewModels
{
    public class NotifyAdminOrderEmailViewModel
    {
        public string UserName { get; set; }
        public string PartNumber { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string  OrderStatus { get; set; }
        public Document PurchaseOrder{ get; set; }
    }
}
