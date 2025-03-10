using Omnae.BusinessLayer.Models;
using Omnae.Common;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Omnae.BusinessLayer.Models
{
    public class SetupTimerViewModel
    {
        public int ProductId { get; set; }
        public int TaskId { get; set; }

        public ProductDetailsViewModel ProductDetails { get; set; }

        [Required(ErrorMessage = "Interval is requried")]
        [Display(Name = "Revision Request Timer Interval (days)")]
        public string RevisionRequestTimerInterval { get; set; }

        [Required(ErrorMessage = "Interval is requried")]
        [Display(Name = "Bid Timer Interval (days)")]
        public string BidTimerInterval { get; set; }

        //[Display(Name = "To be expired Timer Interval (days)")]
        //public string ToBeExpiredInterval { get; set; }

        public bool isEnterprise { get; set; }
        public string Interval { get; set; }

        public TypeOfTimers TimerType { get; set; }

    }


}