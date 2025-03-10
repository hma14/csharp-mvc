using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.WebApi.DTO
{
    public class ProductPriceQuoteDTO
    {
        public int Id { get; set; }
        public int VendorId { get; set; }

        public int ProductId { get; set; }

        public int ProductionLeadTime { get; set; }
        public DateTime ExpireDate { get; set; }

        public string QuoteDocUri { get; set; }

        public bool? IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
        public IList<PriceBreak> PriceBreaks { get; set; }
    }
}