using Omnae.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.BusinessLayer.Models
{
    public class CreateOrderResult
    {
        public string Message { get; set; }
        public string  CustomerName { get; set; }
        public int  TaskId { get; set; }
        public int OrderId { get; set; }
        public int Term { get; set; }
    }
}
