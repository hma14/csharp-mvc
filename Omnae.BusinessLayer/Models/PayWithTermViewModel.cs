using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Omnae.ViewModels
{
    public class PayWithTermViewModel
    {
        [Key]
        public int TaskId { get; set; }
        public int OrderId { get; set; }
        public string CompanyName { get; set; }
        public decimal? Amount { get; set; }
        public int? Term { get; set; }

        public bool IsReorder { get; set; }
        public string CustomerPONumber { get; set; }

    }
}