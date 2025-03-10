using Omnae.Common;
using Omnae.Model.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Omnae.BusinessLayer.Models;

namespace Omnae.ViewModels
{
    public class BidReviewViewModel
    {
        public int BidId { get; set; }
        public int ProductId { get; set; }
        public int ChosenVendorId { get; set; }
        public string ProductName { get; set; }
        public List<Company> Vendors { get; set; }
        public decimal[] ShippingUnitPrices { get; set; }
        public Dictionary<int, string> VendorReason { get; set; }
        public Dictionary<int, bool> SelectVendor { get; set; }
        public List<int> VendorIds { get; set; }
        public string CustomerPriority { get; set; }

        public Dictionary<int, List<PriceBreak>> BidIdPriceBreaksDic { get; set; }
        public Dictionary<int, RFQBid> BidDic { get; set; }
        public Dictionary<int, SelectList> ReasonsDic { get; set; }

        public bool ToShowCheckbox { get; set; }
        public bool isEnterprise { get; set; }
        public ChartTypeViewModel[] Charts  { get; set; }
    }
}