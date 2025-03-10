using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.WebApi.DTO
{
    public class RFQRevisionDTO
    {
        public List<BidRevisingRequest> Descriptions { get; set; }
        public string[] Files { get; set; }
    }

    public class BidRevisingRequest
    {
        public int Id { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; }

        public string Description { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public int RevisionNumber { get; set; }
    }
}