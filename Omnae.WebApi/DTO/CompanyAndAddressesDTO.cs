using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Omnae.BusinessLayer.Models;
using Omnae.Common;

namespace Omnae.WebApi.DTO
{
    public class CompanyDTO
    {
        /// <summary>
        /// ID
        /// </summary>
        public virtual int? Id { get; set; }

        /// <summary>
        /// Company Name
        /// </summary>
        [Required]
        [MaxLength(150)]
        public virtual string Name { get; set; }

        /// <summary>
        /// Company Logo Uri
        /// </summary>
        [DataType(DataType.Url)]
        public virtual string CompanyLogoUri { get; set; }

        /// <summary>
        /// Cust. Payment Term (days)
        /// </summary>
        public int? Term { get; set; }

        /// <summary>
        /// Credit Limit ($)
        /// </summary>
        [DataType(DataType.Currency)]
        public decimal? CreditLimit { get; set; }

        /// <summary>
        /// Stripe Customer ID
        /// </summary>
        public string StripeCustomerId { get; set; }

        /// <summary>
        /// The Company Accounting Email contact
        /// </summary>
        [EmailAddress, CanBeNull]
        public string AccountingEmail { get; set; }
        /// <summary>
        /// The Company Email contact
        /// </summary>
        [EmailAddress, CanBeNull]
        public string Email { get; set; }

        /// <summary>
        /// The main Company office Address
        /// </summary>
        public virtual AddressDTO MainCompanyAddress { get; set; }

        /// <summary>
        /// The Company Mail Address
        /// </summary>
        public virtual AddressDTO Address { get; set; }

        /// <summary>
        /// The Company Billing Address
        /// </summary>
        public virtual AddressDTO BillingAddress { get; set; }

        /// <summary>
        /// The Default Shipping Information and Address for this company.
        /// </summary>
        public virtual ShippingDTO Shipping { get; set; }

        public virtual bool? isEnterprise { get; set; } = true;
        public virtual bool? isQualified { get; set; } = true;

        /// <summary>
        /// All available Company Addresses - This is not used in the 
        /// </summary>
        public virtual ICollection<AddressDTO> AllAddresses { get; set; }
        //public virtual ICollection<ShippingProfileDTO> ShippingProfiles { get; set; }

        public virtual UserPerformance UserPerformance { get; set; }
    }

    public class ShippingProfileDTO
    {
        /// <summary>
        /// Shipping Profile ID
        /// </summary>
        public virtual int Id { get; set; }

        /// <summary>
        /// This Shipping Profile Company Id
        /// </summary>
        [Display(Name = "Company Id")]
        public int? CompanyId { get; set; }

        /// <summary>
        /// Profile Name
        /// </summary>
        [Required]
        [Display(Name = "Profile Name")]
        public virtual string ProfileName { get; set; }

        /// <summary>
        /// Profile Description
        /// </summary>
        [CanBeNull]
        public string ProfileDescription { get; set; }

        /// <summary>
        /// Destination Company Name
        /// </summary>
        [CanBeNull]
        [Display(Name = "Destination Company Name")]
        public virtual string DestinationCompanyName { get; set; }


        [DataType(DataType.Text)]
        [Display(Name = "Attention")]
        public virtual string Attention_FreeText { get; set; }

        [RegularExpression(@"^\s*\+?([0-9]{1,3})*[-.\s]?\(?([0-9]{3})\)?[-.\s]?([0-9]{3,4})[-.\s]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public virtual string PhoneNumber { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9_\+-]+(\.[a-zA-Z0-9_\+-]+)*@[a-z0-9-]+(\.[a-z0-9]+‌​)*\.([a-z]{2,4})(\.[a-z]{2,4})*$", ErrorMessage = "Invalid email format.")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-mail Address")]
        public virtual string EmailAddress { get; set; }

        /// <summary>
        /// The Shipping Address Info
        /// </summary>
        public virtual AddressDTO Address { get; set; }

        /// <summary>
        /// The default Shipping Account to be used with this Shipping Profile
        /// </summary>
        public virtual ShippingAccountDTO ShippingAccount { get; set; }
    }

    public class ShippingAccountDTO
    {
        /// <summary>
        /// The Shipping Account ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Company Id of this Shipping Account
        /// </summary>
        public int CompanyId { get; set; }

        /// <summary>
        /// The Shipping Account Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Shipping Account Carrier Name
        /// </summary>
        public string Carrier { get; set; }

        /// <summary>
        /// The Carrier Shipment Type
        /// </summary>
        public SHIPPING_CARRIER_TYPE CarrierType { get; set; }

        /// <summary>
        /// The Carrier Account Number to be used.
        /// </summary>
        public string AccountNumber { get; set; }
    }
}