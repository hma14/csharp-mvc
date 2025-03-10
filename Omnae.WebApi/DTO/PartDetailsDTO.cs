using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.WebApi.DTO
{
    public class PartDetailsDTO
    {
        public ProductDTO Product { get; set; }
        public List<PriceBreakDTO> PriceBreaks { get; set; }
        public PartRvisionDTO PartRvision { get; set; }
        public List<DocumentDTO> Documents { get; set; }
        public bool? IsEnterprise { get; set; }
        public int UnitOfMeasurement { get; set; }
    }
}