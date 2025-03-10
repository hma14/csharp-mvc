using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Omnae.Common;

namespace Omnae.Model.Models.Aspnet
{
    [Table("AspNetUsers")]
    public class SimplifiedUser
    {
        public string Id { get; set; }
        
        [Required]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        //[Required]
        [Display(Name = "Type")]
        public USER_TYPE UserType { get; set; }

        [Required]
        [Index("IX_Active")]
        [Display(Name = "Is User Active")]
        public bool? Active { get; set; } = true;    

        //[Required]
        [Index("IX_CompanyId")]
        public int? CompanyId { get; set; }

        public bool? IsPrimaryContact { get; set; }

        // Mirror auth0 roles
        public string Role { get; set; }
        public string CustomerRole { get; set; }
        public string VendorRole { get; set; }

        public virtual Company Company { get; set; }

    }
}
