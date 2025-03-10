using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;
using Omnae.Model.Models.Aspnet;

namespace Omnae.Model.Models
{

    public class OrderStateTracking
    {   
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public int? TaskId { get; set; }
        public int? NcrId { get; set; }
        public int? OrderId { get; set; }
        public int StateId { get; set; }
        
        public string UpdatedBy { get; set; }
        public DateTime ModifiedUtc { get; set; }

        [ForeignKey("CreatedByUser")]
        public string CreatedByUserId { get; set; }
        
        [ForeignKey("ModifiedByUser")]
        public string ModifiedByUserId { get; set; }

        // Navigation Property

        public virtual TaskData Task { get; set; }


        [CanBeNull]
        public virtual SimplifiedUser CreatedByUser { get; set; }

        [CanBeNull]
        public virtual SimplifiedUser ModifiedByUser { get; set; }
    }
}