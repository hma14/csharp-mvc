using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;

namespace Omnae.BackgroundWorkers.Model
{
    class HubspotCustomerCsvModel
    {
        [Name("First Name")]
        public string FirstName { get; set; }
        [Name("Last Name")]
        public string LastName { get; set; }
        public string Salutation { get; set; }
        
        [Name("Email Address")]
        public string EmailAddress { get; set; }
        [Name("Website URL")]
        public string WebsiteUrl { get; set; }
        
        [Name("Phone Number")]
        public string PhoneNumber { get; set; }
        [Name("Mobile Phone Number")]
        public string MobilePhoneNumber { get; set; }
        
        [Name("Street Address 1")]
        public string StreetAddress1 { get; set; }
        [Name("Street Address 2")]
        public string StreetAddress2 { get; set; }
        public string City { get; set; }
        [Name("State/Region")]
        public string StateRegion { get; set; }
        public string Country { get; set; }
        [Name("Postal Code")]
        public string PostalCode { get; set; }

        [Name("Lifecycle Stage")]
        public string LifecycleStage { get; set; } = "Customer";

        [Name("Contact Owner")] 
        public string ContactOwner { get; set; } = "slo@omnae.com";

        [Name("Became a customer date")]
        public string FirstBecameACustomerDate { get; set; }

        [Name("Omnae Company ID")]
        public string OmnaeCompanyID { get; set; }

        [Name("Company Name")]
        public string CompanyName { get; set; }

        [Name("Omnae User ID")]
        public string OmnaeUserId { get; set; }

        [Name("Customer Type")]
        public string CustomerType { get; set; } = "Customer";

        [Name("General Role")]
        public string GeneralRole { get; set; } = "Admin";

        [Name("User Type Customer")]
        public string UserTypeCustomer { get; set; }

        [Name("User Type Vendor")]
        public string UserTypeVendor { get; set; }

        [Name("Legacy Customer")]
        public string LegacyCustomer { get; set; } = "No";
    }
}
