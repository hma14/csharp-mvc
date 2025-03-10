using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.ShippingAPI.DHL.Models
{
    public class ResponseQuote
    {
        [Key]
        public int Id { get; set; }
        public BkgDetailsResp BkgDetails { get; set; }
        public List<Service> Srvs { get; set; }
    }
}
