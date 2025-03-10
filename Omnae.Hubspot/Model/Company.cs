using System.ComponentModel;
using System.Runtime.Serialization;
using HubSpot.NET.Api.Company.Dto;

namespace Omnae.Hubspot.Model
{
    public class Company : CompanyHubSpotModel
    {
        //[DataMember(Name = "activities")]
        //public string Business Model { get; set; }

        //[DataMember(Name = "type")]
        //public string Type { get; set; }

        [Description("SaaS or Reseller Model")]
        [DataMember(Name = "business_model")]
        public string BusinessModel { get; set; } //TODO Enum
        
        [Description("Customer Type - Customer or Vendor or both")]
        [DataMember(Name = "customer_type")]
        public string CustomerType { get; set; } //TODO Enum

        [Description("Part of Omnae's legacy database or not")]
        [DataMember(Name = "legacy")]
        public string LegacyClient { get; set; } //TODO Enum

        [Description("The Company ID in the Omnae Database")]
        [DataMember(Name = "omnae_company_id")]
        public int? OmnaeCompanyID { get; set; } 

        [Description("DBA - Doing business as name ")]
        [DataMember(Name = "dba_name")]
        public string DbaName { get; set; }

        [Description("E-Mail Of Contact ")]
        [DataMember(Name = "email_of_contact")]
        public string EmailOfContact { get; set; }

        [Description("Point of Contact")]
        [DataMember(Name = "point_of_contact")]
        public string PointOfContact { get; set; }

        [Description("Total in Invoices")]
        [DataMember(Name = "total_in_invoices")]
        public string TotalInInvoices { get; set; }

        [Description("List of companies/individuals who refer clients for compensation")]
        [DataMember(Name = "referred_by")]
        public string ReferredBy { get; set; }

        [Description("Omnae Client Life Cycle Stage")]
        [DataMember(Name = "omnae_client_life_cycle_stage")]
        public string OmnaeClientLifeCycleStage { get; set; } = "Free Trial";
    }
}