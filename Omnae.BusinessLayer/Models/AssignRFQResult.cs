using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.BusinessLayer.Models
{
    public class AssignRFQResult
    {
        public bool alreadySelected { get; set; }
        public int productId { get; set; }
        public int[] vendors { get; set; }
    }
}
