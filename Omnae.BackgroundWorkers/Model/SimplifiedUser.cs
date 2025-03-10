using Dapper.Contrib.Extensions;

namespace Omnae.BackgroundWorkers.Model
{
    [Table("AspNetUsers")]
    public class SimplifiedUser
    {
        [Key] 
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        //[Required]
        public USER_TYPE UserType { get; set; } = USER_TYPE.Customer;

        public bool Active { get; set; } = true;
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool LockoutEnabled { get; set; }


        //[Required]
        public int? CompanyId { get; set; }

        public bool? IsPrimaryContact { get; set; }

        // Mirror auth0 roles
        public string Role { get; set; }
        public string CustomerRole { get; set; }
        public string VendorRole { get; set; }

    }
    public enum USER_TYPE
    {
        Customer = 1,
        Vendor = 2,
        Admin = 3,
        Unknown
    }
}
