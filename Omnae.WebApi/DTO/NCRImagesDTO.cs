using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.WebApi.DTO
{
    public class NCRImagesDTO
    {
        public int NCReportId { get; set; }
        public string ImageUrl { get; set; }
        public int Type { get; set; }
    }
}