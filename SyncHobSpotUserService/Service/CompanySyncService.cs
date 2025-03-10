using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using HubSpot.NET.Api.Contact.Dto;
using HubSpot.NET.Core;
using HubSpot.NET.Core.OAuth.Dto;
using Microsoft.Azure;
using Omnae.Hubspot.Model;
using Omnae.Hubspot.Util;
//using Serilog;
using Microsoft.Extensions.Logging;



namespace Omnae.SyncHobSpotUser.Service
{
    public class CompanySyncService
    {
        private ILogger Log { get; }

        private SqlConnection OmnaeDbConnection { get; }

        private HubSpotApi HubSpotClient { get; }

        public CompanySyncService(ILogger log, HubSpotApi HubSpotClient)
        {
            Log = log;
            this.HubSpotClient = HubSpotClient;


			//Db Conn
			var dbConnStr = Environment.GetEnvironmentVariable("OmnaeDbContext"); //?? WebConfigurationManager.ConnectionStrings["OmnaeDbContext"]?.ConnectionString;
			OmnaeDbConnection = new SqlConnection(dbConnStr);
        }

		public async Task UpsertCompaniesInHubspotAsync(IEnumerable<int> companiesIds, IEnumerable<Contact> newContacts = null, bool isToUpdateLifeCycleCompanies = true)
		{
			var companiesIdsList = companiesIds.ToList();

			Log.LogInformation("Starting the CompanySyncService.UpsertCompaniesInHubspotAsync, For {Qnt} companies, and isToUpdateLifeCycleCompanies={isToUpdateLifeCycleCompanies}", companiesIdsList.Count, isToUpdateLifeCycleCompanies);

			Log.LogDebug("Listing... all companies in HubSpot.");
			var allCompaniesInHS = HubSpotClient.Company.ListAll<Company>(new List<string> { "omnae_company_id", "name", "domain" }).ToList();
			Log.LogDebug("Listing... all companies in HubSpot. - Done - Found {Qnt}", allCompaniesInHS.Count());

			var companiesIdsInHS = allCompaniesInHS.Select(c => new { c.Id, c.OmnaeCompanyID }).Where(c => c.OmnaeCompanyID != null).ToLookup(k => (int)k.OmnaeCompanyID, e => e.Id);
			var companiesNamesInHS = allCompaniesInHS.Select(c => new { c.Id, Name = c.Name?.ToLowerInvariant() }).Where(c => !string.IsNullOrWhiteSpace(c.Name)).ToLookup(k => k.Name, e => e.Id);
			var companiesDomainInHS = allCompaniesInHS.Select(c => new { c.Id, c.Domain }).Where(c => !string.IsNullOrWhiteSpace(c.Domain)).ToLookup(k => k.Domain, e => e.Id);

			var listOfCompanyToUpdateInHS = companiesIdsList.Intersect(companiesIdsInHS.Select(c => c.Key).Distinct()).ToHashSet();

			var mappedCompany = await LoadAndMapToCompaniesFromOmnaeDB(companiesIdsList);

			Log.LogDebug("Starting the Upsert in HubSpot.");
			foreach (var companyMapped in mappedCompany)
			{
				var isToUpdate = listOfCompanyToUpdateInHS.Contains((int)companyMapped.OmnaeCompanyID) || companiesDomainInHS.Contains(companyMapped.Domain) || companiesNamesInHS.Contains(companyMapped.Name);

				Log.LogDebug("Upsert in HubSpot. CompanyID:{CompanyID}, Name:{CompanyName}, isToUpdate:{isToUpdate}", companyMapped.OmnaeCompanyID, companyMapped.Name, isToUpdate);

				if (isToUpdate)
				{
					if (!isToUpdateLifeCycleCompanies)
						continue;

					var hsCompanyID = (companiesIdsInHS[(int)companyMapped.OmnaeCompanyID].FirstOrDefault()
									   ?? companiesDomainInHS[companyMapped.Domain].FirstOrDefault()
									   ?? companiesNamesInHS[companyMapped.Name?.ToLowerInvariant()].FirstOrDefault());

					if (hsCompanyID == null)
						continue;

					var wasCompanyOnborded = await OmnaeDbConnection.QuerySingleAsync<bool>(@"SELECT [WasOnboarded] FROM [dbo].[Companies] WHERE [Id] = @id", new { Id = companyMapped.OmnaeCompanyID });

					var clientLifeCycleStage = wasCompanyOnborded ? "Invited Partner" : "Free Trial";

					var companyInHS = HubSpotClient.Company.GetById<Company>((long)hsCompanyID);

					if (companyInHS.OmnaeCompanyID == null)
					{
						companyInHS.OmnaeCompanyID = companyMapped.OmnaeCompanyID;
					}
					if (string.IsNullOrWhiteSpace(companyInHS.OmnaeClientLifeCycleStage) || companyInHS.OmnaeClientLifeCycleStage != clientLifeCycleStage)
					{
						companyInHS.OmnaeClientLifeCycleStage = clientLifeCycleStage;
					}

					var result = HubSpotClient.Company.Update<Company>(companyInHS);

					// Update contact
					if (newContacts != null && result.OmnaeCompanyID != null)
					{
						var contact = newContacts.Where(x => x.OmnaeCompanyID == result.OmnaeCompanyID.ToString()).FirstOrDefault();
						if (contact != null)
						{
							var currentContact = HubSpotClient.Contact.GetByEmail<Contact>(contact.Email);
							contact.Id = currentContact.Id;
							contact.AssociatedCompanyId = result.Id;
							HubSpotClient.Contact.Update<Contact>(contact);
						}
					}
				}
				else
				{
					var result = HubSpotClient.Company.Create(companyMapped);

					// Create a new contact associated with this company
					if (newContacts != null && result.OmnaeCompanyID != null)
					{
						var contact = newContacts.Where(x => x.OmnaeCompanyID == result.OmnaeCompanyID.ToString()).FirstOrDefault();
						if (contact != null)
						{
							contact.AssociatedCompanyId = result.Id;
							HubSpotClient.Contact.Create<Contact>(contact);
						}
					}
				}


			}
			Log.LogDebug("Ended the Upsert in HubSpot.");

			Log.LogInformation("Ended the CompanySyncService.UpsertCompaniesInHubspotAsync.");
		}



