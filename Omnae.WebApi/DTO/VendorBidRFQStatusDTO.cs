using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.WebApi.DTO
{
    public class VendorBidRFQStatusDTO
    {
        public int VendorId { get; set; }

        public int ProductId { get; set; }

        public int StateId { get; set; }
        public int TaskId { get; set; }

        public string RejectRFQReason { get; set; }
        public string RejectRFQDescription{ get; set; }

        public int? BidRequestRevisionId { get; set; }

        public int? BidRFQStatusId { get; set; }
        public DateTime? TimeStamp { get; set; }

        public BidRequestRevisionDTO BidRequestRevision { get; set; }

    }
}