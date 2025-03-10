using Omnae.Model.Models;

namespace Omnae.Data.Model
{
    public class OrderHistoryModel
    {
        public Order Order { get; set; }
        public OmnaeInvoice VendorInvoice { get; set; }
        public string VendorName { get; set; }
        public OmnaeInvoice CustomerInvoice { get; set; }
        public string CustomerName { get; set; }
    }
}