using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;
using Omnae.Model.Models.Aspnet;

namespace Omnae.Model.Models
{
    public class ShippingProfile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Profile Name")]
        public string ProfileName { get; set; }

        [CanBeNull]
        [Display(Name = "Destination Company Name")]
        public string DestinationCompanyName { get; set; }

        [ForeignKey("Shipping")]
        public int? ShippingId { get; set; }
        public virtual Shipping Shipping { get; set; }

        [CanBeNull]
        public string Description { get; set; }

        [Display(Name = "Company Id")]
        [ForeignKey("Company")]
        public int? CompanyId { get; set; }
        public virtual Company Company { get; set; }

        [ForeignKey("ShippingAccount")]
        public int? ShippingAccountId { get; set; }
        public virtual ShippingAccount ShippingAccount { get; set; }

        [Required] 
        public DateTime? CreatedAt { get; set; }
        [ForeignKey("CreatedByUser")]
        public string CreatedByUserId { get; set; }
        [CanBeNull]
        public virtual SimplifiedUser CreatedByUser { get; set; }

        [Required]
        public DateTime? ModifiedAt { get; set; }
        [ForeignKey("ModifiedByUser")]
        public string ModifiedByUserId { get; set; }
        [CanBeNull]
        public virtual SimplifiedUser ModifiedByUser { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsDefault { get; set; } = false;
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime? _updatedAt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime? _createdAt { get; set; }

        ////////////////////////////////

        protected bool Equals(ShippingProfile other)
        {
            return Id == other.Id && string.Equals(ProfileName, other.ProfileName) && string.Equals(DestinationCompanyName, other.DestinationCompanyName) && ShippingId == other.ShippingId && string.Equals(Description, other.Description) && CompanyId == other.CompanyId && CreatedAt.Equals(other.CreatedAt) && string.Equals(CreatedByUserId, other.CreatedByUserId) && ModifiedAt.Equals(other.ModifiedAt) && string.Equals(ModifiedByUserId, other.ModifiedByUserId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ShippingProfile) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id;
                hashCode = (hashCode * 397) ^ (ProfileName != null ? ProfileName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (DestinationCompanyName != null ? DestinationCompanyName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ ShippingId.GetHashCode();
                hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ CompanyId.GetHashCode();
                hashCode = (hashCode * 397) ^ CreatedAt.GetHashCode();
                hashCode = (hashCode * 397) ^ (CreatedByUserId != null ? CreatedByUserId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ ModifiedAt.GetHashCode();
                hashCode = (hashCode * 397) ^ (ModifiedByUserId != null ? ModifiedByUserId.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(ShippingProfile left, ShippingProfile right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ShippingProfile left, ShippingProfile right)
        {
            return !Equals(left, right);
        }
    }
}