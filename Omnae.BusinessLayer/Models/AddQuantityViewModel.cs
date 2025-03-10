using System.Collections.Generic;
using Omnae.Model.Models;

namespace Omnae.BusinessLayer.Models
{
    public class AddQuantityViewModel
    {
        public int ProductId { get; set; }
        public int TaskId { get; set; }

        public Product Product { get; set; }
        public TaskData task { get; set; }

        public List<int?> QuantityList { get; set; }
        public ProductDetailsViewModel  ProductDetailsVM { get; set; }
    }
}