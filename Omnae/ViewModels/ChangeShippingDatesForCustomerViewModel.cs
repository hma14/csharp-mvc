using System;
using System.ComponentModel.DataAnnotations;

namespace Omnae.ViewModels
{
    public class ChangeShippingDatesForCustomerViewModel
    {
        public int CustomerId { get; set; }
        public int? OrderId { get; set; }

        [Display(Name = "Select Date to Change")]
        public DateTime DateToChange { get; set; }
    }
}