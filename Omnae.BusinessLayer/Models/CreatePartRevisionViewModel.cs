using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Omnae.Model.Models;

namespace Omnae.BusinessLayer.Models
{
    public class CreatePartRevisionViewModel
    {     
        public string UserName { get; set; }
        public int? TaskId { get; set; }
        public int ProductId { get; set; }

        [Display(Name = "New Revision Number")]
        public string PartRevision { get; set; }

        [Display(Name = "Please describe the changes you are making in this revision")]
        public string PartRevisionDesc { get; set; }
        public List<Document> Documents { get; set; }
        [Display(Name = "New Avatar")]
        public HttpPostedFileBase NewAvatar { get; set; }
        public int? VendorId { get; set; }
        public int? BidRequestRevisionId { get; set; }
    }
}