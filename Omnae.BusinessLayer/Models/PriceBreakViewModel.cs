using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Omnae.Model.Models;

namespace Omnae.BusinessLayer.Models
{
    public class PriceBreakViewModel
    {

        [Required]
        public int ProductId { get; set; }

        public string ProductName { get; set; }
        public string ProductAvatarUri { get; set; }

        //[Required]
        //public int? Quantity { get; set; }

        //[Required]
        //[Display(Name = "Unit Price")]
        //public decimal? UnitPrice { get; set; }

        public List<PriceBreak> PriceBreakList { get; set; }
    }
}