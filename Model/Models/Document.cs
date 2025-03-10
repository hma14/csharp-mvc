using JetBrains.Annotations;
using Omnae.Model.Models.Aspnet;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Omnae.Model.Models
{
    public class Document
    {
        private string _docUri;

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Index("IX_Document", 1, IsUnique = true)]
        public int ProductId { get; set; }
        public int? TaskId { get; set; }

        [Index("IX_Document", 2, IsUnique = true)]
        [Column(TypeName = "VARCHAR")]
        [StringLength(250)]
        [Required]
        public string Name { get; set; }

        [Index("IX_Document", 3, IsUnique = true)]
        public int Version { get; set; }

        [NotMapped]
        [Column(name: "DocUri", TypeName = "VARCHAR")]
        public string DocUri
        {
            get => _docUri = _docUri ?? DocUriFromBd;
            set
            {
                _docUri = value;
                DocUriFromBd = value?.Split(new []{'?'}, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            }
        }

        [Column(name: "DocUri", TypeName = "VARCHAR")]
        [StringLength(500)]
        [Required]
        public string DocUriFromBd { get; set; }

        public int? DocType { get; set; }
        public bool? IsLocked { get; set; }

       
        public DateTime CreatedUtc { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime ModifiedUtc { get; set; }
        public int? UserType { get; set; }

        [ForeignKey("CreatedByUser")]
        public string CreatedByUserId { get; set; }
        
        [ForeignKey("ModifiedByUser")]
        public string ModifiedByUserId { get; set; }

        [ForeignKey("BidRequestRevision")]
        public int? BidRequestRevisionId { get; set; }

        [ForeignKey("PartRevision")]
        public int? PartRevisionId { get; set; }

       



        // Navigation Property

        [CanBeNull]
        public virtual SimplifiedUser CreatedByUser { get; set; }

        [CanBeNull]
        public virtual SimplifiedUser ModifiedByUser { get; set; }

        public virtual Product Product { get; set; }
        public virtual TaskData TaskData { get; set; }

        [CanBeNull]
        public virtual BidRequestRevision BidRequestRevision { get; set; }

        [CanBeNull]
        public virtual PartRevision PartRevision { get; set; }

       

    }
}
