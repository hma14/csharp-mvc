using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Omnae.ViewModels
{
    public class GetInspectionReportsViewModel
    {
        public string ProductName { get; set; }
        public string PartNumber { get; set; }
        public string PartNumberRev { get; set; }
        public string ProductDescription { get; set; }
        public int DocId { get; set; }
        public Document Doc { get; set; }
    }
}