		private async Task<List<Company>> LoadAndMapToCompaniesFromOmnaeDB(IEnumerable<int> companiesIds)
		{
			var companies = await OmnaeDbConnection.QueryAsync<Company>(@"
WITH users as (
	SELECT
	  [Id]
      ,[UserType]
      ,[FirstName]
      ,[MiddleName]
      ,[LastName]
      ,[Email]
      ,[EmailConfirmed]
      ,[PasswordHash]
      ,[SecurityStamp]
      ,[PhoneNumber]
      ,[PhoneNumberConfirmed]
      ,[TwoFactorEnabled]
      ,[LockoutEndDateUtc]
      ,[LockoutEnabled]
      ,[AccessFailedCount]
      ,[UserName]
      ,[CompanyId]
      ,[Active]
  FROM [dbo].[AspNetUsers] users
	--INNER JOIN [dbo].[AspNetUserRoles] roles on roles.UserId = users.Id and roles.RoleId = 'CompanyAdmin'
  WHERE [Active] = 1 
), totalInVoices as (
	SELECT [CompanyId],SUM([Quantity]*[UnitPrice]) as [Total]
	 FROM [dbo].[OmnaeInvoices]
	WHERE [IsOpen] = 0 AND [UnitPrice] > 0
	GROUP BY [CompanyId]
)
SELECT
    isnull(c.[Name],'') as [Name]
	,c.[Id] as [OmnaeCompanyID]
	,(isnull(
		(case when CHARINDEX('@', [AccountingEmail]) > 0 then
			ltrim(right([AccountingEmail], LEN([AccountingEmail]) - CHARINDEX('@', [AccountingEmail])))
		else
			[AccountingEmail]
		end
		),(
		case when CHARINDEX('@', u.[Email]) > 0 then
			ltrim(right(u.[Email], LEN(u.[Email]) - CHARINDEX('@', u.[Email])))
		else
			isnull(u.[Email],'')
		end)
	)) as [CompanyDomain]
	,isnull(u.[PhoneNumber],'') as [PhoneNumber]
	,isnull(addr.[AddressLine1],'') as [StreetAddress1]
	,isnull(addr.[AddressLine2],'') as [StreetAddress2]
	,isnull(addr.City,'') as [City]
	,isnull(addr.StateAbbr,'') as [StateRegion]
	,isnull(addr.CountryName,'') as [Country]
	,isnull(addr.ZipPostalCode,'') as [PostalCode]
	,isnull(u.Email,'') as [CompanyOwner]
	,iif(c.[CompanyType]=1,'Customer',iif(c.[CompanyType]=2,'Vendor', '')) as [CustomerType]
	,iif(c.[isEnterprise]=1,'SaaS','Reseller') as [BusinessModel]
	,ISNULL(u.[FirstName],'') +' '+ ISNULL(u.[MiddleName],'') +' '+ ISNULL(u.[LastName],'') as [PointOfContact]
	,isnull(u.[Email],'') as [EmailOfContact]
	,iif(c.[WasOnboarded] = 1, 'Invited Partner', 'Free Trial') as [OmnaeClientLifeCycleStage]
  FROM (
			SELECT
				c.[Id]
				,c.[Name] as Name
				,[AccountingEmail]
				,[CompanyLogoUri]
				,[CompanyType]
				,isnull(c.[MainCompanyAddress_Id], isnull(c.[AddressId], isnull(c.[BillAddressId], s.[AddressId]))) as AddressId
				,[isEnterprise]
				---OLd Term/Credit with Omnae
				,[Term] 
				,[CreditLimit]
				,[WasOnboarded]
			FROM [dbo].[Companies] c
				left join [dbo].[Shippings] s on s.Id = c.[ShippingId] and s.CompanyId = c.Id
	) c
	left join (
			SELECT 
				addr.[Id]
				,(isnull([AddressLine1], '') + isnull(' - '+[AddressLine2], '')) as [FullAddress]
				,[AddressLine1]
				,[AddressLine2]
				,[City]
				,[CountryId]
				,country.[CountryName] 
				,[StateProvinceId]
				,states.[Name] as StateName
				,states.[Abbreviation] as StateAbbr
				,isnull([ZipCode],[PostalCode]) as ZipPostalCode
			FROM [dbo].[Addresses] addr
			left join [dbo].[Countries] country on country.[Id] = addr.[CountryId]
			left join [dbo].[StateProvinces] states on states.[Id] = addr.[StateProvinceId]
		) addr on addr.Id = c.[AddressId]
	LEFT JOIN users u on u.CompanyId = c.Id
	LEFT JOIN totalInVoices i on i.CompanyId = c.Id
  WHERE 1=1
	AND c.[Name] is not null
    AND c.[Id] IN @Ids 
    AND c.[isEnterprise] = 1
ORDER BY c.[Id] ASC", new { Ids = companiesIds });

			return companies.ToList();
		}

        public async Task<ISet<int>> ListAllNonEnterpriseCompanies()
        {
            var nonEnterprise = await OmnaeDbConnection.QueryAsync<int>(@"SELECT [Id] FROM [dbo].[Companies] WHERE [isEnterprise] = 0");
            return new HashSet<int>(nonEnterprise);
        }
    }
}
