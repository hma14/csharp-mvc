using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Omnae.Model.Models;

namespace Omnae.ViewModels
{
    public class InspectionReportViewModel
    {
        public int VendorId { get; set; }
        public int DocmentId { get; set; }
        public string InspectionReportUri { get; set; }
        public DateTime InspectionReportDate { get; set; }

        public SelectList Vendors { get; set; }
    }
}