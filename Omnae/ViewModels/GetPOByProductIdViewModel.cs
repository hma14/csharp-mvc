using Omnae.Model.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Omnae.ViewModels
{
    public class GetPOByProductIdViewModel
    {
        [Display(Name ="Product ID")]
        public int ProductId { get; set; }
        public List<Document> DocList { get; set; }

    }
}