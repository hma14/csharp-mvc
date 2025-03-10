using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Libs.ViewModel
{
    public class BaseViewModel
    {
        public string UserName { get; set; }
        public string PartNumber { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string PONumber { get; set; }
        public string NCRArbitrateReasonByAdmin { get; set; }
        public List<string> EvidenceFileUrls { get; set; }
        public string CustomerRejectedReasons { get; set; }

        public string CarrierName { get; set; }

        public string TrackingNumber { get; set; }
        public string RejectCorrectivePartsReason { get; set; }

        public string CustomerName { get; set; }
        public string VendorName { get; set; }
        public string RejectReason { get; set; }
        public string DocUri { get; set; }
    }
}
