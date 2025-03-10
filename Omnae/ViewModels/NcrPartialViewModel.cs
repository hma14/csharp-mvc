using Omnae.Common;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Omnae.ViewModels
{
    public class NcrPartialViewModel
    {

        public string Customer { get; set; }
        public string ProductPartNo { get; set; }
        public string PartRevisionNo { get; set; }

        [Display(Name = "PO No.")]
        public int? PONumber { get; set; }

        [Display(Name ="Affected Quantity")]
        public int? Quantity { get; set; }

        [Display(Name = "NC Originator")]
        public string NCOriginator { get; set; }

        [Display(Name = "NCR Detected By")]
        //public NC_DETECTED_BY? NCDetectedby { get; set; }
        public string NCDetectedby { get; set; }

        public DateTime? NCDetectedDate { get; set; }

        [Display(Name = "Describe in detail the problem that was detected")]
        public string NCDescription { get; set; }

        public string Expectation { get; set; }

        [Display(Name = "Evidence Image Url")]
        public string EvidenceImageUrl { get; set; }

    }
}