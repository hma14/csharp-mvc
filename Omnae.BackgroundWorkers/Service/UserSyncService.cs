using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Auth0.Core;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Auth0.ManagementApi.Paging;
using CsvHelper;
using Dapper;
using Dapper.Contrib.Extensions;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentEmail.Mailgun;
using HubSpot.NET.Api.Contact.Dto;
using HubSpot.NET.Core;
using Omnae.BackgroundWorkers.Auth0;
using Serilog;
using Microsoft.Azure;
using Omnae.BackgroundWorkers.Model;
using Omnae.Common.Extensions;
using Omnae.Hubspot;
using Omnae.Hubspot.Model;
using Omnae.Hubspot.Util;
using Omnae.Model.Models;
using Serilog.Context;
using Company = Omnae.Model.Models.Company;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;

namespace Omnae.BackgroundWorkers.Service
{
    class UserSyncService : IDisposable
    {
        private ILogger Log { get; }

        private ManagementApiClient Auth0Api { get; }

        private SqlConnection OmnaeDbConnection { get; }

        private HubSpotApi HubSpotClient { get; }

        private CompanySyncService CompanySyncService { get; }

        private readonly HttpClient hubspotHttpClient = new HttpClient( new HttpClientHandler { UseDefaultCredentials = true });

        public UserSyncService(ILogger log)
        {
            Log = log;

            CompanySyncService = new CompanySyncService(log.ForContext<CompanySyncService>());

            //AuthZero
            var auth0_domain = CloudConfigurationManager.GetSetting("auth0_domain");
            Auth0Token.RefreshToken();
            Auth0Api = new ManagementApiClient(Auth0Token.Token, new Uri(auth0_domain));

            //Email
            Email.DefaultSender = new MailgunSender(
                CloudConfigurationManager.GetSetting("Mailgun.Domain"), // Mailgun Domain
                CloudConfigurationManager.GetSetting("Mailgun.APIKEY") // Mailgun API Key
            );

            //Hubspot
            var hubSpotApikey = CloudConfigurationManager.GetSetting("hubspot_APIKEY");
            HubSpotClient = new HubSpotApi(hubSpotApikey);

            //Db Conn
            OmnaeDbConnection = new SqlConnection(CloudConfigurationManager.GetSetting("OmnaeDbContext"));

            //Configure API
            var baseUrl = CloudConfigurationManager.GetSetting("hubspot_baseurl");
            hubspotHttpClient.BaseAddress = new Uri(baseUrl);
            hubspotHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        protected readonly DateTime BeginOfTheSystemDate = new DateTime(2019, 11, 10);//new DateTime(2019, 01, 01);

        public async Task<bool> SyncAsync()
        {
            var lastCheck = await OmnaeDbConnection.QuerySingleOrDefaultAsync<HubspotIntegrationSyncControl>(@"SELECT TOP 1 * FROM [dbo].[HubspotIntegrationSyncControls] ORDER BY 1 DESC")
                                ?? new HubspotIntegrationSyncControl(BeginOfTheSystemDate);

            lastCheck.LastCheckForNewAuthZeroUsers = lastCheck.LastCheckForNewAuthZeroUsers?.AddSeconds(1);

            Log.Information("Last HubspotIntegrationSyncControl info: {@Data}", lastCheck);

            var endDate = DateTime.Now.AddDays(1);

            //Log.Information("The last user date created in Omnae Database is {0} and last updated was at {1}", lastUserCreatedInOmnaeDB, lastUserUpdatedInOmnaeDB);

            var query = $"created_at:{{ {lastCheck.LastCheckForNewAuthZeroUsers:O} TO {endDate:O} }}";
            Log.Information("query: {query}", query);
            var allNewUsersInAuth0 = await Auth0Api.Users.GetAllAsync(new GetUsersRequest { SearchEngine = "v3", Query = query }, new PaginationInfo());

            if (!allNewUsersInAuth0.Any())
            {
                Log.Information("No new user was found in auth0 created after {0}.", lastCheck.LastCheckForNewAuthZeroUsers);
                //Log.Information("No new user was found in auth0 created after {0} or updated after {1}", lastCheck.LastCheckForNewAuthZeroUsers, lastCheck.LastCheckForNewAuthZeroUsers);
                return false;
            }

            var nonEnterpriseCompanies = await CompanySyncService.ListAllNonEnterpriseCompanies();
            var mappedUsers = allNewUsersInAuth0.Select(Map).Where(au => au.CompanyId == null || !nonEnterpriseCompanies.Contains(au.CompanyId.Value)).ToList();




            /////////////////////////////////////////

            //Email
            await SendEmailWithFilesToImport(mappedUsers, lastCheck, endDate);

            //HS
            await SendNewDataToHubSpot(mappedUsers);
         

            /////////////////////////////////////////
            // Create Parent/Child relationships
            await SetHubspotParentChildRelations(mappedUsers);

            /////////////////////////////////////////
            //Save This new users to Omnae Database
            await SendNewUserToCoreApi(mappedUsers);

            /////////////////////////////////////////
            //Update local data.
            await UpdateLastStateInTheDatabase(allNewUsersInAuth0, lastCheck);

            return true;
        }

        private async Task SendNewUserToCoreApi(List<OAuthUserData> mappedUsers)
        {
            var allUsers = await OmnaeDbConnection.QueryAsync<SimplifiedUser>(@"SELECT * FROM [dbo].[AspNetUsers]");
            foreach (var user in mappedUsers.Select(Map))
            {
                //var users = await OmnaeDbConnection.QueryAsync<SimplifiedUser>(@"SELECT * FROM [dbo].[AspNetUsers] WHERE Companyid = @CompanyId", new { user.CompanyId });
                if (user.CompanyId == null)
                {
                    Log.Error("User companyId is null for UserName: {0}.", user.UserName);
                    continue; 
                }
                var existingUsers = allUsers.Where(x => x.UserName == user.UserName);
                if (existingUsers.Count() > 0) continue;

                var users = allUsers.Where(x => x.CompanyId == user.CompanyId);
                int isPrimaryContact = 0;
                if (users.Count() == 0)
                {
                    isPrimaryContact = 1;
                }
                
                int emailConfirmed = user.EmailConfirmed == true ? 1 : 0;
                int phoneNumberConfirmed = user.PhoneNumberConfirmed == true ? 1 : 0;
                int lockoutEnabled = user.LockoutEnabled == true ? 1 : 0;
                int active = user.Active == true ? 1 : 0;

                Log.Information("{@user}", user);
                DateTime now = DateTime.UtcNow;
                await OmnaeDbConnection.ExecuteAsync($"INSERT INTO [dbo].[AspNetUsers] VALUES('{user.Id}', 1, '{user.FirstName}', '{user.MiddleName}', '{user.LastName}', '{user.Email}', {emailConfirmed}, null, null, '{user.PhoneNumber}', {phoneNumberConfirmed}, 0, null, {lockoutEnabled}, 0, '{user.UserName}', {user.CompanyId},  {active}, '{now}', '{now}', {isPrimaryContact}, '{user.Role}', '{user.CustomerRole}', '{user.VendorRole}')");

            }
        }

        private static SimplifiedUser Map(OAuthUserData source)
        {
            return new SimplifiedUser
            {
                Id = source.oAuthUserId,
                Active = true,
                UserName = (source.UserName ?? source.Email),
                FirstName = source.FirstName.Replace("'", ""),
                MiddleName = source.MiddleName,
                LastName = source.LastName,
                UserType = USER_TYPE.Customer,
                CompanyId = source.CompanyId,
                Email = source.Email,
                EmailConfirmed = source.EmailConfirmed,
                PhoneNumber = source.PhoneNumber,
                PhoneNumberConfirmed = source.PhoneNumberConfirmed,
                LockoutEnabled = source.Blocked,
                IsPrimaryContact = source.IsPrimaryContact,
                Role = source.Role,
                CustomerRole = source.CustomerRole,
                VendorRole = source.VendorRole,
            };
        }

        private async Task SendNewDataToHubSpot(List<OAuthUserData> mappedUsers)
        {
            Log.Information("Start send the data to HubSpot.");

            var companiesIds = mappedUsers.Select(u => u.CompanyId).Where(cid => cid != null).Select(c => (int)c).Distinct().ToList();
            
            var newContacts = mappedUsers.Select(MapToContact);

            //  needs to investigate why isToUpdateLifeCycleCompanies is set to false
            await CompanySyncService.UpsertCompaniesInHubspotAsync(companiesIds, newContacts, false);


            // Add invitee contacts to its company in Hubspot
            if (newContacts != null )
            {
                var allCompaniesInHS = HubSpotClient.Company.ListAll<Omnae.Hubspot.Model.Company>(new List<string> { "omnae_company_id", "name", "domain" }).ToList();
                foreach (var contact in newContacts)
                {
                    if (contact == null || string.IsNullOrEmpty(contact.OmnaeCompanyID)) continue;

                    var hubspotCompany = allCompaniesInHS.Where(x => x.OmnaeCompanyID == int.Parse(contact.OmnaeCompanyID)).FirstOrDefault();
                    contact.AssociatedCompanyId = hubspotCompany.Id;
                    
                    HubSpotClient.Contact.CreateOrUpdate<Contact>(contact);
                }             
            }


            Log.Information("Ended send the data to HubSpot.");
        }

        private async Task SendEmailWithFilesToImport(List<OAuthUserData> mappedUsers, HubspotIntegrationSyncControl lastCheck, DateTime endDate)
        {
            //Generate Files
            var memCompanyFile = await GenerateCompanyFile(mappedUsers);
            var memUserFile = GenerateUserFile(mappedUsers);

            //Send Email file
            await SendEmail(mappedUsers, memCompanyFile, memUserFile, lastCheck.LastCheckForNewAuthZeroUsers ?? BeginOfTheSystemDate, endDate);
        }

        private static OAuthUserData Map(User oAuthUser)
        {
            var data = new OAuthUserData()
            {
                oAuthUserId = oAuthUser.UserId,
                LegacyId = oAuthUser.AppMetadata.legacyId?.ToString(),
                UserName = oAuthUser.Email, //oAuthUser.UserName,
                FirstName = oAuthUser.FirstName ?? oAuthUser.UserMetadata.name,
                LastName = oAuthUser.LastName,

                Email = oAuthUser.Email,
                EmailConfirmed = oAuthUser.EmailVerified ?? false,
                PhoneNumber = oAuthUser.PhoneNumber ?? oAuthUser.UserMetadata.phone,
                PhoneNumberConfirmed = oAuthUser.PhoneVerified ?? false,

                CompanyId = !string.IsNullOrEmpty((string)oAuthUser.AppMetadata.companyId)
                    ? (int)oAuthUser.AppMetadata.companyId
                    : (int?)null,

                UserType = oAuthUser.AppMetadata.userType?.ToString(),
                Role = oAuthUser.AppMetadata.role?.ToString(),
                CustomerRole = oAuthUser.AppMetadata.customerRole?.ToString(),
                VendorRole = oAuthUser.AppMetadata.vendorRole?.ToString(),
                StripeCustomerId = oAuthUser.AppMetadata.stripeCustomerId?.ToString(),

                Blocked = oAuthUser.Blocked ?? false,
                Active = bool.TryParse(oAuthUser.UserMetadata.active, out bool act) ? act : (bool?)null,
            };
            return data;
        }

        private async Task UpdateLastStateInTheDatabase(IPagedList<User> allNewUsersInAuth0, HubspotIntegrationSyncControl lastCheck)
        {
            var currentCheckForNewAuthZeroUsers =
                allNewUsersInAuth0.OrderByDescending(u => u.CreatedAt).FirstOrDefault()?.CreatedAt ??
                lastCheck.LastCheckForNewAuthZeroUsers;

            var qnt = await OmnaeDbConnection.ExecuteAsync(@"
DELETE FROM [dbo].[HubspotIntegrationSyncControls];

INSERT INTO [dbo].[HubspotIntegrationSyncControls] ([LastCheckForNewAuthZeroUsers])
     VALUES (@LastCheckForNewAuthZeroUsers)
", new { LastCheckForNewAuthZeroUsers = currentCheckForNewAuthZeroUsers });
        }

        private async Task<string> GenerateCompanyFile(List<OAuthUserData> mapedUsers)
        {
            var companiesIds = mapedUsers.Select(u => u.CompanyId)
                                         .Where(cid => cid != null)
                                         .Distinct()
                                         .ToList();

            var companies = await OmnaeDbConnection.QueryAsync<HubspotCompanyCsvModel>(@"
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
	INNER JOIN [dbo].[AspNetUserRoles] roles on roles.UserId = users.Id and roles.RoleId = 'CompanyAdmin'
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
ORDER BY c.[Id] ASC", new { Ids = companiesIds });

            using (var stWriter = new StringWriter())
            using (var csv = new CsvWriter(stWriter, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(companies);
                return stWriter.ToString();
            }
        }

        private string GenerateUserFile(List<OAuthUserData> mapedUsers)
        {
            var mappedTOCSVsv = mapedUsers.Select(MapToUserCvs).ToList();

            using (var stWriter = new StringWriter())
            using (var csv = new CsvWriter(stWriter, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(mappedTOCSVsv);
                return stWriter.ToString();
            }
        }

        private HubspotCustomerCsvModel MapToUserCvs(OAuthUserData oAuthUser)
        {
            var company = oAuthUser.CompanyId == null ? null : OmnaeDbConnection.QuerySingleOrDefault<Company>(@"SELECT * FROM [dbo].[Companies] WHERE [ID]=@CompanyId ORDER BY 1 DESC", new { CompanyId = oAuthUser.CompanyId });

            var data = new HubspotCustomerCsvModel()
            {
                FirstName = $"{oAuthUser.FirstName} {oAuthUser.MiddleName}".TrimAll(),
                LastName = oAuthUser.LastName,
                EmailAddress = oAuthUser.Email,
                PhoneNumber = oAuthUser.PhoneNumber,

                OmnaeCompanyID = oAuthUser.CompanyId?.ToString(),
                CompanyName = company?.Name,

                OmnaeUserId = oAuthUser.oAuthUserId,

            };
            return data;
        }

        private Contact MapToContact(OAuthUserData oAuthUser)
        {
            var company = oAuthUser.CompanyId == null ? null : OmnaeDbConnection.QuerySingleOrDefault<Company>(@"SELECT * FROM [dbo].[Companies] WHERE [ID]=@CompanyId ORDER BY 1 DESC", new { CompanyId = oAuthUser.CompanyId });

            var data = new Contact()
            {
                FirstName = $"{oAuthUser.FirstName} {oAuthUser.MiddleName}".TrimAll(),
                LastName = oAuthUser.LastName,
                Email = oAuthUser.Email,
                Phone = oAuthUser.PhoneNumber,

                OmnaeCompanyID = oAuthUser.CompanyId?.ToString(),
                Company = company?.Name,

                OmnaeUserId = oAuthUser.oAuthUserId,
            };
            return data;
        }

        private async Task<List<Hubspot.Model.Company>> MapToCompany(List<OAuthUserData> mapedUsers)
        {
            var companiesIds = mapedUsers.Select(u => u.CompanyId)
                             .Where(cid => cid != null)
                             .Distinct()
                             .ToList();

            var companies = await OmnaeDbConnection.QueryAsync<Hubspot.Model.Company>(@"
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
	INNER JOIN [dbo].[AspNetUserRoles] roles on roles.UserId = users.Id and roles.RoleId = 'CompanyAdmin'
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
ORDER BY c.[Id] ASC", new { Ids = companiesIds });

            return companies.ToList();
        }

        public async Task SendEmail(List<OAuthUserData> mapedUsers, string companyFile, string userFile, DateTime fromDate, DateTime toDate)
        {
            var subject = $"Hubspot integration - Recent New Companies and User to Import - From date {fromDate:s} to {toDate:s}";

            var email = Email
                .From("info@omnae.com", "Omnae Backend Services")
                .ReplyTo("hma@omnae.com", "Omnae Backend Services - Support")
                .Subject(subject)
                .Body(@"This is a automatic email that contains files to import.");

            var toEmails = CloudConfigurationManager.GetSetting("SendToEmails").Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(e => e.Trim()).ToList();
            foreach (var toMail in toEmails)
            {
                email = email.To(toMail);
            }
            if (companyFile != null)
            {
                var fileData = new MemoryStream(Encoding.UTF8.GetBytes(companyFile));
                email = email.Attach(new Attachment() { Filename = $"To Import in Auth0 - New Companies - From date {fromDate:s} to {toDate:s}.csv", Data = fileData });
            }
            if (userFile != null)
            {
                var fileData = new MemoryStream(Encoding.UTF8.GetBytes(userFile));
                email = email.Attach(new Attachment() { Filename = $"To Import in Auth0 - New User - From date {fromDate:s} to {toDate:s}.csv", Data = fileData });
            }

            var response = await email.SendAsync();
            Log.Information("Email with integrations files send. Subject:{Subject}, Status:{Status}", subject, response.Successful);
        }

        private async Task SetHubspotParentChildRelations(List<OAuthUserData> oAuthUsers)
        {
            Log.Information("Number of oAuthUsers: {count}", oAuthUsers.Count);
            var apiKey = CloudConfigurationManager.GetSetting("hubspot_APIKEY"); 
            var allCompaniesInHS = HubSpotClient.Company.ListAll<Hubspot.Model.Company>(new List<string> { "omnae_company_id", "name", "domain" }).ToList();
            foreach (var oAuthUser in oAuthUsers)
            {
                var company = oAuthUser.CompanyId == null ? null : OmnaeDbConnection.QuerySingleOrDefault<Company>(@"SELECT * FROM [dbo].[Companies] WHERE [ID]=@CompanyId ORDER BY 1 DESC", new { CompanyId = oAuthUser.CompanyId });
                
                if (company == null || company.IsActive == false || company.InvitedByCompanyId == null && company.OnboardedByCompanyId == null) continue;
                var parentId = allCompaniesInHS
                    .Where(x => x.OmnaeCompanyID != null && (x.OmnaeCompanyID == company.InvitedByCompanyId || x.OmnaeCompanyID == company.OnboardedByCompanyId))
                    .Select(x => x.Id)
                    .FirstOrDefault();
                if (parentId == null) continue;

                var childId = allCompaniesInHS
                    .Where(x => x.OmnaeCompanyID != null && x.OmnaeCompanyID == company.Id)
                    .Select(x => x.Id).FirstOrDefault();
                if (childId == null) continue;
#if false
                var data = new
                {
                    fromObjectId = childId,
                    toObjectId = parentId,
                    category = "HUBSPOT_DEFINED",
                    definitionId = 14, // Child company to Parent company
                };
#else
                var data = new
                {
                    fromObjectId = parentId,
                    toObjectId = childId,
                    category = "HUBSPOT_DEFINED",
                    definitionId = 13, // Parent company to child company
                };
#endif


                var result = await hubspotHttpClient.PutAsJsonAsync($"/crm-associations/v1/associations?hapikey={apiKey}", data);
                if (!result.IsSuccessStatusCode)
                {
                    var body = await result.Content.ReadAsStringAsync();

                    if (result.StatusCode == HttpStatusCode.Conflict || result.StatusCode == HttpStatusCode.BadRequest)
                    {
                        Log.Warning("The new Front-End API send a BadRequest or a Conflict. {StatusCode} - {Body}", result.StatusCode, body);
                        continue;
                    }
                    Log.Error("The new Front-End API returned a error when creating Parent - Child relationship. {StatusCode} - {Body}", result.StatusCode, body);
                    result.EnsureSuccessStatusCode();
                    continue;
                }
                else
                {
                    Log.Information("Is Success? {result}",  result.IsSuccessStatusCode);
                }
            }
        }

        public void Dispose()
        {
            Auth0Api?.Dispose();
            OmnaeDbConnection?.Dispose();
        }
    }
}
