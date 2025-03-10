using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Intuit.Ipp.Data;
using JetBrains.Annotations;
using Omnae.BusinessLayer.Models;
using Omnae.Common;
using Omnae.Model.Models.Aspnet;

namespace Omnae.BusinessLayer.Models
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

        public bool WasOnboarded { get; set; } = false;
        public bool WasInvited { get; set; } = false;

        public int? InvitedByCompanyId { get; set; }

        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }


        /// <summary>
        /// All available Company Addresses - This is not used in the 
        /// </summary>
        public virtual ICollection<AddressDTO> AllAddresses { get; set; }
        //public virtual ICollection<ShippingProfileDTO> ShippingProfiles { get; set; }

        public virtual UserPerformance UserPerformance { get; set; }
        public DateTime? CompanyAddedAt { get; set; }

        public CurrencyCodes CurrencyCode { get; set; }

    }

    public class ShippingProfileDTO
    {
        /// <summary>
        /// Shipping Profile ID
        /// </summary>
        [Required]
        public virtual int Id { get; set; }

        /// <summary>
        /// This Shipping Profile Company Id
        /// </summary>
        [Display(Name = "Company Id")]
        public int? CompanyId { get; set; }

        /// <summary>
        /// Flag to indicate that is the Default Shipping Profile for this company
        /// </summary>
        public bool? IsDefault { get; set; } = null;

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

        [RegularExpression(@"^\s*\+?([0-9\-\.\s\(\)]{3,22})\s*$", ErrorMessage = "Not a valid Phone number")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public virtual string PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format.")]
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
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// The Company Id of this Shipping Account
        /// </summary>
        [Display(Name = "Company Id")]
        public int? CompanyId { get; set; }

        /// <summary>
        /// Flag to indicate that is the Default Shipping Account for this company
        /// </summary>
        public bool? IsDefault { get; set; } = null;

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