using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.BackgroundWorkers.Auth0
{
    public class OAuthUserData
    {
        /// <summary>
        /// Id Used By the new oAuth User Base
        /// </summary>
        public string oAuthUserId { get; set; }

        /// <summary>
        /// Id Used By Asp.Net Identity
        /// </summary>
        public string LegacyId { get; set; }

        public string UserType { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        //public bool TwoFactorEnabled { get; set; }
        public string UserName { get; set; }
        public int? CompanyId { get; set; }
        public bool Blocked { get; set; }
        public bool? Active { get; set; }
        public string Role { get; set; }
        public string CustomerRole { get; set; }
        public string VendorRole { get; set; }
        public string StripeCustomerId { get; set; }
        public bool? IsPrimaryContact { get; set; }
    }
}
