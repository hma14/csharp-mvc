using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Model.Models
{
    public class BidRequestRevision
    {
        public BidRequestRevision()
        {
            Documents = new List<Document>();
        }
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //[Index("IX_BidRequestRevision", 1, IsUnique = true)]
        public int VendorId { get; set; }

        //[Index("IX_BidRequestRevision", 2, IsUnique = true)]
        public int ProductId { get; set; }

        [ForeignKey("VendorTaskData")]
        public int TaskId { get; set; }

        public int CustomerTaskId { get; set; }
        public DateTime? CreateDateTime { get; set; }
        public int? RevisionNumber { get; set; }

        [ForeignKey("RFQActionReason")]
        public int? RFQActionReasonId { get; set; }


        // Navigation Properties

        public virtual TaskData VendorTaskData { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
        public virtual RFQActionReason RFQActionReason { get; set; }

    }
}
