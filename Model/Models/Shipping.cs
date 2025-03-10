using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Omnae.Model.Models
{
    public class Shipping
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Attention")]
        public string Attention_FreeText { get; set; }

        [Display(Name = "Company Id")]
        public int? CompanyId { get; set; }

        [ForeignKey("Address")]
        public int? AddressId { get; set; }

        [RegularExpression(@"^\s*\+?([0-9\-\.\s\(\)]{3,22})\s*$", ErrorMessage = "Not a valid Phone number")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-mail Address")]
        public string EmailAddress { get; set; }

        public bool IsActive { get; set; } = true;

        //////////////////////////////

        public virtual Address Address { get; set; }

        public virtual Company Company { get; set; }

        //////////////////////////////

        protected bool Equals(Shipping other)
        {
            return Id == other.Id && string.Equals(Attention_FreeText, other.Attention_FreeText) && CompanyId == other.CompanyId && AddressId == other.AddressId && string.Equals(PhoneNumber, other.PhoneNumber) && string.Equals(EmailAddress, other.EmailAddress) && IsActive == other.IsActive;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Shipping) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id;
                hashCode = (hashCode * 397) ^ (Attention_FreeText != null ? Attention_FreeText.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ CompanyId.GetHashCode();
                hashCode = (hashCode * 397) ^ AddressId.GetHashCode();
                hashCode = (hashCode * 397) ^ (PhoneNumber != null ? PhoneNumber.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (EmailAddress != null ? EmailAddress.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ IsActive.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Shipping left, Shipping right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Shipping left, Shipping right)
        {
            return !Equals(left, right);
        }
    }
}
