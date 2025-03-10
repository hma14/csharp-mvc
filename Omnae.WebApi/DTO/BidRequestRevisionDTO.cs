using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.WebApi.DTO
{
    public class BidRequestRevisionDTO
    {
        public int Id { get; set; }

        public DateTime? CreateDateTime { get; set; }

        public string RFQRevisionReason { get; set; }
        public string RFQRevisionDescription { get; set; }

        public List<string> RevisionDocsUri { get; set; }

    }
}