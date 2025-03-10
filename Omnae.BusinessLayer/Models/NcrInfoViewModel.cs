using Omnae.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Omnae.BusinessLayer.Models
{
    public class NcrInfoViewModel
    {
        public int Id { get; set; }
        public States StateId { get; set; }
        public string NCRNumber { get; set; }
        public string NCRNumberForVendor { get; set; }

        public DateTime? DateInitiated { get; set; }
        public DateTime? RootCauseAnalysisDate { get; set; }
        public DateTime? NCRApprovalDate { get; set; }
        public DateTime? DateClosed { get; set; }

        public string RootCause { get; set; }

        public string Vendor { get; set; }

        public decimal? Cost { get; set; }


        public List<string> Months { get; set; }

        public int TotalPartsPerYear { get; set; }
        public int TotalPartsPerMonth { get; set; }

        public int TotalFaultPartsPerYear { get; set; }
        public int TotalFaultPartsPerMonth { get; set; }
        public USER_TYPE UserType { get; set; }
    }
}