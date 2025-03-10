using Omnae.Common;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Omnae.Data.Query.TaskDataQuery;

namespace Omnae.WebApi.DTO
{
    public class ReviewRFQStatusDTO
    {
        public int? RevisionCycle { get; set; }
        public int CusotmerId { get; set; }
        public int State { get; set; }
        public int TaskId { get; set; }
        public int ProductId { get; set; }
        public UserType Type { get; set; }
               
        public List<string> ProductDocsUri { get; set; }
        
        public int? SubmittedVendors { get; set; }
        public int? TotalVendors { get; set; }
        
        public string PartRevisionName { get; set; }
        public string PartRevisionDescription { get; set; }
        public string KeepCurrentRevisionReason { get; set; }
        public string CancelRFQReason { get; set; }
        public string CancelRFQDescription { get; set; }
        public DateTime TimeStamp { get; set; }
        public List<VendorBidRFQStatusDTO> VendorRFQStatus { get; set; }
        
    }
}