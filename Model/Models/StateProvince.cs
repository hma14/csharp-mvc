using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Model.Models
{
    public class StateProvince
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name ="State or Province Name")]
        public string Name { get; set; }

        [MaxLength(2)]
        public string Abbreviation { get; set; }
        public int? Code { get; set; }
        public string Other { get; set; }
    }
}
