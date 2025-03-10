using Omnae.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Omnae.Model.Models.Aspnet;

namespace Omnae.Model.Models
{
    public class PartRevision
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("TaskData")]
        public int? TaskId { get; set; }

        //[Index("IX_PartRevision", 1, IsUnique = true)]
        public int OriginProductId { get; set; }

        //[Index("IX_PartRevision", 2, IsUnique = true)]
        [Column(TypeName = "nvarchar")]    
        [StringLength(100)]
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public States StateId { get; set; }

        public bool? IsForWholesales { get; set; }

        public DateTime CreatedUtc { get; set; }
        [StringLength(50)]
        public string CreatedBy { get; set; }
        //public Product Product { get; set; }

        [ForeignKey("CreatedByUser")]
        public string CreatedByUserId { get; set; }
        
        [ForeignKey("ModifiedByUser")]
        public string ModifiedByUserId { get; set; }


        // Navigation Property

        public virtual TaskData TaskData { get; set; }


        [CanBeNull]
        public virtual SimplifiedUser CreatedByUser { get; set; }

        [CanBeNull]
        public virtual SimplifiedUser ModifiedByUser { get; set; }

    }
}
