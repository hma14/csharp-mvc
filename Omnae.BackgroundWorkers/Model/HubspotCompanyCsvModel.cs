using CsvHelper.Configuration.Attributes;

namespace Omnae.BackgroundWorkers.Model
{
    class HubspotCompanyCsvModel
    {
        public string Name { get; set; }

        [Name("Omnae Company ID")] 
        public string OmnaeCompanyID { get; set; }
        [Name("Company Domain")]
        public string CompanyDomain { get; set; }
        [Name("Phone Number")]
        public string PhoneNumber { get; set; }

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
        [Name("First contact create date")]
        public string FirstContactCreateDate { get; set; } = "";
        [Name("Company Owner")]
        public string CompanyOwner { get; set; }
        [Name("Customer Type")]
        public string CustomerType { get; set; } = "";
        [Name("Legacy Client")]
        public string LegacyClient { get; set; } = "No";
        [Name("Business Model")]
        public string BusinessModel { get; set; } = "Reseller";

        [Name("Point of Contact")]
        public string PointOfContact { get; set; }
        [Name("Email of Contact")]
        public string EmailOfContact { get; set; }

        [Name("Omnae Client Life Cycle Stage")]
        public string OmnaeClientLifeCycleStage { get; set; } = "Free Trial";
    }
}