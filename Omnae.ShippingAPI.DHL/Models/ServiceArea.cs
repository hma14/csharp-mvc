using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.ShippingAPI.DHL.Models
{
    public class ServiceArea
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Facility Code")]
        public string FacilityCode { get; set; }

        [Display(Name = "Service Area Code")]
        public string ServiceAreaCode { get; set; }
    }
}
