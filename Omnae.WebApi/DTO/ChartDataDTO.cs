using Omnae.BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.WebApi.DTO
{
    public class ChartDataDTO
    {
        public ChartTypeViewModel ChartType { get; set; }
        public List<NcrInfoViewModel> NcrInfoList { get; set; }
    }
}