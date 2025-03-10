using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.WebApi.DTO
{
    public class OrderDetailsDTO
    {
        public OrderDTO Order { get; set; }
        public ProductDTO Product { get; set; }
        
        public PartRvisionDTO PartRvision { get; set; }
        public List<NCReportDTO> NCReports { get; set; }
        
        public List<DocumentDTO> Documents { get; set; }
        public List<OrderStateTrackingDTO> OrderStateTrackings { get; set; }

    }
}