using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Omnae.ViewModels
{
    public class ChangeEstForCustomerViewModel
    {
        public int CustomerId { get; set; }
        public int? OrderId { get; set; }

        [Display(Name = "Select Date to Change")]
        public DateTime DateToChange { get; set; }
        public SelectList OrderList { get; set; }
    }
}