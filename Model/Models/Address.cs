using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Model.Models
{
    public class Address
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Address Line1")]
        public string AddressLine1 { get; set; }

        [Display(Name = "Address Line2")]
        public string AddressLine2 { get; set; }

        public string City { get; set; }

        [ForeignKey("Country")]
        [Display(Name = "Country")]
        public int CountryId  { get; set; }

        [ForeignKey("StateProvince")]
        [Display(Name = "State or Province")]
        public int? StateProvinceId { get; set; }

        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }

        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Display(Name= "This is my Billing Address")]
        public bool isBilling { get; set; }

        [Display(Name = "This is my Shipping Address")]
        public bool isShipping { get; set; }

        [Display(Name = "This is my Main Address")]
        public bool isMainAddress { get; set; }

        [Display(Name = "This is my Mailing Address")]
        public bool isMailingAddress { get; set; }


        [ForeignKey("Company")]
        public int? CompanyId { get; set; }


        // Navigation Properties

        public virtual StateProvince StateProvince { get; set; }
        public virtual Country Country { get; set; }

        public virtual Company Company { get; set; }

        /////////////////

        protected bool Equals(Address other)
        {
            return Id == other.Id && string.Equals(AddressLine1, other.AddressLine1) && string.Equals(AddressLine2, other.AddressLine2) && string.Equals(City, other.City) && CountryId == other.CountryId && StateProvinceId == other.StateProvinceId && string.Equals(ZipCode, other.ZipCode) && string.Equals(PostalCode, other.PostalCode) && isBilling == other.isBilling && isShipping == other.isShipping && isMainAddress == other.isMainAddress && isMailingAddress == other.isMailingAddress && CompanyId == other.CompanyId && Equals(StateProvince, other.StateProvince);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Address) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id;
                hashCode = (hashCode * 397) ^ (AddressLine1 != null ? AddressLine1.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (AddressLine2 != null ? AddressLine2.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (City != null ? City.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ CountryId;
                hashCode = (hashCode * 397) ^ StateProvinceId.GetHashCode();
                hashCode = (hashCode * 397) ^ (ZipCode != null ? ZipCode.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (PostalCode != null ? PostalCode.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ isBilling.GetHashCode();
                hashCode = (hashCode * 397) ^ isShipping.GetHashCode();
                hashCode = (hashCode * 397) ^ isMainAddress.GetHashCode();
                hashCode = (hashCode * 397) ^ isMailingAddress.GetHashCode();
                hashCode = (hashCode * 397) ^ CompanyId.GetHashCode();
                hashCode = (hashCode * 397) ^ (StateProvince != null ? StateProvince.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(Address left, Address right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Address left, Address right)
        {
            return !Equals(left, right);
        }
    }
}
