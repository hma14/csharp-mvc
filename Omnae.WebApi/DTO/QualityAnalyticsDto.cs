using Omnae.BusinessLayer.Models;
using Omnae.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.WebApi.DTO
{
    public class QualityAnalyticsDto
    {
        public PagedResultSet<CompanyDTO> Companies { get; set; }
        public RFQChartDataViewModel ChartData { get; set; }
    }
}