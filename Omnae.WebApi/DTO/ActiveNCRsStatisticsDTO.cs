using Humanizer;
using Omnae.BusinessLayer.Models;
using Omnae.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Omnae.WebApi.DTO
{
    public class ActiveNCRsStatisticsDTO
    {
        public int NumberOfAllActiveNCRs { get; set; }

        [DisplayFormat(DataFormatString = "{0:n3}", ApplyFormatInEditMode = true)]
        public decimal TotalAmoutActiveNCRs { get; set; }
        public int NumberOfReviewNCRs { get; set; }
        public int NumberOfActionableReviewNCRs { get; set; }
        public int NumberOfDisputeActiveNCRs { get; set; }
        public int NumberOfActionableDisputeActiveNCRs { get; set; }
        public int NumberOfResolutionActiveNCRs { get; set; }
        public int NumberOfActionableResolutionActiveNCRs { get; set; }

    }

}
