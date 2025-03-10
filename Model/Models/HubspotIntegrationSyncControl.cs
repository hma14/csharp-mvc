using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Omnae.Model.Models
{
    public class HubspotIntegrationSyncControl
    {
        public HubspotIntegrationSyncControl()
        {
        }
        public HubspotIntegrationSyncControl(DateTime initDate)
        {
            LastCheckForNewAuthZeroUsers = LastCheckForNewAuthZeroCompany
                = LastUpdateInOmnaeUserDatabase = LastUpdateInOmnaeCompanyDatabase
                    = LastUpdateInHubspotUser = LastUpdateInHubspotCompany = initDate;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime? LastCheckForNewAuthZeroUsers { get; set; }
        public DateTime? LastCheckForNewAuthZeroCompany { get; set; }

        public DateTime? LastUpdateInOmnaeUserDatabase { get; set; }
        public DateTime? LastUpdateInOmnaeCompanyDatabase { get; set; }

        public DateTime? LastUpdateInHubspotUser { get; set; }
        public DateTime? LastUpdateInHubspotCompany { get; set; }
    }
}