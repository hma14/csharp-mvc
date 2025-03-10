using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.ShippingAPI.DHL.Models
{
    public class Service
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Global Product Code (option)")]
        public string GlobalProductCode { get; set; }
        public List<MrkSrv> MrkSrvs { get; set; }
    }
}
