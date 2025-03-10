using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.WebApi.DTO
{
    public class RelationshipCurrencyDto
    {
        public int CustomerId { get; set; }
        public int VendorId { get; set; }
        public string Currency { get; set; }
    }
}