using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;
using Omnae.Common;
using Omnae.Model.Models.Aspnet;

namespace Omnae.Model.Models
{
    public class Company
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //[Index("IX_Company", 1, IsUnique = true)]
        [Display(Name = "Company Name")]
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }
        
        [Display(Name = "Cust. Payment Term (days)")]
        public int? Term { get; set; }

        [Display(Name = "Credit Limit ($)")]
        [DataType(DataType.Currency)]
        public decimal CreditLimit { get; set; }

        /// <summary>
        /// ID for this Company the Quickbooks (External Service)
        /// </summary>
        public string QboId { get; set; }

        [ForeignKey("Address")]
        public int? AddressId { get; set; }

        [ForeignKey("BillAddress")]
        public int? BillAddressId { get; set; }

        [ForeignKey("MainCompanyAddress")]
        public int? MainCompanyAddress_Id { get; set; }

        [ForeignKey("Shipping")]
        public int? ShippingId { get; set; }

        [DataType(DataType.Url)]
        public string CompanyLogoUri { get; set; }

        [Required]
        public CompanyType? CompanyType { get; set; }

        public bool isEnterprise { get; set; }

        public bool isQualified { get; set; } = true;

        [EmailAddress, CanBeNull]
        public string AccountingEmail { get; set; }
        public string StripeCustomerId { get; set; }

        [ForeignKey("CompanyBankInfo")]
        public int? CompanyBankInfoId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime? _createdAt { get; set; }

        public bool IsActive { get; set; } = true;


        public bool WasOnboarded { get; set; } = false;

        [CanBeNull]
        public int? OnboardedByCompanyId { get; set; }

        [CanBeNull]
        public int? InvitedByCompanyId { get; set; }

        public bool WasInvited { get; set; } = false;

        public string POLegalTerms { get; set; }

        // Navigation Properties

        /// <summary>
        /// Main Company Address (Corporate Addr)
        /// </summary>
        public virtual Address MainCompanyAddress { get; set; }

        /// <summary>
        /// Default Mail Shipping Address
        /// </summary>
        public virtual Address Address { get; set; }

        /// <summary>
        /// Default Company Billing Address
        /// </summary>
        public virtual Address BillAddress { get; set; }

        /// <summary>
        /// Default Company Shipping Address
        /// </summary>
        public virtual Shipping Shipping { get; set; }

        [ForeignKey("CompanyId")]
        public virtual ICollection<Address> AllAddresses { get; set; }
        
        public virtual ICollection<SimplifiedUser> Users { get; set; }

        [ForeignKey("CompanyId")]
        public virtual ICollection<ShippingProfile> ShippingProfiles { get; set; }

        public virtual ICollection<ShippingAccount> ShippingAccounts { get; set; }

        public virtual CompanyBankInfo CompanyBankInfo { get; set; }

        public CurrencyCodes Currency { get; set; }

    }

    public enum CompanyType
    {
        None = 0,
        Customer = 1,
        Vendor = 2,
    }
}
