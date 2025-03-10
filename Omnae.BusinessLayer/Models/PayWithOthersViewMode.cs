using Omnae.Model.Models;
using System.ComponentModel.DataAnnotations;

namespace Omnae.ViewModels
{
    public class PayWithOthersViewModel
    {
        [Key]
        public int TaskId { get; set; }
        public int OrderId { get; set; }
        public string CompanyName { get; set; }
        public decimal? Amount { get; set; }
        
        public Address Address { get; set; }

        public bool IsReorder { get; set; }
        public string CustomerPONumber { get; set; }
    }
}