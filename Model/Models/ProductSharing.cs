using JetBrains.Annotations;
using Omnae.Common;
using Omnae.Model.Models.Aspnet;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Model.Models
{
    public class ProductSharing
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("ProductSharingCompany")]
        [Index("IX_ProductSharing", 1, IsUnique = true)]
        public int SharingCompanyId { get; set; }

        [ForeignKey("ProductOwnerCompany")]        
        public int OwnerCompanyId { get; set; }

        [ForeignKey("Product")]
        [Index("IX_ProductSharing", 2, IsUnique = true)]
        public int ProductId { get; set; }

        [ForeignKey("TaskData")]
        public int? TaskId { get; set; }

        public DateTime? CreatedUtc { get; set; }
        public DateTime? ModifiedUtc { get; set; }



        public bool? HasPermissionToOrder { get; set; }
        public bool? IsRevoked { get; set; }

        [ForeignKey("CreatedByUser")]
        public string CreatedByUserId { get; set; }

        [ForeignKey("ModifiedByUser")]
        public string ModifiedByUserId { get; set; }

        // Navigation Properties

        public virtual Product  Product { get; set; }
        public virtual Company ProductSharingCompany { get; set; }
        public virtual Company ProductOwnerCompany { get; set; }
        public virtual TaskData TaskData { get; set; }

        [CanBeNull]
        public virtual SimplifiedUser CreatedByUser { get; set; }

        [CanBeNull]
        public virtual SimplifiedUser ModifiedByUser { get; set; }
    }
}
