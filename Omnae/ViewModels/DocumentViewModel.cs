using Omnae.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Omnae.ViewModels
{
    public class DocumentViewModel
    {
        [Required]
        public string Version { get; set; }
       
    }
}