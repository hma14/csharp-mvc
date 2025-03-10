using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Omnae.WebApi.ViewModels
{
    public class CreateProductShareViewModel
    {
        [Required]
        public int SharerCompanyId { get; set; }
        [Required]
        public int ShareeCompanyId { get; set; }

        public string CreatedByUserId { get; set; }
    }
}