using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Model.Models
{
    public class Country
    {
        public int Id { get; set; }

        [Display(Name = "Country Name")]
        [StringLength(50)]
        public string CountryName { get; set; }

        [Display(Name = "Country Code")]
        [StringLength(2)]
        public string CountryCode { get; set; }
    }
}
