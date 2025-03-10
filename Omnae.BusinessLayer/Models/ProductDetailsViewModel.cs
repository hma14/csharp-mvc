using Omnae.Model.Models;

namespace Omnae.BusinessLayer.Models
{
    public class ProductDetailsViewModel
    {
        public ExtraQuantityPartDetailsViewModel ExtraQtyPartDetailsVM { get; set; }
        public int SampleLeadTime { get; set; }
        public int ProdLeadTime { get; set; }
        public int? NumberSampleIncluded { get; set; }
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}