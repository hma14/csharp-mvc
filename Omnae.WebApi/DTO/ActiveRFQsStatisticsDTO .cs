using Humanizer;
using Omnae.BusinessLayer.Models;
using Omnae.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Omnae.WebApi.DTO
{
    public class ActiveRFQsStatisticsDTO
    {
        public int NumberOfAllActiveRFQs { get; set; }
        public int? UniqueVendorsOrCustomersInvolvedInActiveRFQs { get; set; }
        public int NumberOfReviewRFQs { get; set; }
        public int NumberOfActionableReviewRFQs { get; set; }
        public int NumberOfBiddingRFQs { get; set; }
        public int NumberOfActionableBiddingRFQs { get; set; }

    }

}
