using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Omnae.BusinessLayer.Models
{

    public class AddressDTO : IValidatableObject
    {
        public virtual int? Id { get; set; }

        [Required]
        public virtual string Country { get; set; }

        public virtual string StateOrProvinceName { get; set; }

        [Required]
        public virtual string City { get; set; }

        [Required]
        public virtual string AddressLine1 { get; set; }

        public virtual string AddressLine2 { get; set; }

        public virtual string ZipCode { get; set; }

        public virtual string PostalCode { get; set; }

        /// <summary>
        /// If this is the Billing Address
        /// </summary>
        public virtual bool isBilling { get; set; }

        /// <summary>
        /// If this is the Shipping Address
        /// </summary>
        public virtual bool isShipping { get; set; }

        /// <summary>
        /// If this is the Main malling Address
        /// </summary>
        public bool isMainAddress { get; set; }

        ////////////////////////////////////////////////

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(ZipCode) && string.IsNullOrWhiteSpace(PostalCode))
            {
                yield return new ValidationResult("ZipCode or PostalCode is Required");
            }
        }
    }

    public class ShippingDTO
    {
        public virtual int? Id { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Attention")]
        public virtual string Attention_FreeText { get; set; }

        [RegularExpression(@"^\s*\+?([0-9\-\.\s\(\)]{3,22})\s*$", ErrorMessage = "Not a valid Phone number")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public virtual string PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-mail Address")]
        public virtual string EmailAddress { get; set; }

        [Required]
        public virtual AddressDTO Address { get; set; }
    }

}
