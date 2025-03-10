using System.ComponentModel;
using System.Runtime.Serialization;
using HubSpot.NET.Api.Contact.Dto;

namespace Omnae.Hubspot.Model
{
    public class Contact : ContactHubSpotModel
    {
        [Description("The Company ID that this User/Contact is part")]
        [DataMember(Name = "omnaecompanyinternalid")]
        public string OmnaeCompanyID { get; set; }

        [Description("The User ID in the System Database")]
        [DataMember(Name = "omnaeuserinternalid")]
        public string OmnaeUserId { get; set; }

        [Description("From Omnae or Padtech Legacy database")]
        [DataMember(Name = "legacy")]
        public string LegacyClient { get; set; } //Need to be fixed in HS

        [Description("Customer Type in Omnae System")]
        [DataMember(Name = "customer_type")]
        public string CustomerType { get; set; } 

        [Description("General Role based on Omnae Platform")]
        [DataMember(Name = "general_role")]
        public string GeneralRole { get; set; }

        [Description("Customer Role in Omnae Platform")]
        [DataMember(Name = "user_type_customer")]
        public string UserTypeCustomer { get; set; }

        [Description("Vendor Role in Omnae Platform")]
        [DataMember(Name = "user_type_vendor")]
        public string UserTypeVendor { get; set; }
        
    }
}
