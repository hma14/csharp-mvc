using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Omnae.ViewModels
{
    public class ResponseViewModel
    {
        [Key]
        public int ProductId { get; set; }
        public bool Forward { get; set; }
        public bool Back { get; set; }
    }
}